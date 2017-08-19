//using System;
//using System.IO;
//using Editroid;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Windows.Forms;
//using System.Security;
//using EditroidPlug;
//using System.Drawing;

//public static class PluginManager
//{

//    static List<EditroidPluginAttribute> _plugins = new List<EditroidPluginAttribute>();
    
//    private static bool Loaded = false;
//    public static void LoadPlugins() {
//        Plugins = _plugins.AsReadOnly();

//        if (Loaded) throw new InvalidOperationException("Attempted to load plugins multiple times.");
//        Loaded = true;

//        if (!Directory.Exists(Program.PluginDirectory)) return;


//        string[] dlls = Directory.GetFiles(Program.PluginDirectory, "*.dll");
//        string[] exes = Directory.GetFiles(Program.PluginDirectory, "*.exe");

//        List<string> assemblies = new List<string>();
//        assemblies.AddRange(dlls);
//        assemblies.AddRange(exes);

//        for (int i = 0; i < assemblies.Count; i++) {
//            Assembly assm = null;
//            try {
//                assm = Assembly.LoadFrom(assemblies[i]);
//            } catch (FileNotFoundException ex) {
//                // Ignore. Stupid exceptions.
//            } catch (ArgumentException ex) {
//                // Ignore. Stupid exceptions.
//            } catch (BadImageFormatException ex) {
//                // Ignore. This exception would typically be thrown for an
//                // unmanaged DLL needed by a plugin.
//            } catch (FileLoadException ex) {
//                // I don't understand this one. Catch-all? Or IO-related error?
//                MessageBox.Show("A FileLoadException occurred while loading a plugin (" + ex.Message + "): " + Environment.NewLine + assemblies[i]);
//            } catch (SecurityException ex) {
//                // I don't understand this one. Catch-all? Or IO-related error?
//                MessageBox.Show("A SecurityException occurred while loading a plugin (" + ex.Message + "): " + Environment.NewLine + assemblies[i]);
//            } catch (PathTooLongException ex) {
//                // I don't understand this one. Catch-all? Or IO-related error?
//                MessageBox.Show("A PathTooLongException occurred while loading a plugin (" + ex.Message + "): " + Environment.NewLine + assemblies[i]);
//            }

//            if (assm != null) LoadAssemblyPlugins(assm);
//        }
//    }

//    private static void LoadAssemblyPlugins(Assembly assm) {
//        object[] atts = assm.GetCustomAttributes(typeof(EditroidPlug.EditroidPluginAttribute),false);

//        for (int i = 0; i < atts.Length; i++) {
//            var attribute = atts[i] as EditroidPluginAttribute;

//            if (attribute != null) _plugins.Add(attribute);
//        }
//    }

//    public static IList<EditroidPluginAttribute> Plugins { get; private set; }
//}

//public class RomProxy : IRom
//{
//    public MetroidRom Rom { get; set; }

//    private void CheckRomLoaded() {
//        if (Rom == null) throw new InvalidOperationException("ROM is not loaded.");
//    }

//    RomLevel ConvertIndex(LevelIndex index) {
//        switch (index) {
//            case LevelIndex.Brinstar:
//                return RomLevel.Brinstar;
//            case LevelIndex.Norfair:
//                return RomLevel.Norfair;
//            case LevelIndex.Tourian:
//                return RomLevel.Tourian;
//            case LevelIndex.Kraid:
//                return RomLevel.Kraid;
//            case LevelIndex.Ridley:
//                return RomLevel.Ridley;
//            case LevelIndex.None:
//                return RomLevel.Title;
//            default:
//                throw new ArgumentException("Invalid level index specified.");
//        }
//    }
//    LevelIndex ConvertIndex(RomLevel index) {
//        switch (index) {
//            case RomLevel.Brinstar:
//                return LevelIndex.Brinstar;
//            case RomLevel.Norfair:
//                return LevelIndex.Norfair;
//            case RomLevel.Tourian:
//                return LevelIndex.Tourian;
//            case RomLevel.Kraid:
//                return LevelIndex.Kraid;
//            case RomLevel.Ridley:
//                return LevelIndex.Ridley;
//            case RomLevel.Title:
//                return LevelIndex.None;
//            default:
//                throw new ArgumentException("Invalid level index specified.");
//        }
//    }
//    #region IRom Members

