using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.Properties;
using System.IO;

namespace Editroid
{
    internal partial class frmHelp:Form
    {
        FeederBacker feedbackAgent = new FeederBacker();

        public frmHelp() {
            InitializeComponent();

            gsPath = GetHelpFilePath("GettingStarted.html");
            GettingStartedHelp.Navigate(gsPath);

            tabs.TabPages.Remove(FeedbackTab);
            tabs.TabPages.Remove(tabGettingStarted);
            tabs.TabPages.Remove(tabHelp);

            ////rtfAbout.Rtf = Resources.tos;
            HelpTree.ExpandAll();
        }

        public void ShowAbout() {
            Show();
            BringToFront();
            tabs.SelectedTab = tabAbout;
        }

        string helpPath = Path.Combine(Application.StartupPath, "Help");
        string gsPath;

        string GetHelpFilePath(string filename) {
            return Path.Combine(helpPath, filename ?? "(unknown)");
        }
        public void ShowGettingStarted() {
            Show();
            BringToFront();
            tabs.SelectedTab = tabGettingStarted;
        }

        public void ShowHelp() {
            Show();
            BringToFront();
            tabs.SelectedTab = tabHelp;
        }

        internal void ShowFeedback() {
            Show();
            BringToFront();
            tabs.SelectedTab = FeedbackTab;
        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e) {
            if(tabs.SelectedTab == tabGettingStarted) {
                if(GettingStartedHelp.Url == null || !GettingStartedHelp.Url.Equals(gsPath))
                    GettingStartedHelp.Navigate(gsPath);
            }
        }

        private void HelpTree_AfterSelect(object sender, TreeViewEventArgs e) {
            string fileName = e.Node.Tag as string;
            if(fileName == "root") {
                helpBrowser.Navigate("http://ilab.ahemm.org/editroid/helpStart");
                return;
            }
            if(e.Node.Nodes.Count > 0) return; // If this is a folder, never mind.

            if(fileName == null || !File.Exists(GetHelpFilePath(fileName))) {
                helpBrowser.DocumentText = "<html><body><font face='trebuchet ms'>Help file <b>" + fileName + "</b> not found in <b>" + helpPath + "</b>.</font></body></html>";
            }else{
                helpBrowser.Navigate(GetHelpFilePath(fileName));
            }
        }



        #region Feedback communication

        private void btnSubmit_Click(object sender, EventArgs e) {
            ////feedbackAgent.EmailAdress = txtEmail.Text;
            //////chkEmailReply.Checked ?
            //////txtEmail :
            //////"[No otherRow-mail reply]";
            ////feedbackAgent.FeedbackType = radBug.Checked ?
            ////    "Bug Report" :
            ////    radComment.Checked ?
            ////        "Comment" :
            ////        radQuestion.Checked ?
            ////            "Question" :
            ////            "Suggestion";
            ////feedbackAgent.Message = txtFeedback.Text;
            ////feedbackAgent.FeedbackUrl = frmMain.FeedbackUrl;
            ////feedbackAgent.RequestReply = chkEmailReply.Checked;

            ////if(chkEmailReply.Checked)
            ////    feedbackAgent.Message += Environment.NewLine + "[This message was tagged for follup-up.]";

            ////FeedbackProgress.Enabled = FeedbackProgress.Visible = true;
            ////btnSubmit.Enabled = btnSubmit.Visible = false;

            ////txtEmail.Text = "";
            ////txtFeedback.Text = "Connecting to server...";
            ////chkEmailReply.Checked = false;

            ////txtEmail.Enabled = txtFeedback.Enabled = chkEmailReply.Enabled = false;
            ////FeedbackWorker.RunWorkerAsync();
        }

