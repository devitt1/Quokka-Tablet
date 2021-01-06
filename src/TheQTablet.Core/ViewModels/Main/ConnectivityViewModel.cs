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
    public enum ConnecivityState
    {
        Loading,
        ScanningDevices,
        ChooseDevice,
        ConnectingDevice,
        ScanningNetworks,
        ChooseNetwork,
        ConnectingNetwork,
        Success,
    }

    public class ConnectivityViewModel : MvxNavigationViewModel
    {
        private readonly IQBoxConnectionService _connectionService;
        private readonly IUserDialogs _userDialogs;

        public MvxAsyncCommand CloseCommand { get; }
        public MvxCommand CheckBluetoothCommand { get; }
        public MvxCommand ScanDevicesCommand { get; }
        public MvxCommand<string> ConnectDeviceCommand { get; }
        public MvxCommand QBoxDetailsCommand { get; }
        public MvxAsyncCommand CheckConnectionCommand { get; }
        public MvxCommand ScanCommand { get; }
        public MvxAsyncCommand<WiFiNetwork> JoinNetworkCommand { get; }

        public ConnecivityState State;

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
            CheckBluetoothCommand = new MvxCommand(CheckBluetooth);
            ScanDevicesCommand = new MvxCommand(ScanDevices);
            ConnectDeviceCommand = new MvxCommand<string>(ConnectDevice);
            QBoxDetailsCommand = new MvxCommand(GetQBoxDetails);
            CheckConnectionCommand = new MvxAsyncCommand(CheckConnectionAsync);
            ScanCommand = new MvxCommand(Scan);
            JoinNetworkCommand = new MvxAsyncCommand<WiFiNetwork>(JoinNetwork);

            _connectionService.QBoxSSIDChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxSSID));
            _connectionService.QBoxIPChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxIP));
            _connectionService.NetworksChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(Networks));
            _connectionService.DevicesChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(Devices));
        }

        private async Task Close()
        {
            await NavigationService.Close(this);
        }

        private void CheckBluetooth()
        {
            _connectionService.EnsureBluetoothEnabled();
        }

        private void ScanDevices()
        {
            _connectionService.ScanDevices();
        }

        private void ConnectDevice(string peripheral)
        {
            _connectionService.Connect(peripheral);
        }

        private void GetQBoxDetails()
        {
            _connectionService.GetQBoxDetails();
        }

        private async Task CheckConnectionAsync()
        {
            var success = await _connectionService.CheckConnection();
            Console.WriteLine(success);
        }

        private void Scan()
        {
            _connectionService.ScanNetworks();
        }

        private async Task JoinNetwork(WiFiNetwork network)
        {
            if(network.Auth == AuthType.None)
            {
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
                    _connectionService.ConnectNetwork(network.SSID, result.LoginText, result.Password);
                }
            }
        }
    }
}
