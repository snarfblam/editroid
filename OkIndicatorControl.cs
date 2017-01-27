using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Editroid
{
    class OkIndicatorControl: PictureBox
    {

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Image Image {
            get { return base.Image; }
            set { base.Image = value; }
        }


        private Image okImage;

        public Image OkImage {
            get { return okImage; }
            set {
                okImage = value;
                if (state)
                    base.Image = value;
            }
        }
        
        private Image invalidImage;
        public Image InvalidImage {
            get { return invalidImage; }
            set { 
                invalidImage = value;
                if (!state)
                    base.Image = value;

            }
        }

        private bool state;

        public bool State {
            get { return state; }
            set { 
                state = value;
                base.Image = state ?
                    okImage : invalidImage;
            }
        }

    }
}
