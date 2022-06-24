using Paletteau.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Palette
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : UserControl, INotifyPropertyChanged
    {
        public Setting setting;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        public string SettingDirectory
        {
            get
            {
                return setting.filename;
            }
        }

        public string LastUpdated
        {
            get
            {
                return setting.lastLoadTime.ToString("MM/dd/yyyy HH:mm:ss");
            }
        }

        public string ErrorMessage
        {
            get
            {
                return setting.lastException?.ToString();
            }
        }

        public SettingWindow(Setting _setting)
        {
            setting = _setting;
            InitializeComponent();
            DataContext = this;
        }

        private void OnSettingDirectoryClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Process.Start(SettingDirectory);
            }
        }

        private void OnReloadConfigurationClick(object sender, RoutedEventArgs e)
        {
            setting.ReloadSetting();
            NotifyPropertyChanged(nameof(ErrorMessage));
            NotifyPropertyChanged(nameof(LastUpdated));
        }
    }
}
