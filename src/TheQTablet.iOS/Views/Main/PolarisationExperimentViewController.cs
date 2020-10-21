using Foundation;
using System;

using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;

using OxyPlot.Xamarin.iOS;

using Cirrious.FluentLayouts.Touch;

using UIKit;

using TheQTablet.Core.ViewModels.Main;
using TheQTablet.iOS.Views;

namespace TheQTablet.iOS.Views.Main
{
    [MvxChildPresentation]
    public partial class PolarisationExperimentViewController : BaseViewController<PolarisationExperimentViewModel>
    {
        private UILabel
            _initialLabel,
            _experimentalResultMeanStaticLabel,
            _averageNumberOfPhotonCapturedLabel,
            _numberOfExperimentsStaticLabel,
            _numberOfExperimentsLabel,
            _telescopeRotationStaticLabel,
            _telescopeRotationLabel,
            _numberOfCapturedPhotonsStaticLabel,
            _numberOfCapturedPhotonsLabel;
        public UISlider _currentTelescopeFilterRotationSlider;
        //private UIStepper _telescopeRotationStepper;
        public UIButton RunOneExperiment, RunMultipleExperiments;

        private PlotView _plotView;

        protected override void CreateView()
        {
            _plotView = new PlotView();
            _plotView.BackgroundColor = UIColor.Clear;

            View.AddSubviews(_plotView);
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();


            _initialLabel = new UILabel
            {
                Text = "This is the Polarisation Experiment Page",
                TextAlignment = UITextAlignment.Center
            };
            Add(_initialLabel);

            _experimentalResultMeanStaticLabel = new UILabel
            {
                Text = "Mean Experimental Result",
                TextAlignment = UITextAlignment.Center
            };
            Add(_experimentalResultMeanStaticLabel);
            _averageNumberOfPhotonCapturedLabel = new UILabel
            {
                Text = "UnsetERM",
                TextAlignment = UITextAlignment.Center
            };
            Add(_averageNumberOfPhotonCapturedLabel);

            _numberOfExperimentsStaticLabel = new UILabel
            {
                Text = "# of experiments done",
                TextAlignment = UITextAlignment.Center
            };
            Add(_numberOfExperimentsStaticLabel);
            _numberOfExperimentsLabel = new UILabel
            {
                Text = "UnsetNOE",
                TextAlignment = UITextAlignment.Center
            };
            Add(_numberOfExperimentsLabel);

            _currentTelescopeFilterRotationSlider = new UISlider
            {
                MaxValue = 100,
                MinValue = 0,
                Value = 0
            };
            Add(_currentTelescopeFilterRotationSlider);

            _telescopeRotationStaticLabel = new UILabel
            {
                Text = "Current Telescope Polarisation Filter",
                TextAlignment = UITextAlignment.Center
            };
            Add(_telescopeRotationStaticLabel);
            _telescopeRotationLabel = new UILabel
            {
                Text = "UnsetRotation",
                TextAlignment = UITextAlignment.Center
            };
            Add(_telescopeRotationLabel);

            _numberOfCapturedPhotonsStaticLabel = new UILabel
            {
                Text = "Number of Captured Photons",
                TextAlignment = UITextAlignment.Center
            };
            Add(_numberOfCapturedPhotonsStaticLabel);

            _numberOfCapturedPhotonsLabel = new UILabel
            {
                Text = "UnsetNOP",
                TextAlignment = UITextAlignment.Center
            };
            Add(_numberOfCapturedPhotonsLabel);

            RunOneExperiment = new UIButton(UIButtonType.RoundedRect);
            RunOneExperiment.SetTitle("OneSimulation", UIControlState.Normal);
            RunOneExperiment.SetTitle("OneSimulation", UIControlState.Highlighted);
            RunOneExperiment.SetTitle("OneSimulation", UIControlState.Selected);
            Add(RunOneExperiment);


            RunMultipleExperiments = new UIButton(UIButtonType.RoundedRect);
            RunMultipleExperiments.SetTitle("MultipleSimulations", UIControlState.Normal);
            RunMultipleExperiments.SetTitle("MultipleSimulations", UIControlState.Highlighted);
            RunMultipleExperiments.SetTitle("MultipleSimulations", UIControlState.Selected);
            Add(RunMultipleExperiments);

        }
        protected override void LayoutView()
        {
            View.AddConstraints(new FluentLayout[]
           {
                _initialLabel.WithSameCenterX(View),
                _initialLabel.AtTopOf(View, 5.0f),

                _currentTelescopeFilterRotationSlider.Below(_initialLabel),
                _currentTelescopeFilterRotationSlider.AtLeftOf(View, 0.0f),
                _currentTelescopeFilterRotationSlider.WithSameWidth(View),
                _telescopeRotationStaticLabel.Below(_currentTelescopeFilterRotationSlider),
                _telescopeRotationStaticLabel.AtLeftOf(View, 10.0f),
                _telescopeRotationLabel.WithSameTop(_telescopeRotationStaticLabel),
                _telescopeRotationLabel.ToRightOf(_telescopeRotationStaticLabel, 30.0f),

                _experimentalResultMeanStaticLabel.Below(_telescopeRotationStaticLabel),
                _experimentalResultMeanStaticLabel.WithSameLeft(_telescopeRotationStaticLabel),
                _averageNumberOfPhotonCapturedLabel.WithSameTop(_experimentalResultMeanStaticLabel),
                _averageNumberOfPhotonCapturedLabel.WithSameLeft(_telescopeRotationLabel),

                _numberOfExperimentsStaticLabel.Below(_experimentalResultMeanStaticLabel),
                _numberOfExperimentsStaticLabel.WithSameLeft(_experimentalResultMeanStaticLabel),
                _numberOfExperimentsLabel.WithSameTop(_numberOfExperimentsStaticLabel),
                _numberOfExperimentsLabel.WithSameLeft(_telescopeRotationLabel),

                _numberOfCapturedPhotonsStaticLabel.Below(_numberOfExperimentsStaticLabel),
                _numberOfCapturedPhotonsStaticLabel.WithSameLeft(_numberOfExperimentsStaticLabel),
                _numberOfCapturedPhotonsLabel.WithSameTop(_numberOfCapturedPhotonsStaticLabel),
                _numberOfCapturedPhotonsLabel.WithSameLeft(_telescopeRotationLabel),

                RunOneExperiment.WithSameCenterX(View),
                RunOneExperiment.Below(_numberOfExperimentsLabel, 20f),
                //RunOneExperiment.WithSameWidth(View),
                //RunMultipleExperiments.WithSameCenterX(View),
                RunMultipleExperiments.WithSameTop(RunOneExperiment),

                RunMultipleExperiments.ToRightOf(RunOneExperiment, 20.0f),

                _plotView.Below(RunMultipleExperiments),
                _plotView.WithSameWidth(View),
                _plotView.AtLeftOf(View),
                _plotView.AtRightOf(View),
                _plotView.AtBottomOf(View)

           });

        }

