using System.Linq;
using System.Reflection;

namespace GodotVoipNet.Tests;
[TestSuite]
public class VoiceInstanceTests
{
    [TestCase]
    public void TestSpeakerAppendReceiveBuffer()
    {
        VoiceInstance voiceInstance = new VoiceInstance();
        voiceInstance.IsRecording = false;
        voiceInstance.Speak([Vector2.Zero, Vector2.Zero ], 0, Vector3.Zero);
        AssertThat(GetPrivateField<Vector2[]>(voiceInstance, "_receiveBuffer")!.Count() == 2).IsTrue();
        voiceInstance._Process(0.1f);
        AssertThat(GetPrivateField<Vector2[]>(voiceInstance, "_receiveBuffer")!.Count() == 0).IsFalse();
    }
    [TestCase]
    public void TestRecordingFrames()
    {
        Node3D parent = new Node3D();
        VoiceInstance voiceInstance = new VoiceInstance();
        parent.AddChild(voiceInstance);
        AddNode(parent);
        voiceInstance.IsRecording = true;
        voiceInstance._Process(0.5d);
        VoiceMic? voiceMic = GetPrivateField<VoiceMic>(voiceInstance, "_voiceMic");
        AssertThat(voiceMic is not null).IsTrue();
        AssertThat(GetPrivateField<bool>(voiceInstance, "_previousFrameIsRecording")).IsTrue();
        //unfortunately it is difficult to test the actual recording of the mic
        //leaving this here as a reminder to come back to it if I can manually add bytes to the audio buffer
    }

    private T? GetPrivateField<T>(object obj, string fieldName)
    {
        return (T?)(typeof(VoiceInstance).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(obj) ?? default(T));
    }
}
