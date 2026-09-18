using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;

namespace MSCCargoYHavy;

public sealed class MainForm : Form
{
    private readonly WebView2 webView = new();
    private readonly Panel loadingOverlay = new();
    private readonly Label loadingTitle = new();
    private readonly Label loadingText = new();
    private readonly Label loadingDots = new();
    private readonly ProgressBar loadingBar = new();
    private readonly Panel errorPanel = new();
    private readonly Label errorTitle = new();
    private readonly Label errorText = new();
    private readonly ToolStripStatusLabel statusLabel = new();
    private readonly ToolStripProgressBar statusProgress = new();
    private readonly Button backButton;
    private readonly Button forwardButton;
    private readonly Button refreshButton;
    private readonly Button homeButton;
    private readonly Button retryButton = new() { Text="Retry", Width=110, Height=38 };
    private readonly Button startButton = new() { Text="Start Again", Width=110, Height=38 };
    private readonly string profilePath;
    private System.Windows.Forms.Timer loadingTimer = new() { Interval=350 };
    private int frame;
    private bool ready;

    public MainForm()
    {
        Text=AppConfig.AppName; StartPosition=FormStartPosition.CenterScreen;
        Width=1280; Height=800; MinimumSize=new Size(900,600);
        BackColor=Color.White;

        var bar=new ToolStrip { Dock=DockStyle.Top, GripStyle=ToolStripGripStyle.Hidden };
        backButton=Tool("←","Back"); forwardButton=Tool("→","Forward");
        refreshButton=Tool("↻","Refresh"); homeButton=Tool("⌂","Home");
        backButton.Click += (_,_)=>{if(webView.CanGoBack)webView.GoBack();};
        forwardButton.Click += (_,_)=>{if(webView.CanGoForward)webView.GoForward();};
        refreshButton.Click += (_,_)=>{if(ready){ShowLoading("Refreshing CargoYHavy...");webView.Reload();}};
        homeButton.Click += (_,_)=>NavigateStartup();
        bar.Items.AddRange(new ToolStripItem[]{backButton,forwardButton,refreshButton,homeButton,
            new ToolStripLabel("  MSC CargoYHavy"){Font=new Font("Segoe UI",10,FontStyle.Bold)}});
        Controls.Add(bar);

        webView.Dock=DockStyle.Fill; webView.DefaultBackgroundColor=Color.White;
        webView.NavigationStarting += (_,_)=>ShowLoading("Loading page...");
        webView.NavigationCompleted += NavigationCompleted;
        webView.NewWindowRequested += NewWindow;
        webView.DownloadStarting += DownloadStarting;
        webView.ProcessFailed += ProcessFailed;
        Controls.Add(webView);

        var status=new StatusStrip {Dock=DockStyle.Bottom,SizingGrip=false};
        statusLabel.Text="Starting MSC CargoYHavy...";
        statusProgress.Width=160; statusProgress.Style=ProgressBarStyle.Marquee;
        status.Items.Add(statusLabel); status.Items.Add(new ToolStripSpringLabel()); status.Items.Add(statusProgress);
        Controls.Add(status);

        BuildLoadingOverlay();
        BuildErrorPanel();

        profilePath=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppConfig.AppName,"WebView2Profile");

