/* File created by Jeremy Brun from UTS:Rapido
 * jeremy.brun@uts.edu.au / jeremy.brun@gmail.com
 * Project: TheQ
 */

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
using System.Collections.ObjectModel;

namespace TheQTablet.Core.ViewModels.Main
{
    public class PolarisationExperimentViewModel : BaseViewModel
    {
        public PolarisationExperimentViewModel(IMvxLog log, ISimulatorService polarisationSimulatorService, IResultAccumulatorService resultAccumulatorService)
        {
            _log = log;
            _log.Trace("PolarisationExperimentViewModel:PolarisationExperimentViewModel()");

            _inited = false;

            _polarisationSimulatorService = polarisationSimulatorService;
            _telescopeAccumulators = new Dictionary<int, PolarisationDataAccumulatedResult>();

            _resultAccumulatorService = resultAccumulatorService;

            _storedResults = new Queue<KeyValuePair<int, bool>>();

            // Hardcoded 5deg step, atmopheric rotation of 90deg, initial telescopefilter 0deg
            InternalInit(10, 30, 0);

            StartOnePolarisationSimulationCommand = new MvxAsyncCommand(RunOneExperimentAsync);
            StartSimulationCommand = new MvxAsyncCommand(RunOneExperimentAsync);
            StartMultipleSimulationsCommand = new MvxAsyncCommand(RunSetNumberofExperimentsAsync);
            ToggleOverlayCosSquare = new MvxAsyncCommand(ToggleOverlayCosSquareAsync);
            ToggleContinuousSimulation = new MvxAsyncCommand(StartContinuousSimulationAsync);
            CurrentTelescopeFilterRotationSliderTouchCancel = new MvxAsyncCommand(CurrentTelescopeFilterRotationSliderTouchCancelAsync);
            ToggleAutoSweepSimulation = new MvxAsyncCommand(ToggleAutoSweepSimulationAsync);
            ToggleAutoFixSimulation = new MvxAsyncCommand(ToggleAutoFixSimulationAsync);
            ToggleExperiment = new MvxAsyncCommand(ToggleExperimentAsync);
        }

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

            currentExperimentOnTimer = ExperimentOnTimerTypes.None;
            _atmosphericPolarisation_deg = AtmosphericPolarisation_deg;

            Debug.Assert(((360 % RotationStep_deg) == 0), "provded RotationStep_deg:" + RotationStep_deg + " CANNOT divide 360 as an interger");
            _rotationStep_deg = RotationStep_deg;
            TelescopePolarisation = InitialTelescopePolarisation_deg;

            // Initialise the dictionary of PolarisationAccumulator
            _telescopeAccumulators = new Dictionary<int, PolarisationDataAccumulatedResult>();
            _satelliteAccumulators = new Dictionary<int, PolarisationDataAccumulatedResult>();
            for (int i = 0; i < 360; i += RotationStep_deg)
            {
                _telescopeAccumulators[i] = new PolarisationDataAccumulatedResult();
                _satelliteAccumulators[i] = new PolarisationDataAccumulatedResult();
            }

            _inited = true;
        }

        // Logs and required Services
        private readonly IMvxLog _log;
        private readonly ISimulatorService _polarisationSimulatorService;
        private readonly IResultAccumulatorService _resultAccumulatorService;

        // The two results accumulator for the telescope and satellite experiments
        private Dictionary<int, PolarisationDataAccumulatedResult> _telescopeAccumulators;
        private Dictionary<int, PolarisationDataAccumulatedResult> _satelliteAccumulators;


        // Internal variables
        private float _frequencyOfExperiment = 10; // in Hz
        private int _currentFilterPolarisation_deg = 0;
        private int _atmosphericPolarisation_deg = 30; 
        private int _rotationStep_deg = 10;
        private int _currentTelescopeSliderPosition = 0;

        private bool _inited = false;   // prevents simulation to be run before internalInit() has been called

