using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using chatbusiness;

namespace chatroom.Classes
{
    public class BindBase : INotifyPropertyChanged
    {
        protected Startup _startup;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void setPropertyChange<T>(ref T member, T input, [CallerMemberName] string propertyName = null)
        {
            if (member == null || !member.Equals(input))
                member = input;

            onPopertyChange(propertyName);
        }          

        protected void onPopertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Startup Startup
        {
            get { return _startup; }
            set { setPropertyChange(ref _startup, value); }
        }   
    }
}
