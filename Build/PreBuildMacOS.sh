#!/bin/bash

set -e
set -x

# Patch 4MB limit to 2MB, as MacOS's limit for buffers is lower.
alias replace='ruby -e "File.write(ARGV[0], File.read(ARGV[0]).gsub(ARGV[1], ARGV[2]))"'
replace netcode.io-import/c/netcode.c "4 * 1024 * 1024" "2 * 1024 * 1024"

pushd netcode.io-import
export LIBS="-lsodium -L/usr/local/lib"
cd c
../../premake5/MacOS/premake5 gmake
make