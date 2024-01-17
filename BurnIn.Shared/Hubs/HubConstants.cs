﻿namespace BurnIn.Shared.Hubs;

public static class HubConstants {
    public static string HubAddress => "http://192.168.68.112:3000/hubs/station";

    public static class Events {
        public static string OnSerialCom => "OnSerialCom";
        public static string OnSerialComMessage => "OnSerialComMessage";
        public static string OnUsbConnect => "OnUsbConnect";
        public static string OnUsbDisconnect => "OnUsbDisconnect";
        public static string OnExecuteCommand => "OnExecuteCommand";
    }

    public static class Methods {
        public static string ConnectUsb => "ConnectUsb";
        public static string Disconnect => "ConnectUsb";
        public static string ExecuteCommand => "ExecuteCommand";
    }
}