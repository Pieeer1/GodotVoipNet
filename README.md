# GodotVoipNet
GodotVoipNet is a .NET 8 overhaul of the Godot Voip Library for Godot 4+ 

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
7. (If there are issues, make sure you have a camera attached. See the example for a full minimal reproduceable sample)
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
| float      |  `IsStereo` |    If True, Audio will listen and send from the left and the right respectively       |

### VoiceOrchestrator 
| Type         | Name | Description |
|--------------|:-----:|-----------:|
| bool |  `IsRecording` |        If true, will transmit data to the other VoiceInstance instances. |
| bool      |  `ShouldListen` |          If true, will play microphone data to the instance recording |
| float      |  `InputThreshold` |    Value above which microhphone data will be sent. < 0 will disable       |
| float      |  `IsStereo` |    If True, Audio will listen and send from the left and the right respectively       |

# Contributing

If you would like to contribute, please feel free to submit a pull request.
There are some essential elements that are required for immediate running of the project.
1. The Location of you Godot Engine EXE (For the build and test process) must be at `C:\Program Files (x86)\Godot\Godot_v4.2.1-stable_mono_win64\Godot_v4.2.1-stable_mono_win64.exe`
2. If you are using a different location, you will need to update the following:
- Properties/launchSettings.json path
- .runsettings path (for tests)
3. The project is built using Visual Studio 2022. Visual Studio Code is not supported at this time.