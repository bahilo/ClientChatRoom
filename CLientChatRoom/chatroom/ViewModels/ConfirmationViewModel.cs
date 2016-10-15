using chatcommon.Classes;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.ViewModels
{
    public class ConfirmationViewModel : INotifyPropertyChanged
    {
        string _message;
        int _response;
        bool _isDialogOpen;
        bool _isLeftBarClosed;

        public event PropertyChangedEventHandler PropertyChanged;

        public ConfirmationViewModel()
        {
            _message = "";
        }

        public void onPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string TxtMessage
        {
            get { return _message; }
            set { _message = value; onPropertyChange("TxtMessage"); }
        }

        public int Response
        {
            get { return _response; }
            set { _response = value; onPropertyChange("Response"); }
        }

        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set { _isDialogOpen = value; onPropertyChange("IsDialogOpen"); }
        }

        public bool IsLeftBarClosed
        {
            get { return _isLeftBarClosed; }
            set { _isLeftBarClosed = value; onPropertyChange("IsLeftBarClosed"); }
        }

        public async Task<int> show(string message)
        {
            IsDialogOpen = false;
            TxtMessage = message;
            object result = await DialogHost.Show(this, "RootDialog");
            if ((result as int?) != null)
                Response = (int)result;
            return Response;
        }

        public async Task<int> show(object viewModel)
        {
            IsDialogOpen = false;
            object result = await DialogHost.Show(viewModel, "RootDialog");
            if ((result as int?) != null)
                Response = (int)result;

            return Response;
        }

        public async void showSearch(string message)
        {
            TxtMessage = message;
            IsDialogOpen = false;
            object result = await DialogHost.Show(new Views.ConfirmationSearchView(), "RootDialog");
        }
    }
}
