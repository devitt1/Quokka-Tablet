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
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await CheckBluetooth();
            await ScanDevices();
        }

        private async Task Close()
        {
            await NavigationService.Close(this);
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
            await _connectionService.Connect(peripheral);
            State = ConnectivityState.ConnectedDevice;

            await GetQBoxDetails();
            var doScan = true;
            if(QBoxIP != null)
            {
                var success = await CheckConnectionAsync();
                if (success)
                {
                    State = ConnectivityState.Success;
                    doScan = false;
                }
            }
            if(doScan)
            {
                await ScanNetworks();
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
