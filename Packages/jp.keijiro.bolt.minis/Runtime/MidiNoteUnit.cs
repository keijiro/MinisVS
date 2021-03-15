using Minis;
using Unity.VisualScripting;
using UnityEngine;

namespace Minis.VisualScripting {

[UnitCategory("MIDI"), UnitTitle("MIDI Note")]
[RenamedFrom("Bolt.Addons.Minis.MidiNoteUnit")]
public sealed class MidiNoteUnit
  : Unit, IGraphElementWithData, IGraphEventListener
{
    #region Data class

    public sealed class Data : IGraphElementData
    {
        public MidiDevice Device { get; private set; }
        public System.Action<EmptyEventArgs> UpdateAction { get; set; }
        public float State { get; set; }
        public float Velocity { get; set; }

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

    [DoNotSerialize, PortLabel("Note#")]
    public ValueInput NoteNumber { get; private set; }

    [DoNotSerialize]
    public ControlOutput NoteOn { get; private set; }

    [DoNotSerialize]
    public ControlOutput NoteOff { get; private set; }

    [DoNotSerialize]
    public ValueOutput IsPressed { get; private set; }

    [DoNotSerialize]
    public ValueOutput Velocity { get; private set; }

    #endregion

    #region Unit implementation

    protected override void Definition()
    {
        isControlRoot = true;
        Channel = ValueInput<int>(nameof(Channel), 1);
        NoteNumber = ValueInput<int>(nameof(NoteNumber), 0);
		NoteOn = ControlOutput(nameof(NoteOn));
		NoteOff = ControlOutput(nameof(NoteOff));
        IsPressed = ValueOutput<bool>(nameof(IsPressed), GetIsPressed);
        Velocity = ValueOutput<float>(nameof(Velocity), GetVelocity);
    }

    bool GetIsPressed(Flow flow)
      => flow.stack.GetElementData<Data>(this).State > 0;

    float GetVelocity(Flow flow)
      => flow.stack.GetElementData<Data>(this).Velocity;

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

            var prev = data.State;

            var vNoteNumber = flow.GetValue<int>(NoteNumber);
            data.State = data.Device.GetNote(vNoteNumber).velocity;
            if (data.State > 0) data.Velocity = data.State;

            if (prev == 0 && data.State > 0) flow.Invoke(NoteOn);
            if (prev > 0 && data.State == 0) flow.Invoke(NoteOff);
        }
    }

    #endregion
}

} // Minis.VisualScripting
