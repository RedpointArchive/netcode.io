# C# bindings for netcode.io

This repository contains C# bindings for netcode.io.

## Install from NuGet

This library is available on NuGet, with support for Windows, macOS and Linux: [netcode.io](https://www.nuget.org/packages/netcode.io)

You can install it with the package manager command line:

```
Install-Package netcode.io
```

Or install it with Protobuild if you're using Protobuild to manage projects:

```
Protobuild.exe --install netcode.io
```

## Compiling from source

To generate the binding projects, run:

```
Protobuild.exe --generate
```

You can then build the bindings using your IDE, or `Protobuild.exe --build` on the command-line.

### MacOS

On MacOS, you'll need to install the libsodium library before compilation will succeed.  Run the following:

```
brew install libsodium --universal
```

### Linux

On Linux, you'll need to install the libsodium library before compilation will succeed.  Install it using your package manager, and make sure to install the `-dev` or `-devel` variant as well so that the necessary include headers are available.

## License

These bindings are made available under the MIT license.