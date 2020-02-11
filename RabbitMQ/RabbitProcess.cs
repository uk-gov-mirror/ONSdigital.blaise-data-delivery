using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StatNeth.Blaise.API.ServerManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Text.Json;

namespace RabbitMQ
{
    public partial class RabbitProcess : ServiceBase
    {
        // Instantiate logger.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        // Objects for connecting and setting up RabbitMQ.
        public IConnection connection;
        public IModel channel;
        public EventingBasicConsumer consumer;

        /// <summary>
        /// Class constructor for initialising the service.
        /// </summary>        
        public RabbitProcess()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is our entry point when debugging. It allows us to use the service without running the installation steps.
        /// </summary>
        public void OnDebug()
        {
            OnStart(null);
        }

        /// <summary>
        /// OnStart method triggers when the service starts.
        /// </summary>
        /// <param name="args">Optional argument that can be passed on service start.</param>
        protected override void OnStart(string[] args)
        {
            log.Info("Blaise Data Delivery service started.");

            // Connect to RabbitMQ and setup channels.
            while (!SetupRabbit())
            {
                // Keep re-trying RabbitMQ connection until connected.
                log.Info("Waiting for RabbitMQ connection...");
                System.Threading.Thread.Sleep(5000);
            }

            // Consume and process messages on the RabbitMQ queue.
            ConsumeMessage();
        }

        /// <summary>
        /// OnStop method triggers when the service stops.
        /// </summary>
        protected override void OnStop()
        {
            log.Info("Blaise Data Delivery service stopped.");
        }

        /// <summary>
        /// Method for connecting to RabbitMQ and setting up the channels.
        /// </summary>
        public bool SetupRabbit()
        {
            log.Info("Setting up RabbitMQ.");

            try
            {
                // Create a connection to RabbitMQ using the Rabbit credentials stored in the app.config file.
                var connFactory = new ConnectionFactory()
                {
                    HostName = ConfigurationManager.AppSettings["RabbitHostName"],
                    UserName = ConfigurationManager.AppSettings["RabbitUserName"],
                    Password = ConfigurationManager.AppSettings["RabbitPassword"]
                };
                connection = connFactory.CreateConnection();
                channel = connection.CreateModel();

                // Get the exchange and queue details from the app.config file.
                string exchangeName = ConfigurationManager.AppSettings["RabbitExchange"];
                string queueName = ConfigurationManager.AppSettings["DataDeliveryQueueName"];

                // Declare the exchange for receiving messages.
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);
                log.Info("Exchange declared - " + exchangeName);

                // Declare the queue for receiving messages.
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                log.Info("Queue declared - " + queueName);

                // Bind the queue for receiving messages.
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: queueName);
                log.Info("Queue binding complete - Queue: " + queueName + " / Exchange: " + exchangeName + " / Routing Key: " + queueName);

                // Declare the queue for sending status updates.
                string DataDeliveryQueueName = ConfigurationManager.AppSettings["DataDeliveryQueueName"];
                channel.QueueDeclare(queue: DataDeliveryQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                log.Info("Queue declared - " + DataDeliveryQueueName);

                // Only consuming one message at a time.
                channel.BasicQos(0, 1, false);

                // Create the consumer object which will run our code when receiving messages.
                consumer = new EventingBasicConsumer(channel);
                log.Info("Consumer object created.");

                log.Info("RabbitMQ setup complete.");

                return true;
            }
            catch
            {
                log.Info("Unable to establish RabbitMQ connection.");
                return false;
            }
        }

        /// <summary>
        /// Method for consuming and processing messages on the RabbitMQ queue.
        /// </summary>
        public void ConsumeMessage()
        {
            // Functionality to be performed when a message is received.
            consumer.Received += (model, ea) =>
            {
                // Read message.
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                log.Info("Message received - " + message);

                // Deserialize message.
                var messageJson = new Dictionary<string, string>();

                try
                {
                    //Now using system.Text.Json as this is build into .net core 3.0 - Newtonsoft.Json now not required.
                    messageJson = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
                }
                catch (Exception e)
                {
                    log.Error("Error Deserializing Message.");
                    log.Error(e.Message);
                    log.Error(e.StackTrace);
                    return;
                }

                // If there are source server park details in the message.
                if ((messageJson["source_hostname"] != "") && (messageJson["source_hostname"] != null)
                    && (messageJson["source_server_park"] != "") && (messageJson["source_server_park"] != null)
                        && (messageJson["source_instrument"] != "") && (messageJson["source_instrument"] != null))
                {
                    string serverName = messageJson["source_hostname"];
                    string userName = ConfigurationManager.AppSettings["BlaiseServerUserName"];
                    string password = ConfigurationManager.AppSettings["BlaiseServerPassword"];
                    string binding = ConfigurationManager.AppSettings["BlaiseServerBinding"];

                    IConnectedServer serverManagerConnection = null;
                    try
                    {
                        // Connect to Bliase server
                        log.Info("Connecting to Blaise Server Manager.");
                        serverManagerConnection = ServerManager.ConnectToServer(serverName, 8031, userName, GetPassword(password), binding);
                    }
                    catch (Exception e)
                    {
                        log.Error("Error connecting to Blaise Server Manager.");
                        log.Error(e.Message);
                        log.Error(e.StackTrace);
                    }

                    // Loop through the server parks on the connected Blaise server.
                    log.Info("Looping through server parks.");
                    foreach (IServerPark serverPark in serverManagerConnection.ServerParks)
                    {
                        log.Info("Server park found - " + serverPark.Name);
                        if (serverPark.Name == messageJson["source_server_park"])
                        {
                            // Loop through the surveys installed on the server park.
                            log.Info("Looping through surveys.");
                            foreach (ISurvey survey in serverManagerConnection.GetServerPark(serverPark.Name).Surveys)
                            {
                                log.Info("Survey found - " + survey.Name);
                                if (survey.Name == messageJson["source_instrument"])
                                {
                                    log.Info("Matching survey found.");
                                    // TO DO - Data Delivery !
                                }
                            }
                        }
                    }
                }

                // If there is a source file in the message.
                if ((messageJson["source_file"] != "") && (messageJson["source_file"] != null))
                {
                    // Using Blaise5 api's, create the SPS, ASC and SAV files for the survey posted here
                    BlaiseDataDelivery.Program.MainDDE(messageJson["source_file"], messageJson["output_filepath"]);
                }

                // Remove from queue when done processing.
                channel.BasicAck(ea.DeliveryTag, false);
            };

            // Consume and process any messages already held on the queue.
            string queueName = ConfigurationManager.AppSettings["DataDeliveryQueueName"];
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        /// <summary>
        /// Converts a password to secure string.
        /// </summary>
        /// <param name="pw">Password to be converted to secure string.</param>
        /// <returns></returns>
        private static SecureString GetPassword(string pw)
        {
            char[] passwordChars = pw.ToCharArray();
            SecureString password = new SecureString();
            foreach (char c in passwordChars)
            {
                password.AppendChar(c);
            }
            return password;
        }
    }
}