using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Editroid
{
    class NumericTextBox:TextBox
    {
        int cachedValue = 0;

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);

            SelectionStart = 0;
            SelectionLength = Text.Length;
            CharacterCasing = CharacterCasing.Upper;
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);

            int value = min - 1;
            bool parsed;
            if (hex)
                parsed = int.TryParse(Text, System.Globalization.NumberStyles.HexNumber, null, out value);
            else
                parsed = int.TryParse(Text, out value);

            if (parsed) {
                value = CheckRange(value);

                cachedValue = value;
            }

            Text = Format(cachedValue);
        }

        private int CheckRange(int value) {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        string Format(int number) {
            if (hex)
                return number.ToString("X");
            return number.ToString();
        }

        [Browsable(false)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        public int Value {
            get {
                int value = min - 1;
                bool parsed;
                if (hex)
                    parsed = int.TryParse(Text, System.Globalization.NumberStyles.HexNumber, null, out value);
                else
                    parsed = int.TryParse(Text, out value);

                if (parsed) return value;
                return cachedValue;
            }
            set { 
                string newText = Format(CheckRange(value));
                if (newText != Text) Text = newText; 
            }
        }


        [DefaultValue(0)]
        public int Minimum {
            get { return min; }
            set { min = value; }
        }
        private int min;


        [DefaultValue(10)]
        public int Maximum {
            get { return max; }
            set { max = value; }
        }
        private int max;

        [DefaultValue(false)]
        public bool Hexadecimal {
            get { return hex; }
            set { hex = value; }
        }
        private bool hex;

    }
}
