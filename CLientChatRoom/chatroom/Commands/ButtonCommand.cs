using chatcommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace chatroom.Commands
{
    public class ButtonCommand<P> : ICommand
    {
        private object _lock = new object();
        private Action<P> _executeAction;
        private Func<P, bool> _canExecuteAction;

        public event EventHandler CanExecuteChanged;

        public ButtonCommand(Action<P> actionToExecute)
        {
            _executeAction = actionToExecute;
        }

        public ButtonCommand(Func<P, bool> canExecute)
        {
            _canExecuteAction = canExecute;
        }

        public ButtonCommand(Action<P> actionToExecute, Func<P, bool> canExecuteAction)
        {
            _executeAction = actionToExecute;
            _canExecuteAction = canExecuteAction;
        }

        public bool CanExecute(Object parameter)
        {
            if (_canExecuteAction != null)
                return _canExecuteAction((P)parameter);

            return false;
        }

        public void Execute(Object parameter)
        {
            if (_executeAction != null)
                _executeAction((P)parameter);
        }

        public void RaiseCanExecuteActionChanged()
        {
            try
            {
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
        }
    }
}
