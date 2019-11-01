using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace luo_music.Common
{
    public interface IListViewAdapter
    {
        void OnAttachToListView(ListViewBase listView);
        void OnChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args);
        void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args);
    }

    public class AdaptiveGridView : GridView
    {
        #region DependencyProperties

        /// <summary>
        /// Minimum height for item
        /// </summary>
        public double MinItemHeight
        {
            get { return (double)GetValue(AdaptiveGridView.MinItemHeightProperty); }
            set { SetValue(AdaptiveGridView.MinItemHeightProperty, value); }
        }

        public static readonly DependencyProperty MinItemHeightProperty =
            DependencyProperty.Register(
                "MinItemHeight",
                typeof(double),
                typeof(AdaptiveGridView),
                new PropertyMetadata(1.0, (s, a) =>
                {
                    if (!double.IsNaN((double)a.NewValue))
                    {
                        ((AdaptiveGridView)s).InvalidateMeasure();
                    }
                }));

        /// <summary>
        /// Minimum width for item (must be greater than zero)
        /// </summary>
        public double MinItemWidth
        {
            get { return (double)GetValue(AdaptiveGridView.MinimumItemWidthProperty); }
            set { SetValue(AdaptiveGridView.MinimumItemWidthProperty, value); }
        }

        public static readonly DependencyProperty MinimumItemWidthProperty =
            DependencyProperty.Register(
                "MinItemWidth",
                typeof(double),
                typeof(AdaptiveGridView),
                new PropertyMetadata(1.0, (s, a) =>
                {
                    if (!Double.IsNaN((double)a.NewValue))
                    {
                        ((AdaptiveGridView)s).InvalidateMeasure();
                    }
                }));

        #endregion DependencyProperties

        private IListViewAdapter _adapter;
        public IListViewAdapter Adapter
        {
            get
            {
                return _adapter;
            }
            set
            {
                if (_adapter != value)
                {
                    _adapter = value;
                    _adapter.OnAttachToListView(this);
                }
            }
        }

        public AdaptiveGridView()
        {
            if (this.ItemContainerStyle == null)
            {
                this.ItemContainerStyle = new Style(typeof(GridViewItem));
            }

            this.ItemContainerStyle.Setters.Add(new Setter(GridViewItem.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));

            this.Loaded += (s, a) =>
            {
                if (this.ItemsPanelRoot != null)
                {
                    this.InvalidateMeasure();
                }
            };

            this.ChoosingItemContainer += AdaptiveGridView_ChoosingItemContainer;
            this.ContainerContentChanging += AdaptiveGridView_ContainerContentChanging;
        }

        private void AdaptiveGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            Adapter?.OnContainerContentChanging(sender, args);
        }

        private void AdaptiveGridView_ChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args)
        {
            Adapter?.OnChoosingItemContainer(sender, args);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var panel = this.ItemsPanelRoot as ItemsWrapGrid;
            if (panel != null)
            {
                if (MinItemWidth == 0)
                    throw new DivideByZeroException("You need to have a MinItemWidth greater than zero");

                var availableWidth = availableSize.Width - (this.Padding.Right + this.Padding.Left);

                var numColumns = Math.Floor(availableWidth / MinItemWidth);
                numColumns = numColumns == 0 ? 1 : numColumns;
                var numRows = Math.Ceiling(this.Items.Count / numColumns);

                var itemWidth = availableWidth / numColumns;
                var aspectRatio = MinItemHeight / MinItemWidth;
                var itemHeight = itemWidth * aspectRatio;

                panel.ItemWidth = itemWidth;
                panel.ItemHeight = itemHeight;
            }

            return base.MeasureOverride(availableSize);
        }
    }

}
