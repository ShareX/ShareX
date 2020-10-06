using ShareX.HelpersLib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShareX
{
    /// <summary>
    /// Interaction logic for FileExistView.xaml
    /// </summary>
    public partial class FileExistView : Window
    {
        public string Filepath { get; private set; }

        private string filename;
        private string uniqueFilepath;

        public FileExistView(string filepath)
        {
            InitializeComponent();

            Filepath = filepath;
            filename = Path.GetFileNameWithoutExtension(Filepath);
            txtNewName.Text = filename;
            btnOverwrite.Content += Path.GetFileName(Filepath);
            uniqueFilepath = Helpers.GetUniqueFilePath(Filepath);
            btnUniqueName.Content += Path.GetFileName(uniqueFilepath);
        }

        private string GetNewFilename()
        {
            string newFilename = txtNewName.Text;

            if (!string.IsNullOrEmpty(newFilename))
            {
                return newFilename + Path.GetExtension(Filepath);
            }

            return "";
        }

        private void UseNewFilename()
        {
            string newFilename = GetNewFilename();

            if (!string.IsNullOrEmpty(newFilename))
            {
                Filepath = Path.Combine(Path.GetDirectoryName(Filepath), newFilename);
                Close();
            }
        }

        private void UseUniqueFilename()
        {
            Filepath = uniqueFilepath;
            Close();
        }

        private void Cancel()
        {
            Filepath = "";
            Close();
        }

        private void txtNewName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newFilename = txtNewName.Text;
            btnNewName.IsEnabled = !string.IsNullOrEmpty(newFilename) && !newFilename.Equals(filename, StringComparison.InvariantCultureIgnoreCase);
            btnNewName.Content = "Use new name: " + GetNewFilename();
        }

        private void txtNewName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }

        private void txtNewName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string newFilename = txtNewName.Text;

                if (!string.IsNullOrEmpty(newFilename))
                {
                    if (newFilename.Equals(filename, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Close();
                    }
                    else
                    {
                        UseNewFilename();
                    }
                }
            }
            else if (e.Key == Key.Escape)
            {
                Cancel();
            }
        }

        private void btnNewName_Click(object sender, RoutedEventArgs e)
        {
            UseNewFilename();
        }

        private void btnOverwrite_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnUniqueName_Click(object sender, RoutedEventArgs e)
        {
            UseUniqueFilename();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }
    }
}
