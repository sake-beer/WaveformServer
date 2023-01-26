using System.Windows;
using WaveformServer.Model;

namespace WaveformServer.ViewModel
{
    public class ViewModelTopLog : BindingData
    {
        public delegate void LogUpdate();


        // Property
        public Visibility IsVisible
        {
            get { return (_log.IsEnabled) ? Visibility.Visible : Visibility.Hidden; }
            set { if (_log != null) _log.SetEnable(value == Visibility.Visible); }
        }
        public string Log { 
            get { return _log.Log; } 
            set { _log.Log = value; }
        }

        public void NotifyEnabledChanged()
        {
            NotifyPropertyChanged("IsVisible");
        }
        public void NotifyLogChanged()
        {
            NotifyPropertyChanged("Log");
            _logUpdate?.Invoke();
        }

        public BindingCommand.CommandForView SaveButtonClicked { get; private set; }
        public BindingCommand.CommandForView ClearButtonClicked { get; private set; }

        // Member
        private TopLog _log;
        private LogUpdate _logUpdate;

        // Constructor
        public ViewModelTopLog(TopModel model, LogUpdate logUpdate)
        {
            _log = model.Log;
            _logUpdate += logUpdate;
            SaveButtonClicked = new BindingCommand.CommandForView(SaveLog);
            ClearButtonClicked = new BindingCommand.CommandForView(ClearLog);
            _log.setLogChanged(NotifyLogChanged);
            _log.setEnabledChanged(NotifyEnabledChanged);
        }

        // Method
        public void ClearLog()
        {
            _log.Clear();
        }
        public void SaveLog()
        {
            _log.Save();
        }



    }
}
