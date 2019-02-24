using Luo.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luo_music.ViewModel.DataViewModel
{
    public class VolDataViewModel : DataViewModelBase<LuoVol>
    {
        protected MainViewModel _mainViewModel;
        protected VolServiceBase _volService;

        public VolDataViewModel(MainViewModel viewModel, VolServiceBase service)
        {
            _mainViewModel = viewModel;
            _volService = service;
        }

        public void Cancel()
        {
            _volService?.Cancel();
        }

        protected override void ClickItem(LuoVol item)
        {
        }

        protected void UpdateHintVisibility(IEnumerable<LuoVol> list)
        {
            //_mainViewModel.FooterLoadingVisibility = Visibility.Collapsed;
            //_mainViewModel.FooterReloadVisibility = Visibility.Collapsed;
            //_mainViewModel.NoNetworkHintVisibility = Visibility.Collapsed;
            //_mainViewModel.EndVisibility = Visibility.Collapsed;

            //// No items at all
            //if (DataList.Count == 0)
            //{
            //    if (list.Count() == 0)
            //    {
            //        _mainViewModel.NoItemHintVisibility = Visibility.Visible;
            //    }
            //}
            //else
            //{
            //    _mainViewModel.NoItemHintVisibility = Visibility.Collapsed;

            //    if (list.Count() == 0)
            //    {
            //        _mainViewModel.EndVisibility = Visibility.Visible;
            //    }
            //}
        }

        protected async override Task<IEnumerable<LuoVol>> GetList(int pageIndex)
        {
            try
            {
                if (pageIndex >= 2)
                {
                    var task = RunOnUiThread(() =>
                    {
                        //_mainViewModel.FooterLoadingVisibility = Visibility.Visible;
                        //_mainViewModel.EndVisibility = Visibility.Collapsed;
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
                return new List<LuoVol>();
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

        protected override void LoadMoreItemCompleted(IEnumerable<LuoVol> list, int pagingIndex)
        {
            foreach (var item in list)
            {
                //item.Init();
            }
        }

        protected async virtual Task<IEnumerable<LuoVol>> RequestAsync(int pageIndex)
        {
            try
            {
                _volService.Page = pageIndex;
                var result = await _volService.GetVolsAsync();
                if (result != null)
                {
                    return result;
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
    }
}
