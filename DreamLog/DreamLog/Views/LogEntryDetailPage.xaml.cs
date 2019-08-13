using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DreamLog.Models;
using DreamLog.ViewModels;

namespace DreamLog.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class LogEntryDetailPage : ContentPage
    {
        private readonly ItemDetailViewModel viewModel;

        public LogEntryDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public LogEntryDetailPage()
        {
            InitializeComponent();

            var item = new DreamLogEntryViewModel();

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }
    }
}