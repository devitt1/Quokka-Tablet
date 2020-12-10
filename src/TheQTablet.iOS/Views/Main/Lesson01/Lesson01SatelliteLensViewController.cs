using System;
using System.Drawing;
using System.Globalization;
using CoreGraphics;
using Foundation;
using MvvmCross.Converters;
using MvvmCross.Platforms.Ios.Views;
using SceneKit;
using TheQTablet.Core.ViewModels.Main;
using TheQTablet.Core.ViewModels.Main.Lesson01;
using TheQTablet.iOS.Views.Custom;
using UIKit;

namespace TheQTablet.iOS.Views.Main
{
    public partial class Lesson01SatelliteLensViewController : Lesson01BaseViewController<Lesson01SatelliteLensViewModel>
    {
        private UIImageView _background;

        private UILabel _signalStrengthText;

        private UIView _infoTextContainer;
        private UILabel _infoText;
        private UIButton _continue;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _background = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("lens_wave_background"),
            };
            View.AddSubview(_background);

            _signalStrengthText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Bold),
                TextColor = ColorPalette.SecondaryText,
                Text = "QUANTUM SENSOR",
            };
            View.AddSubview(_signalStrengthText);

            _infoTextContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White.ColorWithAlpha(0.5f),
            };
            _infoTextContainer.Layer.CornerRadius = 20;
            View.AddSubview(_infoTextContainer);

            _infoText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "The photons from the star pass through the telescope lens",
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            _infoTextContainer.AddSubview(_infoText);

            _continue = ButtonGenerator.PrimaryButton("Continue");
            View.AddSubview(_continue);


            _background.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(_background.WidthAnchor, _background.Image.Size.Height / _background.Image.Size.Width).Active = true;

            _signalStrengthText.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 32).Active = true;
            _signalStrengthText.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -32).Active = true;

            _infoTextContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor, 50).Active = true;
            _infoTextContainer.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

            _infoText.TopAnchor.ConstraintEqualTo(_infoTextContainer.TopAnchor, 20).Active = true;
            _infoText.BottomAnchor.ConstraintEqualTo(_infoTextContainer.BottomAnchor, -20).Active = true;
            _infoText.CenterXAnchor.ConstraintEqualTo(_infoTextContainer.CenterXAnchor).Active = true;

            _infoTextContainer.WidthAnchor.ConstraintGreaterThanOrEqualTo(_infoText.WidthAnchor, 1, 40).Active = true;

            _continue.BottomAnchor.ConstraintEqualTo(_signalStrengthText.CenterYAnchor).Active = true;
            _continue.RightAnchor.ConstraintEqualTo(View.RightAnchor, -32).Active = true;

            var set = CreateBindingSet();
            set.Bind(_continue).To(vm => vm.ContinueCommand);
            set.Apply();
        }
    }
}

