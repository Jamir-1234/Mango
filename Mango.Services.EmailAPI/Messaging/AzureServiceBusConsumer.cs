using Azure.Messaging.ServiceBus;
using Mango.Email.Models;
using Mango.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConusmer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _emailCartprocessor;
        private readonly ServiceBusProcessor _registerUserProcessor;
        private readonly EmailService _emailService;
        public AzureServiceBusConsumer(IConfiguration configuration,EmailService emailService)
        {
            _emailService = emailService;
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCart");
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueuenames:RegisterUserQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartprocessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor=client.CreateProcessor(registerUserQueue);
               
        }

        public async  Task Start()
        {
            _emailCartprocessor.ProcessMessageAsync += OnEmailCartRequestRecieved;
            _emailCartprocessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartprocessor.StopProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestRecieved;
           _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StopProcessingAsync();

        }

        

        public async Task Stop()
        {
           await _emailCartprocessor.StopProcessingAsync();
          await  _emailCartprocessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();
        }
       

        private  async Task OnEmailCartRequestRecieved(ProcessMessageEventArgs args)
        {
            //this is where you will recieve messges 
            var message=args.Message;
            var body=Encoding.UTF8.GetString(message.Body); 
            CartDto objMessage=JsonConvert.DeserializeObject<CartDto>(body);
            try {
                //try to log email
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task OnUserRegisterRequestRecieved(ProcessMessageEventArgs args)
        {
            //this is where you will recieve messges 
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            string email= JsonConvert.DeserializeObject<string>(body);
            try
            {
                //try to log email
                await _emailService.RegisterUserEmailAndLog(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
