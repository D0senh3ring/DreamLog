using DreamLib.Data;
using DreamLib.Data.Models;
using DreamLib.Tools;
using DreamLog.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DreamLog.ViewModels
{
    public class DreamCategoriesViewModel : BaseViewModel
    {
        public ObservableCollection<DreamCategoryViewModel> Items { get; set; }
        public Command LoadCategoriesCommand { get; set; }

        private readonly IDatalayer datalayer;

        public DreamCategoriesViewModel(IDatalayer datalayer)
        {
            this.datalayer = datalayer;

            this.Title = "Categories";
            this.Items = new ObservableCollection<DreamCategoryViewModel>();
            this.LoadCategoriesCommand = new Command(async () => await ExecuteLoadCategoriesCommand());

            MessagingCenter.Subscribe<CreateEditDreamCategoryPage, DreamCategoryEditorViewModel>(this, MessagingCenterMessages.AddCategory, this.OnDreamCategoryCreated);
            MessagingCenter.Subscribe<CreateEditDreamCategoryPage, DreamCategoryEditorViewModel>(this, MessagingCenterMessages.EditCategory, this.OnDreamCategoryEdited);
        }

        private void OnDreamCategoryCreated(CreateEditDreamCategoryPage page, DreamCategoryEditorViewModel model)
        {
            DreamCategory category = model.Copy<DreamCategory>();
            category.DreamLogEntries = new List<DreamLogEntry>();
            category.CategoryId = 0;

            if (this.datalayer.AddDreamCategory(category))
                this.Items.Add(model);

            this.IsBusy = false;
        }

        private void OnDreamCategoryEdited(CreateEditDreamCategoryPage page, DreamCategoryEditorViewModel model)
        {
            DreamCategory original = this.datalayer.GetDreamCategory(model.CategoryId);
            original.Update(model);

            this.datalayer.SaveChanges();
            this.LoadCategoriesCommand.Execute(null);
            this.IsBusy = false;
        }

        private async Task ExecuteLoadCategoriesCommand()
        {
            if(!this.IsBusy)
            {
                this.IsBusy = true;
                try
                {
                    await Task.Run(() => this.LoadCategories());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }

        private void LoadCategories()
        {
            this.Items.Clear();
            IEnumerable<DreamCategory> categories = this.datalayer.DreamCategories;

            foreach(DreamCategory category in categories)
            {
                DreamCategoryViewModel model = new DreamCategoryViewModel();
                model.Update(category);
                model.DreamLogEntries = category.DreamLogEntries?.ToList() ?? new List<DreamLogEntry>();
                this.Items.Add(model);
            }
        }
    }
}
