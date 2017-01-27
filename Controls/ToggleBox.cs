using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Editroid
{
    class ToggleBox : PictureBox
    {
        public ToggleBox() {
            ToggleOnClick = true;
        }


        private void ResetOffImage() { OffImage = null; }
        private Image offImage;

        public Image OffImage {
            get { return offImage; }
            set {
                offImage = value;
                if (!state)
                    base.Image = value;
            }
        }

        private void ResetOnImage() { OnImage = null; }
        private Image onImage;

        public Image OnImage {
            get { return onImage; }
            set {
                onImage = value;
                if (state) base.Image = value;
            }
        }

        [DefaultValue("True")]
        public bool ToggleOnClick { get; set; }

        public new Image Image { get { return null; } set { } }
        protected override void OnClick(EventArgs e) {
            if (ToggleOnClick) {
                CancelEventArgs ce = new CancelEventArgs(false);
                OnTryToggle(ce);

                if (ce.Cancel == false) {
                    State = !State;
                    OnToggled();

                }
            }

            base.OnClick(e);
        }

        private bool state;

        [DefaultValue(false)]
        public bool State {
            get { return state; }
            set {
                state = value;
                base.Image = value ? onImage : offImage;
                OnStateChanged();
            }
        }


        protected virtual void OnTryToggle(CancelEventArgs e) {
            if (TryToggle != null)
                TryToggle(this, e);
        }
        public event CancelEventHandler TryToggle;

        protected virtual void OnStateChanged() {
            if (!(StateChanged == null))
                StateChanged(this, new EventArgs());
        }
        public event EventHandler StateChanged;

        protected virtual void OnToggled() {
            if (!(Toggled == null))
                Toggled(this, new EventArgs());
        }
        public event EventHandler Toggled;

    }
}