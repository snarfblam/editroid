using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Common
{
    /// <summary>
    /// Provides management for single instance forms.
    /// </summary>
    static class Forms<T> where T:Form, new()
    {
        static T instance;
        public static T Instance {
            get { 
                if(Forms.AutoInstantiate)
                    Load();
                return instance; 
            }
        }

        public static Form MainForm { get { return Forms.MainForm; } set { Forms.MainForm = value; } }
        public static bool AutoInstantiate { get { return Forms.AutoInstantiate; } set { Forms.AutoInstantiate = value; } }


        public static bool IsLoaded {
            get { return instance != null && !instance.IsDisposed; }
            set {
                if (value)
                    Load();

            }
        }

        public static void Load() {
            if (!IsLoaded) {
                instance = new T();
                instance.Parent = Forms.MainForm;
            }
        }

        public static void Show() { Instance.Show(); }
        public static void Hide() { Instance.Hide(); }
        public static void Close() { Instance.Close(); }
    }
    internal static class Forms
    {
        private static Form mainForm;
        public static Form MainForm { get { return mainForm; } set { mainForm = value; } }

        private static bool autoInstantiate = true;
        public static bool AutoInstantiate { get { return autoInstantiate; } set { autoInstantiate = value; } }
        
    }
}
