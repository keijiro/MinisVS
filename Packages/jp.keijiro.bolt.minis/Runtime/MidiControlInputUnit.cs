using UnityEngine;
using Ludiq;
using Minis;

namespace Bolt.Addons.Minis {

[UnitCategory("MIDI")]
[UnitTitle("MIDI CC Input")]
public sealed class MidiControlInputUnit : Unit                                               
{                          
    [DoNotSerialize]
    public ValueInput controlNumber { get; private set; }

    [DoNotSerialize]
    [PortLabelHidden]
    public ValueOutput output { get; private set; }

    protected override void Definition()
    {
        controlNumber = ValueInput<int>(nameof(controlNumber), 0);
        output = ValueOutput<float>(nameof(output), Operation);
        Requirement(controlNumber, output);
    }

    private float Operation(Flow flow)
    {
        var dev = MidiDevice.current;
        if (dev == null) return 0;
        var num = flow.GetValue<int>(controlNumber);
        return dev.GetControl(num).ReadValue();
    }
}

} // Bolt.Addons.Minis
