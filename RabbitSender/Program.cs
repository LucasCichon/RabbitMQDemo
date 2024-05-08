using RabbitMQ;
using RabbitMQ.Client;
using System.Text;

namespace RabbitSender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string messageFromSender = string.Empty;
            ConnectionFactory factory = new();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            factory.ClientProvidedName = "Rabbit Sender App";

            IConnection cnn = factory.CreateConnection();

            IModel channel = cnn.CreateModel();

            while(messageFromSender != "exit")
            {
                string exchangeName = "DemoExchange";
                string routingKey = "demo-routing-key";
                string queueName = "DemoQueue";

                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                channel.QueueDeclare(queueName, false, false, false, null);
                channel.QueueBind(queueName, exchangeName, routingKey, null);

                Console.WriteLine("Send message or provide 'exit' to close the sender:");
                messageFromSender = Console.ReadLine();
                if(messageFromSender != "exit")
                {
                    for(int i=0; i<60; i++)
                    {
                        Console.WriteLine($"Adding message ({messageFromSender}:{i})");
                        byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"{messageFromSender}:{i}");
                        channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
                        Thread.Sleep(1000);
                    }
                }
            }

            channel.Close();
            cnn.Close();
        }
    }
}
