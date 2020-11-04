using Ludiq;
using Minis;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("Events/Input"), UnitTitle("On MIDI Note")]
public sealed class OnMidiNote : MachineEventUnit<EmptyEventArgs>
{
    public new class Data : EventUnit<EmptyEventArgs>.Data
    {
        public float state;
    }

    public override IGraphElementData CreateData() => new Data();

    protected override string hookName => EventHooks.Update;

    [DoNotSerialize]
    public ValueInput noteNumber { get; private set; }

    [DoNotSerialize]
    public ValueInput action { get; private set; }

    [DoNotSerialize]
    public ValueOutput velocity { get; private set; }

    protected override void Definition()
    {
        base.Definition();
        noteNumber = ValueInput<int>(nameof(noteNumber), 0);
        action = ValueInput(nameof(action), PressState.Down);
        velocity = ValueOutput<float>(nameof(velocity), Operation);
        Requirement(noteNumber, velocity);
    }

    public override void StartListening(GraphStack stack)
    {
        base.StartListening(stack);
        stack.GetElementData<Data>(this).state = 0;
    }

    protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
    {
        var device = MidiDevice.current;
        if (device == null) return false;

        var data = flow.stack.GetElementData<Data>(this);
        if (!data.isListening) return false;

        var number = flow.GetValue<int>(noteNumber);
        var current = device.GetNote(number).velocity;

        var action = flow.GetValue<PressState>(this.action);
        var trigger = CheckTrigger(action, data.state, current);

        data.state = current;

        return trigger;
    }

    static bool CheckTrigger(PressState action, float prev, float current)
    {
        switch (action)
        {
            case PressState.Down: return prev == 0 && current > 0;
            case PressState.Up: return current == 0 && prev > 0;
            case PressState.Hold: return current > 0;
        }
        return false;
    }

    private float Operation(Flow flow)
    {
        var device = MidiDevice.current;
        if (device == null) return 0;
        var number = flow.GetValue<int>(noteNumber);
        return device.GetNote(number).velocity;
    }
}

} // Bolt.Addons.Minis
