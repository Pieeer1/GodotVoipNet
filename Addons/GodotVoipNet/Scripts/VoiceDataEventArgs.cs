using Godot.Collections;
using System;

namespace GodotVoipNet;
public class VoiceDataEventArgs : EventArgs
{
    public Array<float> Data { get; }
    public int Id { get; }

    public VoiceDataEventArgs(Array<float> data, int id)
    {
        Data = data;
        Id = id;
    }
}
