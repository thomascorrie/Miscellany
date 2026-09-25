[Documentation](README.md) › Releases

# Releases

## Unreleased

Changes on `main` since 1.2.0 that will go into the next release.

**Renamed nodes.** Some classes were renamed so they no longer clash with Dynamo's core nodes when used in DesignScript ([#14](https://github.com/thomascorrie/Miscellany/issues/14)). Graphs that use the old names are upgraded automatically when opened.

| Old name | New name |
|---|---|
| `Geometry.Abstract.CoordinateSystem.Display` | `Geometry.Abstract.CoordinateSystemDisplay` |
| `Geometry.Abstract.Plane.Display` | `Geometry.Abstract.PlaneDisplay` |
| `Geometry.Abstract.Vector.Display` | `Geometry.Abstract.VectorDisplay` |
| `Math.Functions.RunningTotal` | `Maths.RunningTotal` |
| `Math.Functions.ToDecimal` | `Maths.ToDecimal` |
| `Math.Functions.ToDouble` | `Maths.ToDouble` |
| `List.PairItems` | `List.Modifies.PairItems` |

**Container Packing fixes** ([#19](https://github.com/thomascorrie/Miscellany/pull/19), [#20](https://github.com/thomascorrie/Miscellany/pull/20))
- PackContainersWithGroupsContinuously no longer loses Items that don't fit after the last group has been used. They are carried on to later Containers, or reported in `unpackedItems`.
- PackContainersWithGroupsContinuously no longer stalls for about 1.7 seconds per Container when more than 10 passes are needed to reach `minimumItems`.
- PackContainersWithGroups no longer fails on empty groups.
- In both group nodes, `isCompletePack` is only `true` when every Item has been packed.
- Items with a quantity greater than 1 are no longer duplicated when leftovers are re-packed into the next Container ([#16](https://github.com/thomascorrie/Miscellany/issues/16)). **Behaviour change:** every Item returned by the packing nodes is now a single unit with a quantity of 1.

**Platform**
- Built in two versions: **Dynamo2** (.NET Framework 4.8, Dynamo 2.1–2.19, Revit 2020–2024) and **Dynamo3** (.NET 8, Dynamo 3.0 and later, Revit 2025+).
- Node icons now appear in Dynamo 3 and later. Dynamo 3 reads icons in a different format from Dynamo 2, so each version now has its own icon file.
- 3DContainerPacking now comes from its official NuGet package (1.0.2). This includes a 2021 fix to the EB-AFIT algorithm, so some packing results may differ slightly from 1.2.0.
- PackContainer and PackContainers no longer fail when given an empty list of Items.

**New**
- `Item.Sequence`: the order in which each Item was packed.

**Removed**
- The undocumented `orientation` output from PackContainer (it only repeated `percentItemVolumePacked`).

**Other**
- Licence changed to [MIT](../LICENSE) (previously LGPL-3.0 in the repository, AGPL-3.0 on the package listing).
- Documentation moved from the GitHub wiki into this `docs` folder.

## 1.2.0

PackContainersWithGroupsContinuously now looks to the following groups to make up the required minimum number of Items.

Tag: [v1.2.0](https://github.com/thomascorrie/Miscellany/releases/tag/v1.2.0)

## 1.1.7

Fixes [#1 Container Orientation](https://github.com/thomascorrie/Miscellany/issues/1).

Tag: [v1.1.7](https://github.com/thomascorrie/Miscellany/releases/tag/v1.1.7)

## 1.0.0

The first release of Miscellany on the Dynamo package manager.

Tag: [v1.0.0](https://github.com/thomascorrie/Miscellany/releases/tag/v1.0.0)
