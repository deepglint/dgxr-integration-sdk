## 半自动灵航部署工具

### 使用方法
1. 在urls.txt中配置所有下载url，参考urls文件中默认提供的两个
2. 将deploy.bat和urls.txt放在一个文件夹中，将这个文件夹放到需要部署的电脑上，点击deploy.bat运行
   
DeployExample
├── deploy.bat
└── urls.bat 

3. 运行成功之后会在文件夹下生成一个当前日期文件夹，将shortcut文件夹中所有快捷方式拷贝到灵航StreamingAssets/App目录中，将App.json中的路径更改成对应路径即可
   
注意：如果是本工具生成的快捷方式，在配置时需要在路径加上.lnk后缀


DeployExample
├── 20240614
│   ├── shortcut
│   │   └── meta-match_hash_dev.exe
│   ├── meta-match_hash_dev
│   └── meta-starter_hash_dev 
├── deploy.bat
└── urls.bat 