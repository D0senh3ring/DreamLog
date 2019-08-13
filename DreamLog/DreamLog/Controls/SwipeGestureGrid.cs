using System.Diagnostics;
using Xamarin.Forms;
using System;

namespace DreamLog.Controls
{
    public class SwipeGestureGrid : Grid
    {
        private GeneralDirection swipeDirection;
        private double offsetX;
        private double offsetY;
        private bool isSwiping;

        public event EventHandler<SwipeEventArgs> SwipeEnded;
        public event EventHandler<SwipeEventArgs> Swiping;
        public event EventHandler<EventArgs> Tapped;

        public SwipeGestureGrid()
        {
            PanGestureRecognizer panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += this.OnPanGesturePanUpdated;

            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += this.TapGestureTapped;

            this.GestureRecognizers.Add(panGesture);
            this.GestureRecognizers.Add(tapGesture);
        }

        private void TapGestureTapped(object sender, EventArgs e)
        {
            try
            {
                if (!this.isSwiping)
                    this.Tapped?.Invoke(this, EventArgs.Empty);

                this.swipeDirection = GeneralDirection.None;
                this.offsetX = this.offsetY = 0.0d;
                this.isSwiping = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void OnPanGesturePanUpdated(object sender, PanUpdatedEventArgs e)
        {
            try
            {
                
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        if (this.swipeDirection != GeneralDirection.None)
                            this.swipeDirection = this.GetGeneralDirection(e.TotalX, e.TotalY);

                        //e.TotalX and e.TotalY are 0 when e.StatusType == GestureStatus.Completed
                        this.offsetX = e.TotalX;
                        this.offsetY = e.TotalY;
                        Debug.WriteLine($"Is swiping... direction: {this.swipeDirection}, offsetX: {e.TotalX}, offsetY: {e.TotalY}");

                        if (this.swipeDirection != GeneralDirection.None)
                            this.Swiping?.Invoke(this, this.GetSwipeEventArgs(this.swipeDirection, this.offsetX, this.offsetY));
                        break;
                    case GestureStatus.Completed:

                        Debug.WriteLine($"Swipe ended... direction: {this.swipeDirection}, offsetX: {this.offsetX}, offsetY: {this.offsetY}");

                        if (this.swipeDirection != GeneralDirection.None)
                            this.SwipeEnded?.Invoke(this, this.GetSwipeEventArgs(this.swipeDirection, this.offsetX, this.offsetY));
                        break;
                    case GestureStatus.Started:
                        this.isSwiping = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private GeneralDirection GetGeneralDirection(double offsetX, double offsetY)
        {
            if (this.IsHorizontalSwipeEnabled && Math.Abs(offsetX) > Math.Abs(offsetY) && Math.Abs(offsetX) >= Math.Abs(this.SwipeThreshold))
                return GeneralDirection.Horizontal;
            else if (this.IsVerticalSwipeEnabled && Math.Abs(offsetY) >= Math.Abs(this.SwipeThreshold))
                return GeneralDirection.Vertical;
            else
                return GeneralDirection.None;
        }

        private SwipeEventArgs GetSwipeEventArgs(GeneralDirection direction, double offsetX, double offsetY)
        {
            SwipeDirection swipeDir;
            if (direction == GeneralDirection.Horizontal)
                swipeDir = offsetX < 0.0d ? SwipeDirection.Left : SwipeDirection.Right;
            else
                swipeDir = offsetY < 0.0d ? SwipeDirection.Down : SwipeDirection.Up;

            return new SwipeEventArgs(swipeDir, direction == GeneralDirection.Horizontal ? offsetX : offsetY);
        }

        public bool IsVerticalSwipeEnabled
        {
            get { return (bool)this.GetValue(SwipeGestureGrid.IsVerticalSwipeEnabledProperty); }
            set { this.SetValue(SwipeGestureGrid.IsVerticalSwipeEnabledProperty, value); }
        }

        public bool IsHorizontalSwipeEnabled
        {
            get { return (bool)this.GetValue(SwipeGestureGrid.IsHorizontalSwipeEnabledProperty); }
            set { this.SetValue(SwipeGestureGrid.IsHorizontalSwipeEnabledProperty, value); }
        }

        public double SwipeThreshold
        {
            get { return (double)this.GetValue(SwipeGestureGrid.SwipeThresholdProperty); }
            set { this.SetValue(SwipeGestureGrid.SwipeThresholdProperty, value); }
        }

        public static readonly BindableProperty IsVerticalSwipeEnabledProperty =
            BindableProperty.Create("IsVerticalSwipeEnabled", typeof(bool), typeof(SwipeGestureGrid), true);
        public static readonly BindableProperty IsHorizontalSwipeEnabledProperty =
            BindableProperty.Create("IsHorizontalSwipeEnabled", typeof(bool), typeof(SwipeGestureGrid), true);
        public static readonly BindableProperty SwipeThresholdProperty =
            BindableProperty.Create("SwipeThreshold", typeof(double), typeof(SwipeGestureGrid), 0.0d);
    }

    public class SwipeEventArgs : EventArgs
    {
        public SwipeEventArgs(SwipeDirection direction, double offset)
        {
            this.Direction = direction;
            this.Offset = offset;
        }

        public SwipeDirection Direction { get; private set; }
        public double Offset { get; private set; }
    }

    public enum GeneralDirection
    {
        Horizontal,
        Vertical,
        None
    }
}
