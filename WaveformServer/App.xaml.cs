using System.Windows;
using WaveformServer.Model;
using WaveformServer.View;

namespace WaveformServer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TopModel model = new TopModel();


            ViewTopLog viewTopLog = new ViewTopLog(model);
            ViewServerControl viewServerControl = new ViewServerControl(model);
            viewServerControl.Show();



        }

    }
}
