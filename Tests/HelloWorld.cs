using GdUnit4;
using static GdUnit4.Assertions;

namespace GodotVoipNet.Tests;

[TestSuite]
public class HelloWorld
{
    [TestCase]
    public void Test()
    {
        AssertBool(true).IsTrue();
    }
}
