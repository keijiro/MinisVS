using Ludiq;
using Minis;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("MIDI"), UnitTitle("MIDI Note")]
public sealed class MidiNoteUnit : Unit
{
    [DoNotSerialize]
    public ValueInput noteNumber { get; private set; }

    [DoNotSerialize]
    public ValueOutput velocity { get; private set; }

    protected override void Definition()
    {
        noteNumber = ValueInput<int>(nameof(noteNumber), 0);
        velocity = ValueOutput<float>(nameof(velocity), GetVelocity);
        Requirement(noteNumber, velocity);
    }

    private float GetVelocity(Flow flow)
    {
        var device = MidiDevice.current;
        if (device == null) return 0;
        var number = flow.GetValue<int>(noteNumber);
        return device.GetNote(number).velocity;
    }
}

} // Bolt.Addons.Minis
