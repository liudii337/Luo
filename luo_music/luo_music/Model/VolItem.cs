using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Luo.Shared;
using Luo.Shared.Data;
using Luo.Shared.Service;
using LuoMusic.ViewModel;
using LuoMusic.ViewModel.DataViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

namespace LuoMusic.Model
{
    public class VolItem : ModelBase
    {
        [IgnoreDataMember]
        public MainViewModel MainVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MainViewModel>();
            }
        }

        private LuoVol _vol;
        public LuoVol Vol
        {
            get
            {
                return _vol;
            }
            set
            {
                if (_vol != value)
                {
                    _vol = value;
                    RaisePropertyChanged(() => Vol);
                }
            }
        }

        //private SolidColorBrush _infoForeColor;
        //[IgnoreDataMember]
        //public SolidColorBrush InfoForeColor
        //{
        //    get
        //    {
        //        return _infoForeColor;
        //    }
        //    set
        //    {
        //        if (_infoForeColor != value)
        //        {
        //            _infoForeColor = value;
        //            RaisePropertyChanged(() => InfoForeColor);
        //        }
        //    }
        //}

        private RelayCommand _shareCommand;
        [IgnoreDataMember]
        public RelayCommand ShareCommand
        {
            get
            {
                if (_shareCommand != null) return _shareCommand;
                return _shareCommand = new RelayCommand(() =>
                {
                    //ToggleShare();
                });
            }
        }

        //private RelayCommand _navigateHomeCommand;
        //[IgnoreDataMember]
        //public RelayCommand NavigateHomeCommand
        //{
        //    get
        //    {
        //        if (_navigateHomeCommand != null) return _navigateHomeCommand;
        //        return _navigateHomeCommand = new RelayCommand(async () =>
        //        {
        //            if (!string.IsNullOrEmpty(Image.Owner.Links.HomePageUrl))
        //            {
        //                await Launcher.LaunchUriAsync(new Uri(Image.Owner.Links.HomePageUrl));
        //            }
        //        });
        //    }
        //}

        //private RelayCommand _downloadCommand;
        //[IgnoreDataMember]
        //public RelayCommand DownloadCommand
        //{
        //    get
        //    {
        //        if (_downloadCommand != null) return _downloadCommand;
        //        return _downloadCommand = new RelayCommand(() =>
        //        {
        //            var downloaditem = new DownloadItem(this);
        //            var task = downloaditem.DownloadFullImageAsync(JP.Utils.Network.CTSFactory.MakeCTS());
        //            var task2 = DownloadsVM.AddDownloadingImageAsync(downloaditem);
        //        });
        //    }
        //}


        [IgnoreDataMember]
        public StorageFile DownloadedFile { get; set; }

        //public DownloadStatus DownloadStatus { get; set; } = DownloadStatus.Pending;

        //public string ShareText => $"Share {Image.Owner.Name}'s amazing photo from MyerSplash app. {Image.Urls.Full}";


        private VolService _service = new VolService(null, new LuoVolFactory(),
            CancellationTokenSourceFactory.CreateDefault());

        public VolItem()
        {
        }

        public VolItem(LuoVol vol)
        {
            Vol = vol;
            //BitmapSource = new CachedBitmapSource();
        }

        public void Init()
        {
            TitleString = $"Vol.{Vol.VolNum} {Vol.Title}";
            if (MainVM.HeartVM.IsHeartedVol(Vol))
            {
                IsHeartVol = true;
            }
            else
            {
                IsHeartVol = false;
            }
        }

        private string _titleString;
        public string TitleString
        {
            get
            {
                return _titleString;
            }
            set
            {
                if (_titleString != value)
                {
                    _titleString = value;
                    RaisePropertyChanged(() => TitleString);
                }
            }
        }

        private bool _isHeartVol = true;
        public bool IsHeartVol
        {
            get
            {
                return _isHeartVol;
            }
            set
            {
                if (_isHeartVol != value)
                {
                    _isHeartVol = value;
                    if (value)
                    {
                        MainVM.HeartVM.AddVol(this);
                    }
                    else
                    {
                        MainVM.HeartVM.RemoveVol(this);
                    }
                    RaisePropertyChanged(() => IsHeartVol);
                }
            }

        }

        //public string GetFileNameForDownloading()
        //{
        //    var fileName = $"{Image.Owner.Name}  {Image.SimpleCreateTimeString}.jpg";
        //    var invalidChars = Path.GetInvalidFileNameChars();
        //    foreach (var c in invalidChars)
        //    {
        //        if (fileName.Contains(c))
        //        {
        //            fileName = fileName.Replace(c.ToString(), "");
        //        }
        //    }
        //    return fileName;
        //}

        //public async Task SetDataRequestDataAsync(DataRequest request)
        //{
        //    var requestData = request.Data;
        //    requestData.SetWebLink(new Uri(Image.Urls.Full));
        //    requestData.Properties.Title = $"Share a photo by {Image.Owner?.Name ?? "Unknown"}";
        //    requestData.Properties.ContentSourceWebLink = new Uri(Image.Urls.Full);
        //    requestData.Properties.ContentSourceApplicationLink = new Uri(Image.Urls.Full);

        //    requestData.SetText(ShareText);

        //    var file = await StorageFile.GetFileFromPathAsync(BitmapSource.LocalPath);
        //    if (file != null)
        //    {
        //        List<IStorageItem> imageItems = new List<IStorageItem>
        //        {
        //            file
        //        };
        //        requestData.SetStorageItems(imageItems);

        //        var imageStreamRef = RandomAccessStreamReference.CreateFromFile(file);
        //        requestData.SetBitmap(imageStreamRef);
        //        requestData.Properties.Thumbnail = imageStreamRef;
        //    }
        //}

        //public async Task TryLoadBitmapAsync()
        //{
        //    if (BitmapSource.Bitmap != null) return;
        //    var url = GetUrlFromSettings();

        //    if (string.IsNullOrEmpty(url)) return;

        //    var task = CheckAndGetDownloadedFileAsync();

        //    BitmapSource.ExpectedFileName = Image.ID + ".jpg";
        //    BitmapSource.RemoteUrl = url;
        //    await BitmapSource.LoadBitmapAsync();
        //}

        //public async Task CheckAndGetDownloadedFileAsync()
        //{
        //    var name = GetFileNameForDownloading();
        //    var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerSplash", CreationCollisionOption.OpenIfExists);
        //    if (folder != null)
        //    {
        //        if (await folder.TryGetItemAsync(name) is StorageFile file)
        //        {
        //            var pro = await file.GetBasicPropertiesAsync();
        //            if (pro.Size > 10)
        //            {
        //                this.DownloadStatus = DownloadStatus.Ok;
        //                DownloadedFile = file;
        //            }
        //        }
        //    }
        //}

        //public string GetUrlFromSettings()
        //{
        //    var quality = App.AppSettings.LoadQuality;
        //    switch (quality)
        //    {
        //        case 0: return Image.Urls.Regular;
        //        case 1: return Image.Urls.Small;
        //        case 2: return Image.Urls.Thumb;
        //        default: return "";
        //    }
        //}

        //public string GetSaveImageUrlFromSettings()
        //{
        //    var quality = App.AppSettings.SaveQuality;
        //    switch (quality)
        //    {
        //        case 0: return Image.Urls.Raw;
        //        case 1: return Image.Urls.Full;
        //        case 2: return Image.Urls.Regular;
        //        default: return "";
        //    }
        //}

        //public string GetDownloadLocationUrl()
        //{
        //    return Image?.Links?.DownloadLocation;
        //}

        public async Task GetVolDetialAsync()
        {
            var result = await _service.GetVolDetailHtmlAsync(Vol.VolUrl);
            if(result!=null)
            {
                // 暂用新的API
                Vol.GetDetailVol_w(result);
            }
            //if (result.IsRequestSuccessful)
            //{
            //    JsonObject.TryParse(result.JsonSrc, out JsonObject json);
            //    if (json != null)
            //    {
            //        var exifObject = JsonParser.GetJsonObjFromJsonObj(json, "exif");
            //        if (exifObject != null)
            //        {
            //            Image.Exif = JsonConvert.DeserializeObject<ImageExif>(exifObject.ToString());
            //            RaisePropertyChanged(() => SizeString);
            //        }

            //        var locationObj = JsonParser.GetJsonObjFromJsonObj(json, "location");
            //        if (locationObj != null)
            //        {
            //            Image.Location = JsonConvert.DeserializeObject<ImageLocation>(locationObj.ToString());
            //            RaisePropertyChanged(() => LocationString);
            //        }
            //    }
            //}
        }

        //public void ToggleShare()
        //{
        //    DataTransferManager.GetForCurrentView().DataRequested += DownloadItemTemplate_DataRequested;
        //    DataTransferManager.ShowShareUI();
        //}

        //private async void DownloadItemTemplate_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        //{
        //    var deferral = args.Request.GetDeferral();
        //    sender.TargetApplicationChosen += (s, e) =>
        //    {
        //        deferral.Complete();
        //    };
        //    await SetDataRequestDataAsync(args.Request);
        //    deferral.Complete();
        //    DataTransferManager.GetForCurrentView().DataRequested -= DownloadItemTemplate_DataRequested;
        //}
    }
}
