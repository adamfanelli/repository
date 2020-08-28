using System;
using System.Windows.Documents;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using TimelineLib;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Timeline_Project
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            WindowWidth = 800;
            WindowHeight = 600;

            listOfEvents = new ObservableCollection<TimelineEvent>();

            //Testing
            AddEventToList(new TimelineEvent() { EventName = "Test Event", StartYear = 1900, EndYear = 2000 });
            AddEventToList(new TimelineEvent() { EventName = "Test Event 2", StartYear = 1950, EndYear = 2000 });
            AddEventToList(new TimelineEvent() { EventName = "Test Event 3444444444", StartYear = 1990, EndYear = 2000 });

            //Actions
            AddEventCommand = new RelayCommand(
                delegate ()
                {
                    AddEventToList(new TimelineEvent() { EventName = "Test Event 3444444444", StartYear = 1990, EndYear = 2000 });
                });
        }

        //Actions
        public ICommand AddEventCommand { get; set; }


        //Window Dimensions
        private int windowWidth;
        public int WindowWidth
        {
            get { return windowWidth; }
            set { windowWidth = value; OnPropertyChanged("WindowWidth"); }
        }

        private int windowHeight;
        public int WindowHeight
        {
            get { return windowHeight; }
            set { windowHeight = value; OnPropertyChanged("WindowHeight"); }
        }



        private int numberOfEvents;
        public int NumberOfEvents
        {
            get { return numberOfEvents; }
            set { numberOfEvents = value; OnPropertyChanged("NumberOfEvents"); }
        }


        //List of Events
        private ObservableCollection<TimelineEvent> listOfEvents;
        public ObservableCollection<TimelineEvent> ListOfEvents
        {
            get { return listOfEvents; }
            set { listOfEvents = value; }
        }

        public void AddEventToList(TimelineEvent e)
        {
            ListOfEvents.Add(e);

            NumberOfEvents++;
        }

        #region Timeline Properties

        private int mainLineY;
        public int MainLineY
        {
            get { return mainLineY; }
            set
            {
                mainLineY = value;
                OnPropertyChanged("MainLineY");
            }
        }
        
        private int markerHeight;
        public int MarkerHeight
        {
            get { return markerHeight; }
            set
            {
                markerHeight = value;
                OnPropertyChanged("MarkerHeight");
            }
        }

        private int markerOrigin;
        public int MarkerOrigin
        {
            get { return markerOrigin; }
            set
            {
                markerOrigin = value;
                OnPropertyChanged("MarkerOrigin");
            }
        }

        private int markerInterval;
        public int MarkerInterval
        {
            get { return markerInterval; }
            set
            {
                markerInterval = value;
                OnPropertyChanged("MarkerInterval");
            }
        }


        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
