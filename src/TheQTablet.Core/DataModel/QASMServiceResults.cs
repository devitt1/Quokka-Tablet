using System;
using System.Collections.Generic;

namespace TheQTablet.Core.DataModel
{
 
    public class QASMServiceResults
    {
        public string Error { set; get; }
        public List<bool> Results { set; get; }

        public QASMServiceResults(string error = "")
        {
            Error = error;
            Results = new List<bool>();
        }
    }


    public enum ApiType {
        CLASSIC_API,
        BATCH_API,
        QASM_API
    }

    public class AngleResult
    {
        public int Angle { set; get; }
        public bool Value { set; get; }
    }
}
