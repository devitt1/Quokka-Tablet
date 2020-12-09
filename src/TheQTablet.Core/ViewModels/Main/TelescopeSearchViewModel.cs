using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using TheQTablet.Core.ViewModels.Main.Lesson01;

namespace TheQTablet.Core.ViewModels.Main
{
    public class TelescopeSearchViewModel : LessonBaseViewModel
    {
        public MvxAsyncCommand ContinueCommand { get; private set; }

        public TelescopeSearchViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            _starPositions = new List<PointF>
            {
                new PointF(0.25f, 0.23f),
                new PointF(0.5f, 0.58f),
                new PointF(0.34f, 0.79f),
                new PointF(0.77f, 0.53f),
            };

            SignalStrength = 0;
            LensPosition = new PointF(0.5f, 0.5f);

            ContinueCommand = new MvxAsyncCommand(Continue);
        }
        
        private List<PointF> _starPositions;

        private PointF _lensPosition;
        public PointF LensPosition
        {
            get => _lensPosition;
            set
            {
                SetProperty(ref _lensPosition, value);
                CalculateSignalStrength();
            }
        }

        private float _signalStrength;
        public float SignalStrength
        {
            get => _signalStrength;
            set => SetProperty(ref _signalStrength, value);
        }
        private bool _starFound;
        public bool StarFound
        {
            get => _starFound;
            private set {
                SetProperty(ref _starFound, value);
            }
        }

        private void CalculateSignalStrength()
        {
            List<double> distances = _starPositions.ConvertAll((starPosition) => Distance(starPosition, LensPosition));

            var minDistance = MinDistance(distances);

            StarFound = minDistance < 0.05;

            SignalStrength = 1.0f - (float) minDistance;
        }

        private static double Distance(PointF a, PointF b)
        {
            return Math.Sqrt(Sq(a.X - b.X) + Sq(a.Y - b.Y));
        }

        private static double Sq(double value)
        {
            return value * value;
        }

        private static double MinDistance(List<double> list)
        {
            if (list.Count == 0)
                return 0;
            double min = list[0];
            for(var i = 1; i < list.Count; i++)
            {
                if (list[i] < min)
                    min = list[i];
            }
            return min;
        }

        private async Task Continue()
        {
            await NavigationService.Navigate<PlotViewModel>();
        }
    }
}
