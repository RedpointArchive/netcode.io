#!/bin/bash

set -e
set -x

mkdir -pv native-dist/

cp netcode.io-csharp/bin/MacOS/AnyCPU/Release/libnetcode.dylib native-dist/libnetcode.dylib
