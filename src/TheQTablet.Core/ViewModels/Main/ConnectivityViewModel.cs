using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Acr.UserDialogs;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using TheQTablet.Core.Service.Interfaces;

namespace TheQTablet.Core.ViewModels.Main
{
    public enum ConnectivityState
    {
        Loading,
        Loaded,

        ScanningDevices,
        ChooseDevice,
        ConnectingDevice,
        ConnectDeviceFailed,
        ConnectedDevice,

        GettingDetails,
        GotDetails,
        CheckingConnection,
        CheckedConnection,

        ScanningNetworks,
        ChooseNetwork,
        ConnectingNetwork,
        ConnectToNetworkFailed,
        ConnectedToNetworkNoAPI,

        Success,
    }

    public class ConnectivityViewModel : MvxNavigationViewModel
    {
        private readonly IQBoxConnectionService _connectionService;
        private readonly IUserDialogs _userDialogs;

        public MvxAsyncCommand CloseCommand { get; }
        public MvxAsyncCommand<Peripheral> ConnectDeviceCommand { get; }
        public MvxAsyncCommand ScanNetworksCommand { get; }
        public MvxAsyncCommand<WiFiNetwork> JoinNetworkCommand { get; }

        private ConnectivityState _state;
        public ConnectivityState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public string QBoxBTName => _connectionService.QBoxBTName;
        public string QBoxSSID => _connectionService.QBoxSSID;
        public string QBoxIP => _connectionService.QBoxIP;
        public ObservableCollection<WiFiNetwork> Networks => _connectionService.Networks;
        public ObservableCollection<Peripheral> Devices => _connectionService.Devices;

        private EventHandler _bluetoothDeviceDisconnectedEventHandler;
        private EventHandler _bluetoothStateChangedEventHandler;

