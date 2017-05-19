using Xamarin.Forms;

namespace FormsRecyclerViewApp
{
    /// <summary>
    /// A view which can be used as a cell in order to get optimum performance 
    /// </summary>
    /// <remarks>
    /// Based on https://github.com/twintechs/TwinTechsFormsLib
    /// </remarks>
    public abstract class FormsRecyclerCell : ViewCell
    {
        public bool IsInitialized
        {
            get;
            private set;
        }


        /// <summary>
        /// Initializes the cell.
        /// </summary>
        public void PrepareCell()
        {
            this.InitializeCell();
            if (this.BindingContext != null)
            {
                this.SetupCell(false);
            }
            this.IsInitialized = true;
        }

        public object OriginalBindingContext;

        public bool HasSize { get; private set; }

        /// <summary>
        /// Called when the cell is appearing. On iOS just before cell appearing, on android just after cell appearing.
        /// </summary>
        public virtual void OnCellAppearing()
        {
        }

        /// <summary>
        /// Called when the cell has disappeared.
        /// </summary>
        public virtual void OnCellDisappeared()
        {
        }

        /// <summary>
        /// Called when the cell has been recycled (on android only).
        /// </summary>
        public virtual void OnCellRecycled()
        {
        }

        /// <summary>
        /// Called when the size of the view changes. Override to do layout task if required
        /// </summary>
        /// <param name="size">Size.</param>
        public virtual void OnSizeChanged(Size size)
        {
        }

        /// <summary>
        /// This method is called when BindingContext is replaced for the completly new object
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.IsInitialized)
            {
                this.SetupCell(true);
            }
        }

        /// <summary>
        /// This method is called when BindingContext is replaced or in case of update. Override in your cell if you need this.
        /// </summary>
        /// <param name="isNewBindingContext">When true it means that there is completly new binding context object. 
        /// This occurs for instance on start.</param>
        public virtual void OnBindingContextUpdated(bool isNewBindingContext)
        {
        }

        /// <summary>
        /// Do your cell setup using the binding context in here.
        /// </summary>
        /// <param name="isRecycled">If set to <c>true</c> is recycled.</param>
        protected virtual void SetupCell(bool isRecycled)
        {
        }

        /// <summary>
        /// Setups the cell. You should call InitializeComponent in here
        /// </summary>
        protected abstract void InitializeCell();
    }
}
