using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Host_Server
{
    public class Program
    {
        private static string consoleModeArgument = "--console";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var webHostArgs = args.Where(arg => !consoleModeArgument.Equals(arg)).ToArray();

            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:57308/");
        }
    }
}