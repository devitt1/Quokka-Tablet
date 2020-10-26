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

        private Dictionary<int, PolarisationDataAccumulatedResult> _accumulators;
        private float _frequencyOfExperiment = 10; // in Hz
        private int _currentTelescopePolarisation_deg = 0;
        private int _atmosphericPolarisation_deg = 0;
        private int _rotationStep_deg = 5;

        private int _currentTelescopeSliderPosition = 0;

        private bool _inited = false;   // prevents simulation to be run before internalInit() has been called
        private bool _continuousExperimentRunning = false;
        private bool _autoSweepRunning = false; // prevent the UI slider to change the current filter angle during Sweep process
        private bool _autoFixRunning = false; // prevent the UI slider to change the current filter angle during Fix process

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

        public int CurrentTelescopeFilterRotationSlider
        {
            get => _currentTelescopeSliderPosition;
            set
            {
                if (!_autoSweepRunning && !_autoFixRunning)
                {
                    // Rotation Slider only change the angle if not currently in an Auto Sweep to Fix
                    _currentTelescopeSliderPosition = value;
                    RaisePropertyChanged(() => CurrentTelescopeFilterRotationSlider);
                    RecalculateTelescopePolarisation();
                }
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
            get => _continuousExperimentRunning;
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
                //_storedResults.Clear();
                RaisePropertyChanged(() => TelescopePolarisation);
                RaisePropertyChangedForExperiment();
            }
        }

        private async Task CurrentTelescopeFilterRotationSliderTouchCancelAsync()
        {
            if (!_autoSweepRunning && !_autoFixRunning)
            {
                _storedResults.Clear();
            }
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

            _inited = false;
            _continuousExperimentRunning = false;

            _polarisationSimulatorService = polarisationSimulatorService;
            _accumulators = new Dictionary<int, PolarisationDataAccumulatedResult>();

            _resultAccumulatorService = resultAccumulatorService;

            _storedResults = new Queue<KeyValuePair<int, bool>>();

            // Hardcoded 5deg step, atmopheric rotation of 90deg, initial telescopefilter 0deg
            InternalInit(10, 0, 0);

            StartOnePolarisationSimulationCommand = new MvxAsyncCommand(StartSimulationAsync);
            StartSimulationCommand = new MvxAsyncCommand(StartSimulationAsync);
            StartMultipleSimulationsCommand = new MvxAsyncCommand(StartMultipleSimulationsAsync);
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

            if (_autoSweepRunning)
            {
                _log.Trace("ToggleAutoSweepSimulationAsync(): Stop experimenting");
                _autoSweepRunning = false;
                return;
            }


            _log.Trace("ToggleAutoSweepSimulationAsync() about to call QASM");

            //Gathering the first set of experiment for angle 0
            await gatherExperimentResults(0,200,true);

            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("StartExperimenting() Setting up Timer @ " + timerDelay + "ms");

            _autoSweepRunning = true;
            _timerState = new timerState(10);
            this.experimentTimer = new System.Threading.Timer(timerSweepExperimentTick, null, 0, timerDelay);
            _log.Trace("StartExperimenting() Timer setup");
        }

        private class timerState
        {
            public int ReleasePerTick;
            public int CurrentFilterAngle;
            //public Dictionary<int, int> ReleaseData;

            public timerState(int _ReleasePerTick)
            {
                ReleasePerTick = _ReleasePerTick;
                CurrentFilterAngle = 0;
                //ReleaseData = new Dictionary<int, int>();
            }
        }
        private timerState _timerState;

        private void timerSweepExperimentTick(Object state)
        {
            //_log.Trace("timerContinuousExperimentTick() called");
            if (!_autoSweepRunning || _storedResults == null)
            {
                // End of experiment, destroy timer & clean data
                _log.Trace("timerSweepExperimentTick() stopping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                _autoSweepRunning = false;
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
                    _autoSweepRunning = false;
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
                    _resultAccumulatorService.AddExperimentResult(_accumulators[result.Key], result.Value);
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

            if (_autoFixRunning)
            {
                _log.Trace("ToggleAutoFixSimulationAsync(): Stop experimenting");
                _autoFixRunning = false;
                return;
            }

            _autoFixData = new autoFixData(10, _accumulators, 100);
            if (_autoFixData.RequiredData.Count == 0)
            {
                // nothing to do
                return;
            }
            KeyValuePair<int, int> neededDataSet = _autoFixData.RequiredData.Dequeue();
            await gatherExperimentResults(neededDataSet.Key, neededDataSet.Value, true);
            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("ToggleAutoFixSimulationAsync() Setting up Timer @ " + timerDelay + "ms");

            _autoFixRunning = true;
            _timerState = new timerState(10);
            this.experimentTimer = new System.Threading.Timer(timerFixExperimentTick, null, 0, timerDelay);
            _log.Trace("ToggleAutoFixSimulationAsync() Timer setup");

        }

        private void timerFixExperimentTick(Object state)
        {
            //_log.Trace("timerContinuousExperimentTick() called");
            if (!_autoFixRunning || _storedResults == null)
            {
                // End of experiment, destroy timer & clean data
                _log.Trace("timerFixExperimentTick() stopping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                _autoFixRunning = false;
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
                    _autoFixRunning = false;
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
                    _resultAccumulatorService.AddExperimentResult(_accumulators[result.Key], result.Value);
                    tmp_last_angle = result.Key;
                }
                _currentTelescopePolarisation_deg = tmp_last_angle;
                RaisePropertyChangedForExperiment();
            }
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




        private async Task StartSimulationAsync()
        {
            Debug.Assert(_inited, "StartSimulationAsync cannot be called before the InitInternal was called");

            _log.Trace("PolarisationExperimentViewModel:StartSimulationAsync()");
            bool result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, ApiType.QASM_API);
            _log.Trace("PolarisationExperimentViewModel: Got results from QASM()");
            _resultAccumulatorService.AddExperimentResult(_accumulators[_currentTelescopePolarisation_deg], result);

            // Raising the property change event for the accumulators
            RaisePropertyChangedForExperiment();
        }

        private async Task ToggleOverlayCosSquareAsync()
        {
            OverlayCosSquare = !OverlayCosSquare;
        }


        /*
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
        */

        public async Task StartContinuousSimulationAsync()
        {
            if (!_inited)
            {
                throw new Exception("StartContinuousSimulationAsync() failed because the object has not be inited");
            }
            if (_continuousExperimentRunning)
            {
                _log.Trace("StartContinuousSimulationAsync(): Stop experimenting");
                _continuousExperimentRunning = false;
                return;
            }


            // To be pushed to the Tick function later

            _log.Trace("StartContinuousSimulationAsync() about to call QASM");
            await gatherExperimentResults();

            /*
            PolarisationResultList result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, ApiType.QASM_API, 100);
            _log.Trace("StartContinuousSimulationAsync() got result from QASM");

            foreach (bool element in result.Results)
            {
                _storedResults.Enqueue(new KeyValuePair<int, bool>(_currentTelescopePolarisation_deg, element));
            }
            */

            int timerDelay = (int)(1000 / _frequencyOfExperiment);
            _log.Trace("StartExperimenting() Setting up Timer @ "+ timerDelay + "ms");
            this.experimentTimer = new System.Threading.Timer(timerContinuousExperimentTick, null, 0, timerDelay);
            _continuousExperimentRunning = true;
            _log.Trace("StartExperimenting() Timer setup");

        }

        private void timerContinuousExperimentTick(Object state)
        {
            //_log.Trace("timerContinuousExperimentTick() called");
            if (!_continuousExperimentRunning || _storedResults == null)
            {
                // End of experiment, destroy timer & clean data
                _log.Trace("timerTick() stoping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                _continuousExperimentRunning = false;
                return;
            }
            if (_storedResults.Count < 3)
            {
                // If the number of stored data is low, launch an async task to get more data
                _log.Trace("timerContinuousExperimentTick(): getting more data");
                var task = Task.Run(async () => await gatherExperimentResults());
                /*
                var task = Task.Run(async () => {
                    _log.Trace("Anomymous function from timerContinuousExperimentTick(): getting more data");
                    PolarisationResultList result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, ApiType.QASM_API, 100);
                    _log.Trace("Anomymous function from timerContinuousExperimentTick(): got result from QASM");
                    foreach (bool element in result.Results)
                    {
                        _storedResults.Enqueue(new KeyValuePair<int, bool>(_currentTelescopePolarisation_deg, element));
                    }
                });
                */
            }
            if (_storedResults.Count > 0)
            {
                // Only push data to the accumulator if there is some
                KeyValuePair<int, bool> result = _storedResults.Dequeue();
                //_log.Trace("timerContinuousExperimentTick(): AddExperimentResult");
                _resultAccumulatorService.AddExperimentResult(_accumulators[result.Key], result.Value);
                RaisePropertyChangedForExperiment();
            }
        }




        // TODO: NOT USED YET
        public void StartExperimenting()
        { }

        private async Task StartMultipleSimulationsAsync()
        {
            if (!_inited)
            {
                throw new Exception("StartMultipleSimulationsAsync() failed because the object has not be inited");
            }
            if (_continuousExperimentRunning)
            {
                _log.Trace("StartMultipleSimulationsAsync() failed because the object is already experimenting");
                return;
            }


            // To be pushed to the Tick function later

            _log.Trace("StartMultipleSimulationsAsync() about to call QASM");
            PolarisationResultList result = await _polarisationSimulatorService.Run(_atmosphericPolarisation_deg, _currentTelescopePolarisation_deg, ApiType.QASM_API, 100);
            _log.Trace("StartMultipleSimulationsAsync() got result from QASM");

            foreach (bool element in result.Results)
            {
                _log.Trace("StartMultipleSimulationsAsync() creating var for enqueing");
                var test = new KeyValuePair<int, bool>(_currentTelescopePolarisation_deg, element);
                _log.Trace("StartMultipleSimulationsAsync() enqueing var");
                _storedResults.Enqueue(new KeyValuePair<int, bool>(_currentTelescopePolarisation_deg, element));
                //_resultAccumulatorService.AddExperimentResult(_accumulators[test.Key], test.Value);
                //await Task.Delay(100);
                _log.Trace("StartMultipleSimulationsAsync() var enqued");
            }

            _log.Trace("StartMultipleSimulationsAsync() Setting up Timer");
            this.experimentTimer = new System.Threading.Timer(timerTick, null, 0, 100);
            _continuousExperimentRunning = true;
            _log.Trace("StartMultipleSimulationsAsync() Timer setup");


        }

        // TODO: NOT USED YET
        /*
        public void StartExperimenting(float Frequency)
        {
            _frequencyOfExperiment = Frequency;
            StartExperimenting();
        }
        */

        // TODO: NOT USED YET
        public void StopExperienting()
        {
            if (!_inited)
            {
                throw new Exception("StopExperienting() failed because the object has not be inited");
            }
            if (!_continuousExperimentRunning)
            {
                _log.Trace("StopExperienting() failed because the object is not experimenting");
                return;
            }
            _continuousExperimentRunning = false;
        }

        //private Queue<AngleResult> storedResults;
        private System.Threading.Timer experimentTimer;
        private Queue<KeyValuePair<int, bool>> _storedResults;
        private bool _fetchingForStoredResults = false;

        private void timerTick(Object state)
        {

            _log.Trace("timerTick() called");
            if (!_continuousExperimentRunning || _storedResults == null || _storedResults.Count < 1)
            {
                _log.Trace("timerTick() stoping internal timer & destroy unused experimental data");
                _storedResults.Clear();
                experimentTimer.Dispose();
                _continuousExperimentRunning = false;
                return;
            }

            KeyValuePair<int, bool> result = _storedResults.Dequeue();
            _log.Trace("timerTick() AddExperimentResult ");
            _resultAccumulatorService.AddExperimentResult(_accumulators[result.Key], result.Value);
            RaisePropertyChangedForExperiment();
        }
    }
}
