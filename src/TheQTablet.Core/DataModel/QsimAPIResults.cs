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
        public float Im { get; set; } = 0.0f;
        public float Re { get; set; } = 0.0f;
    }

    public class VectorResult
    {
        public string BinaryPattern { get; set; } = "";
        public ComplexValue ComplexValue { get; set; } = new ComplexValue();
        public int State { get; set; } = 0;
    }

    public class VectorOperationResult : BasicOperationResult
    {
        public List<VectorResult> Result { get; set; } = new List<VectorResult>();
    }

    public class ErrorAPIResult
    {
        public string Status { get; set; } = "";
    }


    public class ClassicalRegistersList
    {
        public List<List<int>> c;
    }
    public class QsamOperationResult
    {
        public int error_code;
        public string error;
        public ClassicalRegistersList result;
    }

}
