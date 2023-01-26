using System;
using System.Windows;
using WaveformServer.Model;
using WaveformServer.ViewModel;

namespace WaveformServer.View
{
    public partial class ViewTopLog : Window
    {
        public ViewTopLog(TopModel model)
        {
            DataContext = new ViewModelTopLog(model, LogUpdate);
            InitializeComponent();
        }

        public void LogUpdate()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                TextBoxLog.ScrollToEnd();
            }));
        }
    }
}
