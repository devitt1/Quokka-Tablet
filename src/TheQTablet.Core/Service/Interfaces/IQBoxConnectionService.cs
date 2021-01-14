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

    public class Peripheral
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
    }

    public interface IQBoxConnectionService
    {
        BluetoothState BluetoothState { get; }
        event EventHandler BluetoothStateChanged;
        event EventHandler QBoxDisconnected;

        string QBoxBTName { get; }
        event EventHandler QBoxBTNameChanged;
        string QBoxSSID { get; }
        event EventHandler QBoxSSIDChanged;
        string QBoxIP { get; }
        event EventHandler QBoxIPChanged;
        ObservableCollection<WiFiNetwork> Networks { get; }
        event EventHandler NetworksChanged;
        ObservableCollection<Peripheral> Devices { get; }
        event EventHandler DevicesChanged;

        void ScanDevices();
        Task<bool> Connect(Peripheral peripheral);
        Task ScanNetworks();
        Task<bool> EnsureBluetoothEnabled();
        Task GetQBoxDetails();
        Task<bool> CheckConnection();
        Task<bool> ConnectNetwork(string ssid);
        Task<bool> ConnectNetwork(string ssid, string password);
        Task<bool> ConnectToNetwork(string ssid, string username, string password);
    }
}
