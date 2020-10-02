﻿using System;
using System.Threading.Tasks;

using MvvmCross.ViewModels;

namespace TheQTablet.Core.DataModel
{
    // TODO: separate into data and functional class
    public class DataResult
    {

    }

    //look at linq

    public class PolarisationAccumulator
    {
        /* PolarisationAccumulator
         * Accumulate results from the polarisation experiment
         * Convention: Value is set to 0 if no experiment results are known
         * */

        private int _accumulatedPhotons;
        private int _numberOfExperiment;

        public float Value {
            get
            {
                if (_numberOfExperiment > 0)
                    return _accumulatedPhotons / _numberOfExperiment;
                return (float)0;
            }
        }
        public int AccumulatedPhotons { get => _accumulatedPhotons; }
        public int NumberOfExperiment { get => _numberOfExperiment; }

        public PolarisationAccumulator()
        {
            _accumulatedPhotons = 0;
            _numberOfExperiment = 0;
        }

        public void AddExperimentResult ( int result)
        {
            if (result < 0 || result > 1)
            {
                throw new ArgumentOutOfRangeException("Out of range provided result " + result + " (must be 0 or 1)");
            }
            _accumulatedPhotons += result;
            _numberOfExperiment++;

            //Todo: workout of to signal a property change, maybe this should be done from the ViewModel?
            //RaisePropertyChanged(() => Value);
        }

        public void AddExperimentResult(bool result)
        {
            if (result)
            {
                AddExperimentResult(1);
            }
            else
            {
                AddExperimentResult(0);
            }
        }
    }
}
