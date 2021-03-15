using Minis;
using Unity.VisualScripting;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("MIDI"), UnitTitle("MIDI Control")]
public sealed class MidiControlUnit
  : Unit, IGraphElementWithData, IGraphEventListener
{
    #region Data class

    public sealed class Data : IGraphElementData
    {
        public MidiDevice Device { get; private set; }
        public System.Action<EmptyEventArgs> UpdateAction { get; set; }
        public float Value { get; set; }

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

    [DoNotSerialize, PortLabel("CC#")]
    public ValueInput ControlNumber { get; private set; }

    [DoNotSerialize]
    public ControlOutput Changed { get; private set; }

    [DoNotSerialize]
    public ValueOutput Value { get; private set; }

    #endregion

    #region Unit implementation

    protected override void Definition()
    {
        isControlRoot = true;
        Channel = ValueInput<int>(nameof(Channel), 1);
        ControlNumber = ValueInput<int>(nameof(ControlNumber), 0);
		Changed = ControlOutput(nameof(Changed));
        Value = ValueOutput<float>(nameof(Value), GetValue);
    }

    float GetValue(Flow flow)
      => flow.stack.GetElementData<Data>(this).Value;

    #endregion

    #region Graph event listener

    public void StartListening(GraphStack stack)
    {
        var data = stack.GetElementData<Data>(this);
        if (data.UpdateAction != null) return;

        var reference = stack.ToReference();
        data.UpdateAction = args => OnUpdate(reference);

        var hook = new EventHook(EventHooks.Update, stack.machine);
        EventBus.Register(hook, data.UpdateAction);
    }

    public void StopListening(GraphStack stack)
    {
        var data = stack.GetElementData<Data>(this);
        if (data.UpdateAction == null) return;

        var hook = new EventHook(EventHooks.Update, stack.machine);
        EventBus.Unregister(hook, data.UpdateAction);

        data.UpdateAction = null;
    }

    public bool IsListening(GraphPointer pointer)
      => pointer.GetElementData<Data>(this).UpdateAction != null;

    #endregion

    #region Update hook

    void OnUpdate(GraphReference reference)
    {
        using (var flow = Flow.New(reference))
        {
            var data = flow.stack.GetElementData<Data>(this);

            var vChannel = flow.GetValue<int>(Channel) - 1;
            if (!data.CheckDevice(vChannel)) return;

            var prev = data.Value;

            var vControlNumber = flow.GetValue<int>(ControlNumber);
            data.Value = data.Device.GetControl(vControlNumber).ReadValue();

            if (prev != data.Value) flow.Invoke(Changed);
        }
    }

    #endregion
}

} // Bolt.Addons.Minis
