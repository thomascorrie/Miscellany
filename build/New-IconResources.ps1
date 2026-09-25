<#
.SYNOPSIS
    Builds the node icon resources for Miscellany.customization.dll from the PNG files.

.DESCRIPTION
    Dynamo reads node icons from a "<Assembly>Images.resources" resource in "<Assembly>.customization.dll",
    but the two generations of Dynamo expect different resource types:
      - Dynamo 2.x (.NET Framework) casts each icon to System.Drawing.Bitmap
      - Dynamo 3.x and later (.NET 8+) reads each icon as a byte[] of PNG data
    so this script writes either format from the same PNGs.

    It must run in Windows PowerShell 5.1 (powershell.exe), which runs on .NET Framework: Bitmap resources
    are written with .NET Framework serialization so that Dynamo 2.x can read them.

.PARAMETER Format
    Bitmap for Dynamo 2.x, Bytes for Dynamo 3.x and later.

.PARAMETER ImagesDir
    Folder containing the Large and Small icon folders. Each PNG's file name (without .png) is the resource name.

.PARAMETER OutFile
    The .resources file to write.
#>
param(
    [Parameter(Mandatory = $true)][ValidateSet('Bitmap', 'Bytes')][string]$Format,
    [Parameter(Mandatory = $true)][string]$ImagesDir,
    [Parameter(Mandatory = $true)][string]$OutFile
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$pngs = @(Get-ChildItem -Path (Join-Path $ImagesDir 'Large'), (Join-Path $ImagesDir 'Small') -Filter '*.png' | Sort-Object Name)
if ($pngs.Count -eq 0) {
    throw "No icon PNGs found under $ImagesDir"
}

$outDir = Split-Path -Parent $OutFile
if ($outDir -and -not (Test-Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir | Out-Null
}

$writer = New-Object System.Resources.ResourceWriter($OutFile)
try {
    foreach ($png in $pngs) {
        $name = [System.IO.Path]::GetFileNameWithoutExtension($png.Name)
        if ($Format -eq 'Bytes') {
            $writer.AddResource($name, [System.IO.File]::ReadAllBytes($png.FullName))
        }
        else {
            # Copy into a new Bitmap so the PNG file is not kept open
            $image = [System.Drawing.Image]::FromFile($png.FullName)
            try {
                $writer.AddResource($name, (New-Object System.Drawing.Bitmap($image)))
            }
            finally {
                $image.Dispose()
            }
        }
    }
    $writer.Generate()
}
finally {
    $writer.Close()
}

Write-Host "Wrote $($pngs.Count) $Format icons to $OutFile"
