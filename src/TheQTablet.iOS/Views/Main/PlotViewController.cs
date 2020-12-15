using System;
using System.Globalization;
using CoreGraphics;
using Foundation;
using MvvmCross.Converters;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
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

    [MvxModalPresentation(WrapInNavigationController = false, ModalPresentationStyle = UIModalPresentationStyle.FormSheet)]
    public partial class PlotViewController : BaseViewController<PlotViewModel>, IUIAdaptivePresentationControllerDelegate
    {
        private UIImageView _backgroundGradient;

        private UILabel _heading;
        private UIImageView _closeCross;

        private UIView _headingDivider;
        private UIView _container;

        private UIStackView _toolbar;

        private CompactStackView _toolbarCircuit;
        private UILabel _toolbarCircuitText;
        private UIImageView _toolbarCircuitImage;

        private CompactStackView _toolbarProgress;
        private UILabel _toolbarProgressText;
        private InsetProgressBar _toolbarProgressBar;

        private UIView _plotContainer;
        private PlotView _plotView;

        private UIView _functionButtonsContainer;
        private UIStackView _functionButtonsStack;
        private UILabel _functionButtonsTitle;
        private ToggleButton _cosOverlayButton;

        private KnobContainerView _telescopeAngleKnob;

        private UIView _sceneViewBorder;
        private UIImageView _sceneView;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            PresentationController.Delegate = this;
            PreferredContentSize = new CGSize(UIScreen.MainScreen.Bounds.Width - 80, UIScreen.MainScreen.Bounds.Height);
        }

        protected override void CreateView()
        {
            base.CreateView();

            View.BackgroundColor = ColorPalette.BackgroundDark;
            _backgroundGradient = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("modal_background_gradient"),
            };
            View.AddSubview(_backgroundGradient);

            _heading = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "DETAIL VIEW : COMPUTER",
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            View.AddSubview(_heading);

            _closeCross = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("cross"),
            };
            View.AddSubview(_closeCross);

            _headingDivider = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = ColorPalette.Border,
            };
            View.AddSubview(_headingDivider);

            _container = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_container);

            _toolbar = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = UILayoutConstraintAxis.Horizontal,
                Distribution = UIStackViewDistribution.FillEqually,
                Alignment = UIStackViewAlignment.Center,
            };
            _container.AddSubview(_toolbar);

            _toolbarCircuit = new CompactStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = UILayoutConstraintAxis.Horizontal,
                Alignment = UIStackViewAlignment.Center,
                Spacing = 20,
            };
            _toolbar.AddArrangedSubview(_toolbarCircuit);

            _toolbarCircuitText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "QUANTUM CIRCUIT",
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.PrimaryText,
            };
            _toolbarCircuit.AddArrangedSubview(_toolbarCircuitText);

            _toolbarCircuitImage = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Image = UIImage.FromBundle("plain_circuit"),
            };
            _toolbarCircuit.AddArrangedSubview(_toolbarCircuitImage);

            _toolbarProgress = new CompactStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                PadStart = true,
                Axis = UILayoutConstraintAxis.Horizontal,
                Alignment = UIStackViewAlignment.Center,
                LayoutMarginsRelativeArrangement = true,
                LayoutMargins = new UIEdgeInsets
                {
                    Right = 16,
                },
                Spacing = 20,
            };
            _toolbar.AddArrangedSubview(_toolbarProgress);

            _toolbarProgressText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.PrimaryText,
            };
            _toolbarProgress.AddArrangedSubview(_toolbarProgressText);

            _toolbarProgressBar = new InsetProgressBar
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TrackImage = UIImage.FromBundle("progress_track_horizontal"),
                ProgressImage = UIImage.FromBundle("progress_inner_horizontal"),
                Inset = 5,
            };
            _toolbarProgress.AddArrangedSubview(_toolbarProgressBar);

            _plotContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _plotContainer.Layer.BorderColor = ColorPalette.SecondaryText.CGColor;
            _plotContainer.Layer.BorderWidth = 2;
            _plotContainer.Layer.CornerRadius = 20;
            _container.AddSubview(_plotContainer);

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
            _container.AddSubview(_functionButtonsContainer);

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
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.SecondaryText,
            };
            _functionButtonsStack.AddArrangedSubview(_functionButtonsTitle);

            _cosOverlayButton = new ToggleButton
            {
                Text = "Cos²(x + 30°)",
            };
            _functionButtonsStack.AddArrangedSubview(_cosOverlayButton);

            _telescopeAngleKnob = new KnobContainerView("TELESCOPE LENS")
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Step = ViewModel.Step,
            };
            View.AddSubview(_telescopeAngleKnob);

            _sceneViewBorder = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _sceneViewBorder.Layer.BorderColor = ColorPalette.Border.CGColor;
            _sceneViewBorder.Layer.BorderWidth = 2;
            _sceneViewBorder.Layer.CornerRadius = 20;
            View.AddSubview(_sceneViewBorder);

            _sceneView = new UIImageView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("scene_preview"),
            };
            _sceneViewBorder.AddSubview(_sceneView);
        }

        protected override void LayoutView()
        {
            base.LayoutView();

            _backgroundGradient.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _backgroundGradient.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            _heading.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _heading.TopAnchor.ConstraintEqualTo(View.TopAnchor, 22).Active = true;

            _closeCross.CenterYAnchor.ConstraintEqualTo(_heading.CenterYAnchor).Active = true;
            _closeCross.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;
            _closeCross.WidthAnchor.ConstraintEqualTo(_closeCross.HeightAnchor).Active = true;
            _closeCross.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.02f).Active = true;

            _headingDivider.TopAnchor.ConstraintEqualTo(_heading.BottomAnchor, 22).Active = true;
            _headingDivider.LeftAnchor.ConstraintEqualTo(_container.LeftAnchor).Active = true;
            _headingDivider.RightAnchor.ConstraintEqualTo(_container.RightAnchor).Active = true;
            _headingDivider.HeightAnchor.ConstraintEqualTo(2).Active = true;

            _container.TopAnchor.ConstraintEqualTo(_headingDivider.BottomAnchor, 22).Active = true;
            _container.BottomAnchor.ConstraintEqualTo(_sceneViewBorder.TopAnchor, -22).Active = true;
            _container.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 40).Active = true;
            _container.RightAnchor.ConstraintEqualTo(View.RightAnchor, -40).Active = true;

            _toolbar.TopAnchor.ConstraintEqualTo(_container.TopAnchor).Active = true;
            _toolbar.LeftAnchor.ConstraintEqualTo(_container.LeftAnchor).Active = true;
            _toolbar.WidthAnchor.ConstraintEqualTo(_container.WidthAnchor).Active = true;
            _toolbar.HeightAnchor.ConstraintEqualTo(64).Active = true;

            _toolbarCircuitText.HeightAnchor.ConstraintEqualTo(_toolbarCircuitText.Font.PointSize).Active = true;
            _toolbarCircuitImage.HeightAnchor.ConstraintEqualTo(_toolbarCircuitText.HeightAnchor, 1.2f).Active = true;
            _toolbarCircuitImage.WidthAnchor.ConstraintLessThanOrEqualTo(_toolbarCircuitImage.HeightAnchor, _toolbarCircuitImage.Image.Size.Width / _toolbarCircuitImage.Image.Size.Height).Active = true;

            _toolbarProgressText.HeightAnchor.ConstraintEqualTo(_toolbarProgressText.Font.PointSize).Active = true;
            _toolbarProgressBar.HeightAnchor.ConstraintEqualTo(_toolbarProgressText.HeightAnchor).Active = true;
            _toolbarProgressBar.WidthAnchor.ConstraintLessThanOrEqualTo(_toolbarProgressBar.HeightAnchor, _toolbarProgressBar.TrackImage.Size.Width / _toolbarProgressBar.TrackImage.Size.Height).Active = true;

            _plotContainer.TopAnchor.ConstraintEqualTo(_toolbar.BottomAnchor).Active = true;
            _plotContainer.BottomAnchor.ConstraintEqualTo(_container.BottomAnchor).Active = true;
            _plotContainer.LeftAnchor.ConstraintEqualTo(_container.LeftAnchor).Active = true;
            _plotContainer.RightAnchor.ConstraintEqualTo(_functionButtonsContainer.LeftAnchor).Active = true;

            _plotView.HeightAnchor.ConstraintEqualTo(_plotContainer.HeightAnchor, 0.9f).Active = true;
            _plotView.WidthAnchor.ConstraintEqualTo(_plotContainer.WidthAnchor, 0.9f).Active = true;
            _plotView.LeftAnchor.ConstraintEqualTo(_plotContainer.LeftAnchor).Active = true;
            _plotView.BottomAnchor.ConstraintEqualTo(_plotContainer.BottomAnchor).Active = true;

            _functionButtonsContainer.WidthAnchor.ConstraintEqualTo(_container.WidthAnchor, 0.3f).Active = true;
            _functionButtonsContainer.RightAnchor.ConstraintEqualTo(_container.RightAnchor).Active = true;
            _functionButtonsContainer.TopAnchor.ConstraintEqualTo(_toolbar.BottomAnchor).Active = true;
            _functionButtonsContainer.BottomAnchor.ConstraintEqualTo(_plotView.BottomAnchor, -100).Active = true;

            _functionButtonsStack.WidthAnchor.ConstraintEqualTo(_functionButtonsContainer.WidthAnchor, 0.5f).Active = true;
            _functionButtonsStack.CenterYAnchor.ConstraintEqualTo(_functionButtonsContainer.CenterYAnchor).Active = true;
            _functionButtonsStack.CenterXAnchor.ConstraintEqualTo(_functionButtonsContainer.CenterXAnchor).Active = true;

            _cosOverlayButton.WidthAnchor.ConstraintEqualTo(_functionButtonsStack.WidthAnchor, 0.75f).Active = true;

            _telescopeAngleKnob.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.22f).Active = true;
            _telescopeAngleKnob.TopAnchor.ConstraintEqualTo(_sceneViewBorder.TopAnchor).Active = true;
            _telescopeAngleKnob.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _telescopeAngleKnob.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 16).Active = true;

            _sceneViewBorder.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.33f).Active = true;
            _sceneViewBorder.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -16).Active = true;
            _sceneViewBorder.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;

            _sceneView.WidthAnchor.ConstraintEqualTo(_sceneViewBorder.WidthAnchor, 1, -40).Active = true;
            _sceneView.HeightAnchor.ConstraintEqualTo(_sceneViewBorder.HeightAnchor, 1, -40).Active = true;
            _sceneView.HeightAnchor.ConstraintEqualTo(_sceneView.WidthAnchor, _sceneView.Image.Size.Height / _sceneView.Image.Size.Width).Active = true;
            _sceneView.CenterXAnchor.ConstraintEqualTo(_sceneViewBorder.CenterXAnchor).Active = true;
            _sceneView.CenterYAnchor.ConstraintEqualTo(_sceneViewBorder.CenterYAnchor).Active = true;
        }

        protected override void BindView()
        {
            base.BindView();

            var set = CreateBindingSet();
            set.Bind(_plotView).For(v => v.Model).To(vm => vm.PhotonPlotModel);
            set.Bind(_telescopeAngleKnob.KnobControl).For(v => v.SteppedAngle).To(vm => vm.TelescopeAngle);
            set.Bind(_sceneView).For("Tap").To(vm => vm.CloseModalCommand);
            set.Bind(_closeCross).For("Tap").To(vm => vm.CloseModalCommand);
            set.Bind(_cosOverlayButton).For(v => v.Active).To(vm => vm.ShowCosOverlay);
            set.Bind(_toolbarProgressText).To(vm => vm.Progress).WithConversion<ProgressLabelConverter>();
            set.Bind(_toolbarProgressBar).For(v => v.Progress).To(vm => vm.Progress);
            set.Apply();
        }

        [Export("presentationControllerDidDismiss:")]
        public void DidDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseModalCommand.Execute();
        }
    }
}
