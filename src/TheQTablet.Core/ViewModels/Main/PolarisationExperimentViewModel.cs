using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MvvmCross.Logging;
using MvvmCross.Commands;

using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.DataModel;

namespace TheQTablet.Core.ViewModels.Main
{
    public class PolarisationExperimentViewModel : BaseViewModel
    {
        private readonly IMvxLog _log;
        private readonly ISimulatorService _polarisationSimulatorService;
        private readonly IResultAccumulatorService _resultAccumulatorService;

        private Dictionary<int, PolarisationDataAccumulatedResult> _accumulators;
        private float _frequencyOfExperiment = 1; // in Hz
        private int _currentTelescopePolarisation_deg = 0;
        private int _atmosphericPolarisation_deg = 0;
        private int _rotationStep_deg = 5;

        private bool _inited = false;
        private bool _experimenting = false;

        // MVVM Commands
        public MvxAsyncCommand StartOnePolarisationSimulationCommand { get; private set; }
        public MvxAsyncCommand StartSimulationCommand { get; private set; }


        // To be bound to the ViewController:
        //  * Experimenting
        //  * ExperimentAccumulators
        //  * TelescopePolarisation
        //  * StartOnePolarisationSimulationCommand
        public bool Experimenting
        {
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
        public Dictionary<int, PolarisationDataAccumulatedResult> ExperimentAccumulators { get => _accumulators; }
        public int TelescopePolarisation
        {
            get => _currentTelescopePolarisation_deg;
            set
            {
                // enforcing rotation step constraint
                if ((value % _rotationStep_deg) != 0)
                {
                    throw new Exception("TelescopePolarisation SET failed because value " + value + " is not divisible by provided RotationStep_deg " + _rotationStep_deg);
                }
                _currentTelescopePolarisation_deg = value;
                RaisePropertyChanged(() => TelescopePolarisation);
            }
        }


        public PolarisationExperimentViewModel(IMvxLog log, ISimulatorService polarisationSimulatorService, IResultAccumulatorService resultAccumulatorService)
        {
            _log = log;
            _log.Trace("PolarisationExperimentViewModel:PolarisationExperimentViewModel()");
            _frequencyOfExperiment = 1;

            _inited = false;
            _experimenting = false;

            _polarisationSimulatorService = polarisationSimulatorService;
            _accumulators = new Dictionary<int, PolarisationDataAccumulatedResult>();

            _resultAccumulatorService = resultAccumulatorService;

            StartOnePolarisationSimulationCommand = new MvxAsyncCommand(RunOnePolarisationSimulation);
            StartSimulationCommand = new MvxAsyncCommand(StartSimulationAsync);

        }

        private async Task StartSimulationAsync()
        {
            await _polarisationSimulatorService.Run();
            _log.Trace(" PolarisationSimulatorService: awaited");
        }


        /*
        public override async Task Initialize()
        {
            await base.Initialize();

        }
        */

        private async Task RunOnePolarisationSimulation()
        {
            if (!_inited)
            {
                Init2(5, 0, 0);
            }
            _log.Trace("PolarisationExperimentViewModel:RunOnePolarisationSimulation()");
            bool result = await _polarisationSimulatorService.Run();
            _resultAccumulatorService.AddExperimentResult(_accumulators[_currentTelescopePolarisation_deg], result);

            // Raising the property change event for the accumulators
            await RaisePropertyChanged(() => ExperimentAccumulators);
        }


        // Apparently, Init is a special fuciton name because it get called by an unknown thread with all parameters being 0
        public void Init2(int RotationStep_deg, int AtmosphericPolarisation_deg, int InitialTelescopePolarisation_deg)
        {
            // The aim of this function is to set everything up ready to call StartExperimenting()
            // Can only be called ONCE

            _log.Trace("PolarisationExperimentViewModel:Init() RotationStep_deg:" + RotationStep_deg + " AtmosphericPolarisation_deg:" + AtmosphericPolarisation_deg + " InitialTelescopePolarisation_deg:" + InitialTelescopePolarisation_deg);
            if (_inited)
            {
                throw new Exception("Init() failed because the object has already been inited");
            }
            if (RotationStep_deg == 0)
            {
                throw new Exception("Init() failed because provided RotationStep_deg is 0, and that will cause a divide by zero exception on the next line of code");
            }
            if ((360 % RotationStep_deg) != 0)
            {
                throw new Exception("Init() failed because 360 is not divisible by provided RotationStep_deg " + RotationStep_deg);
            }

            _atmosphericPolarisation_deg = AtmosphericPolarisation_deg;
            _rotationStep_deg = RotationStep_deg;
            TelescopePolarisation = InitialTelescopePolarisation_deg;

            // Initialise the dictionary of PolarisationAccumulator
            _accumulators = new Dictionary<int, PolarisationDataAccumulatedResult>();
            for (int i = 0; i < 360; i += RotationStep_deg)
            {
                _accumulators[i] = new PolarisationDataAccumulatedResult();
            }

            _inited = true;
        }

        public void StartExperimenting()
        {
            if (!_inited)
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
            _frequencyOfExperiment = Frequency;
            StartExperimenting();
        }

        public void StopExperienting()
        {
            if (!_inited)
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
