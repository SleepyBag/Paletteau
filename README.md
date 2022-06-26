Paletteau
===

**Paletteau** is a fork of [Wox](https://github.com/Wox-launcher/Wox).

Recent productive applications often come with a command palette, by which you can fuzzily search through all available commands.
For example, pressing `ctrl+shift+p` in vscode calls a fancy [command palette](https://code.visualstudio.com/docs/getstarted/userinterface#_command-palette).
Windows Terminal also provides a great [command palette](https://docs.microsoft.com/en-us/windows/terminal/command-palette).

Though most applications today doesn't provide such modern thing - or provided but doesn't work well (`ctrl-q` of visual studio, for example).

**Paletteau** aims to provide interactive command palette for these programs.
Basically, it comes with a command table for each supported application.
Each command is mapping to a keyboard shortcut of the application.
When you select a command a command in Paletteau, it sends a key sequence to the application, just like you pressed the shortcut.
This way, Paletteau provides basic frequent functionality of supported applications without an intrusive agent in the application side.
It also causes **limitations**, too - we cannot trigger functionalities without a binding keyboard shortcut using Paletteau.

![demo](https://i.imgur.com/DfuhETR.gif)

Features
--------

- Faster filtering than Wox, powered by a new dynamic programming algorithm.
- Use *pinyin* to search for programs / 支持用 **拼音** 搜索程序
  - wyy / wangyiyun → 网易云音乐
- Customizable themes
- Develop Paletteau plugins yourself
- Highlighting of how results are matched during query search


Installation
------------

- Download from [releases](https://github.com/SleepyBag/Paletteau/releases).
- There is only zip release now. No installer available (welcome for PR!).
- Unzip the archive to anywhere you like.

- Requirements:
  - .NET >= 4.6.2 or Windows version >= 10 1607 (Anniversary Update)

Usage
-----

- Just run the `Paletteau.exe` file in the archive.
- Launch: <kbd>Alt</kbd>+<kbd>q</kbd>
- Cancel/Return: <kbd>Esc</kbd>

Setting
-------

1. Right click on the tray icon of Paleteau, click `Settings`.
2. In `Plugin` tab, you can find the `Palette` plugin. It holds palettes for all programs.
3. In the configuration page of the plugin, get the path of configuration file, open and directly edit it.
4. After editing, press the `Reload Configuration` button to reload palettes.

The format of configuration looks like the following:

```json
{
  "palettes": {
    "explorer": [
      {"action": "^w", "description": "Close Windows Explorer", "type": "key"},
    ],
    "cloudmusic": [
      {"action": "^p", "description": "Play / Pause", "type":  "key"},
      {"action": "^{LEFT}", "description": "Previous music", "type":  "key"},
    ],
    "msedge": [
      {"action": "^+,", "description": "Toggle vertical tabs", "type": "key"},
      {"action": "^+a", "description": "Search tabs", "type": "key"},
    ]
  }
}
```

To add palette for a new program, just add a new item in `palettes` section, with key being the **process name** and value the list of **actions**.
An **action** is a dictionary of three keys: `action`, `description` and `type`, where
- `action` is the binding keyboard shortcut, according to the [`SendKeys` format](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys.send?view=windowsdesktop-6.0).
	- To send a sequence of keys, use `|` as the separator. For example, `^p|%a|+j` means sending `ctrl-p`, then `alt-a`, then `shift-j`.
- `description` is the description of the shortcut. Paletteau filters actions by descriptions.
- `type` is reserved for furtuer use. It could only be `"key"` for now.

Call for Contribution
------------

- Star me please!
- Any PR / issue is welcomed!
- Meaningful work items for PRs:
  - Palettes for more programs!
  - Theme adaption. Only dark theme is tuned for Paletteau for now.
  - Installer project.

Build
-----

Use Visual Studio 2022 with .NET desktop development and UWP development

Thanks
------

- Thanks [Wox](https://github.com/Wox-launcher/Wox) for their wonderful work!
