using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DreamLog.Droid.Renderers;
using DreamLog.Gestures;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using DreamLog.Tools;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(SwipableListViewRenderer))]
namespace DreamLog.Droid.Renderers
{
    public class SwipableListViewRenderer : ListViewRenderer
    {
        private const double SwipeThreshold = 5.0d;

        private List<ListViewSwipeGestureRecognizer> swipeRecognizers = new List<ListViewSwipeGestureRecognizer>();

        private double touchStartX, touchStartY;
        private bool isSwipingHorizontally;
        private bool isSwipingVertically;
        private Element swipedElement;

        public SwipableListViewRenderer(Context context) : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            this.swipeRecognizers = e.NewElement.GestureRecognizers.OfType<ListViewSwipeGestureRecognizer>().ToList();
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                this.OnTouchDown(e);
            }
            else if (e.Action == MotionEventActions.Move)
            {
                this.OnTouchMoved(e);
            }

            return this.isSwipingHorizontally || base.OnInterceptTouchEvent(e);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Move)
            {
                this.OnTouchMoved(e);
            }
            else if (e.Action == MotionEventActions.Up && this.isSwipingHorizontally)
            {
                this.OnTouchUp(e);
                return true;
            }

            return base.OnTouchEvent(e);
        }

        private void OnTouchMoved(MotionEvent e)
        {
            double offsetX = e.GetX() - this.touchStartX;
            double offsetY = e.GetY() - this.touchStartY;

            if (!this.isSwipingVertically && !this.isSwipingHorizontally)
            {
                this.isSwipingVertically = offsetY >= SwipableListViewRenderer.SwipeThreshold && Math.Abs(offsetY) > Math.Abs(offsetX);
                if (!this.isSwipingVertically)
                    this.isSwipingHorizontally = Math.Abs(offsetX) >= SwipableListViewRenderer.SwipeThreshold;
            }

            if (this.isSwipingHorizontally)
                this.swipeRecognizers.ForEach(_swipe => _swipe.OnSwiping(new ListViewSwipeEventArgs(this.swipedElement, this.touchStartX, e.GetX() - this.touchStartX)));
        }

        private void OnTouchUp(MotionEvent e)
        {
            this.swipeRecognizers.ForEach(_swipe => _swipe.OnSwiped(new ListViewSwipeEventArgs(this.swipedElement, this.touchStartX, e.GetX() - this.touchStartX)));

            this.touchStartX = this.touchStartY = 0.0d;
            this.isSwipingVertically = false;
            this.swipedElement = null;

            this.isSwipingHorizontally = false;
        }

        private void OnTouchDown(MotionEvent e)
        {
            this.isSwipingHorizontally = this.isSwipingVertically = false;
            this.touchStartX = e.GetX();
            this.touchStartY = e.GetY();

            IEnumerator<LinearLayout> enumerator = this.GetListItems().GetEnumerator();

            int index = 0;
            while (enumerator.MoveNext() && this.swipedElement is null)
            {
                double positionY = enumerator.Current.GetY();
                if (enumerator.Current.Visibility == ViewStates.Visible && this.touchStartY >= positionY && this.touchStartY <= positionY + enumerator.Current.Height)
                {
                    this.swipedElement = this.Element.GetChildren()[index].View;
                }
                index++;
            }
        }

        private IEnumerable<LinearLayout> GetListItems()
        {
            for (int i = 0; i < this.Control.ChildCount; i++)
            {
                if (this.Control.GetChildAt(i) is LinearLayout layout)
                    yield return layout;
            }
        }
    }
}