#if TOOLS
using Godot;
using System;

[Tool]
public partial class Plugin : EditorPlugin
{
	public override void _EnterTree()
	{
		AddCustomType("VoiceInstance", "Node", GD.Load<Script>("res://Addons/GodotVoipNet/Scripts/VoiceInstance.cs"), GD.Load<Texture2D>("res://Addons/GodotVoipNet/Icons/VoiceInstance.svg"));
		AddCustomType("VoiceOrchestrator", "Node", GD.Load<Script>("res://Addons/GodotVoipNet/Scripts/VoiceOrchestrator.cs"), GD.Load<Texture2D>("res://Addons/GodotVoipNet/Icons/VoiceOrchestrator.svg"));
    }

    public override void _ExitTree()
	{
		RemoveCustomType("VoiceInstance");
		RemoveCustomType("VoiceOrchestrator");
    }
}
#endif
