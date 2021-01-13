using System;
using System.Collections;
using System.Globalization;

using CoreGraphics;

using Foundation;

using MvvmCross.Binding.BindingContext;
using MvvmCross.Converters;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

using TheQTablet.Core.ViewModels.Main;
using TheQTablet.iOS.Views.Custom;

using UIKit;

namespace TheQTablet.iOS.Views.Main
{
    public class ShowDuringStateConverter : MvxValueConverter<ConnectivityState, bool>
    {
        protected override bool Convert(ConnectivityState viewModelState, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            ConnectivityState[] connectivityStates = parameter as ConnectivityState[];
            return Array.IndexOf(connectivityStates, viewModelState) != -1;
        }
    }
    public class TestConverter : MvxValueConverter<IEnumerable, IEnumerable>
    {
        protected override IEnumerable Convert(IEnumerable value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return value;
        }
    }

    public class LoadingTextConverter : MvxValueConverter<ConnectivityState, string>
    {
        protected override string Convert(ConnectivityState value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            switch (value)
            {
                case ConnectivityState.Loading: return "Loading";
                case ConnectivityState.ScanningDevices: return "Scanning Devices";
                case ConnectivityState.ConnectingDevice: return "Connecting Device";
                case ConnectivityState.GettingDetails: return "Getting Details";
                case ConnectivityState.CheckingConnection: return "Checking Connection";
                case ConnectivityState.ScanningNetworks: return "Scanning Networks";
                case ConnectivityState.ConnectingNetwork: return "Connecting";
                default:
                    return null;
            }
        }
    }

    public class InstructionsTextConverter : MvxValueConverter<ConnectivityState, string>
    {
        protected override string Convert(ConnectivityState value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            switch (value)
            {
                case ConnectivityState.ChooseDevice: return "Scanning for Bluetooth devices.\nPlease select your device on the right.";
                case ConnectivityState.ChooseNetwork: return "Choose Wi-Fi network for The Q to connect to.";
                case ConnectivityState.ConnectedToNetworkNoAPI: return (
                    "Device connected to Wi-Fi but unable to communicate with server, " +
                    "please make sure this device is on the same network or choose another network."
                );
                default:
                    return null;
            }
        }
    }


    [MvxModalPresentation(WrapInNavigationController = false, ModalPresentationStyle = UIModalPresentationStyle.FormSheet)]
    public class ConnectivityViewController : BaseViewController<ConnectivityViewModel>, IUIAdaptivePresentationControllerDelegate
    {
        private UIImageView _closeCross;

        private UIView _loadingContainer;
        private UIImageView _loading;
        private UILabel _loadingText;

        private UIView _contentContainer;

        private UIView _leftContainer;
        private UILabel _instructions;

        private Divider _divider;

        private UIView _rightContainer;
        private ConnectivityTableView<DeviceSource> _devicesContainer;
        private ConnectivityTableView<NetworkSource> _networksContainer;

        private UIView _successContainer;
        private UIView _successText;
        private UIButton _successCloseButton;

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

            _loadingContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_loadingContainer);

