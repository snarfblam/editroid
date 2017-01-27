using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    class ItemDataDisassembler
    {
        Level level;
        StringBuilder result = new StringBuilder();
        List<ItemRowEntry> rows = new List<ItemRowEntry>();

        const string longByteCode = ".byte";
        const string longWordCode = ".word";
        const string shortByteCode = ".db";
        const string shortWordCode = ".dw";
        static readonly string nl = Environment.NewLine;

        string byteCode;
        string wordCode;

        private ItemDataDisassembler(Level level, DataDirective directive) {
            this.level = level;
            SetDirectives(directive);
            CreateRowList(level);

            int dataOffset = rows[0].Offset;
            var pointer = level.CreatePointer((pRom)dataOffset);

            result.AppendLine("; Offset of item data");
            result.AppendLine(".org " + FormatWord(pointer.Value));

            for (int i = 0; i < rows.Count; i++) {
                ItemRowEntry row = rows[i];
                ItemRowEntry nextRow = GetNextRow(row);

                DisassmRow(row, nextRow);
            }
        }

        private ItemRowEntry GetNextRow(ItemRowEntry row) {
            var nextRowPointer = row.NextEntryPointer;
            if (nextRowPointer.Value == 0xFFFF) return null;

            var nextRowOffset = (int)level.ToPRom(nextRowPointer);
            for (int i = 0; i < rows.Count; i++) {
                if (rows[i].Offset == nextRowOffset)
                    return rows[i];
            }

            throw new ItemDisassmException("Item disassembly error: item row entry's 'next row pointer' did not point to a location within item data.");
        }

        private static string FormatWord(int p) {
            return "$" + p.ToString("x4");
        }
        private static string FormatByte(int p) {
            return "$" + p.ToString("x2");
        }
        private static string Pad16(string s) {
            return s.PadRight(16, ' ');
        }
        /// <summary>
        /// COMMENT SHOULD NOT INCLUDE SEMICOLON
        /// </summary>
        /// <param name="code"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        private static string FormatLine(string code, string comment) {
            if (comment == null) return code;
            return Pad16(code) + "; " + comment;
        }
        private void WriteLine(string code, string comment) {
            result.AppendLine(FormatLine(code, comment));
        }
        private void CreateRowList(Level level) {
            foreach (ItemRowEntry row in level.ItemTable_DEPRECATED) {
                rows.Add(row);
            }

            rows.Sort(CompareRowsByAddress);
        }

        private void SetDirectives(DataDirective directive) {
            switch (directive) {
                case DataDirective.DotDB:
                    byteCode = shortByteCode;
                    wordCode = shortWordCode;
                    break;
                case DataDirective.DotByte:
                    byteCode = longByteCode;
                    wordCode = longWordCode;
                    break;
                default:
                    throw new ArgumentException("Invalid directive type.");
                    break;
            }
        }

        private static string GetRowLabel(int row){
            return "MapY" + row.ToString();
        }
        private string ByteDirective(int value) {
            return byteCode + " " + FormatByte(value);
        }
        private void DisassmRow(ItemRowEntry row, ItemRowEntry nextRow) {
            result.AppendLine("; ------------------------------");
            
            // Label for map row (for previous row to reference)
            result.AppendLine(GetRowLabel(row.MapY) + ":");
            // Byte specifies which row this represents
            result.AppendLine(Pad16(byteCode + " " + FormatByte(row.MapY)));

            result.AppendLine();

            // Word, pointer to nexn row, or FFFF if last row
            if (nextRow == null) { // Last Row
                WriteLine(wordCode + " " + FormatWord(0xFFFF), "Last row of item data");
            } else {
                WriteLine(wordCode + " " + GetRowLabel(nextRow.MapY), "Pointer to next row's data"); 
            }

            var seeker = row.Seek();

            DisassmScreen(seeker);
            while (seeker.MoreScreensPresent) {
                seeker.NextScreen();
                DisassmScreen(seeker);
            }
        }

        private void DisassmScreen(ItemSeeker seeker) {
            result.AppendLine();
            result.AppendLine();

            // Byte, map X
            result.AppendLine(FormatLine(
                byteCode + " " + FormatByte(seeker.MapX),
                "Map X = " + seeker.MapX.ToString()));

            if (seeker.ScreenEntrySizeByte == 0xFF) {
                // Byte indicates last screen
                WriteLine(ByteDirective(0xFF), "Last screen in row");
            } else {
                // Byte, screen data size
                result.AppendLine(FormatLine(
                    byteCode + " " + FormatByte(seeker.ScreenEntrySizeByte),
                    seeker.ScreenEntrySize + " bytes of data for this screen"));
            }
            DisassmItem(seeker);
            while (seeker.MoreItemsPresent) {
                seeker.NextItem();
                DisassmItem(seeker);
            }

            WriteLine(ByteDirective(0), "End of screen data");

        }


        private void DisassmItem(ItemSeeker seeker) {
            result.AppendLine();

            switch (seeker.ItemType) {
                case ItemTypeIndex.Nothing:
                    throw new ItemDisassmException("Encountered an unexpected item type when disassembling an item.");
                case ItemTypeIndex.Enemy:
                    // byte: Enemy code and sprite slot
                    WriteLine(ByteDirective(seeker.ItemTypeByte), 
                              "Enemy, sprite slot = " + seeker.SpriteSlot.ToString() );
                    // byte: hard flag (0x80), enemy type
                    WriteLine(ByteDirective(seeker.SubTypeByte),
                        "Enemy type = " + seeker.EnemyTypeIndex + (seeker.EnemyIsHard ? ", hard enemy" : ""));
                    // Byte: screen position
                    WriteLine(ByteDirective(seeker.ScreenPosition.Value),
                        "Screen X = " + seeker.ScreenPosition.X.ToString() + "  Y = " + seeker.ScreenPosition.Y.ToString());
                    break;
                case ItemTypeIndex.PowerUp:
                    // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Power Up");
                    // byte: Item type
                    WriteLine(ByteDirective(seeker.SubTypeByte),
                        "Type = " + seeker.PowerUpName);
                    // Byte: screen position
                    WriteLine(ByteDirective(seeker.ScreenPosition.Value),
                        "Screen X = " + seeker.ScreenPosition.X.ToString() + "  Y = " + seeker.ScreenPosition.Y.ToString());
                    break;
                case ItemTypeIndex.Mella:
                    // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Mella");
                    break;
                case ItemTypeIndex.Elevator:
                    // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Elevator");
                    // byte: Item type
                    WriteLine(ByteDirective(seeker.SubTypeByte),
                        "Type = " + seeker.Destination.ToString());
                    break;
                case ItemTypeIndex.Turret:
                    // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Turret, type = " + seeker.SpriteSlot.ToString());
                    // Byte: screen position
                    WriteLine(ByteDirective(seeker.ScreenPosition.Value),
                        "Screen X = " + seeker.ScreenPosition.X.ToString() + "  Y = " + seeker.ScreenPosition.Y.ToString());
                    break;
                case ItemTypeIndex.MotherBrain:
                    // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Mother Brain");
                    break;
                case ItemTypeIndex.Zebetite:
                    // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Zebetite, index = " + seeker.SpriteSlot.ToString());
                    break;
                case ItemTypeIndex.Rinkas:
                // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Rinka " + seeker.SpriteSlot.ToString());
                    break;
                case ItemTypeIndex.Door:
                    // byte: Door
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Doop");
                    // byte: Door type
                    var doorType = seeker.SubTypeByte;
                    DoorSide side = (DoorSide)(doorType & 0xF0);
                    DoorType type = (DoorType)(doorType & 0x0F);
                    WriteLine(ByteDirective(seeker.SubTypeByte),
                              "Door: " + side.ToString() + " " + type.ToString());
                    break;
                case ItemTypeIndex.PalSwap:
                    // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Palette Swap");
                    break;
                case ItemTypeIndex.Unused_b:
                case ItemTypeIndex.Unused_c:
                case ItemTypeIndex.Unused_d:
                case ItemTypeIndex.Unused_e:
                case ItemTypeIndex.Unused_f:
                    // byte: Item code
                    WriteLine(ByteDirective(seeker.ItemTypeByte),
                              "Invalid Item Type!");
                    break;
                default:
                    throw new ItemDisassmException("Unexpected item type in DisassmItem");
                    break;
            }

        }

        private int CompareRowsByAddress(ItemRowEntry a, ItemRowEntry b) {
            return a.Offset - b.Offset;
        }
        public static string GetItemDisassembly(Level l, DataDirective directiveType) {
            var disassembler = new ItemDataDisassembler(l, directiveType);
            return disassembler.GetDisassebly();
            
        }

        private string GetDisassebly() {
            return result.ToString();
        }
        public enum DataDirective
        {
            DotDB,
            DotByte
        }
    }

    public class ItemDisassmException : Exception
    {
        public ItemDisassmException(string message) : base(message) { }
    }
}
