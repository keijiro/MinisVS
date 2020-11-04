using Ludiq;
using Minis;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("MIDI"), UnitTitle("MIDI Control")]
public sealed class MidiControlUnit : Unit, IGraphElementWithData
{
    #region Data class

    public sealed class Data : IGraphElementData
    {
        public MidiDevice Device { get; private set; }

        public bool CheckDevice(int channel)
        {
            if (Device != null && Device.channel == channel) return true;
            Device = DeviceQuery.FindChannel(channel);
            return Device != null;
        }
    }

    public IGraphElementData CreateData() => new Data();

    #endregion

    #region Unit I/O

    [DoNotSerialize]
    public ValueInput Channel { get; private set; }

    [DoNotSerialize]
    public ValueInput ControlNumber { get; private set; }

    [DoNotSerialize]
    public ValueOutput Value { get; private set; }

    #endregion

    #region Unit implementation

    protected override void Definition()
    {
        Channel = ValueInput<int>(nameof(Channel), 0);
        ControlNumber = ValueInput<int>(nameof(ControlNumber), 0);
        Value = ValueOutput<float>(nameof(Value), GetValue);
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
