using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmCross.Binding.BindingContext;
using Cirrious.FluentLayouts.Touch;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using TheQTablet.Core.ViewModels.Main;
using UIKit;

namespace TheQTablet.iOS.Views.Main
{
    [MvxRootPresentation(WrapInNavigationController = true)]
    //    public partial class RootViewController : BaseViewController<RootViewModel>
    public partial class RootViewController : BaseViewController<PolarisationExperimentViewModel>
    {
        private UILabel _labelWelcome, _labelMessage;
        private UIButton _btnRunSimulation;
        public UIButton StartSimulationButton;

        protected override void CreateView()
        {
            _labelWelcome = new UILabel
            {
                Text = "Welcome!!",
                TextAlignment = UITextAlignment.Center
            };
            Add(_labelWelcome);

            _labelMessage = new UILabel
            {
                Text = "App scaffolded with MvxScaffolding",
                TextAlignment = UITextAlignment.Center
            };
            Add(_labelMessage);


            StartSimulationButton = new UIButton(UIButtonType.RoundedRect);
            StartSimulationButton.SetTitle("Start", UIControlState.Normal);
            StartSimulationButton.SetTitle("Start", UIControlState.Highlighted);
            StartSimulationButton.SetTitle("Start", UIControlState.Selected);
            Add(StartSimulationButton);

            // Initialise the ViewModel
            //((PolarisationExperimentViewModel)BindingContext).Init2(5, 0, 0);

            _btnRunSimulation = new UIButton
            {
                BackgroundColor = UIColor.Red
            };
            _btnRunSimulation.Layer.CornerRadius = 8f;
            _btnRunSimulation.SetTitle("Do One Single Simulation", UIControlState.Normal);
            _btnRunSimulation.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnRunSimulation.SetTitleColor(UIColor.LightGray, UIControlState.Selected);
            Add(_btnRunSimulation);

        }

        protected override void LayoutView()
        {
            View.AddConstraints(new FluentLayout[]
           {
                _labelWelcome.WithSameCenterX(View),
                _labelWelcome.WithSameCenterY(View),

                _labelMessage.Below(_labelWelcome, 10f),
                _labelMessage.WithSameWidth(View),

                _btnRunSimulation.Below(_labelMessage, 10f),
                _btnRunSimulation.WithSameWidth(View),

                StartSimulationButton.WithSameCenterX(View),
                StartSimulationButton.Below(_btnRunSimulation,10f),
                StartSimulationButton.WithSameWidth(_labelMessage)
           });

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<RootViewController, PolarisationExperimentViewModel>();
            //set.Bind(_btnRunSimulation).For("TouchUpInside").To(vm => vm.StartOnePolarisationSimulationCommand);
            set.Bind(_btnRunSimulation).To(vm => vm.StartOnePolarisationSimulationCommand);
            set.Apply();

        }

        protected override void BindView()
        {
            base.BindView();

            var set = this.CreateBindingSet<RootViewController, PolarisationExperimentViewModel>();
            set.Bind(StartSimulationButton).For("TouchUpInside").To(vm => vm.StartSimulationCommand);
            set.Apply();

        }
    }
}
