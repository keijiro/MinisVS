using Ludiq;
using Minis;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("Events/Input"), UnitTitle("On MIDI Note")]
public sealed class OnMidiNote : MachineEventUnit<EmptyEventArgs>
{
    #region Data class

    public new sealed class Data : EventUnit<EmptyEventArgs>.Data
    {
        public float Velocity { get; set; }

        public MidiDevice Device { get; private set; }

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

        public bool Update(PressState action, float newVelocity)
        {
            var trigger = CheckTrigger(action, Velocity, newVelocity);
            Velocity = newVelocity;
            return trigger;
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
    public ValueInput NoteNumber { get; private set; }

    [DoNotSerialize]
    public ValueInput Action { get; private set; }

    [DoNotSerialize]
    public ValueOutput Velocity { get; private set; }

    #endregion

    #region Event unit implementation

    protected override string hookName => EventHooks.Update;

    protected override void Definition()
    {
        base.Definition();
        Channel = ValueInput<int>(nameof(Channel), 0);
        NoteNumber = ValueInput<int>(nameof(NoteNumber), 0);
        Action = ValueInput(nameof(Action), PressState.Down);
        Velocity = ValueOutput<float>(nameof(Velocity), GetVelocity);
    }

    protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
    {
        var data = flow.stack.GetElementData<Data>(this);
        if (!data.isListening) return false;
        var vAction = flow.GetValue<PressState>(Action);
        return data.Update(vAction, GetVelocity(flow));
    }

    #endregion

    #region Private method

    float GetVelocity(Flow flow)
    {
        var data = flow.stack.GetElementData<Data>(this);
        var vChannel = flow.GetValue<int>(Channel);
        if (!data.CheckDevice(vChannel)) return 0;
        var vNoteNumber = flow.GetValue<int>(NoteNumber);
        return data.Device.GetNote(vNoteNumber).velocity;
    }

    #endregion
}

} // Bolt.Addons.Minis
