# Servus.Akka

> An Akka.NET quality of life extension library. Filled with features that you didn't know you actually missed ;)

[![NuGet](https://img.shields.io/nuget/v/Servus.Akka.svg)](https://www.nuget.org/packages/Servus.Akka/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/Bavaria-Black/servus.akka/build-and-release.yml?branch=main)](https://github.com/Bavaria-Black/servus.akka/actions)
[![License](https://img.shields.io/github/license/Bavaria-Black/servus.akka)](LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/Servus.Akka.svg)](https://www.nuget.org/packages/Servus.Akka/)

## Overview

I started **Servus.Akka** after I wrote the same quality of life improvements for the third time across different projects. This library contains all those little utilities, extensions, and helpers that make working with Akka.NET more enjoyable and productive.

Instead of copy-pasting utility code between projects or reinventing the wheel, Servus.Akka provides battle-tested solutions for common Akka.NET patterns and pain points.

## Why Servus.Akka?

- **Born from Real Projects**: Every feature comes from actual production use cases
- **Zero Overhead**: Lightweight extensions that don't change Akka.NET's core behavior
- **Developer Friendly**: Intuitive APIs that feel natural in your existing codebase
- **Well Tested**: Comprehensive test coverage ensures reliability
- **Bavarian Quality**: Built with the same attention to detail as a fine German engineering project

## Installation

### Package Manager
```
Install-Package Servus.Akka
```

### .NET CLI
```bash
dotnet add package Servus.Akka
```

### PackageReference
```xml
<PackageReference Include="Servus.Akka" Version="1.0.0" />
```

## Features

### Easy actor registration that supports DependencyInjection

```csharp
var builder = WebApplication.CreateBuilder();
builder.Services.AddAkka("servus", b =>
{
    b.WithResolvableActors(helper =>
        {
            helper
                .Register<TestActor1>()
                .Register<TestActor2>();
        });
});
```

### Resolvable IActorRef<T> from IServiceProvider
```csharp
var a = Host.Services.GetService<IActorRef<ResolvingTestActor>>();
        a.Tell("hello");
```

##


## Documentation

- **[Getting Started Guide](docs/getting-started.md)** - Your first steps with Servus.Akka
- **[API Reference](docs/api-reference.md)** - Detailed API documentation  
- **[Examples](docs/examples/)** - Real-world usage examples
- **[Migration Guide](docs/migration.md)** - Upgrading from previous versions
- **[Best Practices](docs/best-practices.md)** - Recommended patterns and practices

## Compatibility

- **.NET**: 8.0+
- **Akka.NET**: 1.5.0+
- **Platforms**: Windows, Linux, macOS

## Contributing

Contributions are welcome! This library grows with the community's needs.

### How to Contribute

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/amazing-utility`
3. **Write** tests for your changes
4. **Ensure** all tests pass: `dotnet test`
5. **Submit** a Pull Request

### Contribution Guidelines

- Follow existing code style and conventions
- Include unit tests for new features
- Update documentation
- Add examples for complex features
- Keep changes focused and atomic

### Ideas for Contributions

- Documentation improvements

## Community & Support

- **GitHub Issues**: Bug reports and feature requests
- **Discussions**: General questions and community chat

## Acknowledgments

- **Akka.NET Team**: For building the excellent foundation this library extends
- **Contributors**: Everyone who has helped make this library better

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Servus and happy coding!** 🥨

*Feel free to use it and feel free to contribute other useful stuff.*

For questions or support, please [open an issue](https://github.com/Bavaria-Black/servus.akka/issues).
