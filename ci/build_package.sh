#!/usr/bin/env bash

set -e
set -x
echo "Building unitypackage file"
echo $UNITY_DIR
export BUILD_PATH=$UNITY_DIR/Packages
export PROJECT_PATH=$UNITY_DIR
mkdir -p $BUILD_PATH

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' unity-editor} \
  -projectPath $PROJECT_PATH \
  -quit \
  -batchmode \
  -nographics \
  -exportPackage 'Assets' $BUILD_PATH/DGXRIntegrationSDK.unitypackage
 
UNITY_EXIT_CODE=$?

echo "Unity process exited with code $UNITY_EXIT_CODE"

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi

ls -la $BUILD_PATH
[ -n "$(ls -A $BUILD_PATH)" ] # fail job if build folder is empty
