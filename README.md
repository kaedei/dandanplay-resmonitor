# 弹弹play资源监视器 dandanplay-resmonitor

## 一、介绍

### 一句话简介

这是一个动漫资源自动监视+下载的小工具，并接入了弹弹play服务器端API。

### 它能做什么

此工具模拟了 [弹弹play播放器](http://www.dandanplay.com/) Windows版上的“自动下载”功能，能够根据设定好的监视规则自动检测网上是否有新的资源发布，当发现新的资源后自动通知下载工具新建任务并开始下载。

### 技术选型

此工具使用 `ASP.NET Core` 技术编写，编程语言使用了 `C#`。

选择 `ASP.NET Core` 的原因主要是支持直接打包成exe双击运行在本机系统中，也可以通过 `dotnet` 命令行直接启动dll，还支持打包进docker镜像运行。支持的操作系统包括：Windows 10, Windows Server Core（以及 Windows Container）, Linux, macOS。

### 开发工具

本项目使用 macOS 10.15 下的 `Jetbrains Rider` IDE 进行开发。SDK版本目前为 .NET Core SDK 3.1.100。

### 付费

是的，为了补贴各种日常开销，本工具虽是开源产品，但需要赞助作者后才能使用。

本工具内部连接了弹弹play服务器端API，试用期结束后，您需要赞助作者才可以持续使用。赞助方式将通过 [爱发电项目页](https://afdian.net/@kaedei) 进行（__现在还没有开始接受赞助！__）。

对于此工具的新用户，将会在您第一次使用弹弹play登录后向您赠送7天的免费试用时间。

## 二、怎样使用

### 在弹弹play客户端上注册用户并创建规则

TODO

### 本机（PC）运行

TODO

### docker镜像部署（Linux）

TODO

### docker镜像部署（Windows Container）

TODO

## 三、开发者相关

### 配置开发环境

TODO
