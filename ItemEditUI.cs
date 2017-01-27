using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Editroid.ROM;
using Editroid.Actions;
using Editroid.UndoRedo;

namespace Editroid
{
    /// <summary>
    /// Provides a means to define and connect logic for item editing using the item toolbar introduced in Editroid 4.0.
    /// </summary>
    class ItemEditUI
    {


        Dictionary<ItemUiRole, ToolStripItem> Controls = new Dictionary<ItemUiRole, ToolStripItem>();
        Dictionary<ToolStripItem, ItemUiRole> ControlLookup = new Dictionary<ToolStripItem, ItemUiRole>();
        Dictionary<ToolStripItem, ItemUiComboList> ControlLists = new Dictionary<ToolStripItem, ItemUiComboList>();
        Dictionary<ToolStripItem, List<ToolStripItem>> GroupedControls = new Dictionary<ToolStripItem, List<ToolStripItem>>();

        ItemUiScheme _CurrentScheme;
        EditroidUndoRedoQueue _UndoRedoQueue;
        ToolStripComboBox _ItemListCombo;

        int updateLevel = 0;
        void BeginUpdate() {
            updateLevel++;
        }
        void EndUpdate() {
            updateLevel--;
            if (updateLevel < 0) throw new InvalidOperationException("EndUpdate called more times than BeginUpdate.");
        }
        public bool IsUpdating { get { return updateLevel > 0; } }
        public ItemScreenData SelectedScreen { get; private set; }
        public ItemData SelectedItem { get; private set; }
        public EditroidUndoRedoQueue UndoRedoQueue {
            get { return _UndoRedoQueue; }
            set { _UndoRedoQueue = value; }
        }

        public void PerformAction(Editroid.UndoRedo.EditroidAction a) {
        }

        public void AssociateControl(ItemUiRole role, ToolStripItem control) {
            AssociateControl(role, control, null);
        }
        public void AssociateControl(ItemUiRole role, ToolStripItem control, ItemUiComboList listItems) {
            if (control == null) throw new ArgumentNullException("control");
            if (Controls.ContainsKey(role) || Controls.ContainsValue(control)) {
                throw new ArgumentException("Can not assign the same role or the same control more than once.");
            }

            Controls.Add(role, control);
            ControlLookup.Add(control, role);
            ControlLists.Add(control, listItems);
            GroupedControls.Add(control, new List<ToolStripItem>());

            if (control is ToolStripTextBox) {
                ((ToolStripTextBox)control).TextChanged += new EventHandler(OnUiTextChanged);
                ((ToolStripTextBox)control).LostFocus += new EventHandler(OnUiTextLostFocus);
                ((ToolStripTextBox)control).KeyDown += new KeyEventHandler(OnUiTextKeyDown);
            } else if (control is ToolStripButton) {
                ((ToolStripButton)control).Click += new EventHandler(OnUiButtonClick);
            } else if (control is ToolStripComboBox) {
                ((ToolStripComboBox)control).SelectedIndexChanged += new EventHandler(OnUiSelectedIndexChanged);
                if (listItems != null) {
                    listItems.SetOwner((ToolStripComboBox)control);
                }
            }

        }
        public ToolStripComboBox ItemListCombo {
            get { return _ItemListCombo; }
            set {
                if (_ItemListCombo != null) {
                    _ItemListCombo.SelectedIndexChanged -= new EventHandler(OnSelectedItemIndexChanged);
                }

                _ItemListCombo = value;

                if (_ItemListCombo != null) {
                    _ItemListCombo.SelectedIndexChanged += new EventHandler(OnSelectedItemIndexChanged);
                }
            }
        }

        void OnSelectedItemIndexChanged(object sender, EventArgs e) {
            if (!IsUpdating) {
                if (ItemListCombo.SelectedIndex >= 0) {
                    BeginUpdate();
                    ////SelectedItem = SelectedScreen.Items[ItemListCombo.SelectedIndex];
                    ////LoadScreen(SelectedScreen, new ItemInstance(SelectedScreen,SelectedItem));
                    SelectedItemIndexChanged.Raise(this);
                    EndUpdate();
                }
            }
        }
        public int SelectedItemIndex { get { return ItemListCombo.SelectedIndex; } }
        public event EventHandler SelectedItemIndexChanged;
        public void GroupControl(ToolStripItem ownerControl, ToolStripItem ownedControl) {
            GroupedControls[ownerControl].Add(ownedControl);
        }

