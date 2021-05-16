using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ShowScheduler.Areas.Identity.IdentityHostingStartup))]
namespace ShowScheduler.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}