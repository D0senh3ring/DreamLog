using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DreamLib.Tools;
using DreamLog.Models;
using DreamLib.Data.Models;
using DreamLog.ViewModels;
using System.Linq;
using System.Collections.ObjectModel;

namespace DreamLog.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class CreateEditLogEntryPage : ContentPage
    {
        private int selectedCategoryIndex;

        public ObservableCollection<DreamCategoryViewModel> Categories { get; set; }
        public DreamLogEntryViewModel Entry { get; set; }

        public CreateEditLogEntryPage(IEnumerable<DreamCategoryViewModel> categories) : this(new DreamLogEntryViewModel() { Date = DateTime.Today }, categories)
        {
            this.Entry.SelectedCategory = this.Categories.First();
            this.IsEdit = false;

            this.Title = "Create log entry";
        }

        public CreateEditLogEntryPage(DreamLogEntryViewModel entry, IEnumerable<DreamCategoryViewModel> categories)
        {
            this.Categories = new ObservableCollection<DreamCategoryViewModel>(categories);
            this.Entry = entry;

            this.Title = "Edit log entry";
            this.InitializeComponent();

            this.IsEdit = true;

            if (this.Entry.FK_CategoryId.HasValue)
                //this.Entry.SelectedCategory = this.Categories.Single(_cat => _cat.CategoryId == this.Entry.FK_CategoryId);
                this.SelectedCategoryIndex = this.Categories.IndexOf(_category => _category.CategoryId == this.Entry.FK_CategoryId);
            else
                this.SelectedCategoryIndex = this.Categories.IndexOf(_category => _category.CategoryId == -1);
                //this.Entry.SelectedCategory = this.Categories.Single(_cat => _cat.CategoryId == -1);

            this.BindingContext = this;
        }

        public bool IsEdit { get; private set; }

        public int SelectedCategoryIndex
        {
            get { return this.selectedCategoryIndex; }
            set
            {
                if(this.selectedCategoryIndex != value)
                {
                    this.selectedCategoryIndex = value;
                    if(!(this.Entry is null))
                        this.Entry.SelectedCategory = this.Categories[value];
                    this.OnPropertyChanged("SelectedCategoryIndex");
                }
            }
        }

        private async void OnSaveDreamLogEntryClicked(object sender, EventArgs e)
        {
            if (this.ValidateLogEntry())
            {
                MessagingCenter.Send(this, this.IsEdit ? MessagingCenterMessages.EditLogEntry : MessagingCenterMessages.AddLogEntry, this.Entry);
                
                await this.Navigation.PopModalAsync();
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await this.Navigation.PopModalAsync();
        }

        private bool ValidateLogEntry()
        {
            if (String.IsNullOrEmpty(this.Entry.Title))
                return false;
            else if (String.IsNullOrEmpty(this.Entry.Content))
                return false;

            return true;
        }
    }
}