        void OnUiSelectedIndexChanged(object sender, EventArgs e) {
            if (!IsUpdating) {
                var combo = ((ToolStripComboBox)sender);
                int selectedValue = combo.SelectedIndex;

                // If the combobox has an associated list, the actual selected value is the list item whose index is specified by combo.SelectedIndex.
                ItemUiComboList list;
                if (ControlLists.TryGetValue(combo, out list)) {
                    if (selectedValue >= 0 && selectedValue < list.Items.Count) {
                        selectedValue = list.Items[selectedValue].Value;
                    } else {
                        selectedValue = -1;
                    }
                }

                // If selectedValue < 0, then either nothing is actually selected, or an "other" item was selected. In either case, we'll want to ignore the input.
                if (selectedValue >= 0) {
                    // Get the UI logic
                    var elem = GetElementByRole(ControlLookup[combo]);
                    // Execute the "setter", which produces information necessary to create and execute an undo/redo event
                    var args = new ItemUiSetterArgs(SelectedItem);
                    args.EnteredValue = selectedValue;
                    elem.Setter(args);

                    if (args.EditedProperty != Editroid.Actions.ItemProperty.invalid) {
                        CreateAndExecuteAction(args.EditedProperty, args.AssignedValue);
                    }
                }
            }
        }

        void OnUiButtonClick(object sender, EventArgs e) {
            if (!IsUpdating) {
                var button = ((ToolStripButton)sender);
                bool newState = !button.Checked;

                // Get the UI logic
                var elem = GetElementByRole(ControlLookup[button]);
                // Execute the "setter", which produces information necessary to create and execute an undo/redo event
                var args = new ItemUiSetterArgs(SelectedItem);
                args.EnteredValue = newState ? 1 : 0;
                elem.Setter(args);

                if (args.EditedProperty != Editroid.Actions.ItemProperty.invalid) {
                    CreateAndExecuteAction(args.EditedProperty, args.AssignedValue);
                }
            }
        }

        void OnUiTextChanged(object sender, EventArgs e) {
            if (!IsUpdating) {
            }
        }


        void OnUiTextKeyDown(object sender, KeyEventArgs e) {
            if (!IsUpdating) {
                if (e.KeyCode == Keys.Enter) {
                    e.Handled = true;
                    ProcessTextboxInput((ToolStripTextBox)sender);
                }
            }
        }

        private void ProcessTextboxInput(ToolStripTextBox txt) {
            int value;
            bool invalid = false;

            BeginUpdate();

            var elem = GetElementByRole(ControlLookup[txt]);

            if (int.TryParse(txt.Text, System.Globalization.NumberStyles.HexNumber, null, out value)) {
                if (value >= elem.Minimum && value <= elem.Maximum) {
                    ItemUiSetterArgs args = new ItemUiSetterArgs(SelectedItem);
                    args.EnteredValue = value;
                    elem.Setter(args);
                    if (args.EditedProperty != ItemProperty.invalid) {
                        CreateAndExecuteAction(args.EditedProperty, args.AssignedValue);
                    }
                } else {
                    invalid = true;
                }
                
            } else {
                invalid = true;
            }

            // If a nonsensical value is entered, re-display current value
            if (invalid) {
                PopulateUiElement(elem);
            }
            EndUpdate();
        }

        void OnUiTextLostFocus(object sender, EventArgs e) {
            if (!IsUpdating) {
            }
        }


        private void CreateAndExecuteAction(Editroid.Actions.ItemProperty itemProperty, int value) {
            var action = new EditItemProperty(UndoRedoQueue, new Point(SelectedScreen.MapX, SelectedScreen.MapY), SelectedScreen, SelectedItem, itemProperty, value);
            //UndoRedoQueue.Do(action);
            Program.PerformAction(action);
        }

        public void DisplayScheme(ItemUiScheme scheme) {
            BeginUpdate();

            if (scheme != _CurrentScheme) {
                ShowThemeControls(scheme);
            }

            PopulateItemUi(scheme);

            EndUpdate();
        }

        private void PopulateItemUi(ItemUiScheme scheme) {
            foreach (var elem in scheme.Elements) {
                PopulateUiElement(elem);
            }
        }

        private void PopulateUiElement(ItemUiElement elem) {
            ToolStripItem i;
            if (Controls.TryGetValue(elem.Role, out i)) {
                ItemUiGetterArgs args = new ItemUiGetterArgs(SelectedItem);
                elem.Populator(args);

                DisplayValueInControl(i, args.Value);
            }
        }

        private void ShowThemeControls(ItemUiScheme scheme) {
            foreach (ToolStripItem c in Controls.Values) {
                c.Visible = false;

                foreach (var subcontrol in GroupedControls[c]) {
                    subcontrol.Visible = false;
                }
            }

            _CurrentScheme = scheme;
            foreach (var schemeElement in scheme.Elements) {
                ToolStripItem control;
                if (Controls.TryGetValue(schemeElement.Role, out control)) {
                    control.Visible = true;

                    foreach (var subcontrol in GroupedControls[control]) {
                        subcontrol.Visible = true;
                    }

                }
            }
        }

