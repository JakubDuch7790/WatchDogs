
namespace BogusTest;

public interface IDataGeneratorTest
{
    TradeModel GenerateFakeTrade();
    IEnumerable<TradeModel> GenerateFakeTrades();
    List<TradeModel> LoadFakeData();
}