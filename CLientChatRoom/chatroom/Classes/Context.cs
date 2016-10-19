using chatroom.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Classes
{
    public class Context
    {
        private IState _previousState;
        private Func<object, object> _page;

        private IState _nexState;

        public Context(Func<object, object> page)
        {
            _page = page;
        }

        public IState NextState
        {
            get { return _nexState; }
            set { _nexState = value; }
        }

        public IState PreviousState
        {
            get { return _previousState; }
            set { _previousState = value; }
        }

        public Func<object, object> Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public void Request()
        {
            if(_previousState != null)
                _previousState.Handle(this, Page);
        }


    }
}
