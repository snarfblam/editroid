using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid.Actions
{
    class AddOrRemoveObject : ScreenAction
    {
        readonly ObjOperation operation;
        readonly ObjectInstance obj;
        int insertIndex = -1;

        private AddOrRemoveObject(EditroidUndoRedoQueue q, Screen screen, ObjOperation operation, ObjectInstance data)
            : base(q, screen) {
            this.operation = operation;
            this.obj = data;
        }

        public static AddOrRemoveObject AddObject(EditroidUndoRedoQueue q, Screen screen, ObjectInstance obj) {
            return new AddOrRemoveObject(q, screen, ObjOperation.Add, obj);
        }

        public static AddOrRemoveObject RemoveObject(EditroidUndoRedoQueue q, Screen screen, ObjectInstance obj) {
            return new AddOrRemoveObject(q, screen, ObjOperation.Remove, obj);
        }

        public override void Do() {
            Perform(operation);
        }
        public override void Undo() {
            if (operation == ObjOperation.Add)
                Perform(ObjOperation.Remove);
            else if (operation == ObjOperation.Remove)
                Perform(ObjOperation.Add);
        }


        private void Perform(ObjOperation operation) {
            if (operation == ObjOperation.Add) {
                PerformAdd();
            } else if (operation == ObjOperation.Remove) {
                PerformRemove();
            }
        }

        private void PerformRemove() {
            if (obj is EnemyInstance) {
                insertIndex = Screen.Enemies.IndexOf((EnemyInstance)obj);
                Screen.DeleteEnemy((EnemyInstance)obj);
            } else if (obj is StructInstance) {
                insertIndex = Screen.Structs.IndexOf((StructInstance)obj);
                Screen.DeleteObject((StructInstance)obj);
            }
        }

        private void PerformAdd() {
            if (obj is EnemyInstance) {
                if (insertIndex == -1)
                    insertIndex = Screen.Enemies.Count;

                Screen.AddEnemy((EnemyInstance)obj, insertIndex);
            } else if (obj is StructInstance) {
                if (insertIndex == -1)
                    insertIndex = Screen.Structs.Count;

                Screen.AddObject((StructInstance)obj, insertIndex);
            }
        }


        public override string GetText() {
            if (operation == ObjOperation.Add) {
                if (obj is EnemyInstance) {
                    return "Add enemy";
                } else if (obj is StructInstance) {
                    return "Add object";
                }
            } else if (operation == ObjOperation.Remove) {
                if (obj is EnemyInstance) {
                    return "Delete enemy";
                } else if (obj is StructInstance) {
                    return "Delete object";
                }

            }

            return "Unknown operation";
        }
        [Flags]
        private enum ObjOperation
        {
            Add,
            Remove
        }
    }


}
