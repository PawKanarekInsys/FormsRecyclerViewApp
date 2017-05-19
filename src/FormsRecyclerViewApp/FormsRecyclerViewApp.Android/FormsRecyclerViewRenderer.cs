using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using FormsRecyclerViewApp;
using FormsRecyclerViewApp.Droid;

[assembly: ExportRenderer(typeof(FormsRecyclerView), typeof(FormsRecyclerViewRenderer))]
namespace FormsRecyclerViewApp.Droid
{
    /// <summary>
    /// Forms recycler view renderer
    /// </summary>
    /// <remarks>
    /// Based on https://github.com/twintechs/TwinTechsFormsLib
    /// </remarks>
    public class FormsRecyclerViewRenderer : ViewRenderer<FormsRecyclerView, RecyclerView>
    {
        private readonly Orientation orientation = Orientation.Undefined;
        private RecyclerView recyclerView;
        private FormsRecyclerViewAdapter adapter;
        private bool IsDisposed;

        #region Overrides
        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (newConfig.Orientation != this.orientation)
            {
                this.OnElementChanged(new ElementChangedEventArgs<FormsRecyclerView>(this.Element, this.Element));
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FormsRecyclerView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                this.Destroy();
            }

            if (e.NewElement != null)
            {
                this.CreateRecyclerView();
                base.SetNativeControl(recyclerView);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "ItemsSource")
            {
                this.adapter.Items = this.Element.ItemsSource;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (this.Element == null)
            {
                base.SetMeasuredDimension(widthMeasureSpec, heightMeasureSpec);
                return;
            }
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            SizeRequest measure = this.Element.Measure(Context.FromPixels(width), double.PositiveInfinity, MeasureFlags.IncludeMargins);
            int height = (int)Context.ToPixels(this.Height > 0 ? this.Height : measure.Request.Height);
            this.SetMeasuredDimension(width, height);
        }
        #endregion

        private void CreateRecyclerView()
        {
            this.recyclerView = new RecyclerView(Android.App.Application.Context);
            this.recyclerView.SetLayoutManager(new GridLayoutManager(this.Context, 1, OrientationHelper.Vertical, false));
            this.recyclerView.SetClipToPadding(false);
            this.recyclerView.SetItemAnimator(null);
            this.recyclerView.HasFixedSize = true;
            this.recyclerView.DrawingCacheEnabled = true;
            this.recyclerView.DrawingCacheQuality = DrawingCacheQuality.Low;
            this.recyclerView.HorizontalScrollBarEnabled = this.Element.IsHorizontal;
            this.recyclerView.VerticalScrollBarEnabled = !this.Element.IsHorizontal;

            this.adapter = new FormsRecyclerViewAdapter(this.Element.ItemsSource, this.recyclerView, this.Element);
            this.adapter.SelectionEnabled = this.Element.SelectionEnabled;
            this.recyclerView.SetAdapter(this.adapter);
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.Destroy();
            }
            this.IsDisposed = true;
            base.Dispose(disposing);
        }

        private void Destroy()
        {
            this.recyclerView = null;
        }
        #endregion
    }
}