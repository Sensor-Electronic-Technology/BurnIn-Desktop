﻿namespace BurnIn.Shared.Models;

public record StationSerialData {
    public List<double> Voltages { get; set; } = new List<double>();
    public List<double> Currents { get; set; } = new List<double>();
    public List<double> Temperatures { get; set; } = new List<double>();
    public List<long> ProbeRuntimes { get; set; } = new List<long>();
    public bool Heater1State { get; set; }
    public bool Heater2State { get; set; }
    public bool Heater3State { get; set; }
    public int CurrentSetPoint { get; set; }
    public int TemperatureSetPoint { get; set; }
    public long RuntimeSeconds { get; set; }
    public long ElapsedSeconds { get; set; }
    public bool Running { get; set; }
    public bool Paused { get; set; }
}