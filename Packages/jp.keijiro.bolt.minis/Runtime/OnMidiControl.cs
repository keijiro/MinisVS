using Ludiq;
using Minis;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("Events/Input"), UnitTitle("On MIDI Control")]
public sealed class OnMidiControl : MachineEventUnit<EmptyEventArgs>
{
    public new class Data : EventUnit<EmptyEventArgs>.Data
    {
        public float state;
    }

    public override IGraphElementData CreateData() => new Data();

    protected override string hookName => EventHooks.Update;

    [DoNotSerialize]
    public ValueInput controlNumber { get; private set; }

    [DoNotSerialize, PortLabel("Value")]
    public ValueOutput controlValue { get; private set; }

    protected override void Definition()
    {
        base.Definition();
        controlNumber = ValueInput<int>(nameof(controlNumber), 0);
        controlValue = ValueOutput<float>(nameof(controlValue), Operation);
        Requirement(controlNumber, controlValue);
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

        var number = flow.GetValue<int>(controlNumber);
        var current = device.GetControl(number).ReadValue();

        var trigger = data.state != current;
        data.state = current;

        return trigger;
    }

    private float Operation(Flow flow)
    {
        var device = MidiDevice.current;
        if (device == null) return 0;
        var number = flow.GetValue<int>(controlNumber);
        return device.GetControl(number).ReadValue();
    }
}

} // Bolt.Addons.Minis
