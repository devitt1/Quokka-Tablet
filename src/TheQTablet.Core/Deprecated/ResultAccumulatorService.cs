using System;

using TheQTablet.Core.DataModel;
using TheQTablet.Core.Service.Interfaces;


namespace TheQTablet.Core.Service.Implementations
{
    public class ResultAccumulatorService : IResultAccumulatorService
    {
        public ResultAccumulatorService()
        {
        }

        public void AddExperimentResult(PolarisationDataAccumulatedResult accumulator, int result)
        {
            if (result < 0 || result > 1)
            {
                throw new ArgumentOutOfRangeException("Out of range provided result " + result + " (must be 0 or 1)");
            }
            accumulator.AccumulatedPhotons += result;
            accumulator.NumberOfExperiments++;
        }

        public void AddExperimentResult(PolarisationDataAccumulatedResult accumulator, bool result)
        {
            if (result)
            {
                AddExperimentResult(accumulator, 1);
            }
            else
            {
                AddExperimentResult(accumulator, 0);
            }
        }
    }
}
