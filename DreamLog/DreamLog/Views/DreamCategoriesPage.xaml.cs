using DreamLib.Data;
using DreamLib.DependencyInjection;
using DreamLib.Tools;
using DreamLog.Controls;
using DreamLog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DreamLog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DreamCategoriesPage : ContentPage
    {
        private readonly DreamCategoriesViewModel viewModel;
        private readonly IDatalayer datalayer;

        public DreamCategoriesPage()
        {
            InitializeComponent();

            this.datalayer = DependencyContainer.GetSingleton<IDatalayer>();
            this.BindingContext = viewModel = new DreamCategoriesViewModel(this.datalayer);
        }

        private async void OnAddCategoryClicked(object sender, EventArgs e)
        {
            await this.Navigation.PushModalAsync(new NavigationPage(new CreateEditDreamCategoryPage()));
        }

        private async void OnCategorySelected(object sender, SelectedItemChangedEventArgs e)
        {
            //TODO: OnCategorySelected currently cannot be called
            DreamCategoryViewModel model = e.SelectedItem as DreamCategoryViewModel;
            if (!(model is null))
            {
                DreamCategoryEditorViewModel editorModel = new DreamCategoryEditorViewModel();
                editorModel.Update(model);
                await this.Navigation.PushModalAsync(new NavigationPage(new CreateEditDreamCategoryPage(editorModel)));
                this.categoriesListView.SelectedItem = null;
            }
        }

        private async void OnDreamCategoryTapped(object sender, EventArgs e)
        {
            if (sender is OldSwipeGestureGrid grid && grid.BindingContext is DreamCategoryViewModel model)
            {
                if (!(model is null))
                {
                    DreamCategoryEditorViewModel editorModel = new DreamCategoryEditorViewModel();
                    editorModel.Update(model);
                    await this.Navigation.PushModalAsync(new NavigationPage(new CreateEditDreamCategoryPage(editorModel)));
                    this.categoriesListView.SelectedItem = null;
                }
            }
        }

        private void OnLogEntrySwipedLeft(object sender, EventArgs e)
        {
            if (sender is OldSwipeGestureGrid grid)
            {
                grid.ColumnDefinitions[2].Width = new GridLength(50, GridUnitType.Absolute);
            }
        }

        private void OnLogEntrySwipedRight(object sender, EventArgs e)
        {
            if (sender is OldSwipeGestureGrid grid)
            {
                grid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Absolute);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.viewModel.Items.Count == 0)
                this.viewModel.LoadCategoriesCommand.Execute(null);
        }

        private void OnDeleteCategoryClick(object sender, EventArgs e)
        {
            if(sender is Button button && button.Parent.BindingContext is DreamCategoryViewModel model)
            {
                if(this.datalayer.RemoveDreamCategory(model.CategoryId))
                    this.viewModel.LoadCategoriesCommand.Execute(null);
            }
        }
    }
}