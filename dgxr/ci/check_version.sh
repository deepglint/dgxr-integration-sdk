#!/bin/bash

tag=$(echo "$CI_COMMIT_REF_NAME" | sed -n 's/^v([0-9.]*)$/\1/p')
version=$(cat ../Assets/package.json | grep -oP '(?<="version": ")[^"]*')

if [ "$tag" != "$version" ]; then
  echo "Error: Please manually update the version field in package.json to $tag"
  exit 1
fi