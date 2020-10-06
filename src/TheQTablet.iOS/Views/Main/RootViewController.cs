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
    public partial class RootViewController : BaseViewController<RootViewModel>
    {
        private UILabel _labelWelcome, _labelMessage;
        public  UIButton StartSimulationButton;

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

        protected override void BindView()
        {
            base.BindView();

            var set = this.CreateBindingSet<RootViewController, RootViewModel>();
            set.Bind(StartSimulationButton).For("TouchUpInside").To(vm => vm.StartSimulationCommand);
            set.Apply();

        }

        
    }
}
