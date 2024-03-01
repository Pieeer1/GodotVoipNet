using System.Linq;
using System.Text.RegularExpressions;

public partial class Vector3Display : Control
{
    public int PlayerId { get; set; }
    public Node PlayerHolder { get; set; } = null!;
    public bool IsEnabled { get; set; }

    private LineEdit _xEdit = null!;
    private LineEdit _yEdit = null!;
    private LineEdit _zEdit = null!;

    private Label _label = null!;
    public override void _Ready()
    {
        _xEdit = GetNode<LineEdit>("XEdit");
        _yEdit = GetNode<LineEdit>("YEdit");
        _zEdit = GetNode<LineEdit>("ZEdit");

        _label = GetNode<Label>("Label");


        _xEdit.Text = "0";
        _yEdit.Text = "0";
        _zEdit.Text = "0";

        Regex numbersOnly = new Regex(@"^[0-9\.]*$");
        Regex nonNumbers = new Regex(@"[^0-9]");

        Speaker speaker = PlayerHolder.GetChildren().Cast<Speaker>().First(x => x.Name == $"{PlayerId}");

        _xEdit.TextChanged += (string text) =>
        {
            if(!numbersOnly.IsMatch(text))
            {
                _xEdit.Text = nonNumbers.Replace(text, string.Empty);
            }
            if(text.Length > 0)
            {
                speaker.Position = new Vector3(float.Parse(_xEdit.Text), speaker.Position.Y, speaker.Position.Z);
            }
        };

        _yEdit.TextChanged += (string text) =>
        {
            if (!numbersOnly.IsMatch(text))
            {
                _yEdit.Text = nonNumbers.Replace(text, string.Empty);
            }
            if (text.Length > 0)
            {
                speaker.Position = new Vector3(speaker.Position.X, float.Parse(_yEdit.Text), speaker.Position.Z);
            }
        };

        _zEdit.TextChanged += (string text) =>
        {
            if (!numbersOnly.IsMatch(text))
            {
                _zEdit.Text = nonNumbers.Replace(text, string.Empty);
            }
            if (text.Length > 0)
            {
                speaker.Position = new Vector3(speaker.Position.X, speaker.Position.Y, float.Parse(_zEdit.Text));
            }
        };
    }
    public override void _Process(double delta)
    {
        _xEdit.Editable = IsEnabled;
        _yEdit.Editable = IsEnabled;
        _zEdit.Editable = IsEnabled;
    }
    public void UpdateChatLabel(string text)
    {
        _label.Text = text;
    }
}
