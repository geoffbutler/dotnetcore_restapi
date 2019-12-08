using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ContactsCore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureWebHostDefaults(c =>
                    {
                        c.UseIISIntegration()
                            .UseStartup<Startup>();
                    })
                    .Build();

            host.Run();
        }
    }
}
