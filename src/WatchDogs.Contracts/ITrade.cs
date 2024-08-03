
namespace WatchDogs.Contracts;

public interface ITrade
{
    string Action { get; set; }
    int Balance { get; set; }
    string Currency { get; set; }
    int DealID { get; set; }
    decimal Lot { get; set; }
    DateTime TimeStamp { get; set; }
}