using Microsoft.Win32;
using System;
using System.IO;
using System.Text;

namespace WaveformServer.Model
{
    public class TopLog
    {
        public delegate void EnabledChanged();
        public delegate void LogChanged();

        // Property
        public bool IsEnabled
        {
            get { return _enabled; }
            set { _enabled = value; _enabledChanged?.Invoke(); }
        }
        public string Log
        {
            get { return _log; }
            set
            {
                if (_enabled)
                {
                    _log += TopLogFormat(value);
                    _logChanged?.Invoke();
                }
            }
        }

        // Member
        private bool _enabled = false;
        private string _log = "";
        private EnabledChanged _enabledChanged;
        private LogChanged _logChanged;

        // Constructor
        public TopLog()
        {
        }

        // Method
        public void setEnabledChanged(EnabledChanged enabledChanged)
        {
            _enabledChanged += enabledChanged;
        }
        public void setLogChanged(LogChanged logChanged)
        {
            _logChanged += logChanged;
        }
        public string TopLogFormat(string value)
        {
            return "[" + DateTime.Now.ToString() + "] " + value + "\n";
        }

        public void Clear()
        {
            _log = "";
            Log = "Clear log.";
        }

        public void Save()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "top_log.log";
            dialog.Filter = "LOGファイル|*.log|すべてのファイル|*.*";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == true)
            {
                var utf8_encoding = Encoding.GetEncoding("utf-8");
                using (var writer = new StreamWriter(dialog.FileName, false, utf8_encoding))
                {
                    writer.Write(_log);
                }
            }
        }

        public void SetEnable(bool en)
        {
            if(!IsEnabled && en)
            {
                Clear();
                IsEnabled = true;
                Log = "Start Logging";
            }
            else
            {
                IsEnabled = en;
            }
        }

    }
}
