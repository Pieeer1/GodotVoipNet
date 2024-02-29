namespace GodotVoipNet.Tests;
[TestSuite]
public class VoiceOrchestratorTests
{
    [TestCase]
    public void TestVoiceOrchestratorInstantiation()
    {
        int instancesCreated = 0;
        VoiceOrchestrator voiceOrchestrator = new VoiceOrchestrator();
        AddNode(voiceOrchestrator);
        voiceOrchestrator.CreatedInstance += (s, e) => instancesCreated++;
        voiceOrchestrator.Multiplayer.MultiplayerPeer = new TestMultiplayerPeer();
        voiceOrchestrator._Process(0.1d);

        AssertInt(instancesCreated).Equals(1); // self creation
    }
    [TestCase]
    public void TestVoiceOrchestratorUserJoined()
    {
        int instancesCreated = 0;
        VoiceOrchestrator voiceOrchestrator = new VoiceOrchestrator();
        AddNode(voiceOrchestrator);
        voiceOrchestrator.CreatedInstance += (s, e) => instancesCreated++;
        voiceOrchestrator.Multiplayer.MultiplayerPeer = new TestMultiplayerPeer();
        voiceOrchestrator._Process(0.1d);
        voiceOrchestrator.Multiplayer.MultiplayerPeer.EmitSignal(TestMultiplayerPeer.PeerConnectedSignal, 2);
        AssertInt(instancesCreated).Equals(2); // self creation
    }
}
internal partial class TestMultiplayerPeer : MultiplayerPeerExtension
{
    public static string PeerConnectedSignal = SignalName.PeerConnected;
} // dummy class to just instantiate the values