        private void DisplayValueInControl(ToolStripItem control, int value) {
            if (control is ToolStripTextBox) {
                var element = GetElementByRole(ControlLookup[control]);
                bool twoDigits = element.Maximum > 0xF;
                control.Text = value.ToString(twoDigits ? "X2" : "X");
                ((ToolStripTextBox)control).MaxLength = twoDigits ? 2 : 1;
            } else if (control is ToolStripComboBox) {
                ((ToolStripComboBox)control).SelectedIndex = GetListIndex(control, value);
            } else if (control is ToolStripButton) {
                bool isChecked = ((ToolStripButton)control).Checked = value != 0;
                ((ToolStripButton)control).Image = isChecked ? ItemUiCheckboxImages.Checked : ItemUiCheckboxImages.Unchecked;

            }

        }


        /// <summary>
        /// Returns the object that defines the behavior of a control (ItemUiElement) for the given UI role for the current scheme, or NULL if there is no element with the associated role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        ItemUiElement GetElementByRole(ItemUiRole role) {
            if (_CurrentScheme == null) return null;
            for (int i = 0; i < _CurrentScheme.Elements.Count; i++) {
                if (_CurrentScheme.Elements[i].Role == role) return _CurrentScheme.Elements[i];
            }

            return null;
        }

        private int GetListIndex(ToolStripItem control, int value) {
            var items = ControlLists[control].Items;
            int defaultItem = -1;

            for (int i = 0; i < items.Count; i++) {
                if (items[i].Value == value) {
                    return i;
                } else if (items[i].Value == -1) {
                    defaultItem = i;
                }
            }

            return defaultItem;
        }


        ////void RefreshElement(ItemUiRole element) {
        ////    Controls[element].PopulateControl();
        ////}


        /////// <summary>
        /////// Represents a user-editable UI element. This class acts as a base class for control-specific UI elements.
        /////// </summary>
        /////// <remarks>Classes do not inherit directly from this class, but rather from the generic subclass to provide type safety.</remarks>
        ////internal abstract class UiElement {
        ////    public UiElement(object control) {
        ////        this.Control = control;
        ////    }

        ////    /// <summary>
        ////    /// The control this object is associated with
        ////    /// </summary>
        ////    public object Control { get; private set; }

        ////    ItemEditUI _Host;
        ////    ItemUiRole _Role;

        ////    public ItemEditUI Host { get { return _Host; }}
        ////    public ItemUiRole Role { get { return _Role; } }

        ////    internal void SetHost(ItemEditUI host, ItemUiRole role) {
        ////        if (host == null || _Host != null)
        ////            throw new InvalidOperationException("UiElement.Host can only be set once and may not be set to null.");

        ////        _Host = host;
        ////        _Role = role;
        ////    }

        ////    /// <summary>
        ////    /// Causes this control to initialize itself based on the specified item data. Call BeginUpdate on the ItemEditUI host object before calling this method.
        ////    /// </summary>
        ////    /// <param name="data"></param>
        ////    public void PopulateControl() {
        ////        OnPopulateControl();
        ////    }

        ////    /// <summary>
        ////    /// Call in derived classes in response to control events to notify this object that the user has interated with the element. 
        ////    /// </summary>
        ////    /// <param name="data"></param>
        ////    /// <param name="actionToPerform"></param>
        ////    protected void NotifyUserEdit() {
        ////        Editroid.UndoRedo.EditroidAction actionToPerform;
        ////        OnNotifyUserEdit(out actionToPerform);
        ////        if (actionToPerform != null) Host.PerformAction(actionToPerform);
        ////    }

        ////    /// <summary>
        ////    /// Override this method to implement handling for user editing. Check the IsUpdating property of the
        ////    /// ItemEditUI host object to confirm events are non being raised in response to UI population. 
        ////    /// </summary>
        ////    /// <param name="data"></param>
        ////    /// <param name="actionToPerform">Returns null or an EditroidAction object to perform via the undo/redo system</param>
        ////    protected abstract void OnNotifyUserEdit(out Editroid.UndoRedo.EditroidAction actionToPerform);
        ////    /// <summary>
        ////    /// Override this method to implement population of the control from ROM data.
        ////    /// </summary>
        ////    /// <param name="data"></param>
        ////    protected abstract void OnPopulateControl();

        ////}

        /////// <summary>
        /////// Provides an intermediate bridge between the type agnostic base and the type specific classes that inherit from it.
        /////// </summary>
        /////// <typeparam name="TControl"></typeparam>
        /////// <remarks>Control-specific classes inherit this rather than the non-generic superclass UiElement</remarks>
        ////private class UiElement<TControl> : UiElement {
        ////    public UiElement(TControl control) 
        ////    :base(control) {
        ////        this.Control = control;
        ////    }

        ////    /// <summary>
        ////    /// Gets the control associated with this object
        ////    /// </summary>
        ////    public new TControl Control { get; private set; }

        ////    /// <summary>
        ////    /// Raised when the control needs to be populated from ROM data
        ////    /// </summary>
        ////    public event ItemUiPopulator<TControl> Populate;
        ////    /// <summary>
        ////    /// Raised when the user modifies the value of the control.
        ////    /// </summary>
        ////    public event ItemUiEditHandler<TControl> EditOccurred;

