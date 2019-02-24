using Luo.Shared.Data;
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

        public int Count { get; set; } = 30;

        public VolService(string url, LuoVolFactory factory,
            CancellationTokenSourceFactory ctsFactory) : base(factory, ctsFactory)
        {
            RequestUrl = url;
        }

        //public Task<string> GetImageDetailAsync(string id)
        //{
        //    return _cloudService.GetVolDetailAsync(id, GetCancellationToken());
        //}

        public override async Task<IEnumerable<LuoVol>> GetVolsAsync()
        {
            //网络请求获得原料
            var result = await _cloudService.GetVolsAsync(Page, Count, GetCancellationToken(), RequestUrl);
            if (result != null)
            {
                //工厂加工，获得成品
                var volList = await _VolFactory.GetVolsAsync(result);
                return volList;
            }
            else
            {
                return null;
            }
        }
    }
}
