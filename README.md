Paletteau
===

**Paletteau** aims to provide interactive command palette for Windows programs, which do not come with such functionality.
**Paletteau** is a fork of [Wox](https://github.com/Wox-launcher/Wox).

![demo](https://i.imgur.com/DfuhETR.gif)

Features
--------

- Faster filtering than Wox, powered by a new dynamic programming algorithm.
- Use *pinyin* to search for programs / 支持用 **拼音** 搜索程序
  - wyy / wangyiyun → 网易云音乐
- Customizable themes
- Develop plugins yourself
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
- `description` is the description of the shortcut. Paletteau filters actions by descriptions.
- `type` is reserved for furtuer use. It could only be `"key"` for now.

Call for Contribution
------------

- Star me please!
- Any PR / issue is welcomed!
- Meaningful work items for PRs:
  - Palettes for more programs!
  - Installer project.

Build
-----

Use Visual Studio 2022 with .NET desktop development and UWP development

Thanks
------

- Thanks [Wox](https://github.com/Wox-launcher/Wox) for their wonderful work!
