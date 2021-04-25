using dotenv.net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using WebApi.Extensions;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args) =>
                    await CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) => 
                Host.CreateDefaultBuilder(args)
                .ConfigureOwnerMetrics()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    DotEnv.Load();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
