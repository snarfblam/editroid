using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class SamusDropForm : Form
    {
        public SamusDropForm() {
            InitializeComponent();
        }

        private TestRomPatcher patcher;

        public TestRomPatcher Patcher {
            get { return patcher; }
            set {
                patcher = value;

                if (value == null) return;

                VerticalCheck.Checked = value.VerticalScrolling;
                TitleCheck.Checked = value.SkipTitle;
                PasswordCheck.Checked = value.SkipStartScreen;
                IntroCheck.Checked = value.SkipIntro;

                GetEquipment();
                GetFlags();
                GetValues();

                Point start = patcher.InitialScreenPosition;
                start.Offset(-8, -16);
                SetSamusPosition(ref start);
            }
        }

        private void SetSamusPosition(ref Point start) {
            if (start.X < 0) start.X = 0;
            if (start.X > 0xEF) start.X = 0xEF;
            if (start.Y < 0) start.Y = 0;
            if (start.Y > 0xEF) start.Y = 0xEF;

            SamusStartImage.Location = start;
        }

        private void GetValues() {
            HealthBox.Value = patcher.InitialHealth;
            MissileBox.Value = patcher.MissileCount;
            MissileCapacityBox.Value = patcher.MissileCapacity;
            TankBox.Value = patcher.TankCount;
            FullTankBox.Value = patcher.FullTankCount;
        }
        private void SetValues() {
            patcher.InitialHealth = HealthBox.Value;
            patcher.MissileCount = MissileBox.Value;
            patcher.MissileCapacity = MissileCapacityBox.Value;
            patcher.TankCount = TankBox.Value;
            patcher.FullTankCount = FullTankBox.Value;
        }

        private void GetFlags() {
            VerticalCheck.Checked = patcher.VerticalScrolling;
            TitleCheck.Checked = patcher.SkipTitle;
            PasswordCheck.Checked = patcher.SkipStartScreen;
            IntroCheck.Checked = patcher.SkipIntro;
            chkNARPASSWORD.Checked = patcher.Narpassword;
        }
        private void SetFlags() {
            patcher.VerticalScrolling = VerticalCheck.Checked;
            patcher.SkipTitle = TitleCheck.Checked;
            patcher.SkipStartScreen = PasswordCheck.Checked;
            patcher.SkipIntro = IntroCheck.Checked;
            patcher.Narpassword = chkNARPASSWORD.Checked;

        }

        private void GetEquipment() {
            MarumariCheck.Checked = (patcher.Equipment & Editroid.ROM.Equipment.MaruMari) != Editroid.ROM.Equipment.None;
            WaveCheck.Checked = (patcher.Equipment & Editroid.ROM.Equipment.WaveBeam) != Editroid.ROM.Equipment.None;
            LongbeamCheck.Checked = (patcher.Equipment & Editroid.ROM.Equipment.LongBeam) != Editroid.ROM.Equipment.None;
            IceCheck.Checked = (patcher.Equipment & Editroid.ROM.Equipment.IceBeam) != Editroid.ROM.Equipment.None;
            HighjumpCheck.Checked = (patcher.Equipment & Editroid.ROM.Equipment.HighJump) != Editroid.ROM.Equipment.None;
            BombCheck.Checked = (patcher.Equipment & Editroid.ROM.Equipment.Bombs) != Editroid.ROM.Equipment.None;
            ScrewCheck.Checked = (patcher.Equipment & Editroid.ROM.Equipment.ScrewAttack) != Editroid.ROM.Equipment.None;
            VariaCheck.Checked = (patcher.Equipment & Editroid.ROM.Equipment.Varia) != Editroid.ROM.Equipment.None;
        }
        private void SetEquipment() {
            patcher.Equipment = Editroid.ROM.Equipment.None;

            if(MarumariCheck.Checked) patcher.Equipment |= Editroid.ROM.Equipment.MaruMari;
            if (WaveCheck.Checked) patcher.Equipment |= Editroid.ROM.Equipment.WaveBeam;
            if(LongbeamCheck.Checked) patcher.Equipment |= Editroid.ROM.Equipment.LongBeam;
            if(IceCheck.Checked ) patcher.Equipment|= Editroid.ROM.Equipment.IceBeam;
            if(HighjumpCheck.Checked ) patcher.Equipment |= Editroid.ROM.Equipment.HighJump;
            if(BombCheck.Checked)patcher.Equipment |= Editroid.ROM.Equipment.Bombs;
            if(ScrewCheck.Checked) patcher.Equipment  |= Editroid.ROM.Equipment.ScrewAttack;
            if(VariaCheck.Checked)patcher.Equipment |= Editroid.ROM.Equipment.Varia;
        }

        private bool launchTestOnClose;
        public bool LaunchTestOnClose {
            get { return launchTestOnClose; }
        }


        bool expanded = false;
        private void btnSettings_Click(object sender, EventArgs e) {
            //SetEquipment();
            //SetValues();
            //SetFlags();
            //launchTestOnClose = false;
            //Close();

            expanded = !expanded;

            if (expanded) {
                ClientSize = new Size(ClientSize.Width, pnlSettings.Bottom + 8);
            } else {
                ClientSize = new Size(ClientSize.Width, pnlSettings.Top);
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            SetEquipment();
            SetValues();
            SetFlags();
            launchTestOnClose = true;
            Close();
        }

        public void SetImage(Image image) {
            Preview.BackgroundImage = image;
        }


        Point SamusDrag;
        bool dragging = false;
        private void SamusStartImage_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                SamusDrag = e.Location;
                dragging = true;
            }
        }

        private void SamusStartImage_MouseMove(object sender, MouseEventArgs e) {
            if (dragging) {
                Point drag = new Point(
                    e.X - SamusDrag.X,
                    e.Y - SamusDrag.Y);
                if (drag.X != 0 || drag.Y != 0) {
                    Point loc = new Point(SamusStartImage.Left + drag.X, SamusStartImage.Top + drag.Y);

                    SetSamusPosition(ref loc);
                    loc.Offset(8, 16);
                    patcher.InitialScreenPosition = loc;
                }
                
            }
        }

        private void SamusStartImage_MouseUp(object sender, MouseEventArgs e) {
            dragging = false;
        }

        private void SamusDropForm_Load(object sender, EventArgs e) {
            var size = ClientSize;
            size.Height = pnlSettings.Top;
            ClientSize = size;

            txtCommand.Text = Program.MainForm.Settings.TestRoomCommand;
            chkDisplayCmd.Checked = Program.MainForm.Settings.DisplayCmdForTestRoom;
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            Program.MainForm.Settings.TestRoomCommand = txtCommand.Text;
            Program.MainForm.Settings.DisplayCmdForTestRoom = chkDisplayCmd.Checked;

        }
    }
}