//    public RomFormat Format {
//        get {
//            CheckRomLoaded();

//            switch (Rom.RomFormat) {
//                case RomFormats.Standard:
//                    return RomFormat.Normal;
//                case RomFormats.Expando:
//                    return RomFormat.Expanded;
//                case RomFormats.Enhanco:
//                    return RomFormat.Enhanced;
//                default:
//                    return RomFormat.Normal; // lazy
//            }
//        }
//    }



//    public System.Drawing.Bitmap GetLevelBgTiles(RomLevel level) {
//        CheckRomLoaded();

//        if (level == RomLevel.Title) {
//            // Todo: create MetroidRom.GetTitlePatterns

//            if (Rom.RomFormat != RomFormats.Enhanco) throw new NotSupportedException("Editroid can not currently construct title screen tile sheet for unenhanced ROMs.");

//            PatternTable titlePatterns = new PatternTable(true);
//            int chrPage = Rom.ChrUsage.GetBgFirstPage(ConvertIndex(level));

//            int offset = 0x40010 + 0x1000 * chrPage;
//            titlePatterns.BeginWrite();
//            titlePatterns.LoadTiles(Rom.data, offset, 0, 256);
//            titlePatterns.EndWrite();

//            return titlePatterns.PatternImage;
//        } else {
//            return (Bitmap)Rom.Levels[ConvertIndex(level)].Patterns.PatternImage.Clone();
//        }
//    }

//    public System.Drawing.Bitmap GetLevelSpriteTiles(RomLevel level) {
//        throw new NotImplementedException();
//    }

//    #endregion
//}

//class PluginHost
//{
//    List<ProgramProxy> loadedPlugins = new List<ProgramProxy>();
//    public frmMain MainForm { get; private set; }
//    public RomProxy RomHelper { get; private set; }

//    public PluginHost(frmMain mainform) {
//        this.MainForm = mainform;
//        mainform.RomLoaded += new EventHandler(mainform_RomLoaded);
//        mainform.RomSaved += new EventHandler(mainform_RomSaved);
//        mainform.RomSaving += new System.ComponentModel.CancelEventHandler(mainform_RomSaving);

//        RomHelper = new RomProxy();
//    }

//    void mainform_RomSaving(object sender, System.ComponentModel.CancelEventArgs e) {
//        foreach (var plug in loadedPlugins) {
//            plug.onRomSaving();
//        }
//    }

//    void mainform_RomSaved(object sender, EventArgs e) {
//        foreach (var plug in loadedPlugins) {
//            plug.onRomSaved();
//        }
//    }

//    void mainform_RomLoaded(object sender, EventArgs e) {
//        RomHelper.Rom = MainForm.GameRom;

//        foreach (var plug in loadedPlugins) {
//            plug.onRomLoaded();
//        }
//    }

//    internal void InstantiatePlugin(EditroidPluginAttribute plugInfo) {
//        bool InheritsForm = typeof(Form).IsAssignableFrom(plugInfo.PluginType);
//        bool IsPlugin = typeof(IPlugin).IsAssignableFrom(plugInfo.PluginType);

//        if (InheritsForm && IsPlugin) {
//            Form pluginUI = (Form)TryCreateInstance(plugInfo.PluginType);
//            if (pluginUI == null) return;
            

//            pluginUI.Owner = MainForm; // Todo: make this configurable by the plugin
//            ProgramProxy prox = new ProgramProxy((IPlugin)pluginUI, plugInfo, this);
//            loadedPlugins.Add(prox);

//            prox.Initialize();

//            if (MainForm.GameRom != null) {
//                prox.onRomLoaded();
//            }

//            if (plugInfo.PluginUI == PluginUIType.Dialog) {
//                // For modal dialogs, teardown is all done here.
//                pluginUI.ShowDialog();
//                UnloadPlugin(pluginUI);

