﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using NHotkey;
using NHotkey.Wpf;
using Ookii.Dialogs.Wpf; // may be removed later https://github.com/dotnet/wpf/issues/438

using Paletteau.Core.Plugin;
using Paletteau.Core.Resource;
using Paletteau.Infrastructure.Hotkey;
using Paletteau.Infrastructure.UserSettings;
using Paletteau.Plugin;
using Paletteau.ViewModel;

namespace Paletteau
{
    public partial class SettingWindow
    {
        private const string StartupPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public readonly IPublicAPI _api;
        private Settings _settings;
        private SettingWindowViewModel _viewModel;

        public SettingWindow(IPublicAPI api, SettingWindowViewModel viewModel)
        {
            InitializeComponent();
            _settings = Settings.Instance;
            DataContext = viewModel;
            _viewModel = viewModel;
            _api = api;
        }

        #region General

        private void OnAutoStartupChecked(object sender, RoutedEventArgs e)
        {
            SetStartup();
        }

        private void OnAutoStartupUncheck(object sender, RoutedEventArgs e)
        {
            RemoveStartup();
        }

        public static void SetStartup()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                key?.SetValue(Infrastructure.Constant.Paletteau, Infrastructure.Constant.ExecutablePath);
            }
        }

        private void RemoveStartup()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                key?.DeleteValue(Infrastructure.Constant.Paletteau, false);
            }
        }

        public static bool StartupSet()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                var path = key?.GetValue(Infrastructure.Constant.Paletteau) as string;
                if (path != null)
                {
                    return path == Infrastructure.Constant.ExecutablePath;
                }
                else
                {
                    return false;
                }
            }
        }

        private void OnSelectPythonDirectoryClick(object sender, RoutedEventArgs e)
        {
            var dlg = new VistaFolderBrowserDialog()
            {
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                string pythonDirectory = dlg.SelectedPath;
                if (!string.IsNullOrEmpty(pythonDirectory))
                {
                    var pythonPath = Path.Combine(pythonDirectory, PluginsLoader.PythonExecutable);
                    if (File.Exists(pythonPath))
                    {
                        _settings.PluginSettings.PythonDirectory = pythonDirectory;
                        MessageBox.Show("Remember to restart Paletteau use new Python path");
                    }
                    else
                    {
                        MessageBox.Show("Can't find python in given directory");
                    }
                }
            }
        }

        #endregion

        #region Hotkey

        private void OnHotkeyControlLoaded(object sender, RoutedEventArgs e)
        {
            HotkeyControl.SetHotkey(_viewModel.Settings.Hotkey, false);
        }

        void OnHotkeyChanged(object sender, EventArgs e)
        {
            if (HotkeyControl.CurrentHotkeyAvailable)
            {
                SetHotkey(HotkeyControl.CurrentHotkey, (o, args) =>
                {
                    if (!Application.Current.MainWindow.IsVisible)
                    {
                        Application.Current.MainWindow.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Application.Current.MainWindow.Visibility = Visibility.Hidden;
                    }
                });
                RemoveHotkey(_settings.Hotkey);
                _settings.Hotkey = HotkeyControl.CurrentHotkey.ToString();
            }
        }

        void SetHotkey(HotkeyModel hotkey, EventHandler<HotkeyEventArgs> action)
        {
            string hotkeyStr = hotkey.ToString();
            try
            {
                HotkeyManager.Current.AddOrReplace(hotkeyStr, hotkey.CharKey, hotkey.ModifierKeys, action);
            }
            catch (Exception)
            {
                string errorMsg =
                    string.Format(InternationalizationManager.Instance.GetTranslation("registerHotkeyFailed"), hotkeyStr);
                MessageBox.Show(errorMsg);
            }
        }

        void RemoveHotkey(string hotkeyStr)
        {
            if (!string.IsNullOrEmpty(hotkeyStr))
            {
                HotkeyManager.Current.Remove(hotkeyStr);
            }
        }

        private void OnDeleteCustomHotkeyClick(object sender, RoutedEventArgs e)
        {
            var item = _viewModel.SelectedCustomPluginHotkey;
            if (item == null)
            {
                MessageBox.Show(InternationalizationManager.Instance.GetTranslation("pleaseSelectAnItem"));
                return;
            }

            string deleteWarning =
                string.Format(InternationalizationManager.Instance.GetTranslation("deleteCustomHotkeyWarning"),
                    item.Hotkey);
            if (
                MessageBox.Show(deleteWarning, InternationalizationManager.Instance.GetTranslation("delete"),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _settings.CustomPluginHotkeys.Remove(item);
                RemoveHotkey(item.Hotkey);
            }
        }

        private void OnnEditCustomHotkeyClick(object sender, RoutedEventArgs e)
        {
            var item = _viewModel.SelectedCustomPluginHotkey;
            if (item != null)
            {
                CustomQueryHotkeySetting window = new CustomQueryHotkeySetting(this, _settings);
                window.UpdateItem(item);
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show(InternationalizationManager.Instance.GetTranslation("pleaseSelectAnItem"));
            }
        }

        private void OnAddCustomeHotkeyClick(object sender, RoutedEventArgs e)
        {
            new CustomQueryHotkeySetting(this, _settings).ShowDialog();
        }

        #endregion

        #region Plugin

        private void OnPluginToggled(object sender, RoutedEventArgs e)
        {
            var id = _viewModel.SelectedPlugin.PluginPair.Metadata.ID;
            // used to sync the current status from the plugin manager into the setting to keep consistency after save
            _settings.PluginSettings.Plugins[id].Disabled = _viewModel.SelectedPlugin.PluginPair.Metadata.Disabled; 
        }

        private void OnPluginActionKeywordsClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var id = _viewModel.SelectedPlugin.PluginPair.Metadata.ID;
                ActionKeywords changeKeywordsWindow = new ActionKeywords(id, _settings);
                changeKeywordsWindow.ShowDialog();
            }
        }

        private void OnPluginNameClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var website = _viewModel.SelectedPlugin.PluginPair.Metadata.Website;
                if (!string.IsNullOrEmpty(website))
                {
                    var uri = new Uri(website);
                    if (Uri.CheckSchemeName(uri.Scheme))
                    {
                        Process.Start(website);
                    }
                }
            }
        }

        private void OnPluginDirecotyClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var directory = _viewModel.SelectedPlugin.PluginPair.Metadata.PluginDirectory;
                if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                {
                    Process.Start(directory);
                }
            }
        }
        #endregion

        #region Proxy

        private void OnTestProxyClick(object sender, RoutedEventArgs e)
        { // TODO: change to command
            var msg = _viewModel.TestProxy();
            MessageBox.Show(msg); // TODO: add message box service
        }

        #endregion

        private void OnCheckUpdates(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateApp(); // TODO: change to command
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _viewModel.Save();
            PluginManager.Save();
        }

        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
