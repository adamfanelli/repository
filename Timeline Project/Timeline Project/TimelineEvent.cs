using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Timeline_Project
{
    public class TimelineEvent : INotifyPropertyChanged
    {
        private string eventName;
        public string EventName
        {
            get { return eventName; }
            set { eventName = value; OnPropertyChanged("EventName");  }
        }

        private int startYear;

        public int StartYear
        {
            get { return startYear; }
            set { startYear = value; OnPropertyChanged("StartYear"); }
        }

        private int endYear;
        public int EndYear
        {
            get { return startYear; }
            set { startYear = value; OnPropertyChanged("EndYear"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
