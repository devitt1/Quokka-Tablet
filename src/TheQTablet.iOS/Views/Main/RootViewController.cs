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

namespace TheQTablet.iOS
{
    [MvxRootPresentation(WrapInNavigationController = true)]
    public partial class RootViewController : BaseViewController<RootViewModel>
    {
        private UILabel _labelWelcome, _labelMessage;
        private UIButton StartSimulationButton;

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
                Text = "The Q Simulator",
                TextAlignment = UITextAlignment.Center
            };
            Add(_labelMessage);


            StartSimulationButton = new UIButton(UIButtonType.RoundedRect);
            StartSimulationButton.SetTitle("Light Polarisation Lesson", UIControlState.Normal);
            StartSimulationButton.SetTitle("Light Polarisation Lesson", UIControlState.Highlighted);
            StartSimulationButton.SetTitle("Light Polarisation Lesson", UIControlState.Selected);
            Add(StartSimulationButton);

        }

        protected override void LayoutView()
        {
            View.AddConstraints(new FluentLayout[]
           {
                _labelWelcome.WithSameCenterX(View),
                _labelWelcome.WithSameCenterY(View),

                _labelMessage.Below(_labelWelcome, 10f),
                _labelMessage.WithSameWidth(View),

                StartSimulationButton.WithSameCenterX(View),
                StartSimulationButton.Below(_labelMessage,10f),
                StartSimulationButton.WithSameWidth(_labelMessage)
           });

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        protected override void BindView()
        {
            base.BindView();

            var set = this.CreateBindingSet<RootViewController, RootViewModel>();
            set.Bind(StartSimulationButton).For("TouchUpInside").To(vm => vm.NavigateToPolarisationExperimentCommand);
            set.Apply();

        }
    }
}
