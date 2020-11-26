using System;
using System.Globalization;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Converters;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using OxyPlot.Xamarin.iOS;
using TheQTablet.Core.ViewModels.Main;
using TheQTablet.iOS.Views.Custom;
using UIKit;

namespace TheQTablet.iOS.Views.Main
{
    public class AngleDisplayConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return value + "°";
        }
    }

    public class DegreesToRadiansConverter : MvxValueConverter<int, float>
    {
        protected override float Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return (float) (value * ((float)Math.PI / 180.0f));
        }

        protected override int ConvertBack(float value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) (value * (180.0f / (float)Math.PI));
        }
    }

    [MvxChildPresentation]
    public partial class PlotViewController : MvxViewController<PlotViewModel>
    {
        private UILabel _heading;
        private UIView _plotContainer;
        private UIStackView _plotHeader;
        private UIStackView _plotHeaderCircuit;
        private UILabel _plotHeaderCircuitTitle;
        private UIImageView _plotHeaderCircuitImage;
        private UILabel _plotHeaderTitle;
        private UILabel _plotHeaderProgress;
        private PlotView _plotView;
        private UIStackView _knobContainer;
        private UILabel _knobHeader;
        private KnobView _knobControl;
        //private UILabel _telescopeAngle;
        private UIView _sceneView;

        private CAGradientLayer _viewGradient;
        private CAGradientLayer _plotGradient;
        private CAGradientLayer _knobContainerGradient;

        public PlotViewController()
        {
            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            _viewGradient = new CAGradientLayer();
            _viewGradient.Colors = new CGColor[] {
                ColorPalette.BackgroundLight.CGColor,
                ColorPalette.BackgroundDark.CGColor
            };
            View.Layer.AddSublayer(_viewGradient);

            _heading = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "Detailed View",
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.SecondaryText,
            };
            View.AddSubview(_heading);

            _plotContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_plotContainer);

            _plotGradient = new CAGradientLayer();
            _plotGradient.Colors = new CGColor[] {
                ColorPalette.PlotBackgroundLight.CGColor,
                ColorPalette.PlotBackgroundDark.CGColor
            };
            _plotContainer.Layer.AddSublayer(_plotGradient);

            _plotHeader = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = UILayoutConstraintAxis.Horizontal,
                Distribution = UIStackViewDistribution.FillEqually,
                Alignment = UIStackViewAlignment.Center,
            };
            _plotContainer.AddSubview(_plotHeader);

            _plotHeaderCircuit = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = UILayoutConstraintAxis.Horizontal,
                Distribution = UIStackViewDistribution.FillEqually,
                Alignment = UIStackViewAlignment.Center,
            };
            _plotHeader.AddArrangedSubview(_plotHeaderCircuit);

            _plotHeaderCircuitTitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "QUANTUM CIRCUIT",
                Font = FontGenerator.GenerateFont(14, UIFontWeight.Bold),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.SecondaryText,
            };
            _plotHeaderCircuit.AddArrangedSubview(_plotHeaderCircuitTitle);

            _plotHeaderCircuitImage = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Image = new UIImage("circuit.png"),
            };
            _plotHeaderCircuit.AddArrangedSubview(_plotHeaderCircuitImage);

            _plotHeaderTitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "COMPUTER",
                Font = FontGenerator.GenerateFont(20, UIFontWeight.Bold),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.SecondaryText,
            };
            _plotHeader.AddArrangedSubview(_plotHeaderTitle);

            _plotHeaderProgress = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "COLLECTION PROGRESS: 0%",
                Font = FontGenerator.GenerateFont(14, UIFontWeight.Bold),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.SecondaryText,
            };
            _plotHeader.AddArrangedSubview(_plotHeaderProgress);

            _plotView = new PlotView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
            };
            _plotContainer.AddSubview(_plotView);

            _knobContainer = new UIStackView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = UILayoutConstraintAxis.Vertical,
                Alignment = UIStackViewAlignment.Center,
                Distribution = UIStackViewDistribution.FillProportionally,
                LayoutMarginsRelativeArrangement = true,
                LayoutMargins = new UIEdgeInsets
                {
                    Top = 20,
                    Bottom = 4,
                    Left = 4,
                    Right = 4,
                },
                Spacing = 20,
            };
            _knobContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            _knobContainer.Layer.ShadowOpacity = 0.25f;
            _knobContainer.Layer.ShadowOffset = new CGSize(5, 0);
            _knobContainer.Layer.ShadowRadius = 0;
            View.AddSubview(_knobContainer);

            _knobContainerGradient = new CAGradientLayer();
            _knobContainerGradient.Colors = new CGColor[] {
                ColorPalette.PlotBackgroundLight.CGColor,
                ColorPalette.PlotBackgroundDark.CGColor
            };
            _knobContainer.Layer.AddSublayer(_knobContainerGradient);

            _knobHeader = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "θ TELESCOPE LENS",
                Font = FontGenerator.GenerateFont(16, UIFontWeight.Bold),
                TextColor = ColorPalette.SecondaryText,
            };
            _knobHeader.SetContentCompressionResistancePriority((float) UILayoutPriority.Required, UILayoutConstraintAxis.Vertical);
            _knobContainer.AddArrangedSubview(_knobHeader);

            _knobControl = new KnobView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _knobContainer.AddArrangedSubview(_knobControl);

            //_telescopeAngle = new UILabel
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular),
            //    TextColor = ColorPalette.AngleText,
            //};
            //_knobControl.AddSubview(_telescopeAngle);

            _sceneView = new UIView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Blue,
            };
            _sceneView.Layer.CornerRadius = 5;
            View.AddSubview(_sceneView);

            _heading.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _heading.TopAnchor.ConstraintEqualTo(View.TopAnchor, 22).Active = true;

            _plotContainer.TopAnchor.ConstraintEqualTo(_heading.BottomAnchor, 22).Active = true;
            _plotContainer.BottomAnchor.ConstraintEqualTo(_knobContainer.TopAnchor, -22).Active = true;
            _plotContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 16).Active = true;
            _plotContainer.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;

            _plotHeader.TopAnchor.ConstraintEqualTo(_plotContainer.TopAnchor).Active = true;
            _plotHeader.LeftAnchor.ConstraintEqualTo(_plotContainer.LeftAnchor).Active = true;
            _plotHeader.WidthAnchor.ConstraintEqualTo(_plotContainer.WidthAnchor).Active = true;
            _plotHeader.HeightAnchor.ConstraintEqualTo(64).Active = true;

            _plotView.TopAnchor.ConstraintEqualTo(_plotHeader.BottomAnchor).Active = true;
            _plotView.BottomAnchor.ConstraintEqualTo(_plotContainer.BottomAnchor).Active = true;
            _plotView.LeftAnchor.ConstraintEqualTo(_plotContainer.LeftAnchor).Active = true;
            _plotView.RightAnchor.ConstraintEqualTo(_plotContainer.RightAnchor, -150).Active = true;

            _knobContainer.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.22f).Active = true;
            _knobContainer.TopAnchor.ConstraintEqualTo(_sceneView.TopAnchor).Active = true;
            _knobContainer.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _knobContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 16).Active = true;

            //_telescopeAngle.CenterXAnchor.ConstraintEqualTo(_knobControl.CenterXAnchor).Active = true;
            //_telescopeAngle.CenterYAnchor.ConstraintEqualTo(_knobControl.CenterYAnchor).Active = true;

            _sceneView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.33f).Active = true;
            _sceneView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.25f).Active = true;
            _sceneView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -16).Active = true;
            _sceneView.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;

            var set = CreateBindingSet();
            set.Bind(_plotView).For(v => v.Model).To(vm => vm.PhotonPlotModel);
            //set.Bind(_telescopeAngle).To(vm => vm.TelescopeAngle).WithConversion("AngleDisplay");
            set.Bind(_knobControl).For(v => v.Angle).To(vm => vm.TelescopeAngle).WithConversion("DegreesToRadians");
            //set.Bind(_knobControl).For("TouchUp").To(vm => vm.TriggerOneTimeRun);
            set.Apply();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            // Set gradient frame size after views have been laid out
            _viewGradient.Frame = View.Bounds;
            _plotGradient.Frame = _plotContainer.Bounds;
            _knobContainerGradient.Frame = _knobContainer.Bounds;
        }
    }
}