        loadingTimer.Tick += (_,_)=>{frame=(frame+1)%4;loadingDots.Text=new string('●',frame+1);};
        loadingTimer.Start();
        Shown += async (_,_)=>await InitializeAsync();
        FormClosing += (_,_)=>loadingTimer.Stop();
        UpdateButtons();
    }

    private ToolStripButton Tool(string text,string tip)=>new(text){ToolTipText=tip,AutoSize=false,Width=38,Height=30,
        Font=new Font("Segoe UI Symbol",13),DisplayStyle=ToolStripItemDisplayStyle.Text};

    private async Task InitializeAsync()
    {
        try
        {
            ShowLoading("Preparing secure browser...");
            Directory.CreateDirectory(profilePath);
            var env=await CoreWebView2Environment.CreateAsync(null,profilePath);
            ShowLoading("Starting CargoYHavy browser...");
            await webView.EnsureCoreWebView2Async(env);
            ready=true;
            var s=webView.CoreWebView2.Settings;
            s.IsStatusBarEnabled=false; s.IsZoomControlEnabled=true;
            s.AreDefaultContextMenusEnabled=true; s.IsBuiltInErrorPageEnabled=false;
            NavigateStartup();
        }
        catch(Exception ex)
        {
            ShowError("WebView2 could not be started.",
                "Install/repair Microsoft Edge WebView2 Runtime, then click Retry.\r\n\r\n"+ex.Message);
        }
    }

    private void NavigateStartup()
    {
        if(!ready)return;
        HideError();
        ShowLoading("Connecting to CargoYHavy...");
        var url=string.IsNullOrWhiteSpace(AppConfig.DashboardUrl)?AppConfig.RegistrationUrl:AppConfig.DashboardUrl;
        webView.CoreWebView2.Navigate(url);
    }

    private void NavigationCompleted(object? s,CoreWebView2NavigationCompletedEventArgs e)
    {
        if(e.IsSuccess){HideLoading();HideError();statusLabel.Text="Ready";statusProgress.Style=ProgressBarStyle.Continuous;statusProgress.Value=100;}
        else ShowError("The page could not be loaded.","Navigation error: "+e.WebErrorStatus+
            "\r\n\r\nCheck your internet connection and try again.");
        UpdateButtons();
    }

    private void NewWindow(object? s,CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled=true;
        if(Uri.TryCreate(e.Uri,UriKind.Absolute,out var u))
        {
            if(u.Scheme=="http"||u.Scheme=="https")webView.CoreWebView2.Navigate(e.Uri);
            else try{Process.Start(new ProcessStartInfo(u.AbsoluteUri){UseShellExecute=true});}catch{}
        }
    }

    private void DownloadStarting(object? s,CoreWebView2DownloadStartingEventArgs e)
    {
        try{
            var d=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"Downloads");
            Directory.CreateDirectory(d);
            e.ResultFilePath=Path.Combine(d,Path.GetFileName(e.ResultFilePath));
        }catch{}
    }

    private void ProcessFailed(object? s,CoreWebView2ProcessFailedEventArgs e)
    {
        BeginInvoke(new Action(()=>ShowError("CargoYHavy browser stopped unexpectedly.",
            "The embedded browser process stopped. Click Retry to restart the page.")));
    }

    private void BuildLoadingOverlay()
    {
        loadingOverlay.Dock=DockStyle.Fill; loadingOverlay.BackColor=Color.White;
        loadingTitle.Text=AppConfig.AppName; loadingTitle.TextAlign=ContentAlignment.MiddleCenter;
        loadingTitle.Font=new Font("Segoe UI",22,FontStyle.Bold);
        loadingText.Text="Connecting to CargoYHavy..."; loadingText.TextAlign=ContentAlignment.MiddleCenter;
        loadingText.Font=new Font("Segoe UI",11);
        loadingDots.Text="●"; loadingDots.TextAlign=ContentAlignment.MiddleCenter;
        loadingDots.Font=new Font("Segoe UI",24,FontStyle.Bold);
        loadingBar.Style=ProgressBarStyle.Marquee; loadingBar.MarqueeAnimationSpeed=28; loadingBar.Width=320;
        loadingOverlay.Controls.AddRange(new Control[]{loadingTitle,loadingDots,loadingText,loadingBar});
        loadingOverlay.Resize += (_,_)=>LayoutLoading();
        Controls.Add(loadingOverlay); loadingOverlay.BringToFront();
    }

    private void BuildErrorPanel()
    {
        errorPanel.Dock=DockStyle.Fill; errorPanel.BackColor=Color.White; errorPanel.Visible=false;
        errorTitle.TextAlign=ContentAlignment.MiddleCenter; errorTitle.Font=new Font("Segoe UI",20,FontStyle.Bold);
        errorText.TextAlign=ContentAlignment.MiddleCenter; errorText.Font=new Font("Segoe UI",11);
        retryButton.Click+=(_,_)=>NavigateStartup(); startButton.Click+=(_,_)=>NavigateStartup();
        errorPanel.Controls.AddRange(new Control[]{errorTitle,errorText,retryButton,startButton});
        errorPanel.Resize+=(_,_)=>LayoutError(); Controls.Add(errorPanel);
    }

    private void LayoutLoading()
    {
        int x=loadingOverlay.ClientSize.Width/2,y=loadingOverlay.ClientSize.Height/2;
        loadingTitle.Bounds=new Rectangle(x-250,y-100,500,45);
        loadingDots.Bounds=new Rectangle(x-100,y-45,200,45);
        loadingText.Bounds=new Rectangle(x-300,y+5,600,35);
        loadingBar.Location=new Point(x-loadingBar.Width/2,y+55);
    }

    private void LayoutError()
    {
        int x=errorPanel.ClientSize.Width/2,y=errorPanel.ClientSize.Height/2;
        errorTitle.Bounds=new Rectangle(x-350,y-90,700,45);
        errorText.Bounds=new Rectangle(x-400,y-35,800,70);
        retryButton.Location=new Point(x-120,y+55); startButton.Location=new Point(x+10,y+55);
    }

    private void ShowLoading(string msg)
    {
        loadingText.Text=msg; loadingOverlay.Visible=true; loadingOverlay.BringToFront();
        errorPanel.Visible=false; statusLabel.Text=msg; statusProgress.Style=ProgressBarStyle.Marquee;
    }

    private void HideLoading(){loadingOverlay.Visible=false;statusProgress.Style=ProgressBarStyle.Continuous;statusProgress.Value=100;}
    private void ShowError(string title,string msg)
    {
        loadingOverlay.Visible=false; errorPanel.Visible=true; errorPanel.BringToFront();
        errorTitle.Text=title;errorText.Text=msg;statusLabel.Text="Unable to load";
        statusProgress.Style=ProgressBarStyle.Continuous;statusProgress.Value=0;LayoutError();
    }
    private void HideError(){errorPanel.Visible=false;}
    private void UpdateButtons(){backButton.Enabled=ready&&webView.CanGoBack;forwardButton.Enabled=ready&&webView.CanGoForward;refreshButton.Enabled=ready;}
}
