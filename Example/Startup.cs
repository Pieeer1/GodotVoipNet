using Godot;
using System;

public partial class Startup : Node3D
{
    [Export]
    public int MaxPlayers { get; set; } = 4;
    [Export]
    public int ServerPort { get; set; } = 1234;

    private Button _hostButton = null!;
    private Button _joinButton = null!;
    private Button _leaveButton = null!;
    private Button _quitButton = null!;

    private bool _isInSession = false;

    public override void _Ready()
    {
        _hostButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/HostButton");
        _joinButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/JoinButton");
        _leaveButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/LeaveButton");
        _quitButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/QuitButton");

        _hostButton.Pressed += HostButtonPressed;
        _joinButton.Pressed += JoinButtonPressed;
        _leaveButton.Pressed += LeaveButtonPressed;
        _quitButton.Pressed += QuitButtonPressed;

    }

    public override void _Process(double delta)
    {
        HandleButtonAvailability();    
    }

    private void HandleButtonAvailability()
    {
        _leaveButton.Disabled = !_isInSession;
        _joinButton.Disabled = _isInSession;
        _hostButton.Disabled = _isInSession;
    }

    private void QuitButtonPressed()
    {
        GetTree().Quit();
    }

    private void LeaveButtonPressed()
    {
        _isInSession = false;

        Multiplayer.PeerConnected -= AddPlayer;
        Multiplayer.PeerDisconnected -= RemovePlayer;
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
    }

    private void AddPlayer(long id)
    {
        throw new NotImplementedException();
    }
    private void RemovePlayer(long id) 
    {
        throw new NotImplementedException();
    }
}
