namespace DeviceSimulator
{
    using System;
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException(string deviceId) : base($"{deviceId} not found")
        {
        }
    }
}
