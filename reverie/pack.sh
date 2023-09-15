#!/bin/bash

# 检查是否存在 main.exe 文件
if [ -f "main.exe" ]; then
    # 创建一个临时目录用于存放文件
    temp_dir=$(mktemp -d)
    
    # 复制 main.exe 到临时目录
    cp main.exe "$temp_dir"
    
    # 复制 config 目录到临时目录
    if [ -d "config" ]; then
        cp -r config "$temp_dir"
    fi
    
    # 压缩文件并包含日期后缀
    archive_filename="reverie.tar.gz" # 或者 .zip
    
    # 创建压缩包
    tar -czvf "$archive_filename" -C "$temp_dir" .
    
    # 删除临时目录
    rm -r "$temp_dir"
    
    echo "打包完成：$archive_filename"
else
    echo "错误：main.exe 文件不存在"
fi