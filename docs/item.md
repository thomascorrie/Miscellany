[Documentation](README.md) › [Container Packing](container-packing.md) › Item

# Item

An Item is something to be packed into a Container. After packing, it also records where and how it was packed.

Code: [Item.cs](../Miscellany/ContainerPacking/Entities/Item.cs)

## Create

| Item.Item |
|---|
| ![Item](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.Item.Large.png) |

Creates a new Item from an `id` (int), three dimensions `dim1`, `dim2` and `dim3` (doubles) and a `quantity` (int, default 1). The dimensions can be in any order, because the packing algorithm tries every orientation.

## Query

| Item.ID | Item.Quantity | Item.Volume |
|---|---|---|
| ![ID](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.ID.Large.png) | ![Quantity](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.Quantity.Large.png) | ![Volume](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.Volume.Large.png) |

Gets the Item's ID, quantity or volume. The volume is for a single unit. Items returned by the packing nodes have a quantity of 1 (see [How packing works](container-packing.md#how-packing-works)).

| Item.Dim1 | Item.Dim2 | Item.Dim3 |
|---|---|---|
| ![Dim1](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.Dim1.Large.png) | ![Dim2](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.Dim2.Large.png) | ![Dim3](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.Dim3.Large.png) |

Gets one of the three dimensions the Item was created with.

## Packing results

These values are set on Items returned by the [PackingService](container-packing.md#packingservice) nodes.

| Item.IsPacked | Item.Sequence |
|---|---|
| ![IsPacked](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.IsPacked.Large.png) | ![Sequence](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.Sequence.Large.png) |

`IsPacked` says whether the Item was packed. `Sequence` gives the order in which it was packed, counting from 1 across all Containers.

| Item.CoordX | Item.CoordY | Item.CoordZ |
|---|---|---|
| ![CoordX](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.CoordX.Large.png) | ![CoordY](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.CoordY.Large.png) | ![CoordZ](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.CoordZ.Large.png) |

Gets the x, y and z coordinates of the packed Item's position within its Container (Z up).

| Item.PackDimX | Item.PackDimY | Item.PackDimZ |
|---|---|---|
| ![PackDimX](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.PackDimX.Large.png) | ![PackDimY](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.PackDimY.Large.png) | ![PackDimZ](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.Entities.Item.PackDimZ.Large.png) |

Gets the Item's x, y and z dimensions in the orientation it was packed. Together with the coordinates, this gives the box the Item occupies, e.g. for drawing it with `Cuboid.ByCorners`.
