using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using DreamLog.Views;
using System.Collections.Generic;
using DreamLib.Data.Models;
using DreamLib.Tools;
using DreamLib.Data;
using System.Linq;

namespace DreamLog.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<DreamLogEntryViewModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public DreamLogEntryViewModel SelectedLogEntry
        {
            get { return this.selectedLogEntry; }
            set { base.SetProperty(ref this.selectedLogEntry, value); }
        }

        private readonly IDatalayer datalayer;

        private DreamLogEntryViewModel selectedLogEntry;

        public ItemsViewModel(IDatalayer datalayer)
        {
            this.datalayer = datalayer;

            Title = "Browse dreams";
            Items = new ObservableCollection<DreamLogEntryViewModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<CreateEditLogEntryPage, DreamLogEntryViewModel>(this, MessagingCenterMessages.AddLogEntry, this.OnLogEntryCreated);
            MessagingCenter.Subscribe<CreateEditLogEntryPage, DreamLogEntryViewModel>(this, MessagingCenterMessages.EditLogEntry, this.OnLogEntryEdited);
        }

        private void OnLogEntryCreated(CreateEditLogEntryPage page, DreamLogEntryViewModel model)
        {
            DreamLogEntry entry = model.Copy<DreamLogEntry>();
            if (entry.FK_CategoryId == -1)
            {
                entry.FK_CategoryId = null;
                entry.Category = null;
            }
            entry.Log = this.datalayer.DreamLogCollections.First();
            entry.FK_LogId = entry.Log.LogId;
            entry.CreatedAt = DateTime.Now;

            if (this.datalayer.AddLogEntry(entry))
                this.LoadItemsCommand.Execute(null);

            this.IsBusy = false;
        }

        private void OnLogEntryEdited(CreateEditLogEntryPage page, DreamLogEntryViewModel model)
        {
            DreamLogEntry original = datalayer.GetLogEntry(model.EntryId);
            original.Update(model);
            this.datalayer.SaveChanges();
            this.IsBusy = false;
        }

        private async Task ExecuteLoadItemsCommand()
        {
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                try
                {
                    await Task.Run(() => this.LoadItems());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void LoadItems()
        {
            this.Items.Clear();
            IEnumerable<DreamLogEntry> entries = this.datalayer.DreamLogEntries;

            foreach (DreamLogEntry entry in entries)
            {
                this.Items.Add(this.GetDreamLogEntryViewModel(this.datalayer, entry));
            }
        }

        private DreamLogEntryViewModel GetDreamLogEntryViewModel(IDatalayer datalayer, DreamLogEntry entry)
        {
            DreamLogEntryViewModel model = new DreamLogEntryViewModel();
            model.Update(entry);
            if (model.FK_CategoryId.HasValue)
                model.Category = datalayer.GetDreamCategory(entry.FK_CategoryId.Value);
            model.Log = datalayer.DreamLogCollections.First();
            return model;
        }
    }
}