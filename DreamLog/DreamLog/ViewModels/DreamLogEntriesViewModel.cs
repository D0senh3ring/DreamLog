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
using System.ComponentModel;

namespace DreamLog.ViewModels
{
    public class DreamLogEntriesViewModel : BaseViewModel
    {
        public ObservableCollection<DreamLogEntryViewModel> Items { get; }
        public Command LoadEntriesCommand { get; }
        public Command SortEntriesCommand { get; }

        public DreamLogEntryViewModel SelectedLogEntry
        {
            get { return this.selectedLogEntry; }
            set { base.SetProperty(ref this.selectedLogEntry, value); }
        }

        private readonly IDatalayer datalayer;

        private FilterOptionCommandParameter currentSorting = new FilterOptionCommandParameter("Date", ListSortDirection.Descending);
        private DreamLogEntryViewModel selectedLogEntry;

        public DreamLogEntriesViewModel(IDatalayer datalayer)
        {
            this.datalayer = datalayer;
            
            this.Title = "Browse dreams";
            this.Items = new ObservableCollection<DreamLogEntryViewModel>();
            this.LoadEntriesCommand = new Command(async () => await this.ExecuteLoadEntriesCommand());
            this.SortEntriesCommand = new Command(async _param => await this.ExecuteSortEntriesCommand(_param), _param => this.Items.Count > 0);

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
                this.LoadEntriesCommand.Execute(null);

            this.IsBusy = false;
        }

        private void OnLogEntryEdited(CreateEditLogEntryPage page, DreamLogEntryViewModel model)
        {
            DreamLogEntry original = datalayer.GetLogEntry(model.EntryId);
            original.Update(model);
            this.datalayer.SaveChanges();
            this.IsBusy = false;
        }

        private async Task ExecuteLoadEntriesCommand()
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

        private async Task ExecuteSortEntriesCommand(object parameter)
        {
            if (!this.IsBusy && parameter is FilterOptionCommandParameter param)
            {
                this.IsBusy = true;
                this.currentSorting = param;
                try
                {
                    await this.UpdateItemsSorting(this.Items);
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

        private async Task UpdateItemsSorting(IEnumerable<DreamLogEntryViewModel> inputItems)
        {
            IEnumerable<DreamLogEntryViewModel> items;
            switch (this.currentSorting.ParameterName)
            {
                case "Date":
                    items = await this.SortItems(inputItems, _model => _model.Date, this.currentSorting.Direction);
                    break;
                case "Title":
                    items = await this.SortItems(inputItems, _model => _model.Title, this.currentSorting.Direction);
                    break;
                case "Category":
                    items = await this.SortItems(inputItems, _model => _model.FK_CategoryId, this.currentSorting.Direction);
                    break;
                default: throw new NotImplementedException($"Unknown sorting-parameter-name: \"{this.currentSorting.ParameterName}\"");
            }

            if(items?.Count() > 0)
            {
                this.Items.Clear();
                foreach (var item in items)
                    this.Items.Add(item);
            }
        }

        private async Task<IEnumerable<DreamLogEntryViewModel>> SortItems<T>(IEnumerable<DreamLogEntryViewModel> items, Func<DreamLogEntryViewModel, T> selector, ListSortDirection direction)
        {
            DreamLogEntryViewModel[] sorted = null;
            await Task.Run(() =>
            {
                if (direction == ListSortDirection.Ascending)
                    sorted = items.OrderBy(selector).ThenBy(_model => _model.Title).ToArray();
                else
                    sorted = items.OrderByDescending(selector).ThenByDescending(_model => _model.Title).ToArray();
            });
            return sorted.ToArray();
        }

        private async void LoadItems()
        {
            IEnumerable<DreamLogEntryViewModel> entries = this.datalayer?.DreamLogEntries.ToArray()
                                                                         .Select(_entry => this.GetDreamLogEntryViewModel(this.datalayer, _entry));
            await this.UpdateItemsSorting(entries);
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