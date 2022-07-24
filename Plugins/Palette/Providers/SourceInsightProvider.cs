﻿using Paletteau.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Palette.Providers
{
    internal class SourceInsightProvider : IPaletteProvider
    {
        SourceInsightSetting sourceInsightSetting;

        public List<CommandItem> GetItems(ProcessIdentifier processIdentifier)
        {
            return sourceInsightSetting.commandTable.Select(
                commandEntry => new CommandItem
                {
                    Title = commandEntry.Key,
                    SubTitle = commandEntry.Value,
                    Action = (_, process) =>
                    {
                        Task.Run(() =>
                            {
                                Process.Start(process.MainModule.FileName, "-i -c \"" + commandEntry.Key + "\"");
                            }
                        );
                        return true;
                    }
                }
            ).ToList();
        }

        public void Init(ProviderContext providerContext)
        {
            sourceInsightSetting = SourceInsightSetting.LoadSourceInsightSetting();
            providerContext.commandTable.RegisterProvider(new ProcessIdentifier("sourceinsight4"), this);
        }

        public bool IsUpdated()
        {
            // read once and be static
            return false;
        }

        internal class SourceInsightSetting
        {
            // default command list
            public Dictionary<string, string> commandTable = new Dictionary<string, string>
            {
                ["About Source Insight"] = null, ["Activate Menu Commands"] = null, ["Activate Clip Window"] = null, ["Activate FTP Browser"] = null, ["Activate Project File List"] = null, ["Activate Project Search Bar"] = null, ["Activate Project Symbol List"] = null, ["Activate Project Window"] = null, ["Activate Relation Window"] = null, ["Activate Search Bar"] = null, ["Activate Search Results"] = null, ["Activate Snippet Window"] = null, ["Activate Symbol Window"] = null, ["Activate Window List"] = null, ["Add and Remove Project Files"] = null, ["Add File"] = null, ["Add File List"] = null, ["Advanced Options"] = null, ["Arrange Windows"] = null, ["Arrangement Toolbar"] = null, ["Back Tab"] = null, ["Backspace"] = null, ["Beginning of Line"] = null, ["Beginning of Selection"] = null, ["Blank Line Down"] = null, ["Blank Line Up"] = null, ["Block Down"] = null, ["Block Up"] = null, ["Bookmark"] = null, ["Bookmark Window"] = null, ["Bookmark Options"] = null, ["Bottom of File"] = null, ["Bottom of Window"] = null, ["Browse Files"] = null, ["Browse Project Symbols"] = null, ["Browse Global Symbols Dialog box"] = null, ["Browse Local File Symbols"] = null, ["Browser Mode"] = null, ["Calculate"] = null, ["Cascade Windows"] = null, ["Check for Updates"] = null, ["Checkpoint"] = null, ["Checkpoint All"] = null, ["Clear Highlights"] = null, ["Clip Properties"] = null, ["Clip Window"] = null, ["Clip Window Options"] = null, ["Close"] = null, ["Close All"] = null, ["Close Project"] = null, ["Close Window"] = null, ["Color Options"] = null, ["Command Shell"] = null, ["Compare Files"] = null, ["Compare with Backup File"] = null, ["Complete Snippet"] = null, ["Complete Symbol"] = null, ["Conditional Parsing List"] = null, ["Context Window"] = null, ["Context Window Options"] = null, ["Copy"] = null, ["Copy File Path"] = null, ["Copy Line"] = null, ["Copy Line Right"] = null, ["Copy List"] = null, ["Copy Project"] = null, ["Copy Symbol"] = null, ["Copy To Clip"] = null, ["Create Key List"] = null, ["Create Bookmarks from Relation Items"] = null, ["Create Command List"] = null, ["Cursor Down"] = null, ["Cursor Left"] = null, ["Cursor Right"] = null, ["Cursor Up"] = null, ["Custom Commands"] = null, ["Cut"] = null, ["Cut Line"] = null, ["Cut Line Left"] = null, ["Cut Line Right"] = null, ["Cut Selection or Paste"] = null, ["Cut Symbol"] = null, ["Cut To Clip"] = null, ["Cut Word"] = null, ["Cut Word Left"] = null, ["Deactivate License"] = null, ["Delete"] = null, ["Delete All Clips"] = null, ["Delete Character"] = null, ["Delete Clip"] = null, ["Delete File"] = null, ["Delete Line"] = null, ["Directory Compare"] = null, ["Directory Compare Options"] = null, ["Display Options"] = null, ["Drag Line Down"] = null, ["Drag Line Down More"] = null, ["Drag Line Up"] = null, ["Drag Line Up More"] = null, ["Duplicate"] = null, ["Duplicate Symbol"] = null, ["Edit Condition"] = null, ["Enable Event Handlers"] = null, ["End of Line"] = null, ["End of Selection"] = null, ["Exit"] = null, ["Exit and Suspend"] = null, ["Expand"] = null, ["Expand All"] = null, ["Expand Text Variables"] = null, ["Expand Special"] = null, ["Export File as HTML"] = null, ["Export Project File List"] = null, ["Export Project To HTML"] = null, ["File Compare"] = null, ["File Compare Window Options"] = null, ["File Options"] = null, ["File Search Bar"] = null, ["File Search Bar Options"] = null, ["File Type Options"] = null, ["Folder Options"] = null, ["FTP Browser"] = null, ["FTP Browser Options"] = null, ["FTP Site List"] = null, ["Edit FTP Site Properties"] = null, ["Full Screen (F11)"] = null, ["Function Down"] = null, ["Function Up"] = null, ["General Options"] = null, ["Generate Call Tree"] = null, ["Go Back"] = null, ["Go Back Toggle"] = null, ["Go Forward"] = null, ["Go To First Link"] = null, ["Go To Line"] = null, ["Go To Next Change"] = null, ["Go To Previous Change"] = null, ["Go To Next Link"] = null, ["Go To Previous Link"] = null, ["Go To Next Reference Highlight"] = null, ["Go To Previous Reference Highlight"] = null, ["Help"] = null, ["Help Mode"] = null, ["Highlight Word"] = null, ["Incremental Search"] = null, ["Incremental Search Mode"] = null, ["Incremental Search Backward"] = null, ["Horizontal Scroll Bar"] = null, ["HTML Help"] = null, ["Import External Symbols"] = null, ["Import External Symbols for Current Project"] = null, ["Indent Left"] = null, ["Indent Right"] = null, ["Insert ASCII"] = null, ["Insert File"] = null, ["Insert GUID"] = null, ["Insert Line"] = null, ["Insert Line Before Next"] = null, ["Insert New Line"] = null, ["Join Lines"] = null, ["Jump To Base Type"] = null, ["Jump To Caller"] = null, ["Jump To Definition"] = null, ["Jump To Link"] = null, ["Jump To Prototype"] = null, ["Key Assignments"] = null, ["Language Options"] = null, ["Language Properties"] = null, ["Last Window (Ctrl+Tab) or (Ctrl+Shift+Tab)"] = null, ["Layout Toolbar"] = null, ["Line Numbers"] = null, ["Link All Windows"] = null, ["Link Window"] = null, ["Load Configuration"] = null, ["Load File"] = null, ["Load Layout"] = null, ["Load Search String"] = null, ["Lock Context Window"] = null, ["Lock Relation Window"] = null, ["Logging Options"] = null, ["Lookup References"] = null, ["Make Column Selection"] = null, ["Manage License"] = null, ["Manage Visual Themes"] = null, ["Menu Assignments"] = null, ["Mono Font View"] = null, ["New"] = null, ["New Clip"] = null, ["New Relation Window"] = null, ["New Project"] = null, ["New Window"] = null, ["Next File"] = null, ["Next Relation Window View"] = null, ["Open"] = null, ["Open As Encoding"] = null, ["Open Backup File"] = null, ["Open Project"] = null, ["Outline Toolbar"] = null, ["Outlining Options"] = null, ["Overview Options"] = null, ["Page Down"] = null, ["Page Setup"] = null, ["Page Up"] = null, ["Paren Left"] = null, ["Paren Right"] = null, ["Parse Source Links"] = null, ["Paste"] = null, ["Paste From Clip"] = null, ["Paste Line"] = null, ["Paste Symbol"] = null, ["Pick Window"] = null, ["Play Recording"] = null, ["Preferences"] = null, ["Print"] = null, ["Print Relation Window"] = null, ["Project File Types"] = null, ["Project File Type List Properties"] = null, ["Project Folder Browser"] = null, ["Project Folder Browser Options"] = null, ["Project File List"] = null, ["Project File List Options"] = null, ["Project Search Bar"] = null, ["Project Rebuild Notice"] = null, ["Project Symbol Categories"] = null, ["Project Symbol Category Window Options"] = null, ["Project Symbol List"] = null, ["Project Symbol List Options"] = null, ["Project Settings"] = null, ["Project Report"] = null, ["Project Window command"] = null, ["Rebuild Project"] = null, ["Redo"] = null, ["Redo All"] = null, ["Redraw Screen"] = null, ["Reform Paragraph"] = null, ["Reformat Source Code Options"] = null, ["Refresh Relation Window"] = null, ["Reload As Encoding"] = null, ["Relation Window Graph Options"] = null, ["Relation Window"] = null, ["Relation Window Options"] = null, ["Reload File"] = null, ["Reload Modified Files"] = null, ["Remove File"] = null, ["Remove Project"] = null, ["Remote Options"] = null, ["Rename"] = null, ["Renumber"] = null, ["Repeat Typing"] = null, ["Replace"] = null, ["Replace Files"] = null, ["Restore File"] = null, ["Restore Lines"] = null, ["Save"] = null, ["Save A Copy"] = null, ["Save All"] = null, ["Save All Quietly"] = null, ["Save As"] = null, ["Save As Encoding"] = null, ["Save Configuration"] = null, ["Save Layout"] = null, ["Save New Backup File"] = null, ["Save Selection"] = null, ["Save Settings"] = null, ["Save Workspace"] = null, ["Scroll Bar Options"] = null, ["Scroll Half Page Down"] = null, ["Scroll Half Page Up"] = null, ["Scroll Left"] = null, ["Scroll Line Down"] = null, ["Scroll Line Up"] = null, ["Scroll Right"] = null, ["SDK Help"] = null, ["Search"] = null, ["Search Backward"] = null, ["Search Backward for Selection"] = null, ["Search Engines"] = null, ["Search Files"] = null, ["Search Forward"] = null, ["Search Forward for Selection"] = null, ["Search List"] = null, ["Searching Options"] = null, ["Search Project"] = null, ["Search Results Options"] = null, ["Search Web"] = null, ["Select All"] = null, ["Select Block"] = null, ["Select Char Left"] = null, ["Select Char Right"] = null, ["Select Function or Symbol"] = null, ["Select Line"] = null, ["Select Line Down"] = null, ["Select Line Up"] = null, ["Select Match"] = null, ["Select Next Window"] = null, ["Select Sentence"] = null, ["Select Symbol"] = null, ["Select To"] = null, ["Select To End Of File"] = null, ["Select To Top Of File"] = null, ["Select Word"] = null, ["Select Word Left"] = null, ["Select Word Right"] = null, ["Selection History"] = null, ["Setup HTML Help"] = null, ["Setup WinHelp File"] = null, ["Show Clipboard"] = null, ["Show File Status"] = null, ["Simple Tab"] = null, ["Smart End of Line"] = null, ["Smart Beginning of Line"] = null, ["Smart Rename"] = null, ["Smart Tab"] = null, ["Snippet Properties"] = null, ["Snippet Window"] = null, ["Snippet Window Options"] = null, ["Sort Symbol Window"] = null, ["Sort Symbols By Line"] = null, ["Sort Symbols by Name"] = null, ["Sort Symbols By Type"] = null, ["Source Dynamics on the Web"] = null, ["Start Recording"] = null, ["Stop Recording"] = null, ["Style Properties"] = null, ["Symbol Info"] = null, ["Symbol Lookup Options"] = null, ["Symbol Type Filter"] = null, ["Symbol Window command"] = null, ["Symbol Window Options"] = null, ["Sync File Windows"] = null, ["Synchronize Files"] = null, ["Syntax Decorations"] = null, ["Syntax Formatting"] = null, ["Syntax Keyword List"] = null, ["Tab Tray"] = null, ["Tabs to Spaces"] = null, ["Tile Horizontal"] = null, ["Tile One Window"] = null, ["Tile Two Windows"] = null, ["Tile Vertical"] = null, ["Toggle Extend Mode"] = null, ["Toggle Insert Mode"] = null, ["Top of File"] = null, ["Top of Window"] = null, ["Touch All Files in Relation"] = null, ["Typing Options"] = null, ["Undo"] = null, ["Undo All"] = null, ["Vertical Scroll Bar"] = null, ["View Clip"] = null, ["View Relation Outline"] = null, ["View Relation Window Horizontal Graph"] = null, ["View Relation Window Vertical Graph"] = null, ["View Toolbar"] = null, ["Visible Tabs"] = null, ["Visible Tabs and Spaces"] = null, ["Window List"] = null, ["Window List Options"] = null, ["Window Options"] = null, ["Window Tabs"] = null, ["Window Tab Options"] = null, ["Word Fragment Left"] = null, ["Word Fragment Right"] = null, ["Word Left"] = null, ["Word Right"] = null, ["Zoom Window"] = null,
            };

            SourceInsightSetting(XmlDocument xmlSetting)
            {
                // read custom commands
                var customCommandNodes = xmlSetting.SelectNodes("/SourceInsightConfiguration/CustomCommands/Command");
                foreach (XmlNode node in customCommandNodes)
                {
                    var attributes = node.Attributes;
                    string command = attributes["Name"].Value;
                    command = TranslateCommand(command);
                    commandTable[command] = null;
                }

                // read keymaps
                var keyMapItemNodes = xmlSetting.SelectNodes("/SourceInsightConfiguration/Keymaps//item");
                foreach (XmlNode node in keyMapItemNodes)
                {
                    var attributes = node.Attributes;
                    string command = attributes["Command"].Value;
                    string keyCode = attributes["Keycode"].Value;
                    command = TranslateCommand(command);
                    keyCode = TranslateKeyCode(keyCode);
                    commandTable[command] = keyCode;
                }
            }

            private static string TranslateCommand(string command)
            {
                return command.Trim().Trim(new char[] { '.' });
            }

            private static string TranslateKeyCode(string keyCode)
            {
                return keyCode;
            }

            public static SourceInsightSetting LoadSourceInsightSetting()
            {
                string configFilename = Path.Combine(
                    System.Environment.GetEnvironmentVariable("USERPROFILE"),
                    "Documents",
                    "Source Insight 4.0",
                    "Settings",
                    "config_all.xml"
                );
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilename);
                return new SourceInsightSetting(xmlDoc);
            }
        }
    }
}
