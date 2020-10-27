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
        private readonly IMvxLog _log;
        private readonly ISimulatorService _polarisationSimulatorService;
        private readonly IResultAccumulatorService _resultAccumulatorService;

        private Dictionary<int, PolarisationDataAccumulatedResult> _telescopeAccumulators;
        private Dictionary<int, PolarisationDataAccumulatedResult> _satelliteAccumulators;
        private float _frequencyOfExperiment = 10; // in Hz
        private int _currentTelescopePolarisation_deg = 0;
        private int _atmosphericPolarisation_deg = 0;
        private int _rotationStep_deg = 5;

        private int _currentTelescopeSliderPosition = 0;

        private bool _inited = false;   // prevents simulation to be run before internalInit() has been called

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


            var currentRotationSerie = new RectangleBarSeries
            {
                StrokeThickness = 0,
                FillColor = OxyColors.WhiteSmoke
            };
            var currenTotationItem = new RectangleBarItem(_currentTelescopePolarisation_deg - (float)_rotationStep_deg/2, 0, _currentTelescopePolarisation_deg + (float)_rotationStep_deg / 2, 1);
            currentRotationSerie.Items.Add(currenTotationItem);



            model.Series.Add(currentRotationSerie);

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
                if ( !float.IsNaN(entry.Value.Value) )
                {
                    series1.Points.Add(new DataPoint(entry.Key, entry.Value.Value));
                }
            }
            model.Series.Add(series1);

            if (_overlayCosSquare)
            {
                model.Series.Add(new FunctionSeries(cosSquareDegree, 0, 359, 0.1));
            }

            return model;
        }

        private double cosSquareDegree(double x)
        {
            return Math.Cos(2 * Math.PI * x / 360)* Math.Cos(2 * Math.PI * x / 360);
        }

        private int internalCurrentTelescopeFilterRotationSlider
        {
            get => _currentTelescopeSliderPosition;
            set
            {
                _currentTelescopeSliderPosition = value;
                CurrentTelescopeFilterRotationSlider = value;
            }
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
            get
            {
                return currentExperimentOnTimer!=ExperimentOnTimerTypes.None;
            }
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
        public Dictionary<int, PolarisationDataAccumulatedResult> ExperimentAccumulators { get => _telescopeAccumulators; }
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
                //_storedResults.Clear();
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

        private async Task gatherExperimentResults(int number = 100)
        {
            await gatherExperimentResults(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, number, false);
        }
        private async Task gatherExperimentResults(int telescopePolarisaton, int number, bool stackable= false)
        {
            await gatherExperimentResults(_atmosphericPolarisation_deg, telescopePolarisaton, number, stackable);
        }

        private async Task gatherExperimentResults(int atmPolarisation, int telescopePolarisaton, int number, bool stackable)
        {
            if (_fetchingForStoredResults && !stackable)
            {
                // if results are not stackable and  there is already a thread fetching, skip fetching more results
                _log.Trace("gatherExperimentResults() skipping call QASM as _fetchingForStoredResultsis true");
                return;
            }

            _log.Trace("gatherExperimentResults() about to call QASM");
            _fetchingForStoredResults = true;
            PolarisationResultList result = await _polarisationSimulatorService.Run(atmPolarisation, telescopePolarisaton, ApiType.QASM_API, number);
            _log.Trace("gatherExperimentResults() got result from QASM, enqueing...");

            foreach (bool element in result.Results)
            {
                _storedResults.Enqueue(new KeyValuePair<int, bool>(telescopePolarisaton, element));
            }
            _fetchingForStoredResults = false;
            _log.Trace("gatherExperimentResults() result queued done!");
        }

        public int NumberOfExperiments
        {
            get => _telescopeAccumulators[_currentTelescopePolarisation_deg].NumberOfExperiments;
        }
        public int NumberOfCapturedPhotons
        {
            get => _telescopeAccumulators[_currentTelescopePolarisation_deg].AccumulatedPhotons;
        }
        public float AverageNumberOfPhotonCaptured
        {
            get => _telescopeAccumulators[_currentTelescopePolarisation_deg].Value;
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

            _inited = false;

            _polarisationSimulatorService = polarisationSimulatorService;
            _telescopeAccumulators = new Dictionary<int, PolarisationDataAccumulatedResult>();

            _resultAccumulatorService = resultAccumulatorService;

            _storedResults = new Queue<KeyValuePair<int, bool>>();

            // Hardcoded 5deg step, atmopheric rotation of 90deg, initial telescopefilter 0deg
            InternalInit(10, 0, 0);

            StartOnePolarisationSimulationCommand = new MvxAsyncCommand(RunOneExperimentAsync);
            StartSimulationCommand = new MvxAsyncCommand(RunOneExperimentAsync);
            StartMultipleSimulationsCommand = new MvxAsyncCommand(RunSetNumberofExperimentsAsync);
            ToggleOverlayCosSquare = new MvxAsyncCommand(ToggleOverlayCosSquareAsync);
            ToggleContinuousSimulation = new MvxAsyncCommand(StartContinuousSimulationAsync);
            CurrentTelescopeFilterRotationSliderTouchCancel = new MvxAsyncCommand(CurrentTelescopeFilterRotationSliderTouchCancelAsync);
            ToggleAutoSweepSimulation = new MvxAsyncCommand(ToggleAutoSweepSimulationAsync);
            ToggleAutoFixSimulation = new MvxAsyncCommand(ToggleAutoFixSimulationAsync);
            //StopExperimenting = new MvxAsyncCommand(StartContinuousSimulationAsync);

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


            _log.Trace("ToggleAutoSweepSimulationAsync() about to call QASM");

            //Gathering the first set of experiment for angle 0
            _storedResults.Clear();
            await gatherExperimentResults(0, 200, true);

            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("StartExperimenting() Setting up Timer @ " + timerDelay + "ms");

            currentExperimentOnTimer = ExperimentOnTimerTypes.Sweep;
            _timerState = new timerState(10);

            setExperimentTimer(ExperimentOnTimerTypes.Sweep, timerSweepExperimentTick, null, 0, timerDelay);
            _log.Trace("StartExperimenting() Timer setup");
        }

        private class timerState
        {
            public int ReleasePerTick;
            public int CurrentFilterAngle;

            public timerState(int _ReleasePerTick)
            {
                ReleasePerTick = _ReleasePerTick;
                CurrentFilterAngle = 0;
            }
        }
        private timerState _timerState;

        private void timerSweepExperimentTick(Object state)
        {
            //_log.Trace("timerContinuousExperimentTick() called");
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.None)
            {
                // End of experiment, destroy timer & clean data
                _log.Trace("timerSweepExperimentTick() stopping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                return;
            }
            if (_storedResults.Count < _timerState.ReleasePerTick)
            {
                if (_timerState.CurrentFilterAngle < (360- _rotationStep_deg))
                {
                    // If the number of stored data is low, launch an async task to get more data
                    _timerState.CurrentFilterAngle = _timerState.CurrentFilterAngle + _rotationStep_deg;
                    _log.Trace("timerSweepExperimentTick(): getting more data for angle " + _timerState.CurrentFilterAngle);
                    var task = Task.Run(async () => await gatherExperimentResults(_timerState.CurrentFilterAngle, 200, true));
                }
                else if (!_fetchingForStoredResults)
                {
                    // Only destroying the timer (finishing the sweep) if the system is not still waiting for data (if _fetchingForStoredResults is false)
                    // Setting _autoSweepRunning to false will destroy the timer in the next iteration
                    currentExperimentOnTimer = ExperimentOnTimerTypes.None;
                }

            }
            if (_storedResults.Count > 0)
            {
                // Only push data to the accumulator if there is some
                //_log.Trace("timerContinuousExperimentTick(): AddExperimentResult");
                int tmp_last_angle = 0;
                for (int i=0; (i< _timerState.ReleasePerTick && _storedResults.Count > 0);++i)
                {
                    KeyValuePair<int, bool> result = _storedResults.Dequeue();
                    _resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[result.Key], result.Value);
                    tmp_last_angle = result.Key;
                }
                _currentTelescopePolarisation_deg = tmp_last_angle;
                RaisePropertyChangedForExperiment();
            }

        }

        private class autoFixData
        {
            public int ReleasePerTick;
            public int MinExperimentPerAngle;
            //public Dictionary<int, int> RequiredData;
            public Queue<KeyValuePair<int, int>> RequiredData; // [angle] -> [amount of data needed]

            public autoFixData(int _ReleasePerTick, Dictionary<int, PolarisationDataAccumulatedResult> _accumulators, int _minExperimentPerAngle = 100)
            {
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

            _autoFixData = new autoFixData(10, _telescopeAccumulators, 100);
            if (_autoFixData.RequiredData.Count == 0)
            {
                // nothing to do
                return;
            }
            KeyValuePair<int, int> neededDataSet = _autoFixData.RequiredData.Dequeue();
            await gatherExperimentResults(neededDataSet.Key, neededDataSet.Value, true);
            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("ToggleAutoFixSimulationAsync() Setting up Timer @ " + timerDelay + "ms");

            setExperimentTimer(ExperimentOnTimerTypes.AutoFix, timerFixExperimentTick, null, 0, timerDelay);

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
            //_log.Trace("timerContinuousExperimentTick() called");
            if (currentExperimentOnTimer == ExperimentOnTimerTypes.None)
            {
                // End of experiment, destroy timer & clean data
                _log.Trace("timerFixExperimentTick() stopping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                return;
            }
            if (_storedResults.Count < _autoFixData.ReleasePerTick)
            {
                if (_autoFixData.RequiredData.Count > 0 )
                {
                    // If the number of stored data is low, launch an async task to get more data
                    KeyValuePair<int, int> neededDataSet = _autoFixData.RequiredData.Dequeue();
                    
                    _log.Trace("timerFixExperimentTick(): getting more data for angle " + neededDataSet.Key);
                    var task = Task.Run(async () => await gatherExperimentResults(neededDataSet.Key, neededDataSet.Value, true));
                }
                else if (!_fetchingForStoredResults)
                {
                    // Only destroying the timer (finishing the sweep) if the system is not still waiting for data (if _fetchingForStoredResults is false)
                    // Setting _autoSweepRunning to false will destroy the timer in the next iteration
                    currentExperimentOnTimer = ExperimentOnTimerTypes.None;
                }

            }
            if (_storedResults.Count > 0)
            {
                // Only push data to the accumulator if there is some
                //_log.Trace("timerContinuousExperimentTick(): AddExperimentResult");
                int tmp_last_angle = 0;
                for (int i = 0; (i < _autoFixData.ReleasePerTick && _storedResults.Count > 0); ++i)
                {
                    KeyValuePair<int, bool> result = _storedResults.Dequeue();
                    _resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[result.Key], result.Value);
                    tmp_last_angle = result.Key;
                }
                _currentTelescopePolarisation_deg = tmp_last_angle;
                RaisePropertyChangedForExperiment();
            }
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

            _atmosphericPolarisation_deg = AtmosphericPolarisation_deg;

            Debug.Assert(((360 % RotationStep_deg) == 0), "provded RotationStep_deg:" + RotationStep_deg + " CANNOT divide 360 as an interger");
            _rotationStep_deg = RotationStep_deg;
            TelescopePolarisation = InitialTelescopePolarisation_deg;

            // Initialise the dictionary of PolarisationAccumulator
            _telescopeAccumulators = new Dictionary<int, PolarisationDataAccumulatedResult>();
            for (int i = 0; i < 360; i += RotationStep_deg)
            {
                _telescopeAccumulators[i] = new PolarisationDataAccumulatedResult();
            }

            _inited = true;
        }

        private async Task RunOneExperimentAsync()
        {
            Debug.Assert(_inited, "RunOneExperimentAsync cannot be called before the InitInternal was called");

            _log.Trace("RunOneExperimentAsync()");
            bool result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, ApiType.QASM_API);
            _log.Trace("RunOneExperimentAsync() Got results from QASM()");
            _resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[_currentTelescopePolarisation_deg], result);

            // Raising the property change event for the accumulators
            RaisePropertyChangedForExperiment();
        }

        private async Task ToggleOverlayCosSquareAsync()
        {
            OverlayCosSquare = !OverlayCosSquare;
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

            _log.Trace("StartContinuousSimulationAsync() about to call QASM");
            _storedResults.Clear();
            await gatherExperimentResults();


            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("StartExperimenting() Setting up Timer @ "+ timerDelay + "ms");
            setExperimentTimer(ExperimentOnTimerTypes.Continuous, timerContinuousExperimentTick, null, 0, timerDelay);

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
                var task = Task.Run(async () => await gatherExperimentResults());
            }
            if (_storedResults.Count > 0)
            {
                // Only push data to the accumulator if there is some
                KeyValuePair<int, bool> result = _storedResults.Dequeue();
                //_log.Trace("timerContinuousExperimentTick(): AddExperimentResult");
                _resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[result.Key], result.Value);
                RaisePropertyChangedForExperiment();
            }
        }


        // TODO: NOT USED YET
        public void StartExperimenting()
        { }

        private async Task RunSetNumberofExperimentsAsync()
        {
            if (!_inited)
            {
                throw new Exception("RunSetNumberofExperimentsAsync() failed because the object has not be inited");
            }
            if (currentExperimentOnTimer==ExperimentOnTimerTypes.SetNumber)//_continuousExperimentRunning)
            {
                _log.Trace("RunSetNumberofExperimentsAsync() failed because the object is already experimenting");
                return;
            }

            // Number of experiment is hardcoded here at the moment
            int _numberOfExperiments = 100;

            _log.Trace("RunSetNumberofExperimentsAsync() about to call QASM");
            PolarisationResultList result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, ApiType.QASM_API, _numberOfExperiments);
            _log.Trace("RunSetNumberofExperimentsAsync() got result from QASM");

            foreach (bool element in result.Results)
            {
                var test = new KeyValuePair<int, bool>(_currentTelescopePolarisation_deg, element);
                _storedResults.Enqueue(new KeyValuePair<int, bool>(_currentTelescopePolarisation_deg, element));
            }
            _log.Trace("RunSetNumberofExperimentsAsync() _storedResults.Count: " + _storedResults.Count); 

            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("RunSetNumberofExperimentsAsync() Setting up Timer @ " + timerDelay + "ms");

            setExperimentTimer(ExperimentOnTimerTypes.SetNumber, setNumberOfExperimentsTimerTick, null, 0, timerDelay);

            _log.Trace("RunSetNumberofExperimentsAsync() Timer setup");
        }


        // TODO: NOT USED YET
        public void StopExperienting()
        {
            if (!_inited)
            {
                throw new Exception("StopExperienting() failed because the object has not be inited");
            }

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
            _resultAccumulatorService.AddExperimentResult(_telescopeAccumulators[result.Key], result.Value);
            RaisePropertyChangedForExperiment();
        }
    }
}
