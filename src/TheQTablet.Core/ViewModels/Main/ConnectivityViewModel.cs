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
        ConnectedDevice,

        GettingDetails,
        GotDetails,
        CheckingConnection,
        CheckedConnection,

        ScanningNetworks,
        ChooseNetwork,
        ConnectingNetwork,
        ConnectedNetwork,
    }

    public class ConnectivityViewModel : MvxNavigationViewModel
    {
        private readonly IQBoxConnectionService _connectionService;
        private readonly IUserDialogs _userDialogs;

        public MvxAsyncCommand CloseCommand { get; }
        public MvxAsyncCommand CheckBluetoothCommand { get; }
        public MvxAsyncCommand ScanDevicesCommand { get; }
        public MvxAsyncCommand<object> ConnectDeviceCommand { get; }
        public MvxAsyncCommand QBoxDetailsCommand { get; }
        public MvxAsyncCommand CheckConnectionCommand { get; }
        public MvxAsyncCommand ScanCommand { get; }
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
        public ObservableCollection<string> Devices => _connectionService.Devices;

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
            CheckBluetoothCommand = new MvxAsyncCommand(CheckBluetooth);
            ScanDevicesCommand = new MvxAsyncCommand(ScanDevices);
            ConnectDeviceCommand = new MvxAsyncCommand<object>(ConnectDevice);
            QBoxDetailsCommand = new MvxAsyncCommand(GetQBoxDetails);
            CheckConnectionCommand = new MvxAsyncCommand(CheckConnectionAsync);
            ScanCommand = new MvxAsyncCommand(Scan);
            JoinNetworkCommand = new MvxAsyncCommand<WiFiNetwork>(JoinNetwork);

            _connectionService.QBoxBTNameChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxBTName));
            _connectionService.QBoxSSIDChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxSSID));
            _connectionService.QBoxIPChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxIP));
            _connectionService.NetworksChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(Networks));
            _connectionService.DevicesChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(Devices));

            //CheckBluetoothCommand.ExecuteAsync();
            State = ConnectivityState.Loaded;
        }

        private async Task Close()
        {
            await NavigationService.Close(this);
        }

        private async Task CheckBluetooth()
        {
            State = ConnectivityState.Loading;
            _connectionService.EnsureBluetoothEnabled();
            await Task.Delay(500);
            State = ConnectivityState.Loaded;
            // scan devices
        }

        private async Task ScanDevices()
        {
            State = ConnectivityState.ScanningDevices;
            _connectionService.ScanDevices();
            await Task.Delay(500);
            State = ConnectivityState.ChooseDevice;
        }

        private async Task ConnectDevice(object peripheral)
        {
            State = ConnectivityState.ConnectingDevice;
            _connectionService.Connect(peripheral);
            await Task.Delay(500);
            State = ConnectivityState.ConnectedDevice;
            // scan networks
        }

        private async Task GetQBoxDetails()
        {
            State = ConnectivityState.GettingDetails;
            _connectionService.GetQBoxDetails();
            await Task.Delay(500);
            State = ConnectivityState.GotDetails;
        }

        private async Task CheckConnectionAsync()
        {
            State = ConnectivityState.CheckingConnection;
            var success = await _connectionService.CheckConnection();
            Console.WriteLine(success);
            await Task.Delay(500);
            State = ConnectivityState.CheckedConnection;
        }

        private async Task Scan()
        {
            State = ConnectivityState.ScanningNetworks;
            _connectionService.ScanNetworks();
            await Task.Delay(500);
            State = ConnectivityState.ChooseNetwork;
        }

        private async Task JoinNetwork(WiFiNetwork network)
        {
            if (network.Auth == AuthType.None)
            {
                State = ConnectivityState.ConnectingNetwork;
                _connectionService.ConnectNetwork(network.SSID);
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
                    _connectionService.ConnectNetwork(network.SSID, result.Text);
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
                    _connectionService.ConnectToNetwork(network.SSID, result.LoginText, result.Password);
                }
            }
            await Task.Delay(500);
            State = ConnectivityState.ConnectedNetwork;
        }
    }
}
