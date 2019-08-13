using System.ComponentModel;
using DreamLib.Data.Models;
using DreamLog.Tools;
using Xamarin.Forms;
using System;

namespace DreamLog.ViewModels
{
    public class DreamCategoryViewModel : DreamCategory, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Color color;

        public DreamCategoryViewModel()
        {
            this.color = Color.Transparent;
        }

        public override string Name
        {
            get { return base.Name; }
            set
            {
                if (!(base.Name?.Equals(value)).GetValueOrDefault())
                {
                    base.Name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        public override string ColorString
        {
            get { return base.ColorString; }
            set { this.Color = UIUtils.GetColorByName(value); }
        }

        public Color Color
        {
            get { return this.color; }
            set
            {
                if (!this.color.Equals(value))
                {
                    string name = UIUtils.GetColorName(value);
                    if (!String.IsNullOrEmpty(name))
                    {
                        base.ColorString = name;
                        this.color = value;

                        this.RaisePropertyChanged("ColorString");
                        this.RaisePropertyChanged("Color");
                    }
                }
            }
        }

        public bool CanBeDeleted
        {
            get { return this.DreamLogEntries?.Count == 0; }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DreamCategoryEditorViewModel : DreamCategoryViewModel
    {
        private ColorSelectionViewModel selectedColor;

        public ColorSelectionViewModel SelectedColor
        {
            get { return this.selectedColor; }
            set
            {
                if (value?.Color != this.selectedColor?.Color)
                {
                    this.selectedColor = value;
                    this.RaisePropertyChanged("SelectedColor");
                    if (!(value is null))
                    {
                        this.Color = value.Color;
                    }
                }
            }
        }
    }
}
