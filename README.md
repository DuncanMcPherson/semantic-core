
# SemanticRelease.Core

A .NET implementation of semantic versioning and release automation for .NET projects. This project is inspired by the [semantic-release](https://github.com/semantic-release/semantic-release) package for Node.js.

## Overview

SemanticRelease.Core provides automated versioning and package publishing by analyzing commit messages. It follows the [Semantic Versioning](https://semver.org/) specification to determine the next version number based on the types of changes made.

## Features

- Automatic version determination based on commit history
- Semantic versioning compliance
- Automatic CHANGELOG generation
- Git tag management
- Plugin-based architecture for extensibility

## Installation

Install the NuGet package:

```bash
dotnet add package SemanticRelease.CoreBehavior
```

## Dependencies

- .NET Standard 2.1
- LibGit2Sharp (for Git repository operations)
- SemanticRelease.Abstractions

## Usage

The core plugin can be registered with the semantic release lifecycle:

```csharp
var lifecycle = new SemanticLifecycle();
var corePlugin = new CorePlugin();
corePlugin.Register(lifecycle);
```

## How It Works

The plugin analyzes your Git repository to:

1. Find the last release tag
2. Collect commits since that tag
3. Analyze commit messages to determine the next version
4. Update project files with the new version
5. Generate a changelog
6. Create a new Git tag

## Configuration

Configuration is handled through a `semantic-release.json` file in your project root, similar to the Node.js semantic-release package.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgements

This project was inspired by and based on concepts from the [semantic-release](https://github.com/semantic-release/semantic-release) package for Node.js, bringing similar functionality to the .NET ecosystem.
