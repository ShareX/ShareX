#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using System.ComponentModel;
using System.Windows.Forms;
using UploadersLib.OtherServices;

namespace UploadersLib
{
    public partial class GoogleTranslateGUI : Form
    {
        public BackgroundWorker CreateWorker()
        {
            BackgroundWorker bwApp = new BackgroundWorker { WorkerReportsProgress = true };
            bwApp.DoWork += BwApp_DoWork;
            bwApp.ProgressChanged += bwApp_ProgressChanged;
            bwApp.RunWorkerCompleted += bwApp_RunWorkerCompleted;
            return bwApp;
        }

        public void BwApp_DoWork(object sender, DoWorkEventArgs e)
        {
            GoogleTranslateInfo gti = e.Argument as GoogleTranslateInfo;
            gti = new GoogleTranslate(Config.APIKey).TranslateText(gti);
            e.Result = gti;
        }

        private void bwApp_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void bwApp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GoogleTranslateInfo gti = e.Result as GoogleTranslateInfo;
            UpdateGoogleTranslateGUI(gti);
        }

        private void UpdateGoogleTranslateGUI(GoogleTranslateInfo info)
        {
            btnTranslate.Enabled = true;
            btnTranslateTo.Enabled = true;

            txtTranslateText.Text = info.Text;
            txtLanguages.Text = info.SourceLanguage + " -> " + info.TargetLanguage;
            txtTranslateResult.Text = info.Result;
        }
    }
}