using Xamarin.Forms;
using System;

namespace DreamLog.Gestures
{
    public class ListViewSwipeGestureRecognizer : Element, IGestureRecognizer
    {
        public event EventHandler<ListViewSwipeEventArgs> Swiping;
        public event EventHandler<ListViewSwipeEventArgs> Swiped;

        public ListViewSwipeGestureRecognizer()
        { }

        public void OnSwiped(ListViewSwipeEventArgs e)
        {
            this.Swiped?.Invoke(this.Parent, e);
        }

        public void OnSwiping(ListViewSwipeEventArgs e)
        {
            this.Swiping?.Invoke(this.Parent, e);
        }
    }

    public class ListViewSwipeEventArgs : EventArgs
    {
        public ListViewSwipeEventArgs(Element swipedItem, double startPositionX, double offsetX)
        {
            this.StartPositionX = startPositionX;
            this.SwipedItem = swipedItem;
            this.OffsetX = offsetX;
        }

        public double StartPositionX { get; private set; }
        public Element SwipedItem { get; private set; }
        public double OffsetX { get; private set; }
    }

}
