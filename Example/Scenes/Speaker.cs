using Godot;
using GodotVoipNet;

public partial class Speaker : CharacterBody3D
{
    private VoiceInstance _voiceInstance = null!;
    public override void _EnterTree()
    {
        SetMultiplayerAuthority(int.Parse(Name));
    }
    public override void _Ready()
    {
        if (!IsMultiplayerAuthority()) { return; }
        _voiceInstance = GetNode<VoiceInstance>("VoiceInstance");
    }
    public void StartSpeaking()
    {
        _voiceInstance.IsRecording = true;
    }
    public void StopSpeaking()
    {
        _voiceInstance.IsRecording = false;
    }
}
