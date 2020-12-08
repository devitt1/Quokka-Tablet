using System;
using System.Drawing;
using System.Globalization;
using CoreGraphics;
using Foundation;
using MvvmCross.Converters;
using MvvmCross.Platforms.Ios.Views;
using TheQTablet.Core.ViewModels.Main;
using TheQTablet.iOS.Views.Custom;
using UIKit;

namespace TheQTablet.iOS.Views.Main
{
    public class PositionConverter : MvxValueConverter<PointF, CGPoint>
    {
        protected override CGPoint Convert(PointF value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return new CGPoint(value.X, value.Y);
        }

        protected override PointF ConvertBack(CGPoint value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return new PointF((float) value.X, (float) value.Y);
        }
    }

    public partial class TelescopeSearchViewController : BaseViewController<TelescopeSearchViewModel>
    {
        private UIImageView _background;
        private MaskedView _lensMask;
        private UIImageView _highlightedBackground;
        private DraggableView _lens;
        private UIImageView _lensImage;

        private UILabel _signalStrengthText;
        private InsetProgressBar _signalStrength;

        private UIView _goodSignalContainer;
        private UILabel _goodSignalText;
        private UIButton _continue;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _background = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("stars_background"),
            };
            View.AddSubview(_background);

            _lensMask = new MaskedView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_lensMask);

            _highlightedBackground = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("stars_background_highlighted"),
            };
            _lensMask.AddSubview(_highlightedBackground);

            _lens = new DraggableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_lens);

            _lensImage = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("lens"),
            };
            _lens.AddSubview(_lensImage);

            _signalStrengthText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Bold),
                TextColor = ColorPalette.SecondaryText,
                Text = "SIGNAL STRENGTH",
            };
            View.AddSubview(_signalStrengthText);

            _signalStrength = new InsetProgressBar
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TrackImage = UIImage.FromBundle("progress_track_dark_vertical"),
                ProgressImage = UIImage.FromBundle("progress_inner_vertical"),
                Inset = 7,
                Horizontal = false,
            };
            View.AddSubview(_signalStrength);

            _goodSignalContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White.ColorWithAlpha(0.5f),
            };
            _goodSignalContainer.Layer.CornerRadius = 20;
            View.AddSubview(_goodSignalContainer);

            _goodSignalText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "This star has a good signal, tap Continue to move on",
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            _goodSignalContainer.AddSubview(_goodSignalText);

            _continue = ButtonGenerator.DarkButton("Continue");
            View.AddSubview(_continue);

            _lens.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _lens.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;
            _lens.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _lens.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;

            _lensImage.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.3f).Active = true;
            _lensImage.HeightAnchor.ConstraintEqualTo(_lensImage.WidthAnchor).Active = true;

            _background.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(_background.WidthAnchor, _background.Image.Size.Height / _background.Image.Size.Width).Active = true;

            _highlightedBackground.WidthAnchor.ConstraintEqualTo(_background.WidthAnchor).Active = true;
            _highlightedBackground.HeightAnchor.ConstraintEqualTo(_background.HeightAnchor).Active = true;
            _highlightedBackground.LeftAnchor.ConstraintEqualTo(_background.LeftAnchor).Active = true;
            _highlightedBackground.TopAnchor.ConstraintEqualTo(_background.TopAnchor).Active = true;

            _lensMask.CenterXAnchor.ConstraintEqualTo(_lensImage.CenterXAnchor).Active = true;
            _lensMask.CenterYAnchor.ConstraintEqualTo(_lensImage.CenterYAnchor).Active = true;
            _lensMask.WidthAnchor.ConstraintEqualTo(_lensImage.WidthAnchor).Active = true;
            _lensMask.HeightAnchor.ConstraintEqualTo(_lensImage.HeightAnchor).Active = true;

            _signalStrengthText.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 32).Active = true;
            _signalStrengthText.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -32).Active = true;

            _signalStrength.BottomAnchor.ConstraintEqualTo(_signalStrengthText.TopAnchor, -10).Active = true;
            _signalStrength.CenterXAnchor.ConstraintEqualTo(_signalStrengthText.CenterXAnchor).Active = true;
            _signalStrength.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.5f).Active = true;
            _signalStrength.WidthAnchor.ConstraintEqualTo(_signalStrength.HeightAnchor, _signalStrength.TrackImage.Size.Width / _signalStrength.TrackImage.Size.Height).Active = true;

            _goodSignalContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor, 50).Active = true;
            _goodSignalContainer.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

            _goodSignalText.TopAnchor.ConstraintEqualTo(_goodSignalContainer.TopAnchor, 20).Active = true;
            _goodSignalText.BottomAnchor.ConstraintEqualTo(_goodSignalContainer.BottomAnchor, -20).Active = true;
            _goodSignalText.CenterXAnchor.ConstraintEqualTo(_goodSignalContainer.CenterXAnchor).Active = true;

            _goodSignalContainer.WidthAnchor.ConstraintGreaterThanOrEqualTo(_goodSignalText.WidthAnchor, 1, 40).Active = true;

            _continue.BottomAnchor.ConstraintEqualTo(_signalStrengthText.CenterYAnchor).Active = true;
            _continue.RightAnchor.ConstraintEqualTo(View.RightAnchor, -32).Active = true;

            var set = CreateBindingSet();
            set.Bind(_lens).For(v => v.Position).To(vm => vm.LensPosition).WithConversion<PositionConverter>();
            set.Bind(_signalStrength).For(v => v.Progress).To(vm => vm.SignalStrength);
            set.Bind(_continue).For("Visible").To(vm => vm.StarFound);
            set.Bind(_goodSignalContainer).For("Visible").To(vm => vm.StarFound);
            set.Apply();
        }
    }
}

