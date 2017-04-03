#!/bin/bash

pushd netcode.io-import
cd c
../../premake5/MacOS/premake5 gmake
make