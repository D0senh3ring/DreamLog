using DreamLog.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DreamLog.Tools;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DreamLog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateEditDreamCategoryPage : ContentPage
    {
        public CreateEditDreamCategoryPage(DreamCategoryEditorViewModel category)
        {
            this.AvailableColors = this.GetAvailableColors();
            this.Title = "Edit dream category";
            this.Category = category;
            this.IsEdit = true;

            var test1 = ((System.Drawing.Color)this.Category.Color).ToArgb();
            var test2 = this.AvailableColors.Select(_color => ((System.Drawing.Color)_color.Color).ToArgb()).OrderBy(_color => _color);

            var test = this.AvailableColors.FirstOrDefault(_color => ((System.Drawing.Color)_color.Color).ToArgb() == ((System.Drawing.Color)this.Category.Color).ToArgb());
            this.Category.SelectedColor = this.AvailableColors.FirstOrDefault(_color => _color.Color.Equals(this.Category.Color));

            this.InitializeComponent();

            this.BindingContext = this;
        }

        public CreateEditDreamCategoryPage() : this(new DreamCategoryEditorViewModel())
        {
            this.Title = "Create dream category";
            this.IsEdit = false;
        }

        public ObservableCollection<ColorSelectionViewModel> AvailableColors { get; set; }
        public DreamCategoryEditorViewModel Category { get; set; }
        public bool IsEdit { get; set; }


        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await this.Navigation.PopModalAsync();
        }

        private async void OnSaveCategoryClicked(object sender, EventArgs e)
        {
            if(this.ValidateDreamCategory())
            {
                MessagingCenter.Send(this, this.IsEdit ? MessagingCenterMessages.EditCategory : MessagingCenterMessages.AddCategory, this.Category);

                await this.Navigation.PopModalAsync();
            }
        }

        private bool ValidateDreamCategory()
        {
            if (String.IsNullOrEmpty(this.Category.Name))
                return false;
            else if (String.IsNullOrEmpty(this.Category.ColorString))
                return false;

            return true;
        }

        private ObservableCollection<ColorSelectionViewModel> GetAvailableColors()
        {
            ObservableCollection<ColorSelectionViewModel> output = new ObservableCollection<ColorSelectionViewModel>();
            foreach(FieldInfo field in typeof(Color).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if(field.FieldType == typeof(Color) && !field.Name.Equals("Transparent"))
                {
                    output.Add(new ColorSelectionViewModel() { Name = field.Name, Color = (Color)field.GetValue(null) });
                }
            }
            return output;
        }
    }
}