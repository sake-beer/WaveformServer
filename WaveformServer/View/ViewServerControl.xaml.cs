using System.Windows;
using WaveformServer.Model;
using WaveformServer.ViewModel;


namespace WaveformServer.View
{
    public partial class ViewServerControl : Window
    {
        public ViewServerControl(TopModel model)
        {
            DataContext = new ViewModelServerControl(model);
            InitializeComponent();
        }
    }
}
