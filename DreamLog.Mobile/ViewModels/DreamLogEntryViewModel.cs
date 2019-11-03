using System.ComponentModel;
using DreamLog.Data.Models;
using DreamLog.Tools;
using System;

namespace DreamLog.ViewModels
{
    public class DreamLogEntryViewModel : DreamLogEntry, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DreamCategoryViewModel selectedCategory = null;

        public override string Title
        {
            get { return base.Title; }
            set
            {
                if (!(base.Title?.Equals(value)).GetValueOrDefault())
                {
                    base.Title = value;
                    this.RaisePropertyChanged("Title");
                }
            }
        }

        public override string Content
        {
            get { return base.Content; }
            set
            {
                if (!(base.Content?.Equals(value)).GetValueOrDefault())
                {
                    base.Content = value;
                    this.RaisePropertyChanged("Content");
                }
            }
        }

        public override DateTime Date
        {
            get { return base.Date; }
            set
            {
                if (base.Date != value)
                {
                    base.Date = value;
                    this.RaisePropertyChanged("Date");
                }
            }
        }

        public override DreamCategory Category
        {
            get { return base.Category; }
            set
            {
                base.Category = value;
                DreamCategoryViewModel update = new DreamCategoryViewModel();
                update.Update(base.Category);
                this.SelectedCategory = update;
            }
        }

        public DreamCategoryViewModel SelectedCategory
        {
            get { return this.selectedCategory; }
            set
            {
                if (this.selectedCategory?.CategoryId != value?.CategoryId)
                {
                    this.selectedCategory = value;
                    this.FK_CategoryId = value.CategoryId;
                    this.RaisePropertyChanged("SelectedCategory");
                }
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
