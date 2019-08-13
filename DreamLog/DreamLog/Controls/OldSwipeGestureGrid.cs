/*
 * Source: https://stackoverflow.com/questions/38757506/xamarin-forms-listview-view-cell-swipe-left-and-right-events-using-xamal
 * 
 * slightly modified to match this project
 */

using System.Diagnostics;
using Xamarin.Forms;
using System;

namespace DreamLog.Controls
{
    public class OldSwipeGestureGrid : Grid
    {
        #region Private Member
        private double GestureX { get; set; }
        private double GestureY { get; set; }
        private bool IsSwipe { get; set; }
        #endregion

        #region Public Member

        #region Events

        #region Tapped
        public event EventHandler Tapped;
        protected void OnTapped(EventArgs e)
        {
            this.Tapped?.Invoke(this, e);
        }
        #endregion

        #region SwipeUP

        public event EventHandler SwipeUP;
        protected void OnSwipeUP(EventArgs e)
        {
            this.SwipeUP?.Invoke(this, e);
        }

        #endregion

        #region SwipeDown

        public event EventHandler SwipeDown;
        protected void OnSwipeDown(EventArgs e)
        {
            this.SwipeDown?.Invoke(this, e);
        }

        #endregion

        #region SwipeRight

        public event EventHandler SwipeRight;
        protected void OnSwipeRight(EventArgs e)
        {
            this.SwipeRight?.Invoke(this, e);
        }

        #endregion

        #region SwipeLeft

        public event EventHandler SwipeLeft;
        protected void OnSwipeLeft(EventArgs e)
        {
            this.SwipeLeft?.Invoke(this, e);
        }

        #endregion

        #endregion


        public new double Height
        {
            get
            {
                return this.HeightRequest;
            }
            set
            {
                this.HeightRequest = value;
            }
        }
        public new double Width
        {
            get
            {
                return this.WidthRequest;
            }
            set
            {
                this.WidthRequest = value;
            }
        }

        #endregion

        public OldSwipeGestureGrid()
        {
            PanGestureRecognizer panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += PanGesture_PanUpdated;

            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += TapGesture_Tapped;

            GestureRecognizers.Add(panGesture);
            GestureRecognizers.Add(tapGesture);
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsSwipe)
                    this.OnTapped(null);
                this.IsSwipe = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            try
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        {
                            this.GestureX = e.TotalX;
                            this.GestureY = e.TotalY;
                        }
                        break;
                    case GestureStatus.Completed:
                        {
                            this.IsSwipe = true;
                            if (Math.Abs(this.GestureX) > Math.Abs(this.GestureY))
                            {
                                if (this.GestureX > 0)
                                {
                                    this.OnSwipeRight(null);
                                }
                                else
                                {
                                    this.OnSwipeLeft(null);
                                }
                            }
                            else
                            {
                                if (this.GestureY > 0)
                                {
                                    this.OnSwipeDown(null);
                                }
                                else
                                {
                                    this.OnSwipeUP(null);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
