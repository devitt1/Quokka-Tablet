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
                case ConnectivityState.ConnectingDevice: return "Connecting Devices";
                case ConnectivityState.GettingDetails: return "Getting Details";
                case ConnectivityState.CheckingConnection: return "Checking Connection";
                case ConnectivityState.ScanningNetworks: return "Scanning Networks";
                case ConnectivityState.ConnectingNetwork: return "Connecting";
                default:
                    return null;
            }
        }
    }

    public class ItemTable<SourceType> : UIView where SourceType : MvxTableViewSource
    {
        private UILabel _title;
        private UITableView _list;
        private SourceType _source;

        public SourceType Source => _source;

        public ItemTable(string title)
        {
            TranslatesAutoresizingMaskIntoConstraints = false;

            _title = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.SecondaryText,
                Font = FontGenerator.GenerateFont(32, UIFontWeight.Regular),
                Text = title,
            };
            AddSubview(_title);

            _list = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                TableFooterView = new UIView(),
                SeparatorColor = ColorPalette.Border,
            };
            _source = (SourceType)Activator.CreateInstance(typeof(SourceType), _list);//new DeviceSource(_list);
            _list.Source = _source;
            AddSubview(_list);

            _title.TopAnchor.ConstraintEqualTo(_title.Superview.TopAnchor).Active = true;
            _title.LeftAnchor.ConstraintEqualTo(_title.Superview.LeftAnchor).Active = true;
            _title.RightAnchor.ConstraintEqualTo(_title.Superview.RightAnchor).Active = true;

            _list.LeftAnchor.ConstraintEqualTo(_list.Superview.LeftAnchor).Active = true;
            _list.RightAnchor.ConstraintEqualTo(_list.Superview.RightAnchor).Active = true;
            _list.TopAnchor.ConstraintEqualTo(_title.BottomAnchor).Active = true;
            _list.BottomAnchor.ConstraintEqualTo(_list.Superview.BottomAnchor).Active = true;
        }
    }

    [MvxModalPresentation(WrapInNavigationController = false, ModalPresentationStyle = UIModalPresentationStyle.FormSheet)]
    public class ConnectivityViewController : BaseViewController<ConnectivityViewModel>, IUIAdaptivePresentationControllerDelegate
    {
        private UIView _loadingContainer;
        private UIImageView _loading;
        private UILabel _loadingText;

        private UIView _container;

        private UIView _leftContainer;
        private UIStackView _buttons;
        private UIButton _bluetoothEnabled;
        private UIButton _scanDevices;
        private UIButton _qBoxDetails;
        private UIButton _checkConnection;
        private UIButton _scanNetworks;

        private UILabel _qboxName;
        private UILabel _qboxSSID;
        private UILabel _qboxIP;

        private Divider _divider;

        private UIView _rightContainer;

        private UILabel _success;

        private ItemTable<DeviceSource> _devicesContainer;
        //private UILabel _devicesTitle;
        //private DeviceSource _devices;
        //private UITableView _devicesList;

        private ItemTable<NetworkSource> _networksContainer;
        //private UILabel _networksTitle;
        //private NetworkSource _networks;
        //private UITableView _networksList;

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

            _container = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_container);

            _leftContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _container.AddSubview(_leftContainer);

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

            _qBoxDetails = ButtonGenerator.PrimaryButton("Get Details");
            _buttons.AddArrangedSubview(_qBoxDetails);

            _checkConnection = ButtonGenerator.PrimaryButton("Check Connection");
            _buttons.AddArrangedSubview(_checkConnection);

            _scanNetworks = ButtonGenerator.PrimaryButton("Scan Networks");
            _buttons.AddArrangedSubview(_scanNetworks);

            _qboxName = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular),
            };
            _leftContainer.AddSubview(_qboxName);

            _qboxSSID = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular),
                Lines = 0,
            };
            _leftContainer.AddSubview(_qboxSSID);

            _qboxIP = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular),
            };
            _leftContainer.AddSubview(_qboxIP);

            _divider = new Divider
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = DividerAxis.Vertical,
            };
            _container.AddSubview(_divider);

            _rightContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Leading = 10,
                    Trailing = 10,
                    Top = 10,
                    Bottom = 10,
                },
            };
            _container.AddSubview(_rightContainer);

            _success = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(40, UIFontWeight.Regular),
                Text = "Successfully connected to The Q server"
            };
            _rightContainer.AddSubview(_success);

            _devicesContainer = new ItemTable<DeviceSource>("Devices");
            _rightContainer.AddSubview(_devicesContainer);

            _networksContainer = new ItemTable<NetworkSource>("Networks");
            _rightContainer.AddSubview(_networksContainer);

            //_devicesTitle = new UILabel
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    TextColor = ColorPalette.SecondaryText,
            //    Font = FontGenerator.GenerateFont(32, UIFontWeight.Regular),
            //    Text = "Devices",
            //};
            //_devicesContainer.AddSubview(_devicesTitle);

            //_devicesList = new UITableView
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    BackgroundColor = UIColor.Clear,
            //    TableFooterView = new UIView(),
            //    SeparatorColor = ColorPalette.Border,
            //};
            //_devices = new DeviceSource(_devicesList);
            //_devicesList.Source = _devices;
            //_devicesContainer.AddSubview(_devicesList);

            //_networksContainer = new UIView
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //};
            //_container.AddSubview(_networksContainer);

            //_networksTitle = new UILabel
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    TextColor = ColorPalette.SecondaryText,
            //    Font = FontGenerator.GenerateFont(32, UIFontWeight.Regular),
            //    Text = "Networks",
            //};
            //_networksContainer.AddSubview(_networksTitle);

            //_networksList = new UITableView
            //{
            //    TranslatesAutoresizingMaskIntoConstraints = false,
            //    BackgroundColor = UIColor.Clear,
            //    TableFooterView = new UIView(),
            //    SeparatorColor = ColorPalette.Border,
            //};
            //_networks = new NetworkSource(_networksList);
            //_networksList.Source = _networks;
            //_networksContainer.AddSubview(_networksList);
        }

        protected override void LayoutView()
        {
            base.LayoutView();

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

            _container.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            _container.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            _container.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _container.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;

            _leftContainer.LeftAnchor.ConstraintEqualTo(_container.LeftAnchor).Active = true;
            _leftContainer.TopAnchor.ConstraintEqualTo(_container.TopAnchor).Active = true;
            _leftContainer.BottomAnchor.ConstraintEqualTo(_container.BottomAnchor).Active = true;
            _leftContainer.RightAnchor.ConstraintEqualTo(_divider.LeftAnchor).Active = true;

            _buttons.CenterXAnchor.ConstraintEqualTo(_leftContainer.CenterXAnchor).Active = true;
            _buttons.CenterYAnchor.ConstraintEqualTo(_leftContainer.CenterYAnchor).Active = true;

            _qboxName.TopAnchor.ConstraintEqualTo(_buttons.BottomAnchor).Active = true;
            _qboxName.CenterXAnchor.ConstraintEqualTo(_buttons.CenterXAnchor).Active = true;

            _qboxSSID.TopAnchor.ConstraintEqualTo(_qboxName.BottomAnchor).Active = true;
            _qboxSSID.CenterXAnchor.ConstraintEqualTo(_qboxName.CenterXAnchor).Active = true;

            _qboxIP.TopAnchor.ConstraintEqualTo(_qboxSSID.BottomAnchor).Active = true;
            _qboxIP.CenterXAnchor.ConstraintEqualTo(_qboxSSID.CenterXAnchor).Active = true;

            _divider.CenterXAnchor.ConstraintEqualTo(_container.CenterXAnchor).Active = true;
            _divider.CenterYAnchor.ConstraintEqualTo(_container.CenterYAnchor).Active = true;
            _divider.HeightAnchor.ConstraintEqualTo(_container.HeightAnchor, 0.9f).Active = true;

            _rightContainer.RightAnchor.ConstraintEqualTo(_container.RightAnchor).Active = true;
            _rightContainer.TopAnchor.ConstraintEqualTo(_container.TopAnchor).Active = true;
            _rightContainer.BottomAnchor.ConstraintEqualTo(_container.BottomAnchor).Active = true;
            _rightContainer.LeftAnchor.ConstraintEqualTo(_divider.RightAnchor).Active = true;

            _success.CenterXAnchor.ConstraintEqualTo(_rightContainer.CenterXAnchor).Active = true;
            _success.CenterYAnchor.ConstraintEqualTo(_rightContainer.CenterYAnchor).Active = true;

            _devicesContainer.TopAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _devicesContainer.BottomAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.BottomAnchor).Active = true;
            _devicesContainer.LeftAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _devicesContainer.RightAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.RightAnchor).Active = true;

            //_devicesTitle.TopAnchor.ConstraintEqualTo(_devicesTitle.Superview.TopAnchor).Active = true;
            //_devicesTitle.LeftAnchor.ConstraintEqualTo(_devicesTitle.Superview.LeftAnchor).Active = true;
            //_devicesTitle.RightAnchor.ConstraintEqualTo(_devicesTitle.Superview.RightAnchor).Active = true;

            //_devicesList.LeftAnchor.ConstraintEqualTo(_devicesList.Superview.LeftAnchor).Active = true;
            //_devicesList.RightAnchor.ConstraintEqualTo(_devicesList.Superview.RightAnchor).Active = true;
            //_devicesList.TopAnchor.ConstraintEqualTo(_devicesTitle.BottomAnchor).Active = true;
            //_devicesList.BottomAnchor.ConstraintEqualTo(_devicesList.Superview.BottomAnchor).Active = true;

            _networksContainer.TopAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _networksContainer.BottomAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.BottomAnchor).Active = true;
            _networksContainer.LeftAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _networksContainer.RightAnchor.ConstraintEqualTo(_rightContainer.LayoutMarginsGuide.RightAnchor).Active = true;

            //_networksTitle.TopAnchor.ConstraintEqualTo(_networksTitle.Superview.TopAnchor).Active = true;
            //_networksTitle.LeftAnchor.ConstraintEqualTo(_networksTitle.Superview.LeftAnchor).Active = true;
            //_networksTitle.RightAnchor.ConstraintEqualTo(_networksTitle.Superview.RightAnchor).Active = true;

            //_networksList.LeftAnchor.ConstraintEqualTo(_networksList.Superview.LeftAnchor).Active = true;
            //_networksList.RightAnchor.ConstraintEqualTo(_networksList.Superview.RightAnchor).Active = true;
            //_networksList.TopAnchor.ConstraintEqualTo(_networksTitle.BottomAnchor).Active = true;
            //_networksList.BottomAnchor.ConstraintEqualTo(_networksList.Superview.BottomAnchor).Active = true;
        }

        static ConnectivityState[] _loadingStates = {
            ConnectivityState.Loading,
            ConnectivityState.ScanningDevices,
            ConnectivityState.ConnectingDevice,
            ConnectivityState.GettingDetails,
            ConnectivityState.CheckingConnection,
            ConnectivityState.ScanningNetworks,
            ConnectivityState.ConnectingNetwork,
        };
        static ConnectivityState[] _loadedStates = {
            ConnectivityState.Loaded,
            ConnectivityState.ChooseDevice,
            ConnectivityState.ConnectedDevice,
            ConnectivityState.GotDetails,
            ConnectivityState.CheckedConnection,
            ConnectivityState.ChooseNetwork,
            ConnectivityState.ConnectedNetwork,
        };

        protected override void BindView()
        {
            base.BindView();

            var set = CreateBindingSet();
            set.Bind(_container).For(v => v.Hidden).To(vm => vm.State).WithConversion<ShowDuringStateConverter>(_loadingStates);
            set.Bind(_loadingContainer).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(_loadingStates);
            set.Bind(_loadingText).To(vm => vm.State).WithConversion<LoadingTextConverter>();

            set.Bind(_bluetoothEnabled).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(_loadedStates);
            set.Bind(_scanDevices).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.Loaded,
            });
            set.Bind(_devicesContainer).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.ChooseDevice,
            });
            set.Bind(_qBoxDetails).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.ConnectedDevice,
                ConnectivityState.GotDetails,
                ConnectivityState.CheckedConnection,
                ConnectivityState.ChooseNetwork,
                ConnectivityState.ConnectedNetwork,
            });
            set.Bind(_checkConnection).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.GotDetails,
                ConnectivityState.CheckedConnection,
                ConnectivityState.ChooseNetwork,
                ConnectivityState.ConnectedNetwork,
            });
            set.Bind(_scanNetworks).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.CheckedConnection,
            });
            set.Bind(_networksContainer).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.ChooseNetwork,
            });
            set.Bind(_success).For("Visible").To(vm => vm.State).WithConversion<ShowDuringStateConverter>(new ConnectivityState[] {
                ConnectivityState.Success,
            });

            set.Bind(_bluetoothEnabled).For("Tap").To(vm => vm.CheckBluetoothCommand);
            set.Bind(_scanDevices).For("Tap").To(vm => vm.ScanDevicesCommand);
            set.Bind(_qBoxDetails).For("Tap").To(vm => vm.QBoxDetailsCommand);
            set.Bind(_checkConnection).For("Tap").To(vm => vm.CheckConnectionCommand);
            set.Bind(_scanNetworks).For("Tap").To(vm => vm.ScanCommand);
            set.Bind(_qboxName).To(vm => vm.QBoxBTName);
            set.Bind(_qboxSSID).To(vm => vm.QBoxSSID);
            set.Bind(_qboxIP).To(vm => vm.QBoxIP);
            set.Bind(_networksContainer.Source).For(v => v.ItemsSource).To(vm => vm.Networks).WithConversion<TestConverter>();
            set.Bind(_networksContainer.Source).For(v => v.SelectionChangedCommand).To(vm => vm.JoinNetworkCommand);
            set.Bind(_devicesContainer.Source).For(v => v.ItemsSource).To(vm => vm.Devices);
            set.Bind(_devicesContainer.Source).For(v => v.SelectionChangedCommand).To(vm => vm.ConnectDeviceCommand);
            set.Apply();
        }

        [Export("presentationControllerDidDismiss:")]
        public void DidDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
        }
    }
}
