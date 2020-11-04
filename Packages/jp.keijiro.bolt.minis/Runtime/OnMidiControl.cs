using Ludiq;
using Minis;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("Events/Input"), UnitTitle("On MIDI Control")]
public sealed class OnMidiControl : MachineEventUnit<EmptyEventArgs>
{
    #region Data class

    public new sealed class Data : EventUnit<EmptyEventArgs>.Data
    {
        public float Value { get; set; }

        public MidiDevice Device { get; private set; }

        public bool Update(float newValue)
        {
            if (newValue == Value) return false;
            Value = newValue;
            return true;
        }

        public bool CheckDevice(int channel)
        {
            if (Device != null && Device.channel == channel) return true;
            Device = DeviceQuery.FindChannel(channel);
            return Device != null;
        }
    }

    public override IGraphElementData CreateData() => new Data();

    #endregion

    #region Unit I/O

    [DoNotSerialize]
    public ValueInput Channel { get; private set; }

    [DoNotSerialize]
    public ValueInput ControlNumber { get; private set; }

    [DoNotSerialize, PortLabel("Value")]
    public ValueOutput Value { get; private set; }

    #endregion

    #region Event unit implementation

    protected override string hookName => EventHooks.Update;

    protected override void Definition()
    {
        base.Definition();
        Channel = ValueInput<int>(nameof(Channel), 0);
        ControlNumber = ValueInput<int>(nameof(ControlNumber), 0);
        Value = ValueOutput<float>(nameof(Value), GetValue);
    }

    protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
    {
        var data = flow.stack.GetElementData<Data>(this);
        if (!data.isListening) return false;
        return data.Update(GetValue(flow));
    }

    #endregion

    #region Private method

    float GetValue(Flow flow)
    {
        var data = flow.stack.GetElementData<Data>(this);
        var vChannel = flow.GetValue<int>(Channel);
        if (!data.CheckDevice(vChannel)) return 0;
        var vControlNumber = flow.GetValue<int>(ControlNumber);
        return data.Device.GetControl(vControlNumber).ReadValue();
    }

    #endregion
}

} // Bolt.Addons.Minis
