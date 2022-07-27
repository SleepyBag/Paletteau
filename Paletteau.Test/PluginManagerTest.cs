using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Paletteau.Core;
using Paletteau.Core.Configuration;
using Paletteau.Core.Plugin;
using Paletteau.Infrastructure;
using Paletteau.Image;
using Paletteau.Infrastructure.UserSettings;
using Paletteau.Plugin;
using Paletteau.ViewModel;

namespace Paletteau.Test
{
    [TestFixture]
    class PluginManagerTest
    {
        [OneTimeSetUp]
        public void setUp()
        {
            // todo remove i18n from application / ui, so it can be tested in a modular way
            new App();
            Settings.Initialize();
            ImageLoader.Initialize();

            Portable portable = new Portable();
            SettingWindowViewModel settingsVm = new SettingWindowViewModel(portable);

            StringMatcher stringMatcher = new StringMatcher();
            StringMatcher.Instance = stringMatcher;
            // stringMatcher.UserSettingSearchPrecision = Settings.Instance.QuerySearchPrecision;

            PluginManager.LoadPlugins(Settings.Instance.PluginSettings);
            MainViewModel mainVm = new MainViewModel(false);
            PublicAPIInstance api = new PublicAPIInstance(settingsVm, mainVm);
            PluginManager.InitializePlugins(api);

        }

        [TestCase("setting", "Settings")]
        [TestCase("netwo", "Network and Sharing Center")]
        public void BuiltinQueryTest(string QueryText, string ResultTitle)
        {
            
            Query query = QueryBuilder.Build(QueryText.Trim(), null, null, PluginManager.NonGlobalPlugins);
            List<PluginPair> plugins = PluginManager.AllPlugins;
            Result result = plugins.SelectMany(
                    p => PluginManager.QueryForPlugin(p, query)
                )
                .OrderByDescending(r => r.Score)
                .First();

            Assert.IsTrue(result.Title.StartsWith(ResultTitle));
        }
    }
}
