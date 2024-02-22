using Godot;
using System;
using System.Linq;

public partial class Startup : Node3D
{
    [Export]
    public int MaxPlayers { get; set; } = 4;
    [Export]
    public int ServerPort { get; set; } = 1234;

    private readonly PackedScene _speakerScene = ResourceLoader.Load<PackedScene>("res://Example/Scenes/Speaker.tscn");

    private Button _hostButton = null!;
    private Button _joinButton = null!;
    private Button _leaveButton = null!;
    private Button _quitButton = null!;
    private Node _speakerHolder = null!;
    private CheckBox _pushToTalkCheckbox = null!;
    private CheckBox _stereoCheckbox = null!;
    private Label _tooltipLabel = null!;
    private VBoxContainer _speakerUIHolder = null!;
    private bool _isInSession = false;
    private bool _isStereo = false;
    private bool _isPushToTalk = false;
    public override void _Ready()
    {
        _hostButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/HostButton");
        _joinButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/JoinButton");
        _leaveButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/LeaveButton");
        _quitButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/QuitButton");
        _speakerHolder = GetNode<Node>("SpeakerHolder");
        _pushToTalkCheckbox = GetNode<CheckBox>("Control/VBoxContainer/CheckboxContainer/PushToTalkCheckbox");
        _stereoCheckbox = GetNode<CheckBox>("Control/VBoxContainer/CheckboxContainer/StereoCheckbox");
        _tooltipLabel = GetNode<Label>("Control/VBoxContainer/TooltipLabel");
        _speakerUIHolder = GetNode<VBoxContainer>("Control/VBoxContainer/SpeakerContainer/SpeakerHolder");

        _hostButton.Pressed += HostButtonPressed;
        _joinButton.Pressed += JoinButtonPressed;
        _leaveButton.Pressed += LeaveButtonPressed;
        _quitButton.Pressed += QuitButtonPressed;

        _stereoCheckbox.Toggled += (bool isPressed) => _isStereo = isPressed;
        _pushToTalkCheckbox.Toggled += (bool isPressed) => _isPushToTalk = isPressed;
    }

    public override void _Process(double delta)
    {
        HandleButtonAvailability();    
    }

    [Rpc(CallLocal = true)]
    public void RefreshSpeakerList(int id)
    {
        ClearSpeakerList();

        foreach (Node speaker in _speakerHolder.GetChildren().Where(x => x.Name != id.ToString()))
        {
            Label label = new Label();
            label.Text = $"Speaker {speaker.Name}";
            _speakerUIHolder.AddChild(label);
        }
    }
    [Rpc(CallLocal = true)]
    public void RefreshSpeakerList()
    {
        ClearSpeakerList();

        foreach (Node speaker in _speakerHolder.GetChildren())
        {
            Label label = new Label();
            label.Text = $"Speaker {speaker.Name}";
            _speakerUIHolder.AddChild(label);
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

        AddPlayer(1); // multiplayer always starts with id of 1
    }

    private void AddPlayer(long id)
    {
        Speaker speaker = _speakerScene.Instantiate<Speaker>();
        speaker.Name = $"{id}";
        _speakerHolder.AddChild(speaker, true);
        Rpc(nameof(RefreshSpeakerList));
    }
    private void RemovePlayer(long id) 
    {
        _speakerHolder.GetNode<Speaker>(id.ToString()).QueueFree();
        Rpc(nameof(RefreshSpeakerList), id);
    }
}
