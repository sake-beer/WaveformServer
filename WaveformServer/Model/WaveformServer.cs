
using Microsoft.VisualBasic;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WaveformServer.Model
{
    public class WaveformServer
    {
        public enum ServerState
        {
            Null,
            Ready,
            Listening,
        };

        // Property
        public static int MaxNodes { get; } = 8;
        public string Log { get; private set; } = "";
        public ServerState Status { get; private set; } = ServerState.Null;

        // Member
        private TopLog _log;
        private IPAddress? _ip;
        private int _port = 0;
        private static ManualResetEvent _done = new ManualResetEvent(false);
        private static List<Waveform?>? _wf;
        private static CancellationTokenSource? _cs;
        private static Socket? _socket;
        private static TopLog? _stLog;

        // Constructor
        public WaveformServer(TopLog log)
        {
            _log = log;
            _stLog = log;
            Status = ServerState.Ready;
            _wf = new();
            for (int i = 0; i < MaxNodes; i++)
                _wf.Append(null);
            _socket = null;
        }

        public async void Start(string ip, string port)
        {
            if (Status != ServerState.Ready) return;
            try
            {
                _ip = IPAddress.Parse(ip);
                _port = Int32.Parse(port);
            }
            catch (Exception e)
            {
                Log = "[Error] @ WaveformServer " + e.ToString();
                Status = ServerState.Ready;
                return;
            }
            Status = ServerState.Listening;
            _cs = new();
            if (!await StartListening(_cs.Token))
            {
                Status = ServerState.Null;
            }
        }

        public static void End()
        {
            _cs?.Cancel();
            _done.Set();
            _cs = null;
        }

        private static void CloseSocket()
        {
            if(_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }

        private async Task<bool> StartListening(CancellationToken ct)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _port);
            if (_socket != null)
            {
                CloseSocket();
                _log.Log += "Close connection.";
            }
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _socket.Bind(endPoint);
                _socket.Listen(10);
                await Task.Run(() =>
                {
                    _log.Log = "Start listening...";
                    while (true)
                    {
                        if (ct.IsCancellationRequested) break;
                        ct.ThrowIfCancellationRequested();
                        _done.Reset();
                        _socket?.BeginAccept(new AsyncCallback(AcceptCallback), _socket);
                        _done.WaitOne();
                    }
                    CloseSocket();
                    _log.Log = "Cancel listening.";
                }, ct);
            }
            catch (Exception e)
            {
                _log.Log = e.ToString();
            }
            return false;
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            if (_socket == null) return;
            _done.Set();
            Socket? server = (Socket?)ar.AsyncState;
            Socket? client = server?.EndAccept(ar);
            StateObject state = new StateObject();
            state.WorkSocket = client;
            try
            {
                client?.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
                _log.Log = "connected from " + IPAddress.Parse(((IPEndPoint)client.RemoteEndPoint).Address.ToString());
            }
            catch(Exception e)
            {
                _log.Log = e.ToString();
            }
        }
        public static void ReceiveCallback(IAsyncResult ar)
        {
            if (_socket == null) return;
            try
            {
                StateObject? state = (StateObject?)ar.AsyncState;
                Socket? client = state?.WorkSocket;
                if (client == null) return;
                int read = client.EndReceive(ar);
                if (read < 1)
                {
                    client?.Shutdown(SocketShutdown.Both);
                    client?.Close();
                    client = null;
                    CloseSocket();
                }
                else
                {
                    state?.SB.Append(Encoding.ASCII.GetString(state.Buffer, 0, read));
                    string str = state.SB.ToString();
                    if ((str.IndexOf("\r") > -1) || (str.IndexOf("\n") > -1))
                    {
                        str = str.Replace("\r", "").Replace("\n", "").Replace("\0", "");
                        state?.SB.Clear();
                        if(_stLog != null) _stLog.Log = "receive '" + str + "'";
                        if(client != null)
                        {
                            byte[]? ret = Receive(client, str);
                            if(ret != null)
                                client.BeginSend(ret, 0, ret.Length, 0, new AsyncCallback(SendCallback), client);
                        }
                    }
                    if((client != null) && (state != null))
                        client?.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch(Exception e)
            {
                if (_stLog != null) _stLog.Log = e.ToString();
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            if (_socket == null) return;
            try
            {
                Socket? client = (Socket?)ar.AsyncState;
                int sent = client.EndSend(ar);
            }
            catch(Exception e)
            {
                if(_stLog != null) _stLog.Log = e.ToString();
            }
        }

        public class StateObject
        {
            public const int BufferSize = 4096;
            public byte[] Buffer = new byte[BufferSize];
            public StringBuilder SB = new StringBuilder();
            public Socket? WorkSocket = null;
        }

        private static byte[]? Receive(Socket? client, string str)
        {
            byte[]? send = null;
            if (client == null) return send;
            string[] strs = str.Split(' ');
            if (strs.Length < 1 || strs.Length > 2) return send;
            string cmd = strs[0].ToLower();
            string[]? param = null;
            if (strs.Length == 2)
                param = strs[1].Split(',');
            string ret = "";
            if (cmd == "bye")
                ret = WfBye(client);
            else if (cmd == "help")
                ret = WfHelp();
            else if (cmd == "nodes")
                ret = WfNodes();
            else if (cmd == "create")
                ret = WfCreate(param);
            else if(cmd == "delete")
                ret = WfDelete(param);


            send = System.Text.Encoding.ASCII.GetBytes(ret);
            return send;

        }

        private static string WfHelp()
        {
            string s = "Waveform Server\n";
            s += "\thelp     : show this message\n";
            s += "\tnodes    : show valid nodelist\n";
            s += "\tcreate N : create node\n";
            s += "\tdelete N : delete node\n";
            s += "\tbye      : disconnect\n";
            return s;
        }

        private static string WfNodes()
        {
            string s = "Valid nodes :";
            for(int i=0; i<_wf.Count; i++)
                if (_wf[i] != null) s += "[" + i.ToString() + "]";
            return s + "\n";
        }

        private static string WfCreate(string[]? param)
        {
            if (param == null) return "-1 '[Error] no param num'\n";
            if (param.Length != 1) return "-1 '[Error] bad param num'\n";
            int node;
            bool fail = false;
            if (!int.TryParse(param[0], out node))
                fail = true;
            else if (node >= _wf?.Count || _wf?[node] != null)
                fail = true;
            else
                _wf[node] = new Waveform();
            return (fail) ? "-1\n" : "0\n";
        }

        private static string WfDelete(string[]? param)
        {
            if (param == null) return "-1 '[Error] no param num'\n";
            if (param.Length != 1) return "-1 '[Error] bad param num'\n";
            int node;
            bool fail = false;
            if (param.Length != 1)
                fail = true;
            else if (!int.TryParse(param[0], out node))
                fail = true;
            else if (_wf[node] == null)
                fail = true;
            else
            {
                _wf[node]?.Clear();
                _wf[node] = null;
            }
            return (fail) ? "-1\n" : "0\n";
        }

        private static string WfBye(Socket? client)
        {
            try
            {
                client?.Shutdown(SocketShutdown.Both);
                client?.Close();
                client = null;
                return "";
            }
            catch (Exception e)
            {
                // _log.Log = e.ToString();
                return "";
            }
        }



    }
}
