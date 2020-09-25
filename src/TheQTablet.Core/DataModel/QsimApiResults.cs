using System;
using System.Collections.Generic;
namespace TheQTablet.Core.DataModel
{
    public class QsimApiResult_CreateRegister
    {
        public string error { get; set; }
        public int result { get; set; }
    }
    public class QsimApiResult_SetState
    {
        public string error { get; set; }
        public bool result { get; set; }
    }
    public class QsimApiResult_GateOperation
    {
        public string error { get; set; }
        public bool result { get; set; }
    }
    public class QsimApiResult_Measure
    {
        public string error { get; set; }
        public int result { get; set; }
    }
    public class QsimApiResultUnit_Print
    {
        public List<Object> result { get; set; }
    }
    public class QsimApiResult_Print
    {
        public string error { get; set; }
        public List<List<Object>> result { get; set; }

    }
    public class QsimApiResult_DestroyRegister
    {
        public string error { get; set; }
        public int result { get; set; }
    }
}
