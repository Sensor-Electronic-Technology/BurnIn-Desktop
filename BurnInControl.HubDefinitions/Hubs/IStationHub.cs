﻿using BurnInControl.Data.ComponentConfiguration;
using BurnInControl.Data.StationModel.Components;
using BurnInControl.HubDefinitions.HubTransports;
using BurnInControl.Shared.ComDefinitions;
using BurnInControl.Shared.ComDefinitions.Station;
using BurnInControl.Shared.FirmwareData;

namespace BurnInControl.HubDefinitions.Hubs;

public interface IStationHub {

#region StationNotifications
    Task OnStationData(StationSerialData serialData);
    Task OnTuningData(TuningSerialData tuningData);
    Task OnSerialComError(StationMsgPrefix prefix,string message);
    Task OnSerialComMessage(string message);
    Task OnSerialNotifyMessage(string message);
    Task OnSerialErrorMessage(string message);
    Task OnSerialInitMessage(string message);
    Task OnConfigSaveStatus(string type,bool success, string message);
    Task OnRequestConfigHandler(bool success,int configType,string jsonConfig);

#endregion  

#region BurnInTest
    Task OnTestStarted(string message);
    Task OnTestStartedFrom(LoadTestSetupTransport testSetupTransport);
    Task OnTestStartedFromUnknown(LoadTestSetupTransport testSetupTransport);
    Task OnTestStartedFailed(string message);
    Task OnTestCompleted(string message);
    Task OnTestSetup(bool success, string message);
    Task OnLoadFromSavedState(TestSetupTransport testSetupTransport);
    Task OnLoadFromSavedStateError(string message);
    Task OnStopAndSaved(bool success, string message);
    
#endregion

#region FirmwareUpdateNotifications
    Task OnFirmwareUpdateCheck(UpdateCheckStatus checkStatus);
    Task OnFirmwareUpdateCheckFailed(string message);
    Task OnFirmwareUpdateStarted();
    Task OnFirmwareUpdateFailed(string errorMessage);
    Task OnFirmwareUpdateCompleted(string version);
    Task OnFirmwareDownloaded(bool success,string message);
    Task OnFirmwareUpdated(string version,string arduinoOutput);
#endregion

#region ConnectionStatus
    Task OnUsbConnectFailed(string message);
    Task OnUsbDisconnect(string message);
    Task OnUsbConnect(string message);
    Task OnUsbDisconnectFailed(string message);
    Task OnStationConnection(bool usbStatus);
    #endregion





}