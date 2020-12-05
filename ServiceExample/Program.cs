using System;
using Eneter.Messaging.EndPoints.TypedMessages;
using Eneter.Messaging.MessagingSystems.MessagingSystemBase;
using Eneter.Messaging.MessagingSystems.TcpMessagingSystem;
using Eneter.Messaging.MessagingSystems.Composites;
using Eneter.Messaging.DataProcessing.Serializing;

namespace ServiceExample
{
    // Request message type
    public class MyRequest
    {
        public string Text { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
    }

    // Response message type
    public class MyResponse
    {
        public int Length { get; set; }
        public int Length2 { get; set; }
        public int Length3 { get; set; }
    }

    class Program
    {
        private static IDuplexTypedMessageReceiver<MyResponse, MyRequest> myReceiver;

        static void Main(string[] args)
        {
            // Create message receiver receiving 'MyRequest' and receiving 'MyResponse'.
            IDuplexTypedMessagesFactory aReceiverFactory = new DuplexTypedMessagesFactory();
            myReceiver = aReceiverFactory.CreateDuplexTypedMessageReceiver<MyResponse, MyRequest>();

            // Subscribe to handle messages.
            myReceiver.MessageReceived += OnMessageReceived;

            // Create TCP messaging.
            IMessagingSystemFactory aMessaging = new TcpMessagingSystemFactory();
            IDuplexInputChannel anInputChannel
                = aMessaging.CreateDuplexInputChannel("tcp://127.0.0.1:8060/");
                //= aMessaging.CreateDuplexInputChannel("tcp://192.168.173.1:8060/");

            // Attach the input channel and start to listen to messages.
            myReceiver.AttachDuplexInputChannel(anInputChannel);

            Console.WriteLine("The service is running. To stop press enter.");
            Console.WriteLine("Daftar Pesanan");
            Console.ReadLine();

            // Detach the input channel and stop listening.
            // It releases the thread listening to messages.
            myReceiver.DetachDuplexInputChannel();
        }

        // It is called when a message is received.
        private static void OnMessageReceived(object sender, TypedRequestReceivedEventArgs<MyRequest> e)
        {
            Console.WriteLine("Jumlah PC : " + e.RequestMessage.Text);
            Console.WriteLine("Waktu Mulai : " + e.RequestMessage.Text2);
            Console.WriteLine("Lama Pemakaian : " + e.RequestMessage.Text3);


            // Create the response message.
            MyResponse aResponse = new MyResponse();
            //aResponse.Length = e.RequestMessage.Text.Length;

            // Send the response message back to the client.
            myReceiver.SendResponseMessage(e.ResponseReceiverId, aResponse);
        }
    }
}
