using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace LuoMusic.Common
{
    public class ResultData<T>
    {
        public IEnumerable<T> Data { get; set; }

        public bool HasMoreItems { get; set; }
    }

    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public Func<uint, Task<ResultData<T>>> DataFetchDelegate = null;

        public IncrementalLoadingCollection()
        {
        }

        public IncrementalLoadingCollection(Func<uint, Task<ResultData<T>>> dataFetchDelegate)
        {
            this.DataFetchDelegate = dataFetchDelegate ??
                throw new ArgumentNullException("dataFetchDelegate should not be null");
        }

        public bool HasMoreItems
        {
            get;
            private set;
        }

        public bool IsAppending { get; set; } = true;

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        protected async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                if (IsBusy)
                {
                    return new LoadMoreItemsResult() { Count = 0 };
                }

                IsBusy = true;

                var result = await DataFetchDelegate(count);

                var items = result.Data;

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (IsAppending)
                        {
                            this.Add(item);
                        }
                        else this.Insert(0, item);
                    }
                }

                this.HasMoreItems = result.HasMoreItems;

                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.OnLoadMoreCompleted?.Invoke(items == null ? 0 : items.Count());
                });

                return new LoadMoreItemsResult { Count = items == null ? 0 : (uint)items.Count() };
            }
            finally
            {
                IsBusy = false;
            }
        }

        public delegate void LoadMoreStarted(uint count);

        public delegate void LoadMoreCompleted(int count);

        public event LoadMoreCompleted OnLoadMoreCompleted;

        public bool IsBusy = false;
    }
}
