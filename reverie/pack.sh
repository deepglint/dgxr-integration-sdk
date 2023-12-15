#!/bin/bash
TAG=$1
if [ -z $TAG ]; then
  TAG="main"
fi

if [ -f "$TAG.exe" ]; then
    mkdir -p "stardust-$TAG"
    temp_dir="stardust-$TAG"
    # 复制 main.exe 到临时目录
    cp $TAG.exe "$temp_dir"
    
    config="$temp_dir/config"
    dll="$temp_dir/source/output/xbox"
    tempalte="$temp_dir/db/"

    mkdir -p "$dll"
    mkdir -p "$tempalte"
    mkdir -p "$config"
    cp source/output/xbox/ViGEmClient.dll "$dll"
    cp -r db/template "$tempalte"
    cp config/config.yaml "$config"
    echo "打包完成：$temp_dir"
else
    echo "错误：$TAG 文件不存在"
    exit 1
fi