        ////    protected override sealed void OnNotifyUserEdit(out Editroid.UndoRedo.EditroidAction actionToPerform) {
        ////        actionToPerform = null;

        ////        // Ignore UI events while populating controls
        ////        if (!Host.IsUpdating) {
        ////            var evnt = EditOccurred;
        ////            if (evnt != null) {
        ////                evnt(Control, out actionToPerform);
        ////            }
        ////        }
        ////    }

        ////    protected override sealed void OnPopulateControl() {
        ////        var evnt = Populate;
        ////        if (evnt != null) {
        ////            evnt(Control);
        ////        }
        ////    }
        ////}

        //sealed class UiCombobox : UiElement<ToolStripComboBox> {
        //    public UiCombobox(ToolStripComboBox control)
        //        : base(control) {

        //        control.SelectedIndexChanged += new EventHandler(control_SelectedIndexChanged);
        //    }

        //    void control_SelectedIndexChanged(object sender, EventArgs e) {
        //        NotifyUserEdit();
        //    }


        //}
        //class UiTextbox : UiElement<TextBox>
        //{
        //    public UiTextbox(TextBox control)
        //        : base(control) {
        //    }
        //}

        /// <summary>
        /// Defines a list of items to be shown in a combo box.
        /// </summary>
        internal class ItemUiComboList
        {
            public ItemUiComboList() {
                this.Items = new List<ItemUiComboItem>();
            }
            public ItemUiComboList(IEnumerable<ItemUiComboItem> items) 
            :this() {
                this.Items.AddRange(items);
            }
            bool _ShowIcons = true;

            /// <summary>
            /// Gets the list of items to populate the combo box with. Must be populated before the collection is associated with a combo box.
            /// </summary>
            public List<ItemUiComboItem> Items { get; private set; }

            public bool ShowIcons {
                get { return _ShowIcons; }
                set {
                    if (value != _ShowIcons) {
                        _ShowIcons = value;
                        if (_Owner != null) _Owner.Invalidate();
                    }
                }
            }

            ToolStripComboBox _Owner;

            public void SetOwner(ToolStripComboBox owner) {
                if (_Owner != null) {
                    _Owner.ComboBox.DrawItem -= new DrawItemEventHandler(Owner_DrawItem);
                    _Owner.Items.Clear();
                }

                this._Owner = owner;

                if (_Owner != null) {
                    _Owner.ComboBox.DrawItem += new DrawItemEventHandler(Owner_DrawItem);
                    _Owner.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
                    _Owner.ComboBox.ItemHeight = 20;

                    for (int i = 0; i < Items.Count; i++) {
                        _Owner.Items.Add(Items[i]);
                    }
                }

            }

            void Owner_DrawItem(object sender, DrawItemEventArgs e) {
                e.DrawBackground();

                if (e.Index >= 0) {
                    var listitem = Items[e.Index];
                    if (listitem.Icon != null && ShowIcons) {
                        Rectangle src = new Rectangle(0, 0, 16, 16);
                        Rectangle dest = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, 16, 16);

                        e.Graphics.DrawImage(listitem.Icon, dest, src, GraphicsUnit.Pixel);
                    }

                    bool selected = 0 != (e.State & (DrawItemState.HotLight | DrawItemState.Selected));
                    var brush = selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText;

                    int textX = ShowIcons ? e.Bounds.X + 20 : e.Bounds.X + 2;
                    e.Graphics.DrawString(listitem.Text ?? string.Empty, _Owner.Font, brush, new Point(textX, e.Bounds.Y + 2));
                }
            }

            public static ItemUiComboList PowerupTypeList = new ItemUiComboList(new ItemUiComboItem[]{
                new ItemUiComboItem(ItemImages.Bombs, "Bomb", 0),
                new ItemUiComboItem(ItemImages.Boot, "High Jump", 1),
                new ItemUiComboItem(ItemImages.Long, "Long Beam", 2),
                new ItemUiComboItem(ItemImages.Screw, "Screw Attack", 3),
                new ItemUiComboItem(ItemImages.MaruMari, "Marumari", 4),
                new ItemUiComboItem(ItemImages.Varia, "Varia", 5),
                new ItemUiComboItem(ItemImages.Wave, "Wave Beam", 6),
                new ItemUiComboItem(ItemImages.Ice, "Ice Beam", 7),
                new ItemUiComboItem(ItemImages.Energy, "Energy Tank", 8),
                new ItemUiComboItem(ItemImages.Missile, "Missile", 9),
                new ItemUiComboItem(null, "Other", -1),
            });

