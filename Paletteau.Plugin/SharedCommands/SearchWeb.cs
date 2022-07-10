using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Paletteau.Infrastructure
{
    public static class SearchWeb
    {
        /// <summary> 
        /// Opens search in a new browser. If no browser path is passed in then Chrome is used. 
        /// Leave browser path blank to use Chrome.
        /// </summary>
		public static void NewBrowserWindow(this string url, string exePath, string browserName)
        {
            string exeArgs = url;
            switch (browserName)
            {
                case "msedge":
                case "chrome":
                case "chromium":
                    exeArgs = "--new-window " + url;
                    break;
                case "firefox":
                    exeArgs = "-new-window " + url;
                    break;
            }

            try
            {
                Process.Start(exePath, exeArgs);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Process.Start(url);
            }
        }

        /// <summary> 
        /// Opens search as a tab in the default browser chosen in Windows settings.
        /// </summary>
        public static void NewTabInBrowser(this string url, string exePath, string browserName)
        {
            try
            {
                if (!string.IsNullOrEmpty(exePath))
                {
                    Process.Start(exePath, url);
                }
                else
                {
                    Process.Start(url);
                }
            }
            // This error may be thrown for Process.Start(browserPath, url)
            catch (System.ComponentModel.Win32Exception)
            {
                Process.Start(url);
            }
        }
    }
}
