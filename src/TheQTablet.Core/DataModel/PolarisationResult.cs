using System;
namespace TheQTablet.Core.DataModel
{
    public class PolarisationResult
    {
        public string error;
        bool result;

        public PolarisationResult(string _error, bool _result)
        {
            error = _error;
            result = _result;
        }
    }
}
