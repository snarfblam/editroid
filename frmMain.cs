using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Editroid.Properties;
using Microsoft.Win32;
using Editroid.UndoRedo;
using Editroid.ROM;
using Editroid.Actions;
using System.Threading;
using System.Diagnostics;
using Editroid.Patches;
using EditroidPlug;
using Editroid.ROM.Projects;
using System.Text;
using System.Security;

namespace Editroid
{
    internal partial class frmMain:Form, ActionGenerator.IParamSource
    {
        PluginHost plugins;
        Image userFileIcon = Resources.CodeFile;
        Image userFileAsterisk = Resources.CodeFileAsterisk;

        public frmMain() {
            // General initialization
            InitializeComponent();

            actions.Editor = this;
            undoRedo.Editor = this;

            gameView.Map = mapView;
            LoadSettings();

            LoadDefaultMap();
            mapView.SetRenderer(mapImageGenerator);

            mapMaker.RoomDrawn += new EventHandler(OnMapRoomDrawn);

            GlobalEventManager.Manager.StructureChanged += new EventHandler<LevelEventArgs>(Global_StructureChanged);
            GlobalEventManager.Manager.ComboChanged += new EventHandler<ComboEventArgs>(Global_ComboChanged);

            picTitle.BringToFront();

            undoRedo.TotalActionMaxCount = 32;
                

            gameView.Screens.SelectionDragged += new EventHandler(on_ScreenEditorSelectionDragged);
            gameView.Screens.SelectionChanged += new EventHandler(ScreenEditor_SelectedObjectChanged);
            gameView.Screens.UserDraggedSelection += new EventHandler<EventArgs<Point>>(Screens_UserDraggedSelection);
            gameView.WorldViewChanged += new EventHandler<EventArgs<Point>>(gameView_WorldViewChanged);
            gameView.Focus();

            InitializePluginMenu();

            ////ItemDataEditor.MapContrtol = mapView;

            Application.Idle += new EventHandler(Application_Idle);
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            plugins = new PluginHost(this);

            ItemEditingUI.AssociateControl(ItemUiRole.DoorTypeCombo, itDoorTypeList, ItemEditUI.ItemUiComboList.DoorList);
            ItemEditingUI.AssociateControl(ItemUiRole.PowerupCombo, itPowerupList, ItemEditUI.ItemUiComboList.PowerupTypeList);
            ItemEditingUI.AssociateControl(ItemUiRole.TurretCombo, itTurretList, ItemEditUI.ItemUiComboList.TurretList);
            ItemEditingUI.AssociateControl(ItemUiRole.RinkaCombo, itRinkaList, ItemEditUI.ItemUiComboList.RinkaList);
            ItemEditingUI.AssociateControl(ItemUiRole.ZebetiteCombo, itZebetiteList, ItemEditUI.ItemUiComboList.ZebetiteList);
            ItemEditingUI.AssociateControl(ItemUiRole.ElevatorCombo, itElevatorList, ItemEditUI.ItemUiComboList.ElevatorList);
            ItemEditingUI.AssociateControl(ItemUiRole.DifficultToggle, itDifficultToggle);
            ItemEditingUI.AssociateControl(ItemUiRole.EnemyEdit, itEnemyTypeEdit);
            ItemEditingUI.AssociateControl(ItemUiRole.RawEdit, itRawEdit);
            ItemEditingUI.AssociateControl(ItemUiRole.SlotEdit, itSlotEdit);

            ItemEditingUI.GroupControl(itEnemyTypeEdit, itEnemyTypeLabel);
            ItemEditingUI.GroupControl(itSlotEdit, itSlotLabel);
            ItemEditingUI.GroupControl(itRawEdit, itRawLabel);

            ItemEditingUI.ItemListCombo = itItemList;
            ItemEditingUI.UndoRedoQueue = undoRedo;

            ItemEditingUI.SelectedItemIndexChanged += new EventHandler(OnItemUiItemSelected);
            ////ItemEditingUI.DisplayScheme(ItemUiScheme.Elevator);

            ((ToolStripDropDownMenu)btnUndo.DropDown).ShowImageMargin = false;
            ((ToolStripDropDownMenu)btnRedo.DropDown).ShowImageMargin = false;
        }

       

        ItemEditUI ItemEditingUI = new ItemEditUI();

        private void InitializePluginMenu() {
            bool hasEditorSeparator = false;
            bool hasToolSeparator = false;

            var plugins = PluginManager.Plugins;

            if (plugins != null) {
                for (int i = 0; i < plugins.Count; i++) {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Text = plugins[i].Name;
                    item.Tag = plugins[i];
                    item.Click += new EventHandler(PluginMenuItem_Click);

                    if (plugins[i].Category == EditroidPlug.PluginCategory.Editor) {
                        if (!hasEditorSeparator) {
                            hasEditorSeparator = true;

                            mnuEditors.DropDownItems.Add(new ToolStripSeparator() { Tag = "advanced" });
                        }
                        mnuEditors.DropDownItems.Add(item);
                    } else {
                        if (!hasToolSeparator) {
                            hasToolSeparator = true;

                            mnuTools.DropDownItems.Add(new ToolStripSeparator());
                        }
                        mnuTools.DropDownItems.Add(item);
                    }
                }
            }
        }

        void PluginMenuItem_Click(object sender, EventArgs e) {
            plugins.InstantiatePlugin( (EditroidPluginAttribute) ((ToolStripItem)sender).Tag);
        }

        private void LoadDefaultMap() {
            MemoryStream m = new MemoryStream(Editroid.Properties.Resources.DefaultMap);
            mapView.TryLoadData(m, true);
            m.Close();
        }

        #region Constants
        enum PresetScrollSpeeds
        {
            Normal = 1,
            Fast = 4,
            VeryFast = 8
        }

        const string mapExtension = ".MP";
        #endregion

        #region Public Properties

        public MetroidRom GameRom { get { return gameRom; } }
        public Project GameProject { get { return gameProject; } }
        public MapControl MapView { get { return mapView; } }
        public int CurrentScreenIndex { get { return gameView.FocusedEditor.ScreenIndex; } }
        public ScreenImageGenerator MapImageGenerator { get { return mapImageGenerator; } }
        public ActionGenerator Actions { get { return actions; } }
        public ObjectInstance GetSelectedItem() {
            return ScreenEditor.SelectedItem;
        }
        public LevelIndex StructureEditorLevel {
            get {
                if (!Program.StructFormIsLoaded) return LevelIndex.None;
                return Program.StructForm.LevelIndex;
            }
        }
        #endregion

        #region Private Properties
        ScreenEditor ScreenEditor { get { return gameView.FocusedEditor; } }
        Screen CurrentScreen { get { return gameView.FocusedEditor.Screen; } }
        Level currentLevelData {
            get {
                if (gameView.FocusedEditor != null && gameView.FocusedEditor.LevelIndex != LevelIndex.None)
                    return gameRom.GetLevel(gameView.FocusedEditor.LevelIndex);
                else
                    return null;
            }
        }
        #endregion

        #region Objects

        MetroidRom gameRom;
        Project gameProject;
        ScreenImageGenerator mapImageGenerator = new ScreenImageGenerator();
        ActionGenerator actions = new ActionGenerator();
        UndoRedo.EditroidUndoRedoQueue undoRedo = new Editroid.UndoRedo.EditroidUndoRedoQueue();

        #endregion

        #region Settings
        EditroidSettings settings = new EditroidSettings();
        public EditroidSettings Settings { get { return settings; } }


        private void LoadSettings() {
            settings.LoadSettings();
            SaveMapToROM = !settings.MapInSeparateFile;
            gameView.ScrollSpeed = settings.ScrollRate;
            UseNesForBackup = !settings.UseBakExtension;
            btnDrawMap.Checked = mapView.UseScreenImages = !settings.UseClassicMap;
            btnClassicMap.Checked = settings.UseClassicMap;
            
            UpdateScrollSpeedMenu();
        }

        private void UpdateScrollSpeedMenu() {
            btnScrollnormal.Checked = gameView.ScrollSpeed == (int)PresetScrollSpeeds.Normal;
            btnScrollfast.Checked = gameView.ScrollSpeed == (int)PresetScrollSpeeds.Fast;
            btnScrollVeryFast.Checked = gameView.ScrollSpeed == (int)PresetScrollSpeeds.VeryFast;
        }

        private void SaveSettings() {
            settings.UseBakExtension = !UseNesForBackup;
            settings.ScrollRate = gameView.ScrollSpeed;
            settings.MapInSeparateFile = !SaveMapToROM;
            settings.UseClassicMap = btnClassicMap.Checked;
            settings.SaveSettings();
        }


        bool SaveMapToROM {
            get { return mnuSaveMapToROM.Checked; }
            set {
                mnuSaveMapToROM.Checked = value;
                mnuSaveMapToFile.Checked = !value;
            } 
        }
        bool UseNesForBackup {
            get {
                return mnuUseNES.Checked;
            }
            set {
                mnuUseNES.Checked = value;
                mnuUseBAK.Checked = !value;
            }
        }
        #endregion

        #region Undo
        bool undoHasMouse, redoHasMouse;
        private void btnUndo_MouseEnter(object sender, EventArgs e) {
            UndoTooltips.SetToolTip(toolbar, btnUndo.Text);
            undoHasMouse = true;
        }

        private void btnUndo_MouseLeave(object sender, EventArgs e) {
            UndoTooltips.SetToolTip(toolbar, null);
            undoHasMouse = false;
        }

        private void btnRedo_MouseEnter(object sender, EventArgs e) {
            UndoTooltips.SetToolTip(toolbar, btnRedo.Text);
            redoHasMouse = true;
        }

        private void btnRedo_MouseLeave(object sender, EventArgs e) {
            UndoTooltips.SetToolTip(toolbar, null);
            redoHasMouse = false;
        }
        #endregion

        #region Screen copy/paste
        int copiedScreen;
        LevelIndex copiedLevel;

        void CopySelectedScreen() {
            copiedScreen = ScreenEditor.ScreenIndex;
            copiedLevel = ScreenEditor.LevelIndex;
        }

        void PasteSelectedScreen() {
            PerformAction(Actions.ChangeLevel(copiedLevel));
            PerformAction(Actions.SetLayout(copiedScreen));
        }
        #endregion

        #region Public Events

        public event EventHandler RomLoaded;
        private void OnRomLoaded() { RomLoaded.Raise(this); }

        public event EventHandler RomSaved;
        private void OnRomSaved() { RomSaved.Raise(this); }

        public event CancelEventHandler RomSaving;
        private void OnRomSaving(out bool cancel) {
            var ce = new CancelEventArgs(false);

            if (RomSaving != null) RomSaving(this, ce);
            cancel = ce.Cancel;
        }

        #endregion

        #region UI Events
        private void btnScrollnormal_Click(object sender, EventArgs e) {
            gameView.ScrollSpeed = (int)PresetScrollSpeeds.Normal;
            UpdateScrollSpeedMenu();
        }

        private void btnScrollfast_Click(object sender, EventArgs e) {
            gameView.ScrollSpeed = (int)PresetScrollSpeeds.Fast;
            UpdateScrollSpeedMenu();
        }

        private void btnScrollVeryFast_Click(object sender, EventArgs e) {
            gameView.ScrollSpeed = (int)PresetScrollSpeeds.VeryFast;
            UpdateScrollSpeedMenu();
        }

        private void frmMain_Deactivate(object sender, EventArgs e) {
            toolbar.Enabled = false;
        }

        private void frmMain_Activated(object sender, EventArgs e) {
            toolbar.Enabled = true;
        }


        private void btnOpen_Click(object sender, EventArgs e) {
            ShowOpenFileDialog();
        }

        private void ShowOpenFileDialog() {
            if (AskToSaveRom() == askToSaveRomResult.CancelClose)
                return;

            string oldFileName = RomFileName;
            if (cdgOpenRom.ShowDialog() == DialogResult.OK) {
                RomFileName = cdgOpenRom.FileName;
                if (PerformOpen()) {
                    UpdateFormCaption();
                } else {
                    cdgOpenRom.FileName = oldFileName;

                    if (!string.IsNullOrEmpty(oldFileName)) {
                        RomFileName = oldFileName;
                    }
                }
            }
        }

        private void UpdateFormCaption() {
            string title = "Editroid - " + Path.GetFileName(RomFileName);

            if (gameRom != null) {
                if (gameRom.RomFormat == RomFormats.Standard)
                    title += " [MMC1]";
                else if (gameRom.RomFormat == RomFormats.Expando)
                    title += " [MMC1 1/2 Expanded]";
                else if (gameRom.RomFormat == RomFormats.Enhanco)
                    title += " [MMC1 Expanded]";
                else if(gameRom.RomFormat == RomFormats.MMC3)
                    title += " [MMC3]";
                else
                    title += " [" + gameRom.Format.ToString() + "]";
            }

            Text = title;
        }


                /// <summary>
        /// Opens a rom without showing the open file dialog
        /// </summary>
        private bool PerformOpen() {
            var projectFilePath = Path.ChangeExtension(cdgOpenRom.FileName, ".nes.proj");
            using (FileStream gameStream = new FileStream(cdgOpenRom.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                return PerformOpen(gameStream, delegate() { return new FileStream(projectFilePath, FileMode.Open, FileAccess.Read); });
            }

        }
        /// <summary>
        /// Opens a rom without showing the open file dialog
        /// </summary>
        /// <returns>True if the open succeeded, otherwise false.</returns>
        private bool PerformOpen(Stream s, Func<Stream> getProjectStream) {
            MetroidRom oldRom = gameRom;
            Project oldProject = gameProject;
            try {
                using (s) {
                    OpenRom(s, true, getProjectStream);
                }

            } catch (Exception ex) {
                // Todo: an exception caught here may leave the program in an inconsistent state (what happens if gameRom is initialized, but an exception is thrown updating UI? now half the program is looking at the wrong ROM)
                MessageBox.Show("Could not load the specified file (" + ex.GetType().ToString() + "- " + ex.Message + ")");
                return false;

                // This is a hacky bandaid that might still leave some issues unaddressed
                gameRom = oldRom;
                gameProject = oldProject;
            }


            EnableControls();
            undoRedo.Clear();
            mapView.FilterEmptyLocations();
            gameView.RedrawAll();

            mapView.ItemDisplay.ClearItemDisplay();
            CheckItemEditorMenuItem(btnNoItems);

            mapImageGenerator.EnqueueAll();
            MapView.PerformingInitialRender = true;
            //PerformScreenRenderLoop();
            MapView.PerformingInitialRender = false;

            ShowProjectMenuItems(gameProject != null);
            UpdateSelectionStatus(); // This line is here to disable controls that are improperly enabled in EnableControls
            return true;
        }


        private void btnSaveAndPlay_Click(object sender, EventArgs e) {
            SaveAndPlay();
        }

        private void SaveAndPlay() {
            SaveRom();
            //System.Diagnostics.Process.Start(cdgOpenRom.FileName);
            LaunchRom(cdgOpenRom.FileName);
        }

        private void btnClassicMap_Click(object sender, EventArgs e) {
            mapImageGenerator.ClearQueue();

            mapView.UseScreenImages = false;
            btnClassicMap.Checked = true;
            btnDrawMap.Checked = false;
        }

        private void btnDrawMap_Click(object sender, EventArgs e) {
            mapView.UseScreenImages = true;
            btnClassicMap.Checked = false;
            btnDrawMap.Checked = true;

            mapImageGenerator.EnqueueAll();
            //if(gameRom != null)
            //    PerformScreenRenderLoop();
        }

        private void btnUndo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            int count = ((int)e.ClickedItem.Tag) + 1;
            for (int i = 0; i < count; i++) {
                Undo();
            }
        }

        private void btnRedo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            int count = ((int)e.ClickedItem.Tag) + 1;
            for (int i = 0; i < count; i++) {
                Redo();
            }

        }

        ////frmPassword passEditor;

        ////private void mnuPasswordEditor_Click(object sender, EventArgs e) {
        ////    if (passEditor == null || passEditor.IsDisposed) {
        ////        passEditor = new frmPassword();
        ////        passEditor.Rom = gameRom;
        ////        passEditor.MapEditor = mapView;
        ////    }

        ////    passEditor.ShowDialog();
        ////    passEditor.Dispose();
        ////}

        private void mnuStart_Click(object sender, EventArgs e) {
            currentLevelData.StartScreenX = ScreenEditor.MapLocation.X;
            currentLevelData.StartScreenY = ScreenEditor.MapLocation.Y;
        }
        
        private void btnAltPal_Click(object sender, EventArgs e) {
            gameView.UseAltPalette = btnAltPal.Checked;
            gameView.RedrawAll();
            if (Program.StructFormIsLoaded)
                Program.StructForm.UseAlternatePalette = btnAltPal.Checked;

            structurePicker1.UseAlternatePalette = btnAltPal.Checked;
        }

