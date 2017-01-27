using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
	class DoubleBufferedPanel: Panel
	{
		public DoubleBufferedPanel() {
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}
	}
}
