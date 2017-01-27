using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid.Actions
{
    class OverwritePasswordData:EditroidAction
    {
        byte[] oldData, newData;
        public OverwritePasswordData(EditroidUndoRedoQueue q, byte[] data) 
        :base(q){
            newData = data;
            oldData = new byte[newData.Length];

            Array.Copy(q.Rom.data, PasswordData.DataOffset, oldData, 0, oldData.Length);
        }


        public override void Do() {
            WriteData(newData);
        }

        private void WriteData(byte[] bytes) {
            Array.Copy(bytes, 0 , Queue.Rom.data, PasswordData.DataOffset, bytes.Length);
        }

        public override void Undo() {
            WriteData(oldData);
        }

        public override string GetText() {
            return "Generate password data";
        }
    }
}
