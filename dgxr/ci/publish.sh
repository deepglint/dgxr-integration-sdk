#!/bin/bash

npm login --registry=https://registry.npmjs.org/ --username="$CI_NPM_PRIVATE_USER" --password="$CI_NPM_PRIVATE_PASSWORD"
npm config set registry http://package.nemoface.com/repository/unity
npm publish