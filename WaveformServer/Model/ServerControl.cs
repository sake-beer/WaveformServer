using System.Net;

namespace WaveformServer.Model
{
    public class ServerControl
    {
        public delegate void TextChanged();
        public delegate void EnabledChanged();
        public delegate void EndButtonEnabledChanged();
        public delegate void ServerStateChanged();

        // Enum
        public enum ServerState
        {
            Idle,
            Error,
            Busy,
        };

        // Property
        public WaveformServer Server
        {
            get { return _server;}
        }
        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; _ipAddressChanged?.Invoke(); }
        }
        public string PortText
        {
            get { return _portText; }
            set { _portText = value; _portTextChanged?.Invoke(); }
        }
        public bool StartButtonEnabled
        {
            get { return _startButtonEnabled; }
            set { _startButtonEnabled = value; _startButtonEnabledChanged?.Invoke(); }
        }
        public bool EndButtunEnabled
        {
            get { return _endButtonEnabled; }
            set { _endButtonEnabled = value; _endButtonEnabledChanged?.Invoke(); }
        }


        // Member
        private WaveformServer? _server;
        private string _ipAddress;
        private string _portText;
        private bool _startButtonEnabled;
        private bool _endButtonEnabled;
        private TextChanged _ipAddressChanged;
        private TextChanged _portTextChanged;
        private EnabledChanged _startButtonEnabledChanged;
        private EnabledChanged _endButtonEnabledChanged;
        private ServerStateChanged _serverStateChanged;
        private TopLog _log;

        // Constructor
        public ServerControl(TopLog log)
        {
            _log = log;
            _log.Log = "Start ServerControl()";
            StartButtonEnabled = true;
            EndButtunEnabled = false;
            UpdateIpAddress();
            PortText = "54321";
        }

        // Method
        public void setIpAddressChanged(TextChanged ipAddressChanged)
        {
            _ipAddressChanged += ipAddressChanged;
        }
        public void setPortTextChanged(TextChanged portTextChanged)
        {
            _portTextChanged += portTextChanged;
        }
        public void setStartButtonEnabledChanged(EnabledChanged startButtonEnabledChanged)
        {
            _startButtonEnabledChanged += startButtonEnabledChanged;
        }
        public void setEndButtonEnabledChanged(EnabledChanged endButtonEnabledChanged)
        {
            _endButtonEnabledChanged += endButtonEnabledChanged;
        }
        public void setServerStateChanged(ServerStateChanged serverStateChanged)
        {
            _serverStateChanged += serverStateChanged;
        }

        public void UpdateIpAddress()
        {
            string hostname = Dns.GetHostName();
            IPAddress[] list = Dns.GetHostAddresses(hostname);
            _ipAddress = list[list.Length - 1].ToString();
            _log.Log = "IP address = " + _ipAddress;
        }
        public void StartServer()
        {
            if (_server != null) return;
            StartButtonEnabled = false;
            EndButtunEnabled = true;
            _server = new WaveformServer(_log);
            if(_server.Status == WaveformServer.ServerState.Null)
            {
                _log.Log = _server.Log;
                _server = null;
                return;
            }
            _log.Log = "Start server (port=" + PortText + ")";
            _server.Start(IpAddress, PortText);
        }
        public void EndServer()
        {
            if (_server == null) return;
            StartButtonEnabled = true;
            EndButtunEnabled = false;
            WaveformServer.End();
            _log.Log = "End server";
            _server = null;
        }
    }
}
