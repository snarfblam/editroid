using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Editroid.Properties;

namespace Editroid
{
    /// <summary>
    /// Provides an interface to create and apply IPS patches.
    /// </summary>
    public partial class IpsForm:Form
    {
        private bool currentPatched;
        /// <summary>
        /// Gets whether the current ROM is patched.
        /// </summary>
        public bool CurrentPatched {
            get { return currentPatched; }
        }


        /// <summary>
        ///  Creates a form.
        /// </summary>
        public IpsForm() {
            InitializeComponent();
        }

        string NoneString = "(None)";
        string CurrentString = "(Current)";

        private void btnRomCurrent_Click(object sender, EventArgs e) {
            LoadRom(CurrentString);
            picRom.Image = Resources.MemoryFile;
            
        }
        private void btnRomLoad_Click(object sender, EventArgs e) {
            if(RomOpener.ShowDialog() == DialogResult.OK) {
                LoadRom(RomOpener.FileName);
                picRom.Image = Resources.File;
            }
        }

        private void LoadRom(string filename) {
            lblRomPath.Text = filename;
            grpRom.ForeColor = SystemColors.ControlText;
            btnRomApply.Enabled = lblPatchPath.Text != NoneString;

            if(lblComparePath.Text != NoneString && lblRomPath.Text != NoneString)
                btnPatchCreate.Enabled = true;
            EnableApplyButtons();
        }

        private void btnCompareCurrent_Click(object sender, EventArgs e) {
            LoadCompare(CurrentString);
            picCompare.Image = Resources.MemoryFile;
        }

        private void LoadCompare(string filename) {
            lblComparePath.Text = filename;
            grpCompare.ForeColor = SystemColors.ControlText;
            btnCompareApply.Enabled = lblPatchPath.Text != NoneString;

            if(lblComparePath.Text != NoneString && lblRomPath.Text != NoneString)
                btnPatchCreate.Enabled = true;
            EnableApplyButtons();
        }

        private void btnCompareLoad_Click(object sender, EventArgs e) {
            if(RomOpener.ShowDialog() == DialogResult.OK) {
                LoadCompare(RomOpener.FileName);
                picCompare.Image = Resources.File;
            }
        }

        private MetroidRom currentRom;

        /// <summary>
        /// Gets or sets the current rom. This should points to the rom loaded in the editor.
        /// </summary>
        public MetroidRom CurrentRom {
            get { return currentRom; }
            set { 
                currentRom = value;
                btnCompareCurrent.Enabled = btnRomCurrent.Enabled = value != null;
            }
        }

        private void btnPatchLoad_Click(object sender, EventArgs e) {
            if(PatchOpener.ShowDialog() == DialogResult.OK) {
                lblPatchPath.Text = PatchOpener.FileName;
                EnableApplyButtons();

                picIps.Image = Resources.Patch;
                grpPatch.ForeColor = SystemColors.WindowText;
            }
        }

        private void EnableApplyButtons() {
            btnRomApply.Enabled = (lblPatchPath.Text != NoneString && lblRomPath.Text != NoneString);
            btnCompareApply.Enabled = (lblPatchPath.Text != NoneString && lblComparePath.Text != NoneString);
        }

        private void btnRomApply_Click(object sender, EventArgs e) {
            try {
                // Copy specified ROM into a memory stream
                byte[] romData = LoadFileData(lblRomPath.Text);
                MemoryStream dataStream = new MemoryStream();
                dataStream.Write(romData, 0, romData.Length);

                // Put patched rom in romData
                ApplyPatch(dataStream, lblPatchPath.Text);
                romData = dataStream.ToArray();

                bool patchRomIsFromEditor = lblRomPath.Text == CurrentString;

                if(patchRomIsFromEditor) {
                    currentPatched = true;
                    currentRom.data = romData;
                    OnCurrentRomPatched();
                    MessageBox.Show("The patch was applied successfully. The patched ROM has not been saved to disk.");
                } else {
                    File.WriteAllBytes(lblRomPath.Text, romData);
                    MessageBox.Show("The patch was applied successfully.");
                }                                                           
            }
            catch(Exception ex) {
                MessageBox.Show("An error occurred while applying the patch (" + ex.GetType().ToString() + ": " + ex.Message + ").");
            }
        }

        protected void OnCurrentRomPatched() {
            if (currentPatched != null)
                CurrentRomPatched(this, EventArgs.Empty);
        }
        public event EventHandler CurrentRomPatched;

        private void btnCompareApply_Click(object sender, EventArgs e) {
            try {
                // Create memory stream
                byte[] data = LoadFileData(lblRomPath.Text);
                MemoryStream dataStream = new MemoryStream();
                dataStream.Write(data, 0, data.Length);

                ApplyPatch(dataStream, lblPatchPath.Text);
                data = dataStream.ToArray();


                if(lblRomPath.Text == CurrentString) {
                    currentPatched = true;
                    MessageBox.Show("The patch was applied successfully. The patched ROM has not been saved to disk.");
                } else {
                    File.WriteAllBytes(lblRomPath.Text, data);
                    MessageBox.Show("The patch was applied successfully.");
                }
            }
            catch(Exception ex) {
                MessageBox.Show("An error occurred while applying the patch (" + ex.GetType().ToString() + ": " + ex.Message + ").");
            }
        }

        private byte[] LoadFileData(string path) {
            if(path == CurrentString)
                return currentRom.data;

            return File.ReadAllBytes(path);
        }

        private void ApplyPatch(Stream data, string patchPath) {
            Stream s = new FileStream(patchPath, FileMode.Open);
            IpsFile patch = new IpsFile(s);
            s.Close();
            foreach(IpsRecord record in patch) {
                
                record.Apply(data);
            }
        }
        private void btnPatchCreate_Click(object sender, EventArgs e) {
            byte[] rom = LoadFileData(lblRomPath.Text);
            byte[] compare = LoadFileData(lblComparePath.Text);

            IpsFile ips = new IpsFile();

            int size = Math.Min(rom.Length, compare.Length);

            compareState state = compareState.LookingForDifference;
            int differenceStart = 0;
            int differenceLength;

            for(int i = 0; i < size; i++) {
                switch(state) {
                    case compareState.LookingForDifference:
                        if(rom[i] != compare[i]) {
                            differenceStart = i;
                            state = compareState.ScanningDifference;
                        }
                        break;
                    case compareState.ScanningDifference:
                        if(rom[i] == compare[i]){
                            differenceLength = i - differenceStart;
                            state = compareState.LookingForDifference;
                            
                            byte[] data = new byte[differenceLength];
                            Array.Copy(rom, differenceStart, data, 0, differenceLength);

                            ips.AddRecord(new IpsRecord(data, differenceStart));
                        }
                        break;
                }
            }

            if(PatchSaver.ShowDialog() == DialogResult.OK) {
                using (FileStream fs = new FileStream(PatchSaver.FileName, FileMode.Create)){
                    ips.Save(fs);
                }

                lblPatchPath.Text = PatchSaver.FileName;
                picIps.Image = Resources.Patch;
            }

            grpPatch.ForeColor = SystemColors.WindowText;
            EnableApplyButtons();
        }

        enum compareState
        {
            LookingForDifference,
            ScanningDifference
        }

    }
}