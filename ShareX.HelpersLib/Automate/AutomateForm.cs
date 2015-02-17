#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

using ShareX.HelpersLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class AutomateForm : Form
    {
        private static AutomateForm instance;

        public static bool IsRunning { get; private set; }

        public List<ScriptInfo> Scripts { get; private set; }

        private FunctionManager functionManager = new FunctionManager();
        private Tokenizer tokenizer = new Tokenizer();

        private AutomateForm(List<ScriptInfo> scripts)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            tokenizer.Keywords = FunctionManager.Functions.Select(x => x.Key).ToArray();
            cbFunctions.Items.AddRange(tokenizer.Keywords);
            cbFunctions.SelectedIndex = 0;
            Tokenize();

            Scripts = scripts;

            foreach (ScriptInfo scriptInfo in Scripts)
            {
                AddScript(scriptInfo);
            }

            if (lvScripts.Items.Count > 0)
            {
                lvScripts.Items[0].Selected = true;
            }
        }

        public static AutomateForm GetInstance(List<ScriptInfo> scripts)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new AutomateForm(scripts);
            }

            return instance;
        }

        private void AddScript(ScriptInfo scriptInfo)
        {
            lvScripts.Items.Add(scriptInfo.Name).Tag = scriptInfo;
        }

        private void rtbInput_TextChanged(object sender, EventArgs e)
        {
            Tokenize();
        }

        private void Tokenize()
        {
            if (!string.IsNullOrEmpty(rtbInput.Text))
            {
                List<Token> tokens = tokenizer.Tokenize(rtbInput.Text);
                SyntaxHighlighting(tokens);
            }
        }

        private void SyntaxHighlighting(List<Token> tokens)
        {
            int start = rtbInput.SelectionStart;
            int length = rtbInput.SelectionLength;

            rtbInput.BeginUpdate();

            foreach (Token token in tokens)
            {
                Color color;

                switch (token.Type)
                {
                    default:
                        continue;
                    case TokenType.Symbol:
                        color = Color.Red;
                        break;
                    case TokenType.Literal:
                        color = Color.Brown;
                        break;
                    case TokenType.Identifier:
                        color = Color.DarkBlue;
                        break;
                    case TokenType.Numeric:
                        color = Color.Blue;
                        break;
                    case TokenType.Keyword:
                        color = Color.Green;
                        break;
                }

                rtbInput.SelectionStart = token.Position;
                rtbInput.SelectionLength = token.Text.Length;
                rtbInput.SelectionColor = color;
            }

            rtbInput.Select(start, length);
            rtbInput.EndUpdate();
        }

        private void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                btnRun.Text = Resources.Stop;
                string[] lines = rtbInput.Lines;
                functionManager.LineDelay = (int)nudLineDelay.Value;
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += bw_DoWork;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                bw.RunWorkerAsync(lines);
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                functionManager.Stop();
                btnRun.Text = Resources.Start;
                IsRunning = false;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (IsRunning)
                {
                    Stop();
                }
                else
                {
                    Start();
                }
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] lines = e.Argument as string[];

            try
            {
                functionManager.Compile(lines);
                functionManager.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Stop();
        }

        private void btnLoadExample_Click(object sender, EventArgs e)
        {
            rtbInput.Text = @"""This is comment""
Wait 3000
Call KeyboardFunctions
Call MouseFunctions
""You can use 0 to loop forever""
3 Call LoopTest
5 KeyPress enter

Function KeyboardFunctions
KeyDown space
KeyUp space
KeyPress a
KeyPressText ""Test 123""

Function MouseFunctions
MouseMove 300 250
MouseDown left
MouseUp left
MouseClick right
MouseClick 100 450 left
MouseWheel 120

Function LoopTest
Wait 1000
KeyPressText ""Loop""";
        }

        private void btnSaveScript_Click(object sender, EventArgs e)
        {
            string scriptName = txtScriptName.Text;

            if (string.IsNullOrEmpty(scriptName))
            {
                MessageBox.Show("Script name can't be empty.", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScriptInfo scriptInfo = Scripts.FirstOrDefault(x => x.Name.Equals(scriptName, StringComparison.InvariantCultureIgnoreCase));

            if (scriptInfo != null)
            {
                scriptInfo.Script = rtbInput.Text;
                scriptInfo.LineDelay = (int)nudLineDelay.Value;
            }
            else
            {
                scriptInfo = new ScriptInfo()
                {
                    Name = scriptName,
                    Script = rtbInput.Text,
                    LineDelay = (int)nudLineDelay.Value
                };
                Scripts.Add(scriptInfo);
                AddScript(scriptInfo);
            }
        }

        private void btnRemoveScript_Click(object sender, EventArgs e)
        {
            if (lvScripts.SelectedIndices.Count > 0)
            {
                int index = lvScripts.SelectedIndices[0];
                Scripts.RemoveAt(index);
                lvScripts.Items.RemoveAt(index);
                rtbInput.Clear();
                txtScriptName.Clear();
            }
        }

        private void lvScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvScripts.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvScripts.SelectedItems[0];
                ScriptInfo scriptInfo = lvi.Tag as ScriptInfo;
                if (scriptInfo != null)
                {
                    txtScriptName.Text = scriptInfo.Name;
                    rtbInput.Text = scriptInfo.Script;
                    nudLineDelay.Value = scriptInfo.LineDelay;
                    Tokenize();
                }
            }
        }

        private void btnAddMouseMove_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(2000);
                Point position = Cursor.Position;
                this.InvokeSafe(() =>
                {
                    rtbInput.SelectedText = string.Format("MouseMove {0} {1}", position.X, position.Y);
                });
            });

            thread.Start();
        }
    }
}