using DreamLog.DependencyInjection;
using System.ComponentModel;
using DreamLog.ViewModels;
using System.Reflection;
using DreamLog.Data;
using Xamarin.Forms;
using System.Linq;
using System;

namespace DreamLog.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();

            IDatalayer datalayer = DependencyContainer.GetSingleton<IDatalayer>();

            this.BindingContext = new AboutViewModel()
            {
                Version = $"v.{String.Join(".", Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.').Take(3))}",
                Author = "Dosenhering",
                Description = "This app allows you to log your dreams and assign them to different categories",
                CategoryCount = datalayer.DreamCategories.Count(),
                DreamCount = datalayer.DreamLogEntries.Count()
            };
        }
    }
}