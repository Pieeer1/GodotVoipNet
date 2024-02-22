using Godot;
using System;

public partial class Speaker : Node3D
{
    public override void _EnterTree()
    {
        SetMultiplayerAuthority(int.Parse(Name));
    }
}
