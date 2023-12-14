# GodotVoipNet
GodotVoipNet is a C# overhaul of the Godot Voip Library for Godot 4+ 

# Engine Compatability

- Godot Engine 4+

# Setup

<h4>Add to Existing Project</h4> 

(Multiplayer Peer is Required)

1. Download the newest package and move it into the Godot Project.
2. Select the addons/GodotVoipNet/ folder and move it into your Godot project. (Note: make sure the structure is still res://addons/GodotVoipNet/)
3. Enable the Project Audio Settings and enable Input
4. Go to plugins and enable the plugin
5. Add a voice instance to your player scene OR a Voice Orchestrator to the scene. (Must be instantiated AFTER the Multiplayer setup is completed)
6. Get a reference to the instance and set `IsRecording = true`
Example: 
```csharp


	private VoiceInstance _voiceInstance = null!;

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) { return; }

		_voiceInstance = GetNode<VoiceInstance>("VoiceInstance");

		_voiceInstance.IsRecording = true;
	}
	
```

# Documentation

### VoiceInstance
| Type         | Name | Description |
|--------------|:-----:|-----------:|
| bool |  `IsRecording` |        If true, will transmit data to the other VoiceInstance instances. |
| bool      |  `ShouldListen` |          If true, will play microphone data to the instance recording |
| float      |  `InputThreshold` |    Value above which microhphone data will be sent. < 0 will disable       |

### VoiceOrchestrator 
| Type         | Name | Description |
|--------------|:-----:|-----------:|
| bool |  `IsRecording` |        If true, will transmit data to the other VoiceInstance instances. |
| bool      |  `ShouldListen` |          If true, will play microphone data to the instance recording |
| float      |  `InputThreshold` |    Value above which microhphone data will be sent. < 0 will disable       |
# Credit
Main Functionality is thanks to the algorithms created in this repository:
	https://github.com/ikbencasdoei/godot-voip
	
