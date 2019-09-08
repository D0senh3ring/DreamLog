using DreamLib.DependencyInjection;
using Rg.Plugins.Popup.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using DreamLog.ViewModels;
using Xamarin.Forms.Xaml;
using DreamLog.Gestures;
using DreamLog.Icons;
using DreamLib.Tools;
using DreamLog.Tools;
using Xamarin.Forms;
using DreamLib.Data;
using System.Linq;
using System;

namespace DreamLog.Views
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogEntriesPage : ContentPage
    {
        private readonly IEnumerable<FilterOptionViewModel> filterOptions;
        private readonly DreamLogEntriesViewModel viewModel;
        private readonly IDatalayer datalayer;

        public LogEntriesPage()
        {
            this.InitializeComponent();

            this.datalayer = DependencyContainer.GetSingleton<IDatalayer>();
            this.BindingContext = viewModel = new DreamLogEntriesViewModel(this.datalayer);

            this.filterOptions = new[]
            {
                new FilterOptionViewModel() { Icon = MaterialDesignIcons.SortDescending, Title = "By Date from recent to old", TapCommand = this.viewModel.SortEntriesCommand, TapCommandParameter = new FilterOptionCommandParameter("Date", ListSortDirection.Descending) },
                new FilterOptionViewModel() { Icon = MaterialDesignIcons.SortAscending, Title = "By Date from old to recent", TapCommand = this.viewModel.SortEntriesCommand, TapCommandParameter = new FilterOptionCommandParameter("Date", ListSortDirection.Ascending) },
                new FilterOptionViewModel() { Icon = MaterialDesignIcons.SortAscending, Title = "By Title from A to Z", TapCommand = this.viewModel.SortEntriesCommand, TapCommandParameter = new FilterOptionCommandParameter("Title", ListSortDirection.Ascending) },
                new FilterOptionViewModel() { Icon = MaterialDesignIcons.SortDescending, Title = "By Title from Z to A", TapCommand = this.viewModel.SortEntriesCommand, TapCommandParameter = new FilterOptionCommandParameter("Title", ListSortDirection.Descending) },
                new FilterOptionViewModel() { Icon = MaterialDesignIcons.SortAscending, Title = "By Category-Id ascending", TapCommand = this.viewModel.SortEntriesCommand, TapCommandParameter = new FilterOptionCommandParameter("Category", ListSortDirection.Ascending) },
                new FilterOptionViewModel() { Icon = MaterialDesignIcons.SortDescending, Title = "By Category-Id descending", TapCommand = this.viewModel.SortEntriesCommand, TapCommandParameter = new FilterOptionCommandParameter("Category", ListSortDirection.Descending) }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.LoadEntriesCommand.Execute(null);
            this.viewModel.IsBusy = false;
        }

        private IEnumerable<DreamCategoryViewModel> GetDreamCategories()
        {
            return new DreamCategoryViewModel[]
            {
                new DreamCategoryViewModel { CategoryId = -1, Name = "None", Color = Color.Transparent },
            }.Union(this.datalayer.DreamCategories.Select(_category =>
            {
                DreamCategoryViewModel model = new DreamCategoryViewModel();
                model.Update(_category);
                return model;
            }
             )).ToArray();
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem is DreamLogEntryViewModel model)
            {
                await Navigation.PushModalAsync(new NavigationPage(new CreateEditLogEntryPage(model, this.GetDreamCategories())));
                this.ItemsListView.SelectedItem = null;
            }
        }

        private async void OnAddLogEntryClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new CreateEditLogEntryPage(this.GetDreamCategories())));
        }

        private async void OnEntriesListViewSwiped(object sender, ListViewSwipeEventArgs e)
        {
            if (e.SwipedItem is Grid grid && grid.Children.Count == 2)
            {
                grid.Children[1].TranslationX = 0;

                if (grid.Children[0].IsEnabled && grid.BindingContext is DreamLogEntryViewModel model)
                {
                    await grid.Children[1].TranslateToAbsolute(-grid.Width, 0);

                    if (this.datalayer.RemoveLogEntry(model.EntryId))
                        this.viewModel.Items.Remove(model);

                    //TODO: Timed undo action
                }
                else
                {
                    await grid.Children[1].TranslateToAbsolute(0, 0);
                }
            }
        }

        private void OnEntriesListViewSwiping(object sender, ListViewSwipeEventArgs e)
        {
            if (e.SwipedItem is Grid grid && grid.Children.Count == 2 && e.OffsetX <= 0.0d)
            {
                if (Math.Abs(e.OffsetX) >= grid.Width / 3.0d != grid.Children[0].IsEnabled)
                    grid.Children[0].IsEnabled = !grid.Children[0].IsEnabled;
                grid.Children[1].TranslationX = e.OffsetX;
            }
        }

        private void OnSortEntriesClicked(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new SortingPopupPage(this.filterOptions), true);
        }
    }
}