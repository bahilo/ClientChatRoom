using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chatroom.Classes
{
    public class DiscussionGroupNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueToProcess = value as string;
            if(valueToProcess != null)
            {
                int nbCharToDisplay;
                valueToProcess = valueToProcess.Split('-')[0];
                if (valueToProcess.Length > 50)
                    nbCharToDisplay = 50;
                else
                    nbCharToDisplay = valueToProcess.Length;
                valueToProcess = valueToProcess.Substring(0, nbCharToDisplay) + "...";

                return valueToProcess;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
