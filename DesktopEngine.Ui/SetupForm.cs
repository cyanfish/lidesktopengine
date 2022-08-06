using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;

namespace DesktopEngine.Ui;

public class SetupForm : Form
{
    // TODO: Set the real scope needed
    private const string CREATE_TOKEN_URL =
        "https://lichess.org/account/oauth/token/create?description=Lichess%20Desktop%20Engine&scopes[]=preference:read";

    private readonly string _id;
    private readonly TextBox _machineName = new() { Width = 200 };
    private readonly TextBox _oauthToken = new() { Width = 200 };
    private readonly RadioButton _stockfish;
    private readonly RadioButton _customEngine;
    private readonly TextBox _customEnginePath = new() { Width = 200 };
    private readonly NumericMaskedTextBox<int> _maxHash = new();
    private readonly NumericMaskedTextBox<int> _maxThreads = new();

    public SetupForm()
    {
        Title = UiResources.Setup;
        MinimumSize = new Size(200, 400);

        _stockfish = new() { Text = UiResources.Stockfish };
        _customEngine = new(_stockfish) { Text = UiResources.CustomEngine };

        var createToken = new Command();
        createToken.Executed += (_, _) =>
            Process.Start(new ProcessStartInfo(CREATE_TOKEN_URL) { UseShellExecute = true });

        var pickEngine = new Command();
        pickEngine.Executed += (_, _) =>
        {
            // TODO: Filters (esp. on windows)
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog(this) == DialogResult.Ok)
            {
                _customEnginePath.Text = ofd.FileName;
            }
        };

        Content = new StackLayout
        {
            Padding = 10,
            Items =
            {
                UiResources.MachineName,
                _machineName,
                Spacer(),
                UiResources.OauthToken,
                _oauthToken,
                new LinkButton
                {
                    Text = UiResources.CreateToken,
                    Command = createToken
                },
                Spacer(),
                _stockfish,
                Spacer(),
                _customEngine,
                Spacer(),
                UiResources.CustomEnginePath,
                new StackLayout()
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 5,
                    Items =
                    {
                        _customEnginePath,
                        new Button
                        {
                            // TODO: Verify this works ok for accessibility
                            Text = "...",
                            ToolTip = UiResources.Browse,
                            Command = pickEngine,
                            Width = 30
                        }
                    }
                },
                Spacer(),
                UiResources.MaxHash,
                _maxHash,
                Spacer(),
                UiResources.MaxThreads,
                _maxThreads
            }
        };

        var config = UserConfig.Load();
        _id = config.Id;
        _machineName.Text = config.MachineName;
        _oauthToken.Text = config.AuthToken;
        _stockfish.Checked = !config.UseCustomEngine;
        _customEngine.Checked = config.UseCustomEngine;
        _customEnginePath.Text = config.CustomEnginePath;
        _maxHash.Text = config.MaxHash.ToString();
        _maxThreads.Text = config.MaxThreads.ToString();

        UpdateEnabled();
        _stockfish.CheckedChanged += (_, _) => UpdateEnabled();

        _machineName.TextChanged += SaveChanges;
        _oauthToken.TextChanged += SaveChanges;
        _stockfish.CheckedChanged += SaveChanges;
        _customEnginePath.TextChanged += SaveChanges;
        _maxHash.TextChanged += SaveChanges;
        _maxThreads.TextChanged += SaveChanges;
    }

    private void SaveChanges(object sender, EventArgs args)
    {
        UserConfig.Save(new UserConfig
        {
            Id = _id,
            MachineName = _machineName.Text,
            MaxHash = _maxHash.Value,
            MaxThreads = _maxThreads.Value,
            AuthToken = _oauthToken.Text,
            UseCustomEngine = _customEngine.Checked,
            CustomEnginePath = _customEnginePath.Text
        });
    }

    private void UpdateEnabled()
    {
        _customEnginePath.Enabled = _customEngine.Checked;
    }

    private Label Spacer()
    {
        return new Label { Text = "", Height = 10 };
    }
}