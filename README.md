![Logo](Miscellany/Resources/Images/Large/Miscellany.About.Miscellany.Large.png)

# Miscellany for Dynamo

[![Build](https://github.com/thomascorrie/Miscellany/actions/workflows/build.yml/badge.svg)](https://github.com/thomascorrie/Miscellany/actions/workflows/build.yml)
[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](CONTRIBUTING.md)
[![GitHub version](https://badge.fury.io/gh/thomascorrie%2FMiscellany.svg)](https://badge.fury.io/gh/thomascorrie%2FMiscellany)

A collection of miscellaneous nodes for [Dynamo](https://dynamobim.org/) including an implementation of the C# library [3DContainerPacking](https://github.com/davidmchapman/3DContainerPacking) to use the [EB-AFIT container packing algorithm](https://github.com/wknechtel/3d-bin-pack) in Dynamo

![Nodes](Samples/Miscellany-Samples-Nodes.png)

## Installation
**Miscellany** is available on the Dynamo package manager.

## Documentation and Samples
See the [documentation](docs/README.md) and the [sample dyn files](Samples).

## Future Development and Issue Tracking
See [Issues](https://github.com/thomascorrie/Miscellany/issues) for planned improvements and open issues.

## Contributing
Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for how to build, test and submit changes.

## Requirements
Miscellany is built in two versions:

| Build | Dynamo | Revit | .NET |
|---|---|---|---|
| Dynamo2 | 2.1 – 2.19 | 2020 – 2024 | .NET Framework 4.8 |
| Dynamo3 | 3.0 and later | 2025 and later | .NET 8 (also loads in Dynamo's .NET 10 releases) |

## Licence
[MIT](LICENSE). Releases up to and including 1.2.0 were published under earlier licences (LGPL-3.0 in this repository, AGPL-3.0 on the Dynamo package listing).

Third-party components are listed in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).

## Author
Thomas Corrie: [GitHub](https://github.com/thomascorrie) - [Twitter](https://twitter.com/didymuscoombe) - [Website](http://www.thomascorrie.com)

## Acknowledgements
The Container Packing nodes build on the C# library [3DContainerPacking](https://github.com/davidmchapman/3DContainerPacking) by [David Chapman](https://github.com/davidmchapman) which in turn is based on the C library [3d-bin-pack](https://github.com/wknechtel/3d-bin-pack/) by [Bill Knechtel](https://github.com/wknechtel) which resurrects a thesis by Ehran Baltacıoğlu at the Air Force Institute of Technology.
