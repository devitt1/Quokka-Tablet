using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Timers;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using TheQTablet.Core.Service.Interfaces;

namespace TheQTablet.Core.ViewModels.Main
{
    public class Result
    {
        public int count;
        public float total;
        public float Averaged
        {
            get => (count > 0) ? (total / count) : 0;
        }

        public Result()
        {
            count = 0;
            total = 0;
        }
    }

    public class PlotViewModel : MvxNavigationViewModel
    {
        private ISimulatorService _simulatorService;

        public MvxAsyncCommand TriggerOneTimeRun { get; private set; }

        public PlotViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, ISimulatorService simulatorService) : base(logProvider, navigationService)
        {
            _simulatorService = simulatorService;

            _results = new Result[360];
            for (var i = 0; i < _results.Length; i++)
            {
                _results[i] = new Result();
            }
            _telescopeAngle = 0;
            _atmosphereAngle = 30;

            _runTimer = new Timer(1000);
            _runTimer.Elapsed += TimerCallback;
            _runTimer.AutoReset = true;

            TriggerOneTimeRun = new MvxAsyncCommand(RunOneTime);
        }

        private PlotModel CreatePlotModel()
        {
            var model = new PlotModel()
            {
                PlotAreaBorderColor = OxyColors.Transparent,
            };
            var xAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "θ ANGLE",
                Minimum = -5, // Padding to show 0 values
                Maximum = 365,
                MajorStep = 30,
                TickStyle = TickStyle.None,
                AxisTickToLabelDistance = 22,
                AxisTitleDistance = 31,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = OxyColors.White,
                AxislineThickness = 2,
                TitleColor = OxyColor.Parse("#40506B"),
                TitleFontWeight = FontWeights.Bold,
                TitleFontSize = 12,
            };
            model.Axes.Add(xAxis);

            var yPadding = 0.1;
            var yAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "PHOTONS COLLECTED",
                Minimum = -yPadding, // Padding to show 0 values
                Maximum = 1 + yPadding,
                //MajorStep = 25,
                TickStyle = TickStyle.None,
                AxisTickToLabelDistance = 22,
                AxisTitleDistance = 31,
                AxislineStyle = LineStyle.Solid,
                AxislineColor = OxyColors.White,
                AxislineThickness = 2,
                TitleColor = OxyColor.Parse("#40506B"),
                TitleFontWeight = FontWeights.Bold,
                TitleFontSize = 12,
            };
            model.Axes.Add(yAxis);

            var points = new Collection<ScatterPoint>();
            for(var i = 0; i < _results.Length; i++)
            {
                if (_results[i].count > 0)
                {
                    points.Add(new ScatterPoint(i, _results[i].Averaged));
                }
            }
            var scatterSeries = new ScatterSeries()
            {
                ItemsSource = points,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColor.Parse("#5B9CEF"),
            };
            model.Series.Add(scatterSeries);

            return model;
        }

        public PlotModel PhotonPlotModel => CreatePlotModel();

        private Result[]_results;
        private int _telescopeAngle;
        public int TelescopeAngle
        {
            get => _telescopeAngle;
            set
            {
                SetProperty(ref _telescopeAngle, AsAngle(value));
            }
        }

        private int _atmosphereAngle;
        public int AtmosphereAngle
        {
            get => _atmosphereAngle;
            set
            {
                SetProperty(ref _atmosphereAngle, AsAngle(value));
            }
        }

        private Timer _runTimer;

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            _runTimer.Start();
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();

            _runTimer.Stop();
        }

        private async Task RunOneTime()
        {
            var result = await _simulatorService.Run(AtmosphereAngle, TelescopeAngle, DataModel.ApiType.QASM_API);

            _results[TelescopeAngle].total += result ? 1 : 0;
            _results[TelescopeAngle].count++;

            await RaisePropertyChanged(() => PhotonPlotModel);
        }

        private async void TimerCallback(object source, ElapsedEventArgs args)
        {
            await RunOneTime();
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
    }
}
