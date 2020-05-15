using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SSPreventer
{
    internal class Program
    {
        private static async Task Main(string[] args)
            => await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddSingleton<MouseController>()
                    .AddSingleton<CursorPositionGetter>()
                    .AddSingleton<ISSPreventerConfig, SSPreventerConfig>(provider =>
                        provider.GetRequiredService<IConfiguration>().Get<SSPreventerConfig>())
                    .AddHostedService<SSPreventer>())
                .RunConsoleAsync();
    }
}
