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

        public Task<string> GetVolDetailHtmlAsync(string url)
        {
            return _cloudService.GetVolDetailHtmlAsync(url);
        }

        public override async Task<IEnumerable<LuoVol>> GetVolsAsync()
        {
            //网络请求获得原料
            var result = await _cloudService.GetVolsAsync(Page, Count, GetCancellationToken(), RequestUrl);
            if (result != null)
            {
                //工厂加工，获得成品
                var volList = _VolFactory.GetVols(result);
                return volList;
            }
            else
            {
                return null;
            }
        }
    }
}
