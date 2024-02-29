namespace GodotVoipNet.Tests;
[TestSuite]
public class VoiceMicTests
{

    [TestCase]
    public void TestVoiceMicInstantiation()
    {
        VoiceMic voiceMic = new VoiceMic();
        voiceMic._Ready();

        AssertThat(AudioServer.BusCount).Equals(2); // main bus and newly created bus
        AssertThat(AudioServer.GetBusName(1) == "VoiceMicRecorder_1");
        AssertThat(AudioServer.GetBusEffect(1, 0) != null).IsTrue();
    }
}
