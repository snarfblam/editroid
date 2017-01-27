using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;
using Editroid.Actions;

namespace Editroid
{
    public partial class PasswordDataGenerator : Form
    {
        TreeNode DoorNode;
        TreeNode OmittedNode;

        public PasswordDataGenerator() {
            InitializeComponent();

            DoorNode = dataList.Nodes["DoorNode"];
            OmittedNode = dataList.Nodes["OmittedNode"];
            foreach (TreeNode n in dataList.Nodes) {
                n.ImageIndex = -1;
                n.SelectedImageIndex = -1;
            }
       }


        MetroidRom rom;
        MapControl map;
        int checkedItemCount = 0;
        internal void LoadRom(MetroidRom gameRom, MapControl map) {
            this.rom = gameRom;
            this.map = map;

            FindDoors();
            FindItems();

            UpdateItemCount();
        }

        private void UpdateItemCount() {
            ItemLabel.Text = checkedItemCount.ToString() + " items.";
            if (checkedItemCount > ROM.PasswordData.DataCount) {
                ItemLabel.Text += " (Too many)";
                btnGenerate.Enabled = false;
            } else {
                btnGenerate.Enabled = true;
            }
        }

        private void FindItems() {
            LoadItems(LevelIndex.Brinstar);
            LoadItems(LevelIndex.Norfair);
            LoadItems(LevelIndex.Ridley);
            LoadItems(LevelIndex.Kraid);
            LoadItems(LevelIndex.Tourian);
        }

        
        private void LoadItems(LevelIndex level) {
            TreeNode node = GetNode(level);
            Level l = rom.GetLevel(level);

            ////foreach (ItemRowEntry row in l.ItemTable_DEPRECATED) {
            ////    foreach (ScreenItems screen in row) {
            foreach (ItemScreenData screen in l.Items) {
                foreach (ItemData item in screen.Items) {
                    if (item.ItemType == ItemTypeIndex.PowerUp) {
                        int x = screen.MapX;
                        int y = screen.MapY;
                        var powerUp = ((ItemPowerupData)item).PowerUp;
                        int powerupIndex = (int)powerUp;

                        TreeNode n = new TreeNode("(" + x.ToString("x") + ", " + y.ToString("x") + ") " + powerUp.ToString().Replace('_',' '), powerupIndex, powerupIndex);
                        n.Tag = new datTag(new Point(x, y), powerupIndex, level);
                        if (powerUp == PowerUpType.IceBeam
                            || powerUp == PowerUpType.WaveBeam
                            || powerUp == PowerUpType.LongBeam) {
                            OmittedNode.Nodes.Add(n);
                        } else {
                            node.Nodes.Add(n);
                            checkedItemCount++;
                        }

                    }
                }
            }
            ////    }
            ////}
        }

        private TreeNode GetNode(LevelIndex levelIndex) {
            switch (levelIndex) {
                case LevelIndex.Brinstar:
                    return dataList.Nodes["Brinstar"];
                case LevelIndex.Norfair:
                    return dataList.Nodes["Norfair"];
                case LevelIndex.Tourian:
                    return dataList.Nodes["Tourian"];
                case LevelIndex.Kraid:
                    return dataList.Nodes["Kraid"];
                case LevelIndex.Ridley:
                    return dataList.Nodes["Ridley"];
                case LevelIndex.None:
                    return DoorNode;
            }

            return null;
        }

        private void FindDoors() {
            for (int y = 0; y < 32; y++) {
                for (int x = 0; x < 32; x++) {
                    LevelIndex level = map.GetLevel(x, y);
                    if (level != LevelIndex.None) {
                        int screenIndex = rom.GetScreenIndex(x, y);
                        if (screenIndex < rom[level].Screens.Count) {
                            Screen screen = rom.GetLevel(level).Screens[screenIndex];

                            if (screen.RightDoor != null && (screen.RightDoor.Type == DoorType.Missile || screen.RightDoor.Type == DoorType.TenMissile)) {
                                TreeNode n = new TreeNode("(" + x.ToString("x") + ", " + y.ToString("x") + ")", 10, 10);
                                n.Tag = new datTag(new Point(x, y), 10);
                                DoorNode.Nodes.Add(n);

                                checkedItemCount++;
                            }
                        }
                    }
                }
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            if (e.Node.Tag is datTag) {
                Program.MainForm.CenterOnScreen(((datTag)e.Node.Tag).p);
            }
        }
        
        struct datTag
        {
            public Point p;
            public int type;
            public LevelIndex level;

            public datTag(Point p, int type) {
                this.p = p;
                this.type = type;
                level = LevelIndex.None;
            }
            public datTag(Point p, int type, LevelIndex level) {
                this.p = p;
                this.type = type;
                this.level = level;
            }

        }

        private void dataList_DoubleClick(object sender, EventArgs e) {
            Point mouse = dataList.PointToClient(MousePosition);
            TreeNode selection = dataList.SelectedNode;

            if (dataList.GetNodeAt(mouse) == selection && selection.Tag is datTag) {
                if (selection.Parent == OmittedNode) {
                    selection.Parent.Nodes.Remove(selection);
                    GetNode(((datTag)selection.Tag).level).Nodes.Add(selection);
                    checkedItemCount++;
                } else {
                    selection.Parent.Nodes.Remove(selection);
                    OmittedNode.Nodes.Add(selection); 
                    checkedItemCount--;

                }
                UpdateItemCount();
                dataList.SelectedNode = selection;
                selection.EnsureVisible();
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e) {
            byte[] data = new byte[PasswordData.DataCount* 2];
            int index = 0;
            WriteNodes(DoorNode, ref index, data);
            WriteNodes(GetNode(LevelIndex.Brinstar), ref index, data);
            WriteNodes(GetNode(LevelIndex.Norfair), ref index, data);
            WriteNodes(GetNode(LevelIndex.Ridley), ref index, data);
            WriteNodes(GetNode(LevelIndex.Kraid), ref index, data);
            WriteNodes(GetNode(LevelIndex.Tourian), ref index, data);

            Program.PerformAction(Program.Actions.OverwritePasswordData(data));

            Close();
        }

        private void WriteNodes(TreeNode node, ref int index, byte[] data) {
            foreach (TreeNode n in node.Nodes) {
                PasswordDatum writer = new PasswordDatum(data, index * 2);
                index++;

                datTag tag = (datTag)n.Tag;
                writer.MapX = tag.p.X;
                writer.MapY = tag.p.Y;
                writer.Item = tag.type;
            }
        }

 }
}