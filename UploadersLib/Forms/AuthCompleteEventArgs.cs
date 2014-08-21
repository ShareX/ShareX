using System;

namespace UploadersLib.Forms
{
  public class AuthCompleteEventArgs : EventArgs
  {

    public AuthResult Result { get; set; }
  }

  public delegate void AuthCallback(AuthCompleteEventArgs eventArgs);
}
