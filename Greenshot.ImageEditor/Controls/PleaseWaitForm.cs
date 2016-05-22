/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using GreenshotPlugin.Core;
using System;
using System.Threading;
using System.Windows.Forms;

namespace GreenshotPlugin.Controls
{
    /// <summary>
    /// Description of PleaseWaitForm.
    /// </summary>
    internal partial class PleaseWaitForm : Form
    {
        private Thread waitFor;
        private string title;

        public PleaseWaitForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            Icon = GreenshotResources.getGreenshotIcon();
        }

        /// <summary>
        /// Prevent the close-window button showing
        /// </summary>
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ClassStyle = createParams.ClassStyle | CP_NOCLOSE_BUTTON;
                return createParams;
            }
        }

        /// <summary>
        /// Show the "please wait" form, execute the code from the delegate and wait until execution finishes.
        /// The supplied delegate will be wrapped with a try/catch so this method can return any exception that was thrown.
        /// </summary>
        /// <param name="title">The title of the form (and Thread)</param>
        /// <param name="text">The text in the form</param>
        /// <param name="waitDelegate">delegate { with your code }</param>
        public void ShowAndWait(string title, string text, ThreadStart waitDelegate)
        {
            this.title = title;
            Text = title;
            label_pleasewait.Text = text;
            cancelButton.Text = "Cancel";

            // Make sure the form is shown.
            Show();

            // Variable to store the exception, if one is generated, from inside the thread.
            Exception threadException = null;
            try
            {
                // Wrap the passed delegate in a try/catch which makes it possible to save the exception
                waitFor = new Thread(new ThreadStart(
                    delegate
                    {
                        try
                        {
                            waitDelegate.Invoke();
                        }
                        catch (Exception ex)
                        {
                            LOG.Error("invoke error:", ex);
                            threadException = ex;
                        }
                    })
                );
                waitFor.Name = title;
                waitFor.IsBackground = true;
                waitFor.SetApartmentState(ApartmentState.STA);
                waitFor.Start();

                // Wait until finished
                while (!waitFor.Join(TimeSpan.FromMilliseconds(100)))
                {
                    Application.DoEvents();
                }
                LOG.DebugFormat("Finished {0}", title);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                throw;
            }
            finally
            {
                Close();
            }
            // Check if an exception occured, if so throw it
            if (threadException != null)
            {
                throw threadException;
            }
        }

        /// <summary>
        /// Called if the cancel button is clicked, will use Thread.Abort()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, EventArgs e)
        {
            LOG.DebugFormat("Cancel clicked on {0}", title);
            cancelButton.Enabled = false;
            waitFor.Abort();
        }
    }
}