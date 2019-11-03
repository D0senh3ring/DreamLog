using DreamLog.DependencyInjection;
using System.ComponentModel;
using DreamLog.ViewModels;
using Xamarin.Forms.Xaml;
using DreamLog.Tools;
using DreamLog.Tools;
using DreamLog.Data;
using Xamarin.Forms;
using System;

namespace DreamLog.Views
{
    [DesignTimeVisible(false)]
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
            if (e.SelectedItem is DreamCategoryViewModel model)
            {
                DreamCategoryEditorViewModel editorModel = new DreamCategoryEditorViewModel();
                editorModel.Update(model);
                await this.Navigation.PushModalAsync(new NavigationPage(new CreateEditDreamCategoryPage(editorModel)));
                this.categoriesListView.SelectedItem = null;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.viewModel.Items.Count == 0)
                this.viewModel.LoadCategoriesCommand.Execute(null);
        }

        private async void OnCategoriesListViewSwiped(object sender, Gestures.ListViewSwipeEventArgs e)
        {
            if (e.SwipedItem is Grid grid && grid.Children.Count == 2)
            {
                grid.Children[1].TranslationX = 0;

                if (e.Direction == SwipeDirection.Left && grid.Children[0].IsEnabled && grid.BindingContext is DreamCategoryViewModel model)
                {
                    await grid.Children[1].TranslateToAbsolute(-grid.Width, 0);

                    if (this.datalayer.RemoveDreamCategory(model.CategoryId))
                        this.viewModel.Items.Remove(model);

                    //TODO: Timed undo action
                }
                else
                {
                    await grid.Children[1].TranslateToAbsolute(0, 0);
                }
            }
        }

        private void OnCategoriesListViewSwiping(object sender, Gestures.ListViewSwipeEventArgs e)
        {
            if (e.SwipedItem is Grid grid && grid.Children.Count == 2 && e.OffsetX <= 0.0d && grid.BindingContext is DreamCategoryViewModel model)
            {
                if (Math.Abs(e.OffsetX) >= grid.Width / 3.0d != grid.Children[0].IsEnabled)
                    grid.Children[0].IsEnabled = !grid.Children[0].IsEnabled && model.DreamLogEntries?.Count == 0;
                grid.Children[1].TranslationX = e.OffsetX;
            }
        }
    }
}