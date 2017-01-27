using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;

namespace Editroid.UndoRedo
{
    abstract class ScreenItemAction:ScreenAction 
    {
        /// <summary>Stores object instance index. This is preferred because an ObjectInstance can become invalid, but can not be done for items.</summary>
        int index;

        ////Editroid.ROM.ItemInstance item;
        ////ObjectInstanceType objType;
        ObjectInstance _obj;

        ////protected bool IsEnemy { get { return objType == ObjectInstanceType.Enemy; } }
        ////protected ObjectInstanceType ObjectType { get { return objType; } }
        

        public ScreenItemAction(EditroidUndoRedoQueue q, Screen screen, ObjectInstance obj)
            : base(q, screen) {
            ////objType = obj.InstanceType;
            ////if (objType == ObjectInstanceType.Item) {
            ////    item = obj.Item;
            ////} else {
            ////    index = Queue.Rom.GetLevel(level).Screens[screen].GetIndex(obj);
            ////}
            _obj = obj;
        }

        public ObjectInstance GetItem() {
            ////if (objType == ObjectInstanceType.Item)
            ////    return new ObjectInstance(item);
            ////else if (objType == ObjectInstanceType.Enemy)
            ////    return new ObjectInstance(Screen.Enemies[index]);
            ////else
            ////    return new ObjectInstance(Screen.Objects[index]);
            return _obj;
        }

        public override void Do() {
            Do(GetItem());
        }

        protected virtual void Do(ObjectInstance screenItem) {
        }

        public override void Undo() {
            Undo(GetItem());
        }

        protected virtual void Undo(ObjectInstance screenItem) {
            
        }


        public bool CompareItemReference(ScreenItemAction a) {
            //return a.objType == objType && a.index == index && a.AffectedLevel == AffectedLevel && a.AffectedScreenIndex == AffectedScreenIndex;
            return a._obj == _obj;
        }
    }
}