        private void btnStructBack_Click(object sender, EventArgs e) {
            SelectPreviousType();
        }

        private void SelectPreviousType() {
            var item = ScreenEditor.SelectedItem as ItemInstance;
            var door = ScreenEditor.SelectedItem as DoorInstance;

            if (item != null && item.Data.ItemType == ItemTypeIndex.PowerUp) {
                int index = (int)((ItemPowerupData)item.Data).PowerUp;
                index = (index + 9) % 10;

                PerformAction(actions.EditItemProperty(
                    ScreenEditor.MapLocation,
                    ScreenEditor.ScreenItems,
                    item.Data,
                    Editroid.Actions.ItemProperty.power_up_type,
                    index
                    ));
            } else if (door != null) {
                PerformAction(Actions.ChangeDoor(
                    door.Side,
                    (DoorType)(((int)door.Type + 3) % 4)
                ));
            } else {
                PerformAction(actions.PreviousType());
            }
        }

        private void btnStructNext_Click(object sender, EventArgs e) {
            SelectNextType();
        }

        private void SelectNextType() {
            var item = ScreenEditor.SelectedItem as ItemInstance;
            var door = ScreenEditor.SelectedItem as DoorInstance;

            if (item != null && item.Data.ItemType == ItemTypeIndex.PowerUp) {
                int powIndex = (int)((ItemPowerupData)item.Data).PowerUp;
                powIndex = (powIndex + 1) % 10;

                PerformAction(actions.EditItemProperty(
                    ScreenEditor.MapLocation,
                    ScreenEditor.ScreenItems,
                    item.Data,
                    Editroid.Actions.ItemProperty.power_up_type,
                    powIndex
                    ));
            } else if (door != null) {
                PerformAction(Actions.ChangeDoor(
                    door.Side, 
                    (DoorType)(((int)door.Type + 1) % 4)
                ));
            } else {
                PerformAction(actions.NextType());
            }
        }

        private void mnuEditPalette_Click(object sender, EventArgs e) {
            Program.LoadPaletteForm();

            Program.PaletteForm.Show();
            Program.PaletteForm.BringToFront();
        }
        private void mnuTitleText_Click(object sender, EventArgs e) {
            if (titleForm == null || titleForm.IsDisposed)
                titleForm = new frmTitleText();

            titleForm.Rom = gameRom;
            Program.Dialogs.ShowDialog(titleForm,this);
            titleForm.Dispose();
        }

 
        private void btnPalette_Click(object sender, EventArgs e) {
            if(ModifierKeys == Keys.Shift) {
                PerformAction(actions.NextCAT());
            } else {
                PerformAction(actions.ChangePalette());
            }
        }

        private void btnBringFront_Click(object sender, EventArgs e) {
            PerformAction(actions.BringToFront());
        }

        private void btnSendBack_Click(object sender, EventArgs e) {
            PerformAction(actions.SendToBack());
        }

        private void ScreenEditor_SelectedObjectChanged(object sender, EventArgs e) {
            UpdateSelectionStatus();
            GlobalEventManager.Manager.OnObjectSelected(this, ScreenEditor.LevelIndex, ScreenEditor.SelectedItem);
            if (Program.PaletteFormIsLoaded && ScreenEditor.CanEdit)
                Program.PaletteForm.LevelData = gameRom.GetLevel(ScreenEditor.LevelIndex);

            numericInput.Clear();
        }

