# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.4.0] - 2024-11-08

- **New**:
- Added new "Toggle Auto Import" button in the "Tools/Addressables/" Menu item to allow pausing processing all asset importers when code recompiles. Allowing the editor to be faster reloading on script changes

- **Refactor**:
- Changed *IAssetLoader* to execute a callback when operation is completed. This allows for non-Task operations to execute logic in it's own scope

- **Fixed**:
- Prevented the generation of *AddressableId* when it failed to properly process an asset type. This way avoids future compilation errors while properly reporting the Asset Type failed to load.

## [0.3.0] - 2024-04-27

- **New**:
- Added new AddressableId Generator Settings scriptable object to control some of the settings being generated

- **Refactor**:
- Moved the Unity Editor commands to the Tools/AddressableId Generator path

## [0.2.1] - 2023-09-04

- **Refactor**:
- Changed AddressableConfig from a struct to a class. This change enhances the flexibility and efficiency of memory usage in our codebase, as AddressableConfig instances can now be shared or null, reducing potential redundancy. Please note that this may alter how AddressableConfig is used in some contexts due to the shift from value type to reference type.

## [0.2.0] - 2023-08-27

- **New**:
- Introduced AssetsImporter tool for importing assets data in Unity, accessible from the Unity Editor's Tools menu.
- Added AssetReferenceScene class to validate scene assets.

- **Refactor**:
- Updated AddressableIdsGenerator to include new namespaces, enums, and lookup methods. Improved asset filtering based on labels.
- Changed parameter type of LoadAssetAsync and InstantiateAsync methods from string to object in AddressablesAssetLoader.
- Renamed namespace from "GameLovers.AddressablesExtensions" to "GameLovers.AssetsImporter" across multiple files.

- **Fixed**:
- Prevented destruction of GameObjects in UnloadAsset method of AddressablesAssetLoader.

## [0.1.1] - 2020-08-31

- **Fixed**:
- Fixed the namespace set of the generated file from the *AddressablesIdGenerator*

## [0.1.0] - 2020-08-31

- Initial submission for package distribution
