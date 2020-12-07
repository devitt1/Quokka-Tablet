﻿using System;
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

        public MvxAsyncCommand TriggerOneTimeRunCommand { get; private set; }
        public MvxCommand CloseModalCommand { get; private set; }

        public PlotViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, ISimulatorService simulatorService) : base(logProvider, navigationService)
        {
            _simulatorService = simulatorService;

            _results = new Result[360];
            for (var i = 0; i < _results.Length; i++)
            {
                _results[i] = new Result();
            }
            Step = 5;
            _telescopeAngle = 0;
            _atmosphereAngle = 30;
            _showCosOverlay = false;

            _runTimer = new Timer(1000);
            _runTimer.Elapsed += TimerCallback;
            _runTimer.AutoReset = true;

            TriggerOneTimeRunCommand = new MvxAsyncCommand(RunOneTime);
            CloseModalCommand = new MvxCommand(Close);
        }

        private PlotModel CreatePlotModel()
        {
            var FadedOrange = OxyColor.Parse("#E69975");
            var BrightOrange = OxyColor.Parse("#F47527");

            var model = new PlotModel()
            {
                PlotAreaBorderColor = OxyColors.Transparent,
            };

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
                TitleFontSize = 12,
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
                TitleFontSize = 12,
                MajorGridlineColor = OxyColor.FromAColor(128, FadedOrange),
                MajorGridlineThickness = 1,
                MajorGridlineStyle = LineStyle.Solid,
            };
            model.Axes.Add(yAxis);

            if(_showCosOverlay)
            {
                model.Series.Add(new FunctionSeries(PlotCosFunction, 0, 359, 0.1)
                {
                    Color = OxyColor.Parse("#B6CFE9"),
                    StrokeThickness = 3,
                    //Title = "Cos(x + 30°)",
                });
            }

            var points = new Collection<ScatterPoint>();
            for(var i = 0; i < _results.Length; i++)
            {
                if (_results[i].count > 0)
                {
                    points.Add(new ScatterPoint(i, _results[i].Averaged * 100));
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

            return model;
        }

        public PlotModel PhotonPlotModel => CreatePlotModel();

        private Result[] _results;
        public int Step;

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
            set
            {
                SetProperty(ref _progress, value);
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

            int filledCount = 0;
            int fillableCount = _results.Length / Step;
            for(var i = 0; i < _results.Length; i++)
            {
                if(_results[i].count > 0)
                {
                    filledCount++;
                }
            }
            Progress = (float) filledCount / fillableCount;

            await RaisePropertyChanged(() => PhotonPlotModel);
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

        private static double ToRad(double deg)
        {
            return deg * (Math.PI / 180.0);
        }

        private double PlotCosFunction(double x)
        {
            var cos = Math.Cos(ToRad(x + AtmosphereAngle));
            return (1 + cos) * 100 * 0.5;
        }
    }
}
