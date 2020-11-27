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
    public class ProgressLabelConverter : MvxValueConverter<float, string>
    {
        protected override string Convert(float value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return "COLLECTION PROGRESS " + (value * 100).ToString("0.#") + "%";
        }
    }

    [MvxModalPresentation(WrapInNavigationController = false)]
    public partial class PlotViewController : MvxViewController<PlotViewModel>
    {
        private UILabel _heading;
        private UIImageView _closeCross;

        private UIView _plotContainer;
        private UIStackView _plotHeader;
        private UIStackView _plotHeaderCircuit;
        private UILabel _plotHeaderCircuitTitle;
        private UIImageView _plotHeaderCircuitImage;
        private UILabel _plotHeaderTitle;
        private UIStackView _plotHeaderProgress;
        private UILabel _plotHeaderProgressTitle;
        private UIProgressView _plotHeaderProgressBar;
        private PlotView _plotView;

        private UIView _functionButtonsContainer;
        private UIStackView _functionButtonsStack;
        private UILabel _functionButtonsTitle;
        private ToggleButton _cosOverlayButton;

        private KnobContainerView _telescopeAngleKnob;

        private UIView _sceneView;

        private CAGradientLayer _viewGradient;
        private CAGradientLayer _plotGradient;

        public PlotViewController()
        {
            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            _viewGradient = new CAGradientLayer
            {
                Colors = new CGColor[] {
                    ColorPalette.BackgroundLight.CGColor,
                    ColorPalette.BackgroundDark.CGColor
                }
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

            _closeCross = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = new UIImage("cross.png"),
            };
            _closeCross.Layer.AffineTransform = CGAffineTransform.MakeRotation((float) Math.PI / 2.0f);
            View.AddSubview(_closeCross);

            _plotContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_plotContainer);

            _plotGradient = new CAGradientLayer
            {
                Colors = new CGColor[] {
                    ColorPalette.PlotBackgroundLight.CGColor,
                    ColorPalette.PlotBackgroundDark.CGColor
                }
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

            _plotHeaderProgress = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = UILayoutConstraintAxis.Horizontal,
                Distribution = UIStackViewDistribution.Fill,
                Alignment = UIStackViewAlignment.Center,
                LayoutMarginsRelativeArrangement = true,
                LayoutMargins = new UIEdgeInsets
                {
                    Right = 16,
                },
                Spacing = 20,
            };
            _plotHeader.AddArrangedSubview(_plotHeaderProgress);

            _plotHeaderProgressTitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(14, UIFontWeight.Bold),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.SecondaryText,
            };
            _plotHeaderProgress.AddArrangedSubview(_plotHeaderProgressTitle);

            _plotHeaderProgressBar = new UIProgressView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TrackTintColor = UIColor.White,
            };
            _plotHeaderProgressBar.Layer.CornerRadius = 10;
            _plotHeaderProgress.AddArrangedSubview(_plotHeaderProgressBar);

            _plotView = new PlotView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
            };
            _plotContainer.AddSubview(_plotView);

            _functionButtonsContainer = new UIView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _plotContainer.AddSubview(_functionButtonsContainer);

            _functionButtonsStack = new UIStackView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alignment = UIStackViewAlignment.Center,
                Distribution = UIStackViewDistribution.EqualSpacing,
                Axis = UILayoutConstraintAxis.Vertical,
                Spacing = 20,
            };
            _functionButtonsContainer.AddSubview(_functionButtonsStack);

            _functionButtonsTitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "GRAPH UNDERLAY",
                Font = FontGenerator.GenerateFont(16, UIFontWeight.Bold),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.SecondaryText,
            };
            _functionButtonsStack.AddArrangedSubview(_functionButtonsTitle);

            _cosOverlayButton = new ToggleButton
            {
                Text = "Cos(x + 30°)",
            };
            _functionButtonsStack.AddArrangedSubview(_cosOverlayButton);

            _telescopeAngleKnob = new KnobContainerView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Step = ViewModel.Step,
            };
            View.AddSubview(_telescopeAngleKnob);

            _sceneView = new UIView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Blue,
            };
            _sceneView.Layer.CornerRadius = 5;
            View.AddSubview(_sceneView);

            _heading.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _heading.TopAnchor.ConstraintEqualTo(View.TopAnchor, 22).Active = true;

            _closeCross.CenterYAnchor.ConstraintEqualTo(_heading.CenterYAnchor).Active = true;
            _closeCross.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;
            _closeCross.WidthAnchor.ConstraintEqualTo(_closeCross.HeightAnchor).Active = true;
            _closeCross.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.03f).Active = true;

            _plotContainer.TopAnchor.ConstraintEqualTo(_heading.BottomAnchor, 22).Active = true;
            _plotContainer.BottomAnchor.ConstraintEqualTo(_sceneView.TopAnchor, -22).Active = true;
            _plotContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 16).Active = true;
            _plotContainer.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;

            _plotHeader.TopAnchor.ConstraintEqualTo(_plotContainer.TopAnchor).Active = true;
            _plotHeader.LeftAnchor.ConstraintEqualTo(_plotContainer.LeftAnchor).Active = true;
            _plotHeader.WidthAnchor.ConstraintEqualTo(_plotContainer.WidthAnchor).Active = true;
            _plotHeader.HeightAnchor.ConstraintEqualTo(64).Active = true;

            _plotHeaderProgressBar.HeightAnchor.ConstraintEqualTo(10).Active = true;

            _plotView.TopAnchor.ConstraintEqualTo(_plotHeader.BottomAnchor).Active = true;
            _plotView.BottomAnchor.ConstraintEqualTo(_plotContainer.BottomAnchor).Active = true;
            _plotView.LeftAnchor.ConstraintEqualTo(_plotContainer.LeftAnchor).Active = true;
            _plotView.RightAnchor.ConstraintEqualTo(_functionButtonsContainer.LeftAnchor).Active = true;

            _functionButtonsContainer.WidthAnchor.ConstraintEqualTo(_plotContainer.WidthAnchor, 0.15f).Active = true;
            _functionButtonsContainer.RightAnchor.ConstraintEqualTo(_plotContainer.RightAnchor).Active = true;
            _functionButtonsContainer.TopAnchor.ConstraintEqualTo(_plotHeader.BottomAnchor).Active = true;
            _functionButtonsContainer.BottomAnchor.ConstraintEqualTo(_plotView.BottomAnchor, -100).Active = true;

            _functionButtonsStack.WidthAnchor.ConstraintEqualTo(_functionButtonsContainer.WidthAnchor).Active = true;
            _functionButtonsStack.CenterYAnchor.ConstraintEqualTo(_functionButtonsContainer.CenterYAnchor).Active = true;
            _functionButtonsStack.CenterXAnchor.ConstraintEqualTo(_functionButtonsContainer.CenterXAnchor).Active = true;

            _cosOverlayButton.WidthAnchor.ConstraintEqualTo(_functionButtonsStack.WidthAnchor, 0.75f).Active = true;

            _telescopeAngleKnob.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.22f).Active = true;
            _telescopeAngleKnob.TopAnchor.ConstraintEqualTo(_sceneView.TopAnchor).Active = true;
            _telescopeAngleKnob.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _telescopeAngleKnob.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 16).Active = true;

            _sceneView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.33f).Active = true;
            _sceneView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.25f).Active = true;
            _sceneView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -16).Active = true;
            _sceneView.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;

            var set = CreateBindingSet();
            set.Bind(_plotView).For(v => v.Model).To(vm => vm.PhotonPlotModel);
            set.Bind(_telescopeAngleKnob.KnobControl).For(v => v.SteppedAngle).To(vm => vm.TelescopeAngle);
            set.Bind(_sceneView).For("Tap").To(vm => vm.CloseModalCommand);
            set.Bind(_closeCross).For("Tap").To(vm => vm.CloseModalCommand);
            set.Bind(_cosOverlayButton).For(v => v.Active).To(vm => vm.ShowCosOverlay);
            set.Bind(_plotHeaderProgressTitle).To(vm => vm.Progress).WithConversion<ProgressLabelConverter>();
            set.Bind(_plotHeaderProgressBar).To(vm => vm.Progress);
            set.Apply();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            // Set gradient frame size after views have been laid out
            _viewGradient.Frame = View.Bounds;
            _plotGradient.Frame = _plotContainer.Bounds;
        }
    }
}
