using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace PharmexTargets.Import
{

    public class ExcelParserException : Exception
    {
        public ExcelParserException(string message) : base(message)
        {
        }

        public ExcelParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
