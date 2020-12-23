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
    public class ConnectivityViewModel : MvxNavigationViewModel
    {
        private readonly IQBoxConnectionService _connectionService;
        private readonly IUserDialogs _userDialogs;

        public MvxAsyncCommand CloseCommand { get; }
        public MvxCommand CheckBluetoothCommand { get; }
        public MvxCommand FindQBoxCommand { get; }
        public MvxCommand QBoxDetailsCommand { get; }
        public MvxAsyncCommand CheckConnectionCommand { get; }
        public MvxCommand ScanCommand { get; }
        public MvxAsyncCommand<string> JoinNetworkCommand { get; }

        public string QBoxSSID => _connectionService.QBoxSSID;
        public string QBoxIP => _connectionService.QBoxIP;
        public ObservableCollection<string> Networks => _connectionService.Networks;

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
            FindQBoxCommand = new MvxCommand(FindQBox);
            QBoxDetailsCommand = new MvxCommand(GetQBoxDetails);
            CheckConnectionCommand = new MvxAsyncCommand(CheckConnectionAsync);
            ScanCommand = new MvxCommand(Scan);
            JoinNetworkCommand = new MvxAsyncCommand<string>(JoinNetwork);

            _connectionService.QBoxSSIDChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxSSID));
            _connectionService.QBoxIPChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(QBoxIP));
            _connectionService.NetworksChanged += (object sender, EventArgs e) => RaisePropertyChanged(nameof(Networks));
        }

        private async Task Close()
        {
            await NavigationService.Close(this);
        }

        private void CheckBluetooth()
        {
            _connectionService.EnsureBluetoothEnabled();
        }

        private void FindQBox()
        {
            _connectionService.Connect();
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

        private async Task JoinNetwork(string networkSSID)
        {
            var result = await _userDialogs.PromptAsync(new PromptConfig()
            {
                Title = networkSSID,
                Message = "Enter network password.",
                OkText = "Send Credentials",
                IsCancellable = true,
                
            });
            if(result.Ok)
            {
                _connectionService.SendNetworkCredentials(networkSSID, result.Text);
            }
        }
    }
}
