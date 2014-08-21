using System;

namespace UploadersLib.Forms
{
  public class AuthResult
  {
    public AuthResult(Uri resultUri)
    {
      string[] queryParams = resultUri.Query.TrimStart('?').Split('&');
      foreach (string param in queryParams)
      {
        string[] kvp = param.Split('=');
        switch (kvp[0])
        {
          case "code":
            this.AuthorizeCode = kvp[1];
            break;
          case "error":
            this.ErrorCode = kvp[1];
            break;
          case "error_description":
            this.ErrorDescription = Uri.UnescapeDataString(kvp[1]);
            break;
        }
      }
    }

    public string AuthorizeCode { get; private set; }
    public string ErrorCode { get; private set; }
    public string ErrorDescription { get; private set; }
  }
}