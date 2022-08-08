using System.ComponentModel;
using Eto.Forms;
using Eto.Drawing;

namespace DesktopEngine.Ui;

public class MainForm : Form
{
    private const int STATUS_UPDATE_INTERVAL = 1000;

    private static readonly Bitmap STATUS_INACTIVE =  DrawStatusBitmap(Color.FromArgb(96, 96, 96));
    private static readonly Bitmap STATUS_OK = DrawStatusBitmap(Color.FromArgb(0, 160, 0));
    private static readonly Bitmap STATUS_WARN = DrawStatusBitmap(Color.FromArgb(200, 150, 0));
    private static readonly Bitmap STATUS_ERROR = DrawStatusBitmap(Color.FromArgb(160, 0, 0));

    private readonly Button _startButton = new() { Text = UiResources.Start, Enabled = false };
    private readonly Button _setupButton = new() { Text = UiResources.Setup };
    private readonly ImageView _statusIndicator = new() { Width = 16, Height = 16 };
    private readonly Label _statusLabel = new();

    private readonly Timer _statusUpdateTimer;
    private int _currentStatus;
    private bool _isStartedFromThisInstance;

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
            if (_currentStatus == -1)
            {
                // Start
                DaemonService.StartServiceProcess();
                UpdateStatus();
                _isStartedFromThisInstance = true;
            }
            else
            {
                // Stop
                DaemonService.SendKillMessage();
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

        _statusUpdateTimer = new Timer(_ => UpdateStatus(), null, 0, STATUS_UPDATE_INTERVAL);
        UserConfig.Saved += (_, _) => UpdateStatus();
        
        Closing += OnClosing;
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        if (_currentStatus == -1 || !_isStartedFromThisInstance)
        {
            return;
        }
        switch (MessageBox.Show(this, UiResources.KeepRunningOnClose, MessageBoxButtons.YesNoCancel,
                MessageBoxType.Question))
        {
            case DialogResult.Yes:
                break;
            case DialogResult.No:
                DaemonService.SendKillMessage();
                break;
            default:
                e.Cancel = true;
                break;
        }
    }

    private void UpdateStatus()
    {
        _currentStatus = DaemonService.SendGetStatusMessage();
        if (_currentStatus == -1)
        {
            _isStartedFromThisInstance = false;
        }
        Application.Instance.Invoke(UpdateStatusUi);
    }

    private void UpdateStatusUi()
    {
        var config = UserConfig.Load();

        bool isRunning = _currentStatus != -1;
        _startButton.Text = isRunning ? UiResources.Stop : UiResources.Start;
        _startButton.Enabled = isRunning || config.IsValid;

        if (!config.IsValid && !isRunning)
        {
            _statusLabel.Text = "";
            _statusIndicator.Visible = false;
            return;
        }

        if (_currentStatus == ExternalEngineApi.CONNECTED_IDLE)
        {
            _statusLabel.Text = UiResources.WaitingForAnalysis;
            _statusIndicator.Visible = true;
            _statusIndicator.Image = STATUS_OK;
        }
        else if (_currentStatus == ExternalEngineApi.DISCONNECTED)
        {
            _statusLabel.Text = UiResources.TryingToConnect;
            _statusIndicator.Visible = true;
            _statusIndicator.Image = STATUS_WARN;
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
        }
        else if (_currentStatus == -1)
        {
            _statusLabel.Text = "";
            _statusIndicator.Visible = false;
        }
    }

    private static Bitmap DrawStatusBitmap(Color color)
    {
        const int s = 32; // Rendered size
        const int d = 6; // Pixels between outer and inner circle
        const int w = 2; // Width of outer circle
        var bitmap = new Bitmap(s, s, PixelFormat.Format32bppRgba);
        using var graphics = new Graphics(bitmap);
        graphics.DrawEllipse(new Pen(color, w), w - 1, w - 1, s - w * 2 + 1, s - w * 2 + 1);
        graphics.FillEllipse(color, d, d, s - 2 * d, s - 2 * d);
        return bitmap;
    }
}