            public static ItemUiComboList ElevatorList = new ItemUiComboList(new ItemUiComboItem[]{
                new ItemUiComboItem(ItemImages.down, "Brinstar ▼ Brinstar", 0),
                new ItemUiComboItem(ItemImages.downPal, "Brinstar ▼ Norfair", 1),
                new ItemUiComboItem(ItemImages.down, "Brinstar ▼ Kraid", 2),
                new ItemUiComboItem(ItemImages.down, "Brinstar ▼ Tourian", 3),
                new ItemUiComboItem(ItemImages.downPal, "Norfair ▼ Ridley", 4),
                new ItemUiComboItem(ItemImages.upPal, "Norfair ▲ Brinstar", 0x81),
                new ItemUiComboItem(ItemImages.up, "Kraid ▲ Brinstar", 0x82),
                new ItemUiComboItem(ItemImages.up, "Tourian ▲ Brinstar", 0x83),
                new ItemUiComboItem(ItemImages.upPal, "Ridley ▲ Norfair", 0x84),
                new ItemUiComboItem(ItemImages.up, "End of game", 0x8F),
                new ItemUiComboItem(null, "Other", -1),

            });

            public static ItemUiComboList ZebetiteList = new ItemUiComboList(new ItemUiComboItem[]{
                new ItemUiComboItem(null, "Zebetite 0", 0),
                new ItemUiComboItem(null, "Zebetite 1", 1),
                new ItemUiComboItem(null, "Zebetite 2", 2),
                new ItemUiComboItem(null, "Zebetite 3", 3),
                new ItemUiComboItem(null, "Zebetite 4", 4),
                new ItemUiComboItem(null, "other", -1),
            }) { ShowIcons = false };
            public static ItemUiComboList TurretList = new ItemUiComboList(new ItemUiComboItem[]{
                new ItemUiComboItem(null, "Up Left Down", 0),
                new ItemUiComboItem(null, "Up Right Down", 1),
                new ItemUiComboItem(null, "Left Down Right", 2),
                new ItemUiComboItem(null, "Left Down", 3),
                new ItemUiComboItem(null, "other", -1),
            }) { ShowIcons = false };
            public static ItemUiComboList RinkaList = new ItemUiComboList(new ItemUiComboItem[]{
                new ItemUiComboItem(null, "Rinka 0", 0),
                new ItemUiComboItem(null, "Rinka 1", 1),
                new ItemUiComboItem(null, "other", -1),
            }) { ShowIcons = false };