        protected override void BindView()
        {
            base.BindView();

            var set = this.CreateBindingSet<PolarisationExperimentViewController, PolarisationExperimentViewModel>();
            set.Bind(RunOneExperiment).For("TouchUpInside").To(vm => vm.StartSimulationCommand);
            set.Bind(RunMultipleExperiments).For("TouchUpInside").To(vm => vm.StartMultipleSimulationsCommand);
            set.Bind(_telescopeRotationLabel).To(vm => vm.TelescopePolarisation);
            set.Apply();
            set = this.CreateBindingSet<PolarisationExperimentViewController, PolarisationExperimentViewModel>();
            set.Bind(_currentTelescopeFilterRotationSlider).To(vm => vm.CurrentTelescopeFilterRotationSlider);            
            set.Bind(_averageNumberOfPhotonCapturedLabel).To(vm => vm.AverageNumberOfPhotonCaptured);
            set.Bind(_numberOfExperimentsLabel).To(vm => vm.NumberOfExperiments);
            set.Bind(_numberOfCapturedPhotonsLabel).To(vm => vm.NumberOfCapturedPhotons);
            set.Bind(_plotView).For(v => v.Model).To(vm => vm.PlotModel);

            set.Apply();


        }
        /*
        public PolarisationExperimentViewController (IntPtr handle) : base (handle)
        {
        }
        */
    }
}