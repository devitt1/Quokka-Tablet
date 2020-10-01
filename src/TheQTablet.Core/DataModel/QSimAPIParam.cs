using System;
using System.Collections.Generic;

namespace TheQTablet.Core.DataModel
{
    public class QsimApiParam_GateOperation
    {
        public int register { get; set; }
        public string gate { get; set; }
        public int q { get; set; }
        public float theta { get; set; }
    }
    public class QsimApiParam_Measure
    {
        public int register { get; set; }
        public List<int> lq2m { get; set; }
    }

}
