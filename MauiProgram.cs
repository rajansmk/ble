using CommunityToolkit.Maui;
using MauiShinyTest.Extension;
using Microsoft.Extensions.Logging;

using Shiny;
using Shiny.Infrastructure;

namespace MauiShinyTest;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            // THIS IS REQUIRED TO BE DONE FOR SHINY TO RUN
            .UseShiny()
            .RegisterInfrastructure()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
            .RegisterShinyServices();

#if DEBUG
		builder.Logging.AddDebug();
        builder.Services.AddBluetoothLE();
        


#endif

        return builder.Build();
	}
    static MauiAppBuilder RegisterShinyServices(this MauiAppBuilder builder)
    {
        

        return builder;
    }
}
