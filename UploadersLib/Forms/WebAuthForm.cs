using System.Windows.Forms;

namespace UploadersLib.Forms
{
  public partial class WebAuthForm : Form
  {
    private readonly string _startUrl;
    private readonly string _endUrl;
    private readonly AuthCallback _callback;

    public WebAuthForm(string startUrl, string endUrl, AuthCallback callback)
    {
      _startUrl = startUrl;
      _endUrl = endUrl;
      _callback = callback;
      InitializeComponent();
      this.Load += WebAuthForm_Load;
    }

    private void WebAuthForm_Load(object sender, System.EventArgs e)
    {
      webBrowser.Navigated += WebBrowser_Navigated;
      webBrowser.Navigate(_startUrl);
    }

    private void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      if (this.webBrowser.Url.AbsoluteUri.StartsWith(this._endUrl))
      {
        if (this._callback != null)
        {
          this._callback(new AuthCompleteEventArgs
          {
            Result = new AuthResult(webBrowser.Url)
          });
        }
      }
    }
  }
}