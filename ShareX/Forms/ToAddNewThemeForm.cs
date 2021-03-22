using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.Forms
{
    public partial class ToAddNewThemeForm : Form
    {
        ApplicationSettingsForm applicationSettingsForm;
        Action<ShareXTheme> AddTheme;
        public ToAddNewThemeForm(ApplicationSettingsForm applicationSettingsForm,Action<ShareXTheme> AddThemeFunction)
        {
            InitializeComponent();

            //applying theme
            Program.MainForm.UpdateTheme();
            ShareXResources.ApplyTheme(this);

            //initialize property grid
            pgTheme.SelectedObject = ShareXTheme.NewTheme.ShallowCopy();

            this.AddTheme = AddThemeFunction;
        }

        private void pgTheme_Click(object sender, EventArgs e)
        {

        }

        private void btnThemeAdd_Click(object sender, EventArgs e)
        {
            ShareXTheme newTheme =(ShareXTheme) pgTheme.SelectedObject;
            //AddTheme(newTheme);
        }
    }
}
