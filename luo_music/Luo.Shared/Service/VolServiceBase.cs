using Luo.Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luo.Shared.Data
{
    public abstract class VolServiceBase : IService
    {
        protected CloudService _cloudService = new CloudService();
        protected LuoVolFactory _VolFactory;
        private CancellationTokenSourceFactory _ctsFactory;
        private CancellationTokenSource _cts;

        public int Page { get; set; } = 1;

        public VolServiceBase(LuoVolFactory factory, CancellationTokenSourceFactory ctsFactory)
        {
            _VolFactory = factory;
            _ctsFactory = ctsFactory;
        }

        protected CancellationToken GetCancellationToken()
        {
            Cancel();
            _cts = _ctsFactory.Create();
            return _cts.Token;
        }

        public abstract Task<IEnumerable<LuoVol>> GetVolsAsync();

        public void Cancel()
        {
            _cts?.Cancel();
        }
    }
}
