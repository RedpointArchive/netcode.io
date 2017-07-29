#!/bin/bash

# Try to install libsodium for Linux
apt-get install -y libsodium-dev libsodium18 swig || true

pushd netcode.io-import
cd c
../../premake5/Linux/premake5 gmake
make