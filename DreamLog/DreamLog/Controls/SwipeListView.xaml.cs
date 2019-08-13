using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DreamLog.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SwipeListView : ContentView
    {
        public SwipeListView()
        {
            InitializeComponent();
        }

        private void OnItemHostTapped(object sender, EventArgs e)
        {
            Debug.WriteLine("Tapped item host");
        }

        private void OnItemHostPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Debug.WriteLine($"Pan updated ... status: {e.StatusType}, x: {e.TotalX}, y: {e.TotalY}");
        }
    }
}