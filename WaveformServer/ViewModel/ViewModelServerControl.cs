using WaveformServer.Model;

namespace WaveformServer.ViewModel
{
    public class ViewModelServerControl : BindingData
    {

        // Property
        public bool IsLogEnabled
        {
            get {  return (_log == null)? false : _log.IsEnabled; }
            set { if (_log != null) _log.SetEnable(value); }
        }
        public string IpAddress
        {
            get { return "IP address = " + _control.IpAddress; }
            set { _control.IpAddress = value; }
        }
        public string PortText
        {
            get { return _control.PortText; }
            set { _control.PortText = value; }
        }
        public bool StartButtonEnabled
        {
            get { return _control.StartButtonEnabled; }
            set { _control.StartButtonEnabled = value; }
        }
        public bool EndButtonEnabled
        {
            get { return _control.EndButtunEnabled; }
            set { _control.EndButtunEnabled = value; }
        }

        public void NotifyIpAddressChanged()
        {
            NotifyPropertyChanged("IpAddress");
        }
        public void NotifyPortTextChanged()
        {
            NotifyPropertyChanged("PortText");
        }
        public void NotifyStartButtonEnabledChanged()
        {
            NotifyPropertyChanged("StartButtonEnabled");
        }
        public void NotifyEndButtonEnabledChanged()
        {
            NotifyPropertyChanged("EndButtonEnabled");
        }



        public BindingCommand.CommandForView IpButtonClicked { get; private set; }
        public BindingCommand.CommandForView StartButtonClicked { get; private set; }
        public BindingCommand.CommandForView EndButtonClicked { get; private set; }

        // Member
        private string _ipAddress = "";
        private TopLog _log;
        private ServerControl _control;


        // Constructor
        public ViewModelServerControl(TopModel model)
        {
            _log = model.Log;
            _control = model.Control;
            IpButtonClicked = new BindingCommand.CommandForView(UpdateIpAddress);
            StartButtonClicked = new BindingCommand.CommandForView(StartServer);
            EndButtonClicked = new BindingCommand.CommandForView(EndServer);
            _control.setIpAddressChanged(NotifyIpAddressChanged);
            _control.setPortTextChanged(NotifyPortTextChanged);
            _control.setStartButtonEnabledChanged(NotifyStartButtonEnabledChanged);
            _control.setEndButtonEnabledChanged(NotifyEndButtonEnabledChanged);
        }

        // Method
        public void UpdateIpAddress()
        {
            _control.UpdateIpAddress();
        }
        public void StartServer()
        {
            _control.StartServer();
        }
        public void EndServer()
        {
            _control.EndServer();
        }



    }
}
