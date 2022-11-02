using Chat.Shared.BrokerContracts;
using MassTransit;
using Quote.Worker.Consumer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddTransient<AddQuoteMessageConsumer>();
        services.AddMassTransit(x =>
        {
            var timeout = TimeSpan.FromSeconds(30);
            var quoteResponseUri = new Uri("queue:quote-response");
            x.AddRequestClient<IQuoteMessageResponse>(quoteResponseUri, timeout);
            EndpointConvention.Map<IQuoteMessageResponse>(quoteResponseUri);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq", "/", h => {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("quote-request", e =>
                {
                    e.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30)));
                    e.Consumer<AddQuoteMessageConsumer>(context);
                });
            });
        });

        services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                // if specified, waits until the bus is started before
                // returning from IHostedService.StartAsync
                // default is false
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(60);
            });
    })
    .Build();

await host.RunAsync();
