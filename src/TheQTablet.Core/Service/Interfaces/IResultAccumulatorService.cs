using System;

using TheQTablet.Core.DataModel;


namespace TheQTablet.Core.Service.Interfaces
{
    public interface IResultAccumulatorService
    {
        void AddExperimentResult(PolarisationDataAccumulatedResult accumulator, int result);
        void AddExperimentResult(PolarisationDataAccumulatedResult accumulator, bool result);
    }
}
