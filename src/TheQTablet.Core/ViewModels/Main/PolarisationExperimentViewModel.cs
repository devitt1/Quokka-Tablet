using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

using MvvmCross.Logging;
using MvvmCross.Commands;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

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

        private int _currentTelescopeSliderPosition = 0;

        private bool _inited = false;
        private bool _experimenting = false; // TODO: NOT USED YET

        // MVVM Commands
        public MvxAsyncCommand StartOnePolarisationSimulationCommand { get; private set; }
        public MvxAsyncCommand StartSimulationCommand { get; private set; }
        public MvxAsyncCommand StartMultipleSimulationsCommand { get; private set; }

        // MVVM Properties
        public PlotModel PlotModel => GeneratePlotModel();

        // Generating graph
        private PlotModel GeneratePlotModel()
        {
            var model = new PlotModel
            {
                PlotAreaBorderColor = OxyColors.LightGray,
                LegendTextColor = OxyColors.LightGray,
                LegendTitleColor = OxyColors.LightGray,
                TextColor = OxyColors.White
            };
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "TIME TITLE",
                TitleColor = OxyColors.White,
                AxislineColor = OxyColors.LightGray,
                TicklineColor = OxyColors.LightGray
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = 1,
                Minimum = 0,
                Title = "TARGETS TITLE",
                TitleColor = OxyColors.White,
                AxislineColor = OxyColors.LightGray,
                TicklineColor = OxyColors.LightGray
            });

            var series1 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.Red,
                Color = OxyColors.Red
            };

            // Plotting the data from the PolarisationDataAccumulatedResult
            foreach (KeyValuePair<int, PolarisationDataAccumulatedResult> entry in ExperimentAccumulators)
            {
                series1.Points.Add(new DataPoint(entry.Key, entry.Value.Value));
            }

            model.Series.Add(series1);

            return model;
        }


        public int CurrentTelescopeFilterRotationSlider
        {
            get => _currentTelescopeSliderPosition;
            set
            {
                _currentTelescopeSliderPosition = value;
                RaisePropertyChanged(() => CurrentTelescopeFilterRotationSlider);
                RecalculateTelescopePolarisation();
            }
        }


        // To be bound to the ViewController:
        //  * Experimenting
        //  * ExperimentAccumulators
        //  * TelescopePolarisation
        //  * StartOnePolarisationSimulationCommand

        // TODO: NOT USED YET
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
                Debug.Assert(((value % _rotationStep_deg) == 0), "TelescopePolarisation SET failed because value " + value + " is not divisible by provided RotationStep_deg " + _rotationStep_deg);
                if ((value % _rotationStep_deg) != 0)
                {
                    throw new Exception("TelescopePolarisation SET failed because value " + value + " is not divisible by provided RotationStep_deg " + _rotationStep_deg);
                }
                _currentTelescopePolarisation_deg = value;
                RaisePropertyChanged(() => TelescopePolarisation);
                RaisePropertyChangedForExperiment();
            }
        }

        private void RaisePropertyChangedForExperiment()
        {
            RaisePropertyChanged(() => NumberOfExperiments);
            RaisePropertyChanged(() => NumberOfCapturedPhotons);
            RaisePropertyChanged(() => AverageNumberOfPhotonCaptured);
            RaisePropertyChanged(() => PlotModel);
            
        }

        public int NumberOfExperiments
        {
            get => _accumulators[_currentTelescopePolarisation_deg].NumberOfExperiments;
        }
        public int NumberOfCapturedPhotons
        {
            get => _accumulators[_currentTelescopePolarisation_deg].AccumulatedPhotons;
        }
        public float AverageNumberOfPhotonCaptured
        {
            get => _accumulators[_currentTelescopePolarisation_deg].Value;
        }

        private void RecalculateTelescopePolarisation()
        {
            int number_of_steps_per_turn = (int) 360 / _rotationStep_deg;
            int number_of_steps = (int)Math.Round( (double) number_of_steps_per_turn * CurrentTelescopeFilterRotationSlider / 100);
            TelescopePolarisation = number_of_steps * _rotationStep_deg;
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

            // Hardcoded 5deg step, atmopheric rotation of 90deg, initial telescopefilter 0deg
            InternalInit(5, 90, 0);

            StartOnePolarisationSimulationCommand = new MvxAsyncCommand(StartSimulationAsync);
            StartSimulationCommand = new MvxAsyncCommand(StartSimulationAsync);
            StartMultipleSimulationsCommand = new MvxAsyncCommand(StartMultipleSimulationsAsync);

        }

        
        private async Task StartSimulationAsync()
        {
            Debug.Assert(_inited, "StartSimulationAsync cannot be called before the InitInternal was called");

            _log.Trace("PolarisationExperimentViewModel:StartSimulationAsync()");
            bool result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, ApiType.QASM_API);
            _resultAccumulatorService.AddExperimentResult(_accumulators[_currentTelescopePolarisation_deg], result);

            // Raising the property change event for the accumulators
            RaisePropertyChangedForExperiment();
        }
        


        private async Task StartMultipleSimulationsAsync()
        {
            Debug.Assert(_inited, "StartMultipleSimulationsAsync cannot be called before the InitInternal was called");

            _log.Trace("StartMultipleSimulationsAsync:StartSimulationAsync()");
            PolarisationResultList result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, ApiType.QASM_API, 100);

            foreach(bool element in result.Results)
            {
                _resultAccumulatorService.AddExperimentResult(_accumulators[_currentTelescopePolarisation_deg], element);
            }

            // Raising the property change event for the accumulators
            RaisePropertyChangedForExperiment();
        }



        /*
        public override async Task Initialize()
        {
            await base.Initialize();

        }
        */


        // Internal Init function
        // This is where this initial degree per step is set
        // and where the accumulators are instanciated
        public void InternalInit(int RotationStep_deg, int AtmosphericPolarisation_deg, int InitialTelescopePolarisation_deg)
        {
            // The aim of this function is to set everything up ready to call StartExperimenting()
            // Can only be called ONCE

            _log.Trace("PolarisationExperimentViewModel:InternalInit() RotationStep_deg:" + RotationStep_deg + " AtmosphericPolarisation_deg:" + AtmosphericPolarisation_deg + " InitialTelescopePolarisation_deg:" + InitialTelescopePolarisation_deg);
            if (_inited)
            {
                throw new Exception("InternalInit() failed because the object has already been inited");
            }
            if (RotationStep_deg == 0)
            {
                throw new Exception("InternalInit() failed because provided RotationStep_deg is 0, and that will cause a divide by zero exception on the next line of code");
            }
            if ((360 % RotationStep_deg) != 0)
            {
                throw new Exception("InternalInit() failed because 360 is not divisible by provided RotationStep_deg " + RotationStep_deg);
            }

            _atmosphericPolarisation_deg = AtmosphericPolarisation_deg;

            Debug.Assert(((360 % RotationStep_deg) == 0), "provded RotationStep_deg:" + RotationStep_deg + " CANNOT divide 360 as an interger");
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

        // TODO: NOT USED YET
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

        // TODO: NOT USED YET
        public void StartExperimenting(float Frequency)
        {
            _frequencyOfExperiment = Frequency;
            StartExperimenting();
        }

        // TODO: NOT USED YET
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

        // TODO: NOT USED YET
        /*
        private async Task Tick()
        {
            // This is the function to execute ONE (1) experiment
            // 
            //
            _polarisationSimulatorService.Run();
        }
        */

    }
}
