using Rg.Plugins.Popup.Extensions;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using DreamLog.ViewModels;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace DreamLog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SortingPopupPage : PopupPage
    {
        public SortingPopupPage(IEnumerable<FilterOptionViewModel> options)
        {
            this.InitializeComponent();

            this.BindingContext = options;
        }

        private void OnOptionSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem is FilterOptionViewModel model)
            {
                model.TapCommand?.Execute(model.TapCommandParameter);

                this.Navigation.PopPopupAsync();
            }
        }
    }
}