# MouseClickTool

<img src="https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/img/MouseClickTool_round.png" alt="MouseClickTool Logo" width="64" />

[![MouseClickTool Latest Version](https://img.shields.io/github/v/release/lalakii/MouseClickTool?logo=github)](https://github.com/lalakii/MouseClickTool/releases)
[![Downloads](https://img.shields.io/github/downloads/lalakii/MouseClickTool/total)](https://github.com/lalakii/MouseClickTool/releases)
[![MouseClickTool.exe Windows Program](https://img.shields.io/badge/windows-.exe-0078D4?logo=windows)](https://mouseclicktool.sourceforge.io/)

[ [English](./README_en.md) | [简体中文](./README.md) ]

> 简单好用的鼠标连点器，玩游戏需要鼠标连点，在网上找了好多鼠标连点器，有的居然还要收费...
>
> 一向穷苦的我感觉为此花钱很不值得，索性自己写了个。。。

## 功能介绍

- 支持切换鼠标左/右键点击/长按点击/滚轮滚动
- 支持自定义点击间隔，单位是毫秒
- 支持自定义热键
- 支持指定时间触发 / 定时启动外部程序
- 自动记忆配置参数，配置文件存储在“我的文档”目录，不污染注册表
- 已适配深色模式
- 已添加随机扰动，避免被检测
- 自定义脚本(Beta) [如何编写脚本?](https://github.com/lalakii/MouseClickTool?tab=readme-ov-file#%E7%BC%96%E5%86%99%E8%87%AA%E5%AE%9A%E4%B9%89%E8%84%9A%E6%9C%AC)

## 下载

[本地下载](https://github.com/lalakii/MouseClickTool/releases) | [123 云盘](https://www.123865.com/s/jE3Sjv-gfxxd) | [蓝奏云](https://a01.lanzout.com/b0hdl1xde) **密码：7tgq**

<img src="https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/img/MouseClickTool.png?v=2.0" alt="Screenshot of MouseClickTool"/>

## 编写自定义脚本

MouseClickTool 脚本文件，文件后缀名为"*.msck"

**注意事项**

  + 注释使用'#'开头
  + 注释应当另起一行，不要和代码混在一起
  + 代码所在的行，不应出现多余空格
  + 文件内的空行不会影响脚本执行，为了便于阅读可以多加空行

  [查看Demo脚本示例](./Scripts/demo.msck)

```c
# 修改窗体标题栏上的文字, 1个参数
title("Your title")

# 等待n毫秒, 1个参数
delay(ms)

# 鼠标左键单击, 2个参数，x,y坐标
left_click(x,y)

# 鼠标右键单击, 2个参数, x,y坐标
right_click(x,y)

# 左键长按, 3个参数, x,y坐标, type可选1(按下)或0(松开) 
# 长按时需要注意顺序, 必须先按下再松开, 搭配delay可以实现长按的时间
# 如果不添加delay, 相当于一次普通单击
left_click_long(x,y,type)

# 右键长按, 3个参数, x,y坐标, type可选1(按下)或0(松开)
right_click_long(x,y,type)

# 鼠标滚轮滚动, 1个参数, value可以为是正数或负数, 分别是向上或向下滚动
mouse_wheel(value)

# 启动程序, 1个参数, fileName表示程序完整路径, 可携带参数
create_process("fileName")

# 结束当前脚本, 无参数, 脚本默认循环执行, 需要循环执行时不要添加
once()

# 结束进程, 无参数, 直接退出连点器
exit()
```

## 常见问题

如何退出？如果鼠标点击频率过快无法停下来，请先让程序窗口到前台（ALT+TAB），然后按 ALT+F4 关闭程序。

如果有软件无法被点击，尝试以管理员身份或者TrustedInstaller特权运行此软件。

这里附上一个提权工具: [M2TeamArchived/NSudo](https://github.com/M2TeamArchived/NSudo/releases/)

设置热键时留意是否会和其他程序冲突。

如果需要测试连点器速度，我知道这些网站：

- [点击速度测试 10 秒](https://cps-check.com/cn/)
- [CPS 测试 - 鼠标点击速度测试](https://www.arealme.com/click-speed-test/cn/)

## By lalaki.cn
