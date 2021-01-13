using System;
using CoreGraphics;
using SceneKit;
using TheQTablet.Core.Utils;
using TheQTablet.Core.ViewModels.Main.Lesson01;
using UIKit;

namespace TheQTablet.iOS.Views.Main
{
    public class InitialSceneRotation : SCNSceneRendererDelegate
    {
        private bool _initialRotationDone = false;
        private SCNQuaternion _initialRotation;

        public InitialSceneRotation(SCNQuaternion rotation)
        {
            _initialRotation = rotation;
        }

        public override void WillRenderScene(ISCNSceneRenderer renderer, SCNScene scene, double timeInSeconds)
        {
            if (!_initialRotationDone)
            {
                scene.RootNode.LocalRotate(_initialRotation);
                _initialRotationDone = true;
            }
        }
    }

    public partial class Lesson01SatelliteLensViewController : Lesson01BaseViewController<Lesson01SatelliteLensViewModel>
    {
        private UIImageView _background;

        private UILabel _signalStrengthText;

        private UIView _infoTextContainer;
        private UILabel _infoText;
        private UIButton _continue;

        private SCNScene _lensScene;
        private SCNView _lensSceneView;

        private CGPoint _lastTranslation;
        private UIPanGestureRecognizer _gestureRecognizer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _gestureRecognizer = new UIPanGestureRecognizer();
            _gestureRecognizer.AddTarget(() => Pan(_gestureRecognizer));
            View.AddGestureRecognizer(_gestureRecognizer);
        }

        protected override void CreateView()
        {
            base.CreateView();

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

            _continue = ButtonGenerator.PrimaryButton("Go to Simulation");
            View.AddSubview(_continue);

            _lensScene = SCNScene.FromFile("Art.scnassets/happyIdleProf.dae");

            _lensSceneView = new SCNView(View.Frame)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _lensSceneView.BackgroundColor = UIColor.Clear;
            _lensSceneView.AutoresizingMask = UIViewAutoresizing.All;
            _lensSceneView.Scene = _lensScene;
            _lensSceneView.SceneRendererDelegate = new InitialSceneRotation(
                SCNQuaternion.FromAxisAngle(SCNVector3.UnitY, MathHelpers.ToRadF(60)) *
                SCNQuaternion.FromAxisAngle(SCNVector3.UnitX, MathHelpers.ToRadF(-30))
            );

            View.AddSubview(_lensSceneView);
        }

        protected override void LayoutView()
        {
            base.LayoutView();

            _background.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

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

            _lensSceneView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _lensSceneView.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
            _lensSceneView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5f).Active = true;
            _lensSceneView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.5f).Active = true;
        }

        protected override void BindView()
        {
            base.BindView();

            var set = CreateBindingSet();
            set.Bind(_continue).To(vm => vm.ContinueCommand);
            set.Apply();
        }

        private void Pan(UIPanGestureRecognizer gesture)
        {
            if (gesture.State == UIGestureRecognizerState.Began)
            {
                _lastTranslation = new CGPoint(0, 0);
            }

            var pan = gesture.TranslationInView(View);
            float diffX = (float)(pan.X - _lastTranslation.X);
            float diffY = (float)(pan.Y - _lastTranslation.Y);

            // Get drag distance along 30 degree line (match angle of lens)
            var angle = MathHelpers.ToRad(30);
            SCNVector3 axis = SCNVector3.Normalize(new SCNVector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0));
            SCNVector3 movement = new SCNVector3(diffX, diffY, 0);
            var dist = SCNVector3.Dot(axis, movement);
            if (_lensScene != null)
            {
                _lensScene.RootNode.LocalRotate(SCNQuaternion.FromAxisAngle(SCNVector3.UnitZ, dist / 100.0f));
            }

            // Use only vertical drag
            //_lensScene.RootNode.LocalRotate(SCNQuaternion.FromAxisAngle(SCNVector3.UnitZ, diffY / 100.0f));

            _lastTranslation = pan;
        }
    }
}

