using System;
using System.Collections.Generic;

namespace TheQTablet.Core.DataModel
{
    public class PolarisationResult
    {
        public string Error { set; get; }
        public bool Result { set; get; }

        public PolarisationResult(string _error, bool _result)
        {
            Error = _error;
            Result = _result;
        }
    }

    public class PolarisationResultList
    {
        public string Error { set; get; }
        public List<bool> Results { set; get; }

        public PolarisationResultList(string _error)
        {
            Error = _error;
            Results = new List<bool>();
        }
    }


    public enum ApiType {
        CLASSIC_API,
        BATCH_API,
        QASM_API
    }
}
