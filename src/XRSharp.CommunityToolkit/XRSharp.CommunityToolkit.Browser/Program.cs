﻿using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace XRSharp.CommunityToolkit.Browser;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        var host = builder.Build();
        await host.RunAsync();
    }

    public static void RunApplication()
    {
        Application.RunApplication(async () =>
        {
            await XRSharp.Root3D.Initialize();
            var app = new XRSharp.CommunityToolkit.App();
        });
    }
}
