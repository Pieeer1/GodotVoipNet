#if TOOLS
using Godot;
using System;

[Tool]
public partial class plugin : EditorPlugin
{
	public override void _EnterTree()
	{
		AddCustomType("VoiceInstance", "Node", GD.Load<Script>("res://addons/GodotVoipNet/Scripts/VoiceInstance.cs"), GD.Load<Texture2D>("res://addons/GodotVoipNet/icons/VoiceInstance.svg"));
		AddCustomType("VoiceOrchestrator", "Node", GD.Load<Script>("res://addons/GodotVoipNet/Scripts/VoiceOrchestrator.cs"), GD.Load<Texture2D>("res://addons/GodotVoipNet/icons/VoiceOrchestrator.svg"));
    }

    public override void _ExitTree()
	{
		RemoveCustomType("VoiceInstance");
		RemoveCustomType("VoiceOrchestrator");
    }
}
#endif