            _loading = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("back_logo"),
            };
            _loadingContainer.AddSubview(_loading);

            _loadingText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.SecondaryText,
                Font = FontGenerator.GenerateFont(32, UIFontWeight.Regular),
            };
            _loadingContainer.AddSubview(_loadingText);

            _contentContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Top = 20,
                    Bottom = 20,
                    Leading = 20,
                    Trailing = 20,
                }
            };
            View.AddSubview(_contentContainer);

            _leftContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _contentContainer.AddSubview(_leftContainer);

            _instructions = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            _leftContainer.AddSubview(_instructions);

            _divider = new Divider
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = DividerAxis.Vertical,
            };
            _contentContainer.AddSubview(_divider);

            _rightContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Leading = 20,
                    Trailing = 20,
                    Top = 20,
                    Bottom = 20,
                },
            };
            _contentContainer.AddSubview(_rightContainer);

            _successContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_successContainer);

            _successText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(40, UIFontWeight.Regular),
                Text = "Successfully connected to The Q server",
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            _successContainer.AddSubview(_successText);

            _successCloseButton = ButtonGenerator.PrimaryButton("Close");
            _successContainer.AddSubview(_successCloseButton);

            _devicesContainer = new ConnectivityTableView<DeviceSource>("Devices");
            _rightContainer.AddSubview(_devicesContainer);

            _networksContainer = new ConnectivityTableView<NetworkSource>("Networks");
            _rightContainer.AddSubview(_networksContainer);

            _closeCross = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("cross"),
            };
            View.AddSubview(_closeCross);
        }

        protected override void LayoutView()
        {
            base.LayoutView();

            _closeCross.TopAnchor.ConstraintEqualTo(View.TopAnchor, 16).Active = true;
            _closeCross.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;
            _closeCross.WidthAnchor.ConstraintEqualTo(_closeCross.HeightAnchor).Active = true;
            _closeCross.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.02f).Active = true;

            _loadingContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            _loadingContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            _loadingContainer.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _loadingContainer.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;

            _loading.CenterXAnchor.ConstraintEqualTo(_loading.Superview.CenterXAnchor).Active = true;
            _loading.CenterYAnchor.ConstraintEqualTo(_loading.Superview.CenterYAnchor).Active = true;
            _loading.HeightAnchor.ConstraintEqualTo(_loading.Superview.HeightAnchor, 0.25f).Active = true;
            _loading.AspectRatioConstraint().Active = true;

            _loadingText.CenterXAnchor.ConstraintEqualTo(_loading.Superview.CenterXAnchor).Active = true;
            _loadingText.TopAnchor.ConstraintEqualTo(_loading.BottomAnchor, 20).Active = true;

            _contentContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            _contentContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            _contentContainer.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _contentContainer.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;

            _leftContainer.LeftAnchor.ConstraintEqualTo(_contentContainer.LeftAnchor).Active = true;
            _leftContainer.TopAnchor.ConstraintEqualTo(_contentContainer.TopAnchor).Active = true;
            _leftContainer.BottomAnchor.ConstraintEqualTo(_contentContainer.BottomAnchor).Active = true;
            _leftContainer.RightAnchor.ConstraintEqualTo(_divider.LeftAnchor).Active = true;

            _instructions.CenterYAnchor.ConstraintEqualTo(_leftContainer.CenterYAnchor).Active = true;
            _instructions.CenterXAnchor.ConstraintEqualTo(_leftContainer.CenterXAnchor).Active = true;
            _instructions.WidthAnchor.ConstraintLessThanOrEqualTo(_leftContainer.LayoutMarginsGuide.WidthAnchor, 0.75f).Active = true;

            _divider.CenterXAnchor.ConstraintEqualTo(_contentContainer.CenterXAnchor).Active = true;
            _divider.CenterYAnchor.ConstraintEqualTo(_contentContainer.CenterYAnchor).Active = true;
            _divider.TopAnchor.ConstraintEqualTo(_contentContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _divider.BottomAnchor.ConstraintEqualTo(_contentContainer.LayoutMarginsGuide.BottomAnchor).Active = true;

            _rightContainer.RightAnchor.ConstraintEqualTo(_contentContainer.LayoutMarginsGuide.RightAnchor).Active = true;
            _rightContainer.TopAnchor.ConstraintEqualTo(_contentContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _rightContainer.BottomAnchor.ConstraintEqualTo(_contentContainer.LayoutMarginsGuide.BottomAnchor).Active = true;
            _rightContainer.LeftAnchor.ConstraintEqualTo(_divider.RightAnchor).Active = true;

            _successContainer.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _successContainer.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
            _successContainer.WidthAnchor.ConstraintLessThanOrEqualTo(View.WidthAnchor).Active = true;

            _successContainer.WidthAnchor.ConstraintGreaterThanOrEqualTo(_successText.WidthAnchor).Active = true;
            _successContainer.WidthAnchor.ConstraintGreaterThanOrEqualTo(_successCloseButton.WidthAnchor).Active = true;

            _successText.TopAnchor.ConstraintEqualTo(_successContainer.TopAnchor).Active = true;
            _successText.CenterXAnchor.ConstraintEqualTo(_successContainer.CenterXAnchor).Active = true;

            _successCloseButton.TopAnchor.ConstraintEqualTo(_successText.BottomAnchor, 20).Active = true;
            _successCloseButton.BottomAnchor.ConstraintEqualTo(_successContainer.BottomAnchor).Active = true;
            _successCloseButton.CenterXAnchor.ConstraintEqualTo(_successText.CenterXAnchor).Active = true;

            _devicesContainer.TopAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _devicesContainer.BottomAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.BottomAnchor).Active = true;
            _devicesContainer.LeftAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _devicesContainer.RightAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.RightAnchor).Active = true;

            _networksContainer.TopAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _networksContainer.BottomAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.BottomAnchor).Active = true;
            _networksContainer.LeftAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _networksContainer.RightAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.RightAnchor).Active = true;
        }

        protected override void BindView()
        {
            base.BindView();

            var set = CreateBindingSet();
            set.Bind(_contentContainer).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.Loaded,
                ConnectivityState.ChooseDevice,
                ConnectivityState.ConnectedDevice,
                ConnectivityState.GotDetails,
                ConnectivityState.CheckedConnection,
                ConnectivityState.ChooseNetwork,
                ConnectivityState.ConnectedToNetworkNoAPI,
            });
            set.Bind(_loadingContainer).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.Loading,
                ConnectivityState.ScanningDevices,
                ConnectivityState.ConnectingDevice,
                ConnectivityState.GettingDetails,
                ConnectivityState.CheckingConnection,
                ConnectivityState.ScanningNetworks,
                ConnectivityState.ConnectingNetwork,
            });
            set.Bind(_loadingText).To(vm => vm.State).WithConversion<LoadingTextConverter>();

            set.Bind(_devicesContainer).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.ChooseDevice,
            });
            set.Bind(_networksContainer).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.ChooseNetwork,
                ConnectivityState.ConnectedToNetworkNoAPI,
            });
            set.Bind(_successContainer).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.Success,
            });

            set.Bind(_instructions).To(vm => vm.State).WithConversion<InstructionsTextConverter>();
            set.Bind(_networksContainer.Source).For(v => v.ItemsSource).To(vm => vm.Networks).WithConversion<TestConverter>();
            set.Bind(_networksContainer.Source).For(v => v.SelectionChangedCommand).To(vm => vm.JoinNetworkCommand);
            set.Bind(_devicesContainer.Source).For(v => v.ItemsSource).To(vm => vm.Devices);
            set.Bind(_devicesContainer.Source).For(v => v.SelectionChangedCommand).To(vm => vm.ConnectDeviceCommand);
            set.Bind(_closeCross).For("Tap").To(vm => vm.CloseCommand);
            set.Bind(_successCloseButton).For("Tap").To(vm => vm.CloseCommand);
            set.Apply();
        }

        [Export("presentationControllerDidDismiss:")]
        public void DidDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
        }
    }
}
