
namespace WaveformServer.Model
{
    public class TopModel
    {
        // Property
        public TopLog Log { get { return _log; } }
        public ServerControl Control { get { return _control; } }

        // Member
        private TopLog _log;
        private ServerControl _control;

        public TopModel()
        {
            _log = new TopLog();
            _control = new ServerControl(_log);

        }


    }
}
