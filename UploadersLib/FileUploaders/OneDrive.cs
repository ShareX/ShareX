#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

// Credits: https://github.com/l0nley

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Live;
using UploadersLib.HelperClasses;

namespace UploadersLib.FileUploaders
{
  public sealed class OneDrive : FileUploader
  {
    public OAuth2Info AuthInfo { get; private set; }

    public string FolderId { get; set; }

    public OneDrive(OAuth2Info authInfo)
    {
      AuthInfo = authInfo;
    }

    public override UploadResult Upload(Stream stream, string fileName)
    {
      try
      {
        // This code should be refactored. This code uses async\await api in synchronous manner,
        // but be aware that async/await adding may cause deadlocks here, because of
        // custom implementation in Microsoft.Live library. 
        // Officially Microsoft doesnot support .net4 with Live SDK. 
        // I port this library from scraps and build for .net4
        var client = new LiveAuthClient(AuthInfo.Client_ID);
        var tsk = client.ExchangeAuthCodeAsync(AuthInfo.Client_Secret);
        try
        {
          tsk.Start();
        }catch
        {
        }
        tsk.Wait();
        var result = tsk.Result;
        var connectClient = new LiveConnectClient(result);
        var upload = connectClient.UploadAsync("me/skydrive/", fileName, stream, OverwriteOption.Overwrite);
        try
        {
          upload.Start();
        }
        catch
        {
        }
        upload.Wait();
        var uploadResult = upload.Result;
        var downloadLink = string.Format("{0}", uploadResult.Result["source"]);
        return new UploadResult
        {
          IsSuccess = true,
          URL = downloadLink
        };
      }
      catch (Exception e)
      {
        return new UploadResult {IsSuccess = false, Errors = new List<string> { e.Message }};
      }
    }
  }
}