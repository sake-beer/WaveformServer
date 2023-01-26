
namespace WaveformServer.Model
{
    public class Waveform
    {
        public delegate void EnabledChanged();
        public delegate void LogChanged();

        public struct WaveformParam
        {
            int Bufsize;
            double Period = 1.0;
            double Amp = 1.0;
            double Param = 1.0;
            double Offset = 0.0;
            public WaveformParam(int bufsize, double period, double amp, double param, double offset)
            {
                Bufsize = bufsize;
                Period = period;
                Amp = amp;
                Param = param;
                Offset = offset;
            }
        }


        public void Clear()
        {

        }
    }


}
