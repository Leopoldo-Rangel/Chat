using Chat.Shared.BrokerContracts;
using CsvHelper;
using MassTransit;
using Quote.Worker.Model;
using System.Globalization;

namespace Quote.Worker.Consumer
{
    public class AddQuoteMessageConsumer : IConsumer<IAddQuoteMessage>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IHttpClientFactory _clientFactory;

        public AddQuoteMessageConsumer(ISendEndpointProvider sendEndpointProvider, 
            IHttpClientFactory clientFactory)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _clientFactory = clientFactory;
        }

        public async Task Consume(ConsumeContext<IAddQuoteMessage> context)
        {
            var url = $"https://stooq.com/q/l/?s={context.Message.StockName}&f=sd2t2ohlcv&h&e=csv";
            var httpClient = _clientFactory.CreateClient();
            using var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var record = csv.GetRecords<QuoteModel>().SingleOrDefault();
            if (record == null)
            {
                await _sendEndpointProvider.Send<IQuoteMessageResponse>(new
                {
                    Success = false,
                    context.Message.ChatRoomId,
                    Error = "There was an unknown error while retrieving the stock quote, please try again"
                });
            }
            else if (record.Close == "N/D")
            {
                await _sendEndpointProvider.Send<IQuoteMessageResponse>(new
                {
                    Success = false,
                    context.Message.ChatRoomId,
                    Error = "Stock was not found"
                });
            }
            else
            {
                await _sendEndpointProvider.Send<IQuoteMessageResponse>(new
                {
                    Success = true,
                    context.Message.ChatRoomId,
                    Quote = $"{context.Message.StockName} quote is ${record.Close} per share"
                });
            }
        }
    }
}
