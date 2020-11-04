using Minis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace Bolt.Addons.Minis {

static class DeviceQuery
{
    public static MidiDevice FindChannel(int channel)
    {
        // Matcher object with Minis devices
        var match = new InputDeviceMatcher().WithInterface("Minis");

        // Channel number specifier with a capability match
        match = match.WithCapability("channel", channel);

        // Scan all the devices found in the input system.
        foreach (var dev in InputSystem.devices)
            if (match.MatchPercentage(dev.description) > 0)
                return (MidiDevice)dev;

        return null;
    }
}

} // Bolt.Addons.Minis
