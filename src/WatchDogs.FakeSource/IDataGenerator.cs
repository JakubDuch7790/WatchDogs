
namespace WatchDogs.FakeSource;

public interface IDataGenerator
{
    TradeModel GenerateFakeTrade();
    IEnumerable<TradeModel> GenerateFakeTrades();
    List<TradeModel> LoadFakeData();
}