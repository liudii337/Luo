using Luo.Shared.Data;
using Luo.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Service
{
    public class VolService : VolServiceBase
    {
        protected string RequestUrl { get; set; }

        public int Count { get; set; } = 20;
        // to realize 20 each time
        public int Offset { get; set; } = 1;
        public int LastPage { get; set; } = 1;

        public int TagOffset { get; set; } = 1;
        public int TagLastPage { get; set; } = 1;


        public VolService(string url, LuoVolFactory factory,
            CancellationTokenSourceFactory ctsFactory) : base(factory, ctsFactory)
        {
            RequestUrl = url;
        }

        public Task<string> GetVolDetailHtmlAsync(string url)
        {
            // 暂用新的API
            return _cloudService.GetVolDetailHtmlAsync_w(url);
        }

        public override async Task<IEnumerable<LuoVol>> GetVolsAsync()
        {
            //网络请求获得原料
            // 暂用新的API
            // check if it's tag request
            if(RequestUrl.Contains("all"))
            {
                //var result = await _cloudService.GetVolsAsync_w(Offset, Count, GetCancellationToken(), RequestUrl);
                var result = await _cloudService.GetNumVolsAsync_w(Offset, Count, GetCancellationToken(), RequestUrl);
                if (result != null)
                {
                    //工厂加工，获得成品
                    var volList = _VolFactory.GetVols(result);
                    // to realize 20 each time
                    if ((volList.Count - (Page - LastPage) * Count) > Count)
                    {
                        //the rest is more than 20
                        var list = volList.ToList().GetRange((Page - LastPage) * Count, Count);
                        return list;
                    }
                    else
                    {
                        //the rest is less than 20
                        var list = volList.Skip((Page - LastPage) * Count);
                        Offset = Offset + 1;
                        LastPage = Page + 1;
                        return list;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var result = await _cloudService.GetTagVolsAsync_w(TagOffset, Count, GetCancellationToken(), RequestUrl);
                if (result != null)
                {
                    //工厂加工，获得成品
                    var volList = _VolFactory.GetVols(result);
                    // to realize 20 each time
                    if ((volList.Count - (Page - TagLastPage) * Count) > Count)
                    {
                        //the rest is more than 20
                        var list = volList.ToList().GetRange((Page - TagLastPage) * Count, Count);
                        return list;
                    }
                    else
                    {
                        //the rest is less than 20
                        var list = volList.Skip((Page - TagLastPage) * Count);
                        TagOffset = TagOffset + 1;
                        TagLastPage = Page + 1;
                        return list;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<LuoVol> GetLatestVolAsync()
        {
            // 获取最新一期Vol信息
            var result = await _cloudService.GetNumVolsAsync_w(1, 0, GetCancellationToken(), Request.GetAllVol_q);
            if (result != null)
            {
                //工厂加工，获得成品
                var volList = _VolFactory.GetVols(result);
                // to realize 20 each time
                if (volList != null)
                {
                    return volList[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }

    }
}
