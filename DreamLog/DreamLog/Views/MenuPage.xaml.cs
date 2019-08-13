using DreamLog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DreamLog.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        private readonly List<HomeMenuItem> menuItems;

        private MainPage RootPage { get => Application.Current.MainPage as MainPage; }

        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem { Id = MenuItemType.DreamLogEntries, Title="Dream log entries" },
                new HomeMenuItem { Id = MenuItemType.DreamCategories, Title="Dream categories" },
                //new HomeMenuItem { Id = MenuItemType.DreamLogs, Title="Dream logs" },
                new HomeMenuItem { Id = MenuItemType.About, Title="About" }
            };

            this.ListViewMenu.ItemsSource = menuItems;

            this.ListViewMenu.SelectedItem = menuItems[0];
            this.ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}