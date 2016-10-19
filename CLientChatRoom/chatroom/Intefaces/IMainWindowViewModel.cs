using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Intefaces
{
    public interface IMainWindowViewModel
    {
        object getObject(string objectName);
        Object navigation(Object centralPageContent = null);
    }
}
