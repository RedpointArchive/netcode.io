#!/bin/bash

set -e
set -x

ruby -e 'File.write(ARGV[0], File.read(ARGV[0]).gsub(ARGV[1], ARGV[2]))' netcode.io-import/c/netcode.c '4 * 1024 * 1024' '2 * 1024 * 1024'

pushd netcode.io-import
export LIBS="-lsodium -L/usr/local/lib"
cd c
../../premake5/MacOS/premake5 gmake
make