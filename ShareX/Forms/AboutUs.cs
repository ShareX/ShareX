using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.Forms
{
    public partial class AboutUs : Form
    {
        public AboutUs()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/WeiHann");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel5.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/ChinHungNg");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel6.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/JiTeng01");           
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel4.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/JonsanWoon");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel7.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/awyihan0019");
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel8.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/JiTeng01/ShareX.git");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
