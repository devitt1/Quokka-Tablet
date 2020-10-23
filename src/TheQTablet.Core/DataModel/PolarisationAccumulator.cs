using System;
using System.Threading.Tasks;

using MvvmCross.ViewModels;

namespace TheQTablet.Core.DataModel
{

    public class PolarisationDataAccumulatedResult
    {
        public int AccumulatedPhotons { get; set; }
        public int NumberOfExperiments { get; set; }
        public float Value
        {
            get
            {
                if (NumberOfExperiments > 0)
                    return (float)AccumulatedPhotons / NumberOfExperiments;
                return float.NaN;
            }
        }

        public PolarisationDataAccumulatedResult()
        {
            AccumulatedPhotons = 0;
            NumberOfExperiments = 0;
        }
    }

    //look at linq

}
