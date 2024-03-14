﻿using AsyncAwaitBestPractices;
using BurnInControl.Application.ProcessSerial.Handlers;
using BurnInControl.Shared.ComDefinitions;
using BurnInControl.Shared.ComDefinitions.Packets;
using BurnInControl.Shared.ComDefinitions.Station;
using BurnInControl.Shared.Hubs;
using ErrorOr;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using StationService.Infrastructure.Firmware;
using StationService.Infrastructure.Hub;
using StationService.Infrastructure.TestLogs;
using System.Text.Json;
using Wolverine;
using BurnInControl.Application.ProcessSerial.Messages;
using MediatR;
namespace StationService.Infrastructure.SerialCom;

public class StationMessageHandler:IStationMessageHandler{
    private readonly BurnInTestService _testService;
    private readonly IHubContext<StationHub, IStationHub> _hubContext;
    private readonly ILogger<StationMessageHandler> _logger;
    private readonly FirmwareUpdateService _firmwareService;
    private readonly IMediator _mediator;
    
    public StationMessageHandler(ILogger<StationMessageHandler> logger,
        BurnInTestService testService,
        IHubContext<StationHub, IStationHub> hubContext,
        FirmwareUpdateService firmwareService,
        IMediator mediator) {
        this._testService = testService;
        this._logger = logger;
        this._hubContext = hubContext;
        this._firmwareService = firmwareService;
        this._mediator = mediator;
    }
    
    public Task Handle(StationMessage message,CancellationToken cancellationToken) {
        try {
            if (!string.IsNullOrEmpty(message.Message)) {
                if (message.Message.Contains("Prefix")) {
                    var doc=JsonSerializer.Deserialize<JsonDocument>(message.Message);
                    if (doc != null) { 
                        return this.Parse(doc);
                    } else {
                        this._logger.LogWarning("JsonDocument was null");
                        return Task.CompletedTask;
                    }
                } else {
                    this._logger.LogWarning("MessagePacket did not contain Prefix");
                    return Task.CompletedTask;
                }
            } else {
                this._logger.LogWarning("Mediator request MessagePacket json text was null or empty");
                return Task.CompletedTask;
            }
        } catch {
            this._logger.LogWarning($"Message had errors.  Message: {message}");
            return Task.CompletedTask;
        }
    }
    
    private Task Parse(JsonDocument doc) {
        var prefixValue=doc.RootElement.GetProperty("Prefix").ToString();
        if (!string.IsNullOrEmpty(prefixValue)) {
            var prefix=StationMsgPrefix.FromValue(prefixValue);
            if (prefix != null) {
                var packetElem=doc.RootElement.GetProperty("Packet");
                switch (prefix.Name) {
                    case nameof(StationMsgPrefix.DataPrefix): {
                        return this.HandleData(packetElem);
                    }
                    case nameof(StationMsgPrefix.MessagePrefix): {
                        return this.HandleMessage(packetElem, false);
                    }
                    case nameof(StationMsgPrefix.InitMessage): {
                        return this.HandleMessage(packetElem, false);
                    }
                    /*case nameof(StationMsgPrefix.IdRequest): {
                        return this.HandleIdChanged(packetElem);
                    }
                    case nameof(StationMsgPrefix.VersionRequest): {
                        return this.HandleVersionRequest(packetElem);
                    }*/
                    case nameof(StationMsgPrefix.TestStatus): {
                        return this.HandleTestStatus(packetElem);
                    }
                    default: {
                        this._logger.LogWarning($"Prefix value {prefix.Value} not implemented");
                        return Task.CompletedTask;
                    }
                }
            } else {
                this._logger.LogWarning("ArduinoMsgPrefix.FromValue(prefixValue) was null");
                return Task.CompletedTask;
            }
        } else {
            this._logger.LogWarning("Prefix value null or empty");
            return Task.CompletedTask;
        }
    }
    
    private Task HandleData(JsonElement element) {
        try {
            var serialData=element.Deserialize<StationSerialData>();
            if (serialData != null) {
                this._testService.Log(serialData);
                return this._hubContext.Clients.All.OnSerialCom(serialData);
            }
            return Task.CompletedTask;
        } catch(Exception e) {
            this._logger.LogWarning("Failed to deserialize station data");
            return Task.CompletedTask;
        }
    }

    private Task HandleMessage(JsonElement element,bool isInit) {
        var message=element.GetProperty("Message").ToString();
        return this._hubContext.Clients.All.OnSerialComMessage(message);
    }

    /*private Task HandleIdChanged(JsonElement element) {
        try {
            var id = element.GetString();
            return Task.CompletedTask;
            /*return this._mediator.Publish(new ControllerIdReceived() {
                ControllerId = id
            });#1#
        } catch {
            this._logger.LogError("Failed to parse Controller Id");
            return Task.CompletedTask;
        }
    }*/
    
    /*private async Task HandleVersionRequest(JsonElement element) {
        try {
            var version = element.GetString();
            if (!string.IsNullOrEmpty(version)) {
                /*await this._mediator.Send(new CheckIfNewerVersion() {
                    ControllerVersion = version
                });#1#
            } else {
                this._logger.LogError("Failed to check firmware version. Version string was null or empty");
            }
        } catch(Exception e) {
            this._logger.LogError("Update check failed Exception: {Error}",e.Message);
        }
    }*/

    private Task HandleTestStatus(JsonElement element) {
        try {
            var success = element.GetProperty("Status").GetBoolean();
            var message = element.GetProperty("Message").GetString();
            return success ? 
                this._hubContext.Clients.All.OnTestStatus(new StartTestStatus() { Status = Result.Success })
                : this._hubContext.Clients.All.OnTestStatus(new StartTestStatus(){Status = Error.Failure(description:message)});
        } catch(Exception e) {
            var message = $"Failed to parse test status message packet. Exception: {e.Message}";
            this._logger.LogError(message);
            return this._hubContext.Clients.All.OnTestStatus(new StartTestStatus() {
                Status = Error.Unexpected(description: message)
            });
        }
    }
}