        private void gameView_KeyDown(object sender, KeyEventArgs e) {
            OnKeyDown(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.OemCloseBrackets) {
                if (gameRom != null && !gameRom.Format.UsesStandardPpuLoader) {
                    SelectNextChr();
                    return true;
                }
            }
            if (keyData == Keys.OemOpenBrackets) {
                if (gameRom != null && !gameRom.Format.UsesStandardPpuLoader) {
                    SelectPreviousChr();
                    return true;
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }


        bool IsShifting { get { return (ModifierKeys & Keys.Shift) == Keys.Shift; } }
        bool IsControlling { get { return (ModifierKeys & Keys.Control) == Keys.Control; } }
        private void gameView_CmdKeyPressed(object sender, KeyEventArgs e) {
            // Note: if any funtions are added that can work when a disabled editor
            //      is selected, they should go above this check.
            if (ScreenEditor == null || !ScreenEditor.CanEdit) return;

            if (e.KeyCode == Keys.Left) {
                if (IsShifting || IsControlling) {
                    gameView.ScrollScreen(-1, 0);
                } else {
                    ScreenEditor.RaiseMoveObject(-1, 0);
                }
                e.Handled = true;
            } else if (e.KeyCode == Keys.Up) {
                if (IsShifting || IsControlling) {
                    gameView.ScrollScreen(0, -1);
                } else {
                    ScreenEditor.RaiseMoveObject(0, -1);
                }
                e.Handled = true;
            } else if (e.KeyCode == Keys.Right) {
                if (IsShifting || IsControlling) {
                    gameView.ScrollScreen(1, 0);
                } else {
                    ScreenEditor.RaiseMoveObject(1, 0);
                }
                e.Handled = true;
            } else if (e.KeyCode == Keys.Down) {
                if (IsShifting || IsControlling) {
                    gameView.ScrollScreen(0, 1);
                } else {
                    ScreenEditor.RaiseMoveObject(0, 1);
                }
                e.Handled = true;
            } else if (e.KeyCode == Keys.Tab) {
                if (!IsControlling) {
                    if (IsShifting) {
                        ScreenEditor.SelectPrevious();
                        ScreenEditor.Redraw();
                    } else {
                        ScreenEditor.SelectNext();
                        ScreenEditor.Redraw();
                    }
                }
                e.Handled = true;

            }
        }


        protected override bool ProcessKeyMessage(ref Message m) {
            return base.ProcessKeyMessage(ref m);
        }

        public ObjectInstance SelectedItem { get { return ScreenEditor.SelectedItem; } }

        string keybuffer = new string(' ', 20);
        const string secretmenustring = "ILIKETURTLES";

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            keybuffer += (char)e.KeyValue;
            keybuffer = keybuffer.Substring(1);
            if (string.Compare(keybuffer, keybuffer.Length - secretmenustring.Length, secretmenustring, 0, secretmenustring.Length, true) == 0) {
                mnuSecret.Visible = true;
            }

            if (gameRom == null) return;

            CheckNumpad(e);

            bool shift = (ModifierKeys & Keys.Shift) == Keys.Shift;
            bool ctrl = (ModifierKeys & Keys.Control) == Keys.Control;
            bool alt = (ModifierKeys & Keys.Alt) == Keys.Alt;

            if (shift | alt) gameView.UpdateCursor();

            if (ScreenEditor.CanEdit) {
                if (e.KeyCode == Keys.PageUp) {
                    if (shift) {
                        PerformAction(actions.SeekLayout(-8));
                    } else {
                        PerformAction(actions.PreviousLayout());
                    }
                } else if (e.KeyCode == Keys.PageDown) {
                    if (shift) {
                        PerformAction(actions.SeekLayout(8));
                    } else {
                        PerformAction(actions.NextLayout());
                    }
                } else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract) {
                    //if (ScreenEditor.CanEdit && !ScreenEditor.SelectedItem.IsNothing && (SelectedItem.SupportsTypeIndex || (SelectedItem.InstanceType == ObjectInstanceType.Item  && SelectedItem.ThisItem.Data.ItemType == ItemType.PowerUp)))
                    if (btnStructBack.Enabled)
                        SelectPreviousType();
                } else if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add) {
                    //if (ScreenEditor.CanEdit && !ScreenEditor.SelectedItem.IsNothing && (SelectedItem.SupportsTypeIndex || (SelectedItem.InstanceType == ObjectInstanceType.Item  && SelectedItem.ThisItem.Data.ItemType == ItemType.PowerUp)))
                    if (btnStructNext.Enabled)
                        SelectNextType();
                } else if (e.KeyCode == Keys.Home) {
                    if (SelectedItem is StructInstance)
                        PerformAction(actions.BringToFront());
                } else if (e.KeyCode == Keys.End) {
                    if (SelectedItem is StructInstance)
                        PerformAction(actions.SendToBack());
                } else if (e.KeyCode == Keys.P || e.KeyCode == Keys.Space) {
                    if (SelectedItem is StructInstance || SelectedItem is EnemyInstance)
                        PerformAction(actions.ChangePalette());
                } else if ((ModifierKeys & Keys.Shift) == Keys.Shift &&
                    e.KeyCode == Keys.Oemtilde) {
                    if (SelectedItem is StructInstance || SelectedItem is EnemyInstance)
                        PerformAction(actions.ChangePalette(0));
                } else if ((ModifierKeys & Keys.Shift) == Keys.Shift &&
                    e.KeyCode == Keys.D1) {
                    if (SelectedItem is StructInstance || SelectedItem is EnemyInstance)
                        PerformAction(actions.ChangePalette(1));
                } else if (shift && e.KeyCode == Keys.D2) {
                    if (SelectedItem is StructInstance || SelectedItem is EnemyInstance)
                        PerformAction(actions.ChangePalette(2));
                } else if (shift && e.KeyCode == Keys.D3) {
                    if (SelectedItem is StructInstance || SelectedItem is EnemyInstance)
                        PerformAction(actions.ChangePalette(3));
                } else if (e.KeyCode == Keys.Tab) {
                    if (IsControlling) {
                        ScreenEditor.ToggleSelectionType();
                    }
                    ScreenEditor.Redraw();
                } else if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete) {
                    if (mnuRemove.Enabled)
                        PerformAction(actions.RemoveSelection());
                } else if (e.KeyCode == Keys.Oemcomma) {
                    if (mnuAddObject.Enabled)
                        PerformAction(actions.AddObject());
                } else if (e.KeyCode == Keys.OemPeriod) {
                    if (mnuAddEnemy.Enabled)
                        PerformAction(actions.AddEnemy());
                } else if (!shift && !ctrl && getIntValue(e.KeyCode) != -1) {
                    OnNumericInput(getIntValue(e.KeyCode));
                    return;
                }
            }
            
            if (e.KeyCode == Keys.C && ctrl) {
                CopySelectedScreen();
            } else if (e.KeyCode == Keys.V && ctrl) {
                PasteSelectedScreen();
            }

            numericInput.Clear();
        }
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);


            bool shift = (ModifierKeys & Keys.Shift) == Keys.Shift || e.KeyCode == Keys.ShiftKey;
            bool ctrl = (ModifierKeys & Keys.Control) == Keys.Control || e.KeyCode == Keys.ControlKey;
            bool alt = (ModifierKeys & Keys.Alt) == Keys.Alt;
            bool maybeAlsoAlt = e.KeyData == Keys.Menu;

            if (shift | alt | maybeAlsoAlt) gameView.UpdateCursor();
        }


        private void mnuRemove_Click(object sender, EventArgs e) {
            PerformAction(actions.RemoveSelection());
        }

        private void mnuAddObject_Click(object sender, EventArgs e) {
            PerformAction(actions.AddObject());
        }

        private void mnuAddEnemy_Click(object sender, EventArgs e) {
            PerformAction(actions.AddEnemy());
        }

        private void btnPrevScreen_Click(object sender, EventArgs e) {
            PerformAction(actions.PreviousLayout());
        }
        private void btnNextScreen_Click(object sender, EventArgs e) {
            PerformAction(actions.NextLayout());
        }

        private void btnUndo_Click(object sender, EventArgs e) {
            Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e) {
            Redo();
        }

        private void btnSave_Click(object sender, EventArgs e) {
            SaveRom();
        }

        private void mnuPointer_Click(object sender, EventArgs e) {
            if (pointerForm == null || pointerForm.IsDisposed)
                pointerForm = new frmPointer();

            pointerForm.Show();
        }

        private void mnuEditStructs_Click(object sender, EventArgs e) {
            Program.LoadStructForm();

            Program.StructForm.UseAlternatePalette = btnAltPal.Checked;
            Program.StructForm.Show();
            Program.StructForm.BringToFront();

        }

        internal void InitializeStructureEditorData(frmStruct form) {
            form.Rom = gameRom;
            form.UseAlternatePalette = btnPalette.Checked;
            if (ScreenEditor == null || !ScreenEditor.CanEdit) {
                form.LevelIndex = LevelIndex.Brinstar;
            } else {
                form.LevelIndex = ScreenEditor.LevelIndex;
                GlobalEventManager.Manager.OnObjectSelected(this, ScreenEditor.LevelIndex, ScreenEditor.SelectedItem);
            }
        }

        private void mnuSaveMapTo_Clicked(object sender, EventArgs e) {
            SaveMapToROM = sender == mnuSaveMapToROM;
        }

        private void mnuPhysics_Click(object sender, EventArgs e) {
            gameView.ShowPhysics = mnuPhysics.Checked;
            gameView.RedrawAll();
        }

        private void MapEditor_RoomCleared(object sender, int X, int Y) {
            gameRom.SetScreenIndex(X, Y, 255);
        }

        private void mnuBackup_Click(object sender, EventArgs e) {
            DateTime time = DateTime.Now;

            string Filename = time.Year.ToString() + "_" + time.Month.ToString().PadLeft(2, '0') + "_" + time.Day.ToString().PadLeft(2, '0') + "_" + time.Hour.ToString().PadLeft(2, '0') + time.Minute.ToString().PadLeft(2, '0') + time.Second.ToString().PadLeft(2, '0') +
                (UseNesForBackup ? ".nes" : ".bak");
            Filename = Path.Combine(
                Path.GetDirectoryName(cdgOpenRom.FileName),
                Filename);
            string ProjectFilename = time.Year.ToString() + "_" + time.Month.ToString().PadLeft(2, '0') + "_" + time.Day.ToString().PadLeft(2, '0') + "_" + time.Hour.ToString().PadLeft(2, '0') + time.Minute.ToString().PadLeft(2, '0') + time.Second.ToString().PadLeft(2, '0') +
                (UseNesForBackup ? ".nes.proj" : ".bak.proj"); // Todo: declare a constant somewhere for project filename
            ProjectFilename = Path.Combine(
                Path.GetDirectoryName(cdgOpenRom.FileName),
                ProjectFilename);

            

            FileStream gameStream = new FileStream(Filename, FileMode.Create, FileAccess.Write);
            gameRom.SaveTo(gameStream);
            SaveMap(gameStream, null, true);
            gameStream.Close();

            if (gameProject != null)
                using (var projectStream = new FileStream(ProjectFilename, FileMode.Create, FileAccess.Write)) {
                    gameProject.Save(projectStream);
                }

        }

        private void mnuUseNES_Click(object sender, EventArgs e) {
            UseNesForBackup = true;
        }

        private void mnuUseBAK_Click(object sender, EventArgs e) {
            UseNesForBackup = false;
        }

        private void btnAbout_Click(object sender, EventArgs e) {
            MagicForms.HelpForm.ShowAbout();
        }

        private void btnGettingStarted_Click(object sender, EventArgs e) {
            MagicForms.HelpForm.ShowGettingStarted();
        }

        private void btnHelp_Click(object sender, EventArgs e) {
            ////MagicForms.HelpForm.ShowHelp();
            Process.Start("http://ilab.ahemm.org/editroid/help");
        }


        private void mnuPointerExplorer_Click(object sender, EventArgs e) {
            Program.PointerExplorerForm.Show();
        }

        private void btnRespawn_Click(object sender, EventArgs e) {
            PerformAction(actions.ToggleRespawn());
        }


        private void mnuFeedback_Click(object sender, EventArgs e) {
            MagicForms.HelpForm.ShowFeedback();
        }

        private void mnuIPS_Click(object sender, EventArgs e) {
            MagicForms.IpsForm.CurrentRom = gameRom;

            MagicForms.IpsForm.CurrentRomPatched += new EventHandler(IpsForm_CurrentRomPatched);
            Program.Dialogs.ShowDialog(MagicForms.IpsForm,this);
            MagicForms.IpsForm.CurrentRomPatched -= new EventHandler(IpsForm_CurrentRomPatched);

        }

        void IpsForm_CurrentRomPatched(object sender, EventArgs e) {
            ReloadCurrentRomFromMemory();
        }

        /// <summary>
        /// Reloads the current rom from the image in memory (recreates the ROM object using the same data).
        /// </summary>
        public void ReloadCurrentRomFromMemory() {
            if (gameRom.RomFormat != RomFormats.Standard)
                SerializeMapToExpandedRom(gameRom.data);

            byte[] romBuffer = gameRom.data;
            ReloadCurrentRomFromMemory(romBuffer, gameProject);
        }
        /// <summary>
        /// Reloads the current rom from the image in memory (recreates the ROM object using the same data).
        /// </summary>
        public void ReloadCurrentRomFromMemory(byte[] romBuffer, Project project) {
            MemoryStream romBufferStream = new MemoryStream(romBuffer);
            // Re-loads the current ROM to reflect changes. Reloaded rom will still have same filename
            if (gameProject == null) {
                OpenRom(romBufferStream, false, null);
            } else {
                // Save project to memory
                MemoryStream projectData = new MemoryStream();
                project.Save(projectData);
                //projectData = new MemoryStream(projectData.GetBuffer(), 0, (int)projectData.Length);
                projectData.Seek(0, SeekOrigin.Begin);

                OpenRom(romBufferStream, false, delegate() { return (Stream)projectData; });
            }
            EnableControls();
            mapImageGenerator.EnqueueAll();
            romBufferStream.Close();

            mapView.FilterEmptyLocations();
            gameView.RedrawAll();
        }

        private void ScreenGrid_KeyDown(object sender, KeyEventArgs e) {

        }






        private void btnEnemySlot_Click(object sender, EventArgs e) {
            PerformAction(actions.NextSpriteSlot());
        }

        private void btnBridge_Click(object sender, EventArgs e) {
            if (btnBridge.Checked)
                PerformAction(actions.RemoveBridge());
            else
                PerformAction(actions.AddBridge());
        }

        private void frmMain_Load(object sender, EventArgs e) {
            ShowProjectMenuItems(false);
            mnuProjectCreate.Enabled = false;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameStream"></param>
        /// <param name="LoadMapFromFile"></param>
        /// <param name="projectFileGetter">A function that returns a stream containing project data, or null.</param>
        /// <param name="project">A Project object to serve as the ROM's project. This overrides the projectFileGetter parameter if not null.</param>
        private void OpenRom(Stream gameStream, bool LoadMapFromFile, Func<Stream> projectFileGetter) {
            mapImageGenerator.ClearQueue();
            CloseWindows();

            bool errMissingProjectFile = false;

            gameRom = new MetroidRom(gameStream);
            gameProject = null;


            if (gameRom.EditorData.HasAssociatedProject) {
                if (projectFileGetter != null) {
                    try {
                        LoadProjectFile(projectFileGetter());
                    } catch (FileNotFoundException ex) {
                        MessageBox.Show("Warning: The project file associated with this ROM was not found. Saving the ROM will remove the project file association.");
                        gameRom.EditorData.HasAssociatedProject = false;
                    }
                }
            } else {
                gameProject = null;
            }

            if (gameRom.Format.HasChrAnimationTable && gameProject != null) {
                gameRom.SetChrAnimationNames(gameProject.ChrAnimationNameList);
            }
            LoadMap(gameStream, LoadMapFromFile);
            FixOutOfRangeScreens();
            gameRom.CorrectDoubledScreens(this.mapView);

            HandleUnusedCode();

            gameView.Rom = gameRom;
            gameView.Screens.MakeDefaultSelection();
            actions.Rom = gameRom;
            undoRedo.Rom = gameRom;
            mapImageGenerator.Rom = gameRom;
            mapView.Rom = gameRom;
            ItemDataEditor.Rom = gameRom;
            passwordEditor.Rom = gameRom;


            ClearUndo();
            CheckItemEditorMenuItem(btnNoItems);

            btnAddRoom.Enabled = btnDeleteRoom.Enabled = btnCopyroom.Enabled = false;
            picTitle.ClearImages();

            btnSaveSansMap.Visible = btnSaveSansMap.Enabled = gameRom.RomFormat == RomFormats.Standard;
            btnChrUsage.Enabled = gameRom.Format.HasChrUsageTable;
            btnChrUsage.Visible = !gameRom.Format.HasChrAnimationTable;
            btnChrAnimation.Visible = gameRom.Format.HasChrAnimationTable;

            mnuProjectCreate.Enabled = (gameRom.RomFormat == RomFormats.Enhanco || gameRom.RomFormat == RomFormats.MMC3);

            OnRomLoaded();


            structurePicker1.ShowStructs(gameRom.Brinstar, structurePicker1.Palette, 0);

        }

        private void FixOutOfRangeScreens() {
            for (int mapY = 0; mapY < 32; mapY++) {
                for (int mapX = 0; mapX < 32; mapX++) {
                    int screenIndex = gameRom.GetScreenIndex(mapX, mapY);
                    var levelIndex = mapView.GetLevel(mapX, mapY);

                    if (levelIndex == LevelIndex.None) {
                        // If the map location is marked as entry in editor map, but the game map has it marked as used...
                        if (screenIndex != 0xFF) {

                            // Pick a level that allows this screen index, if possible
                            for (int iLevel = 0; iLevel < gameRom.Levels.Count; iLevel++) {
                                if (screenIndex < gameRom.Levels[(LevelIndex)iLevel].Screens.Count) {
                                    mapView.SetLevel(mapX, mapY, (LevelIndex)iLevel);
                                    break;
                                }
                            }
                        }
                    } else {
                        // If the editor map says this location is used, but it is empty in the game map, clear it
                        if (screenIndex == 0xFF) {
                            mapView.SetLevel(mapX, mapY, LevelIndex.None);
                        } else {
                            var level = gameRom[levelIndex];
                            // If the screen index is out of bounds for the level specified, pick another level
                            if (screenIndex >= level.Screens.Count) {
                                // Pick a level that allows this screen index, if possible
                                for (int iLevel = 0; iLevel < gameRom.Levels.Count; iLevel++) {
                                    if (screenIndex < gameRom.Levels[(LevelIndex)iLevel].Screens.Count) {
                                        mapView.SetLevel(mapX, mapY, (LevelIndex)iLevel);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// If gameProject is not null and there is unused code, the user will be prompted to move the code or lose it.
        /// </summary>
        private void HandleUnusedCode() {
            if (gameProject != null && gameProject.UnusedScreenLoadAsm.Count > 0) {
                var msgResult = MessageBox.Show(
                    "This ROM and the associated project file do not appear to match. There is ASM in the project file that will become orphaned." + Environment.NewLine + Environment.NewLine + "Copy orphaned code to the general code file? (This is not permanent if the ROM is not saved.)",
                    "Mismatched Files",
                    MessageBoxButtons.YesNo);

                if (msgResult == DialogResult.Yes) {
                    StringBuilder sb = new StringBuilder();
                        sb.AppendLine();
                        sb.AppendLine("; Orphaned screen-load routines");
                        sb.AppendLine();

                    for (int i = 0; i < gameProject.UnusedScreenLoadAsm.Count; i++) {
                        var code = gameProject.UnusedScreenLoadAsm[i];

                        if (code.Level != LevelIndex.None) {
                            // Add label for screen-load code
                            // Label will be something like BrinstarScreen2F:
                            sb.Append(code.Level.ToString());
                            sb.Append("Screen");
                            sb.Append(code.ScreenIndex.ToString("X2"));
                            sb.AppendLine(":");
                        }
                        sb.AppendLine();
                        sb.AppendLine(code.ASM);
                        sb.AppendLine();

                    }

                    gameProject.GeneralCodeFile = gameProject.GeneralCodeFile + sb.ToString();
                } else {
                    gameProject.UnusedScreenLoadAsm.Clear();
                }
            }
        }

        private void LoadProjectFile(Stream stream) {
            try {
                gameProject = new Project(gameRom, stream);
                stream.Close();
            } catch (ProjectLoadException ex) {
                MessageBox.Show("An error occurred while loading the associated project file. Saving the ROM will cause the files to become un-associated." + Environment.NewLine + Environment.NewLine + "Error message: " + ex.Message);
                gameProject = null;
                return;
            }

            if (gameProject.CouldNotParseAllData) {
                MessageBox.Show("The project file contains data that could not be parsed. Saving the file will cause that data to be lost.");
            }
            mnuProjDebug.Checked = gameProject.Debug;
            mnuBuildoptionsBuildontest.Checked = gameProject.BuildOnTestRoom;
        }


        private void ClearUndo() {
            undoRedo.Clear();
            btnUndo.Enabled = false;
            btnRedo.Enabled = false;
        }

        // Todo: the ROM object should load/save/own map data, using a callback if 
        private void LoadMap(Stream gameStream, bool LoadMapFromFile) {
            // Load map (preference in order of: embedded (in ROM), external (MP file), default)
            
            bool loadedFromRom = false;
            if (gameRom.ExpandedRom) {
                gameStream.Seek(ExpandedOrEnhanced_MapDataOffset, SeekOrigin.Begin);
                loadedFromRom = mapView.TryLoadData(gameStream, true);
            } else {
                loadedFromRom = mapView.TryLoadData(gameStream);
            }
            if (!loadedFromRom) {
                if (File.Exists(GetMapFileName()) && LoadMapFromFile) {
                    mapView.LoadData(GetMapFileName());
                } else {
                    byte[] defaultMap = Resources.DefaultMap;
                    MemoryStream defaultMapStream = new MemoryStream(defaultMap);
                    mapView.TryLoadData(defaultMapStream);
                }
            }
        }

        void gameView_WorldViewChanged(object sender, EventArgs<Point> e) {

        }

        void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e) {

        }

        void Application_Idle(object sender, EventArgs e) {
            if (mapImageGenerator.HasRemainingTasks) {
                if (MapThread.IsBusy) return; // Don't access rendering resources while map is rendering
                PerformScreenRenderLoop();
            }
        }

        private void PerformScreenRenderLoop() {
            mnuMap.Enabled = false;

            mapImageGenerator.PerformScreenRenderLoop();

            mnuMap.Enabled = true;
        }

        void Screens_UserDraggedSelection(object sender, EventArgs<Point> e) {
            PerformAction(actions.MoveObject(e.Value));
        }

        void on_ScreenEditorSelectionDragged(object sender, EventArgs e) {
        }

        void Global_StructureChanged(object sender, LevelEventArgs e) {
            gameView.Redraw(e.Level);
        }

        void Global_ComboChanged(object sender, LevelEventArgs e) {
            gameView.Redraw(e.Level);
        }

        List<string> temporaryFiles = new List<string>();

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            if (AskToSaveRom() == askToSaveRomResult.CancelClose) {
                e.Cancel = true;
                return;
            }

            foreach (string tempFile in temporaryFiles) {
                try { File.Delete(tempFile); } catch { }
            }

            mapImageGenerator.ClearQueue();
            SaveSettings();
            Application.Idle -= new EventHandler(Application_Idle);
            SystemEvents.DisplaySettingsChanged -= new EventHandler(SystemEvents_DisplaySettingsChanged);

            GlobalEventManager.Manager.StructureChanged -= new EventHandler<LevelEventArgs>(Global_StructureChanged);
            GlobalEventManager.Manager.ComboChanged -= new EventHandler<ComboEventArgs>(Global_ComboChanged);

            foreach (string file in temporaryFiles) {
                try {
                    File.Delete(file);
                } catch { }
            }
        }

        private askToSaveRomResult AskToSaveRom() {
            if (gameRom == null) return  askToSaveRomResult.None;

            gameRom.SerializeAllData();
            if (Program.CodeEditorLoaded) Program.CodeEditor.SaveCurrentDocument();

            bool unsavedProjectChanges = gameProject != null && gameProject.UnsavedChanges;

            if (unsavedProjectChanges || gameRom.CompareToSavedVersion().Count != 0) {
                DialogResult r = MessageBox.Show("Would you like to save changes to the ROM?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel) {
                    return askToSaveRomResult.CancelClose;
                } else if (r == DialogResult.Yes) {
                    SaveRom();
                }
            }
            return askToSaveRomResult.None;
        }
        enum askToSaveRomResult { None, CancelClose }


        
        /// <summary>
        /// Updates all controls and menus related to screen editing, as well as the screen display.
        /// </summary>
        private void UpdateSelectionStatus() {
            var editor = ScreenEditor;
            var screen = (editor == null) ? null : editor.Screen;

            bool canEdit = editor != null && editor.CanEdit;
            bool canAddDelete_RoomsStructs = gameRom.Format.SupportsEnhancedMemoryManagement;
            ObjectInstance selection = editor.SelectedItem;
            int freeSpace = currentLevelData == null ? 0 : currentLevelData.FreeScreenData;
            int freeStructSpace = currentLevelData == null ? 0 : currentLevelData.FreeStructureMemory;
            ////// We may need an extra byte to create an enemy data marker
            ////int enemyFreespace = freeSpace;
            ////if (canEdit && !screen.HasEnemyDataSegment) 
            ////    enemyFreespace -= 1; // Done: fuck this shit, we got something better


            // Selection
            bool isEnemy = selection is EnemyInstance;
            bool isStruct = selection is StructInstance;
            bool isEnemyOrStruct = isEnemy | isStruct;
            bool isDoor = selection is DoorInstance;
            bool isPowerUp = (selection is ItemInstance && ((ItemInstance)selection).Data.ItemType == ItemTypeIndex.PowerUp);

            // Menu
            mnuSetLevel.Enabled = canEdit;
            btnItemDisassm.Enabled = canEdit;
            btnItemDisassm.Text = btnItemDisassm.Tag.ToString().Replace("*", canEdit ? currentLevelData.Index.ToString() : "Level");
            btnDropSamus.Enabled = btnDropSamusOptions.Enabled = mnuStart.Enabled = btnDisableScreen.Enabled = canEdit;
            btnBridge.Checked = canEdit && screen.HasBridge;
            btnTilePhys.Enabled = screen != null;
            btnTilePhys.Visible = gameRom.Format.SupportsCustomTilePhysics;

            // Toolbar
            btnNextScreen.Enabled = canEdit;
            btnPrevScreen.Enabled = canEdit;
            btnScreenGrid.Enabled = canEdit;
            btnStructNext.Enabled = canEdit && (isEnemyOrStruct || isDoor || isPowerUp);
            btnStructBack.Enabled = btnStructNext.Enabled;
            btnGrid.Visible = canEdit && isStruct;
            btnBoss.Checked = isEnemy && ((EnemyInstance)selection).IsLevelBoss;
            btnPalette.Enabled = isEnemyOrStruct;
            mnuAddEnemy.Enabled = canEdit && freeSpace >= screen.GetBytesNeededFor(ScreenObjectType.Enemy);
            mnuAddObject.Enabled = canEdit && freeSpace >= screen.GetBytesNeededFor(ScreenObjectType.Struct);
            btnAddLeftDoor.Enabled = canEdit && screen.LeftDoor == null && freeSpace >= screen.GetBytesNeededFor(ScreenObjectType.Door);
            btnAddRightDoor.Enabled = canEdit && screen.RightDoor == null && freeSpace >= screen.GetBytesNeededFor(ScreenObjectType.Door);
            mnuRemove.Enabled = canEdit && (isEnemyOrStruct || isDoor);
            btnBridge.Enabled = btnBridge.Checked ||
                (canEdit && editor.LevelIndex == LevelIndex.Brinstar && freeSpace >= screen.GetBytesNeededFor(ScreenObjectType.Bridge));
            btnAddRoom.Enabled = canEdit && canAddDelete_RoomsStructs && freeSpace >= 4 && currentLevelData.Screens.Count < 0xFE;
            btnDeleteRoom.Enabled = canEdit && canAddDelete_RoomsStructs && ScreenEditor.ScreenIndex > 0;
            if (canEdit) {
                int spaceNeededToCopy = 4; // Pointer = 2, header = 1, footer = 1;
                spaceNeededToCopy += screen.Size;
                btnCopyroom.Enabled = freeSpace >= spaceNeededToCopy && currentLevelData.Screens.Count < 0xF0;
            } else {
                btnCopyroom.Enabled = false;
            }
            btnAddStruct.Enabled = canEdit && canAddDelete_RoomsStructs && freeStructSpace >= 5 && currentLevelData.StructCount < 0xF0;
            btnDeleteStruct.Enabled = canEdit && canAddDelete_RoomsStructs && selection is StructInstance && ((StructInstance)selection).ObjectType > 0;
            bool emptyScreen = editor != null && !editor.CanEdit;
            btnBrinstar.Visible = btnNorfair.Visible = btnKraid.Visible = btnRidley.Visible = btnTourian.Visible = emptyScreen;
            btnEditScreenLoad.Enabled = canEdit;
            btnEditScreenLoad.Visible = canEdit && gameProject != null;
            btnEditScreenLoadSeparator.Visible = canEdit && gameProject != null;

            // Show/Hide
            btnRespawn.Visible = btnBoss.Visible = btnEnemySlot.Visible = canEdit && selection is EnemyInstance;
            btnSendBack.Visible = btnBringFront.Visible = canEdit && selection is StructInstance;
            btnPalette.Visible = canEdit && isEnemyOrStruct;

            UpdateItemUI();

            var level = (screen == null) ? null : screen.Level;
            if(structurePicker1.Level != level) {
                structurePicker1.ShowStructs(level, structurePicker1.Palette, 0);
                //structurePicker1.Level = level;
            }
            structurePicker1.Visible = level != null;

            if (isStruct && EditStructureMode) {
                // Display selected structure
                
                structurePicker1.FocusOnStructure(((StructInstance)selection).ObjectType, (int)((StructInstance)selection).PalData);
            }

            ItemEditToolstrip.BringToFront();

        }

        private void UpdateItemUI() {
            var item = SelectedItem as ItemInstance;

            ////if (item != null) {
            ////    ItemDataEditor.LoadScreen(ScreenEditor.LevelIndex, ScreenEditor.MapLocation, ScreenEditor.Level.GetItemsAt(ScreenEditor.MapLocation), item);
            ////    ItemDataEditor.Show();
            ////    passwordEditor.BringToFront();
            ////} else {
            ////    ItemDataEditor.Hide();
            ////}

            if (item != null) {
                ItemEditingUI.LoadScreen(ScreenEditor.Level.GetItemsAt(ScreenEditor.MapLocation), item);

                ItemEditToolstrip.Visible = true;
            } else {
                ItemEditToolstrip.Visible = false;
            }
        }

        ////private void UpdateDoorMenus() {
        ////    if (!ScreenEditor.CanEdit) {
        ////        mnuRightNone.Enabled = mnuRightNormal.Enabled = mnuRight5.Enabled = mnuRight10.Enabled = false;
        ////        mnuLeftNone.Enabled = mnuLeftNormal.Enabled = mnuLeft5.Enabled = mnuLeft10.Enabled = false;
        ////        return;
        ////    }

        ////    mnuLeft10.Checked = mnuLeft5.Checked = mnuLeftNormal.Checked =
        ////        mnuRight10.Checked = mnuRight5.Checked = mnuRightNormal.Checked =
        ////        false;

        ////    mnuLeftNone.Checked = mnuRightNone.Checked = true;

        ////    DoorInstance[] Doors = CurrentScreen.Doors;
        ////    foreach (DoorInstance d in Doors) {
        ////        if (d.Side == DoorSide.Left)
        ////            switch (d.Type) {
        ////                case DoorType.Normal:
        ////                    mnuLeftNormal.Checked = true;
        ////                    mnuLeftNone.Checked = false;
        ////                    break;
        ////                case DoorType.Missile:
        ////                    mnuLeft5.Checked = true;
        ////                    mnuLeftNone.Checked = false;
        ////                    break;
        ////                case DoorType.TenMissile:
        ////                    mnuLeft10.Checked = true;
        ////                    mnuLeftNone.Checked = false;
        ////                    break;
        ////            } else if (d.Side == DoorSide.Right)
        ////            switch (d.Type) {
        ////                case DoorType.Normal:
        ////                    mnuRightNormal.Checked = true;
        ////                    mnuRightNone.Checked = false;
        ////                    break;
        ////                case DoorType.Missile:
        ////                    mnuRight5.Checked = true;
        ////                    mnuRightNone.Checked = false;
        ////                    break;
        ////                case DoorType.TenMissile:
        ////                    mnuRight10.Checked = true;
        ////                    mnuRightNone.Checked = false;
        ////                    break;
        ////            }
        ////    }

        ////    // Enable menu currentLevelItems to add doors if there is enough memory
        ////    // or enable currentLevelItems if there is alread a door
        ////    int neededSpace = 2; // space needed for doors
        ////    if (!CurrentScreen.HasEnemyDataSegment) neededSpace++;

        ////    mnuLeftNone.Enabled = mnuRightNone.Enabled = ScreenEditor.CanEdit;
        ////    mnuLeftNormal.Enabled = mnuLeft10.Enabled = mnuLeft5.Enabled =
        ////        ScreenEditor.CanEdit && (mnuLeftNone.Checked == false || currentLevel.FreeRoomData >= neededSpace);
        ////    mnuRightNormal.Enabled = mnuRight10.Enabled = mnuRight5.Enabled =
        ////        ScreenEditor.CanEdit && (mnuRightNone.Checked == false || currentLevel.FreeRoomData >= neededSpace);
        ////}

        /// <summary>
        /// Handles keyboard events for numpad (map editing).
        /// </summary>
        private void CheckNumpad(KeyEventArgs e) {
            switch(e.KeyCode) {
                case Keys.NumPad0:
                    PerformAction(actions.ChangeLevel(LevelIndex.None));
                    break;
                case Keys.NumPad1:
                    PerformAction(actions.ChangeLevel(LevelIndex.Brinstar));
                    break;
                case Keys.NumPad2:
                    PerformAction(actions.ChangeLevel(LevelIndex.Norfair));
                    break;
                case Keys.NumPad3:
                    PerformAction(actions.ChangeLevel(LevelIndex.Ridley));
                    break;
                case Keys.NumPad4:
                    PerformAction(actions.ChangeLevel(LevelIndex.Kraid));
                    break;
                case Keys.NumPad5:
                    PerformAction(actions.ChangeLevel(LevelIndex.Tourian));
                    break;
                default:
                    return;
            }
        }

        #region Keyboard input for struct/enemy count
        List<byte> numericInput = new List<byte>();
        private void OnNumericInput(int p) {
            // Register the keypress
            numericInput.Add((byte)p);

            if(SelectedItem is StructInstance | SelectedItem is EnemyInstance) {
                // Remove least-recently typed digits until the value is within range
                while(GetNumericInputValue() > currentLevelData.StructCount - 1) {
                    numericInput.RemoveAt(0);
                }

                // Update object
                PerformAction(actions.SetType(GetNumericInputValue()));
            }
            
        }

        /// <summary>Gets the value that has been typed in by the user.</summary>
        int GetNumericInputValue() {
            int result = 0;
            int multiple = 1;
            for(int i = numericInput.Count; i > 0; i--) {
                result += numericInput[i - 1] * multiple;
                multiple *= 0x10;
            }

            return result;
        }

        /// <summary>
        /// Gets the value of the specified key, or -1 if the key does not represent a value.
        /// </summary>
        int getIntValue(Keys key) {
            switch(key) {
                case Keys.D0:
                    return 0;
                case Keys.D1:
                    return 1;
                case Keys.D2:
                    return 2;
                case Keys.D3:
                    return 3;
                case Keys.D4:
                    return 4;
                case Keys.D5:
                    return 5;
                case Keys.D6:
                    return 6;
                case Keys.D7:
                    return 7;
                case Keys.D8:
                    return 8;
                case Keys.D9:
                    return 9;
                case Keys.A:
                    return 0xA;
                case Keys.B:
                    return 0xB;
                case Keys.C:
                    return 0xC;
                case Keys.D:
                    return 0xD;
                case Keys.E:
                    return 0xE;
                case Keys.F:
                    return 0xF;
            }

            return -1;
        }

        #endregion

        public void PerformAction(Editroid.UndoRedo.EditroidAction a){
            if (a.IsNullAction) return;

            undoRedo.Do(a);
            CorrectSelection(a);


            PerformUpdatesForAction(a, false);
                
        }

        private void Undo() {
            EditroidAction a = undoRedo.Undo();
            PerformUpdatesForAction(a, true);

            if (undoHasMouse) {
                UndoTooltips.SetToolTip(toolbar, btnUndo.Text);
            }
        }



        private void Redo() {
            EditroidAction a = undoRedo.Redo();
            PerformUpdatesForAction(a, false);

            if (redoHasMouse) {
                UndoTooltips.SetToolTip(toolbar, btnRedo.Text);
            }
        }

        /// <summary>
        /// Updates UI and calculated values in reaction to an action. Does
        /// NOT cause ANY change that would be reflected in ROM data.
        /// </summary>
        private void PerformUpdatesForAction(EditroidAction a, bool isUndo) {
            UpdateFreeSpaceForStructs(a);
            UpdateUndoButtons();
            UpdateEditors(a, isUndo);

            // In theory, an action that does not affect the selected screen should have no 
            // effect whatsoever on these parts of UI (selected screen, toolbar, struct editor)
            if (ActionEffectsSelectedScreen(a)) {
                UpdateSelection(a);
                UpdateSelectionStatus();
                // This happens last because it needs to account for changes 
                // in selection resulting from changes to rom data.
                UpdateStructForm(a, isUndo);
            }
        }

        /// <summary>
        /// Returns true if the specified action will have any effect on the data or UI state of the selected screen.
        /// </summary>
        bool ActionEffectsSelectedScreen(EditroidAction a) {
            //if (ScreenEditor == null || ScreenEditor.IsEmpty) return false;
            if (a.AffectedLevel == ScreenEditor.LevelIndex) return true;
            if (a.AffectedMapLocation == ScreenEditor.MapLocation) return true;

            // Am I missing anything? Different level or level-agnostic-and-different-map pos.
            return false;
        }

        private void UpdateFreeSpaceForStructs(EditroidAction a) {
            if (a is StructureAction)
                gameRom.SerializeAllData();
        }

        public bool UpdatePassDataForItems {
            get {
                return btnUpdatePass.Checked;
            }
        }

        private void UpdateStructForm(EditroidAction a, bool undo) {
            if (!Program.StructFormIsLoaded) return;

            if (a is ScreenAction || a is StructureAction) {

                if (a is ModifyObject && SelectedItem is StructInstance) {
                    // Update struct form if user changes palette or index
                    if (Program.StructForm.PaletteIndex != SelectedItem.GetPalette()) {
                        Program.StructForm.PaletteIndex = SelectedItem.GetPalette();
                    }
                    if (Program.StructForm.StructIndex != SelectedItem.GetTypeIndex()) {
                        Program.StructForm.StructIndex = SelectedItem.GetTypeIndex();
                    }
                } else if (ScreenEditor != null && ScreenEditor.CanEdit && ScreenEditor.SelectedItem is StructInstance) {
                    // Update struct form in case an action changes the selection or the selected struct changes
                    Program.StructForm.LevelIndex = ScreenEditor.LevelIndex;
                    Program.StructForm.StructIndex = ScreenEditor.SelectedItem.GetTypeIndex();
                } else if(a is StructureAction) {
                    Program.StructForm.LoadStruct(); // Reload the structure. This updates free mem labels too.
                }
                
            }
        }

        /// <summary>Corrects selection on order-changing actions.</summary>
        private void CorrectSelection(EditroidAction a) {
            Actions.ReorderObjects reorderAction = a as Actions.ReorderObjects;
            if (reorderAction != null) {
                if (reorderAction.ToFront) {
                    ScreenEditor.SelectedItem = ScreenEditor.Screen.Structs[ScreenEditor.Screen.Structs.Count - 1];
                } else {
                    ScreenEditor.SelectedItem = ScreenEditor.Screen.Structs[0];
                }
            }
        }

        /// <summary>
        /// Ensures that an item is selected.
        /// </summary>
        private void UpdateSelection(EditroidAction a) {
            if (ScreenEditor.SelectedItem == null)
                ScreenEditor.MakeDefaultSelection();
        }

        private void UpdateEditors(EditroidAction a, bool undo)
        {
            if (Program.AdvancedPalFormIsLoaded)
                Program.AdvancedPalForm.NotifyAction(a, undo);

            var palEdit = a as AdvancedPaletteEdit;
            if (palEdit != null) {
                if (palEdit.EditsAltLevelPalette || palEdit.EditsLevelPalette) {
                    mapImageGenerator.Enqueue(palEdit.Level.Index);
                }
            }

            if (a.AffectedMapLocation == EditroidAction.nullMapLocation) {
                gameView.Redraw(a.AffectedLevel, a.AffectedScreenIndex);
                mapImageGenerator.Enqueue(a.AffectedLevel, a.AffectedScreenIndex);
            } else {
                gameView.Redraw(a.AffectedMapLocation);
                mapImageGenerator.Enqueue(
                    mapView.GetLevel(a.AffectedMapLocation.X, a.AffectedMapLocation.Y),
                    gameRom.GetScreenIndex(a.AffectedMapLocation.X, a.AffectedMapLocation.Y));
            }

            ////var item = SelectedItem as ItemInstance;
            ////if ((a is SetItemRowPosition || a is SetItemTilePosition) && item != null) {
            ////    if (((ItemAction)a).ItemIndex.IsInSameScreen(item.Data.ID)) { 
            ////        ScreenEditor.MakeDefaultSelection();
            ////        ScreenEditor.Redraw();
            ////    }
            ////}

            if (a is AddOrRemoveObject || a is ChangeDoor)
                gameView.UpdateStatus();

            if (a is LevelAction && Program.StructFormIsLoaded)
                Program.StructForm.NotifyAction((LevelAction)a, undo);

            if (a is Editroid.Actions.StructureAction || a is Editroid.Actions.ModifyCombo) {
                mapImageGenerator.Enqueue(a.AffectedLevel);

            }

            if (Program.PaletteFormIsLoaded) {
                Program.PaletteForm.NotifyAction(a);
            }
            if (a is Editroid.Actions.SetPaletteColor) {
                mapImageGenerator.Enqueue(a.AffectedLevel);

            }
            if (a is EditItemProperty) {
                ItemEditingUI.NotifyAction((EditItemProperty)a, undo);
            }
            mapView.NotifyActionOccurred(a);
            passwordEditor.NotifyAction(a);
        }

        private void UpdateUndoButtons()
        {
            if(btnUndo.Enabled = undoRedo.CanUndo)
                btnUndo.Text = "Undo \"" + undoRedo.UndoText + "\"";
            else
                btnUndo.Text = "";

            if(btnRedo.Enabled = undoRedo.CanRedo)
                btnRedo.Text = "Redo \"" + undoRedo.RedoText + "\"";
            else
                btnRedo.Text = "";

        }



        string GetMapFileName() {
            return Path.ChangeExtension(cdgOpenRom.FileName, mapExtension);
        }
        string GetProjectFilename() {
            return Path.ChangeExtension(cdgOpenRom.FileName, ".nes.proj");
            
        }

        #region IParamSource Members
        Editroid.UndoRedo.EditroidUndoRedoQueue ActionGenerator.IParamSource.Queue { get { return undoRedo; } }
        Point ActionGenerator.IParamSource.MapLocation { get { return ScreenEditor.MapLocation; } }
        #endregion


        public string RomPath {
            get {
                if (gameRom == null) return null;
                return cdgOpenRom.FileName;
            }
        }



        private void EnableControls() {
            btnAltPal.Enabled = true;
            //btnSave.Enabled = true;
            //btnSaveAndPlay.Enabled = true;
            //btnSaveAs.Enabled = true;
            //btnCreateBackup.Enabled = true;
            btnShowItems.Enabled = true;
            mapView.Enabled = true;
            MemoryMenu.Enabled = true;
            //mnuSmetDoors.Enabled = true;
            //mnuExtraDoorTiles.Enabled = true;
            mnuPatches.Enabled = true;

            foreach (var item in mnuFile.DropDownItems) {
                var menuItem = item as ToolStripMenuItem;
                if (menuItem != null) {
                    menuItem.Enabled = true;
                }
            }
            foreach (var item in mnuEditors.DropDownItems) {
                var menuItem = item as ToolStripMenuItem;
                if (menuItem != null) {
                    menuItem.Enabled = true;
                }
            }
            foreach (var item in mnuTools.DropDownItems) {
                var menuItem = item as ToolStripMenuItem;
                if (menuItem != null) {
                    menuItem.Enabled = true;
                }
            }
            ////foreach(object o in btnTools.DropDownItems) {
            ////    ToolStripMenuItem i = o as ToolStripMenuItem;
            ////    if(i != null && (i.Tag == null || !i.Tag.ToString().Contains("NoEnable")))
            ////        i.Enabled = true;
            ////}

            foreach (var o in mnuView.DropDownItems) {
                ToolStripMenuItem i = o as ToolStripMenuItem;
                if (i != null && (i.Tag == null || !i.Tag.ToString().Contains("NoEnable")))
                    i.Enabled = true;
            }

            gameView.Visible = true;
            //MapView.Visible = true;
            picTitle.Visible = false;

            mnuPatternSource.Enabled = gameRom.Format.UsesStandardPpuLoader;
            btnChrUsage.Enabled = !gameRom.Format.UsesStandardPpuLoader;
            btnBankAlloc.Enabled = gameRom.Format.HasPrgAllocationTable;
            mnuChrNext.Visible = mnuChrPrev.Visible = sepChr.Visible = !gameRom.Format.UsesStandardPpuLoader;

            bool canAddDelete = gameRom.Format.SupportsEnhancedMemoryManagement;
            separator_Room.Visible = canAddDelete;
            btnAddRoom.Visible = canAddDelete;
            btnCopyroom.Visible = canAddDelete;
            btnDeleteRoom.Visible = canAddDelete;
            sepratator_Struct.Visible = canAddDelete;
            btnAddStruct.Visible = canAddDelete;
            btnDeleteStruct.Visible = canAddDelete;

            // Controls enabled dependant upon whether ROM is expanded
            mnuSmetDoors.Enabled = (gameRom.RomFormat == RomFormats.Standard);
            btnExpand.Enabled = (gameRom.RomFormat == RomFormats.Standard || gameRom.RomFormat == RomFormats.Expando || gameRom.RomFormat == RomFormats.Enhanco);
            btnAdvancedItems.Enabled = gameRom.Format.SupportsEnhancedMemoryManagement;
        }

        public bool Plugin_SaveRom() {
            // Todo: consider some kind of "Allow plugins to save the ROM?" check
            return SaveRom();
        }

        private bool SaveRom() {
            if (Program.CodeEditorLoaded) Program.CodeEditor.SaveCurrentDocument();

            bool cancel;
            OnRomSaving(out cancel);
            if (cancel) return false;

            if (File.Exists(cdgOpenRom.FileName)) {
                FileAttributes atts = File.GetAttributes(cdgOpenRom.FileName);
                if ((atts & FileAttributes.ReadOnly) != 0) {
                    if (MessageBox.Show("The ROM file is read-only. It can not be written to unless the read-only attribute is disabled. Would you like to do this now?", "Error Saving File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        File.SetAttributes(cdgOpenRom.FileName, atts & ~FileAttributes.ReadOnly);
                    }
                }
            }
            using (MemoryStream memStream = new MemoryStream(gameRom.data)) {
                SaveMap(memStream, GetMapFileName());
            }
            using (FileStream gameStream = new FileStream(cdgOpenRom.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)) {
                gameRom.SaveTo(gameStream);
            }
            if (gameProject != null) {
                if (gameRom.Format.HasChrAnimationTable)
                    gameProject.ChrAnimationNameList = gameRom.GetChrAnimationNames();

                var projectFilePath = GetProjectFilename();
                using (var projStream = new FileStream(projectFilePath, FileMode.Create, FileAccess.Write)) {
                    gameProject.Save(projStream);
                    gameProject.UnsavedChanges = false;
                }
            }


            mapView.ReloadItemData();
            lastWriteTime = File.GetLastWriteTime(RomFileName);

            OnRomSaved();

            return true;
        }


        /// <summary>
        /// Saves the editor map for a ROM.
        /// </summary>
        /// <param name="gameStream">The filestream the game is saved to.</param>
        /// <param name="filename">The filename to use if the map is to be saved to an external file.</param>
        /// <remarks>If an extraneous map is found (there is one in both the ROM image and an external file) it will be deleted.</remarks>
        private void SaveMap(Stream  mapStream, string filename) {
            bool saveToRom = SaveMapToROM;
            if(gameRom.Format.MapLocation == Editroid.ROM.Formats.FormatMapLocation.Bank_0F){
                saveToRom = true;
            }else if(gameRom.Format.MapLocation != Editroid.ROM.Formats.FormatMapLocation.Unspecified){
                // This exception should never get thrown, but if we ever add a 
                // new FormatMapLocation, we'll throw so we can't overlook it here.
                throw new Exception("Unexpected FormatMapLocation value.");
            }

            ////bool saveToRom = gameRom.ExpandedRom ? true : SaveMapToROM;
            SaveMap(mapStream, filename, saveToRom);
        }

        const int ExpandedOrEnhanced_MapDataOffset = 0x38010;
        const int Standard_MapDataOffset = 0x20010;

        /// <summary>
        /// Saves the editor map for a ROM.
        /// </summary>
        /// <param name="gameStream">The filestream the game is saved to.</param>
        /// <param name="filename">The filename to use if the map is to be saved to an external file.</param>
        /// <param name="saveMapToRom">Whether to save to a stream or the ROM.</param>
        /// <remarks>If an extraneous map is found (there is one in both the ROM image and an external file) it will be deleted.</remarks>
        private void SaveMap(Stream gameStream, string filename, bool saveMapToRom) {
            if (saveMapToRom) {
                ////if (filename != null && File.Exists(filename) && MessageBox.Show("Delete external .MAP file?", "Editroid", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                ////    // Delete separate map file
                ////    try {
                ////        File.Delete(filename);
                ////    } catch (Exception) {
                ////        // Ignore
                ////    }
                ////}

                // If the ROM is expanded or enhanced, we have memory specifically allocated for the map within the ROM. Otherwise it just goes on the end.
                bool expanded = gameStream.Length >= MetroidRom.ExpandedRomSize;
                if (expanded) {
                    gameStream.Seek(ExpandedOrEnhanced_MapDataOffset, SeekOrigin.Begin);
                } else {
                    gameStream.Seek(Standard_MapDataOffset, SeekOrigin.Begin);
                }
                
                SaveMapToStream(gameStream);
            } else {
                FileStream mapStream = null;
                try {
                    mapStream = new FileStream(filename, FileMode.Create);
                    SaveMapToStream(mapStream);
                } catch (Exception ex) {
                    MessageBox.Show("The editor map could not be saved.", ex.GetType().ToString());
                } finally {
                    if (mapStream != null)
                        mapStream.Close();
                }
            }
        }

        private void SaveMapToStream(Stream stream) {
            mapView.SaveData(stream);
        }





        internal void InitializePaletteEditorData(frmPalette paletteForm) {
            if (ScreenEditor.CanEdit)
                paletteForm.LevelData = gameRom.GetLevel(ScreenEditor.LevelIndex);
            else
                paletteForm.LevelData = gameRom.Brinstar;

        }


        frmPointer pointerForm;


        frmTitleText titleForm;

        MapMaker mapMaker = new MapMaker();

        frmMapArea mapForm;
        frmMapStatus progress;

        private void mnuMap_Click(object sender, EventArgs e) {
            if(mapForm == null || mapForm.IsDisposed)
                mapForm = new frmMapArea();
            mapForm.MapControl = mapView;


            if(Program.Dialogs.ShowDialog(mapForm, this) == DialogResult.OK) {
                // Load data needed for map
                mapMaker.Rom = gameRom;
                mapMaker.Scale = mapForm.DrawScale;
                mapMaker.MapArea = mapForm.MapSelection;
                mapMaker.LevelMap = mapView;
                mapMaker.ShowPhysics = mapForm.ShowPhysics;
                mapMaker.HighQuality = mapForm.Quality;
                mapMaker.HideEnemies = mapForm.HideEnemies;
                mapMaker.LevelFilter = mapForm.LevelFilter;
                mapMaker.FillEmptySpots = mapForm.FillEmptySpots;

                // Display progress form
                progress = new frmMapStatus();
                progress.RoomCount = mapMaker.MapArea.Width * mapMaker.MapArea.Height;
                progress.Show();
                Application.DoEvents();

                // Create Map
                MapThread.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Called each time a map room is drawn when a map is being created to update progress.
        /// </summary>
        void OnMapRoomDrawn(object sender, EventArgs e) {
            MapThread.ReportProgress(0);
        }

        #region Map Making
        Bitmap map = null;
        private void MapThread_DoWork(object sender, DoWorkEventArgs e) {
            map = mapMaker.Render();

        }
        private void MapThread_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progress.CompleteRoom();
        }

        private void MapThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            progress.Close();

            // Display Map
            frmMap mapDisplay = new frmMap();
            mapDisplay.MapImage = map;
            Application.DoEvents();
            try {
                mapDisplay.Show();
            }catch(Exception ex)
                {
            }
        }
        #endregion



        private void MapEditor_selectedRoomModified(object sender, EventArgs e) {
        }

        private void MapEditor_RoomSet(object sender, int X, int Y) {
            if (mapView.GetLevel(X, Y) != LevelIndex.None && gameRom.GetScreenIndex(X, Y) == 0xFF) {
                gameRom.SetScreenIndex(X, Y, 0);
            }
        }


        private void txtFeedback_TextChanged(object sender, EventArgs e) {
        }


        public LevelIndex CurrentLevel {
            get { return ScreenEditor.LevelIndex; }
        }

        private void ScreenGrid_SelectedObjectDragged(object sender, EventArgs e) {
        }


        private void MapView_SelectionChanged(object sender, int X, int Y) {
            gameView.SetViewportLocation(new Point(X*16, Y*16*15/16));
            
        }

        private void MapView_BeginDragSelection(object sender, EventArgs e) {
            gameView.BeginExternalScroll();
        }

        private void MapView_EndDragSelection(object sender, EventArgs e) {
            gameView.EndExternalScroll();
        }


        const int undoredoListMaxCount = 16;
        private void btnUndo_DropDownOpening(object sender, EventArgs e) {
            PopulateUndoRedoDropDown(btnUndo, undoRedo.Undos);
        }

        private void PopulateUndoRedoDropDown(ToolStripSplitButton menu, EditroidUndoRedoQueue.ActionStack actions) {
            int count = Math.Min(undoredoListMaxCount, actions.Count);
            
            while (menu.DropDownItems.Count < count) {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Tag = menu.DropDownItems.Count;
                menu.DropDownItems.Add(item);
            }

            for (int i = 0; i < count; i++) {
                menu.DropDownItems[i].Text = actions[i].GetText();
                menu.DropDownItems[i].Visible = true;
            }
            for (int i = count; i < menu.DropDownItems.Count; i++) {
                menu.DropDownItems[i].Visible = false;
            }
        }

        private void btnRedo_DropDownOpening(object sender, EventArgs e) {
            PopulateUndoRedoDropDown(btnRedo, undoRedo.Redos);
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e) {
            //ItemDataEditor.LoadItems(LevelIndex.None);
            mapView.ItemDisplay.ClearItemDisplay();
            HidePasswordData();
            CheckItemEditorMenuItem(sender as ToolStripMenuItem);
        }

        private void brinstarToolStripMenuItem_Click(object sender, EventArgs e) {
            //ItemDataEditor.LoadItems(LevelIndex.Brinstar);
            mapView.ItemDisplay.LoadLevel(LevelIndex.Brinstar);
            CheckItemEditorMenuItem(sender as ToolStripMenuItem);
            HidePasswordData();
            ShowMap();
        }

        private void norfairToolStripMenuItem_Click(object sender, EventArgs e) {
            //ItemDataEditor.LoadItems(LevelIndex.Norfair);
            mapView.ItemDisplay.LoadLevel(LevelIndex.Norfair);

            CheckItemEditorMenuItem(sender as ToolStripMenuItem);
            HidePasswordData();
            ShowMap();
        }

        private void ridleyToolStripMenuItem_Click(object sender, EventArgs e) {
            //ItemDataEditor.LoadItems(LevelIndex.Ridley);
            mapView.ItemDisplay.LoadLevel(LevelIndex.Ridley);
            CheckItemEditorMenuItem(sender as ToolStripMenuItem);
            HidePasswordData();
            ShowMap();
        }

        private void kraidToolStripMenuItem_Click(object sender, EventArgs e) {
            //ItemDataEditor.LoadItems(LevelIndex.Kraid);
            mapView.ItemDisplay.LoadLevel(LevelIndex.Kraid);
            CheckItemEditorMenuItem(sender as ToolStripMenuItem);
            HidePasswordData();
            ShowMap();
        }

        private void TourianToolStripMenuItem_Click(object sender, EventArgs e) {
            //ItemDataEditor.LoadItems(LevelIndex.Tourian);
            mapView.ItemDisplay.LoadLevel(LevelIndex.Tourian);
            CheckItemEditorMenuItem(sender as ToolStripMenuItem);
            HidePasswordData();
            ShowMap();
        }

        private void CheckItemEditorMenuItem(ToolStripMenuItem menuItem) {
            btnNoItems.Checked =
                btnBrinstarItems.Checked =
                btnNorfairItems.Checked =
                btnRidleyItems.Checked =
                btnKraidItems.Checked =
                btnTourianItems.Checked =
                false;

            if (menuItem != null) menuItem.Checked = true;
        }






        internal void CenterOnScreen(Point mapPosition) {
            Size viewSize = gameView.Size;
            
            if (viewSize.Height < mapView.Height * 2) {
                // If map obscures the center area, we will move the "center" to the left
                if (viewSize.Width > 512)
                    viewSize.Width -= mapView.Width;
                else
                    viewSize.Width -= (512 - viewSize.Width) / 2;
            ////} else if (ItemDataEditor.Visible == true) {
            ////    // If the gameItem editing panel obscures the view, move center left
            ////    viewSize.Width -= 256;
            } else if (viewSize.Width < 480) {
                // If the window is too narrow, move the center up
                ////if (ItemDataEditor.Visible == true) {
                ////    if (viewSize.Height > 752)
                ////        viewSize.Height -= 512;
                ////    else
                ////        viewSize.Height -= (752 - viewSize.Height) / 2;
                ////} else {
                if (viewSize.Height > 480)
                    viewSize.Height -= 240;
                else
                    viewSize.Height -= (480 - viewSize.Height) / 2;
                ////}
            }

            Point ScreenCenterPoint = new Point(
                mapPosition.X * ScreenEditor.CellSize.Width + ScreenEditor.CellSize.Width / 2,
                mapPosition.Y * ScreenEditor.CellSize.Height + ScreenEditor.CellSize.Height / 2);
            Point ViewWindowCorner = new Point(
                ScreenCenterPoint.X - viewSize.Width / 2,
                ScreenCenterPoint.Y - viewSize.Height / 2);

            gameView.SetViewportLocation(ViewWindowCorner);
        }

        /// <summary>
        /// Selects an item by index if there is already an item selected.
        /// </summary>
        /// <param name="selectedIndexCollection"></param>
        internal void SelectItemInScreen(int index) {
            ItemInstance item = ScreenEditor.SelectedItem as ItemInstance;
            if (item == null) return;

            var screen = ScreenEditor.ScreenItems;
            if (index >= screen.Items.Count) return;

            var specifiedItem = screen.Items[index];

            ScreenEditor.SelectItem(specifiedItem);

        }
        void OnItemUiItemSelected(object sender, EventArgs e) {
            SelectItemInScreen(ItemEditingUI.SelectedItemIndex);
        }

        private void btnPassData_Click(object sender, EventArgs e) {
            CheckItemEditorMenuItem(btnPassData);
            ShowMap();
            ShowPasswordData();
        }

        private void HidePasswordData() {
            mapView.PasswordDisplay.Visible = false;
            passwordEditor.Visible = false;
            //ItemDataEditor.Enabled = true;

            if (btnPassData.Checked)
                btnPassData.Checked = false;

        }

        private void ShowPasswordData() {
            if (!btnNoItems.Checked) {
                mapView.ItemDisplay.LoadLevel(LevelIndex.None);
                CheckItemEditorMenuItem(btnPassData);
            }

            mapView.PasswordDisplay.Visible = true;
            passwordEditor.Visible = true;
            passwordEditor.BringToFront();
            //ItemDataEditor.Enabled = false;
            
            if (!btnPassData.Checked)
                btnPassData.Checked = true;
        }

        private void mapView_PasswordEntrySelected(object sender, EventArgs e) {
            passwordEditor.SelectEntry(mapView.SelectedPasswordDataIndex);
        }

        private void passwordEditor_EntrySelected(object sender, EventArgs e) {
            mapView.SelectedPasswordDataIndex = passwordEditor.SelectedEntryIndex;
        }


        private void btnAddRightDoor_Click(object sender, EventArgs e) {
            PerformAction(actions.ChangeDoor(DoorSide.Right, DoorType.Normal));
        }

        private void btnAddLeftDoor_Click(object sender, EventArgs e) {
            PerformAction(actions.ChangeDoor(DoorSide.Left, DoorType.Normal));
        }

        private void btnPasswordData_Click(object sender, EventArgs e) {
            PasswordDataGenerator passwordForm = new PasswordDataGenerator();
            passwordForm.LoadRom(gameRom,mapView);

            LockMap();
            Program.Dialogs.ShowDialog( passwordForm, this);
            UnlockMap();
        }

        private void btnSaveAs_Click(object sender, EventArgs e) {
            RomSaver.FileName = cdgOpenRom.FileName;
            if (RomSaver.ShowDialog() == DialogResult.OK) {
                RomFileName = RomSaver.FileName;
                cdgOpenRom.FileName = RomSaver.FileName;
                UpdateFormCaption();

                SaveRom();
            }
        }

        string RomFileName {
            get { return cdgOpenRom.FileName; }
            set {
                cdgOpenRom.FileName = value;
                fileWatcher.Path = Path.GetDirectoryName(value);
                fileWatcher.Filter = Path.GetFileName(value);
            }
        }


        DateTime lastWriteTime;
                bool processingChange = false;

        private void fileWatcher_Changed(object sender, FileSystemEventArgs e) {
            if (processingChange) return;
            processingChange = true;

            try {
                DateTime writeTime = File.GetLastWriteTime(RomFileName);
                bool changed = writeTime != lastWriteTime;

                if (changed) {
                    gameRom.SerializeAllData();

                    if (File.Exists(RomFileName)) {
                        byte[] newFile = TryLoadChangedFile();
                        if (newFile == null) return;

                        IList<MetroidRom.ChangeRange> externalChanges = MetroidRom.CompareVersions(gameRom.SavedVersion, newFile);
                        IList<MetroidRom.ChangeRange> unsavedChanges = gameRom.CompareToSavedVersion();

                        // If we don't find any changes, nevermind
                        if (externalChanges == null || externalChanges.Count == 0) return;

                        using (frmFileChangeWarning dialog = new frmFileChangeWarning()) {
                            dialog.HasUnsavedChanges = unsavedChanges.Count > 0;
                            dialog.SetChangedBytes(GetTotalBytes(externalChanges));
                            dialog.SetUnsavedBytes(GetTotalBytes(unsavedChanges));
                            dialog.BringToFront();
                            Program.Dialogs.ShowDialog(dialog,this);

                            PerformFileChangedActions(dialog, externalChanges, unsavedChanges);
                        }
                    }
                }
            } finally {
                processingChange = false;
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            ////if (!settings.HasRunBefore_v3A0) {
            ////    settings.HasRunBefore_v3A0 = true;

            ////    ShowIntroForm();
            ////}
        }

        /// <summary>Performs actions specified by user via a FileChangeWarning window after a file is changed externally.</summary>
        private void PerformFileChangedActions(frmFileChangeWarning dialog, IList<MetroidRom.ChangeRange> externalChanges, IList<MetroidRom.ChangeRange> unsavedChanges) {
            if (dialog.Action == FileChangeAction.Ignore) return; 

            if (dialog.Action == FileChangeAction.Reload || dialog.Action == FileChangeAction.LoadDisk)
                PerformOpen();
            if (dialog.Action == FileChangeAction.LoadSaved) {
                PerformOpen(new MemoryStream(gameRom.SavedVersion), null);
            }
            if (dialog.Action == FileChangeAction.LoadPatternsOnly) {
                try {
                    using (FileStream s = new FileStream(RomFileName, FileMode.Open)) {
                        gameRom.ReloadPatterns(s);
                    }
                } catch (Exception x) {
                    MessageBox.Show("An error occurred while loading the patterns (" + x.GetType().ToString() + ":" + x.Message + ").");
                }

                //The following will be done by PerformOpen in the above cases
                CloseWindows();

                ClearUndo();
                CheckItemEditorMenuItem(btnNoItems);
                gameView.RedrawAll();
            }
            UpdateFormCaption();
        }

        private static void CloseWindows() {
            Program.Dialogs.CloseAllDialogs();
            if (Program.StructFormIsLoaded) Program.StructForm.Close();
            if (Program.PaletteFormIsLoaded) Program.PaletteForm.Close();
            if (Program.PointerExplorerFormIsLoaded) Program.PointerExplorerForm.Close();
            if (Program.AdvancedPalFormIsLoaded) Program.AdvancedPalForm.Close();
            if (Program.CodeEditorLoaded) Program.CodeEditor.Close();

        }

        private int GetTotalBytes(IList<MetroidRom.ChangeRange> changes) {
            int result = 0;
            foreach(MetroidRom.ChangeRange c in changes){
                result += c.length;
            }

            return result;
        }

        private byte[] TryLoadChangedFile() {
            Thread.Sleep(0);
            DateTime start = DateTime.Now;


            // ... is this a good idea?
            while ((DateTime.Now - start).TotalMilliseconds < 1000) {
                try {
                    return File.ReadAllBytes(RomFileName);
                } catch (IOException) {
                }
            }
            

            return null;
        }


        private void btnBoss_Click(object sender, EventArgs e) {
            PerformAction(Actions.ToggleIsBoss());
        }

        private void btnDropSamus_Click(object sender, EventArgs e) {
            LaunchTestRom();
        }

        private void btnDropSamusOptions_Click(object sender, EventArgs e) {
            ShowDropSamusOptions();
        }

        private void LaunchTestRom() {
            string testRomFilename = "test rom.nes";
            var folder = Program.TemporaryFileManager.GetTemporaryFolder();
            string romName = testRomFilename;

            if (ScreenEditor == null || !ScreenEditor.CanEdit || ScreenEditor.LevelIndex == LevelIndex.None) return;

            Program.RomPatcher.StartMapLocation = ScreenEditor.MapLocation;
            Program.RomPatcher.StartLevel = ScreenEditor.LevelIndex;

            gameRom.SerializeAllData();
            ////if (gameRom.RomFormat == RomFormat.Standard || gameRom.RomFormat == RomFormat.Expando) {
            ////    foreach (Level l in gameRom.Levels.Values)
            ////        l.ItemTable_DEPRECATED.ResortEntries();
            ////}
            mapView.ReloadItemData();

            byte[] romToPatch = gameRom.data;
            Program.RomPatcher.Rom = gameRom;

            if (gameProject != null && mnuBuildoptionsBuildontest.Checked) {
                gameProject.DebugOutputDirectory = folder.Path;
                var buildRom = PerformBuild(testRomFilename);
                if (buildRom == null) return; // Can not test a play a ROM that failed to build

                romToPatch = buildRom.data;
                Program.RomPatcher.Rom = buildRom;
            }

            byte[] testRom = Program.RomPatcher.CreatePatchedRom();

            string err;
            if (folder.WriteFile(romName, testRom, out err)) {
                LaunchTestRom(folder.Path, romName, folder.GetFullPath(romName));
            } else {
                MessageBox.Show(string.Format("Could not save test rom ({0}).", err), "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //string tempFile = Path.GetTempFileName();
            //string newFile = Path.GetTempFileName();
            //string newFileNes = newFile + ".nes";
            //while (File.Exists(newFileNes)) {
            //    newFile += "q";
            //    newFileNes = newFile + ".nes";
            //}
            //File.Move(tempFile, newFileNes);
            //File.WriteAllBytes(newFileNes, testRom);
            //temporaryFiles.Add(newFileNes);
            //LaunchRom(newFileNes);
        }
        private void LaunchRom(string romFullPath) {
            try {
                Process.Start(romFullPath);
            } catch (Win32Exception x) {
                if (x.Message.IndexOf("associated", StringComparison.InvariantCultureIgnoreCase) != -1) {
                    MessageBox.Show("Could not launch the ROM. An emulator must be associated with the .NES extension.");
                } else {
                    MessageBox.Show("Could not launch the ROM (error: " + x.Message + ".");
                }
            }
        }
        private void LaunchTestRom(string romPath, string romName, string romFullPath) {
            try {
                string command = settings.TestRoomCommand;
                if (string.IsNullOrEmpty(command)) command = "%d";
                command = command.Replace("$d", "{0}");
                command = command.Replace("$p", "{1}");
                command = command.Replace("$r", "{2}");
                command = command.Replace("$f", "{3}");
                command = string.Format(command, "\"" + romFullPath + "\"", romFullPath, romName, romPath);

                ProcessStartInfo p = new ProcessStartInfo();
                p.FileName = command;
                p.UseShellExecute = false;
                //Process.Start(p);
                
                //var result = Windows.Kernel32.CreateProcess(null, command);
                //if (result == null) {
                //    var error =  System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                //    var errdesc = new Win32Exception(error).Message;
                //    MessageBox.Show("Could not launch the ROM: " + errdesc);
                //}
                Program.ExecuteCommand(command, settings.DisplayCmdForTestRoom);
            } catch (Win32Exception x) {
                if (x.Message.IndexOf("associated", StringComparison.InvariantCultureIgnoreCase) != -1) {
                    MessageBox.Show("Could not launch the ROM. An emulator must be associated with the .NES extension.");
                } else {
                    MessageBox.Show("Could not launch the ROM (error: " + x.Message + ".");
                }
            }
        }
        private void ShowDropSamusOptions() {
            if (!ScreenEditor.CanEdit || ScreenEditor.LevelIndex == LevelIndex.None) return;

            SamusDropForm frm = new SamusDropForm();
            frm.Patcher = Program.RomPatcher;
            frm.SetImage(ScreenEditor.GetScreenImage());
            Program.Dialogs.ShowDialog(frm, this);

            if (frm.LaunchTestOnClose)
                LaunchTestRom();
        }

        private void gameView_LaunchPreview(object sender, PreviewEventArgs e) {
            Point start = e.StartPosition;
            start.Offset(0, -16);
            Program.RomPatcher.InitialScreenPosition = start;

            if (e.ShowOptions)
                ShowDropSamusOptions();
            else
                LaunchTestRom();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e) {
            TileSourceEditor frm = new TileSourceEditor();
            frm.Rom = gameRom;

            Program.Dialogs.ShowDialog(frm, this);
        }

        ////private void brinstarToolStripMenuItem_Click_1(object sender, EventArgs e) {
        ////    PerformAction(Actions.ChangeLevel(LevelIndex.Brinstar));
        ////}
        ////private void norfairToolStripMenuItem_Click_1(object sender, EventArgs e) {
        ////    PerformAction(Actions.ChangeLevel(LevelIndex.Norfair));
        ////}
        ////private void ridleyToolStripMenuItem_Click_1(object sender, EventArgs e) {
        ////    PerformAction(Actions.ChangeLevel(LevelIndex.Ridley));
        ////}
        ////private void kraidToolStripMenuItem_Click_1(object sender, EventArgs e) {
        ////    PerformAction(Actions.ChangeLevel(LevelIndex.Kraid));
        ////}
        ////private void TourianToolStripMenuItem_Click_1(object sender, EventArgs e) {
        ////    PerformAction(Actions.ChangeLevel(LevelIndex.Tourian));
        ////}
        private void blankToolStripMenuItem_Click(object sender, EventArgs e) {
            PerformAction(Actions.ChangeLevel(LevelIndex.None));
        }

        private void toolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

        private void btnNewFeatures_Click(object sender, EventArgs e) {
            ShowNewFeatures();
        }

        private void ShowNewFeatures() {
            foreach (var frm in Application.OpenForms) {
                if (frm is frmIntro) {
                    ((Form)frm).BringToFront();
                    return;
                }
            }
            ShowIntroForm();
        }

        private void ShowIntroForm() {
            frmIntro introForm = new frmIntro();
            introForm.Owner = this;
            introForm.Show();

        }

        private void gameView_PasswordIconClicked(object sender, EventArgs e) {
            ShowPasswordData();
            mapView.SelectPasswordDat(ScreenEditor.MapLocation);
        }

        private void superMetroidDoorPatchToolStripMenuItem_Click(object sender, EventArgs e) {
            if (PatchForm.TryApplyPatch(new SmetDoorPatch(gameRom.ExpandedRom), gameRom,this)) {
                ReloadCurrentRomFromMemory();
            }
        }

        private void extraDoorTilesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (PatchForm.TryApplyPatch(new DoorExpansionPatch(gameRom.ExpandedRom), gameRom, this)) {
                ReloadCurrentRomFromMemory();
            }

        }
        private void mnuScrollPatch_Click(object sender, EventArgs e) {
            PatchForm.TryApplyPatch(new DoorScrollPatch(gameRom.ExpandedRom), gameRom, this);
        }

        private void btnAddRoom_Click(object sender, EventArgs e) {
            ClearUndo();
            
            // Use new room in selected screen
            int roomIndex = currentLevelData.AddRoom();
            gameRom.SetScreenIndex(ScreenEditor.MapLocation.X, ScreenEditor.MapLocation.Y, (byte) roomIndex);
            
            // Update editors
            ScreenEditor.RefreshData();
            gameView.Redraw(ScreenEditor.MapLocation);
            UpdateSelectionStatus();
        }

        private void btnCopyroom_Click(object sender, EventArgs e) {
            ClearUndo();
            
            // Use new room in selected screen
            int roomIndex = currentLevelData.CloneRoom(ScreenEditor.ScreenIndex);
            gameRom.SetScreenIndex(ScreenEditor.MapLocation.X, ScreenEditor.MapLocation.Y, (byte)roomIndex);

            // Update editors
            ScreenEditor.RefreshData();
            gameView.Redraw(ScreenEditor.MapLocation);

            UpdateSelectionStatus();
        }

        private void tourainToolStripMenuItem_Click_1(object sender, EventArgs e) {

        }

        private void tourainToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void btnDeleteRoom_Click(object sender, EventArgs e) {
            //var response = MessageBox.Show("Editroid will delete the data for this room. Would you" + Environment.NewLine + "like update the map too?", "Delete Room", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            var response = DialogResult.Yes;

            if (response == DialogResult.Yes || response == DialogResult.No) {
                bool updateIndecies = response == DialogResult.Yes;

                ClearUndo();
                int deletedRoomIndex = ScreenEditor.ScreenIndex;
                var currentLevel = CurrentLevel;
                gameRom.Levels[currentLevel].DeleteScreen(deletedRoomIndex);

                if (updateIndecies) {
                    for (int x = 0; x < 32; x++) {
                        for (int y = 0; y < 32; y++) {
                            if (mapView.GetLevel(x, y) == currentLevel) {
                                int oldScreenIndex=gameRom.GetScreenIndex(x, y);
                                if (oldScreenIndex == deletedRoomIndex) { // Remove all instances of deleted room from map
                                    gameRom.SetScreenIndex(x, y, 0xFF);
                                    mapView.SetLevel(x, y, LevelIndex.None);
                                } else if (oldScreenIndex > deletedRoomIndex && oldScreenIndex != 0xFF) { // Update any screen whose index has changed
                                    gameRom.SetScreenIndex(x, y, (byte)(oldScreenIndex - 1));
                                }
                            }
                        }
                    }
                }

                mapImageGenerator.Enqueue(CurrentLevel);
                gameView.RedrawAll();
                gameView.UpdateStatus();
                if (updateIndecies || deletedRoomIndex >= gameRom.Levels[currentLevel].Screens.Count) gameView.HideStatus(); // If room is removed from map, hide status panel                
                UpdateSelectionStatus();
            }
        }

        private void btnAddStruct_Click(object sender, EventArgs e) {
            ClearUndo();

            int newStructIndex = currentLevelData.AddStructure();
            if (ScreenEditor.CanEdit && ScreenEditor.SelectedItem is StructInstance) {
                var s = ScreenEditor.SelectedItem as StructInstance;
                s.ObjectType = newStructIndex;
                ScreenEditor.Redraw();

                if (Program.StructFormIsLoaded) {
                    Program.StructForm.StructIndex = newStructIndex;
                }
            }

            structurePicker1.ReloadData();
            UpdateSelectionStatus();

        }

        private void btnDeleteStruct_Click(object sender, EventArgs e) {
            ClearUndo();

            if (ScreenEditor.SelectedItem == null) {
                Debug.Fail("Attempted to delete a struct when none was selected.");
                return;
            }

            int deletedIndex = ScreenEditor.SelectedItem.GetTypeIndex();
            currentLevelData.DeleteStructure(deletedIndex);
            if (Program.StructFormIsLoaded) {
                Program.StructForm.NotifyStructureDeleted(CurrentLevel, deletedIndex);
            }

            structurePicker1.ReloadData();
            mapImageGenerator.Enqueue(CurrentLevel);
            gameView.Redraw(CurrentLevel);
        }

        private void newBtnHelp_Click(object sender, EventArgs e) {
            ShowNewFeatures();
        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e) {
            string disasm = ItemDataDisassembler.GetItemDisassembly(currentLevelData, ItemDataDisassembler.DataDirective.DotByte);
            TextForm.ShowText(currentLevelData.Index.ToString() + " Item Disassembly", disasm);
        }

        private void gameView_DefaultPaletteIconClicked(object sender, EventArgs e) {
            if (ScreenEditor != null && ScreenEditor.CanEdit) {
                PerformAction(actions.NextCAT());
            }
        }

        private void advancedPaletteEditorToolStripMenuItem_Click(object sender, EventArgs e) {
            if(!Program.AdvancedPalFormIsLoaded){
                Program.CreateAdvancedPalForm();
                Program.AdvancedPalForm.Show();
            }

            if(currentLevelData == null)
                Program.AdvancedPalForm.LoadPalettes(gameRom.Brinstar);
            else
            Program.AdvancedPalForm.LoadPalettes(currentLevelData);
        }

        private void advancedItemEditorToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var form = new frmAdvancedItems()) {
                var levelToEdit = currentLevelData ?? gameRom.Brinstar;
                form.LoadItemData(levelToEdit);
                form.SetMapImage(mapView.Image);
                Program.Dialogs.ShowDialog(form, this);

                ItemDataEditor.UnloadScreen();
                ItemEditingUI.DisplayScheme(ItemUiScheme.Blank);

                if (mapView.ItemDisplay.IsLoaded && mapView.ItemDisplay.CurrentLevel == levelToEdit.Index) {
                    mapView.ItemDisplay.LoadLevel(levelToEdit.Index);
                }
            }
        }

        private void btnExpand_Click(object sender, EventArgs e) {
            using (ExpandoDialog dlg = new ExpandoDialog()) {
                dlg.CurrentRomFormat = gameRom.RomFormat;
                dlg.ShowMMC3Options = gameProject != null;

                if (Program.Dialogs.ShowDialog(dlg, this) == DialogResult.OK) {
                    bool updateExpansion = dlg.UpdateExpansionFile;
                    bool updateFixed = dlg.UpdateFixedBankRefs;

                    // Roms that have not been expanded at all must first be converted to an "expando" ROM
                    if (gameRom.RomFormat == RomFormats.Standard) {
                        Editroid.ROM.Expando.Expander expander = new Editroid.ROM.Expando.Expander(gameRom);
                        byte[] buffer = expander.ExpandedRomData;
                        SerializeMapToExpandedRom(buffer);

                        ReloadCurrentRomFromMemory(expander.ExpandedRomData, gameProject);
                    }

                    // "Expando" (1/2 expanded) are then converted to "enhanco" (fully expanded)
                    if (gameRom.RomFormat == RomFormats.Expando) {
                        Editroid.ROM.Enhanced.Enhancer enhancer = new Editroid.ROM.Enhanced.Enhancer(gameRom);
                        byte[] buffer = enhancer.CreateEnhancedRom();

                        ReloadCurrentRomFromMemory(buffer, gameProject);
                    }

                    // Enhanced ROM expanded to MMC3
                    if (gameRom.RomFormat == RomFormats.Enhanco) {
                        var expander = new Editroid.ROM.MMC3.MMC3Expander(gameRom, gameProject);
                        expander.UpdateExpansionFile = updateExpansion;
                        expander.UpdateFixedBankReferences = updateFixed;

                        Editroid.ROM.MMC3.MMC3Expander.PostWorkDelegate finalizationWork;
                        byte[] buffer = expander.ExpandRom(out finalizationWork);

                        ReloadCurrentRomFromMemory(buffer, gameProject);
                        finalizationWork(gameRom, gameProject);

                    }
                }
            }
            UpdateFormCaption();
            mnuProjectCreate.Enabled = (gameRom.RomFormat == RomFormats.Enhanco || gameRom.RomFormat == RomFormats.MMC3);

        }

        /// <summary>
        /// Saves the map data to a rom image. THIS ONLY WORKS ON EXPANDED ROMS.
        /// </summary>
        /// <param name="buffer"></param>
        private void SerializeMapToExpandedRom(byte[] buffer) {
            MemoryStream gameData = new MemoryStream(buffer);
            SaveMap(gameData, null, true);
        }

        private void btnBrinstar_Click(object sender, EventArgs e) {
            PerformAction(Actions.ChangeLevel(LevelIndex.Brinstar));
        }

        private void btnKraid_Click(object sender, EventArgs e) {
            PerformAction(Actions.ChangeLevel(LevelIndex.Kraid));
        }

        private void btnNorfair_Click(object sender, EventArgs e) {
            PerformAction(Actions.ChangeLevel(LevelIndex.Norfair));
        }

        private void btnRidley_Click(object sender, EventArgs e) {
            PerformAction(Actions.ChangeLevel(LevelIndex.Ridley));
        }

        private void btnTourian_Click(object sender, EventArgs e) {
            PerformAction(Actions.ChangeLevel(LevelIndex.Tourian));
        }

        private void enhancedRomTestToolStripMenuItem_Click(object sender, EventArgs e) {
            //Editroid.ROM.Expando.Expander expander = new Editroid.ROM.Expando.Expander(gameRom);
            gameRom.SerializeAllData();

            Editroid.ROM.Enhanced.Enhancer enhancer = new Editroid.ROM.Enhanced.Enhancer(gameRom);
            byte[] buffer = enhancer.CreateEnhancedRom();
            using (SaveFileDialog sfd = new SaveFileDialog()) {
                if (sfd.ShowDialog() == DialogResult.OK) {
                    File.WriteAllBytes(sfd.FileName, buffer);
                    Process.Start(sfd.FileName);
                }
            }
            //SerializeMapTo(buffer);
            //ReloadCurrentRomFromMemory(expander.ExpandedRomData);
            //File.WriteAllBytes("D:\\Hack\\met.nes", expander.ExpandedRomData);
        }

        private void picMap_Click(object sender, EventArgs e) {
            ShowMap();
        }

        private void picMap_MouseHover(object sender, EventArgs e) {
            ShowMap();
        }

        private void LockMap() {
            mapView.LockVisible();
        }
        private void UnlockMap() {
            mapView.UnlockVisible();
        }
        private void ShowMap() {
            mapView.Show();
        }

        private void btnSaveSansMap_Click(object sender, EventArgs e) {
            RomSaver.FileName = cdgOpenRom.FileName;
            if (RomSaver.ShowDialog() == DialogResult.OK) {
                string filename = RomSaver.FileName;
                    byte[] tempFile = new byte[0x20010];
                    Array.Copy(gameRom.data, 0, tempFile, 0, tempFile.Length);
                    try {
                        File.WriteAllBytes(filename, tempFile);
                    }catch (UnauthorizedAccessException){
                        MessageBox.Show("Editroid is unauthorized to access this file or directory.");
                    } catch (IOException) {
                        MessageBox.Show("An error occurred writing the file.");
                    }
            }
        }

        private void btnGrid_Click(object sender, EventArgs e) {
            var editor = gameView.FocusedEditor;

            if (editor.CanEdit && editor.SelectedItem is StructInstance) {
                var selection = editor.SelectedItem as StructInstance;

                frmStructBrowser brahzer = new frmStructBrowser();
                brahzer.UseAlternatePalette = btnAltPal.Checked;
                var result = brahzer.ShowStructs(editor.Level,selection.PalData,selection.ObjectType - (selection.ObjectType % 16),selection.ObjectType);
                if (result >= 0) {
                    PerformAction(actions.SetType(result));
                }

                // Stupid graphical glitch in toolstrip when calling showdialog
                btnGrid.Enabled = false;
                btnGrid.Enabled = true;

                brahzer.Dispose();
            }
        }

        private void btnScreenGrid_Click(object sender, EventArgs e) {
            int newIndex = frmScreenBrowser.GetScreenIndex(ScreenEditor.Level, ScreenEditor.ScreenIndex, btnAltPal.Checked, mnuPhysics.Checked);
            if (newIndex >= 0) {
                PerformAction(actions.SetLayout(newIndex));
            }

            // Stupid graphical glitch in toolstrip when calling showdialog
            btnGrid.Enabled = false;
            btnGrid.Enabled = true;
        }

        bool mapLocked = false;
        private void btnLockMap_Click(object sender, EventArgs e) {
            if (mapLocked) {
                UnlockMap();
                mapLocked = false;
            } else {
                LockMap();
                mapLocked = true;
            }
            btnLockMap.Checked = mapLocked;
        }

        private void btnChrUsage_Click(object sender, EventArgs e) {
            frmChrTable.ShowChrDialog(gameRom, this);
        }

        private void picTitle_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left && MouseButtons == MouseButtons.None) {
                ShowOpenFileDialog();
            }
        }







        internal void ClearUndoQueue() {
            undoRedo.Clear();
            UpdateUndoButtons();
        }

        private void mnuTools_DropDownOpening(object sender, EventArgs e) {
            mnuTileArranger.Enabled = gameRom != null &&  gameRom.RomFormat == RomFormats.Enhanco && (gameRom != null && CurrentLevel != LevelIndex.None);
            mnuTileArranger.Visible = ModifierKeys == Keys.Shift;
        }

        private void mnuTileArranger_Click(object sender, EventArgs e) {
            var lvl = currentLevelData;
            if (lvl == null) return;

            using (frmRearrageTiles poop = new frmRearrageTiles()) {
                poop.LoadRomData(lvl);

                if (Program.Dialogs.ShowDialog(poop, this) == DialogResult.OK) {
                    ReloadCurrentRomFromMemory();
                }
            }
        }

        private void buildAndLaunchToolStripMenuItem_Click(object sender, EventArgs e) {
            ////var project = new Editroid.ROM.Projects.Project(gameRom);
            ////var builder = new Editroid.ROM.Projects.Project.Builder(gameRom, project);

            ////var rom = builder.BuildRom();
            ////File.WriteAllBytes(temptxtBuildAndLaunch.Text, rom.data);
            ////Process.Start(temptxtBuildAndLaunch.Text);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e) {
            if (CurrentScreen != null) {
                Program.CodeEditor.LoadScreen(CurrentScreen);
            }
        }
        private void mnuProjectExpansion_Click(object sender, EventArgs e) {
            Program.CodeEditor.LoadExpansionFile();
        }
        private void mnuProjectCreate_Click(object sender, EventArgs e) {
            if (gameProject != null) {
                MessageBox.Show("This ROM already has an associated project.");
                return;
            }

            gameProject = new Project(gameRom);
            gameRom.EditorData.HasAssociatedProject = true;

            ShowProjectMenuItems();
            Program.CodeEditor.LoadGeneralFile();
        }

        private void HideProjectMenuItems() {
            ShowProjectMenuItems(false); 
        }

        private void ShowProjectMenuItems() {
            ShowProjectMenuItems(true); 
        }
        private void ShowProjectMenuItems(bool projectLoaded) {
            for (int i = 0; i < mnuProject.DropDownItems.Count; i++) {
                var item = mnuProject.DropDownItems[i];

                if ("loaded".Equals(item.Tag as string, StringComparison.Ordinal)) {
                    item.Visible = projectLoaded;
                    item.Enabled = projectLoaded;
                } else if ("unloaded".Equals(item.Tag as string, StringComparison.Ordinal)) {
                    item.Visible = !projectLoaded;
                    item.Enabled = !projectLoaded;
                }
            }
            ////mnuProjectCreate.Visible = !projectLoaded;
            ////mnuProjectCreateSeparator.Visible = false; //!projectLoaded;

            ////mnuProjDebug.Visible = projectLoaded;
            ////mnuProjDebugSeparator.Visible = projectLoaded;

            ////mnuProjectEditscreen.Visible = projectLoaded;
            ////mnuProjectEditgeneral.Visible = projectLoaded;
            ////mnuProjectOther.Visible = projectLoaded;

            ////mnuProjBuildSeparator.Visible = projectLoaded;
            ////mnuProjectBuild.Visible = projectLoaded;
            ////mnuProjectBuildlaunch.Visible = projectLoaded;
            ////mnuProjectBuildoptions.Visible = projectLoaded;


            btnEditScreenLoad.Visible = projectLoaded;
            btnEditScreenLoadSeparator.Visible = projectLoaded;

            mnuProjectCreate.Enabled = gameRom != null && (gameRom.RomFormat == RomFormats.Enhanco || gameRom.RomFormat == RomFormats.MMC3) && !projectLoaded;
        }


        /// <summary>
        /// Builds the currently loaded ROM in memory and returns the result, or returns null if there are fatal errors
        /// </summary>
        /// <param name="filename">The filename (not including directory) of the output ROM</param>
        MetroidRom PerformBuild(string filename) {
            var builder = new Editroid.ROM.Projects.Project.Builder(gameRom, gameProject, filename);

            var rom = builder.BuildRom();
            if (builder.Errors.Count > 0) {
                var errorForm = Program.BuildErrorForm;
                errorForm.SetErrorList(builder.Errors);
                errorForm.Show();
            }

            if (builder.FatalError) {
                return null;
            }

            return rom;
        }

        private void mnuProjectEditgeneral_Click(object sender, EventArgs e) {
            Program.CodeEditor.LoadGeneralFile();
        }

        private void mnuProjectDeclaraion_Click(object sender, EventArgs e) {
            Program.CodeEditor.LoadDeclaresFile();
        }

        private void mnuProjDebug_Click(object sender, EventArgs e) {
            if (gameProject != null) gameProject.Debug = mnuProjDebug.Checked;
        }

        private void btnEditScreenLoad_Click(object sender, EventArgs e) {
            if (CurrentScreen != null) {
                Program.CodeEditor.LoadScreen(CurrentScreen);
            }
        }

        private void mnuBuildoptionsBuildontest_Click(object sender, EventArgs e) {
            if (gameProject != null)
                gameProject.BuildOnTestRoom = mnuBuildoptionsBuildontest.Checked;
        }



        private void mnuProjectBuild_Click(object sender, EventArgs e) {
            PerformBuildAndSave(false);
        }

        /// <summary>
        /// Builds a ROM and saves it to an external file
        /// </summary>
        /// <param name="launch"></param>
        private void PerformBuildAndSave(bool launch) {
            //if (ProjectBuildSaver.ShowDialog() == DialogResult.OK) {
            string buildFileName = Path.ChangeExtension(cdgOpenRom.FileName, ".build.nes");
            if (Program.CodeEditorLoaded) Program.CodeEditor.SaveCurrentDocument();
            SaveRom();

            string outputDir = Path.GetDirectoryName(buildFileName);
            string outputFile = Path.GetFileName(buildFileName);
            gameProject.DebugOutputDirectory = outputDir;

            var rom = PerformBuild(outputFile);

            if (rom != null) {
                File.WriteAllBytes(buildFileName, rom.data);

                if (launch) {
                    //Process.Start(ProjectBuildSaver.FileName);
                    LaunchRom(buildFileName);
                }
            }
            //}
        }
        private void mnuProjectBuildlaunch_Click(object sender, EventArgs e) {
            PerformBuildAndSave(true);
        }


        private void mnuChrPrev_Click(object sender, EventArgs e) {
            SelectPreviousChr();    
        }



        private void mnuChrNext_Click(object sender, EventArgs e) {
            SelectNextChr();
        }


        int chrAnimationIndex = 0;

        private void SelectPreviousChr() {
            if (currentLevelData == null) return;
            if (gameRom.Format.HasChrUsageTable) {

                int minChrBank = gameRom.ChrUsage.GetBgFirstPage(CurrentLevel);
                int currentChrIndex = currentLevelData.PreferredChrBank ?? minChrBank;

                // Use previous bank (wrap around from lowest to highest if necessary)
                int newChrIndex = currentChrIndex - 1;
                if (newChrIndex < minChrBank) {
                    int maxChrBank = gameRom.ChrUsage.GetBgLastPage(CurrentLevel);
                    newChrIndex = maxChrBank;
                }

                // Load it!
                if (newChrIndex != currentChrIndex) {
                    currentLevelData.PreferredChrBank = newChrIndex;
                    ReloadLevelChr(CurrentLevel);
                }
            } else if (gameRom.Format.HasChrAnimationTable) {
                chrAnimationIndex = Math.Max(0, chrAnimationIndex - 1);
                ReloadChrAnimations(chrAnimationIndex);
            }
        }
 

        private void SelectNextChr() {
            if (currentLevelData == null) return;
            if (gameRom.Format.HasChrUsageTable) {

                int minChrBank = gameRom.ChrUsage.GetBgFirstPage(CurrentLevel);
                int maxChrBank = gameRom.ChrUsage.GetBgLastPage(CurrentLevel);
                int currentChrIndex = currentLevelData.PreferredChrBank ?? minChrBank;

                // Use next CHR bank (wrap from highest to lowest if necessary)
                int newChrIndex = currentChrIndex + 1;
                if (newChrIndex > maxChrBank) {
                    newChrIndex = minChrBank;
                }

                // Loat it!
                if (newChrIndex != currentChrIndex) {
                    currentLevelData.PreferredChrBank = newChrIndex;
                    ReloadLevelChr(CurrentLevel);
                }
            } else if (gameRom.Format.HasChrAnimationTable) {
                chrAnimationIndex = Math.Min(255, chrAnimationIndex + 1);
                ReloadChrAnimations(chrAnimationIndex);

            }
        }

        private void ReloadLevelChr(LevelIndex levelIndex) {
            gameRom.Levels[levelIndex].ReloadPatterns();
            gameView.Redraw(levelIndex);
            if (Program.StructFormIsLoaded) {
                Program.StructForm.RefreshChr();
            }
        }

        private void ReloadChrAnimations(int chrAnimationIndex) {
            gameRom.ReloadChrAnimations(chrAnimationIndex);
            gameView.RedrawAll();
            if (Program.StructFormIsLoaded) {
                Program.StructForm.RefreshChr();
            }
        }


        List<ToolStripMenuItem> projectFileMenuItems = new List<ToolStripMenuItem>();
        private void mnuProjectOther_DropDownOpening(object sender, EventArgs e) {
            if (GameProject == null || GameProject.Files.Count == 0) {
                UserFileSeparator.Visible = false;
                for (int i = 0; i < projectFileMenuItems.Count; i++) {
                    projectFileMenuItems[i].Visible = false;
                }
            } else {
                UserFileSeparator.Visible = true;
                for (int i = 0; i < GameProject.Files.Count; i++) {
                    var file = gameProject.Files[i];
                    ToolStripMenuItem menuitem;

                    if (i >= projectFileMenuItems.Count) {
                        menuitem = new ToolStripMenuItem();
                        menuitem.Click += new EventHandler(OnProjectFileSelected);
                        projectFileMenuItems.Add(menuitem);
                        mnuProjectOther.DropDownItems.Add(menuitem);
                    } else {
                        menuitem = projectFileMenuItems[i];
                    }

                    menuitem.Text = file.Name;
                    menuitem.Image = file.AutoBuild ? userFileAsterisk : userFileIcon;
                    menuitem.Visible = true;
                }
                for (int i = GameProject.Files.Count; i < projectFileMenuItems.Count; i++) {
                    projectFileMenuItems[i].Visible = false;
                }
            }
        }

        void OnProjectFileSelected(object sender, EventArgs e) {
            string name = ((ToolStripMenuItem)sender).Text;
            Program.CodeEditor.LoadUserFile(name);
        }

        private void mnuProjectAddFile_Click(object sender, EventArgs e) {
            List<string> takenNames = GetAsmFileList();

            using (FilenameForm f = new FilenameForm()) {
                f.ReservedFilenames = takenNames;
                Program.Dialogs.ShowDialog(f, this);

                if (!string.IsNullOrEmpty(f.EnteredFilename)) {
                    GameProject.Files.Add(new AsmFile() { Name = f.EnteredFilename, AutoBuild = true  });
                    Program.CodeEditor.LoadUserFile(f.EnteredFilename);
                }
            }
        }

        private List<string> GetAsmFileList() {
            List<string> takenNames = new List<string>();
            takenNames.AddRange(Project.ReservedFilenames);
            foreach (var file in GameProject.Files) {
                takenNames.Add(file.Name);
            }
            return takenNames;
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var frm = new frmExportAsm()) {
                frm.SetFileList(GetAsmFileList());
                if (Program.Dialogs.ShowDialog(frm, this) == DialogResult.OK) {
                    var files = frm.SelectedFiles;
                    var path = frm.SelectedFolder;
                    bool useAsmExt = frm.UseAsmExtension;
                    StringBuilder errorList = new StringBuilder();
                    

                    foreach (var file in files) {
                        string errorMsg = null;
                        if (string.Equals(file, Project.declaresFilename, StringComparison.OrdinalIgnoreCase)) {
                            ExportFile(path, definesFilename, gameProject.DefinesFile, out errorMsg);
                        } else if (string.Equals(file, Project.generalCodeFilename, StringComparison.OrdinalIgnoreCase)) {
                            ExportFile(path, generalcodeFilename, gameProject.GeneralCodeFile, out errorMsg);
                        } else if (string.Equals(file, Project.expansionCodeFilename, StringComparison.OrdinalIgnoreCase)) {
                            ExportFile(path, expansionFilename, gameProject.ExpansionFile, out errorMsg);
                        } else {
                            string filename = file;
                            if (useAsmExt) {
                                if (!filename.EndsWith(".asm", StringComparison.OrdinalIgnoreCase)) {
                                    filename += ".asm";
                                }
                            }

                            ExportFile(path, filename, gameProject.GetFileByName(file).Code, out errorMsg);
                        }

                        if (errorMsg != null) {
                            errorList.AppendLine(file + ": " + errorMsg);
                        }
                    }

                    if (errorList.Length > 0) {
                        MessageBox.Show("Errors occured while exporting the following files." + Environment.NewLine + Environment.NewLine + errorList.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        /// <param name="errorMsg">Null if no errors occurred.</param>
        private void ExportFile(string path, string name, string contents, out string errorMsg) {
            errorMsg = null;
            try {
                File.WriteAllText(Path.Combine(path, name), contents);
            } catch (ArgumentException) {
                errorMsg = "Invalid file path";
            } catch (PathTooLongException) {
                errorMsg = "Path too long";
            } catch (DirectoryNotFoundException) {
                errorMsg = "Directory not found.";
            } catch (IOException) {
                errorMsg = "An I/O error occurred.";
            } catch (UnauthorizedAccessException) {
                errorMsg = "Unauthorized access";
            } catch (NotSupportedException) {
                errorMsg = "Invalid path format";
            } catch (SecurityException) {
                errorMsg = "You do not have the required permissions";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMsg">Null if no errors occurred.</param>
        private string ImportFile(string path, out string errorMsg) {
            errorMsg = null;
            try {
                return File.ReadAllText(path);
            } catch (ArgumentException) {
                errorMsg = "Invalid file path";
            } catch (PathTooLongException) {
                errorMsg = "Path too long";
            } catch (DirectoryNotFoundException) {
                errorMsg = "Directory not found.";
            } catch (IOException) {
                errorMsg = "An I/O error occurred.";
            } catch (UnauthorizedAccessException) {
                errorMsg = "Unauthorized access";
            } catch (NotSupportedException) {
                errorMsg = "Invalid path format";
            } catch (SecurityException) {
                errorMsg = "You do not have the required permissions";
            }

            return null;
        }
        private void importToolStripMenuItem_Click(object sender, EventArgs e) {
            if (AsmImportDialog.ShowDialog() == DialogResult.OK) {
                List<AsmFile> newFiles = new List<AsmFile>();
                List<AsmFile> coreFiles = new List<AsmFile>();
                string errMsg = null;
                StringBuilder errorList = new StringBuilder();

                // Load files from disk
                var fileList = AsmImportDialog.FileNames;
                for (int i = 0; i < fileList.Length; i++) {
                    var file = fileList[i];
                    var fileText = ImportFile(file, out errMsg);

                    if (fileText != null) {
                        AsmFile newFile = new AsmFile();
                        newFile.Name = Path.GetFileName(file);
                        newFile.Code = fileText;
                        newFiles.Add(newFile);
                    }

                    if (errMsg != null) {
                        errorList.AppendLine(file + ": " + errMsg);
                    }
                }

                // Warn user if there were any errors
                if (errorList.Length > 0) {
                    MessageBox.Show("One or more files could not be loaded." + Environment.NewLine + Environment.NewLine, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Add any files that don't conflict with existing files
                for (int i = newFiles.Count - 1; i >= 0; i--) {
                    if (IsCoreAsmFile(newFiles[i].Name)) {
                        // Move file from newFiles to system files
                        coreFiles.Add(newFiles[i]);
                        newFiles.RemoveAt(i);
                    } else if (gameProject.TryGetFileByName(newFiles[i].Name) == null) {
                        // Move file from newFiles to project
                        gameProject.Files.Add(newFiles[i]);
                        newFiles.RemoveAt(i);
                    }
                }

                // Any remaining files have names conflicting with existing files
                if (newFiles.Count > 0) {
                    StringBuilder fileConflictList = new StringBuilder("The following project files will be overwritten:");
                    fileConflictList.AppendLine();
                    fileConflictList.AppendLine();
                    for (int i = 0; i < newFiles.Count; i++) {
                        fileConflictList.AppendLine(newFiles[i].Name);
                    }

                    if (MessageBox.Show(fileConflictList.ToString(), "Overwrite Files", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) {
                        for (int i = 0; i < newFiles.Count; i++) {
                            GameProject.Files.Remove(GameProject.GetFileByName(newFiles[i].Name));
                            GameProject.Files.Add(newFiles[i]);
                        }
                    }
                }

                // It's quite likely that the reloaded core files are identical
                for (int i = coreFiles.Count - 1; i >= 0; i--) {
                    bool unchanged = GetCoreFileContents(coreFiles[i].Name).Equals(coreFiles[i].Code);
                    if (unchanged) coreFiles.RemoveAt(i);
                }
                // Ask user before overwriting core files

                if (coreFiles.Count > 0) {
                    StringBuilder coreFileList = new StringBuilder("Warning: the following core files will be overwritten:");
                    coreFileList.AppendLine();
                    coreFileList.AppendLine();
                    for (int i = 0; i < coreFiles.Count; i++) {
                        coreFileList.AppendLine(coreFiles[i].Name);
                    }

                    if (MessageBox.Show(coreFileList.ToString(), "Overwrite Core Files", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) {
                        for (int i = 0; i < coreFiles.Count; i++) {
                            SetCoreFileContents(coreFiles[i].Name, coreFiles[i].Code);
                        }
                    }
                }
            }
        }

        string GetCoreFileContents(string corefilename) {
            if (string.Equals(corefilename, definesFilename, StringComparison.OrdinalIgnoreCase)) {
                return GameProject.DefinesFile;
            } else if (string.Equals(corefilename, generalcodeFilename, StringComparison.OrdinalIgnoreCase)) {
                return GameProject.GeneralCodeFile;
            } else if (string.Equals(corefilename, expansionFilename, StringComparison.OrdinalIgnoreCase)) {
                return GameProject.ExpansionFile;
            }
            throw new ArgumentException("The specified file name is not a valid core file name");
        }
        void SetCoreFileContents(string corefilename, string text) {
            if (string.Equals(corefilename, definesFilename, StringComparison.OrdinalIgnoreCase)) {
                GameProject.DefinesFile = text;
            } else if (string.Equals(corefilename, generalcodeFilename, StringComparison.OrdinalIgnoreCase)) {
                GameProject.GeneralCodeFile = text;
            } else if (string.Equals(corefilename, expansionFilename, StringComparison.OrdinalIgnoreCase)) {
                GameProject.ExpansionFile = text;
            } else {
                throw new ArgumentException("The specified file name is not a valid core file name");
            }
       
        }

        const string definesFilename = "_defines.asm";
        const string generalcodeFilename = "_generalCode.asm";
        const string expansionFilename = "_expansion.asm";
        bool IsCoreAsmFile(string name) {
            return (string.Equals(name, definesFilename, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, generalcodeFilename, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, expansionFilename, StringComparison.OrdinalIgnoreCase));
            //ExportFile(path, "_defines.asm", gameProject.DefinesFile, out errorMsg);
            //ExportFile(path, "_generalCode.asm", gameProject.GeneralCodeFile, out errorMsg);
            //ExportFile(path, "_expansion.asm", gameProject.ExpansionFile, out errorMsg);

        }

        private void mnuEditors_DropDownOpening(object sender, EventArgs e) {
            bool showAdvancedItems = ModifierKeys == Keys.Shift;


            foreach (var item in mnuEditors.DropDownItems) {
                var tsItem = item as ToolStripItem;
                if (tsItem != null) {
                    if(tsItem.Tag is EditroidPluginAttribute || String.Equals( tsItem.Tag as string ,"advanced", StringComparison.OrdinalIgnoreCase)){
                        tsItem.Visible = showAdvancedItems;
                    }
                }
            }

            if (gameRom != null) {
                btnChrAnimation.Enabled = !gameRom.ChrAnimationTableMissing;
            }
        }

        private void btnExpansionTest_Click(object sender, EventArgs e) {



            //if (ProjectBuildSaver.ShowDialog() == DialogResult.OK) {
            //    if (Program.CodeEditorLoaded) Program.CodeEditor.SaveCurrentDocument();
            //    SaveRom();

            //    var rom = PerformBuild();

            //    if (rom != null) {
            //        var expander = new Editroid.ROM.MMC3.MMC3Expander(rom);
            //        File.WriteAllBytes("D:\\temp\\mexpand.nes", expander.ExpandRom());

            //    }
            //}
            var expander = new Editroid.ROM.MMC3.MMC3Expander(gameRom, gameProject);

            Editroid.ROM.MMC3.MMC3Expander.PostWorkDelegate postWork;
            var result = expander.ExpandRom(out postWork);
            MetroidRom newRom = new MetroidRom(new MemoryStream(result));
            postWork(newRom, null);
            newRom.SerializeAllData();

            File.WriteAllBytes("D:\\temp\\mexpand.nes", result);


        }

        private void btnBankAlloc_Click(object sender, EventArgs e) {
            using (var dlg = new frmBankAllocation()) {
                dlg.SetBanks(gameRom.BankAllocation);
                Program.Dialogs.ShowDialog(dlg, this);
                
            }
        }

        private void mnuSecretExpansion_Click(object sender, EventArgs e) {
            Editroid.ROM.MMC3.MMC3Expander.ReplaceExpansionFile(gameProject);
        }

        private void mnuSecretFixedrefs_Click(object sender, EventArgs e) {
            Editroid.ROM.MMC3.MMC3Expander.UpdateFixedBankRefs(gameProject);

        }

        private void btnTilePhys_Click(object sender, EventArgs e) {
            if (CurrentScreen != null) {
                frmTilePhysics.EditTilePhysics(CurrentScreen.Level, this);
            }
        }

        private void mnuSecretChrConversion_Click(object sender, EventArgs e) {
            Editroid.ROM.MMC3.MMC3Expander.ConvertChrAnimation(gameRom, gameProject);
            gameRom.ReloadChrAnimationTable();
        }

        private void btnChrAnimation_Click(object sender, EventArgs e) {
            using (frmChrAnimation frm = new frmChrAnimation()) {
                frm.SetRom(gameRom, gameProject);
                //frm.ShowDialog();
                Program.Dialogs.ShowDialog(frm,this);
            }
        }

        private void mnuSecretPatch_Click(object sender, EventArgs e) {
            Editroid.ROM.MMC3.MMC3Expander.ApplyExpansionPatch(gameRom.data);
        }

        bool EditStructureMode {
            get { return btnEditStructs.Checked; }
            set {
                btnInsertStructs.Checked = !value;
                btnEditStructs.Checked = value;
            }
        }
        private void btnInsertStructs_Click(object sender, EventArgs e) {
            EditStructureMode = false;
        }

        private void btnEditStructs_Click(object sender, EventArgs e) {
            EditStructureMode = true;
        }

        private void structurePicker1_StructureClicked(object sender, Editroid.Controls.StructurePicker.StructureIndexEventArgs e) {
            if (EditStructureMode) {
                if (SelectedItem is StructInstance) {
                    bool samePal = (int)((StructInstance)SelectedItem).PalData == structurePicker1.Palette;

                    if (!samePal) {
                        PerformAction(actions.ChangePalette(structurePicker1.Palette));
                    }
                    PerformAction(actions.SetType(e.Value));
                   
                }
            } else {
                PerformAction(actions.AddObject(e.Value, (byte)structurePicker1.Palette));
            }
        }

        private void btnAddItemData_DropDownOpened(object sender, EventArgs e) {
            int freemem = 0;
            if (currentLevelData != null) {
                freemem = currentLevelData.Format.AvailableItemMemory;
            }

            for (int i = 0; i < btnAddItemData.DropDownItems.Count; i++) {
                int neededMem = ((string)btnAddItemData.DropDownItems[i].Tag)[0] - '0';
                btnAddItemData.DropDownItems[i].Enabled = freemem >= neededMem;
            }
        }

        // Todo: need to calculate if an item data row needs to be created (+ 3 bytes) or an item data screen (+ 3 bytes) when determining needed space
        //       need to create item data screen if necessary when adding items
        //       need to serialize item data for some things to continue working correctly
        private ItemScreenData FindOrCreateScreenItemData(Level level, Point point) {
            if (level == null) return null;
            throw new NotImplementedException();
        }

        private void btnAddItemEnemy_Click(object sender, EventArgs e) {
            ItemScreenData screenData = FindOrCreateScreenItemData(currentLevelData, ScreenEditor.MapLocation);
        }

        private void btnAddItemPowerup_Click(object sender, EventArgs e) {

        }

        private void btnAddItemMella_Click(object sender, EventArgs e) {

        }

        private void btnAddItemElevator_Click(object sender, EventArgs e) {

        }

        private void btnAddItemTurret_Click(object sender, EventArgs e) {

        }

        private void btnAddItemMB_Click(object sender, EventArgs e) {

        }

        private void btnAddItemZebetite_Click(object sender, EventArgs e) {

        }

        private void btnAddItemRinkas_Click(object sender, EventArgs e) {

        }

        private void btnAddItemDoor_Click(object sender, EventArgs e) {

        }

        private void btnAddItemPalswap_Click(object sender, EventArgs e) {

        }






    }

    delegate T Func<T> ();
}
