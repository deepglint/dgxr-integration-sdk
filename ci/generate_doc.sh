#!/bin/sh

echo 'Setting up the script...'
set -e

DOXYFILE=$UNITY_DIR/Documentation/Doxyfile
# rm -rf *
echo 'Generating Doxygen code documentation...'
# 更新 Doxyfile 中的PROJECT_NUMBER字段
echo "version $CI_COMMIT_TAG to $DOXYFILE"
sed -i "51s|^PROJECT_NUMBER[[:space:]]*=.*|PROJECT_NUMBER           = $CI_COMMIT_TAG|" $DOXYFILE

doxygen $DOXYFILE 2>&1
tee doxygen.log

mkdir -p /cosfs/unity/dgxr/dgxr-doc-$CI_COMMIT_TAG/
cp -r $UNITY_DIR/code_docs/html/ /cosfs/unity/dgxr/dgxr-doc-$CI_COMMIT_TAG/
echo "Documentation Link: $COS_REPO/dgxr/dgxr-doc-$CI_COMMIT_TAG/html/index.html Finished!"