        public ConnectivityViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService,
            IQBoxConnectionService connectionService,
            IUserDialogs userDialogs
        ) : base(logProvider, navigationService)
        {
            _connectionService = connectionService;
            _userDialogs = userDialogs;

            CloseCommand = new MvxAsyncCommand(Close);
            ConnectDeviceCommand = new MvxAsyncCommand<Peripheral>(ConnectDevice);
            ScanNetworksCommand = new MvxAsyncCommand(ScanNetworks);
            JoinNetworkCommand = new MvxAsyncCommand<WiFiNetwork>(JoinNetwork);

            _connectionService.QBoxBTNameChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxBTName));
            _connectionService.QBoxSSIDChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxSSID));
            _connectionService.QBoxIPChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxIP));
            _connectionService.NetworksChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(Networks));
            _connectionService.DevicesChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(Devices));

            _bluetoothDeviceDisconnectedEventHandler = new EventHandler(async (sender, e) => await BluetoothDeviceDisconnected(sender, e));
            _connectionService.QBoxDisconnected += _bluetoothDeviceDisconnectedEventHandler;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await CheckBluetooth();
            await ScanDevices();
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();

            if (_bluetoothDeviceDisconnectedEventHandler != null)
            {
                _connectionService.QBoxDisconnected -= _bluetoothDeviceDisconnectedEventHandler;
            }
            if (_bluetoothStateChangedEventHandler != null)
            {
                _connectionService.BluetoothStateChanged -= _bluetoothStateChangedEventHandler;
            }
        }

        private async Task Close()
        {
            await NavigationService.Close(this);
        }

        private async Task BluetoothStateChanged(object sender, EventArgs e)
        {
            if(_connectionService.BluetoothState != BluetoothState.PoweredOn)
            {
                await _userDialogs.AlertAsync(new AlertConfig()
                {
                    Title = "Bluetooth Unavailable",
                    Message = "Bluetooth has been disabled, please enable.",
                    OkText = "Exit"

                });
                await Close();
            }
        }

        private async Task BluetoothDeviceDisconnected(object sender, EventArgs e)
        {
            await _userDialogs.AlertAsync(new AlertConfig()
            {
                Title = "Disconnected",
                Message = "The Q device has disconnected, please ensure it is powered on.",
                OkText = "Exit"
            });
            await Close();
        }

        private async Task CheckBluetooth()
        {
            State = ConnectivityState.Loading;
            var success = await _connectionService.EnsureBluetoothEnabled();
            if (!success)
            {
                await Close();
            }
            else
            {
                _bluetoothStateChangedEventHandler = new EventHandler(async (sender, e) => await BluetoothStateChanged(sender, e));
                _connectionService.BluetoothStateChanged += _bluetoothStateChangedEventHandler;
                State = ConnectivityState.Loaded;
            }
        }

        private async Task ScanDevices()
        {
            State = ConnectivityState.ScanningDevices;
            _connectionService.ScanDevices();
            State = ConnectivityState.ChooseDevice;
        }

        private async Task ConnectDevice(Peripheral peripheral)
        {
            State = ConnectivityState.ConnectingDevice;
            var connected = await _connectionService.Connect(peripheral);
            if(connected)
            {
                State = ConnectivityState.ConnectedDevice;

                await GetQBoxDetails();
                var doScan = true;
                if (QBoxIP != null)
                {
                    var success = await CheckConnectionAsync();
                    if (success)
                    {
                        State = ConnectivityState.Success;
                        doScan = false;
                    }
                }
                if (doScan)
                {
                    await ScanNetworks();
                }
            }
            else
            {
                State = ConnectivityState.ConnectDeviceFailed;
            }
        }

        private async Task GetQBoxDetails()
        {
            State = ConnectivityState.GettingDetails;
            await _connectionService.GetQBoxDetails();
            State = ConnectivityState.GotDetails;
        }

        private async Task<bool> CheckConnectionAsync()
        {
            State = ConnectivityState.CheckingConnection;
            var success = await _connectionService.CheckConnection();
            Console.WriteLine(success);
            State = ConnectivityState.CheckedConnection;

            return success;
        }

        private async Task ScanNetworks()
        {
            State = ConnectivityState.ScanningNetworks;
            await _connectionService.ScanNetworks();
            State = ConnectivityState.ChooseNetwork;
        }

        private async Task JoinNetwork(WiFiNetwork network)
        {
            bool joinedSuccess = false;
            if (network.Auth == AuthType.None)
            {
                State = ConnectivityState.ConnectingNetwork;
                joinedSuccess = await _connectionService.ConnectNetwork(network.SSID);
            }
            else if(network.Auth == AuthType.Password)
            {
                var result = await _userDialogs.PromptAsync(new PromptConfig()
                {
                    Title = network.SSID,
                    Message = "Enter network password.",
                    OkText = "Connect",
                    IsCancellable = true,
                    InputType = InputType.Password,
                });
                if (result.Ok)
                {
                    State = ConnectivityState.ConnectingNetwork;
                    joinedSuccess = await _connectionService.ConnectNetwork(network.SSID, result.Text);
                }
            }
            else if(network.Auth == AuthType.UsernamePassword)
            {
                var result = await _userDialogs.LoginAsync(new LoginConfig()
                {
                    Title = network.SSID,
                    Message = "Enter network credentials.",
                    OkText = "Connect",
                });
                if (result.Ok)
                {
                    State = ConnectivityState.ConnectingNetwork;
                    joinedSuccess = await _connectionService.ConnectToNetwork(network.SSID, result.LoginText, result.Password);
                }
            }
            if (joinedSuccess)
            {
                var connectSuccess = await CheckConnectionAsync();
                if (connectSuccess)
                {
                    State = ConnectivityState.Success;
                }
                else
                {
                    State = ConnectivityState.ConnectedToNetworkNoAPI;
                }
            }
            else
            {
                State = ConnectivityState.ConnectToNetworkFailed;
            }
        }
    }
}
