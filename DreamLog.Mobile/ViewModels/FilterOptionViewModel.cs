using System.Runtime.CompilerServices;
using System.ComponentModel;
using Xamarin.Forms;
using System;

namespace DreamLog.ViewModels
{
    public class FilterOptionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private FilterOptionCommandParameter tapCommandParameter;
        private Command tapCommand;
        private string title;
        private string icon;

        public string Icon
        {
            get { return this.icon; }
            set
            {
                if(!String.Equals(this.icon, value))
                {
                    this.icon = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Title
        {
            get { return this.title; }
            set
            {
                if(!String.Equals(this.title, value))
                {
                    this.title = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Command TapCommand
        {
            get { return this.tapCommand; }
            set
            {
                if(this.tapCommand != value)
                {
                    this.tapCommand = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public FilterOptionCommandParameter TapCommandParameter
        {
            get { return this.tapCommandParameter; }
            set
            {
                if(this.tapCommandParameter != value)
                {
                    this.tapCommandParameter = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        protected void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public struct FilterOptionCommandParameter
    {
        public static bool operator == (FilterOptionCommandParameter first, FilterOptionCommandParameter second)
        {
            return String.Equals(first.ParameterName, second.ParameterName) && first.Direction == second.Direction;
        }

        public static bool operator !=(FilterOptionCommandParameter first, FilterOptionCommandParameter second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (obj is FilterOptionCommandParameter second)
                return this == second;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public FilterOptionCommandParameter(string parameterName, ListSortDirection direction)
        {
            this.ParameterName = parameterName;
            this.Direction = direction;
        }

        public ListSortDirection Direction { get; }
        public string ParameterName { get; }
    }
}
