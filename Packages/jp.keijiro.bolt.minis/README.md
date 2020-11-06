BoltMinis
=========

**BoltMinis** is an add-on for Unity's [Bolt visual scripting system] that adds
custom units for handling MIDI input.

[Bolt visual scripting system]:
  https://assetstore.unity.com/packages/tools/visual-scripting/bolt-163802

It uses [Minis] as a backend. If you're interested in using MIDI without visual
scripting, check the [Minis] project.

[Minis]: https://github.com/keijiro/Minis

Custom Units
------------

### MIDI Note unit

![note unit](https://i.imgur.com/VWshujo.png)

**MIDI Note** is a unit for handling MIDI note input. You can specify a MIDI
channel and a MIDI note number to which the unit listens.

You can use the *Note On/Off* ports to hook up a flow to MIDI note events.

The *Is-Pressed* port outputs true while the specified note is on.

The *Velocity* port outputs the velocity value on the last note.

### MIDI Control unit

![control unit](https://i.imgur.com/C9MVkn2.png)

**MIDI Control** is a unit for handling MIDI control input (CC). You can specify
a MIDI channel and a MIDI control number to which the unit listens.

You can use the *Changed* port to hook up a flow to MIDI control-change events.

The *Value* port outputs the current value of the control element.

How to try the sample project
-----------------------------

This repository doesn't contain the Bolt assets due to the license restriction.
You have to import [Bolt via Asset Store] manually.

[Bolt via Asset Store]:
  https://assetstore.unity.com/packages/tools/visual-scripting/bolt-163802

You can't use the "Install Bolt" tool due to compilation errors caused by
missing file references. You have to manually double-click the
`Bolt_1_4_X_NET4.unitypackage` file in the "Install Bolt" directory instead.

![unitypackage](https://i.imgur.com/cNxH458.png)

After importing the unitypackage file, it automatically opens the Bolt Setup
Wizard.

![wizard](https://i.imgur.com/wxlvRh7.png)

On the Assembly Options page, add `Bolt.Addons.Minis.Runtime` to the assembly
list.

![assembly options](https://i.imgur.com/R87yar0.png)

The sample project shows how to move a sphere using MIDI note input and MIDI
control (CC) input. You may want to change the note/CC numbers for your MIDI
device.

![sample graph](https://i.imgur.com/J3zuM5C.png)

How to install the add-on to an existing project
------------------------------------------------

### Installing the package via Package Manager

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
"jp.keijiro.bolt.minis": "1.0.0"
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
    "jp.keijiro.bolt.minis": "1.0.0",
    ...
```

### Enabling the new input system

When importing the package, the system may ask you to enable the new input
system.

![new input system](https://i.imgur.com/Oo0bkx7.png)

Press Yes to enable it.

### Adding the add-on assembly

Navigate to "Tools" > "Bolt" > "Unit Options Wizard".

![assembly options](https://i.imgur.com/R87yar0.png)

Add `Bolt.Addons.Minis.Runtime` to the assembly list. Then press "Next" and
"Generate."
