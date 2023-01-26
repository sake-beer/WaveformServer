using System.ComponentModel;

namespace WaveformServer.ViewModel
{
    public class BindingData : INotifyPropertyChanged
    {
        // Data Binding
        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(PropertyName);
                PropertyChanged?.Invoke(this, e);
            }
        }

    }
}
