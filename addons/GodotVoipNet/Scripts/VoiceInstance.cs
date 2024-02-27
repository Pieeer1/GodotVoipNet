using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace GodotVoipNet;
public partial class VoiceInstance : Node
{
    private AudioStreamPlayer _voice = null!;
    private VoiceMic _voiceMic = null!;
    private AudioEffectCapture _audioEffectCapture = null!;
    private AudioStreamGeneratorPlayback? _playback;
    private Vector2[] _receiveBuffer = [];
    private bool _previousFrameIsRecording = false;
    [Export]
    public bool IsStereo { get; set; } = false;
    [Export]
    public bool IsRecording { get; set; } = false;
    [Export]
    public bool ShouldListen { get; set; } = false;
    [Export]
    public double InputThreshold { get; set; } = 0.005f;

    public event EventHandler<VoiceDataEventArgs>? ReceivedVoiceData;
    public event EventHandler<VoiceDataEventArgs>? SentVoiceData;

    public override void _Process(double delta)
    {
        if (_playback is not null)
        {
            ProcessVoice();
        }
        ProcessMic();
    }

    private void CreateMic()
    { 
        _voiceMic = new VoiceMic();
        AddChild(_voiceMic);
        int recordBusIdx = AudioServer.GetBusIndex(_voiceMic.Bus);
        _audioEffectCapture = (AudioEffectCapture)AudioServer.GetBusEffect(recordBusIdx, 0);
    }

    private void CreateVoice(float mixRate)
    {
        _voice = new AudioStreamPlayer();
        AddChild(_voice);

        var generator = new AudioStreamGenerator();
        generator.BufferLength = 0.1f;
        generator.MixRate = mixRate;

        _voice.Stream = generator;
        _voice.Play();
        _playback = _voice.GetStreamPlayback() as AudioStreamGeneratorPlayback;
    }

    [Rpc(CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void Speak(Array<Vector2> data, int id, float mixRate)
    {
        if (_playback is null)
        {
            CreateVoice(mixRate);
        }

        ReceivedVoiceData?.Invoke(this, new VoiceDataEventArgs(data, id));
        _receiveBuffer = [.. _receiveBuffer, .. data];
    }

    private void ProcessVoice()
    {
        int framesAvailable = _playback?.GetFramesAvailable() ?? 0;
        if (framesAvailable < 1) { return; }

        _playback?.PushBuffer(_receiveBuffer);
        _receiveBuffer = [];
    }

    private void ProcessMic()
    {
        if (IsRecording)
        {
            if (_audioEffectCapture is null)
            {
                CreateMic();
            }
            if(!_previousFrameIsRecording)
            {
                _audioEffectCapture?.ClearBuffer();
            }
            Vector2[] stereoData = _audioEffectCapture?.GetBuffer(_audioEffectCapture.GetFramesAvailable()) ?? [];
            if (stereoData.Any())
            {
                var data = new Array<Vector2>();
                data.Resize(stereoData.Length);

                float maxValue = 0.0f;

                for (int i = 0; i < stereoData.Length; i++)
                {
                    float value = (stereoData[i].X + stereoData[i].Y) / 2.0f;
                    maxValue = Math.Max(value, maxValue);
                    data[i] = IsStereo ? stereoData[i] : new Vector2(value, value);
                }
                if (maxValue < InputThreshold)
                {
                    return;
                }
                if (ShouldListen)
                {
                    Speak(data, Multiplayer.GetUniqueId(), AudioServer.GetMixRate());
                }
                Rpc(nameof(Speak), [data, Multiplayer.GetUniqueId(), AudioServer.GetMixRate()]);
                SentVoiceData?.Invoke(this, new VoiceDataEventArgs(data, Multiplayer.GetUniqueId()));
            }
        }
        _previousFrameIsRecording = IsRecording;
    }
}
