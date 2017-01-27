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
	/// Control to edit a four-color paletteIndex table
	/// </summary>
#if DEBUG
	public partial class PalTableEditor:UserControl
#else
	public partial class PalTableEditor:PictureBox
#endif
	{
        Bitmap selectionImage = Editroid.Properties.Resources.PaletteSelectOverlay;
        Bitmap normalImage = Editroid.Properties.Resources.PaletteOverlay;

		Color[] _Palette = NesPalette.NesColors;
		/// <summary>
		/// Gets/sets the paletteIndex to be used for this paletteIndex editor.
		/// </summary>
		/// <remarks>This value defaults to the normal NES color paletteIndex. This may be changed
		/// to reflect de-emphasized NES color, but any modifications to this paletteIndex
		/// will not be reflected in the color selector at the bottom.</remarks>
		public Color[] Palette {
			get {
				return _Palette;
			}
			set {
				_Palette = value;
			}
		}
		private bool ShouldSerializePalette() { return false; }

		private byte[] _Data;

		/// <summary>
		/// Gets/sets the ROM image data to edit.
		/// </summary>
		public byte[] Data {
			get { return _Data; }
			set { _Data = value; ShowColors(); }
		}
		private bool ShouldSerializeData() { return false; }

		private int _Offset;

		/// <summary>
		/// Gets/sets the offset of the paletteIndex data to edit.
		/// </summary>
		public int Offset {
			get { return _Offset; }
			set { _Offset = value; ShowColors(); }
		}
		private bool ShouldSerializeOffset() { return false; }

		private void ShowColors() {
			SetEntry(0, GetEntry(0));
			SetEntry(1, GetEntry(1));
			SetEntry(2, GetEntry(2));
			SetEntry(3, GetEntry(3));
		}

		/// <summary>
		/// Returns the paletteIndex entry at the given count, or zero if no ROM image is specified for this control.
		/// </summary>
		/// <param name="count">The count of the entry</param>
		/// <returns>The value of the entry</returns>
		public int GetEntry(int index) {
			if(index < 0 || index > 3) throw new ArgumentException("Invalid index", "index");
			if(_Data == null) return 0;

			return _Data[_Offset + index];
		}

		/// <summary>
		/// Sets the paletteIndex entry at the given count.
		/// </summary>
		/// <param name="count">Index of the entry.</param>
		/// <param name="entry">New value of the entry.</param>
		/// <remarks>This function will fail of no ROM image is provided to the Data property, but no exception will be raised.</remarks>
		public void SetEntry(int index, int entry) {
			if(Data == null) return;
			if(index < 0 || index > 3) throw new ArgumentException("Invalid index", "index");
			Data[_Offset + index] = (byte)(entry & 255);
			switch(index) {
				case 0:
					pal0.BackColor = _Palette[entry];
					break;
				case 1:
					pal1.BackColor = _Palette[entry];
					break;
				case 2:
					pal2.BackColor = _Palette[entry];
					break;
				case 3:
					pal3.BackColor = _Palette[entry];
					break;
			}
		}
		/// <summary>
		/// Instantiates this control.
		/// </summary>
		public PalTableEditor() {
			InitializeComponent();

			pal0.Click += new EventHandler(OnChildClick);
			pal1.Click += new EventHandler(OnChildClick);
			pal2.Click += new EventHandler(OnChildClick);
			pal3.Click += new EventHandler(OnChildClick);
		}

		private void OnChildClick(Object sender, EventArgs e) {
            PictureBox s = sender as PictureBox;

            // Don't reselect pal entry
            if(s.Image == selectionImage) return;

			s.Image = selectionImage;
			if(PaletteEntrySelected != null) {
				PaletteSelectionInfo i = new PaletteSelectionInfo(
					s,
					this,
					s == pal0 ?
						0 :
					s == pal1 ?
						1 :
					s == pal2 ?
						2 : 3,
					(ModifierKeys == Keys.Shift)
				);
				PaletteEntrySelected(this, i);
			}

		}

		/// <summary>
		/// Defines the event handler for a paletteIndex entry selection.
		/// </summary>
		/// <param name="sender">The control that raises the exception.</param>
		/// <param name="otherRow">Data about the selection.</param>
		public delegate void PaletteSelectionEvent(object sender, PaletteSelectionInfo e);
		/// <summary>
		/// Raised when the user selectes a paletteIndex entry to modify.
		/// </summary>
		public event PaletteSelectionEvent PaletteEntrySelected;

        internal void MakeDefaultSelection() {
            OnChildClick(pal0, new EventArgs());
        }

    }

	/// <summary>
	/// Contains data about a paletteIndex selection for the paletteIndex editor controls.
	/// </summary>
	public class PaletteSelectionInfo
	{
		private PictureBox _Control;
		private PalTableEditor _Host;
		private int _EntryIndex;
		private bool _AddToSelection;

		internal PaletteSelectionInfo(PictureBox control, PalTableEditor host, int entryIndex, bool addToSelection) {
			_Control = control;
			_Host = host;
			_EntryIndex = entryIndex;
			_AddToSelection = addToSelection;
		}

		/// <summary>
		/// The control that represents the entry this object refers to.
		/// </summary>
		public PictureBox Control {
			get { return _Control; }
		}
		/// <summary>
		/// The PalTableEditor that contians the entry this object refers to.
		/// </summary>
		public PalTableEditor Host {
			get { return _Host; }
		}
		/// <summary>
		/// The count within a paletteIndex table of the entry this object refers to.
		/// </summary>
		public int EntryIndex{
			get { return _EntryIndex; }
		}

		/// <summary>
		/// Gets or sets the value of the entry this object refers to.
		/// </summary>
		public int EntryValue {
			get {
				return Host.GetEntry(_EntryIndex);
			}
			set {
				Host.SetEntry(_EntryIndex, value);
			}
		}

		/// <summary>
		/// If true, this entry should be added to the current selection.
		/// If false, this entry should replace the entire selection.
		/// </summary>
		public bool AddToSelection {
			get { return _AddToSelection; }
		}
		/// <summary>
		/// Deselects the paletteIndex entry this object refers to.
		/// </summary>
		public void Deselect() {
			_Control.Image = Editroid.Properties.Resources.PaletteOverlay;
		}
	}
}
