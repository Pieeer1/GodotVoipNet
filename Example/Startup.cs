using Godot;
using System;
using System.Linq;

public partial class Startup : Node
{
    [Export]
    public int MaxPlayers { get; set; } = 4;
    [Export]
    public int ServerPort { get; set; } = 1234;

    private readonly PackedScene _speakerScene = ResourceLoader.Load<PackedScene>("res://Example/Scenes/Speaker.tscn");
    private readonly PackedScene _vector3DisplayScene = ResourceLoader.Load<PackedScene>("res://Example/Scenes/UI/Vector3Display.tscn");

    private Button _hostButton = null!;
    private Button _joinButton = null!;
    private Button _leaveButton = null!;
    private Button _quitButton = null!;
    private Node _speakerHolder = null!;
    private CheckBox _pushToTalkCheckbox = null!;
    private CheckBox _stereoCheckbox = null!;
    private CheckBox _positionalAudioCheckbox = null!;
    private Label _tooltipLabel = null!;
    private VBoxContainer _speakerUIHolder = null!;
    private bool _isInSession = false;
    private bool _isStereo = false;
    private bool _isPushToTalk = false;
    private int _id = 0;
    public override void _Ready()
    {
        _hostButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/HostButton");
        _joinButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/JoinButton");
        _leaveButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/LeaveButton");
        _quitButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/QuitButton");
        _speakerHolder = GetNode<Node3D>("SpeakerHolder");
        _pushToTalkCheckbox = GetNode<CheckBox>("Control/VBoxContainer/CheckboxContainer/PushToTalkCheckbox");
        _stereoCheckbox = GetNode<CheckBox>("Control/VBoxContainer/CheckboxContainer/StereoCheckbox");
        _positionalAudioCheckbox = GetNode<CheckBox>("Control/VBoxContainer/CheckboxContainer/PositionalAudioCheckbox");
        _tooltipLabel = GetNode<Label>("Control/VBoxContainer/TooltipLabel");
        _speakerUIHolder = GetNode<VBoxContainer>("Control/VBoxContainer/SpeakerContainer/SpeakerHolder");

        _hostButton.Pressed += HostButtonPressed;
        _joinButton.Pressed += JoinButtonPressed;
        _leaveButton.Pressed += LeaveButtonPressed;
        _quitButton.Pressed += QuitButtonPressed;

        _stereoCheckbox.Toggled += (bool isPressed) => _isStereo = isPressed;
        _pushToTalkCheckbox.Toggled += (bool isPressed) =>
        {
            _isPushToTalk = isPressed;
            if (!_isPushToTalk)
            {
                _speakerHolder.GetChildren().Cast<Speaker>().First(x => x.Name == $"{_id}").StartSpeaking();
            }
            else
            {
                _speakerHolder.GetChildren().Cast<Speaker>().FirstOrDefault(x => x.Name == $"{_id}")?.StopSpeaking();
            }
        };
        _positionalAudioCheckbox.Toggled += (bool isPressed) =>
        {
            _speakerHolder.GetChildren().Cast<Speaker>().First(x => x.Name == $"{_id}").SetIsPositional(isPressed);
            foreach (var vector3Display in _speakerUIHolder.GetChildren().SelectMany(x => x.GetChildren()).Where(x => x is Vector3Display).Cast<Vector3Display>())
            {
                vector3Display.IsEnabled = isPressed;
            }
        };
        //set defaults programatically for now
        _pushToTalkCheckbox.ButtonPressed = true;
        _isPushToTalk = true;
    }

    public override void _Process(double delta)
    {
        HandleButtonAvailability();
        if (_isPushToTalk)
        {
            if (Input.IsActionPressed("voice_input"))
            {
                _speakerHolder.GetChildren().Cast<Speaker>().FirstOrDefault(x => x.Name == $"{_id}")?.StartSpeaking();
            }
            else
            {
                _speakerHolder.GetChildren().Cast<Speaker>().FirstOrDefault(x => x.Name == $"{_id}")?.StopSpeaking();
            }
        }
    }

    [Rpc(CallLocal = true)]
    public void RefreshSpeakerList(int id)
    {
        ClearSpeakerList();

        foreach (Node speaker in _speakerHolder.GetChildren().Where(x => x.Name != id.ToString()))
        {
            CreateNewLabels(speaker);
        }
    }
    [Rpc(CallLocal = true)]
    public void RefreshSpeakerList()
    {
        ClearSpeakerList();

        foreach (Node speaker in _speakerHolder.GetChildren())
        {
            CreateNewLabels(speaker);
        }
    }


    private void CreateNewLabels(Node speaker)
    {
        HBoxContainer hBoxContainer = new HBoxContainer();
        Label label = new Label();
        label.Text = $"Speaker {speaker.Name}";
        hBoxContainer.AddChild(label);

        if (_id == int.Parse(speaker.Name))
        {
            Vector3Display locationSetup = _vector3DisplayScene.Instantiate<Vector3Display>();
            locationSetup.PlayerId = int.Parse(speaker.Name);
            locationSetup.PlayerHolder = _speakerHolder;
            hBoxContainer.AddChild(locationSetup);
            _speakerUIHolder.AddChild(hBoxContainer);
            locationSetup.UpdateChatLabel("Location: ");
        }
        else
        {
            _speakerUIHolder.AddChild(hBoxContainer);
        }

    }

    [Rpc]
    public void SetId(int id)
    {
        if (_id == 0)
        {
            _id = id;
        }
    }
    private void HandleButtonAvailability()
    {
        _leaveButton.Disabled = !_isInSession;
        _joinButton.Disabled = _isInSession;
        _hostButton.Disabled = _isInSession;
        _pushToTalkCheckbox.Disabled = !_isInSession;
        _stereoCheckbox.Disabled = !_isInSession;
        _tooltipLabel.Visible = _isInSession && _isPushToTalk;
    }

    private void QuitButtonPressed()
    {
        GetTree().Quit();
    }
    private void ClearSpeakerList()
    {
        foreach (var child in _speakerUIHolder.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void LeaveButtonPressed()
    {
        _isInSession = false;

        ClearSpeakerList();

        if (Multiplayer.IsServer())
        {
            RemovePlayer(1);
            Multiplayer.PeerConnected -= AddPlayer;
            Multiplayer.PeerDisconnected -= RemovePlayer;
        }

        Multiplayer.MultiplayerPeer.Close();
    }

    private void JoinButtonPressed()
    {
        _isInSession = true;

        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        peer.CreateClient("127.0.0.1", ServerPort);
        Multiplayer.MultiplayerPeer = peer;
    }

    private void HostButtonPressed()
    {
        _isInSession = true;

        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        peer.CreateServer(ServerPort, MaxPlayers);
        Multiplayer.MultiplayerPeer = peer;

        Multiplayer.PeerConnected += AddPlayer;
        Multiplayer.PeerDisconnected += RemovePlayer;
        _id = 1;
        AddPlayer(1); // multiplayer always starts with id of 1
    }

    private void AddPlayer(long id)
    {
        Speaker speaker = _speakerScene.Instantiate<Speaker>();
        speaker.Name = $"{id}";
        speaker.Position = Vector3.Zero;
        _speakerHolder.AddChild(speaker, true);
        Rpc(nameof(SetId), id);
        Rpc(nameof(RefreshSpeakerList));
    }
    private void RemovePlayer(long id) 
    {
        _speakerHolder.GetNode<Speaker>(id.ToString()).QueueFree();
        Rpc(nameof(RefreshSpeakerList), id);
    }
}
