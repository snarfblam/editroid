using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Editroid.ROM
{
    class Dumper
    {
        MetroidRom rom;
        IList<DumpInsertItem> dataList;

        JsonObject globalData;
        JsonObject brinstarData;
        JsonObject norfairData;
        JsonObject tourianData;
        JsonObject kraidData;
        JsonObject ridleyData;

        public Dumper(MetroidRom rom, IList<DumpInsertItem> dataList) {
            this.rom = rom;
            this.dataList = dataList;
        }

        public void Dump(TextWriter output) {
            Dump(output, false);
        }
        public void Dump(TextWriter output, bool pretty) {
            globalData = new JsonObject();
            brinstarData = new JsonObject();
            norfairData = new JsonObject();
            tourianData = new JsonObject();
            kraidData = new JsonObject();
            ridleyData = new JsonObject();

            JsonObject root = new JsonObject();
            root.Add("Global", globalData);
            root.Add("Brinstar", brinstarData);
            root.Add("Norfair", norfairData);
            root.Add("Tourian", tourianData);
            root.Add("Ridley", ridleyData);
            root.Add("Kraid", kraidData);

            foreach (var item in dataList) {
                if (item.Area == LevelIndex.None) {
                    DumpGlobalData(item.DataType);
                } else {
                    DumpAreaData(item.Area, item.DataType);
                }
            }

            if (pretty) {
                JsonWriter.Write(output, 4, root);
            } else {
                JsonWriter.Write(output, root);
            }
       }

        private void DumpAreaData(LevelIndex area, DumpInsertType dataType) {
            if (area == LevelIndex.None) throw new InvalidOperationException("Can not dump area data for LevelIndex.None");

            var areaJson = GetAreaData(area);
            var areaData = rom.Levels[area];
            

            switch (dataType) {
                case DumpInsertType.CHR:
                    // Todo: come back to this one
                    break;
                case DumpInsertType.CHRAnimation:
                    if (rom.Format.HasChrUsageTable) {
                        areaJson["chrUsage"] = GetChrUsageJson(area);
                    } else if (rom.Format.HasChrAnimationTable) {
                        areaJson["chrAnimation"] = GetChrAnimationJson(areaData);
                    }
                    break;
                case DumpInsertType.Item:
                    areaJson.Add("items", GetItemsJson(areaData));
                    break;
                case DumpInsertType.Palette:
                    areaJson["palette"] = GetPaletteJson(areaData);
                    break;
                case DumpInsertType.Combo:
                    areaJson["comboData"] = GetCombosAsBase64(areaData);
                    break;
                case DumpInsertType.Structure:
                    break;
                case DumpInsertType.Screen:
                    break;
                case DumpInsertType.AltMusic:
                    break;
                case DumpInsertType.Asm:
                    break;
                case DumpInsertType.TilePhysics:
                    break;
                case DumpInsertType.Password:
                    break;
                default:
                    break;
            }
        }

        private string GetCombosAsBase64(Level areaData) {
            var comboCount = rom.Format.SupportsExtendedComboTable ? 256 : 64;
            var byteCount = comboCount * 4;
            var buffer = new byte[byteCount];
            Array.Copy(rom.data, areaData.Combos.Offset, buffer, 0, byteCount);
            var b64 = Convert.ToBase64String(buffer);
            return b64;
        }

        private static List<JsonObject> GetPaletteJson(Level areaData) {
            var paletteListJson = new List<JsonObject>();
            for (var i = 0; i < areaData.PalettePointers.Count; i++) {
                var palette = areaData.GetPaletteMacro(i);
                var paletteJson = new JsonObject();

                int[] data = new int[palette.MacroSize];
                for (int iByte = 0; iByte < data.Length; iByte++) {
                    data[iByte] = palette.GetMacroByte(iByte);
                }

                paletteJson["ppuDestination"] = (int)palette.PpuDestination;
                paletteJson["data"] = data;
            }
            return paletteListJson;
        }

        private static List<JsonObject> GetItemsJson(Level areaData) {
            var areaItemsJson = new List<JsonObject>();
            foreach (var screenData in areaData.Items) {
                var screenJson = new JsonObject();

                var itemListJson = new List<object>();
                foreach (var itemData in screenData.Items) {
                    // It's easier just to process each item as binary data
                    var itemJson = new int[itemData.Size];
                    var buffer = new byte[itemJson.Length];

                    int offset = 0;
                    itemData.WriteData(buffer, ref offset);

                    // Serializer doesn't like bytes
                    for (int i = 0; i < buffer.Length; i++) {
                        itemJson[i] = buffer[i];
                    }

                    itemListJson.Add(itemJson);
                }

                screenJson["mapX"] = screenData.MapX;
                screenJson["mapY"] = screenData.MapY;
                screenJson["items"] = itemListJson;

                areaItemsJson.Add(screenJson);
            }
            return areaItemsJson;
        }

        private JsonObject GetChrAnimationJson(Level areaData) {
            var areaAnimationsJson = new JsonObject();
            var areaAnimData = areaData.ChrAnimation;

            // Each area has *multiple* animations
            var animListJson = new List<JsonObject>();
            foreach (var anim in areaAnimData.Animations) {
                var animJson = new JsonObject();
                var framesJson = new List<JsonObject>();

                foreach (var frame in anim.Frames) {
                    var singleFrameJson = new JsonObject();
                    singleFrameJson["bank0"] = (int)frame.Bank0;
                    singleFrameJson["bank1"] = (int)frame.Bank1;
                    singleFrameJson["bank2"] = (int)frame.Bank2;
                    singleFrameJson["bank3"] = (int)frame.Bank3;
                    singleFrameJson["frameDataByte"] = frame.FrameDataByte;
                    framesJson.Add(singleFrameJson);
                }

                animJson["name"] = anim.Name;
                animJson["frames"] = framesJson;
            }

            areaAnimationsJson["sprBank0"] = (int)areaAnimData.SprBank0;
            areaAnimationsJson["sprBank1"] = (int)areaAnimData.SprBank1;
            areaAnimationsJson["animations"] = animListJson;
            
            return areaAnimationsJson;
        }

        private JsonObject GetChrUsageJson(LevelIndex area) {
            var animJson = new JsonObject();
            animJson["bgFirstPage"] = rom.ChrUsage.GetBgFirstPage(area);
            animJson["bgLastPage"] = rom.ChrUsage.GetBgLastPage(area);
            animJson["sprPage"] = rom.ChrUsage.GetSprPage(area);
            animJson["animRate"] = rom.ChrUsage.GetAnimRate(area);
            return animJson;
        }

        private void DumpGlobalData(DumpInsertType dumpInsertType) {
        }

        JsonObject GetAreaData(LevelIndex i) {
            switch (i) {
                case LevelIndex.Brinstar:
                    return brinstarData;
                case LevelIndex.Norfair:
                    return norfairData;
                case LevelIndex.Tourian:
                    return tourianData;
                case LevelIndex.Kraid:
                    return kraidData;
                case LevelIndex.Ridley:
                    return ridleyData;
                case LevelIndex.None:
                    return globalData;
                default:
                    throw new ArgumentException("Invalid level index: " + i.ToString());
            }
        }

        string ToHex4(int value) {
            return value.ToString("x").PadLeft(4, '0');
        }

        // Take data list, convert to json object
        // {
        //     "areaName":{
        //         "dataName":{...}
        //     }...
        // }
    }
}
