using Android.Views;
using System.Collections.Generic;

namespace FormsRecyclerViewApp.Droid
{
    /// <summary>
    /// Make this class available through your favourite IOC/service method
    /// </summary>
    /// <remarks>
    /// Based on https://github.com/twintechs/TwinTechsFormsLib
    /// </remarks>
    public class FormsRecyclerCellCache
    {
        private Dictionary<Android.Views.View, CachedData> cachedDataByView;

        private static FormsRecyclerCellCache instance;

        public FormsRecyclerCellCache()
        {
            this.cachedDataByView = new Dictionary<Android.Views.View, CachedData>();
        }

        public static FormsRecyclerCellCache Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FormsRecyclerCellCache();
                }
                return instance;
            }
        }

        public CachedData GetCellCache(ViewGroup parent)
        {
            if (!this.cachedDataByView.ContainsKey(parent))
            {
                this.cachedDataByView[parent] = new CachedData();
            }
            return this.cachedDataByView[parent];
        }

        public void FlushAllCaches()
        {
            foreach (var cachedData in this.cachedDataByView.Values)
            {
                cachedData.Reset();
            }
            this.cachedDataByView = new Dictionary<Android.Views.View, CachedData>();
        }

        public class CachedData
        {
            private Dictionary<Android.Views.View, FormsRecyclerCell> CellItemsByCoreCells { get; set; }

            private Dictionary<FormsRecyclerCell, object> OriginalBindingContextsForReusedItems { get; set; }

            internal CachedData()
            {
                this.Reset();
            }

            /// <summary>
            /// Reset this instance. 
            /// </summary>
            internal void Reset()
            {
                this.CellItemsByCoreCells = new Dictionary<Android.Views.View, FormsRecyclerCell>();
                this.OriginalBindingContextsForReusedItems = new Dictionary<FormsRecyclerCell, object>();
            }

            public void RecycleCell(Android.Views.View view, FormsRecyclerCell newCell)
            {
                if (this.CellItemsByCoreCells.ContainsKey(view))
                {
                    var reusedItem = this.CellItemsByCoreCells[view];
                    if (this.OriginalBindingContextsForReusedItems.ContainsKey(newCell))
                    {
                        reusedItem.BindingContext = this.OriginalBindingContextsForReusedItems[newCell];
                    }
                    else
                    {
                        reusedItem.BindingContext = newCell.BindingContext;
                    }
                }
            }

            public bool IsCached(Android.Views.View view)
            {
                return this.CellItemsByCoreCells.ContainsKey(view);
            }

            public void CacheCell(FormsRecyclerCell cell, Android.Views.View view)
            {
                this.CellItemsByCoreCells[view] = cell;
                this.OriginalBindingContextsForReusedItems[cell] = cell.BindingContext;
            }

            public object GetBindingContextForReusedCell(FormsRecyclerCell cell)
            {
                if (this.OriginalBindingContextsForReusedItems.ContainsKey(cell))
                {
                    return this.OriginalBindingContextsForReusedItems[cell];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}