#!/bin/sh

echo 'Setting up the script...'
set -e

# rm -rf *
echo 'Generating Doxygen code documentation...'
doxygen $DOXYFILE 2>&1
tee doxygen.log

mkdir -p /cosfs/unity/dgxr/dgxr-doc-$CI_COMMIT_TAG/
cp -r $UNITY_DIR/code_docs/html/ /cosfs/unity/dgxr/dgxr-doc-$CI_COMMIT_TAG/
echo "Documentation Link: $COS_REPO/dgxr/dgxr-doc-$CI_COMMIT_TAG/html/index.html Finished!"