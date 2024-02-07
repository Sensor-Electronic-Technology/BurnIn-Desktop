﻿using BurnIn.Shared.Models;
using System.Data;
namespace BurnIn.Shared.Hubs;

public interface IStationHub {
    Task OnUsbConnect(bool connected);
    Task OnUsbDisconnect(bool disconnected);
    Task OnExecuteCommand(bool executed);
    Task OnSerialCom(StationSerialData serialData);
    Task OnIdChanged(string Id);
    Task OnSerialComMessage(string message);
    Task OnSettingsUploaded(bool uploaded);

    Task OnUpdateChecked(FirmwareUpdateStatus firmwareUpdateStatus);
    Task OnFirmwareUpdated(bool completed,string newVersion,string message,string consoleMessage);
    
    //Test Related
    Task OnTestStarted();
    Task OnTestStartedFailed(string message);
    Task OnTestPaused(string message);
    Task OnTestPausedFailed(string message);
    Task OnTestContinued(string message);
    Task OnTestContinuedFailed(string message);
    Task OnTestCompleted(string message);
    Task OnTestCompletedFailed(string message);
    Task OnReceiveFirmwareUploadText(string output);
    Task OnTestSetupSucceeded();
    Task OnTestSetupFailed(string? message);
}