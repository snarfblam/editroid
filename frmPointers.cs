using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;

namespace Editroid
{
    internal partial class frmPointers : Form
    {
        public frmPointers() {
            InitializeComponent();

        }

        MetroidRom rom;
        public void SetRom(MetroidRom rom) {
            this.rom = rom;
            LoadTree();
        }

        private void LoadTree() {
            rom.SerializeAllData();

            PointerTree.Nodes.Clear();
            PointerTree.Nodes.Add(MakeNode(rom));
            PointerTree.Nodes[0].Expand();
        }

        private TreeNode MakeNode(IRomDataObject item) {
            TreeNode node = new TreeNode(item.DisplayName);
            node.Tag = item;
            node.ImageKey = node.SelectedImageKey = node.StateImageKey = getImageKey(item);

            IRomDataParentObject p = item as IRomDataParentObject;
            if (p != null) {
                if (p.HasSubItems) {
                    List<IRomDataObject> objects = new List<IRomDataObject>();
                    objects.AddRange(p.GetSubItems());
                    objects.Sort(romDataItemOffsetCompare);
                    foreach (IRomDataObject subItem in objects) {
                        node.Nodes.Add(MakeNode(subItem));
                    }
                }
            }

            return node;
        }

        int romDataItemOffsetCompare(IRomDataObject a, IRomDataObject b) {
            if (a is ItemLoader || b is ItemLoader) { }
            return a.Offset - b.Offset ;
        }


        private string getImageKey(IRomDataObject o) {
            if (o is MetroidRom)
                return "ROM";
            if (o is Level)
                return "Level";
            if (o is ComboTable)
                return "Combo";
            if (o is StructureCollection)
                return "Struct";
            if (o is PaletteTable)
                return "Palette";
            if (o is PatternTable)
                return "Patterns";
            if (o is ScreenCollection)
                return "Screen";
            if (o is PointerTableData)
                return "Pointers";
            if (o is ItemLoader || o is ItemRowEntry || o is ScreenItems)
                return "Items";

            return "Misc";
        }



        private void PointerTree_AfterSelect(object sender, TreeViewEventArgs e) {
            DisplayNode(e.Node);
        }

        private void DisplayNode(TreeNode node) {
            IRomDataParentObject data = node.Tag as IRomDataParentObject;
            if (data != null && data.HasListItems) {
                ShowObjectSubitems(data);
            }

            IRomDataObject obj = node.Tag as IRomDataObject;
            var ex = obj as IRomDataObjectEx;

            bool nonContiguous = false; // Assume data is contiguous if obj does not specify
            if (ex != null) nonContiguous = !ex.Contiguous;

            // Show hex data for an object that has contiguous data and a specified size
            if (!nonContiguous && (obj != null && obj.Size != 0)) {
                DisplayDataString(RomDataObjects.FormatData(obj, rom.data), obj.Offset, obj.Size);
            }
        }

        private void ShowObjectSubitems(IRomDataParentObject data) {
            PointerList.Items.Clear();

            IList<LineDisplayItem> items = data.GetListItems();
            foreach (LineDisplayItem item in items) {
                ListViewItem newItem = new ListViewItem(new string[]{
                        item.text,
                        item.offset.ToString("X"),
                        item.length.ToString("X"),
                        item.FormatData()
                    });
                newItem.Tag = item;
                PointerList.Items.Add(newItem);
            }
        }

        private void DisplayDataString(string text, int offset, int size) {
            HexDisplay.Text = text;
            HexBoxHeader.Text = "0x" + size.ToString("X") + " byte" + Tools.Quantify(size) + " at 0x" + offset.ToString("X");
        }

        private void DisplayPalette(Level level) {
            ItemHeader.Text = "Name";

            // Bg paletteIndex
            ListViewItem bgItem = new ListViewItem("Background");
            bgItem.ImageKey = "Palette";
            bgItem.SubItems.Add(level.BgPalette.Offset.ToString("x"));
            PointerList.Items.Add(bgItem);

            // SpriteDefinition pal
            ListViewItem spriteItem = new ListViewItem("Sprite");
            spriteItem.ImageKey = "Palette";
            spriteItem.SubItems.Add(level.SpritePalette.Offset.ToString("x"));
            PointerList.Items.Add(spriteItem);

            // Alt paletteIndex
            if (level.Index == LevelIndex.Brinstar || level.Index == LevelIndex.Norfair) {
                ListViewItem bgItemAlt = new ListViewItem("Background Alt");
                bgItemAlt.ImageKey = "Palette";
                bgItemAlt.SubItems.Add(level.BgAltPalette.Offset.ToString("x"));
                PointerList.Items.Add(bgItemAlt);
            }
        }

        private void DisplayStructures(Level level) {
            ItemHeader.Text = "Index";

            int structCount = level.StructCount;
            for (int i = 0; i < structCount; i++) {
                ListViewItem newItem = new ListViewItem(i.ToString("X"));
                newItem.SubItems.Add(level.GetStruct(i).Offset.ToString("X"));

                PointerList.Items.Add(newItem);
            }
        }

        private void DisplayScreens(Level level) {
            ItemHeader.Text = "Index";

            int screenCount = level.Screens.Count;
            for (int i = 0; i < screenCount; i++) {
                ListViewItem newItem = new ListViewItem(i.ToString("X"));
                newItem.SubItems.Add(level.Screens[i].Offset.ToString("X"));

                PointerList.Items.Add(newItem);
            }
        }

        private void PointerList_SelectedIndexChanged(object sender, EventArgs e) {
            if (PointerList.SelectedIndices.Count > 0) {
                ListViewItem item = PointerList.SelectedItems[0];
                if (item.Tag is LineDisplayItem) {
                    LineDisplayItem data = (LineDisplayItem)item.Tag;
                    DisplayDataString(item.SubItems[3].Text, data.offset, data.length);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            LoadTree();
        }

        protected override void OnDeactivate(EventArgs e) {
            base.OnDeactivate(e);

            toolbar.Enabled = false;
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);

            toolbar.Enabled = true;
        }

    }

    class NodeTag
    {
        public NodeTag() { }
        public NodeTag(Level level, int offset, int byteCount) {
            this.level = level;
            this.offset = offset;
            this.byteCount = byteCount;
        }

        private int offset;

        public int Offset {
            get { return offset; }
            set { offset = value; }
        }

        private int byteCount;

        public int ByteCount {
            get { return byteCount; }
            set { byteCount = value; }
        }

        private Level level;

        public Level Level {
            get { return level; }
            set { level = value; }
        }

        private nodeTypes type = nodeTypes.auto;

        public nodeTypes Type {
            get { return type; }
            set { type = value; }
        }

    }

    class ScreenTag : NodeTag
    {
        private Screen screen;

        public Screen Screen {
            get { return screen; }
            set { screen = value; }
        }

        public ScreenTag(Level level, Screen screen) 
        :base(level,screen.Offset,GetScreenByteCount(screen)) {
            this.screen = screen;
        }

        private static int GetScreenByteCount(Screen screen)
        {
            int result = 2 + screen.Structs.Count * 3 + screen.Enemies.Count * 3 + screen.Doors.Count * 2;
            if (screen.HasBridge)
                result++;
            if (screen.HasEnemyDataSegment)
                result++;

            return result;
        }
    }

    enum nodeTypes
    {
        auto,
        comboList,
        structList,
        paletteList,
        patternRanges,
        areaStart,
        passwordData,
        titleText,
        screenStructlist,
        screenEnemylist,
        screenDoorlist
    }


}