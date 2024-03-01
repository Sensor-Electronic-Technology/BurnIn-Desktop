﻿using BurnInControl.Data.ComponentConfiguration;
using MongoDB.Bson;
namespace BurnInControl.Data.StationModel;

public enum StationState {
    Running,Idle,Offline
}

public class Station {
    public ObjectId _Id { get; set; }
    public string StationId { get; set; }
    public string StationPosition { get; set; }
    public string? FirmwareVersion { get; set; }
    public bool UpdateAvailable { get; set; }
    public StationState State { get; set; }
    public ObjectId? RunningTest { get; set; }
    public BurnStationConfiguration? Configuration { get; set; }
}
