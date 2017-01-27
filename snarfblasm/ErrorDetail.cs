using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snarfblasm
{
    public class ErrorDetail
    {

        public ErrorDetail(int line, string file, ErrorCode code, string message) {
            this.LineNumber = line;
            this.File = file;
            this.Code = code;
            this.Message = message;
        }

        public int LineNumber { get; private set; }
        public string File { get; private set; }
        public ErrorCode  Code { get; private set; }
        public string Message { get; private set; }
    }
}
