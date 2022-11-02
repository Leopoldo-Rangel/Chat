namespace Chat.Shared.BrokerContracts
{
    public interface IAddQuoteMessage
    {
        string StockName { get; set; }
        int ChatRoomId { get; set; }
    }
}
