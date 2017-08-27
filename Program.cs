using System;
using System.Threading;
using System.Threading.Tasks;
using SystemEx;
using WampSharp.V2;
using WampSharp.V2.Fluent;
using WampSharp.V2.Client;
using WampSharp.V2.Rpc;
using WampSharp.V2.Authentication;
using System.Reactive.Subjects;

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
            await Task.Delay(200); // Avoid warning
            return "johan-" + input + "-johan";
        }
    }

    class Program
    {
        static IAsyncDisposable registration;

        static async Task StartWamp()
        {
            WampChannelFactory cf = new WampChannelFactory();

            IWampChannel channel =
                cf.ConnectToRealm("jdm")
//                       .WebSocketTransport("ws://40.86.85.83:8080/ws")
                       .WebSocketTransport("ws://ws01.jdm1.maassluis:9001/wamp")
                       .JsonSerialization()
                       //.Authenticator(new TicketAuthenticator())
                       .Build();
            var mon = channel.RealmProxy.Monitor;
            mon.ConnectionEstablished += (p1, p2) => Console.WriteLine("Established!");
            mon.ConnectionError += (p1, p2) => Console.WriteLine("Error!");
            mon.ConnectionBroken += (p1, p2) => Console.WriteLine("Broken!");
            Console.WriteLine("Connecting...");
            await channel.Open();
            Console.WriteLine("Connected!");

            IWhisper instance = new Whisper();

            IWampRealmProxy realm = channel.RealmProxy;

            registration = await realm.Services.RegisterCallee(instance);
            Console.WriteLine("Registered!");
            ISubject<string> newWhisper = realm.Services.GetSubject<string>("nl.jdm.newWhisper");
            newWhisper.OnNext("johan");
        }

        static async Task StopWamp()
        {
            Console.WriteLine("Stopping...");
            if (registration != null)
                await registration.DisposeAsync();
            Console.WriteLine("Stopped.");
        }

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (v1, v2) =>
            {
                StopWamp().Wait();
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
