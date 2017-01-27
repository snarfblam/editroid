using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.ComponentModel;

namespace Editroid
{
    [ToolboxItem(true)]
    internal class FeederBacker: Component
    {
        private string feedbackUrl;

        /// <summary>
        /// Gets/sets the URL for the feedback server.
        /// </summary>
        public string FeedbackUrl {
            get { return feedbackUrl; }
            set { feedbackUrl = value; }
        }

        /// <summary>
        /// Gets the response from the server after feedback.
        /// </summary>
        private string responseResult;
        public string ResponseResult {
            get { return responseResult; }
        }

        private string feedbackType;
        public string FeedbackType {
            get { return feedbackType; }
            set { feedbackType = value; }
        }

        private string message;

        public string Message {
            get { return message; }
            set { message = value; }
        }

        private string email;

        public string EmailAdress {
            get { return email; }
            set { email = value; }
        }

        private bool requestReply;
        /// <summary>
        /// Gets/sets whether the user submitting feedback has requested a
        /// follow-up otherRow-mail.
        /// </summary>
        public bool RequestReply {
            get { return requestReply; }
            set { requestReply = value; }
        }


        /// <summary>Creates POST data and stores it in postBuffer.</summary>
        void CreatePost() {
            if(string.IsNullOrEmpty(feedbackType)) feedbackType = "[no type]";
            if(string.IsNullOrEmpty(email)) email = "[no email]";
            if(string.IsNullOrEmpty(message)) message = "[no message]";
            string appVersion = Application.ProductVersion;

            string post = "&type=" + Uri.EscapeUriString(feedbackType) +
                "&followup=" + (requestReply ? "yes" : "no") +
                "&email=" + Uri.EscapeUriString(email) +
                "&version=" + Uri.EscapeUriString(appVersion) +
                "&message=" + Uri.EscapeUriString(message);

            
            postBuffer = System.Text.Encoding.ASCII.GetBytes(post);
        }
        /// <summary>Frees memory used by postBuffer.</summary>
        void unCreatePost() {
            postBuffer = null;
        }

        byte[] postBuffer;
        
        public void SendFeedback() {
            CreatePost();

            // Create and initialize the request
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(feedbackUrl);
            request.UserAgent = "Editroid Feedback Agent";
            request.Method = "POST";
            
            // Write POST data to request
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBuffer.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBuffer, 0, postBuffer.Length);
            requestStream.Close();
            
            // Execute request and read response.
            HttpWebResponse response = (HttpWebResponse)
                request.GetResponse();
            ReadResponse(response);

            // Free memory
            unCreatePost();
        }

        private void ReadResponse(HttpWebResponse response) {
            // Used to buffer response data
            StringBuilder responseBuffer = new StringBuilder();
            byte[] binaryBuffer = new byte[0x2000];

            // Get response stream
            Stream responseStream = response.GetResponseStream();

            int count = 0;

            do {
                // Read from stream and add the data to the response output.
                count = responseStream.Read(binaryBuffer, 0, binaryBuffer.Length);
                if(count != 0) { // If there is data left to read
                    responseBuffer.Append(
                        Encoding.ASCII.GetString(binaryBuffer, 0, count));
                }
            }
            while(count > 0); // Loop while there is still data

            responseResult = responseBuffer.ToString();
        }
    }
}