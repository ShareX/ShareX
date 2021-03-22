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
        
        Action<ShareXTheme> AddTheme;
        int currentlySelectedThemeIndex;
        bool userHasAddedTheme=false;
        ShareXTheme temporaryThemePreview;
        public ToAddNewThemeForm(Action<ShareXTheme> AddThemeFunction)
        {
            InitializeComponent();

          
            ApplySelectedTheme();

            
           
            this.AddTheme = AddThemeFunction;
            AddTemporaryThemePreview();
          

        
        }
        private void ApplySelectedTheme()
        {
            Program.MainForm.UpdateTheme();
            ShareXResources.ApplyTheme(this);
        }
     

        private void btnThemeAdd_Click(object sender, EventArgs e)
        {
            RemoveTemporaryThemePreview();
            ApplySelectedTheme();
            ShareXTheme newTheme =(ShareXTheme) pgTheme.SelectedObject;
            AddTheme(newTheme);
            this.userHasAddedTheme = true;
            this.Close();
           
        }

        private void pgTheme_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
           
            ApplySelectedTheme();

    
        }

        private void pgTheme_Click(object sender, EventArgs e)
        {

        }

        private void ToAddNewThemeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            if (!userHasAddedTheme)
            {
                RemoveTemporaryThemePreview();
          
                ApplySelectedTheme();
            }
           

        }
        /// <summary>
        /// Called either when the form is closed or the user adds a new theme
        /// </summary>
        private void RemoveTemporaryThemePreview()
        {
            Program.Settings.Themes.RemoveAt(Program.Settings.Themes.Count - 1);
            Program.Settings.SelectedTheme = this.currentlySelectedThemeIndex;
        }

        /// <summary>
        ///Add a new theme to [Program.Settings.Themes] and select it as current theme temporarily (for preview purpose)
        /// </summary>
        private void AddTemporaryThemePreview()
        {
            this.currentlySelectedThemeIndex = Program.Settings.SelectedTheme;
            Program.Settings.Themes.Add(ShareXTheme.NewTheme.ShallowCopy());
            Program.Settings.SelectedTheme = Program.Settings.Themes.Count - 1;
            pgTheme.SelectedObject = Program.Settings.Themes[Program.Settings.SelectedTheme];
            
        }
    }
}
