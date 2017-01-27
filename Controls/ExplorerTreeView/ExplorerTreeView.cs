using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace WilsonProgramming
{
    public partial class ExplorerTreeView : UserControl
    {
        public ExplorerTreeView()
        {
            InitializeComponent();

            // Set the TreeView image list to the system image list.
            SystemImageList.SetTVImageList(treeWnd.Handle);
            LoadRootNodes();
        }
        ShellItem m_shDesktop;
        /// <summary>
        /// Loads the root TreeView nodes.
        /// </summary>
        private void LoadRootNodes()
        {
            // Create the root shell item.
            m_shDesktop = new ShellItem();

            // Create the root node.
            TreeNode tvwRoot = new TreeNode();
            tvwRoot.Text = m_shDesktop.DisplayName;
            tvwRoot.ImageIndex = m_shDesktop.IconIndex;
            tvwRoot.SelectedImageIndex = m_shDesktop.IconIndex;
            tvwRoot.Tag = m_shDesktop;

            // Now we need to add any children to the root node.
            ArrayList arrChildren = m_shDesktop.GetSubFolders();
            foreach (ShellItem shChild in arrChildren)
            {
                TreeNode tvwChild = new TreeNode();
                tvwChild.Text = shChild.DisplayName;
                tvwChild.ImageIndex = shChild.IconIndex;
                tvwChild.SelectedImageIndex = shChild.IconIndex;
                tvwChild.Tag = shChild;

                // If this is a folder item and has children then add a place holder node.
                if (shChild.IsFolder && shChild.HasSubFolder)
                    tvwChild.Nodes.Add("PH");
                tvwRoot.Nodes.Add(tvwChild);
            }

            // Add the root node to the tree.
            treeWnd.Nodes.Clear();
            treeWnd.Nodes.Add(tvwRoot);
            tvwRoot.Expand();
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            m_shDesktop.Dispose();
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        internal ShellItem SelectedItem {
            get {
                if (treeWnd.SelectedNode == null) return null;
                return treeWnd.SelectedNode.Tag as ShellItem;
            }
        }
    }
}
