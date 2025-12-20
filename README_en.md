# MouseClickTool

<img src="https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/img/MouseClickTool_round.png" alt="MouseClickTool Logo" width="64" />

[![MouseClickTool Latest Version](https://img.shields.io/github/v/release/lalakii/MouseClickTool?logo=github)](https://github.com/lalakii/MouseClickTool/releases)
[![Downloads](https://img.shields.io/github/downloads/lalakii/MouseClickTool/total)](https://github.com/lalakii/MouseClickTool/releases)
[![MouseClickTool.exe Windows Program](https://img.shields.io/badge/windows-.exe-0078D4?logo=windows)](https://mouseclicktool.sourceforge.io/)

[ [简体中文](./README.md) | [English](./README_en.md) ]

> A simple and easy-to-use mouse auto-clicker.

## Features

- Left/right mouse clicks, long-press, and mouse wheel scrolling
- Customizable click interval (in milliseconds)
- Customizable hotkeys
- Scheduled triggering and timed launching of external programs
- Dark mode support
- Automatically saves settings to a temporary file (no registry modifications)
- Customizable scripts (Beta): [How do I write scripts?](https://github.com/lalakii/MouseClickTool/blob/master/README_en.md#write-custom-scripts)

## Download

[Github Releases](https://github.com/lalakii/MouseClickTool/releases)

<img src="https://fastly.jsdelivr.net/gh/lalakii/MouseClickTool/img/MouseClickTool_en.png?v=2.0" alt="Screenshot of MouseClickTool"/>

## Write custom scripts

The MouseClickTool script file has the extension "*.msck".

**Precautions**

  + Comments begin with '#'
  + Comments should be on a separate line and not mixed with the code
  + There should be no extra spaces on the line containing the code
  + Blank lines within the file will not affect script execution; additional blank lines can be added for easier readability.

  [Demo script example](./Scripts/demo_en.msck)

```c
# Sets the text displayed in the title bar of the window.
title("Your title")

# Wait N milliseconds, 1 parameter.
delay(ms)

# Left-click, two parameters: x and y coordinates.
left_click(x,y)

# Right-click, two parameters: x and y coordinates.
right_click(x,y)

# Left-LongClick, three parameters, x and y coordinates, type: 1 indicates pressing, 2 indicates releasing.
# When long-pressing, pay attention to the order; you must press down first and then release. Using delay can adjust the duration of the long press.
# Without adding a delay, it is just a regular click.
left_click_long(x,y,type)

# Right-LongClick, three parameters, x and y coordinates, type: 1 indicates pressing, 2 indicates releasing.
right_click_long(x,y,type)

# Mouse wheel scrolling, 1 parameter, the value can be positive or negative, indicating whether the scrolling direction is up or down.
mouse_wheel(value)

# Launch an external program, only one parameter is required: fileName, which represents the full path to the program and can include application startup parameters.
create_process("fileName")

# Stop the running script; no parameters are required. Do not use this function if looping is required.
once()

# Terminate the current process to stop the mouse clicker; no parameters are required.
exit()
```

## FAQs

If the tool cannot click in certain applications or games, try running it as Administrator or with TrustedInstaller privileges.

Here's a helpful tool for running with elevated privileges: [M2TeamArchived/NSudo](https://github.com/M2TeamArchived/NSudo/releases/).

Be careful when setting up hotkeys that they don't conflict with other programs.

If you need to test the speed of a mouse clicker, I know these sites:

- [Click test 10 seconds | CPS Check](https://cps-check.com/)
- [CPS Test / CPS Tester - Check Your CPS with Clicks Tracking Chart](https://www.arealme.com/click-speed-test/)

## By lalaki.cn
