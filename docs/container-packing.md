[Documentation](README.md) › Container Packing

# Container Packing

Miscellany includes an implementation of the C# library [3DContainerPacking](https://github.com/davidmchapman/3DContainerPacking), which runs the [EB-AFIT container packing algorithm](https://github.com/wknechtel/3d-bin-pack) in Dynamo.

The nodes fall into two groups:

- **Entities**: nodes for creating [Items](item.md) (the things to be packed) and [Containers](container.md) (the things to pack into).
- **PackingService**: nodes that do the packing.

Sample file: [Miscellany-Samples-PackContainer.dyn](../Samples/Miscellany-Samples-PackContainer.dyn)

## How packing works

- Each **Item** has three dimensions and a quantity. The algorithm may rotate an Item into any orientation to fit it in. The orientation it chose is given by `PackDimX`, `PackDimY` and `PackDimZ`, and the Item's position within the Container by `CoordX`, `CoordY` and `CoordZ`.
- Coordinates and packed dimensions use Dynamo's axes, with Z up. (3DContainerPacking uses Y up internally, and Miscellany swaps Y and Z for you.)
- An Item with a `quantity` greater than 1 is packed as that many separate units. Each packed or unpacked Item in the outputs is **one unit** with a quantity of 1. The exception is an input Item that was never reached, e.g. because the Containers ran out: it keeps its original quantity in `unpackedItems`.
- Each packed Item gets a **`Sequence`** number giving the order it was packed in, counting from 1 across all Containers.
- The multi-container nodes are **greedy**. They pack each Container as well as possible in turn, so the overall result is good but not guaranteed to be optimal.
- `algorithm` selects the packing algorithm. `1` is EB-AFIT, the only algorithm 3DContainerPacking currently provides, and is the default.

## PackingService

### PackingService.PackContainer

![PackContainer](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.PackingService.PackContainer.Large.png)

Packs a list of Items into a **single** Container.

| Input | Description |
|---|---|
| `container` | The Container to pack into |
| `itemsToPack` | The Items to pack |
| `algorithm` | Algorithm ID (default `1`, EB-AFIT) |

| Output | Description |
|---|---|
| `packedItems` | Items that were packed |
| `unpackedItems` | Items that did not fit |
| `isCompletePack` | `true` if every Item was packed |
| `packTimeInMilliseconds` | Time taken to pack |
| `percentContainerVolumePacked` | Percentage of the Container's volume that is filled |
| `percentItemVolumePacked` | Percentage of the Items' total volume that was packed |

### PackingService.PackContainers

![PackContainers](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.PackingService.PackContainers.Large.png)

Packs a list of Items into a **list of Containers**, in order. Each Container is filled as fully as possible, and whatever doesn't fit moves on to the next Container. Packing stops when every Item is packed or the Containers run out.

Inputs are as for PackContainer, but with `containers` (a list, in packing order) in place of `container`.

| Output | Description |
|---|---|
| `packedItems` | A list of packed Items for each Container used |
| `unpackedItems` | Items left over after the last Container |
| `isCompletePack` | `true` if every Item was packed |
| `packTimeInMilliseconds` | Pack time for each Container |
| `totalPackTimeInMilliseconds` | Total pack time |
| `percentContainerVolumePacked` | Percentage filled, for each Container |
| `percentItemVolumePacked` | Percentage of the offered Items' volume packed, for each Container |

### PackingService.PackContainersWithGroups

![PackContainersWithGroups](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.PackingService.PackContainersWithGroups.Large.png)

Like PackContainers, but the Items come in **groups** (a list of lists) and groups are never mixed. Each Container is packed only from the current group. When a group runs out, packing moves to the **next Container** and starts on the next group, so every group begins in a fresh Container. Empty groups are skipped.

Useful when groups must be kept apart, e.g. one group per room or per delivery.

Inputs: `containers`, `itemsToPack` (a list of groups of Items), `algorithm`. The outputs are the same as PackContainers, and `unpackedItems` includes whatever is left from every unfinished group.

### PackingService.PackContainersWithGroupsContinuously

![PackContainersWithGroupsContinuously](../Miscellany/Resources/Images/Large/Miscellany.ContainerPacking.PackingService.PackContainersWithGroupsContinuously.Large.png)

Like PackContainersWithGroups, but **groups can share a Container** so less space is wasted. The groups keep their order. Before each Container is packed, the node collects at least `minimumItems` Items to consider: first any leftovers from the previous Container, then Items from the current group, then Items from the following groups until it has enough.

| Extra input | Description |
|---|---|
| `minimumItems` | The minimum number of Items to consider for each Container (default `20`; values below 1 are treated as 1) |

A larger `minimumItems` gives the algorithm more choice, so Containers are usually filled better, but Items from later groups are pulled forward sooner. The outputs are the same as PackContainers.
