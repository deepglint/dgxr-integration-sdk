#!/bin/sh

echo 'Setting up the script...'
set -e

DOXYFILE=Doxyfile
# rm -rf *
echo 'Generating Doxygen code documentation...'
# 更新 Doxyfile 中的PROJECT_NUMBER字段
# sed -i "51s/PROJECT_NUMBER           = .*/PROJECT_NUMBER           = $CI_COMMIT_TAG" $DOXYFILE
sed -i "51s/^PROJECT_NUMBER[[:space:]]*=.*/PROJECT_NUMBER           = $CI_COMMIT_TAG/" $DOXYFILE

doxygen $DOXYFILE 2>&1
tee doxygen.log

mkdir -p /cosfs/unity/dgxr/dgxr-doc-$CI_COMMIT_TAG/
cp -r $UNITY_DIR/code_docs/html/ /cosfs/unity/dgxr/dgxr-doc-$CI_COMMIT_TAG/
echo "Documentation Link: $COS_REPO/dgxr/dgxr-doc-$CI_COMMIT_TAG/html/index.html Finished!"