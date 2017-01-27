using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    class FeedbackComponent
    {
        public void DoFeedBack() {
            System.Net.WebClient x = new System.Net.WebClient();
            byte[] result = x.UploadFile("http://ilab.ahemm.org/upload/", "C:\\test.txt");
            MessageBox.Show(System.Text.Encoding.ASCII.GetString(result));

        }
    }
}
