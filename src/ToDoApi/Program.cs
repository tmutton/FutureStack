using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ToDoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://*:5000")  // listen on port 5000 on all network interfaces; needed for containers
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}