//                if (plugInfo.ReloadOnClose) {
//                    MainForm.ReloadCurrentRomFromMemory();
//                }
//            } else {
//                // For non-modal plugins, teardown must be deferred until plugin closed (by user, by self, or automatically [e.g. current rom is unloaded]).
//                pluginUI.FormClosed += new FormClosedEventHandler(NonmodalPlugin_FormClosed);
//                pluginUI.Show();
//            }
//        } else {
//            MessageBox.Show("Could not load the plugin (wrong UI type or does not support the plugin interface).");
//        }
//    }

//    void NonmodalPlugin_FormClosed(object sender, FormClosedEventArgs e) {
//        throw new NotImplementedException();
//        // todo: if plugin is non-modal, UnloadPlugin() must disconnect FormClosed handler
//    }

//    private void UnloadPlugin(Control UI) {
//        ProgramProxy prox = GetAssociatedProxy(UI);
//        if (prox == null) throw new InvalidOperationException("Attempted to unload plugin that was not found in plugin list.");

//        bool nonModal = prox.PlugInfo.PluginUI == PluginUIType.Window;
//        if (nonModal && UI is Form) {
//            ((Form)UI).FormClosed -= NonmodalPlugin_FormClosed;
//        }

//        loadedPlugins.Remove(prox);
//        prox.Dispose();
//        UI.Dispose();
//    }

//    private ProgramProxy GetAssociatedProxy(Control UI) {
//        return loadedPlugins.Find(prox => prox.Plugin == UI);
//    }

//    /// <summary>
//    /// Instantiates the type, or shows an error message to the user.
//    /// </summary>
//    /// <param name="type"></param>
//    /// <returns></returns>
//    private object TryCreateInstance(Type type) {
//        try {
//            return Activator.CreateInstance(type);
//        } catch (TargetInvocationException) {
//            MessageBox.Show("An error occurred in the plugin while loading.");
//        } catch (MissingMethodException) {
//            MessageBox.Show("Could not load the plugin (no default constructor).");
//        } catch (MethodAccessException) {
//            MessageBox.Show("Could not load the plugin (no public default constructor).");
//        } catch (MemberAccessException) {
//            MessageBox.Show("Could not load the plugin (no public default constructor or the plugin class was abstract or invalid).");
//        }

//        // Other unanticipated exceptions are not handled.

//        return null;
//    }
//}

//class ProgramProxy : IPlugHost, IDisposable
//{
//    public ProgramProxy(IPlugin plugin, EditroidPluginAttribute info, PluginHost host) {
//        this.Plugin = plugin;
//        this.Host = host;
//        this.PlugInfo = info;
//    }

//    internal void Initialize() {
//        Plugin.Initialize(this);
//    }

//    public event EventHandler RomLoaded;
//    internal void onRomLoaded() { RomLoaded.Raise(this); }

//    public event EventHandler RomSaved;
//    internal void onRomSaved() { RomSaved.Raise(this); }

//    public event EventHandler RomSaving;
//    internal void onRomSaving() { RomSaving.Raise(this); }

//    public EditroidPluginAttribute PlugInfo { get; private set; }
//    public IPlugin Plugin { get; private set; }
//    public PluginHost Host { get; private set; }
//    frmMain MainForm { get { return Host.MainForm; } }

//    public string RomPath {
//        get { return MainForm.RomPath; }
//    }

//    public byte[] RomImage {
//        get {
//            if (MainForm.GameRom == null) return null;
//            return MainForm.GameRom.data;
//        }
//    }

//    public IRom RomHelper {
//        get { return Host.RomHelper; }
//    }

//    public bool HasUnsavedChanges {
//        get { return MainForm.GameRom.CompareToSavedVersion().Count == 0; }
//    }

//    public void ClearUndo() {
//        MainForm.ClearUndoQueue();
//    }

//    public bool SaveRom() {
//        return MainForm.Plugin_SaveRom();
//    }

//    #region IDisposable Members

//    public void Dispose() {
//        RomLoaded = RomSaved = RomSaving = null;
//        Plugin = null;
//        Host = null;
//    }

//    #endregion
//}
