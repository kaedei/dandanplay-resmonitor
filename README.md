# 弹弹play资源监视器 dandanplay-resmonitor

![License](https://img.shields.io/github/license/kaedei/dandanplay-resmonitor)
![GitHub repo size](https://img.shields.io/github/repo-size/kaedei/dandanplay-resmonitor)
![Docker Image Size (latest by date)](https://img.shields.io/docker/image-size/kaedei/dandanplay-resmonitor)

[![Build status](https://dev.azure.com/kaedei/dandanplay-resmonitor/_apis/build/status/dandanplay-resmonitor-ASP.NET%20Core-CI)](https://dev.azure.com/kaedei/dandanplay-resmonitor/_build/latest?definitionId=11)
[![DockerHub](https://images.microbadger.com/badges/version/kaedei/dandanplay-resmonitor.svg)](https://hub.docker.com/repository/docker/kaedei/dandanplay-resmonitor)
[![Github Release](https://vsrm.dev.azure.com/kaedei/_apis/public/Release/badge/9739e25e-bed4-42b9-9872-fa328f18783b/2/3)](https://github.com/kaedei/dandanplay-resmonitor/releases)

## 一、介绍

### 一句话简介

这是一个动漫资源自动监视+下载的小工具。

### 它能做什么

此工具模拟了 [弹弹play播放器](http://www.dandanplay.com/) Windows版上的“自动下载”功能，能够根据设定好的监视规则自动检测网上是否有新的资源发布，当发现新的资源后自动通知下载工具新建任务并开始下载。

简单来说，一般从网上找资源的流程是 `访问资源站->搜索->选择->下载->观看`。此工具可以帮你把 `访问资源站->搜索->选择` 这前几步变成完全自动的，不用等每次资源有更新时再手动操作一遍了，能为你节省很多时间和精力。

此工具没有内置下载器，但是支持调用已有的 __Aria2__、__Transmission__ 等下载工具进行BT下载。

### 开发工具

此工具使用 `ASP.NET Core` 技术编写，编程语言使用了 `C#`。
本项目使用 macOS 10.15 下的 `Jetbrains Rider` IDE 进行开发。SDK版本目前为 .NET Core SDK 3.1.100。

### 付费

是的，为了补贴各种服务器费用和日常开销，本工具虽是开源产品，但需要赞助作者后才能使用。

本工具内部连接了弹弹play服务器端API，试用期结束后，您需要赞助作者才可以持续使用。赞助方式将通过 [爱发电项目页](https://afdian.net/@kaedei) 进行。

对于此工具的新用户，将会在您第一次使用弹弹play登录后向您赠送7天的免费试用时间。

## 二、怎样使用

### 在弹弹play客户端上注册用户并创建规则

1. 去弹弹play官网 [dandanplay.com](http://www.dandanplay.com) 下载安装最新版的Windows客户端或UWP客户端

2. 在首页或者“个人中心”页登录或注册一个新的弹弹play账号

3. 登录弹弹play账号后，点击上方菜单导航到“资源”-“自动下载”页面

4. 新建一个“自动下载”规则，填写对应的监视参数。新建完毕后此规则会被自动同步到云端。

### 在本机运行（使用 dotnet 命令）

1. 目前还没有提供特定操作系统的本机代码版本，所以如要运行此工具，必须要先安装[.NET Core 3.1 运行时或SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)。

2. 然后在[Github发布页面](https://github.com/kaedei/dandanplay-resmonitor/releases)下载编译好的最新版本，将所有文件解压到本地的某个文件夹中。比如说 `C:\dandanplay-resmonitor\`。

3. 进入此文件夹，使用编辑器软件打开 `appsettings.json` 文件，修改软件配置。这里将会有一篇详细的说明文档：[本机运行](https://github.com/kaedei/dandanplay-resmonitor/wiki/%E4%BF%AE%E6%94%B9%E9%85%8D%E7%BD%AE#%E6%9C%AC%E6%9C%BA%E8%BF%90%E8%A1%8C)。建议修改完成后备份此配置文件，免得将来升级时被不小心覆盖。

4. 使用命令行（cmd Terminal等）进入此文件夹，然后执行 `dotnet ResourceMonitor.dll` 命令，即可启动。启动后将会持续运行，直到你关闭窗口或是按 Ctrl+C 结束进程。

### docker镜像部署

[![DockerHub](https://images.microbadger.com/badges/version/kaedei/dandanplay-resmonitor.svg)](https://hub.docker.com/repository/docker/kaedei/dandanplay-resmonitor)

1. 运行命令 `docker run -it kaedei/dandanplay-resmonitor` 即可启动此工具

2. 添加 `-e` 参数可以（通过改变环境变量的方式）自定义程序运行参数。这一步是必需的，详情请参考文档 [docker部署](https://github.com/kaedei/dandanplay-resmonitor/wiki/%E4%BF%AE%E6%94%B9%E9%85%8D%E7%BD%AE#docker)

3. 添加 `-d` 参数可以让容器在后台持续运行

4. 添加 `-p 本机端口:80` 可以暴露内部的web服务，之后通过浏览器访问刚才指定的本机IP和端口（例如 http://192.168.1.100:8080) 即可看到此工具的当前状态。

5. 此工具的日志文件位于容器内的 `/app/log` 目录，可以通过 `-v` 挂载本机目录，然后查看这些日志文件。

## 三、开发者相关

### 配置开发环境

1. 通过 [GitHub的打包下载](https://github.com/kaedei/dandanplay-resmonitor/archive/master.zip) 或通过 `git clone` 下载最新版的代码到本机目录

2. 安装 `.NET Core SDK 3.1.100` 或更高的版本。[官方下载链接](https://dotnet.microsoft.com/download/dotnet-core/3.1)

### 编译代码

1. 命令行导航到源代码所在目录，例如 `C:\code\dandanplay-resmonitor`

2. 进入`ResourceMonitor.csproj` 所在的文件夹，大概在 `C:\code\dandanplay-resmonitor\ResourceMonitor\ResourceMonitor\ResourceMonitor.csproj`

3. 运行命令 `dotnet build -c Release` 即可以Release模式编译代码，编译后的文件位于 `\bin\Release\netcoreapp3.1\publish` 文件夹中

### 其他说明

- 为了保证程序可以随时启动、终止并保证状态一致，代码中没有设计存储层，即不会将状态保存到外部数据库、Redis、文件系统中。

- 可以通过扩展实现 `IDownloader` 接口来支持更多的下载工具
