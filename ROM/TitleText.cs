using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    public class TitleText:IRomDataParentObject
    {
        /// <summary>ROM offset of the "PUSH START BUTTON" text.</summary>
        public const int Line1_Offset = 0x514;
        /// <summary>ROM offset of copyright line in title screen.</summary>
        public const int Line2_Offset = 0x52B;
        /// <summary>Length of the "PUSH START BUTTON" text.</summary>
        public const int Line1_Length = 17;
        /// <summary>Length of the copyright line in the title screen.</summary>
        public const int Line2_Length = 15;


        MetroidRom rom;

        public TitleText(MetroidRom rom) {
            this.rom = rom;
        }
        /// <summary>
        /// Gets or sets the text displayed below "Metroid" on the title screen.
        /// </summary>
        public String Line1 {
            get {
                return rom.GetRomText(Line1_Offset, Line1_Length);
            }
            set {
                if (value == null) value = string.Empty;
                if (value.Length > Line1_Length)
                    value = value.Substring(0, Line1_Length);

                rom.SetRomText(Line1_Offset, value.PadRight(Line1_Length, ' '));
            }
        }
        /// <summary>
        /// Gets or sets the copyright line on the title screen.
        /// </summary>
        public String Line2 {
            get {
                return rom.GetRomText(Line2_Offset, Line2_Length);
            }
            set {
                if (value == null) value = string.Empty;
                if (value.Length > Line2_Length)
                    value = value.Substring(0, Line2_Length);

                rom.SetRomText(Line2_Offset, value.PadRight(Line2_Length, ' '));
            }
        }


        #region IRomDataParentObject Members

        int IRomDataObject.Offset { get { return 0; } }
        int IRomDataObject.Size { get { return 0; } }

        bool IRomDataObject.HasListItems { get { return true; } }
        bool IRomDataObject.HasSubItems { get { return false; } }

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return RomDataObjects.EmptyNode;
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            return new LineDisplayItem[]{
                new LineDisplayItem("Line 1", Line1_Offset, Line1_Length, rom.data),
                new LineDisplayItem("Line 2", Line2_Offset, Line2_Length, rom.data),
            };
        }

        string IRomDataObject.DisplayName { get { return "Title Screen Text"; } }


        #endregion


    }
}
