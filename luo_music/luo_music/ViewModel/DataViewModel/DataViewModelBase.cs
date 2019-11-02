using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LuoMusic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LuoMusic.ViewModel.DataViewModel
{
    public abstract class DataViewModelBase<T> : ViewModelBase
    {
        public static int DEFAULT_PAGE_INDEX => 1;

        private int PageIndex { get; set; } = DEFAULT_PAGE_INDEX;

        public event Action<IEnumerable<T>, int> OnLoadIncrementalDataCompleted;

        public event Action<bool> OnHasMoreItemChanged;

        private IncrementalLoadingCollection<T> _dataList;
        public IncrementalLoadingCollection<T> DataList
        {
            get
            {
                return _dataList;
            }
            set
            {
                _dataList = value;
                RaisePropertyChanged(() => DataList);
            }
        }

        private RelayCommand<T> _viewDetailCommand;
        public RelayCommand<T> ViewDetailCommand
        {
            get
            {
                if (_viewDetailCommand != null) return _viewDetailCommand;
                return _viewDetailCommand = new RelayCommand<T>((data) =>
                {
                    try
                    {
                        if (data == null) return;
                        ClickItem(data);
                    }
                    catch (Exception e)
                    {
                        //var task = Logger.LogAsync(e);
                    }
                });
            }
        }

        public DataViewModelBase()
        {
            DataList = new IncrementalLoadingCollection<T>(count =>
            {
                return Task.Run(() => GetIncrementalListData(PageIndex++));
            });
        }

        public async Task<bool> RefreshAsync()
        {
            try
            {
                if (DataList.IsBusy)
                {
                    return false;
                }

                PageIndex = DEFAULT_PAGE_INDEX;

                if (DataList == null)
                {
                    DataList = new IncrementalLoadingCollection<T>(count =>
                    {
                        return GetIncrementalListData(PageIndex++);
                    });
                }

                await DataList.LoadMoreItemsAsync(20u);

                RefreshCompleted();

                return true;
            }
            catch (Exception e)
            {
                //var task = Logger.LogAsync(e);
                return false;
            }
        }

        public async Task RetryAsync()
        {
            await DataList.LoadMoreItemsAsync(20u);
        }

        private async Task<ResultData<T>> GetIncrementalListData(int pageIndex)
        {
            IEnumerable<T> newList = new List<T>();
            bool HasMoreItems = false;
            try
            {
                newList = await GetList(pageIndex);
                HasMoreItems = newList?.Count() > 0;

                await RunOnUiThread(() =>
                {
                    if (pageIndex == DEFAULT_PAGE_INDEX)
                    {
                        DataList.Clear();
                    }

                    LoadMoreItemCompleted(newList, pageIndex);
                });
            }
            catch (Exception e)
            {
                //var task = Logger.LogAsync(e);
            }
            return new ResultData<T>() { Data = newList, HasMoreItems = HasMoreItems };
        }

        protected async Task RunOnUiThread(DispatchedHandler handler)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);
        }

        protected abstract Task<IEnumerable<T>> GetList(int pageIndex);

        protected abstract void ClickItem(T item);

        protected abstract void RefreshCompleted();

        protected abstract void LoadMoreItemCompleted(IEnumerable<T> list, int index);
    }
}
