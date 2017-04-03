#!/bin/bash

set -e
set -x

pushd netcode.io-import
export LIBS="-lsodium -L/usr/local/lib"
cd c
make