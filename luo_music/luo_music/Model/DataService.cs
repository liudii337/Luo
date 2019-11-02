using Luo.Shared.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LuoMusic.Model
{
    public class DataService : IDataService
    {
        public Task<DataItem> GetData()
        {
            // Use this to connect to the actual data service

            // Simulate by returning a DataItem
            var item = new DataItem("Welcome to MVVM Light");
            return Task.FromResult(item);
        }

        public Task<ObservableCollection<LuoVol>> GetVolList()
        {
            return LuoVolFactory.getlist();
        }
    }
}