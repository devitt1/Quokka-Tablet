using System;

using TheQTablet.Core.Rest.Interfaces;

namespace TheQTablet.Core.Rest.Implementations
{
    public class QSimClient : IQSimClient
    {
        private string _host = "127.0.0.1";
        public string Host {
            get => _host;
            set => _host = value;
        }

        public string BaseURL => "http://" + Host + ":5002/qsim/";

        public QSimClient()
        {
        }
    }
}
