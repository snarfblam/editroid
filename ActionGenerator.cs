using System;
using System.Collections.Generic;
using System.Text;
using Editroid.Actions;
using System.Drawing;
using Editroid.Graphic;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid
{
    class ActionGenerator
    {
        public ActionGenerator() {
        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { rom = value; }
        }

        #region Screen Actions
        public AddOrRemoveObject AddObject() {
            return AddOrRemoveObject.AddObject(queue,
                CurrentScreen,
                StructInstance.GetNew()
                );
        }
        public AddOrRemoveObject AddObject(int index, byte palette) {
            return AddOrRemoveObject.AddObject(queue,
                CurrentScreen,
                StructInstance.GetNew(index, palette)
                );
        }

        public ModifyCombo ModifyCombo(LevelIndex level, int comboIndex, int tileIndex, int newValue) {
            return new ModifyCombo(queue, rom.Levels[level], comboIndex, tileIndex, newValue);
        }

        public AddOrRemoveObject AddEnemy() {
            var currentScreen = rom.Levels[editor.CurrentLevel].Screens[editor.CurrentScreenIndex];
            int slotIndex = -1;
            bool slotAvailable = false;
            while (slotIndex < 6 && !slotAvailable) {
                slotIndex++;
                slotAvailable = true;
                for (int i = 0; i < currentScreen.Enemies.Count; i++) {
                    if (currentScreen.Enemies[i].SpriteSlot == slotIndex)
                        slotAvailable = false;
                }
            }
            slotIndex = slotIndex % 6; // If 6, set to 0

            //int objTypeByte = (1 | slotIndex << 4) & 0xFF;
            var newEnemy = EnemyInstance.GetNew();
            newEnemy.CompositeLocation = 0x22;
            newEnemy.EnemyType = CurrentLevel.GetDefaultEnemyType();
            newEnemy.SpriteSlot = slotIndex;
            ////return new AddOrRemoveObject(queue,
            ////    editor.CurrentLevel, editor.CurrentScreenIndex, MemoryFunction.AddEnemy, CurrentScreen.Enemies.Count, newEnemy);
            return AddOrRemoveObject.AddObject(queue,
                CurrentScreen,
                newEnemy
                );
        }

        public ChangeColorAttributeTable NextCAT() {
            return new ChangeColorAttributeTable(queue, CurrentScreen, (CurrentScreen.ColorAttributeTable + 1) % 4);
        }

        public EditItemProperty EditItemProperty(Point location, ItemScreenData screen, ItemData item, ItemProperty prop, int newValue) {
            return new EditItemProperty(queue, location, screen, item, prop, newValue);
        }

        public AddOrRemoveBridge AddBridge() {
            return new AddOrRemoveBridge(queue, CurrentScreen, true);
        }

        public AddOrRemoveBridge RemoveBridge() {
            return new AddOrRemoveBridge(queue, CurrentScreen, false);
        }

        public EditroidAction RemoveSelection() {
            if (editor.GetSelectedItem() is DoorInstance) {
                return new ChangeDoor(queue,  CurrentScreen, ((DoorInstance)editor.GetSelectedItem()).Side, DoorType.Invalid);
            } else {
                return AddOrRemoveObject.RemoveObject(queue,
                    CurrentScreen,  
                    editor.GetSelectedItem()
                    );
            }
        }
 
        public ModifyObject ChangePalette() {
            ObjectInstance selection = editor.GetSelectedItem();
            if (selection is StructInstance) {
                return new ModifyObject(queue,  CurrentScreen, selection, (selection.GetPalette() + 1) % 4, ObjectModification.Palette);
            } else if (selection is EnemyInstance) {
                return new ModifyObject(queue,  CurrentScreen, selection, (selection.GetPalette() + 8) % 16, ObjectModification.Palette);
            }
            throw new InvalidOperationException("An attempt was made to edit the selection when there was no selection.");
        }
        public ModifyObject ChangePalette(int index) {
            ObjectInstance selection = editor.GetSelectedItem();
            if (selection is StructInstance) {
                return new ModifyObject(queue,  CurrentScreen, selection, index, ObjectModification.Palette);
            } else if (selection is EnemyInstance) {
                return new ModifyObject(queue,  CurrentScreen, selection, (index == 0) ? 0 : 8, ObjectModification.Palette);
            }
            throw new InvalidOperationException("An attempt was made to edit the selection when there was no selection.");
        }

        public SetScreenIndex PreviousLayout() {
            return new SetScreenIndex(queue, editor.MapLocation, (rom.GetScreenIndex(editor.MapLocation.X, editor.MapLocation.Y) + CurrentLevel.Screens.Count - 1) % CurrentLevel.Screens.Count);
        }
        public SetScreenIndex NextLayout() {
            return new SetScreenIndex(queue, editor.MapLocation, (rom.GetScreenIndex(editor.MapLocation.X, editor.MapLocation.Y) + 1) % CurrentLevel.Screens.Count);
        }
        /// <summary>
        /// Adds the value of diff to the current map location's screen index.
        /// </summary>
        /// <param name="diff"></param>
        /// <returns></returns>
        internal EditroidAction SeekLayout(int diff) {
            int newScreenIndex = rom.GetScreenIndex(editor.MapLocation.X, editor.MapLocation.Y) + diff;
            while (newScreenIndex < 0)
                newScreenIndex += CurrentLevel.Screens.Count;
            newScreenIndex = newScreenIndex % CurrentLevel.Screens.Count;
            return new SetScreenIndex(queue, editor.MapLocation, newScreenIndex);
        }
        internal EditroidAction SetLayout(int index) {
            int newScreenIndex = index;
            newScreenIndex = Math.Min(CurrentLevel.Screens.Count - 1, index);
            return new SetScreenIndex(queue, editor.MapLocation, newScreenIndex);
        }
        public ModifyObject MoveObject(Point newPosition) {
            return new ModifyObject(queue,  CurrentScreen, editor.GetSelectedItem(), newPosition);
        }
        ////public SetItemTilePosition MoveItemTile(Level level, ItemIndex_DEPRECATED itemTile, int newX) {
        ////    return new SetItemTilePosition(queue, level, itemTile, newX, editor.UpdatePassDataForItems);
        ////}
        ////public SetItemRowPosition MoveItemRow(Level level, ItemIndex_DEPRECATED index, int newY) {
        ////    return new SetItemRowPosition(queue, level, index, newY, editor.UpdatePassDataForItems);
        ////}

        public ReorderObjects BringToFront() {
            return new ReorderObjects(queue,  CurrentScreen, editor.GetSelectedItem(), true);
        }

        public ReorderObjects SendToBack() {
            return new ReorderObjects(queue,  CurrentScreen, editor.GetSelectedItem(), false);
        }

        public ModifyObject NextType() {
            ObjectInstance selection = editor.GetSelectedItem();
            int newIndex = selection.GetTypeIndex();

            if (selection is StructInstance)
                newIndex = (newIndex + 1) % CurrentLevel.StructCount;
            else if (selection is EnemyInstance) {
                SpriteDefinition[] levelSprites = LevelSprites.GetSprites(editor.CurrentLevel);
                int enemyTypeCount = levelSprites.Length;

                do {
                    newIndex = (newIndex + 1) % enemyTypeCount;
                } while (levelSprites[newIndex].IsBlank || levelSprites[newIndex].IsInvalid);
            }
            return new ModifyObject(queue,  CurrentScreen, editor.GetSelectedItem(), newIndex, ObjectModification.Type);
        }

        public ModifyObject SetType(int index) {
            return new ModifyObject(queue,  CurrentScreen, editor.GetSelectedItem(), index, ObjectModification.Type);
        }

        public ModifyObject PreviousType() {
            ObjectInstance selection = editor.GetSelectedItem();
            int newIndex = selection.GetTypeIndex();

            if (selection is StructInstance)
                newIndex = (newIndex + CurrentLevel.StructCount - 1) % CurrentLevel.StructCount;
            else if (selection is EnemyInstance) {
                SpriteDefinition[] levelSprites = LevelSprites.GetSprites(editor.CurrentLevel);
                int enemyTypeCount = levelSprites.Length;

                do {
                    newIndex = (newIndex + enemyTypeCount - 1) % enemyTypeCount;
                } while (levelSprites[newIndex].IsBlank || levelSprites[newIndex].IsInvalid);
            }
            return new ModifyObject(queue,  CurrentScreen, editor.GetSelectedItem(), newIndex, ObjectModification.Type);
        }

        public ModifyObject ToggleRespawn() {
            ObjectInstance selection = editor.GetSelectedItem();
            return new ModifyObject(queue,  CurrentScreen, selection, !((EnemyInstance)selection).Respawn, ObjectModification.Respawnable);
        }
        public ModifyObject ToggleIsBoss() {
            ObjectInstance selection = editor.GetSelectedItem();
            return new ModifyObject(queue,  CurrentScreen, selection, !((EnemyInstance)selection).IsLevelBoss, ObjectModification.IsBoss);
        }

        public ModifyObject NextSpriteSlot() {
            ObjectInstance i = editor.GetSelectedItem();
            return new ModifyObject(queue,  CurrentScreen, i, (((EnemyInstance)i).SpriteSlot + 1) % 6, ObjectModification.SpriteSlot);
        }

        public ChangeDoor ChangeDoor(DoorSide side, DoorType type) {
            return new ChangeDoor(queue, CurrentScreen, side, type);
        }

        public ToggleAlternateMusic ToggleAlternateMusic(bool state) {
            return new ToggleAlternateMusic(queue, CurrentLevel, editor.CurrentScreenIndex, state);
        }
        #endregion

        #region Struct Actions

 
        public EditStructTile SetStructTile(int structIndex, int x, int y, int tileValue) {
            return new EditStructTile(queue, rom.Levels[editor.StructureEditorLevel], structIndex, new StructTileEdit(x, y, tileValue));
        }
        public AddTileToStructure AddStructTile(int structIndex, int x, int y, byte tile) {
            return new AddTileToStructure(queue, rom.Levels[editor.StructureEditorLevel], structIndex, x, y, tile);
        }

        //public ShiftStructRow ShiftStructRowLeft(int index, int y) {
        //    return new ShiftStructRow(queue, editor.StructureEditorLevel, index, y, -1);
        //}
        //public ShiftStructRow ShiftStructRowRight(int index, int y) {
        //    return new ShiftStructRow(queue, editor.StructureEditorLevel, index, y, 1);
        //}
        #endregion

        #region Password Actions
        public OverwritePasswordData OverwritePasswordData(byte[] data) {
            return new OverwritePasswordData(queue, data);
        }

        public PasswordDataAction SetPassDataMapLocation(int dataIndex, int mapX, int mapY) {
            PasswordDataAction a = new PasswordDataAction(queue, dataIndex);
            a.NewX = mapX;
            a.NewY = mapY;
            return a;
        }
        public PasswordDataAction SetPassDataItem(int dataIndex, int item) {
            PasswordDataAction a = new PasswordDataAction(queue, dataIndex);
            a.NewValue = item;
            return a;
        }
        #endregion

        public ChangeMapRoomLevel ChangeLevel(LevelIndex level) {
            return new ChangeMapRoomLevel(queue, editor.MapLocation, level);
        }

        public SetPaletteColor SetPaletteColor(Level level, PaletteType type, int pal, int entry, int value) {
            return new SetPaletteColor(queue, level, type, pal, entry, value);
        }

        public AdvancedPaletteEdit SetPalMacroOffset(Level level, int palIndex, pCpu newOffset) {
            return new AdvancedPaletteEdit(queue, level, AdvancedPaletteEditType.DataOffset, palIndex, newOffset.Value);
        }
        public AdvancedPaletteEdit SetPalMacroDest(Level level, int palIndex, pCpu newDest) {
            return new AdvancedPaletteEdit(queue, level, AdvancedPaletteEditType.DataDest, palIndex, newDest.Value);
        }
        public AdvancedPaletteEdit SetPalMacroSize(Level level, int palIndex, int newSize) {
            return new AdvancedPaletteEdit(queue, level, AdvancedPaletteEditType.DataSize, palIndex, newSize);
        }
        public AdvancedPaletteEdit SetPalMacroByte(Level level, int palIndex, int byteIndex, byte value) {
            return new AdvancedPaletteEdit(queue, level, AdvancedPaletteEditType.MacroDataEdit, palIndex, value, byteIndex);
        }


        Level CurrentLevel { get { return rom.GetLevel(editor.CurrentLevel); } }
        Screen CurrentScreen { get { return CurrentLevel.Screens[editor.CurrentScreenIndex]; } }
        EditroidUndoRedoQueue queue { get { return editor.Queue; } }
        
        
        /// <summary>
        /// A very pointless interface to make a pointless abstraction in case
        /// I want to substitute a completely different class that does exactly the same thing.
        /// </summary>
        public interface IParamSource
        {
            LevelIndex CurrentLevel{get;}
            int CurrentScreenIndex{get;}
            ObjectInstance GetSelectedItem();
            EditroidUndoRedoQueue Queue { get;}
            Point MapLocation { get;}
            bool UpdatePassDataForItems { get;}

            LevelIndex StructureEditorLevel { get; }
        }

        private IParamSource editor;
        public IParamSource Editor { get { return editor; } set { editor = value; } }




    }
}
