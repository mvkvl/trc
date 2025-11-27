using System;
using System.Windows.Input;

namespace TradesAnalyser.Commands
{
    public class CommandDelegate : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public delegate void CommandHandler(object obj);

        private readonly CommandHandler _handler;

        public CommandDelegate(CommandHandler handler)
        {
            this._handler = handler;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _handler?.Invoke(parameter);
        }
    }
}
