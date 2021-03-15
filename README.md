MinisVS
=======

![gif](https://i.imgur.com/p6xh9FO.gif)
![gif](https://i.imgur.com/xdNSsu8.gif)

**MinisVS** is an extension for visual scripting in Unity that adds custom
units for handling MIDI input.

It uses [Minis] as a backend. If you're interested in using MIDI without visual
scripting, check the [Minis] project.

[Minis]: https://github.com/keijiro/Minis

Custom Units
------------

### MIDI Note unit

![note unit](https://i.imgur.com/kGOaAoY.png)

**MIDI Note** is a unit for handling MIDI note input. You can specify a MIDI
channel and a MIDI note number to which the unit listens.

You can use the *Note On/Off* ports to hook up a flow to MIDI note events.

The *Is-Pressed* port outputs true while the specified note is on.

The *Velocity* port outputs the velocity value on the last note.

### MIDI Control unit

![control unit](https://i.imgur.com/QuqiPWG.png)

**MIDI Control** is a unit for handling MIDI control input (CC). You can specify
a MIDI channel and a MIDI control number to which the unit listens.

You can use the *Changed* port to hook up a flow to MIDI control-change events.

The *Value* port outputs the current value of the control element.

How to install
--------------

This package uses the [scoped registry] feature to resolve package
dependencies. Please add the following sections to the manifest file
(Packages/manifest.json).

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.minis.visualscripting": "1.0.5"
```

After changes, the manifest file should look like below:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.minis.visualscripting": "1.0.5",
    ...
```
