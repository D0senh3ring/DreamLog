using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DreamLog.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Android.Widget.ListView), typeof(SwipableListViewRenderer))]
namespace DreamLog.Droid.Renderers
{
    public class SwipableListViewRenderer : ListViewRenderer
    {
        public SwipableListViewRenderer(Context context) : base(context)
        {}

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            System.Diagnostics.Debug.WriteLine($"Motion intercept recognized...");
            return base.OnInterceptTouchEvent(ev);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"Motion recognized...");
            return base.OnTouchEvent(e);
        }
    }
}