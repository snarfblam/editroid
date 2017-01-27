using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Reflection;

namespace Editroid
{
    class EditroidSettings
    {
        public void SaveSettings() {
            TrySaveFile();
            //TrySaveRegistry();
        }
        public void LoadSettings() {
            if (!TryLoadFile()) {
            }
             //   TryLoadRegistry();
        }

        bool TrySaveRegistry() {
            // Settings are saved to the registry incase a config file is not present
            // (for example, if the EXE is moved w/o the config file)
            try {
                using (RegistryKey settingsKey = Registry.CurrentUser.CreateSubKey("Software\\Ilab\\Editriod\\Version2")) {
                    if (settingsKey == null) return false;
                    Type SettingsType = GetType();

                    foreach (PropertyInfo prop in SettingsType.GetProperties()) {
                        settingsKey.SetValue(prop.Name, prop.GetValue(this, null).ToString());
                    }
                }

                return true;
            } catch (Exception) {
                return false;
            }
        }
        bool TrySaveFile() {
            try {
                Type SettingsType = GetType();
                List<String> settings = new List<string>();

                foreach (PropertyInfo prop in SettingsType.GetProperties()) {
                    string settingString = prop.Name + "=" + prop.GetValue(this, null).ToString();
                    settings.Add(settingString);
                }

                String filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Editroid.config");
                System.IO.File.WriteAllLines(filePath, settings.ToArray());
                return true;
            } catch (Exception) {
                // Under no circumstances should a failure to save the settings be fatal
                return false;
            }
        }
        bool TryLoadFile() {
            try {
                String filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Editroid.config");
                if (!System.IO.File.Exists(filePath)) return false;
                String[] settings = System.IO.File.ReadAllLines(filePath);
                Type SettingsType = GetType();

                foreach (string prop in settings) {
                    String[] part = prop.Split('=');
                    string name = part[0];
                    string value = part[1];

                    PropertyInfo info = SettingsType.GetProperty(name);
                    if (info != null) {
                        if (info.PropertyType == typeof(string))
                            info.SetValue(this, value, null);
                        else
                            info.SetValue(this, info.PropertyType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { value }), null);
                    }
                }

                return true;
            } catch (Exception) {
                // Under no circumstances should a failure to save the settings be fatal
                return false;
            }
        }

        bool TryLoadRegistry() {
            bool succeed = true;
            // Settings are read from the registry in the case that a config file is not present
            try {
                using (RegistryKey settingsKey = Registry.CurrentUser.OpenSubKey("Software\\Ilab\\Editriod\\Version2")) {
                    if (settingsKey == null) return false;
                    Type SettingsType = GetType();

                    foreach (PropertyInfo prop in SettingsType.GetProperties()) {
                        object value = settingsKey.GetValue(prop.Name);

                        // If a value is not present, we will complete the operation, but return a failure
                        if (value == null)
                            succeed = false;
                        else {
                            if(prop.PropertyType == typeof(string))
                                prop.SetValue(this, value.ToString(), null);
                            else
                                prop.SetValue(this, prop.PropertyType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { value.ToString() }), null);
                        }
                    }
                }

                return succeed;
            } catch (Exception x) {
                // Under no circumstances should a failure to load the backup settings be fatal
                return false;
            }
        }

        private bool useBakExtension = false;
        public bool UseBakExtension {
            get { return useBakExtension; }
            set { useBakExtension = value; }
        }

        private bool useClassicMap = false;
        public bool UseClassicMap {
            get { return useClassicMap; }
            set { useClassicMap = value; }
        }

        private bool mapInSeparateFile = false;
        public bool MapInSeparateFile {
            get { return mapInSeparateFile; }
            set { mapInSeparateFile = value; }
        }

        private int scrollRate = 1;
        public int ScrollRate {
            get { return scrollRate; }
            set { scrollRate = value; }
        }


        private string testRoomCommand = "$d";
        public string TestRoomCommand {
            get { return testRoomCommand; }
            set { testRoomCommand = value; }
        }

        bool _displayCmdForTestRoom = false;
        public bool DisplayCmdForTestRoom {
            get { return _displayCmdForTestRoom; }
            set { _displayCmdForTestRoom = value; }
        }

        private bool hasRunBefore_v3A0;
        /// <summary>Whether version 2.0 alpha has run before.</summary>
        public bool HasRunBefore_v3A0 {
            get { return hasRunBefore_v3A0; }
            set { hasRunBefore_v3A0 = value; }
        }


    }
}
