﻿using System;
using System.Collections.Generic;

namespace Paletteau.Plugin
{
    /// <summary>
    /// Public APIs that plugin can use
    /// </summary>
    public interface IPublicAPI
    {
        /// <summary>
        /// Push result to query box
        /// </summary>
        /// <param name="query"></param>
        /// <param name="plugin"></param>
        /// <param name="results"></param>
        [Obsolete("This method will be removed in Paletteau 1.4")]
        void PushResults(Query query, PluginMetadata plugin, List<Result> results);

        /// <summary>
        /// Change Paletteau query
        /// </summary>
        /// <param name="query">query text</param>
        /// <param name="requery">
        /// force requery By default, Paletteau will not fire query if your query is same with existing one. 
        /// Set this to true to force Paletteau requerying
        /// </param>
        void ChangeQuery(string query, bool requery = false);

        /// <summary>
        /// Just change the query text, this won't raise search
        /// </summary>
        /// <param name="query"></param>
        [Obsolete]
        void ChangeQueryText(string query, bool selectAll = false);

        /// <summary>
        /// Close Paletteau
        /// </summary>
        [Obsolete]
        void CloseApp();

        /// <summary>
        /// Restart Paletteau
        /// </summary>
        void RestarApp();

        /// <summary>
        /// Hide Paletteau
        /// </summary>
        [Obsolete]
        void HideApp();

        /// <summary>
        /// Show Paletteau
        /// </summary>
        [Obsolete]
        void ShowApp();

        /// <summary>
        /// Save all Paletteau settings
        /// </summary>
        void SaveAppAllSettings();

        /// <summary>
        /// Reloads any Plugins that have the 
        /// IReloadable implemented. It refeshes
        /// Plugin's in memory data with new content
        /// added by user.
        /// </summary>
        void ReloadAllPluginData();

        /// <summary>
        /// Check for new Paletteau update
        /// </summary>
        void CheckForNewUpdate();

        /// <summary>
        /// Show message box
        /// </summary>
        /// <param name="title">Message title</param>
        /// <param name="subTitle">Message subtitle</param>
        /// <param name="iconPath">Message icon path (relative path to your plugin folder)</param>
        void ShowMsg(string title, string subTitle = "", string iconPath = "");

        /// <summary>
        /// Show message box
        /// </summary>
        /// <param name="title">Message title</param>
        /// <param name="subTitle">Message subtitle</param>
        /// <param name="iconPath">Message icon path (relative path to your plugin folder)</param>
        /// <param name="useMainWindowAsOwner">when true will use main windows as the owner</param>
        void ShowMsg(string title, string subTitle, string iconPath, bool useMainWindowAsOwner = true);

        /// <summary>
        /// Open setting dialog
        /// </summary>
        void OpenSettingDialog();

        /// <summary>
        /// Show loading animation
        /// </summary>
        [Obsolete("automatically start")]
        void StartLoadingBar();

        /// <summary>
        /// Stop loading animation
        /// </summary>
        [Obsolete("automatically stop")]
        void StopLoadingBar();

        /// <summary>
        /// Install Paletteau plugin
        /// </summary>
        /// <param name="path">Plugin path (ends with .wox)</param>
        void InstallPlugin(string path);

        /// <summary>
        /// Get translation of current language
        /// You need to implement IPluginI18n if you want to support multiple languages for your plugin
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetTranslation(string key);

        /// <summary>
        /// Get all loaded plugins 
        /// </summary>
        /// <returns></returns>
        List<PluginPair> GetAllPlugins();

        /// <summary>
        /// Fired after global keyboard events
        /// if you want to hook something like Ctrl+R, you should use this event
        /// </summary>
        event WoxGlobalKeyboardEventHandler GlobalKeyboardEvent;
    }
}
