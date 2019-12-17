using Luo.Shared.Data;
using LuoMusic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LuoMusic.ViewModel.DataViewModel
{
    public class VolDataViewModel : DataViewModelBase<VolItem>
    {
        protected MainViewModel _mainViewModel;
        protected VolServiceBase _volService;

        private Visibility _footerLoadingVisibility=Visibility.Visible;
        public Visibility FooterLoadingVisibility
        {
            get
            {
                return _footerLoadingVisibility;
            }
            set
            {
                if (_footerLoadingVisibility != value)
                {
                    _footerLoadingVisibility = value;
                    RaisePropertyChanged(() => FooterLoadingVisibility);
                }
            }
        }

        private Visibility _endVisiblity=Visibility.Collapsed;
        public Visibility EndVisibility
        {
            get
            {
                return _endVisiblity;
            }
            set
            {
                if (_endVisiblity != value)
                {
                    _endVisiblity = value;
                    RaisePropertyChanged(() => EndVisibility);
                }
            }
        }

        public VolDataViewModel(MainViewModel viewModel, VolServiceBase service)
        {
            _mainViewModel = viewModel;
            _volService = service;
        }

        public void Cancel()
        {
            _volService?.Cancel();
        }

        protected override void ClickItem(VolItem item)
        {
        }

        protected IEnumerable<VolItem> CreateVolItems(IEnumerable<LuoVol> vols)
        {
            var list = new List<VolItem>();
            foreach (var i in vols)
            {
                list.Add(new VolItem(i));
            }
            return list;
        }

        protected void UpdateHintVisibility(IEnumerable<VolItem> list)
        {
            FooterLoadingVisibility = Visibility.Collapsed;
            ////_mainViewModel.FooterReloadVisibility = Visibility.Collapsed;
            ////_mainViewModel.NoNetworkHintVisibility = Visibility.Collapsed;
            EndVisibility = Visibility.Collapsed;

            // No items at all
            if (DataList.Count == 0)
            {
                if (list == null || list.Count() == 0)
                {
                    //_mainViewModel.NoItemHintVisibility = Visibility.Visible;
                }
            }
            else
            {
                //_mainViewModel.NoItemHintVisibility = Visibility.Collapsed;
                //VolCover = DataList[0].Vol.Cover;
                if (list == null || list.Count() == 0)
                {
                    EndVisibility = Visibility.Visible;
                }
            }
        }

        protected async override Task<IEnumerable<VolItem>> GetList(int pageIndex)
        {
            try
            {
                if (pageIndex >= 2)
                {
                    var task = RunOnUiThread(() =>
                    {
                        FooterLoadingVisibility = Visibility.Visible;
                        EndVisibility = Visibility.Collapsed;
                        //_mainViewModel.NoItemHintVisibility = Visibility.Collapsed;
                    });
                }

                var result = await RequestAsync(pageIndex);

                await RunOnUiThread(() =>
                {
                    UpdateHintVisibility(result);
                });

                return result;
            }
            catch (Exception e2)
            {
                //var task = Logger.LogAsync(e2);
                HandleFailed(e2);
                return new List<VolItem>();
            }
        }

        private void HandleFailed(Exception e)
        {
            var task = RunOnUiThread(() =>
            {
                //_mainViewModel.FooterLoadingVisibility = Visibility.Collapsed;
                //_mainViewModel.FooterReloadVisibility = Visibility.Collapsed;

                //_mainViewModel.IsRefreshing = false;

                //if (_mainViewModel.DataVM.DataList?.Count == 0)
                //{
                //    _mainViewModel.NoNetworkHintVisibility = Visibility.Visible;
                //}
                //else
                //{
                //    _mainViewModel.NoNetworkHintVisibility = Visibility.Collapsed;
                //    _mainViewModel.FooterReloadVisibility = Visibility.Visible;
                //}

                //ToastService.SendToast(e.Message);
            });
        }

        protected override void LoadMoreItemCompleted(IEnumerable<VolItem> list, int pagingIndex)
        {
            if(list!=null)
            {
                foreach (var item in list)
                {
                    item.Init();
                }
            }
        }

        protected async virtual Task<IEnumerable<VolItem>> RequestAsync(int pageIndex)
        {
            try
            {
                _volService.Page = pageIndex;
                var result = await _volService.GetVolsAsync();
                if (result != null)
                {
                    return CreateVolItems(result);
                }
                else
                    return null;
                    //throw new APIException("Request failed");
            }
            catch (TaskCanceledException)
            {
                return null;
                //throw new APIException("Request timeout");
            }
        }

        protected override void RefreshCompleted()
        {
        }
    }
}
