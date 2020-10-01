using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MvvmCross.Logging;

using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.DataModel;

namespace TheQTablet.Core.ViewModels.Main
{
    public class PolarisationExperimentViewModel : BaseViewModel
    {
        private readonly IMvxLog _log;
        private readonly ISimulatorService _polarisationSimulatorService;

        private Dictionary<int, PolarisationAccumulator> _Accumulators;
        private float _FrequencyOfExperiment = 1; // in Hz
        private int _CurrentTelescopePolarisation_deg = 0;
        private int _AtmosphericPolarisation_deg = 0;
        private int _RotationStep_deg = 5;

        private bool _initited = false;
        private bool _experimenting = false;

        // To be bound to the ViewController:
        //  * Experimenting
        //  * ExperimentAccumulators
        //  * TelescopePolarisation
        public bool Experimenting {
            get => _experimenting;
            set
            {
                if (value)
                {
                    StartExperimenting();
                }
                else
                {
                    StopExperienting();
                }
            }
        }
        public Dictionary<int, PolarisationAccumulator> ExperimentAccumulators { get => _Accumulators; }
        public int TelescopePolarisation {
            get => _CurrentTelescopePolarisation_deg;
            set
            {
                // enforcing rotation step constraint
                if ((value % _RotationStep_deg) != 0)
                {
                    throw new Exception("TelescopePolarisation SET failed because value "+value+" is not divisible by provided RotationStep_deg " + _RotationStep_deg);
                }
                _CurrentTelescopePolarisation_deg = value;
                RaisePropertyChanged(() => TelescopePolarisation);
            }
        }


        public PolarisationExperimentViewModel(IMvxLog log, ISimulatorService polarisationSimulatorService)
        {
            _log = log;
            _log.Trace("PolarisationExperimentViewModel:PolarisationExperimentViewModel()");
            _FrequencyOfExperiment = 1;

            _initited = false;
            _experimenting = false;

            _polarisationSimulatorService = polarisationSimulatorService;
            _Accumulators = new Dictionary<int, PolarisationAccumulator>();

        }

        /*
        public override async Task Initialize()
        {
            await base.Initialize();

        }
        */

        public void Init(int RotationStep_deg, int AtmosphericPolarisation_deg, int InitialTelescopePolarisation_deg)
        {
            // The aim of this function is to set everything up ready to call StartExperimenting()
            // Can only be called ONCE

            _log.Trace("PolarisationExperimentViewModel:Init() RotationStep_deg:"+ RotationStep_deg + " AtmosphericPolarisation_deg:"+ AtmosphericPolarisation_deg + " InitialTelescopePolarisation_deg:" + InitialTelescopePolarisation_deg);
            if (_initited)
            {
                throw new Exception("Init() failed because the object has already been inited");
            }
            if ((360 % RotationStep_deg) != 0)
            {
                throw new Exception("Init() failed because 360 is not divisible by provided RotationStep_deg "+ RotationStep_deg);
            }

            _AtmosphericPolarisation_deg = AtmosphericPolarisation_deg;
            _RotationStep_deg = RotationStep_deg;
            TelescopePolarisation = InitialTelescopePolarisation_deg;

            // Initialise the dictionary of PolarisationAccumulator
            _Accumulators = new Dictionary<int, PolarisationAccumulator>();
            for (int i=0; i<360; i+= RotationStep_deg)
            {
                _Accumulators[i] = new PolarisationAccumulator();
            }

            _initited = true;
        }

        public void StartExperimenting()
        {
            if (!_initited)
            {
                throw new Exception("StartExperimenting() failed because the object has not be inited");
            }
            if (_experimenting)
            {
                _log.Trace("StartExperimenting() failed because the object is already experimenting");
                return;
            }

            /*
             * TODO: Do the Experimenting thing, how to set a timer calling the Tick function
             * 
             * */


        }
        public void StartExperimenting(float Frequency)
        {
            _FrequencyOfExperiment = Frequency;
            StartExperimenting();
        }

        public void StopExperienting()
        {
            if (!_initited)
            {
                throw new Exception("StopExperienting() failed because the object has not be inited");
            }
            if (!_experimenting)
            {
                _log.Trace("StopExperienting() failed because the object is not experimenting");
                return;
            }

            /*
            * TODO: Stop the Experimenting thing
            * */
        }

        /*
        private async Task Tick()
        {
            // This is the function to execute ONE (1) experiment
             // 
             //
            _polarisationSimulatorService.Run()
        // send the event here about variable change state.
        }
        */
             
    
    }
}
