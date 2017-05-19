using Android.Content;
using Android.OS;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using XForms = Xamarin.Forms;

namespace FormsRecyclerViewApp.Droid
{
    public class FormsRecyclerViewCellContainer : ViewGroup, XForms.INativeElementView
    {
        private readonly float density = Android.App.Application.Context.Resources.DisplayMetrics.Density;

        private readonly FormsRecyclerCell fastCell;
        private readonly View parent;
        private readonly double initialWidth;

        private IVisualElementRenderer view;

        public FormsRecyclerViewCellContainer(Context context, FormsRecyclerCell fastCell, View parent, double width,
            FormsRecyclerView formsRecyclerView)
            : base(context)
        {
            this.fastCell = fastCell;
            this.parent = parent;
            this.initialWidth = width;

            using (var handler = new Handler(Looper.MainLooper))
            {
                handler.Post(() =>
                {
                    this.fastCell.PrepareCell();
                    this.view = Platform.CreateRenderer(fastCell.View);
                    Platform.SetRenderer(fastCell.View, view);
                    this.AddView(this.view.ViewGroup);
                });
            }
        }

        public XForms.Element Element
        {
            get
            {
                return this.fastCell;
            }
        }

        public void Update(object bindingContext)
        {
            bool isNew = this.fastCell.BindingContext != bindingContext;
            this.fastCell.BindingContext = bindingContext;
            this.fastCell.OnBindingContextUpdated(isNew);

            var viewAsLayout = this.fastCell.View as XForms.Layout;
            viewAsLayout?.ForceLayout();
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            using (var handler = new Handler(Looper.MainLooper))
            {
                handler.Post(() =>
                {
                    double width = Context.FromPixels(r - l);
                    double height = Context.FromPixels(b - t);

                    Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(this.view.Element, new XForms.Rectangle(0, 0, width, height));
                    this.view.UpdateLayout();
                });
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (this.view == null || this.view.Element == null)
            {
                base.SetMeasuredDimension(widthMeasureSpec, heightMeasureSpec);
                return;
            }

            XForms.SizeRequest measure = this.view.Element.Measure(this.initialWidth, double.PositiveInfinity, XForms.MeasureFlags.IncludeMargins);
            int height = (int)Context.ToPixels(this.fastCell.Height > 0 ? this.fastCell.Height : measure.Request.Height);
            this.SetMeasuredDimension((int)Context.ToPixels(this.initialWidth), height);
        }
    }
}