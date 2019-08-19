using DreamLib.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using DreamLog.ViewModels;
using Xamarin.Forms.Xaml;
using DreamLog.Gestures;
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
        private readonly ItemsViewModel viewModel;
        private readonly IDatalayer datalayer;

        public LogEntriesPage()
        {
            this.InitializeComponent();

            this.datalayer = DependencyContainer.GetSingleton<IDatalayer>();
            this.BindingContext = viewModel = new ItemsViewModel(this.datalayer);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
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

                if(grid.Children[0].IsEnabled && grid.BindingContext is DreamLogEntryViewModel model)
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
                if(Math.Abs(e.OffsetX) >= grid.Width / 3.0d != grid.Children[0].IsEnabled)
                    grid.Children[0].IsEnabled = !grid.Children[0].IsEnabled;
                grid.Children[1].TranslationX = e.OffsetX;
            }
        }
    }
}