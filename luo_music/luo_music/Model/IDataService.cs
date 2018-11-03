using System.Threading.Tasks;

namespace luo_music.Model
{
    public interface IDataService
    {
        Task<DataItem> GetData();
    }
}