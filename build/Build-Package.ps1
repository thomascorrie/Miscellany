<#
.SYNOPSIS
    Assembles the Dynamo package folders from a built solution.

.DESCRIPTION
    Run after "dotnet build -c <Configuration>". Produces one package per Dynamo generation:
      dist\Dynamo2\Miscellany   net48 build for Dynamo 2.x (Revit 2020-2024)
      dist\Dynamo3\Miscellany   net8.0 build for Dynamo 3.x and later (Revit 2025+)
    each with the standard Dynamo package layout (pkg.json, bin, extra) plus a zip of the folder.

    To try a package locally, copy its Miscellany folder into Dynamo's packages folder, e.g.
    %AppData%\Dynamo\Dynamo Revit\<version>\packages\Miscellany

.PARAMETER Configuration
    The build configuration to package (default Release).

.PARAMETER OutputDir
    Where to write the packages (default dist in the repository root).
#>
param(
    [string]$Configuration = 'Release',
    [string]$OutputDir
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
if (-not $OutputDir) { $OutputDir = Join-Path $root 'dist' }

$variants = @(
    @{ Name = 'Dynamo2'; Framework = 'net48'; EngineVersion = '2.1.0' },
    @{ Name = 'Dynamo3'; Framework = 'net8.0'; EngineVersion = '3.0.4' }
)

$template = Get-Content -Raw -Path (Join-Path $root 'Miscellany\pkg.json') | ConvertFrom-Json

foreach ($variant in $variants) {
    $libraryBin = Join-Path $root "Miscellany\bin\$Configuration\$($variant.Framework)"
    $customizationBin = Join-Path $root "Miscellany.Customization\bin\$Configuration\$($variant.Framework)"

    $binFiles = @(
        (Join-Path $libraryBin 'Miscellany.dll'),
        (Join-Path $libraryBin 'Miscellany.xml'),
        (Join-Path $libraryBin 'Miscellany_DynamoCustomization.xml'),
        (Join-Path $libraryBin 'Miscellany.Migrations.xml'),
        (Join-Path $libraryBin 'CromulentBisgetti.ContainerPacking.dll'),
        (Join-Path $customizationBin 'Miscellany.customization.dll')
    )
    foreach ($file in $binFiles) {
        if (-not (Test-Path $file)) { throw "Missing build output $file. Run 'dotnet build -c $Configuration' first." }
    }

    $packageRoot = Join-Path $OutputDir "$($variant.Name)\Miscellany"
    if (Test-Path $packageRoot) { Remove-Item -Recurse -Force $packageRoot }
    $bin = New-Item -ItemType Directory -Path (Join-Path $packageRoot 'bin')
    $extra = New-Item -ItemType Directory -Path (Join-Path $packageRoot 'extra')

    Copy-Item -Path $binFiles -Destination $bin
    Copy-Item -Path (Join-Path $root 'Samples\*.dyn') -Destination $extra
    Copy-Item -Path (Join-Path $root 'LICENSE'), (Join-Path $root 'THIRD-PARTY-NOTICES.md') -Destination $packageRoot

    # pkg.json: static metadata from Miscellany\pkg.json, version and assembly names from the build
    $libraryName = [System.Reflection.AssemblyName]::GetAssemblyName((Join-Path $bin 'Miscellany.dll'))
    $customizationName = [System.Reflection.AssemblyName]::GetAssemblyName((Join-Path $bin 'Miscellany.customization.dll'))
    $version = $libraryName.Version
    $pkg = $template | Select-Object *
    $pkg.version = "$($version.Major).$($version.Minor).$($version.Build)"
    $pkg.engine_version = $variant.EngineVersion
    $pkg.keywords = @($template.keywords | Where-Object { $_ })
    $pkg.node_libraries = @($libraryName.FullName, $customizationName.FullName)
    $pkg | ConvertTo-Json -Depth 5 | Set-Content -Path (Join-Path $packageRoot 'pkg.json') -Encoding UTF8

    $zip = Join-Path $OutputDir "Miscellany-$($pkg.version)-$($variant.Name).zip"
    if (Test-Path $zip) { Remove-Item -Force $zip }
    Compress-Archive -Path $packageRoot -DestinationPath $zip

    Write-Host "$($variant.Name): $packageRoot (Miscellany $($pkg.version), engine $($variant.EngineVersion))"
}
