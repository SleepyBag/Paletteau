﻿using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Paletteau.Core.Configuration;
using Paletteau.Core.Plugin;
using Paletteau.Image;
using Paletteau.Infrastructure;
using Paletteau.Infrastructure.UserSettings;
using Paletteau.Plugin;
using Paletteau.ViewModel;

namespace Paletteau.Test
{
    [TestFixture]
    class PluginProgramTest
    {

        private Plugin.Program.Main plugin;

        [OneTimeSetUp]
        public void Setup()
        {
            Settings.Initialize();
            Portable portable = new Portable();
            SettingWindowViewModel settingsVm = new SettingWindowViewModel(portable);
            StringMatcher stringMatcher = new StringMatcher();
            StringMatcher.Instance = stringMatcher;
            stringMatcher.UserSettingSearchPrecision = Settings.Instance.QuerySearchPrecision;
            PluginManager.LoadPlugins(Settings.Instance.PluginSettings);
            MainViewModel mainVm = new MainViewModel(false);
            PublicAPIInstance api = new PublicAPIInstance(settingsVm, mainVm);

            plugin = new Plugin.Program.Main();
            plugin.InitSync(new PluginInitContext()
            {
                API = api,
            });
        }

        //[TestCase("powershell", "Windows PowerShell")] skip for appveyror
        [TestCase("note", "Notepad")]
        [TestCase("computer", "computer")]
        public void Win32Test(string QueryText, string ResultTitle)
        {
            Query query = QueryBuilder.Build(QueryText.Trim(), new Dictionary<string, PluginPair>());
            Result result = plugin.Query(query).OrderByDescending(r => r.Score).First();
            Assert.IsTrue(result.Title.StartsWith(ResultTitle));
        }
    }
}
