using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DreamLog.Models;
using DreamLog.Views;
using DreamLog.ViewModels;
using DreamLib.Tools;
using DreamLib.DependencyInjection;
using DreamLib.Data;
using DreamLog.Controls;
using System.Diagnostics;

namespace DreamLog.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class LogEntriesPage : ContentPage
    {
        private readonly ItemsViewModel viewModel;
        private readonly IDatalayer datalayer;

        public LogEntriesPage()
        {
            InitializeComponent();

            this.datalayer = DependencyContainer.GetSingleton<IDatalayer>();
            this.BindingContext = viewModel = new ItemsViewModel(this.datalayer);
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //TODO: OnItemSelected currently cannot be called
            DreamLogEntryViewModel model = args.SelectedItem as DreamLogEntryViewModel;
            if (!(model is null))
            {
                await Navigation.PushModalAsync(new NavigationPage(new CreateEditLogEntryPage(model, this.GetDreamCategories())));
                this.ItemsListView.SelectedItem = null;
            }
        }

        private async void OnAddLogEntryClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new CreateEditLogEntryPage(this.GetDreamCategories())));
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

        private void OnDeleteEntryClick(object sender, EventArgs e)
        {
            if(sender is Button button && button.Parent.BindingContext is DreamLogEntryViewModel model)
            {
                if(this.datalayer.RemoveLogEntry(model.EntryId))
                    this.viewModel.LoadItemsCommand.Execute(null);
            }
        }

        private void OnLogEntrySwipedLeft(object sender, EventArgs e)
        {
            if(sender is SwipeGestureGrid grid)
            {
                grid.ColumnDefinitions[2].Width = new GridLength(50, GridUnitType.Absolute);
            }
        }

        private void OnLogEntrySwipedRight(object sender, EventArgs e)
        {
            if(sender is SwipeGestureGrid grid)
            {
                grid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Absolute);
            }
        }

        private async void OnLogEntryTapped(object sender, EventArgs e)
        {
            if (sender is SwipeGestureGrid grid && grid.BindingContext is DreamLogEntryViewModel model)
            {
                if (!(model is null))
                {
                    await Navigation.PushModalAsync(new NavigationPage(new CreateEditLogEntryPage(model, this.GetDreamCategories())));
                    this.ItemsListView.SelectedItem = null;
                }
            }
        }

        private void OnLogEntrySwiping(object sender, SwipeEventArgs e)
        {
            if (sender is SwipeGestureGrid grid && grid.Children[1] is Grid target)
            {
                if(e.Direction == SwipeDirection.Left)
                {
                    target.TranslateTo(Math.Min(0.0d, e.Offset), 0, 100);
                }
            }
        }

        private async void OnLogEntrySwipeEnded(object sender, SwipeEventArgs e)
        {
            if (sender is SwipeGestureGrid grid && grid.Children[1] is Grid target)
            {
                if(e.Direction == SwipeDirection.Left)
                {
                    if (-e.Offset >= target.Width / 3.0d && grid.BindingContext is DreamLogEntryViewModel model)
                    {
                        await target.TranslateTo(target.Width + 10.0d, 0, 250);
                        await target.ScaleTo(0.0d, 250);

                        if (this.datalayer.RemoveLogEntry(model.EntryId))
                            this.viewModel.LoadItemsCommand.Execute(null);
                    }
                    else
                    {
                        await target.TranslateTo(e.Offset, 0, 250);
                    }
                }
            }
        }
    }
}