using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;
using Editroid.UndoRedo;

namespace Editroid.Actions
{
    class PasswordDataAction:EditroidAction
    {
        private int dataIndex;
        /// <summary>
        /// Gets the index of the data item this action operates on.
        /// </summary>
        public int DataIndex {
            get { return dataIndex; }
        }

        int oldX, oldY;
        int oldValue;

        private int newX;
        public int NewX {
            get { return newX; }
            set { newX = value; }
        }

        private int newY;
        public int NewY {
            get { return newY; }
            set { newY = value; }
        }

        private int newValue;
        public int NewValue {
            get { return newValue; }
            set { newValue = value; }
        }

        public PasswordDataAction(EditroidUndoRedoQueue q, int dataIndex) 
        :base(q) {
            this.dataIndex = dataIndex;
            PasswordDatum data = GetData();

            oldX = newX = data.MapX;
            oldY = newY = data.MapY;
            oldValue = newValue = data.Item;
        }

        PasswordDatum GetData() {
            return Queue.Rom.PasswordData.GetDatum(dataIndex);
        }


        public override void Do() {
            PasswordDatum data = GetData();

            data.MapX = newX;
            data.MapY = newY;
            data.Item = newValue;

        }

        public override void Undo() {
            PasswordDatum data = GetData();

            data.MapX = oldX;
            data.MapY = oldY;
            data.Item = oldValue;
        }

        public override string GetText() {
            return "Modify password data";
        }

        public override bool TryCombine(EditroidAction newerAction) {
            PasswordDataAction a = newerAction as PasswordDataAction;
            if (a == null || a.dataIndex != dataIndex) return false;

            newX = a.newX;
            newY = a.newY;
            newValue = a.newValue;
            return true;
        }
    }
}
