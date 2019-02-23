using Luo.Shared.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace luo_music.Model
{
    public interface IDataService
    {
        Task<DataItem> GetData();
        Task<ObservableCollection<LuoVol>> GetVolList();
    }
}