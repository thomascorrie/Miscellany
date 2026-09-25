# Contributing

Contributions are welcome, whether that's a bug report, an idea in [Issues](https://github.com/thomascorrie/Miscellany/issues) or a pull request.

## Submitting a change

1. Fork this repository.
2. Create a branch from `main`, e.g. `fix/my-fix` or `feature/my-feature`.
3. Make your changes, adding or updating tests in `Miscellany.Tests` and the documentation in `docs/` where relevant.
4. Push your branch and open a pull request against `main`. The pull request is built and tested automatically.

## Building

You need Windows and the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0). The node icons are generated with Windows PowerShell, so the build only runs on Windows.

```
dotnet build -c Release
dotnet test -c Release
```

The solution builds two versions of the package:

- `net48` for Dynamo 2.x (Revit 2020–2024)
- `net8.0` for Dynamo 3.x and later (Revit 2025+)

| Project | Purpose |
|---|---|
| `Miscellany` | The node library |
| `Miscellany.Customization` | `Miscellany.customization.dll`, which holds the node icons. It's generated from the PNGs in `Miscellany/Resources/Images` by `build/New-IconResources.ps1`, because Dynamo 2.x reads icons as `Bitmap` objects and Dynamo 3+ reads them as PNG bytes |
| `Miscellany.Tests` | xUnit tests, run against both frameworks and the real 3DContainerPacking library |

## Trying a build in Dynamo

After building, assemble the Dynamo packages:

```
powershell -File build/Build-Package.ps1
```

This writes `dist/Dynamo2/Miscellany` and `dist/Dynamo3/Miscellany` (plus a zip of each). Copy the folder that matches your Dynamo version into Dynamo's packages folder, e.g. `%AppData%\Dynamo\Dynamo Revit\<version>\packages\Miscellany`, then restart Dynamo.

## Adding a node icon

Add a 128×128 PNG to `Miscellany/Resources/Images/Large` and a 32×32 PNG to `Miscellany/Resources/Images/Small`, named after the node's full name, e.g. `Miscellany.Maths.RunningTotal.Large.png` and `Miscellany.Maths.RunningTotal.Small.png`. They are picked up automatically on the next build.
