using System;
using System.Windows.Input;

namespace WaveformServer.ViewModel
{
    public class BindingCommand
    {
        // Command for view
        public delegate void Command();
        public class CommandForView : ICommand
        {
            private Command _command;
            public event EventHandler CanExecuteChanged;
            public bool CanExecute(object parameter) { return true; }
            public void Execute(object parameter)
            {
                _command?.Invoke();
            }
            public CommandForView(Command command)
            {
                _command = command;
            }
        }


    }
}
