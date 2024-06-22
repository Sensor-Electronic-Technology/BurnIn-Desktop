using System.Diagnostics;
using BurnInControl.HubDefinitions.Hubs;
using BurnInControl.Shared;
using Microsoft.AspNetCore.SignalR;
using Docker.DotNet;

namespace BurnInControl.HostRunner.Hubs;
public class HostHub:Hub {
    private readonly ILogger<HostHub> _logger;
    private readonly IDockerClient _dockerClient;
    
    public HostHub(ILogger<HostHub> logger,IDockerClient dockerClient) {
        this._logger = logger;
        this._dockerClient = dockerClient;
    }
    
    public Task RestartService() {
        return Task.CompletedTask;
    }

    public async Task RestartBrowser() {
        await this.CloseBrowser();
        await this.OpenBrowser();
    }
    
    private async Task CloseBrowser() {
        this._logger.LogInformation("Closing browser...");
        using Process process = new Process();
        process.StartInfo.FileName = "pkill";
        process.StartInfo.Arguments = "-f chromium-browser";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        try {
            process.Start();
            var result = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            Console.WriteLine(result);
            
        } catch(Exception e) {
            this._logger.LogError("Error while closing browser" +
                                  "\n  {ErrorMessage}", e.ToErrorMessage());
            
        }
    }
    
    private async Task OpenBrowser() {
        this._logger.LogInformation("Opening browser...");
        using Process process = new Process();
        process.StartInfo.FileName = "chromium-browser";
        process.StartInfo.Arguments = "--start-fullscreen http://localhost";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        try {
            process.Start();
            var result = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            Console.WriteLine(result);
        } catch(Exception e) {
            this._logger.LogError("Error while opening browser" +
                                  "\n  {ErrorMessage}", e.ToErrorMessage());
            
        }
    }
}