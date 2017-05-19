using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;

namespace FormsRecyclerViewApp.Droid
{
    /// <summary>
    /// Forms recycler view adapter 
    /// </summary>
    /// <remarks>
    /// Based on https://github.com/twintechs/TwinTechsFormsLib
    /// </remarks>
    public class FormsRecyclerViewAdapter : RecyclerView.Adapter
    {
        private readonly RecyclerView recyclerView;
        private readonly DisplayMetrics displayMetrics;
        private readonly int screenDpWidth;
        private IList items;

        public bool IsDisposed { get; protected set; }

        public FormsRecyclerView Element { get; set; }

        public IList Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.RemoveListenerCollectionChanged();

                this.items = value;

                var newCollection = this.items as INotifyCollectionChanged;
                if (newCollection != null)
                {
                    newCollection.CollectionChanged += this.NewCollection_CollectionChanged;
                }
                this.TryNotifyDataSetChanged();
            }
        }

        public override void OnViewAttachedToWindow(Java.Lang.Object holderObj)
        {
            base.OnViewAttachedToWindow(holderObj);
            FormsRecyclerViewCell holder = holderObj as FormsRecyclerViewCell;
            if (holder?.ViewCellContainer.Element != null && holder.AdapterPosition >= 0)
            {
                FormsRecyclerCell fastCell = holder.ViewCellContainer.Element as FormsRecyclerCell;
                fastCell?.OnCellAppearing();
            }
        }

        public override void OnViewDetachedFromWindow(Java.Lang.Object holderObj)
        {
            base.OnViewDetachedFromWindow(holderObj);
            FormsRecyclerViewCell holder = holderObj as FormsRecyclerViewCell;
            if (holder?.ViewCellContainer.Element != null && holder?.AdapterPosition >= 0)
            {
                FormsRecyclerCell fastCell = holder.ViewCellContainer.Element as FormsRecyclerCell;
                fastCell?.OnCellDisappeared();
            }
        }

        public override void OnViewRecycled(Java.Lang.Object holderObj)
        {
            base.OnViewRecycled(holderObj);
            FormsRecyclerViewCell holder = holderObj as FormsRecyclerViewCell;
            if (holder?.ViewCellContainer.Element != null && holder?.AdapterPosition >= 0)
            {
                FormsRecyclerCell fastCell = holder.ViewCellContainer.Element as FormsRecyclerCell;
                fastCell?.OnCellRecycled();
            }
        }

        public FormsRecyclerViewAdapter(IList items, RecyclerView recyclerView, FormsRecyclerView gridView)
        {
            this.Items = items;
            this.recyclerView = recyclerView;
            this.Element = gridView;
            this.displayMetrics = Android.App.Application.Context.Resources.DisplayMetrics;
            this.screenDpWidth = (int)(this.displayMetrics.WidthPixels / this.displayMetrics.Density);
        }

        public class FormsRecyclerViewCell : RecyclerView.ViewHolder
        {
            public FormsRecyclerViewCellContainer ViewCellContainer { get; set; }

            public FormsRecyclerViewCell(FormsRecyclerViewCellContainer view) : base(view)
            {
                this.ViewCellContainer = view;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [selection enable].
        /// </summary>
        /// <value><c>true</c> if [selection enable]; otherwise, <c>false</c>.</value>
        public bool SelectionEnabled
        {
            get;
            set;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            FormsRecyclerCell fastCell;

            fastCell = this.Element.ItemTemplate.CreateContent() as FormsRecyclerCell;

            var view = new FormsRecyclerViewCellContainer(parent.Context, fastCell, parent, this.Element.ItemWidth, this.Element);
            if (this.SelectionEnabled)
            {
                view.Click += MainView_Click;
            }

            var dpW = this.ConvertDpToPixels(this.Element.ItemWidth);
            var dpH = this.ConvertDpToPixels(this.Element.ItemHeight);
            view.SetMinimumWidth(dpW);
            view.SetMinimumHeight(dpH);
            view.LayoutParameters = new GridLayoutManager.LayoutParams(dpW, GridLayoutManager.LayoutParams.WrapContent);
            return new FormsRecyclerViewCell(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            FormsRecyclerViewCell myHolder = holder as FormsRecyclerViewCell;

            var item = this.Items.Cast<object>().ElementAt(position);
            myHolder.ViewCellContainer.Update(item);
            myHolder.ViewCellContainer.Invalidate();
            myHolder.ViewCellContainer.ForceLayout();
        }

        private void MainView_Click(object sender, EventArgs e)
        {
            int position = this.recyclerView.GetChildAdapterPosition((Android.Views.View)sender);
            var item = this.Items.Cast<object>().ElementAt(position);
        }

        public override int ItemCount
        {
            get
            {
                return this.Items?.Count ?? 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            return 0;
        }

        public static int IndexOf(IList collection, object element, IEqualityComparer comparer = null)
        {
            int i = 0;
            comparer = comparer ?? EqualityComparer<object>.Default;
            foreach (var currentElement in collection)
            {
                if (comparer.Equals(currentElement, element))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private void RemoveListenerCollectionChanged()
        {
            var oldCollection = this.items as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= this.NewCollection_CollectionChanged;
            }
        }

        private int ConvertDpToPixels(double dpValue)
        {
            return ((int)dpValue == this.screenDpWidth) ? this.displayMetrics.WidthPixels : (int)(((dpValue) * this.displayMetrics.Density));
        }

        #region CollectionChanged events 
        private async void NewCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (this.IsDisposed)
            {
                this.RemoveListenerCollectionChanged();
            }
            else
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        await this.CollectionChangedAddAsync(e);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        await this.CollectionChangedMoveAsync(e);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        await this.CollectionChangedRemoveAsync(e);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        await this.CollectionChangedReplaceAsync(e);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        await this.CollectionChangedResetAsync();
                        break;
                }
            }
        }

        private async Task<bool> CanNotifyCollectionAsync()
        {
            bool retValue = false;
            for (int i = 0; i < 30; i++)//30 frames == half second 
            {
                if (this.IsDisposed)
                {
                    this.RemoveListenerCollectionChanged();
                    break;
                }
                else if (!recyclerView.IsComputingLayout)
                {
                    retValue = true;
                    break;
                }
                else
                {
                    await Task.Delay(17);//one frame
                }
            }
            return retValue;
        }

        /// <summary>
        /// Method that checks if we can notify the recyclerView that collection has changed
        /// </summary>
        /// <param name="notifyAction">Action to notify</param>
        private async Task NotifyWrapperAsync(Action notifyAction)
        {
            //We wait maximum half second to recyclerView stop compiuting layout. (Cannot notify recyclerView when layout it's computing, because of exceptions)
            //During that time, recyclerView can be removed, so we have to check it every time we try to notify
            if (await this.CanNotifyCollectionAsync())
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (this.IsDisposed)
                    {
                        this.RemoveListenerCollectionChanged();
                    }
                    else
                    {
                        notifyAction.Invoke();
                    }
                });
            }
        }

        private async Task CollectionChangedAddAsync(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                await this.NotifyWrapperAsync(() =>
                {
                    var index = IndexOf(items, item);
                    this.NotifyItemInserted(index);
                });
            }
        }

        private async Task CollectionChangedMoveAsync(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null || e.OldItems == null || e.NewItems.Count == 0 || e.OldItems.Count == 0)
            {
                return;
            }
            var oldIndex = e.OldStartingIndex;
            var newIndex = e.NewStartingIndex;

            foreach (var item in e.NewItems)
            {
                await this.NotifyWrapperAsync(() =>
                {
                    this.NotifyItemMoved(oldIndex, newIndex);
                    ++oldIndex;
                    ++newIndex;
                });
            }
        }

        private async Task CollectionChangedRemoveAsync(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null)
            {
                return;
            }

            foreach (var item in e.OldItems)
            {
                await this.NotifyWrapperAsync(() =>
                {
                    this.NotifyItemRemoved(e.OldStartingIndex);
                });
            }
        }

        private async Task CollectionChangedReplaceAsync(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null || e.OldItems == null || e.NewItems.Count == 0 || e.OldItems.Count == 0 ||
                (e.NewItems.Count > 0 && e.OldItems.Count > 0 && e.NewItems[0] == e.OldItems[0]))
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                await this.NotifyWrapperAsync(() =>
                {
                    var index = IndexOf(this.items, item);
                    this.NotifyItemChanged(index);
                });
            }
        }

        private async Task CollectionChangedResetAsync()
        {
            await this.NotifyWrapperAsync(() =>
            {
                this.TryNotifyDataSetChanged();
            });
        }

        private void TryNotifyDataSetChanged()
        {
            //Exception happens when we try to change data set, and InotifyCollectionChanged was fired, but we left page with recycleVIew....
            try
            {
                this.NotifyDataSetChanged();
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.RemoveListenerCollectionChanged();
            this.IsDisposed = true;
            base.Dispose(disposing);
        }
        #endregion
    }
}