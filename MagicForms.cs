using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
    static class MagicForms
    {
        static frmHelp _helpForm;
        static public frmHelp HelpForm {
            get {
                if (_helpForm == null || _helpForm.IsDisposed)
                    _helpForm = new frmHelp();

                return _helpForm;
            }
        }

        static IpsForm ipsForm;
        static public IpsForm IpsForm {
            get {
                if (ipsForm == null || ipsForm.IsDisposed)
                    ipsForm = new IpsForm();

                return ipsForm;
            }
        }
    }
}
