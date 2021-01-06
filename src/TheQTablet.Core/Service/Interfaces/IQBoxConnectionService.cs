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

    public enum AuthType
    {
        None,
        Password,
        UsernamePassword,
    }

    public class WiFiNetwork
    {
        public string SSID { get; }
        public AuthType Auth { get; }

        public WiFiNetwork(string ssid, AuthType auth)
        {
            SSID = ssid;
            Auth = auth;
        }
    }

    public interface IQBoxConnectionService
    {
        BluetoothState BluetoothState { get; }
        string QBoxSSID { get; }
        event EventHandler QBoxSSIDChanged;
        string QBoxIP { get; }
        event EventHandler QBoxIPChanged;
        ObservableCollection<WiFiNetwork> Networks { get; }
        event EventHandler NetworksChanged;
        ObservableCollection<string> Devices { get; }
        event EventHandler DevicesChanged;

        void ScanDevices();
        void Connect(string peripheral);
        void ScanNetworks();
        void EnsureBluetoothEnabled();
        void GetQBoxDetails();
        Task<bool> CheckConnection();
        void ConnectNetwork(string ssid);
        void ConnectNetwork(string ssid, string password);
        void ConnectToNetwork(string ssid, string username, string password);
    }
}
