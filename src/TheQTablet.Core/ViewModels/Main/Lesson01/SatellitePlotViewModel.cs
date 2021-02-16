using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Timers;
using Acr.UserDialogs;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.Utils;

namespace TheQTablet.Core.ViewModels.Main.Lesson01
{
    public class SatellitePlotViewModel : MvxNavigationViewModel
    {
        private ISimulatorService _simulatorService;
        private readonly IUserDialogs _userDialogs;

        private bool _errorDialogVisible;

        public MvxAsyncCommand TriggerOneTimeRunCommand { get; private set; }
        public MvxCommand CloseModalCommand { get; private set; }

        public SatellitePlotViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, ISimulatorService simulatorService, IUserDialogs userDialogs) : base(logProvider, navigationService)
        {
            _simulatorService = simulatorService;
            _userDialogs = userDialogs;
            _results_telescope = new Result[360];
            for (var i = 0; i < _results_telescope.Length; i++)
            {
                _results_telescope[i] = new Result();
            }

            _results_satellite = new Result[360];
            for (var i = 0; i < _results_satellite.Length; i++)
            {
                _results_satellite[i] = new Result();
            }
            Step = 10;
            _telescopeAngle = 0;
            _satelliteAngle = 0;
            _atmosphereAngle = 30;
            _showCosOverlay = false;

            TriggerOneTimeRunCommand = new MvxAsyncCommand(RunOneTime);
            CloseModalCommand = new MvxCommand(Close);
        }

        private PlotModel CreatePlotModel()
        {
            var FadedOrange = OxyColor.Parse("#E69975");
            var BrightOrange = OxyColor.Parse("#F47527");
            var BlueColor = OxyColor.Parse("#9AB5D9");

            var model = new PlotModel()
            {
                PlotAreaBorderColor = OxyColors.Transparent,
                DefaultFont = "Akrobat-Regular.ttf",
            };

            var titleFontSize = 18;
            var fontSize = 18;

            var xAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Angle",
                Minimum = -5, // Padding to show 0 values
                Maximum = 365,
                MajorStep = 30,
                TickStyle = TickStyle.None,
                AxisTickToLabelDistance = 22,
                AxisTitleDistance = 31,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = FadedOrange,
                AxislineThickness = 2,
                TextColor = FadedOrange,
                TitleColor = OxyColors.White,
                TitleFontWeight = FontWeights.Bold,
                TitleFontSize = titleFontSize,
                FontSize = fontSize,
            };
            model.Axes.Add(xAxis);

