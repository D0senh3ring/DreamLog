using System;

using DreamLog.Models;

namespace DreamLog.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public DreamLogEntryViewModel LogEntry { get; set; }
        public ItemDetailViewModel(DreamLogEntryViewModel model = null)
        {
            Title = model?.Title;
            LogEntry = model;
        }
    }
}
