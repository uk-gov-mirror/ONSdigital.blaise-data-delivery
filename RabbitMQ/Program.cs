using System;

namespace RabbitMQ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Listening on Rabbit MQ ...");
            RabbitProcess rabbit = new RabbitProcess();
            rabbit.OnDebug();
        }
    }
}
