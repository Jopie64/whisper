using System;
using System.Threading;
using System.Threading.Tasks;
using SystemEx;
using WampSharp.V2;
using WampSharp.V2.Fluent;
using WampSharp.V2.Client;
using WampSharp.V2.Rpc;

namespace whisper
{
    public interface IWhisper
    {
        [WampProcedure("nl.jdm.johan")]
        Task<string> johan(string input);
    }

    public class Whisper : IWhisper
    {
        async Task<string> IWhisper.johan(string input)
        {
            return "johan-" + input + "-johan";
        }
    }

    class Program
    {
        static async Task StartWamp()
        {
            WampChannelFactory cf = new WampChannelFactory();
            var test = new WampSharp.V2.Client.Authenticator();

            IWampChannel channel =
                cf.ConnectToRealm("jdm")
                       .WebSocketTransport(new Uri("ws://40.86.85.83:443/ws"))
                       .JsonSerialization()
                       .Authenticator(test)
                       .Build();
            Console.WriteLine("Connecting...");
            await channel.Open();
            Console.WriteLine("Connected!");

            IWhisper instance = new Whisper();

            IWampRealmProxy realm = channel.RealmProxy;

            await realm.Services.RegisterCallee(instance);
            Console.WriteLine("Registered!");
        }

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (v1, v2) =>
            {
                Console.WriteLine("Stopping...");
            };
            Console.WriteLine("Starting...");
            Task.Run(async () => { await StartWamp(); }).Wait();
            Console.WriteLine("Now waiting for exit...");
            while(true)
            {
                var line = Console.Read();
                Console.WriteLine($"Input: {line}");
            }
        }
    }
}
