// See https://aka.ms/new-console-template for more information
using DotNetEnv;
using Azure.Messaging.ServiceBus;

namespace azure_servicebus_queue_devcontainer
{
    class Program
    {
        // connection string to your Service Bus namespace
        string connectionString = "CONNECTION_STRING";

        // name of your Service Bus topic
        string queueName = "QUEUE_NAME";
        
        static void Main(string[] args){
            DotNetEnv.Env.Load();
            Console.WriteLine("DotNetEnv Test");
            Console.WriteLine(Program.connectionString);
            // Console.WriteLine(Environment.GetEnvironmentVariable(connectionString));
        } 
    }
}