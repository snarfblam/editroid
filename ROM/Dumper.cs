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
                    areaJson["chr"] = GetChrJson(areaData);
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
                    areaJson["structures"] = GetStructureJson(areaData);
                    break;
                case DumpInsertType.Screen:
                    areaJson["screens"] = GetScreenJson(areaData);
                    break;
                case DumpInsertType.AltMusic:
                    areaJson["altMusic"] = getAltMusicJson(areaData);
                    break;
                case DumpInsertType.Asm:
                    areaJson["screenLoadRoutines"] = GetScreenloadJson(areaData);
                    break;
                case DumpInsertType.TilePhysics:
                    if (rom.Format.SupportsCustomTilePhysics) {
                        areaJson["tilePhysics"] = GetTilePhysicsJson(areaData);
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid DumpInsertType for level data: " + dataType.ToString());
            }
        }

        private static JsonObject GetChrJson(Level areaData) {
            var chrJson = new JsonObject();
            var chrData = areaData.Format.GetRawPatterns();

            // Convert blobs to base-64
            var blobList = new List<string>();
            var bgListJson = new List<JsonObject>();

            foreach (var blob in chrData.Blobs) blobList.Add(Convert.ToBase64String(blob));
            foreach (var bgEntry in chrData.BgBlobs) {
                var bgEntryJson = new JsonObject();
                bgEntryJson["blob"] = bgEntry.BlobNumber;
                if (bgEntry.Frame != null) bgEntryJson["frame"] = bgEntry.Frame.Value;
                if (bgEntry.Section != null) bgEntryJson["section"] = bgEntry.Section.Value;

                bgListJson.Add(bgEntryJson);
            }
            chrJson["blobs"] = blobList;
            chrJson["bg"] = bgListJson;
            chrJson["sprBlob"] = chrData.SprBlob;
            chrJson["sprAltBlob"] = chrData.SprAltBlob;
            if (chrData.Spr2Blob != null) {
                chrJson["spr2Blob"] = chrData.Spr2Blob;
            }
            return chrJson;
        }

        private string GetTilePhysicsJson(Level areaData) {
            int offset = (int)areaData.TilePhysicsTableLocation;
            byte[] data = new byte[256];
            Array.Copy(rom.data, offset, data, 0, data.Length);
            string b64 = Convert.ToBase64String(data);
            return b64;
        }

        private static List<string> GetScreenloadJson(Level areaData) {
            var screenLoadJson = new List<string>();
            for (int i = 0; i < areaData.Screens.Count; i++) {
                screenLoadJson.Add(areaData.Screens[i].ScreenLoadASM);
            }
            return screenLoadJson; 
        }

        private static List<int> getAltMusicJson(Level areaData) {
            var altMusicJson = new List<int>();

            var count = areaData.AlternateMusicRooms.UsedEntryCount;
            for (int i = 0; i < count; i++) {
                altMusicJson.Add(areaData.AlternateMusicRooms[i]);
            }
            return altMusicJson;
        }

        private static List<JsonObject> GetScreenJson(Level areaData) {
            var screenListJson = new List<JsonObject>();
            for (int iScreen = 0; iScreen < areaData.Screens.Count; iScreen++) {
                var screenData = areaData.Screens[iScreen];
                var screenJson = new JsonObject();
                var doorListJson = new List<JsonObject>();
                var enemyListJson = new List<JsonObject>();
                var structListJson = new List<JsonObject>();

                foreach (var door in screenData.Doors) {
                    var doorJson = new JsonObject();
                    doorJson["side"] = door.Side.ToString();
                    doorJson["type"] = door.Type.ToString();
                    doorListJson.Add(doorJson);
                }
                foreach (var enemy in screenData.Enemies) {
                    var enemyJson = new JsonObject();
                    enemyJson["compositeLocation"] = enemy.CompositeLocation;
                    enemyJson["difficult"] = enemy.Difficult;
                    enemyJson["type"] = enemy.EnemyType;
                    enemyJson["boss"] = enemy.IsLevelBoss;
                    enemyJson["respawn"] = enemy.Respawn;
                    enemyJson["spriteSlot"] = enemy.SpriteSlot;
                    enemyListJson.Add(enemyJson);
                }
                foreach (var structure in screenData.Structs) {
                    var structJson = new JsonObject();
                    structJson["compositeLocation"] = structure.CompositeLocation;
                    structJson["type"] = structure.ObjectType;
                    structJson["palette"] = structure.PalData;
                    structListJson.Add(structJson);
                }

                screenJson["hasBridge"] = screenData.HasBridge;
                screenJson["colorAttribute"] = screenData.ColorAttributeTable;
                screenJson["doors"] = doorListJson;
                screenJson["enemies"] = enemyListJson;
                screenJson["structures"] = structListJson;

                screenListJson.Add(screenJson);
            }
            return screenListJson;
        }

        private static List<List<JsonObject>> GetStructureJson(Level areaData) {
            var structListJson = new List<List<JsonObject>>(); // listception
            for (int iStruct = 0; iStruct < areaData.Structures.Count; iStruct++) {
                var structData = areaData.Structures[iStruct];
                var structJson = new List<JsonObject>();

                for (int iRow = 0; iRow < structData.RowCount; iRow++) {
                    var rowJson = new JsonObject();

                    int offsetX = structData.Data.GetFirstTileX(iRow);
                    int size = structData.Data.GetLastTileX(iRow) - offsetX + 1;
                    byte[] tiles = new byte[size];
                    for (int iTile = 0; iTile < size; iTile++) {
                        tiles[iTile] = structData.Data[offsetX + iTile, iRow];
                    }

                    rowJson["offsetX"] = offsetX;
                    rowJson["data"] = Convert.ToBase64String(tiles);
                    structJson.Add(rowJson);
                }

                structListJson.Add(structJson);
            }
            return structListJson;
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
                animListJson.Add(animJson);
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
    }

    
}
