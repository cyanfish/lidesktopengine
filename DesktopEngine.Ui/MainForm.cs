using Eto.Forms;
using Eto.Drawing;

namespace DesktopEngine.Ui;

public class MainForm : Form
{
    private const int STATUS_UPDATE_INTERVAL = 1000;

    private static readonly Color COLOR_INACTIVE = Color.FromArgb(96, 96, 96);
    private static readonly Color COLOR_OK = Color.FromArgb(0, 160, 0);
    private static readonly Color COLOR_WARN = Color.FromArgb(200, 150, 0);
    private static readonly Color COLOR_ERROR = Color.FromArgb(160, 0, 0);

    private readonly Button _startButton = new() { Text = UiResources.Start };
    private readonly Button _setupButton = new() { Text = UiResources.Setup };
    private readonly ImageView _statusIndicator = new();
    private readonly Label _statusLabel = new();

    private readonly Timer _statusUpdateTimer;
    private Timer _demoTimer;
    private bool _isStarted;
    private bool _showRunningForDemo;

    public MainForm()
    {
        Title = UiResources.TitleText;
        MinimumSize = new Size(400, 130);

        Content = new StackLayout
        {
            Padding = 10,
            Items =
            {
                new Label
                {
                    Text = UiResources.WelcomeMessage
                },
                new StackLayout
                {
                    Padding = 10,
                    Spacing = 10,
                    Orientation = Orientation.Horizontal,
                    Items =
                    {
                        _setupButton,
                        _startButton
                    }
                },
                new StackLayout
                {
                    Padding = 10,
                    Spacing = 10,
                    Orientation = Orientation.Horizontal,
                    Items =
                    {
                        _statusIndicator,
                        _statusLabel
                    }
                }
            }
        };

        var setupCommand = new Command();
        setupCommand.Executed += (_, _) => EtoHelpers.ShowSingletonForm<SetupForm>();
        _setupButton.Command = setupCommand;

        var startCommand = new Command();
        startCommand.Executed += (_, _) =>
        {
            _demoTimer?.Dispose();
            if (_isStarted)
            {
                _showRunningForDemo = false;
                _isStarted = false;
                UpdateStatus();
            }
            else
            {
                _isStarted = true;
                UpdateStatus();
            }
        };
        _startButton.Command = startCommand;

        var aboutCommand = new Command { MenuText = UiResources.About };
        aboutCommand.Executed += (_, _) => new AboutDialog
        {
            // TODO: License, logo, and other metadata
            Website = new Uri("https://github.com/cyanfish/lidesktopengine"),
            WebsiteLabel = "Github"
        }.ShowDialog(this);

        Menu = new MenuBar
        {
            AboutItem = aboutCommand
        };
        Width = 400;

        UpdateStatus();
        _statusUpdateTimer = new Timer(_ =>
            Application.Instance.Invoke(UpdateStatus), null, 0, STATUS_UPDATE_INTERVAL);
        UserConfig.Saved += (_, _) =>
            Application.Instance.Invoke(UpdateStatus);
    }

    private void UpdateStatus()
    {
        var config = UserConfig.Load();

        _startButton.Text = _isStarted ? UiResources.Stop : UiResources.Start;
        _startButton.Enabled = _isStarted || config.IsValid;

        if (!config.IsValid && !_isStarted)
        {
            _statusLabel.Text = "";
            _statusIndicator.Visible = false;
            return;
        }

        if (_showRunningForDemo)
        {
            _statusLabel.Text = UiResources.WaitingForAnalysis;
            _statusIndicator.Visible = true;
            _statusIndicator.Image = DrawStatusBitmap(COLOR_OK);
        }
        else if (_isStarted)
        {
            _statusLabel.Text = UiResources.TryingToConnect;
            _statusIndicator.Visible = true;
            _statusIndicator.Image = DrawStatusBitmap(COLOR_WARN);
            // var status = ExternalEngineApi.GetStatus();
            // var statusText = status switch
            // {
            //     ExternalEngineApi.NOT_STARTED => "Press Start to connect",
            //     ExternalEngineApi.DISCONNECTED => "Trying to connect...",
            //     ExternalEngineApi.CONNECTED_IDLE => "Connected, waiting for analysis request",
            //     ExternalEngineApi.CONNECTED_RUNNING => "Connected, analysis in progress",
            //     ExternalEngineApi.ENGINE_ERROR => "Error with engine setup",
            //     ExternalEngineApi.UNKNOWN_ERROR => "Unknown error",
            //     _ => "Unknown status"
            // };
            // _statusLabel.Text = statusText;
            _demoTimer = new Timer(_ =>
            {
                _showRunningForDemo = true;
                Application.Instance.Invoke(UpdateStatus);
            }, null, 2000, 0);
        }
        else
        {
            _statusLabel.Text = "";
            _statusIndicator.Visible = false;
        }
    }

    private Bitmap DrawStatusBitmap(Color color)
    {
        var bitmap = new Bitmap(16, 16, PixelFormat.Format32bppRgba);
        using var graphics = new Graphics(bitmap);
        graphics.DrawEllipse(color, 0, 0, 15, 15);
        graphics.FillEllipse(color, 3, 3, 10, 10);
        return bitmap;
    }
}