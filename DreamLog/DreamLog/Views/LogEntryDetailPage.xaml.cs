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
        private readonly DreamLogEntryViewModel viewModel;

        public LogEntryDetailPage(DreamLogEntryViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public LogEntryDetailPage()
        {
            InitializeComponent();

            viewModel = new DreamLogEntryViewModel();
            BindingContext = viewModel;
        }
    }
}