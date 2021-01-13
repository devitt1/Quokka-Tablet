using System;
using CoreGraphics;
using SceneKit;
using TheQTablet.Core.Utils;
using TheQTablet.Core.ViewModels.Main.Lesson01;
using TheQTablet.iOS.Views.Custom;

using UIKit;

namespace TheQTablet.iOS.Views.Main
{
    public partial class Lesson01ProfessorViewController : Lesson01BaseViewController<Lesson01ProfessorViewModel>
    {
        private UIImageView _background;

        private UIView _infoTextContainer;
        private UILabel _infoText;
        private UIButton _helpButton;
        private UIButton _refuseButton;
        private TriangleMaskedView _speechTriangle;

        private UIImageView _plotPreview;

        protected override void CreateView()
        {
            base.CreateView();

            _background = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("professor_scene"),
            };
            View.AddSubview(_background);

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
                Text = "Hi! I'm the professor, it looks like you could use some help",
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            _infoTextContainer.AddSubview(_infoText);

            _helpButton = ButtonGenerator.PrimaryButton("Yes please", 40);
            _infoTextContainer.AddSubview(_helpButton);

            _refuseButton = ButtonGenerator.TertiaryButton("Not now", 40);
            _infoTextContainer.AddSubview(_refuseButton);

            _speechTriangle = new TriangleMaskedView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White.ColorWithAlpha(0.5f),
            };
            _infoTextContainer.AddSubview(_speechTriangle);

            _plotPreview = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("plot_preview"),
                ClipsToBounds = true,
            };
            _plotPreview.Layer.CornerRadius = 20;
            View.AddSubview(_plotPreview);
        }

        protected override void LayoutView()
        {
            base.LayoutView();

            _background.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            _infoTextContainer.BottomAnchor.ConstraintEqualTo(View.CenterYAnchor, -200).Active = true;
            _infoTextContainer.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

            _infoText.TopAnchor.ConstraintEqualTo(_infoTextContainer.TopAnchor, 20).Active = true;
            _infoText.CenterXAnchor.ConstraintEqualTo(_infoTextContainer.CenterXAnchor).Active = true;

            _infoTextContainer.WidthAnchor.ConstraintGreaterThanOrEqualTo(_infoText.WidthAnchor, 1, 40).Active = true;

            _speechTriangle.TopAnchor.ConstraintEqualTo(_infoTextContainer.BottomAnchor).Active = true;
            _speechTriangle.RightAnchor.ConstraintEqualTo(_infoTextContainer.CenterXAnchor).Active = true;
            _speechTriangle.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.025f).Active = true;
            _speechTriangle.WidthAnchor.ConstraintEqualTo(_speechTriangle.HeightAnchor).Active = true;

            _helpButton.TopAnchor.ConstraintEqualTo(_infoText.BottomAnchor, 20).Active = true;
            _helpButton.BottomAnchor.ConstraintEqualTo(_infoTextContainer.BottomAnchor, -20).Active = true;
            _helpButton.RightAnchor.ConstraintEqualTo(_infoTextContainer.CenterXAnchor, -10).Active = true;

            _refuseButton.TopAnchor.ConstraintEqualTo(_helpButton.TopAnchor).Active = true;
            _refuseButton.LeftAnchor.ConstraintEqualTo(_infoTextContainer.CenterXAnchor, 10).Active = true;

            _plotPreview.AspectRatioConstraint().Active = true;
            _plotPreview.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.35f).Active = true;
            _plotPreview.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -60).Active = true;
            _plotPreview.RightAnchor.ConstraintEqualTo(View.RightAnchor, -60).Active = true;
        }

        protected override void BindView()
        {
            base.BindView();

            var set = CreateBindingSet();
            set.Bind(_plotPreview).For("Tap").To(vm => vm.ContinueCommand);
            set.Apply();
        }
    }
}

