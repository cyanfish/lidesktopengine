using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;

namespace DesktopEngine.Ui;

public class SetupForm : Form
{
    // TODO: Set the real scope needed
    private const string CREATE_TOKEN_URL =
        "https://lichess.org/account/oauth/token/create?description=Lichess%20Desktop%20Engine&scopes[]=preference:read";

    private readonly TextBox _machineName = new() { Width = 200 };
    private readonly TextBox _oauthToken = new() { Width = 200 };
    private readonly RadioButton _stockfish;
    private readonly RadioButton _customEngine;
    private readonly TextBox _customEnginePath = new() { Width = 200 };
    private readonly Button _browseButton = new()
    {
        // TODO: Verify this works ok for accessibility
        Text = "...",
        ToolTip = UiResources.Browse,
        Width = 30
    };
    private readonly NumericMaskedTextBox<int> _maxHash = new();
    private readonly NumericMaskedTextBox<int> _maxThreads = new();
    private readonly CheckBox _runOnStartup = new() { Text = UiResources.RunOnStartup };

    private readonly string _id;

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
        _browseButton.Command = pickEngine;

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
                        _browseButton
                    }
                },
                Spacer(),
                UiResources.MaxHash,
                _maxHash,
                Spacer(),
                UiResources.MaxThreads,
                _maxThreads,
                Spacer(),
                _runOnStartup
            }
        };

        var config = UserConfigManager.Load();
        _id = config.Id;
        _machineName.Text = config.MachineName;
        _oauthToken.Text = config.AuthToken;
        _stockfish.Checked = !config.UseCustomEngine;
        _customEngine.Checked = config.UseCustomEngine;
        _customEnginePath.Text = config.CustomEnginePath;
        _maxHash.Text = config.MaxHash.ToString();
        _maxThreads.Text = config.MaxThreads.ToString();
        _runOnStartup.Checked = OsServiceManager.Instance?.IsRegistered ?? false;
        
        UpdateEnabled();
        _stockfish.CheckedChanged += (_, _) => UpdateEnabled();

        _machineName.TextChanged += SaveChanges;
        _oauthToken.TextChanged += SaveChanges;
        _stockfish.CheckedChanged += SaveChanges;
        _customEnginePath.TextChanged += SaveChanges;
        _maxHash.TextChanged += SaveChanges;
        _maxThreads.TextChanged += SaveChanges;

        _runOnStartup.Visible = OsServiceManager.Instance != null;
        _runOnStartup.CheckedChanged += ToggleStartupServiceRegistration;
    }

    private void ToggleStartupServiceRegistration(object o, EventArgs eventArgs)
    {
        if (_runOnStartup.Checked ?? false)
        {
            try
            {
                OsServiceManager.Instance.Register();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex, "Error registering startup service");
            }
        }
        else
        {
            try
            {
                OsServiceManager.Instance.Unregister();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex, "Error unregistering startup service");
            }
        }
        SaveChanges(null, EventArgs.Empty);
    }

    private void SaveChanges(object sender, EventArgs args)
    {
        UserConfigManager.Save(new UserConfig
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
        _browseButton.Enabled = _customEngine.Checked;
    }

    private Label Spacer()
    {
        return new Label { Text = "", Height = 10 };
    }
}