        // fields related to overlaying the Cos^2 function in the graph
        private bool _overlayCosSquare = false;
        public bool OverlayCosSquare
        {
            get => _overlayCosSquare;
            set
            {
                _overlayCosSquare = value;
                GeneratePlotModel();
                RaisePropertyChanged(() => PlotModel);
            }
        }

        // Showing/hiding the telescope/satellite graph experiment
        private bool _showTelescopeSeries = true;
        private bool _showSatelliteSeries = true;


        // Set of possible experiments on timer
        private enum ExperimentOnTimerTypes
        {
            None,
            SetNumber,
            Continuous,
            AutoFix,
            Sweep
        }
        ExperimentOnTimerTypes currentExperimentOnTimer = ExperimentOnTimerTypes.None;

        private enum ExperimentTypes
        {
            Telescope,
            Satellite
        }
        ExperimentTypes currentExperiment = ExperimentTypes.Telescope;

        // MVVM Commands
        public MvxAsyncCommand StartOnePolarisationSimulationCommand { get; private set; }
        public MvxAsyncCommand StartSimulationCommand { get; private set; }
        public MvxAsyncCommand StartMultipleSimulationsCommand { get; private set; }
        public MvxAsyncCommand ToggleOverlayCosSquare { get; private set; }
        public MvxAsyncCommand ToggleContinuousSimulation { get; private set; }
        public MvxAsyncCommand ToggleSweepSimulation { get; private set; }
        public MvxAsyncCommand CurrentTelescopeFilterRotationSliderTouchCancel { get; private set; }
        public MvxAsyncCommand ToggleAutoSweepSimulation { get; private set; }
        public MvxAsyncCommand ToggleAutoFixSimulation { get; private set; }
        public MvxAsyncCommand ToggleExperiment { get; private set; }

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
                Maximum = 360,
                Minimum = -5,
                Title = "TIME TITLE",
                TitleColor = OxyColors.White,
                AxislineColor = OxyColors.LightGray,
                TicklineColor = OxyColors.LightGray
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = 1.02,
                Minimum = -0.02,
                Title = "TARGETS TITLE",
                TitleColor = OxyColors.White,
                AxislineColor = OxyColors.LightGray,
                TicklineColor = OxyColors.LightGray
            });

            // The gray rectangle showing current selection
            var currentRotationSerie = new RectangleBarSeries
            {
                StrokeThickness = 0,
                FillColor = OxyColors.WhiteSmoke
            };
            var currentRotationItem = new RectangleBarItem(_currentFilterPolarisation_deg - (float)_rotationStep_deg/2, 0, _currentFilterPolarisation_deg + (float)_rotationStep_deg / 2, 1);
            currentRotationSerie.Items.Add(currentRotationItem);

            model.Series.Add(currentRotationSerie);

            if (_showTelescopeSeries)
            {
                // Telescope experiment line
                var telescopeLineSeries = new LineSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = OxyColors.Red,
                    Color = OxyColors.Red,
                    Title = "Telescope"
                };

                // Plotting the data from the PolarisationDataAccumulatedResult
                foreach (KeyValuePair<int, PolarisationDataAccumulatedResult> entry in _telescopeAccumulators)
                {
                    if (!float.IsNaN(entry.Value.Value))
                    {
                        telescopeLineSeries.Points.Add(new DataPoint(entry.Key, entry.Value.Value));
                    }
                }
                model.Series.Add(telescopeLineSeries);
            }

            if (_showSatelliteSeries)
            {
                // Satellite experiment line
                var satelliteLineSeries = new LineSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = OxyColors.Blue,
                    Color = OxyColors.Blue,
                    Title = "Satellite"
                };

                // Plotting the data from the PolarisationDataAccumulatedResult
                foreach (KeyValuePair<int, PolarisationDataAccumulatedResult> entry in _satelliteAccumulators)
                {
                    if (!float.IsNaN(entry.Value.Value))
                    {
                        satelliteLineSeries.Points.Add(new DataPoint(entry.Key, entry.Value.Value));
                    }
                }
                model.Series.Add(satelliteLineSeries);
            }

            // Cos^2 overlay
            if (_overlayCosSquare)
            {
                model.Series.Add(new FunctionSeries(cosSquareDegree, 0, 359, 0.1, "Cos^2"));
            }

            return model;
        }

        private double cosSquareDegree(double x)
        {
            return Math.Cos(2 * Math.PI * x / 360)* Math.Cos(2 * Math.PI * x / 360);
        }


        public int CurrentTelescopeFilterRotationSlider
        {
            get => _currentTelescopeSliderPosition;
            set
            {
                if (isRunningAutomaticExperiment() && _currentTelescopeSliderPosition != value)
                {
                    // if currently running a sweep or a fix, skip setting the current angle
                    return;
                }

                // Rotation Slider only change the angle if not currently in an Auto Sweep to Fix
                _currentTelescopeSliderPosition = value;
                RaisePropertyChanged(() => CurrentTelescopeFilterRotationSlider);
                RecalculateFilterPolarisation();
            }
        }
        


        // To be bound to the ViewController:
        //  * Experimenting
        //  * ExperimentAccumulators
        //  * TelescopePolarisation
        //  * StartOnePolarisationSimulationCommand

        //public Dictionary<int, PolarisationDataAccumulatedResult> ExperimentAccumulators { get => _telescopeAccumulators; }
        public int TelescopePolarisation
        {
            get => _currentFilterPolarisation_deg;
            set
            {
                // enforcing rotation step constraint
                Debug.Assert(((value % _rotationStep_deg) == 0), "TelescopePolarisation SET failed because value " + value + " is not divisible by provided RotationStep_deg " + _rotationStep_deg);
                if ((value % _rotationStep_deg) != 0)
                {
                    throw new Exception("TelescopePolarisation SET failed because value " + value + " is not divisible by provided RotationStep_deg " + _rotationStep_deg);
                }
                _currentFilterPolarisation_deg = value;
                RaisePropertyChanged(() => TelescopePolarisation);
                RaisePropertyChangedForExperiment();
            }
        }

        private async Task CurrentTelescopeFilterRotationSliderTouchCancelAsync()
        {
            // When the user release the slider selector while running a continuous experiment
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.Continuous)
            {
                _log.Trace("CurrentTelescopeFilterRotationSliderTouchCancelAsync() clearing stored data");
                _storedResults.Clear();
            }
        }

        private bool isRunningAutomaticExperiment()
        {
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.AutoFix
                || currentExperimentOnTimer == ExperimentOnTimerTypes.Sweep)
                return true;
            return false;
        }

        private void RaisePropertyChangedForExperiment()
        {
            RaisePropertyChanged(() => NumberOfExperiments);
            RaisePropertyChanged(() => NumberOfCapturedPhotons);
            RaisePropertyChanged(() => AverageNumberOfPhotonCaptured);
            RaisePropertyChanged(() => PlotModel);
        }

        // Gather and queues experiment results to be used by timer callback functions
        private async Task gatherExperimentResults(int atmPolarisation, int filterPolarisaton, int number, bool stackable)
        {
            if (_fetchingForStoredResults && !stackable)
            {
                // if results are not stackable and  there is already a thread fetching, skip fetching more results
                _log.Trace("gatherExperimentResults() skipping call QASM as _fetchingForStoredResultsis true");
                return;
            }

            _log.Trace("gatherExperimentResults() about to call QASM");
            _fetchingForStoredResults = true;
            PolarisationResultList result = await _polarisationSimulatorService.Run(atmPolarisation, filterPolarisaton, ApiType.QASM_API, number);
            _log.Trace("gatherExperimentResults() got result from QASM, enqueing...");

            foreach (bool element in result.Results)
            {
                _storedResults.Enqueue(new KeyValuePair<int, bool>(filterPolarisaton, element));
            }
            _fetchingForStoredResults = false;
            _log.Trace("gatherExperimentResults() result queued done!");
        }

        public int NumberOfExperiments
        {
            get
            {
                switch (currentExperiment)
                {
                    case ExperimentTypes.Satellite:
                        return _satelliteAccumulators[_currentFilterPolarisation_deg].NumberOfExperiments;
                    case ExperimentTypes.Telescope:
                    default:
                        return _telescopeAccumulators[_currentFilterPolarisation_deg].NumberOfExperiments;
                }
            }
        }
        public int NumberOfCapturedPhotons
        {
            get
            {
                switch (currentExperiment)
                {
                    case ExperimentTypes.Satellite:
                        return _satelliteAccumulators[_currentFilterPolarisation_deg].AccumulatedPhotons;
                    case ExperimentTypes.Telescope:
                    default:
                        return _telescopeAccumulators[_currentFilterPolarisation_deg].AccumulatedPhotons;
                }
            }
        }
        public float AverageNumberOfPhotonCaptured
        {
            get
            {
                switch (currentExperiment)
                {
                    case ExperimentTypes.Satellite:
                        return _satelliteAccumulators[_currentFilterPolarisation_deg].Value;
                    case ExperimentTypes.Telescope:
                    default:
                        return _telescopeAccumulators[_currentFilterPolarisation_deg].Value;
                }
            }
        }

        private void RecalculateFilterPolarisation()
        {
            int number_of_steps_per_turn = (int) 360 / _rotationStep_deg;
            int number_of_steps = (int)Math.Round( (double) number_of_steps_per_turn * CurrentTelescopeFilterRotationSlider / 100);
            TelescopePolarisation = number_of_steps * _rotationStep_deg;
        }



        private async Task ToggleAutoSweepSimulationAsync()
        {
            if (!_inited)
            {
                throw new Exception("ToggleAutoSweepSimulationAsync() failed because the object has not be inited");
            }

            if (currentExperimentOnTimer == ExperimentOnTimerTypes.Sweep)
            {
                _log.Trace("ToggleAutoSweepSimulationAsync(): Stop experimenting");
                currentExperimentOnTimer = ExperimentOnTimerTypes.None;
                return;
            }

            // Clearing any left over results in the store
            _storedResults.Clear();

            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("StartExperimenting() Setting up Timer @ " + timerDelay + "ms");


            Dictionary<int, PolarisationDataAccumulatedResult> _accumulator;

            switch (currentExperiment)
            {
                case ExperimentTypes.Satellite:
                    _log.Trace("ToggleAutoFixSimulationAsync() Satellite experiment");
                    _sweepData = new sweepData(0, 10);
                    _accumulator = _satelliteAccumulators;
                    break;
                case ExperimentTypes.Telescope:
                default:
                    _accumulator = _telescopeAccumulators;
                    _sweepData = new sweepData(_atmosphericPolarisation_deg, 10);
                    _log.Trace("ToggleAutoFixSimulationAsync() Telescope experiment");
                    break;
            }

            //Gathering the first set of experiment for angle 0
            _log.Trace("ToggleAutoSweepSimulationAsync() about to call QASM");
            await gatherExperimentResults(_sweepData.AtmophericPolarisation, 0, 80, true);

            currentExperimentOnTimer = ExperimentOnTimerTypes.Sweep;
            setExperimentTimer(ExperimentOnTimerTypes.Sweep, timerSweepExperimentTick, _accumulator, 0, timerDelay);
            _log.Trace("StartExperimenting() Timer setup");
        }

        private class sweepData
        {
            public int ReleasePerTick;
            public int CurrentFilterAngle;
            public int AtmophericPolarisation;


            public sweepData(int _AtmophericPolarisation, int _ReleasePerTick)
            {
                AtmophericPolarisation = _AtmophericPolarisation;
                ReleasePerTick = _ReleasePerTick;
                CurrentFilterAngle = 0;
            }
        }
        private sweepData _sweepData;

        private void timerSweepExperimentTick(Object state)
        {
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.None)
            {
                // End of experiment, destroy timer & clean data
                _log.Trace("timerSweepExperimentTick() stopping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                return;
            }

            if (_storedResults.Count < _sweepData.ReleasePerTick
                && _sweepData.CurrentFilterAngle < (360 - _rotationStep_deg)
                && !_fetchingForStoredResults)
            {
                // If the number of stored data is low, launch an async task to get more data
                _sweepData.CurrentFilterAngle = _sweepData.CurrentFilterAngle + _rotationStep_deg;
                _log.Trace("timerSweepExperimentTick(): getting more data for angle " + _sweepData.CurrentFilterAngle);
                var task = Task.Run(async () => await gatherExperimentResults(_sweepData.AtmophericPolarisation, _sweepData.CurrentFilterAngle, 80, true));
                _fetchingForStoredResults = true;
            }

            if (_storedResults.Count > 0)
            {
                // Only push data to the accumulator if there is some
                //_log.Trace("timerContinuousExperimentTick(): AddExperimentResult");
                int tmp_last_angle = 0;
                for (int i=0; (i< _sweepData.ReleasePerTick && _storedResults.Count > 0);++i)
                {
                    KeyValuePair<int, bool> result = _storedResults.Dequeue();
                    //_resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[result.Key], result.Value);
                    Dictionary<int, PolarisationDataAccumulatedResult> tmp = (Dictionary<int, PolarisationDataAccumulatedResult>)state;
                    _resultAccumulatorService.AddExperimentResult(tmp[result.Key], result.Value);
                    tmp_last_angle = result.Key;
                }
                _currentFilterPolarisation_deg = tmp_last_angle;
                RaisePropertyChangedForExperiment();
            }

            if (!_fetchingForStoredResults
                && _storedResults.Count == 0
                && _sweepData.CurrentFilterAngle >= (360 - _rotationStep_deg))
            {
                // Only destroying the timer (finishing the sweep) if the system is not still waiting for data (if _fetchingForStoredResults is false)
                // Setting _autoSweepRunning to false will destroy the timer in the next iteration
                _log.Trace("timerSweepExperimentTick(): sweep finished!");
                currentExperimentOnTimer = ExperimentOnTimerTypes.None;
            }


        }

        private class autoFixData
        {
            public int ReleasePerTick;
            public int MinExperimentPerAngle;
            public int AtmophericPolarisation;
            public Queue<KeyValuePair<int, int>> RequiredData; // [angle] -> [amount of data needed]

            public autoFixData(int _AtmophericPolarisation, int _ReleasePerTick, Dictionary<int, PolarisationDataAccumulatedResult> _accumulators, int _minExperimentPerAngle = 100)
            {
                AtmophericPolarisation = _AtmophericPolarisation;
                ReleasePerTick = _ReleasePerTick;
                MinExperimentPerAngle = _minExperimentPerAngle;
                RequiredData = new Queue<KeyValuePair<int, int>>();
                foreach (KeyValuePair<int, PolarisationDataAccumulatedResult> entry in _accumulators)
                {
                    if (entry.Value.NumberOfExperiments < _minExperimentPerAngle)
                    {
                        RequiredData.Enqueue(new KeyValuePair<int, int> (entry.Key, _minExperimentPerAngle - entry.Value.NumberOfExperiments));
                    }
                }
            }
        }

        private autoFixData _autoFixData;

        private async Task ToggleAutoFixSimulationAsync()
        {
            if (!_inited)
            {
                throw new Exception("ToggleAutoFixSimulationAsync() failed because the object has not be inited");
            }

            if (currentExperimentOnTimer == ExperimentOnTimerTypes.AutoFix)
            {
                _log.Trace("ToggleAutoFixSimulationAsync(): Stop experimenting");
                currentExperimentOnTimer = ExperimentOnTimerTypes.None;
                return;
            }

            // Clearing any left over results in the store
            _storedResults.Clear();

            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            Dictionary<int, PolarisationDataAccumulatedResult> _accumulator;

            switch (currentExperiment)
            {
                case ExperimentTypes.Satellite:
                    _log.Trace("ToggleAutoFixSimulationAsync() Satellite experiment");
                    _autoFixData = new autoFixData(0, 10, _satelliteAccumulators, 100);
                    _accumulator = _satelliteAccumulators;
                    break;
                case ExperimentTypes.Telescope:
                default:
                    _accumulator = _telescopeAccumulators;
                    _autoFixData = new autoFixData(_atmosphericPolarisation_deg, 10, _telescopeAccumulators, 100);
                    _log.Trace("ToggleAutoFixSimulationAsync() Telescope experiment");
                    break;
            }

            if (_autoFixData.RequiredData.Count == 0)
            {
                // NO required data => nothing to do
                return;
            }

            // Gathering the first set of experiments
            KeyValuePair<int, int> neededDataSet = _autoFixData.RequiredData.Dequeue();
            await gatherExperimentResults(_autoFixData.AtmophericPolarisation, neededDataSet.Key, neededDataSet.Value, true);

            _log.Trace("ToggleAutoFixSimulationAsync() Setting up Timer @ " + timerDelay + "ms");
            setExperimentTimer(ExperimentOnTimerTypes.AutoFix, timerFixExperimentTick, _accumulator, 0, timerDelay);
            _log.Trace("ToggleAutoFixSimulationAsync() Timer setup");
        }

        private void setExperimentTimer(ExperimentOnTimerTypes type, System.Threading.TimerCallback _callback, Object _obj, int _delay, int _repeat)
        {
            // Avoid duplicate timers
            if (experimentTimer != null)
                experimentTimer.Dispose();

            // Reset experiment booleans
            currentExperimentOnTimer = type;
            experimentTimer = new System.Threading.Timer(_callback, _obj, _delay, _repeat);
        }

        private void timerFixExperimentTick(Object state)
        {
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.None)
            {
                // End of experiment, destroy timer & clean data
                _log.Trace("timerFixExperimentTick() stopping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                return;
            }

            if (_storedResults.Count < _autoFixData.ReleasePerTick && !_fetchingForStoredResults && _autoFixData.RequiredData.Count > 0)
            {
                // Needs and can fetch more data
                // If the number of stored data is low, launch an async task to get more data
                KeyValuePair<int, int> neededDataSet = _autoFixData.RequiredData.Dequeue();
                
                _log.Trace("timerFixExperimentTick(): getting more data for angle " + neededDataSet.Key);
                var task = Task.Run(async () => await gatherExperimentResults(_autoFixData.AtmophericPolarisation, neededDataSet.Key, neededDataSet.Value, true));
                _fetchingForStoredResults = true;
            }

            if (_storedResults.Count > 0)
            {
                // Push some stored results into the accumuator & display
                // Only push data to the accumulator if there is some
                //_log.Trace("timerContinuousExperimentTick(): AddExperimentResult");
                int tmp_last_angle = 0;
                for (int i = 0; (i < _autoFixData.ReleasePerTick && _storedResults.Count > 0); ++i)
                {
                    KeyValuePair<int, bool> result = _storedResults.Dequeue();
                    //                    _resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[result.Key], result.Value);
                    Dictionary<int, PolarisationDataAccumulatedResult> tmp = (Dictionary<int, PolarisationDataAccumulatedResult>)state;
                    _resultAccumulatorService.AddExperimentResult(tmp[result.Key], result.Value);
                    tmp_last_angle = result.Key;
                }
                _currentFilterPolarisation_deg = tmp_last_angle;
                RaisePropertyChangedForExperiment();
            }

            if (!_fetchingForStoredResults && _storedResults.Count ==0 && _autoFixData.RequiredData.Count == 0)
            {
                // Set to destroy timer as:
                //  - there is no more and data AND
                //  - the system is not trying to fetch data at the moment AND
                //  - there is no more required data
                //
                // Only destroying the timer (finishing the sweep) if the system is not still waiting for data (if _fetchingForStoredResults is false)
                // Setting _autoSweepRunning to false will destroy the timer in the next iteration
                _log.Trace("timerFixExperimentTick(): _fetchingForStoredResults is False, and stored data is empty End of Data, setting state to None ");
                currentExperimentOnTimer = ExperimentOnTimerTypes.None;
            }
        }

        private async Task RunOneExperimentAsync()
        {
            Debug.Assert(_inited, "RunOneExperimentAsync cannot be called before the InitInternal was called");

            _log.Trace("RunOneExperimentAsync()");

            Dictionary<int, PolarisationDataAccumulatedResult> _accumulator;
            bool result;

            switch (currentExperiment)
            {
                case ExperimentTypes.Satellite:
                    _log.Trace("RunOneExperimentAsync() Satellite experiment");
                    _accumulator = _satelliteAccumulators;
                    result = await _polarisationSimulatorService.Run(0, _currentFilterPolarisation_deg, ApiType.QASM_API);
                    break;
                case ExperimentTypes.Telescope:
                default:
                    _log.Trace("RunOneExperimentAsync() Telescope experiment");
                    _accumulator = _telescopeAccumulators;
                    result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentFilterPolarisation_deg, ApiType.QASM_API);
                    break;
            }


            _log.Trace("RunOneExperimentAsync() Got results from QASM()");
            _resultAccumulatorService.AddExperimentResult(_accumulator[_currentFilterPolarisation_deg], result);

            // Raising the property change event for the accumulators
            RaisePropertyChangedForExperiment();
        }

        private async Task ToggleOverlayCosSquareAsync()
        {
            OverlayCosSquare = !OverlayCosSquare;
        }

        private async Task ToggleExperimentAsync()
        {
            _log.Trace("ToggleExperimentAsync(): Stop experimenting");
            StopExperienting();
            _log.Trace("ToggleExperimentAsync(): Switching experiment");
            if (currentExperiment == ExperimentTypes.Telescope)
                currentExperiment = ExperimentTypes.Satellite;
            else if (currentExperiment == ExperimentTypes.Satellite)
                currentExperiment = ExperimentTypes.Telescope;

            RaisePropertyChangedForExperiment();

            return;
        }



        public async Task StartContinuousSimulationAsync()
        {
            if (!_inited)
            {
                throw new Exception("StartContinuousSimulationAsync() failed because the object has not be inited");
            }
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.Continuous)
            {
                _log.Trace("StartContinuousSimulationAsync(): Stop experimenting");
                currentExperimentOnTimer = ExperimentOnTimerTypes.None;
                return;
            }

            _storedResults.Clear();
            int timerDelay = (int)(1000 / _frequencyOfExperiment);

            switch (currentExperiment)
            {
                case ExperimentTypes.Satellite:
                    _log.Trace("StartContinuousSimulationAsync() Satellite experiment");
                    await gatherExperimentResults(0, _currentFilterPolarisation_deg, 100, false);
                    setExperimentTimer(ExperimentOnTimerTypes.Continuous, timerContinuousExperimentTick, _satelliteAccumulators, 0, timerDelay);
                    break;
                case ExperimentTypes.Telescope:
                default:
                    _log.Trace("StartContinuousSimulationAsync() Telescope experiment");
                    await gatherExperimentResults(_atmosphericPolarisation_deg, _currentFilterPolarisation_deg, 100, false);
                    setExperimentTimer(ExperimentOnTimerTypes.Continuous, timerContinuousExperimentTick, _telescopeAccumulators, 0, timerDelay);
                    break;
            }
            _log.Trace("StartExperimenting() Timer setup");

        }

        private void timerContinuousExperimentTick(Object state)
        {
            //_log.Trace("timerContinuousExperimentTick() called");
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.None)
            {
                // End of experiment, destroy timer & clean data
                _log.Trace("timerContinuousExperimentTick() stoping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                return;
            }
            if (_storedResults.Count < 3)
            {
                // If the number of stored data is low, launch an async task to get more data
                _log.Trace("timerContinuousExperimentTick(): getting more data");
                switch (currentExperiment)
                {
                    case ExperimentTypes.Satellite:
                        var task = Task.Run(async () => await gatherExperimentResults(0, _currentFilterPolarisation_deg, 100, false));
                        break;
                    case ExperimentTypes.Telescope:
                    default:
                        var task2 = Task.Run(async () => await gatherExperimentResults(_atmosphericPolarisation_deg, _currentFilterPolarisation_deg, 100, false));
                        break;
                }
            }
            if (_storedResults.Count > 0)
            {
                // Only push data to the accumulator if there is some
                KeyValuePair<int, bool> result = _storedResults.Dequeue();
                //_log.Trace("timerContinuousExperimentTick(): AddExperimentResult");
                //_resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[result.Key], result.Value);
                Dictionary<int, PolarisationDataAccumulatedResult> tmp = (Dictionary<int, PolarisationDataAccumulatedResult>)state;
                _resultAccumulatorService.AddExperimentResult(tmp[result.Key], result.Value);

                RaisePropertyChangedForExperiment();
            }
        }


        private async Task RunSetNumberofExperimentsAsync()
        {
            if (!_inited)
            {
                throw new Exception("RunSetNumberofExperimentsAsync() failed because the object has not be inited");
            }
            if (currentExperimentOnTimer==ExperimentOnTimerTypes.SetNumber)
            {
                _log.Trace("RunSetNumberofExperimentsAsync() failed because the object is already experimenting");
                return;
            }

            // Number of experiment is hardcoded here at the moment
            int _numberOfExperiments = 100;

            Dictionary<int, PolarisationDataAccumulatedResult> _accumulator;

            switch (currentExperiment)
            {
                case ExperimentTypes.Satellite:
                    _log.Trace("RunSetNumberofExperimentsAsync() Satellite experiment");
                    await gatherExperimentResults(0, _currentFilterPolarisation_deg, _numberOfExperiments, false);
                    _accumulator = _satelliteAccumulators;
                    break;
                case ExperimentTypes.Telescope:
                default:
                    _log.Trace("RunSetNumberofExperimentsAsync() Telescope experiment");
                    await gatherExperimentResults(_atmosphericPolarisation_deg, _currentFilterPolarisation_deg, _numberOfExperiments, false);
                    _accumulator = _telescopeAccumulators;
                    break;
            }

            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("RunSetNumberofExperimentsAsync() Setting up Timer @ " + timerDelay + "ms");

            setExperimentTimer(ExperimentOnTimerTypes.SetNumber, setNumberOfExperimentsTimerTick, _accumulator, 0, timerDelay);

            _log.Trace("RunSetNumberofExperimentsAsync() Timer setup");
        }


        public void StopExperienting()
        {
            if (!_inited)
            {
                throw new Exception("StopExperienting() failed because the object has not be inited");
            }

            if (experimentTimer != null)
                experimentTimer.Dispose();
            _storedResults.Clear();
            currentExperimentOnTimer = ExperimentOnTimerTypes.None;
            _log.Trace("StopExperienting() cleaning all running experiments");
        }

        private System.Threading.Timer experimentTimer;
        private Queue<KeyValuePair<int, bool>> _storedResults;
        private bool _fetchingForStoredResults = false;

        
        private void setNumberOfExperimentsTimerTick(Object state)
        {

            _log.Trace("setNumberOfExperimentsTimerTick() called");
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.None)
            {
                _log.Trace("setNumberOfExperimentsTimerTick() stopping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                return;
            }
            else if (currentExperimentOnTimer != ExperimentOnTimerTypes.SetNumber)
            {
                _log.Trace("setNumberOfExperimentsTimerTick() current experimenton timer notSetNumber, ignoring the call");
                return;
            }
            else if (currentExperimentOnTimer == ExperimentOnTimerTypes.SetNumber && _storedResults.Count < 1)
            {
                _log.Trace("setNumberOfExperimentsTimerTick() out of data");
                currentExperimentOnTimer = ExperimentOnTimerTypes.None;
                return;
            }

            KeyValuePair<int, bool> result = _storedResults.Dequeue();
            _log.Trace("setNumberOfExperimentsTimerTick() AddExperimentResult ");
            //_resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[result.Key], result.Value);
            Dictionary<int, PolarisationDataAccumulatedResult> tmp = (Dictionary<int, PolarisationDataAccumulatedResult>)state;
            _resultAccumulatorService.AddExperimentResult(tmp[result.Key], result.Value);

            RaisePropertyChangedForExperiment();
        }
    }
}