            var yAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Photons Collected",
                Minimum = -5, // Padding to show 0 values
                Maximum = 105,
                MajorStep = 25,
                TickStyle = TickStyle.None,
                AxisTickToLabelDistance = 22,
                AxisTitleDistance = 31,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = FadedOrange,
                AxislineThickness = 2,
                TextColor = FadedOrange,
                TitleColor = OxyColors.White,
                TitleFontWeight = FontWeights.Bold,
                TitleFontSize = titleFontSize,
                FontSize = fontSize,
                MajorGridlineColor = OxyColor.FromAColor(128, FadedOrange),
                MajorGridlineThickness = 1,
                MajorGridlineStyle = LineStyle.Solid,
            };
            model.Axes.Add(yAxis);

            if (_showCosOverlay)
            {
                model.Series.Add(new FunctionSeries(PlotCosSquaredFunction, 0, 359, 0.1)
                {
                    Color = OxyColor.Parse("#B6CFE9"),
                    StrokeThickness = 3,
                    //Title = "Cos²(x + 30°)",
                });
            }

            var points = new Collection<ScatterPoint>();
            for (var i = 0; i < _results_telescope.Length; i++)
            {
                if (_results_telescope[i].count > 0)
                {
                    points.Add(new ScatterPoint(i, _results_telescope[i].Averaged * 100));
                }
            }
            var scatterSeries = new ScatterSeries()
            {
                ItemsSource = points,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = BrightOrange,
                //Title = "Photons",
            };
            model.Series.Add(scatterSeries);

            var poinSatellites = new Collection<ScatterPoint>();
            for (var i = 0; i < _results_satellite.Length; i++)
            {
                if (_results_satellite[i].count > 0)
                {
                    poinSatellites.Add(new ScatterPoint(i, _results_satellite[i].Averaged * 100));
                }
            }

            var scatterSatelliteSeries = new ScatterSeries()
            {
                ItemsSource = poinSatellites,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = BlueColor,
                //Title = "Photons",
            };

            model.Series.Add(scatterSatelliteSeries);
            return model;
        }

        public PlotModel PhotonPlotModel => CreatePlotModel();

        private Result[] _results_telescope;
        private Result[] _results_satellite;
        public int Step;

        private int _telescopeAngle;
        public int TelescopeAngle
        {
            get => _telescopeAngle;
            set => SetProperty(ref _telescopeAngle, AsAngle(value));
        }

        private int _satelliteAngle;
        public int SatelliteAngle
        {
            get => _satelliteAngle;
            set => SetProperty(ref _satelliteAngle, AsAngle(value));
        }

        private int _atmosphereAngle;
        public int AtmosphereAngle
        {
            get => _atmosphereAngle;
            set => SetProperty(ref _atmosphereAngle, AsAngle(value));
        }

        private bool _showCosOverlay;
        public bool ShowCosOverlay
        {
            get => _showCosOverlay;
            set
            {
                _showCosOverlay = value;
                RaisePropertyChanged(() => PhotonPlotModel);
            }
        }

        private float _progress;
        public float Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private Timer _runTimer;

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            _runTimer = new Timer(200);
            _runTimer.Elapsed += TimerCallback;
            _runTimer.AutoReset = true;
            _runTimer.Start();
        }

        public override void ViewDisappearing()
        {
            _runTimer.Elapsed -= TimerCallback;
            _runTimer.Stop();

            base.ViewDisappearing();
        }

        private async Task RunOneTime()
        {
            var result = await _simulatorService.RunQASMAsync(AtmosphereAngle, TelescopeAngle,SatelliteAngle,1);

           
            if (!result.Error.Equals("no error"))
            {
                _runTimer.Stop();

                if (!_errorDialogVisible)
                {
                    _errorDialogVisible = true;
                    await _userDialogs.AlertAsync(new AlertConfig()
                    {
                        Title = "Connection Error",
                        Message = "Sorry, something went wrong.\nPlease check the connection with your Quantum Computer.",
                        OkText = "OK, Start Again"

                    });
                    _errorDialogVisible = false;

                    CloseModalCommand.Execute();
                }
            }
            else
            {
                var resultValue = result.Results[0];
                _results_telescope[TelescopeAngle].total += resultValue[0] ? 1 : 0;
                _results_telescope[TelescopeAngle].count++;

                int filledCount = 0;
                int fillableCount = _results_telescope.Length / Step;
                for (var i = 0; i < _results_telescope.Length; i++)
                {
                    if (_results_telescope[i].count > 0)
                    {
                        filledCount++;
                    }
                }

                var resultSatelliteValue = result.Results[0];
                _results_satellite[SatelliteAngle].total += resultValue[1] ? 1 : 0;
                _results_satellite[SatelliteAngle].count++;

                int filledSatellite = 0;
                int fillableSatellite = _results_satellite.Length / Step;
                for (var i = 0; i < _results_satellite.Length; i++)
                {
                    if (_results_satellite[i].count > 0)
                    {
                        filledSatellite++;
                    }
                }


                Progress = (float)filledCount / fillableCount;

                if (Progress == 1)
                {
                    _runTimer.Stop();

                    await _userDialogs.AlertAsync(new AlertConfig()
                    {
                        Title = "Collection Completed",
                        Message = "Great! you have collected enough photons, let’s continue exploring the night sky",
                        OkText = "OK"

                    });

                    CloseModalCommand.Execute();

                }

                await RaisePropertyChanged(() => PhotonPlotModel);
            }
        }

        private async void TimerCallback(object source, ElapsedEventArgs args)
        {
            await RunOneTime();
        }

        private void Close()
        {
            NavigationService.Close(this);
        }

        private static int AsAngle(int value)
        {
            var remainder = value % 360;
            if (remainder < 0)
            {
                remainder += 360;
            }
            return remainder;
        }

        private double PlotCosSquaredFunction(double x)
        {
            var cos = Math.Cos(MathHelpers.ToRad(x + AtmosphereAngle));
            return (cos * cos) * 100;
        }
    }
}


