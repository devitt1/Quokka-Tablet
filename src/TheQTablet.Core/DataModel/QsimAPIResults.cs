using System;
using System.Collections.Generic;
namespace TheQTablet.Core.DataModel
{
    public class BasicOperationResult : ErrorAPIResult
    {
        public string Operation { get; set; } = "";

        public string Error { get; set; } = "";
    }

    public class BoolOperationResult : BasicOperationResult
    {
        public bool Result { get; set; } = false;
    }

    public class QbitsOperationResult : BasicOperationResult
    {
        public int Result { get; set; } = 0;
    }

    public class VectorOperationResult : BasicOperationResult
    {
        public List<List<object>> Result { get; set; } = new List<List<object>>();
    }

    public class ErrorAPIResult
    {
        public string Status { get; set; } = "";
    }
    
}
