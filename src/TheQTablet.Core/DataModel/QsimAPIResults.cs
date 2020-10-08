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

    public class ComplexValue
    {
        public float im { get; set; } = 0.0f;
        public float re { get; set; } = 0.0f;
    }

    public class VectorResult
    {
        public string binary_pattern { get; set; } = "";
        public ComplexValue complex_value { get; set; } = new ComplexValue();
        public int state { get; set; } = 0;
    }

    public class VectorOperationResult : BasicOperationResult
    {
        public List<VectorResult> Result { get; set; } = new List<VectorResult>();
    }

    public class ErrorAPIResult
    {
        public string Status { get; set; } = "";
    }

}