        private void FeedbackWorker_DoWork(object sender, DoWorkEventArgs e) {
            // Set up default values for feedback results
            Feedback_CurrentVersion = new Version(0, 0);
            Feedback_Status = "Unknown";
            Feedback_Message = "The server did not send a message.";
            Feedback_FollowupStatus = "Unknown - The server did not provide follow-up information.";

            try {
                // Perform actual feedback transaction
                feedbackAgent.SendFeedback();

                // Parse values for feedback results
                string[] result = feedbackAgent.ResponseResult.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string resultString in result) {
                    ProcessFeedbackResult(resultString);
                }
            }
            catch(Exception ex) {
                // Setup error values for feedback results
                Feedback_CurrentVersion = new Version(0, 0);
                Feedback_Status = "Error - Failed";
                Feedback_Message = "Feedback operation failed - " + ex.GetType().ToString() +
                    "\r\nMessage: " + ex.Message;
                Feedback_FollowupStatus = "Unavailable - Error";
            }
        }

        private void ProcessFeedbackResult(string resultString) {
            try {
                if(resultString.StartsWith("Status:", StringComparison.InvariantCultureIgnoreCase)) {
                    Feedback_Status = resultString.Substring(7);
                } else if(resultString.StartsWith("Message:", StringComparison.InvariantCultureIgnoreCase)) {
                    Feedback_Message = resultString.Substring(8);
                } else if(resultString.StartsWith("CurrentVersion:", StringComparison.InvariantCultureIgnoreCase)) {
                    Feedback_CurrentVersion = new Version(resultString.Substring(15));
                } else if(resultString.StartsWith("FollowupAvailable:", StringComparison.InvariantCultureIgnoreCase)) {
                    Feedback_FollowupStatus = resultString.Substring(18);
                }
            }
            catch {
            }
        }

        Version Feedback_CurrentVersion;
        String Feedback_Status;
        String Feedback_Message;
        string Feedback_FollowupStatus;
        const string homepage = "http://ilab.ahemm.org/editroid";

        private void FeedbackWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            string version = ConstructVersionString();

            if(!this.IsDisposed) {
                txtEmail.Enabled = txtFeedback.Enabled = chkEmailReply.Enabled = true;
                txtFeedback.Text = "";
                FeedbackProgress.Enabled = FeedbackProgress.Visible = false;
                btnSubmit.Enabled = btnSubmit.Visible = true;
            }

            MessageBox.Show(
                Feedback_Message + Environment.NewLine + Environment.NewLine + "Follow-up: " + Feedback_FollowupStatus + version,
                "Feedback Status: " + Feedback_Status);

        }


        /// <summary>After the feedback agent has run, this method constructs a string to explain to the user what information is
        /// available about the newest version of Editroid.</summary>
        private string ConstructVersionString() {
            // Default message is that info is not available
            string VersionString = Environment.NewLine + Environment.NewLine +
                "Information on the most current version could not be found. Check " + homepage + " for the most current version.";

            // If we do have info...
            if(Feedback_CurrentVersion != null) {
                // Show user most current version
                VersionString = Environment.NewLine + Environment.NewLine +
                    "Most current version: " + Feedback_CurrentVersion.ToString(2);

                // And let him know if he is up to date
                if(Feedback_CurrentVersion.CompareTo(new Version(Application.ProductVersion)) > 0)
                    VersionString += Environment.NewLine + "A new version of Editroid is available. Visit " + homepage + " for downloads.";
                else
                    VersionString += Environment.NewLine + "This software is up to date.";
            }

            return VersionString;
        }
        #endregion

        bool BugMessageShown = false;
        private void radBug_CheckedChanged(object sender, EventArgs e) {
            if(radBug.Checked && !BugMessageShown) {
                BugMessageShown = true;

                MessageBox.Show(
                    "Bug reports are always appreciated. Even if you just report a teensy-weensy thing. When sending a bug report, please specify as much of the following as possible:" + Environment.NewLine +
                    "• Symptom: What is happening that shouldn't?" + Environment.NewLine + 
                    "• Cause: What was done that resulted in the symptom, or better yet, how can the symptom be reproduced?" + Environment.NewLine + 
                    "• ROM version (if known): You know those \"tags\" at the end of the rom name (like [U] and [o1]). Yeah, those." + Environment.NewLine + 
                    Environment.NewLine +
                    "The most recent version of Editroid can be found at " + homepage + ". You will also be informed of the most recent version when you submit feedback.",
                    "Bug Reports");

            }
        }






    }
}
