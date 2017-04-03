#!/bin/bash

pushd netcode.io-import
cd c
../../premake5/Linux/premake5 gmake
make