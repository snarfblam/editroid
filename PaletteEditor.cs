using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
	/// <summary>
	/// 
	/// </summary>
#if DEBUG
	public partial class PaletteEditor:UserControl
#else
	public partial class PaletteEditor:Panel
#endif
	{
		/// <summary>
		/// Instantiates this control.
		/// </summary>
		public PaletteEditor() {
			InitializeComponent();

			table0.PaletteEntrySelected += new PalTableEditor.PaletteSelectionEvent(OnEntrySelected);
			table1.PaletteEntrySelected += new PalTableEditor.PaletteSelectionEvent(OnEntrySelected);
			table2.PaletteEntrySelected += new PalTableEditor.PaletteSelectionEvent(OnEntrySelected);
			table3.PaletteEntrySelected += new PalTableEditor.PaletteSelectionEvent(OnEntrySelected);
		}

		private byte[] _Data;

		/// <summary>
		/// Gets/sets the ROM image data to edit.
		/// </summary>
		public byte[] Data {
			get { return _Data; }
			set { 
				_Data = value;
				table0.Data =
				table1.Data =
				table2.Data =
				table3.Data =
				value;
			}
		}

		private int _Offset;

		/// <summary>
		/// Gets/sets the base offset of paletteIndex data.
		/// </summary>
		public int Offset {
			get { return _Offset; }
			set { 
				_Offset = value;
				table0.Offset = value;
				table1.Offset = value + 4;
				table2.Offset = value + 8;
				table3.Offset = value + 12;
			}
		}
	

		void OnEntrySelected(object sender, PaletteSelectionInfo e) {
			if(EntrySelected != null) EntrySelected(this, e);
		}

		/// <summary>
		/// Raised when an entry is selected to be edited.
		/// </summary>
		public event PalTableEditor.PaletteSelectionEvent EntrySelected;

        internal void MakeDefaultSelection() {
            table0.MakeDefaultSelection();
        }
    }
}
