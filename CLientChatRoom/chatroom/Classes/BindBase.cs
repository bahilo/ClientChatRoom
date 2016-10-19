using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using chatbusiness;
using chatroom.ViewModels;
using chatroom.Interfaces;

namespace chatroom.Classes
{
    public class BindBase : IState, INotifyPropertyChanged, IDisposable
    {
        protected Startup _startup;
        protected ConfirmationViewModel _dialog;
        public event PropertyChangedEventHandler PropertyChanged;

        public BindBase()
        {
            _dialog = new ConfirmationViewModel();
            //_startup = new Startup();
        }

        protected void setPropertyChange<T>(ref T member, T input, [CallerMemberName] string propertyName = null)
        {
            if (member == null || !member.Equals(input))
                member = input;

            onPropertyChange(propertyName);
        }          

        protected void onPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Dispose()
        {
            
        }

        public void Handle(Context context, Func<object, object> page)
        {
            var prev = context.PreviousState;
            context.PreviousState = context.NextState;
            context.NextState = prev;
            page(context.NextState);
        }

        public Startup Startup
        {
            get { return _startup; }
            set { setPropertyChange(ref _startup, value); }
        }

        public ConfirmationViewModel Dialog
        {
            get { return _dialog; }
            set { setPropertyChange(ref _dialog, value); }
        }
    }
}
