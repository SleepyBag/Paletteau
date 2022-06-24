Paletteau
===

**Paletteau** aims to provide interactive command palette for Windows programs, which do not come with such functionality.
**Paletteau** is a fork of [Wox](https://github.com/Wox-launcher/Wox).

![demo](http://i.imgur.com/DtxNBJi.gif)

Features
--------

- Use *pinyin* to search for programs / 支持用 **拼音** 搜索程序
  - wyy / wangyiyun → 网易云音乐
- Customizable themes
- Develop plugins yourself
- Highlighting of how results are matched during query search


Installation
------------

- Download from [releases](https://github.com/SleepyBag/Paletteau/releases).

- Requirements:
  - .NET >= 4.6.2 or Windows version >= 10 1607 (Anniversary Update)

Usage
-----

- Launch: <kbd>Alt</kbd>+<kbd>Space</kbd>
- Context Menu: <kbd>Ctrl</kbd>+<kbd>O</kbd>
- Cancel/Return: <kbd>Esc</kbd>
- Install/Uninstall plugin: type `wpm install/uninstall`
- Reset: delete `%APPDATA%\Wox`
- Log: `%APPDATA%\Wox\Logs`

Contribution
------------

- First and most importantly, star it!
- Send PR to master branch
- I'd appreciate if you could solve [help_wanted](https://github.com/Wox-launcher/Wox/issues?q=is%3Aopen+is%3Aissue+label%3A%22help+wanted%22) labeled issue

Build
-----

Use Visual Studio 2022 with .NET desktop development and UWP development

Thanks
------

- [Wox](https://github.com/Wox-launcher/Wox) for their wonderful work!
