using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FormsRecyclerViewApp
{
    /// <summary>
    /// Class FormsRecyclerView.
    /// </summary>
    /// <remarks>
    /// Based on https://github.com/twintechs/TwinTechsFormsLib
    /// </remarks>
    public class FormsRecyclerView : ContentView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormsRecyclerView"/> class.
        /// </summary>InvokeItemSelectedEvent

        #region Static Bindable properties declarations
        /// <summary>
        /// The items source property
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource), typeof(IList), typeof(FormsRecyclerView));

        /// <summary>
        /// The item template property
        /// </summary>
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate), typeof(DataTemplate), typeof(FormsRecyclerView));

        /// <summary>
        /// The item width property
        /// </summary>
        public static readonly BindableProperty ItemWidthProperty = BindableProperty.Create(
            nameof(ItemWidth), typeof(double), typeof(FormsRecyclerView), (double)100);

        /// <summary>
        /// The item height property
        /// </summary>
        public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create(
            nameof(ItemHeight), typeof(double), typeof(FormsRecyclerView), (double)100);

        /// <summary>
        /// The is horizontal property
        /// </summary>
        public static readonly BindableProperty IsHorizontalProperty = BindableProperty.Create(
            nameof(IsHorizontal), typeof(bool), typeof(FormsRecyclerView), false);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IList ItemsSource
        {
            get
            {
                return (IList)base.GetValue(FormsRecyclerView.ItemsSourceProperty);
            }
            set
            {
                base.SetValue(FormsRecyclerView.ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item template.
        /// </summary>
        /// <value>The item template.</value>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(FormsRecyclerView.ItemTemplateProperty);
            }
            set
            {
                base.SetValue(FormsRecyclerView.ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the item.
        /// </summary>
        /// <value>The width of the item.</value>
        public double ItemWidth
        {
            get
            {
                return (double)base.GetValue(FormsRecyclerView.ItemWidthProperty);
            }
            set
            {
                base.SetValue(FormsRecyclerView.ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the item.
        /// </summary>
        /// <value>The height of the item.</value>
        public double ItemHeight
        {
            get
            {
                return (double)base.GetValue(FormsRecyclerView.ItemHeightProperty);
            }
            set
            {
                base.SetValue(FormsRecyclerView.ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [selection enabled].
        /// </summary>
        /// <value><c>true</c> if [selection enabled]; otherwise, <c>false</c>.</value>
        public bool SelectionEnabled
        {
            get;
            set;
        }

        public bool IsHorizontal
        {
            get
            {
                return (bool)base.GetValue(FormsRecyclerView.IsHorizontalProperty);
            }
            set
            {
                base.SetValue(FormsRecyclerView.IsHorizontalProperty, value);
            }
        }
        #endregion Properties
        public event EventHandler<int> OnAppearing;

        public void SetScrollYOnAppearing(int positionY)
        {
            this.OnAppearing?.Invoke(this, positionY);
        }
    }
}
