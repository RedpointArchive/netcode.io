#!/bin/bash

set -e
set -x

mkdir -pv native-dist/

cp netcode.io-csharp/bin/Linux/AnyCPU/Release/libnetcode32.so native-dist/libnetcode32.so
cp netcode.io-csharp/bin/Linux/AnyCPU/Release/libnetcode64.so native-dist/libnetcode64.so
cp netcode.io-csharp/bin/Linux/AnyCPU/Release/netcodeBinding.dll.config native-dist/netcodeBinding.dll.config
