using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public static class AppSettings
    {
        private static IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;

        static AppSettings()
        {
            if (Settings.Contains("Version") == false)
            {
                UstawDomsylneUstawienia();
            }
        }

        public static void StoreSetting(string settingName, string value)
        {
            StoreSetting<string>(settingName, value);
        }

        public static void StoreSetting<TValue>(string settingName, TValue value)
        {
            if (!Settings.Contains(settingName))
                Settings.Add(settingName, value);
            else
                Settings[settingName] = value;

            Settings.Save();
        }

        public static bool TryGetSetting<TValue>(string settingName, out TValue value)
        {
            if (Settings.Contains(settingName))
            {
                value = (TValue)Settings[settingName];
                return true;
            }

            if (Settings.Contains(settingName))
            {
                value = (TValue)Settings[settingName];
                return true;
            }
            
            value = default(TValue);
            return false;
        }

        public static void UstawDomsylneUstawienia()
        {
            Settings["Version"] = 1.0;
            Settings["HighlightNumbers"] = true;
            Settings["HighlightRowColumn"] = true;
            Settings["ShowErrors"] = true;
            Settings["Difficulty"] = 40;

            Settings["PreviousGame"] = null;
            Settings["PreviousGameCompleteBoard"] = null;
            Settings["PreviousGameEditableFields"] = null;
            Settings["PreviousGameEditableFields"] = null;
            Settings["PreviousGameTime"] = new TimeSpan();
        }
    }
}
