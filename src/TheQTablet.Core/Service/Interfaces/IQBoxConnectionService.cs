using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TheQTablet.Core.Service.Interfaces
{
    public enum BluetoothState
    {
        Unknown,
        Resetting,
        Unsupported,
        PoweredOff,
        PoweredOn,
        Unauthorized,
    }

    public interface IQBoxConnectionService
    {
        BluetoothState BluetoothState { get; }
        string QBoxSSID { get; }
        event EventHandler QBoxSSIDChanged;
        string QBoxIP { get; }
        event EventHandler QBoxIPChanged;
        ObservableCollection<string> Networks { get; }
        event EventHandler NetworksChanged;


        void Connect();
        void ScanNetworks();
        void EnsureBluetoothEnabled();
        void GetQBoxDetails();
        Task<bool> CheckConnection();
        void SendNetworkCredentials(string ssid, string password);
    }
}
