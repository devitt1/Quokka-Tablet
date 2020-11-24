// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using MvvmCross.Platforms.Ios.Views;
using TheQTablet.Core.ViewModels.Main;
using UIKit;

namespace TheQTablet.iOS
{
	[MvxFromStoryboard("Main")]
	public partial class QuantumComputingInfoView : MvxViewController<QuantumComputingInfoViewModel>
	{
        private UIButton _nextButton;

        public QuantumComputingInfoView (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _nextButton = ButtonGenerator.DarkButton("CONTINUE");

            this.View.AddSubview(_nextButton);
            _nextButton.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -46).Active = true;
            _nextButton.RightAnchor.ConstraintEqualTo(View.RightAnchor, -46).Active = true;

            var set = CreateBindingSet();
            set.Bind(_nextButton).For("TouchUpInside").To(vm => vm.StartSimulationCommand);
            set.Apply();
        }
    }
}