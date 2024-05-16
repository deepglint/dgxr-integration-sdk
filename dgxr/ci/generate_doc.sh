#!/bin/sh

echo 'Setting up the script...'
set -e

# rm -rf *
echo 'Generating Doxygen code documentation...'
doxygen $DOXYFILE 2>&1
# tee doxygen.log
