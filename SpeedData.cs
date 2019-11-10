using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SSRSpeedMonitor
{
    public class SpeedData : INotifyPropertyChanged
    {
        public string name { get; set; }
        public string server { get; set; }

        private int _ping_times;
        public int ping_times
        {
            get
            {
                return this._ping_times;
            }
            set
            {
                if (this._ping_times != value)
                {
                    this._ping_times = value;
                    OnPropertyChanged("ping_times");
                }
            }
        }

        public int _ping_times_success;
        public int ping_times_success
        {
            get
            {
                return this._ping_times_success;
            }
            set
            {
                if (this._ping_times_success != value)
                {
                    this._ping_times_success = value;
                    OnPropertyChanged("ping_times_success");
                }
            }
        }

        public int _ping_times_faild;
        public int ping_times_faild
        {
            get
            {
                return this._ping_times_faild;
            }
            set
            {
                if (this._ping_times_faild != value)
                {
                    this._ping_times_faild = value;
                    OnPropertyChanged("ping_times_faild");
                }
            }
        }

        public int _delay_last;
        public int delay_last
        {
            get
            {
                return this._delay_last;
            }
            set
            {
                if (this._delay_last != value)
                {
                    this._delay_last = value;
                    OnPropertyChanged("delay_last");
                }
            }
        }

        private int _delay_average;
        public int delay_average
        {
            get
            {
                return this._delay_average;
            }
            set
            {
                if (this._delay_average != value)
                {
                    this._delay_average = value;
                    OnPropertyChanged("delay_average");
                }
            }
        }

        public int _delay_min;
        public int delay_min
        {
            get
            {
                return this._delay_min;
            }
            set
            {
                if (this._delay_min != value)
                {
                    this._delay_min = value;
                    OnPropertyChanged("delay_min");
                }
            }
        }

        public int _delay_max;
        public int delay_max
        {
            get
            {
                return this._delay_max;
            }
            set
            {
                if (this._delay_max != value)
                {
                    this._delay_max = value;
                    OnPropertyChanged("delay_max");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
