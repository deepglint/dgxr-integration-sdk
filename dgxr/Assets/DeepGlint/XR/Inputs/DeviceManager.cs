using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace DeepGlint.XR.Inputs
{
    public delegate void TooManyActiveDevicesDelegate();
    
    /// <summary>
    /// Manage all the Deepglint XR devices
    /// </summary>
    public static class DeviceManager
    {
        /// <summary>
        /// count of all the active Deepglint XR devices
        /// </summary>
        internal static int m_ActiveDeviceCount = 0;

        /// <summary>
        /// max count of all the active Deepglint XR devices
        /// </summary>
        public static int MaxActiveDeviceCount { get; set; }

        public static TooManyActiveDevicesDelegate OnTooManyActiveDevices;

        /// <summary>
        /// all the active Deepglint XR devices
        /// </summary>
        internal static ConcurrentDictionary<string, InputDevice> m_ActiveDevices =  new ConcurrentDictionary<string, InputDevice>();
        
        /// <summary>
        /// all the Deepglint XR devices 
        /// </summary>
        private static ConcurrentDictionary<string, InputDevice> _devices =  new ConcurrentDictionary<string, InputDevice>();
        
        /// <summary>
        /// all the active Deepglint XR devices
        /// </summary>
        public static ReadOnlyArray<InputDevice> AllActiveDevices => new ReadOnlyArray<InputDevice>(m_ActiveDevices.Values.ToArray(), 0, m_ActiveDeviceCount);

        /// <summary>
        /// Get an active device by the serial
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public static InputDevice GetActiveDeviceBySerial(string serial)
        {
            if (m_ActiveDevices.ContainsKey(serial))
            {
                return m_ActiveDevices[serial];
            }
            return null;
        }

        /// <summary>
        /// Add or active a Deepglint XR device with serial to the InputSystem
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="product"></param>
        public static InputDevice AddOrActiveDevice(string serial, string product)
        {
            var device = GetActiveDeviceBySerial(serial);
            if (device == null)
            {
                if (_devices.ContainsKey(serial))
                {
                    device = _devices[serial];
                    InputSystem.AddDevice(device); 
                }
                else
                {
                    device =  InputSystem.AddDevice(new InputDeviceDescription
                    {
                        serial = serial,
                        interfaceName = product, 
                        product = product,
                        manufacturer = "deepglint",
                    });
                    _devices[serial] = device; 
                }
                m_ActiveDevices[serial] = device;
                m_ActiveDeviceCount++;
                Debug.LogFormat("Device {0} which serial is {1} which type is {2} was created", device.deviceId, serial, product);
                if (m_ActiveDeviceCount >= MaxActiveDeviceCount)
                {
                    OnTooManyActiveDevices?.Invoke();
                }
            }

            return device;
        }

        /// <summary>
        /// Remove the device with serial from InputSystem
        /// </summary>
        /// <param name="serial"></param>
        public static void RemoveDevice(string serial)
        {
            var device = GetActiveDeviceBySerial(serial);
            if (device != null)
            {
                InputSystem.RemoveDevice(device);
                m_ActiveDeviceCount--;
                m_ActiveDevices.TryRemove(serial, out device);
                Debug.LogFormat("Device {0} which serial is {1} was removed", device.deviceId, serial);
            }
        }
    }
}