            public static ItemUiComboList DoorList = new ItemUiComboList(new ItemUiComboItem[]{
                new ItemUiComboItem(null, "Left Missile Door", 0xB0),
                new ItemUiComboItem(null, "Left Door", 0xB1),
                new ItemUiComboItem(null, "Left 10-Missile Door", 0xB2),
                new ItemUiComboItem(null, "Left Music Change Door", 0xB3),
                new ItemUiComboItem(null, "Right Missile Door", 0xA0),
                new ItemUiComboItem(null, "Right Door", 0xA1),
                new ItemUiComboItem(null, "Right 10-Missile Door", 0xA2),
                new ItemUiComboItem(null, "Right Music Change Door", 0xA3),
                new ItemUiComboItem(null, "other", -1),
            }) { ShowIcons = false };

        }

        /// <summary>
        /// Represents a selectable value in a combo box
        /// </summary>
        internal class ItemUiComboItem
        {
            public ItemUiComboItem() { }
            public ItemUiComboItem(Image icon, string text, int value) {
                this.Icon = icon;
                this.Text = text;
                this.Value = value;
            }

            /// <summary>The icon to display.</summary>
            public Image Icon { get; set; }

            /// <summary>The text to display.</summary>
            public string Text { get; set; }

            /// <summary>The raw value associated with the item. A negative value indicates that this item
            /// should function as an 'other' item, displayed when no other item is applicable, and selected
            /// by the user to enter a custom value via another UI element.</summary>
            public int Value { get; set; }



        }

        ////internal ToolStripItem GetElement(ItemUiRole element) {
        ////    return Controls[element];
        ////}

        internal void UpdateElement(ItemUiRole element) {
            throw new NotImplementedException();
        }

        internal void LoadScreen(ItemScreenData screenData, ItemInstance selectedItem) {
            SelectedScreen = screenData;
            this.SelectedItem = selectedItem.Data;

            ItemListCombo.BeginUpdate();
            ItemListCombo.Items.Clear();

            int selection = -1;
            for (int i = 0; i < screenData.Items.Count; i++) {
                ItemListCombo.Items.Add(screenData.Items[i].ItemType.ToString());
                if (selectedItem.Data == screenData.Items[i])
                    selection = i;
            }

            ItemListCombo.SelectedIndex = selection;
            ItemListCombo.EndUpdate();

            switch (selectedItem.Data.ItemType) {
                case ItemTypeIndex.Enemy:
                    DisplayScheme(ItemUiScheme.Enemy);
                    break;
                case ItemTypeIndex.PowerUp:
                    DisplayScheme(ItemUiScheme.PowerUp);
                    break;
                case ItemTypeIndex.Mella:
                    DisplayScheme(ItemUiScheme.Blank);
                    break;
                case ItemTypeIndex.Elevator:
                    DisplayScheme(ItemUiScheme.Elevator);
                    break;
                case ItemTypeIndex.Turret:
                    DisplayScheme(ItemUiScheme.Turret);
                    break;
                case ItemTypeIndex.MotherBrain:
                    DisplayScheme(ItemUiScheme.Blank);
                    break;
                case ItemTypeIndex.Zebetite:
                    DisplayScheme(ItemUiScheme.Zebetite);
                    break;
                case ItemTypeIndex.Rinkas:
                    DisplayScheme(ItemUiScheme.Rinka);
                    break;
                case ItemTypeIndex.Door:
                    DisplayScheme(ItemUiScheme.Door);
                    break;
                case ItemTypeIndex.PalSwap:
                    DisplayScheme(ItemUiScheme.Blank);
                    break;
            }
        }

        /// <summary>
        /// Notifies this object that an undoable action has been done or undone. This triggers UI updates as necessary.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="undo"></param>
        internal void NotifyAction(EditItemProperty a, bool undo) {
            if (a.Item == SelectedItem) {
                BeginUpdate();
                PopulateItemUi(_CurrentScheme);
                EndUpdate();
            }
        }
    }

    ///// <summary>
    ///// Defines a set of controls and behavior used to edit an item.
    ///// </summary>
    //class ItemUiScheme
    //{
    //    public ItemUiScheme() {
    //        Items = new List<Item>();
    //    }
    //    public ItemUiScheme(IEnumerable<Item> items) 
    //    :this() {
    //        Items.AddRange(items);
    //    }

    //    public static ItemUiScheme TestScheme = new ItemUiScheme(
    //        new Item[] {
    //            new Item(){ 
    //                Element = ItemUiRole.DoorSizeCombo,
    //                Populate = delegate(ItemEditArgs<ToolStripComboBox> args) {
    //                    //args.Control.SelectedIndex = args.Item.
    //                }
    //            }
    //        });

    //    public List<Item> Items { get; private set; }

    //    /// <summary>
    //    /// Represents a UI element within a scheme.
    //    /// </summary>
    //    internal class Item
    //    {
    //        /// <summary>Identifies the UI element this item represents</summary>
    //        public ItemUiRole Element { get; private set; }
    //        /// <summary>Identifies the minimum value to allow the user to select for numerical controls.</summary>
    //        public int Minimum { get; private set; }
    //        /// <summary>Identifies the maximum value to allow the user to select for numerical controls.</summary>
    //        public int Maximum { get; private set; }

    //        public ItemElementEvent Populate { get; set; }
    //        public ItemElementEvent UserEdit { get; set; }

    //    }

    //    class ItemEditArgs<TControl>
    //    {
    //        public ItemEditArgs(ItemEditUI host, ItemUiRole element) {
    //            _Host = host;
    //            Control = (TControl)host.GetElement(element);
    //        }
    //        ItemEditUI _Host;
    //        public ItemData Item { get { return _Host.SelectedItem; } }
    //        public TControl Control { get; private set; }

    //        void UpdateElement(ItemUiRole element) {
    //            _Host.UpdateElement(element);
    //        }
    //    }
    //}

    //internal delegate void ItemUiPopulator<TControl>(TControl control);
    //internal delegate void ItemUiEditHandler<TControl>(TControl control, out Editroid.UndoRedo.EditroidAction action);

    //internal delegate void ItemElementEvent<TControl>(ItemUiScheme.ItemEditArgs<TControl> args);

    enum ItemUiRole
    {
        SlotEdit,
        EnemyEdit,
        ElevatorCombo,
        TurretCombo,
        ZebetiteCombo,
        RinkaCombo,
        DoorTypeCombo,
        DoorSizeCombo,
        PowerupCombo,
        DifficultToggle,
        RawEdit
    }



    class ItemUiScheme
    {
        public ItemUiScheme() {
        }
        public ItemUiScheme(IEnumerable<ItemUiElement> items) {
            _Elements.AddRange(items);
        }

        List<ItemUiElement> _Elements = new List<ItemUiElement>();
        public List<ItemUiElement> Elements { get { return _Elements; } }

        public static ItemUiScheme Enemy = new ItemUiScheme(new ItemUiElement[] {
                ItemUiElement.DifficultToggle,
                ItemUiElement.EnemyType,
                ItemUiElement.SpriteSlot,
            });
        public static ItemUiScheme PowerUp = new ItemUiScheme(new ItemUiElement[] {
                ItemUiElement.PowerUpCombo, 
                ItemUiElement.PowerUpRaw,
            });
        public static ItemUiScheme Elevator = new ItemUiScheme(new ItemUiElement[] {
                ItemUiElement.ElevatorCombo,
                ItemUiElement.ElevatorRaw,
            });
        public static ItemUiScheme Turret = new ItemUiScheme(new ItemUiElement[] {
                ItemUiElement.TurretCombo,
                ItemUiElement.TurretRaw,
            });
        public static ItemUiScheme Zebetite = new ItemUiScheme(new ItemUiElement[] {
                ItemUiElement.ZebetiteCombo,
                ItemUiElement.ZebetiteRaw,
            });
        public static ItemUiScheme Rinka = new ItemUiScheme(new ItemUiElement[] {
                ItemUiElement.RinkaCombo,
                ItemUiElement.RinkaRaw,
            });
        public static ItemUiScheme Door = new ItemUiScheme(new ItemUiElement[] {
                ItemUiElement.DoorTypeCombo,
                ItemUiElement.DoorRaw,
            });
        public static ItemUiScheme Blank = new ItemUiScheme();
    }


    /// <summary>
    /// Represents a item editing UI element within a scheme.
    /// </summary>
    /// <remarks>
    /// The same ItemUiElement may be used in multiple schemes. Additionally,
    /// multiple ItemUiElement objects may be defined to use for the same UI element
    /// in separate schemes (e.g. the raw value box)</remarks>
    class ItemUiElement
    {
        public ItemUiElement(ItemUiRole role) {
            this.Role = role;
        }
        public ItemUiElement(ItemUiRole role, int min, int max) {
            this.Role = role;
            this.Minimum = min;
            this.Maximum = max;
        }

        public ItemUiRole Role { get; private set; }
        /// <summary>
        /// Gets the minimal enterable value. Applies to text boxes only.
        /// </summary>
        public int Minimum { get; private set; }
        /// <summary>
        /// Gets the maximum enterable value. Applies to text boxes only.
        /// </summary>
        public int Maximum { get; private set; }

        /// <summary>
        /// Specifies the function to use to get the value that will initially populate the control.
        /// </summary>
        public Action<ItemUiGetterArgs> Populator { get; set; }

        /// <summary>
        /// Specifies the function to use to update item data in response to UI input
        /// </summary>
        public Action<ItemUiSetterArgs> Setter { get; set; }

        #region UI Element Definitions
        public static ItemUiElement SpriteSlot = new ItemUiElement(ItemUiRole.SlotEdit, 0, 15) {
            Populator = args => args.Value = args.Data.SpriteSlot,
            Setter = args => SetItemSlot(args)
        };

        public static ItemUiElement DifficultToggle = new ItemUiElement(ItemUiRole.DifficultToggle) {
            Populator = args => args.Value = args.EnemyData.Difficult ? 1 : 0,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.enemy_difficulty, args)
        };
        public static ItemUiElement EnemyType = new ItemUiElement(ItemUiRole.EnemyEdit, 0, 15) {
            Populator = args => args.Value = args.EnemyData.EnemyType,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.enemy_type, args)
        };

        public static ItemUiElement PowerUpCombo = new ItemUiElement(ItemUiRole.PowerupCombo) {
            Populator = args => args.Value = (int)args.PowerupData.PowerUp,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.power_up_type, args)
        };
        public static ItemUiElement PowerUpRaw = new ItemUiElement(ItemUiRole.RawEdit, 0, 15) {
            Populator = args => args.Value = (int)args.PowerupData.PowerUp,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.power_up_type, args)
        };
        public static ItemUiElement ElevatorCombo = new ItemUiElement(ItemUiRole.ElevatorCombo) {
            Populator = args => args.Value = (int)args.ElevatorData.ElevatorType,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.elevator_destination, args)
        };
        public static ItemUiElement ElevatorRaw = new ItemUiElement(ItemUiRole.RawEdit, 0, 255) {
            Populator = args => args.Value = (int)args.ElevatorData.ElevatorType,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.elevator_destination, args)
        };

        public static ItemUiElement TurretCombo = new ItemUiElement(ItemUiRole.TurretCombo) {
            Populator = args => args.Value = args.Data.SpriteSlot,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.turret_type, args)
        };
        public static ItemUiElement TurretRaw = new ItemUiElement(ItemUiRole.RawEdit, 0, 15) {
            Populator = args => args.Value = args.Data.SpriteSlot,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.turret_type, args)
        };

        public static ItemUiElement ZebetiteCombo = new ItemUiElement(ItemUiRole.ZebetiteCombo) {
            Populator = args => args.Value = args.Data.SpriteSlot,
            Setter = args => SetItemSlot(args)
        };

        public static ItemUiElement ZebetiteRaw = new ItemUiElement(ItemUiRole.RawEdit, 0, 15) {
            Populator = args => args.Value = args.Data.SpriteSlot,
            Setter = args => SetItemSlot(args)
        };

        public static ItemUiElement RinkaCombo = new ItemUiElement(ItemUiRole.RinkaCombo) {
            Populator = args => args.Value = args.Data.SpriteSlot,
            Setter = args => SetItemSlot(args)

        };
        public static ItemUiElement RinkaRaw = new ItemUiElement(ItemUiRole.RawEdit, 0, 15) {
            Populator = args => args.Value = args.Data.SpriteSlot,
            Setter = args => SetItemSlot(args)

        };


        public static ItemUiElement DoorTypeCombo = new ItemUiElement(ItemUiRole.DoorTypeCombo) {
            Populator = args => args.Value = (int)args.DoorData.Side | (int)args.DoorData.Type,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.door_value, args)
        };

        public static ItemUiElement DoorRaw = new ItemUiElement(ItemUiRole.RawEdit, 0, 255) {
            Populator = args => args.Value = (int)args.DoorData.Side | (int)args.DoorData.Type,
            Setter = args => SetItemValue(Editroid.Actions.ItemProperty.door_value, args)
            //Setter = args => {
            //    args.DoorData.Side = (DoorSide)(args.EnteredValue & 0xF0);
            //    args.DoorData.Type = (DoorType)(args.EnteredValue & 0x0F);
            //    args.UpdateElement(ItemUiRole.DoorTypeCombo);
            //    args.UpdateElement(ItemUiRole.DoorSideCombo);
            //}
        };


        /// <summary>
        /// Helper method. Applies the value change specified by 'args' to the item property specified if the value is >= 0.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="args"></param>
        static void SetItemValue(Editroid.Actions.ItemProperty property, ItemUiSetterArgs args) {
            if (args.EnteredValue >= 0) {
                args.EditedProperty = property;
                args.AssignedValue = args.EnteredValue;
            }
        }
        /// <summary>
        /// Helper method. Applies the value change specified by 'args' to the item's sprite slot value if the value is >= 0.
        /// </summary>
        /// <param name="args"></param>
        static void SetItemSlot(ItemUiSetterArgs args) {
            SetItemValue(Editroid.Actions.ItemProperty.enemy_slot, args);
        }
        #endregion
    }


    class ItemUiArgs
    {
        public ItemUiArgs(ItemData data) {
            this.Data = data;
        }

        public ItemData Data { get; private set; }


        public ItemSingleByteData SingleByteData { get { return Data as ItemSingleByteData; } }
        public ItemTurretData TurretData { get { return Data as ItemTurretData; } }
        public ItemElevatorData ElevatorData { get { return Data as ItemElevatorData; } }
        public ItemDoorData DoorData { get { return Data as ItemDoorData; } }
        public ItemEnemyData EnemyData { get { return Data as ItemEnemyData; } }
        public ItemPowerupData PowerupData { get { return Data as ItemPowerupData; } }
    }

    class ItemUiGetterArgs : ItemUiArgs
    {
        public ItemUiGetterArgs(ItemData data)
            : base(data) {
            this.Data = data;
        }

        public ItemData Data { get; private set; }
        /// <summary>
        /// Set this property to the value that should be displayed in the UI element
        /// </summary>
        public int Value { get; set; }

        ////public ItemSingleByteData SingleByteData { get { return Data as ItemSingleByteData; } }
        ////public ItemTurretData TurretData { get { return Data as ItemTurretData; } }
        ////public ItemElevatorData ElevatorData { get { return Data as ItemElevatorData; } }
        ////public ItemDoorData DoorData { get { return Data as ItemDoorData; } }
        ////public ItemEnemyData EnemyData { get { return Data as ItemEnemyData; } }
        ////public ItemPowerupData PowerupData { get { return Data as ItemPowerupData; } }

    }

    class ItemUiSetterArgs : ItemUiArgs
    {
        public ItemUiSetterArgs(ItemData data)
            : base(data) {
        }

        List<ItemUiRole> _elementsToUpdate;

        /// <summary>
        /// Gets the value that was entered into the UI element
        /// </summary>
        public int EnteredValue { get; set; }

        /// <summary>
        /// Set this property to specify what aspect of the selected item should be modified.
        /// </summary>
        public Editroid.Actions.ItemProperty EditedProperty { get; set; }
        /// <summary>
        /// Set the property to specify what value should be assigned to the selected item.
        /// </summary>
        public int AssignedValue { get; set; }
        /// <summary>
        /// Call this method to specify that another UI should refresh itself in response to an edit in this element
        /// (e.g. editing 'raw' will update an elevator's destination)
        /// </summary>
        /// <param name="element"></param>
        public void UpdateElement(ItemUiRole element) {
            if (_elementsToUpdate == null) _elementsToUpdate = new List<ItemUiRole>();
            _elementsToUpdate.Add(element);
        }

        public IList<ItemUiRole> GetElementsToUpdate() {
            return _elementsToUpdate.AsReadOnly();
        }
    }

    static class ItemUiCheckboxImages
    {
        static Bitmap _Checked;
        static Bitmap _Unhecked;

        public static Image Checked {
            get {
                if (_Checked == null) _Checked = CreateImage(true);
                return _Checked;
            }
        }
        public static Image Unchecked {
            get {
                if (_Unhecked == null) _Unhecked = CreateImage(false);
                return _Unhecked;
            }
        }

        private static Bitmap CreateImage(bool isChecked) {
            var result = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(result)) {
                g.Clear(Color.Transparent);
                CheckBoxRenderer.DrawCheckBox(g, new Point(2, 1), isChecked ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal : System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            }

            return result;
        }
    }
}
