using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
        }
    }
}