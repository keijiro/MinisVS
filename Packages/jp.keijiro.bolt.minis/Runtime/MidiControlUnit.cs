using Ludiq;
using Minis;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("MIDI"), UnitTitle("MIDI Control")]
public sealed class MidiControlUnit : Unit
{
    [DoNotSerialize]
    public ValueInput controlNumber { get; private set; }

    [DoNotSerialize, PortLabel("Value")]
    public ValueOutput controlValue { get; private set; }

    protected override void Definition()
    {
        controlNumber = ValueInput<int>(nameof(controlNumber), 0);
        controlValue = ValueOutput<float>(nameof(controlValue), GetControlValue);
        Requirement(controlNumber, controlValue);
    }

    private float GetControlValue(Flow flow)
    {
        var device = MidiDevice.current;
        if (device == null) return 0;
        var number = flow.GetValue<int>(controlNumber);
        return device.GetControl(number).ReadValue();
    }
}

} // Bolt.Addons.Minis
