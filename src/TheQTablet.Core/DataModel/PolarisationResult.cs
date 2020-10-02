using System;
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
}
