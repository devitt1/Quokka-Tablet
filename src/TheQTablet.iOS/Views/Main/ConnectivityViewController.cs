using System;
using System.Globalization;

using CoreGraphics;

using Foundation;

using MvvmCross.Binding.BindingContext;
using MvvmCross.Converters;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.ViewModels.Main;
using TheQTablet.iOS.Views.Custom;

using UIKit;

namespace TheQTablet.iOS.Views.Main
{
    public class AuthHideLockConverter : MvxValueConverter<AuthType, bool>
    {
        protected override bool Convert(AuthType value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return value == AuthType.None;
        }
    }

    public class NetworkCell : MvxTableViewCell
    {
        public static readonly NSString CellId = new NSString("NetworkCell");

        private UIImageView _lock;
        private UILabel _ssid;

        NetworkCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.Clear;
            ContentView.DirectionalLayoutMargins = new NSDirectionalEdgeInsets
            {
                Top = 10,
                Bottom = 10,
                Leading = 10,
                Trailing = 10,
            };

            _lock = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("lock"),
            };
            ContentView.AddSubview(_lock);

            _ssid = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
                Lines = 0,
            };
            ContentView.AddSubview(_ssid);

            _lock.TopAnchor.ConstraintGreaterThanOrEqualTo(ContentView.LayoutMarginsGuide.TopAnchor).Active = true;
            _lock.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor).Active = true;
            _lock.BottomAnchor.ConstraintLessThanOrEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor).Active = true;
            _lock.HeightAnchor.ConstraintEqualTo(24).Active = true;
            _lock.AspectRatioConstraint().Active = true;

            _ssid.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor).Active = true;
            _ssid.LeadingAnchor.ConstraintEqualTo(_lock.TrailingAnchor).Active = true;
            _ssid.TrailingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TrailingAnchor).Active = true;
            _ssid.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor).Active = true;

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<NetworkCell, WiFiNetwork>();
                set.Bind(_ssid).For(v => v.Text).To(vm => vm.SSID);
                set.Bind(_lock).For(v => v.Hidden).To(vm => vm.Auth).WithConversion<AuthHideLockConverter>();
                set.Apply();
            });
        }
    }

    public class NetworkSource : MvxTableViewSource
    {
        public NetworkSource(UITableView tableView)
            : base(tableView)
        {
            tableView.RegisterClassForCellReuse(typeof(NetworkCell), NetworkCell.CellId);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = tableView.DequeueReusableCell(NetworkCell.CellId, indexPath);
            cell.BackgroundColor = UIColor.Clear;
            return cell;
        }
    }

    public class DeviceCell : MvxTableViewCell
    {
        public static readonly NSString CellId = new NSString("DeviceCell");

        private UILabel _name;

        DeviceCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.Clear;
            ContentView.DirectionalLayoutMargins = new NSDirectionalEdgeInsets
            {
                Top = 10,
                Bottom = 10,
                Leading = 10,
                Trailing = 10,
            };

            _name = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            ContentView.AddSubview(_name);

            _name.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor).Active = true;
            _name.LeadingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.LeadingAnchor).Active = true;
            _name.TrailingAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TrailingAnchor).Active = true;
            _name.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor).Active = true;

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<DeviceCell, string>();
                set.Bind(_name).For(v => v.Text).To(vm => vm);
                set.Apply();
            });
        }
    }

    public class DeviceSource : MvxTableViewSource
    {
        public DeviceSource(UITableView tableView)
            : base(tableView)
        {
            tableView.RegisterClassForCellReuse(typeof(DeviceCell), DeviceCell.CellId);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = tableView.DequeueReusableCell(DeviceCell.CellId, indexPath);
            cell.BackgroundColor = UIColor.Clear;
            return cell;
        }
    }

    [MvxModalPresentation(WrapInNavigationController = false, ModalPresentationStyle = UIModalPresentationStyle.FormSheet)]
    public class ConnectivityViewController : BaseViewController<ConnectivityViewModel>, IUIAdaptivePresentationControllerDelegate
    {
        private UIView _leftContainer;
        private UIStackView _buttons;
        private UIButton _bluetoothEnabled;
        private UIButton _scanDevices;
        private UIButton _qBoxDetails;
        private UIButton _checkConnection;
        private UIButton _scanNetworks;

        private UILabel _qboxSSID;
        private UILabel _qboxIP;

        private Divider _divider;

        private UIView _rightContainer;
        private UILabel _devicesTitle;
        private NetworkSource _networks;
        private UITableView _networksList;
        private UILabel _networksTitle;
        private DeviceSource _devices;
        private UITableView _devicesList;

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

            _leftContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_leftContainer);

            _buttons = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Spacing = 20,
                Axis = UILayoutConstraintAxis.Vertical,
            };
            _leftContainer.AddSubview(_buttons);

            _bluetoothEnabled = ButtonGenerator.PrimaryButton("Bluetooth");
            _buttons.AddArrangedSubview(_bluetoothEnabled);

            _scanDevices = ButtonGenerator.PrimaryButton("Scan Devices");
            _buttons.AddArrangedSubview(_scanDevices);

            _qBoxDetails = ButtonGenerator.PrimaryButton("Details");
            _buttons.AddArrangedSubview(_qBoxDetails);

            _checkConnection = ButtonGenerator.PrimaryButton("Ping");
            _buttons.AddArrangedSubview(_checkConnection);

            _scanNetworks = ButtonGenerator.PrimaryButton("Scan Networks");
            _buttons.AddArrangedSubview(_scanNetworks);

            _qboxSSID = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.PrimaryText,
                Lines = 0,
            };
            _leftContainer.AddSubview(_qboxSSID);

            _qboxIP = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.PrimaryText,
            };
            _leftContainer.AddSubview(_qboxIP);

            _divider = new Divider
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = DividerAxis.Vertical,
            };
            View.AddSubview(_divider);

            _rightContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_rightContainer);

            _devicesTitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.SecondaryText,
                Font = FontGenerator.GenerateFont(32, UIFontWeight.Regular),
                Text = "Devices",
            };
            _rightContainer.AddSubview(_devicesTitle);

            _devicesList = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                TableFooterView = new UIView(),
                SeparatorColor = ColorPalette.Border,
            };
            _devices = new DeviceSource(_devicesList);
            _devicesList.Source = _devices;
            _rightContainer.AddSubview(_devicesList);

            _networksTitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.SecondaryText,
                Font = FontGenerator.GenerateFont(32, UIFontWeight.Regular),
                Text = "Networks",
            };
            _rightContainer.AddSubview(_networksTitle);

            _networksList = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                TableFooterView = new UIView(),
                SeparatorColor = ColorPalette.Border,
            };
            _networks = new NetworkSource(_networksList);
            _networksList.Source = _networks;
            _rightContainer.AddSubview(_networksList);
        }

        protected override void LayoutView()
        {
            base.LayoutView();

            _leftContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            _leftContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            _leftContainer.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _leftContainer.RightAnchor.ConstraintEqualTo(_divider.LeftAnchor).Active = true;

            _buttons.CenterXAnchor.ConstraintEqualTo(_leftContainer.CenterXAnchor).Active = true;
            _buttons.CenterYAnchor.ConstraintEqualTo(_leftContainer.CenterYAnchor).Active = true;

            _qboxSSID.TopAnchor.ConstraintEqualTo(_buttons.BottomAnchor).Active = true;
            _qboxSSID.CenterXAnchor.ConstraintEqualTo(_buttons.CenterXAnchor).Active = true;

            _qboxIP.TopAnchor.ConstraintEqualTo(_qboxSSID.BottomAnchor).Active = true;
            _qboxIP.CenterXAnchor.ConstraintEqualTo(_qboxSSID.CenterXAnchor).Active = true;

            _divider.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _divider.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
            _divider.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.9f).Active = true;

            _rightContainer.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            _rightContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            _rightContainer.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _rightContainer.LeftAnchor.ConstraintEqualTo(_divider.RightAnchor).Active = true;

            _devicesTitle.TopAnchor.ConstraintEqualTo(_rightContainer.TopAnchor).Active = true;
            _devicesTitle.LeftAnchor.ConstraintEqualTo(_rightContainer.LeftAnchor, 10).Active = true;
            _devicesTitle.RightAnchor.ConstraintEqualTo(_rightContainer.RightAnchor).Active = true;

            _devicesList.LeftAnchor.ConstraintEqualTo(_rightContainer.LeftAnchor).Active = true;
            _devicesList.RightAnchor.ConstraintEqualTo(_rightContainer.RightAnchor).Active = true;
            _devicesList.TopAnchor.ConstraintEqualTo(_devicesTitle.BottomAnchor).Active = true;
            _devicesList.BottomAnchor.ConstraintEqualTo(_rightContainer.CenterYAnchor).Active = true;

            _networksTitle.TopAnchor.ConstraintEqualTo(_devicesList.BottomAnchor).Active = true;
            _networksTitle.LeftAnchor.ConstraintEqualTo(_rightContainer.LeftAnchor, 10).Active = true;
            _networksTitle.RightAnchor.ConstraintEqualTo(_rightContainer.RightAnchor).Active = true;

            _networksList.LeftAnchor.ConstraintEqualTo(_rightContainer.LeftAnchor).Active = true;
            _networksList.RightAnchor.ConstraintEqualTo(_rightContainer.RightAnchor).Active = true;
            _networksList.TopAnchor.ConstraintEqualTo(_networksTitle.BottomAnchor).Active = true;
            _networksList.BottomAnchor.ConstraintEqualTo(_rightContainer.BottomAnchor).Active = true;
        }

        protected override void BindView()
        {
            base.BindView();

            var set = CreateBindingSet();
            set.Bind(_bluetoothEnabled).For("Tap").To(vm => vm.CheckBluetoothCommand);
            set.Bind(_scanDevices).For("Tap").To(vm => vm.ScanDevicesCommand);
            set.Bind(_qBoxDetails).For("Tap").To(vm => vm.QBoxDetailsCommand);
            set.Bind(_checkConnection).For("Tap").To(vm => vm.CheckConnectionCommand);
            set.Bind(_scanNetworks).For("Tap").To(vm => vm.ScanCommand);
            set.Bind(_qboxSSID).To(vm => vm.QBoxSSID);
            set.Bind(_qboxIP).To(vm => vm.QBoxIP);
            set.Bind(_networks).For(v => v.ItemsSource).To(vm => vm.Networks);
            set.Bind(_networks).For(v => v.SelectionChangedCommand).To(vm => vm.JoinNetworkCommand);
            set.Bind(_devices).For(v => v.ItemsSource).To(vm => vm.Devices);
            set.Bind(_devices).For(v => v.SelectionChangedCommand).To(vm => vm.ConnectDeviceCommand);
            set.Apply();
        }

        [Export("presentationControllerDidDismiss:")]
        public void DidDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
        }
    }
}
