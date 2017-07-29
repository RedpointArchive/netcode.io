#!/bin/bash

set -e
set -x

export LANG=en_US.UTF-8
export LANGUAGE=en_US.UTF-8
export LC_ALL=en_US.UTF-8
ruby -e 'File.write(ARGV[0], File.read(ARGV[0]).gsub(ARGV[1], ARGV[2]))' netcode.io-import/c/netcode.c '4 * 1024 * 1024' '2 * 1024 * 1024'

pushd netcode.io-import
export LIBS="-lsodium -L/usr/local/lib"
cd c
../../premake5/MacOS/premake5 gmake
make