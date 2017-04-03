#!/bin/bash

set -e
set -x

pushd netcode.io-import
export LIBS="-lsodium -L/usr/local/lib"
cd c
../../premake5/MacOS/premake5 gmake
make