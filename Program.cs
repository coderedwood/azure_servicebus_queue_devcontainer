// See https://aka.ms/new-console-template for more information
using DotNetEnv;
using Azure.Messaging.ServiceBus;

namespace azure_servicebus_queue_devcontainer
{
    class Program
    {   
        //hardcoded method
        // // connection string to your Service Bus namespace
        // private const string connectionString = "<<CONNECTION STRING>>";

        //hardcoded method
        // // name of your Service Bus topic
        // private const string queueName = "<<QUEUE NAME>>";
        
        //For use with DotNetEnv
        // connection string to your Service Bus namespace
        private const string connectionString = "CONNECTION_STRING"; //Key contatining actual value in .env file

        //For use with DotNetEnv
        // name of your Service Bus topic
        private const string queueName = "QUEUE_NAME"; //Key containing actual value in .env file

        public static async Task Main(string[] args){
            //Load Environment Variables (loads global(system variables) and local(.env variables) )
            DotNetEnv.Env.Load();

            // the client that owns the connection and can be used to create senders and receivers
            ServiceBusClient client;

            // the sender used to publish messages to the queue
            ServiceBusSender sender;

            //For use with hardcoded method
            // Create the clients that we'll use for sending and processing messages.
            // client = new ServiceBusClient(connectionString);
            // sender = client.CreateSender(queueName);

            //For use with DotNetEnv
            // Create the clients that we'll use for sending and processing messages.
            client = new ServiceBusClient(Environment.GetEnvironmentVariable(connectionString));
            sender = client.CreateSender(Environment.GetEnvironmentVariable(queueName));

            // create a batch 
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= 3; i++)
            {
                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    // if an exception occurs
                    throw new Exception($"Exception {i} has occurred.");
                }
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of three messages has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine("Follow the directions in the exercise to review the results in the Azure portal.");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            ServiceBusProcessor processor;
            // client = new ServiceBusClient(connectionString);
            
            //For use with DotNetEnv
            client = new ServiceBusClient(Environment.GetEnvironmentVariable(connectionString));

            // create a processor that we can use to process the messages
            // processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());


            //For use with DotNetEnv
            processor = client.CreateProcessor(Environment.GetEnvironmentVariable(queueName), new ServiceBusProcessorOptions());

            try
            {
                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }

            // handle received messages
            async Task MessageHandler(ProcessMessageEventArgs args)
            {
                string body = args.Message.Body.ToString();
                Console.WriteLine($"Received: {body}");

                // complete the message. messages is deleted from the queue. 
                await args.CompleteMessageAsync(args.Message);
            }

            // handle any errors when receiving messages
            Task ErrorHandler(ProcessErrorEventArgs args)
            {
                Console.WriteLine(args.Exception.ToString());
                return Task.CompletedTask;
            }
        
            // DotNetEnv.Env.Load();
            // Console.WriteLine("DotNetEnv Test");
            // Console.WriteLine(connectionString);
            // Console.WriteLine(Environment.GetEnvironmentVariable(connectionString));
        } 
    }
}