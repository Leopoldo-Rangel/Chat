namespace Chat.Shared.BrokerContracts
{
    public interface IQuoteMessageResponse
    {
        bool Success { get; set; }
        string Quote { get; set; }
        int ChatRoomId { get; set; }
        string Error { get; set; }
    }
}
