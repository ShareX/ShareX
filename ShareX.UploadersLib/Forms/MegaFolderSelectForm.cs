using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ShareX.UploadersLib.FileUploaders.Mega;

namespace ShareX.UploadersLib.Forms
{
    public partial class MegaFolderSelectForm : Form
    {
        private ImageList imageList;

        private bool _noise = false; // When true, selection change events are ignored.

        /// <summary>
        /// The entire collection of nodes in the users account.
        /// </summary>
        private List<DisplayNode> allNodes = new List<DisplayNode>();
        private int selectedNodeIdx = -1;
        /// <summary>
        /// The displayed nodes at the currently selected node idx.
        /// </summary>
        private List<DisplayNode> displayedNodes = new List<DisplayNode>();
        

        /// <summary>
        /// Init Dialog. 
        /// </summary>
        /// <param name="Mega">Logged in Mega client. No sanity checks here, do beforehand</param>
        public MegaFolderSelectForm(Mega mega)
        {
            InitializeComponent();
            imageList = new ImageList();
            imageList.Images.Add(Resources.folder);
            listViewNodes.LargeImageList = imageList;
            listViewNodes.SmallImageList = imageList; // 2-lazy for this
            listViewNodes.Enabled = false;
            listViewNodes.RetrieveVirtualItem += ListViewNodes_RetrieveVirtualItem;
            listViewNodes.SelectedIndexChanged += ListViewNodes_SelectedIndexChanged;
            
            BackgroundWorker nodeLoader = new BackgroundWorker();
            nodeLoader.DoWork += NodeLoader_DoWork;
            nodeLoader.RunWorkerCompleted += NodeLoader_RunWorkerCompleted;
            nodeLoader.RunWorkerAsync(mega);
            Text = "Loading...";
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
            buttonSelectFolder.Enabled = false;
        }

        public String getSelectedNodeID()
        {
            if (selectedNodeIdx == -1) return null;
            return allNodes[selectedNodeIdx].Node.Id;
        }

        public String getSelectedFolderPath()
        {
            return allNodes[selectedNodeIdx].DisplayName;
        }

        private void NodeLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            allNodes.Clear();
            allNodes.AddRange(((Mega)e.Argument).GetDisplayNodes());
        }

        private void NodeLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (allNodes.Count > 1) selectNode(1);
            progressBar1.Visible = false;
            Text = "Select Folder";
            listViewNodes.Enabled = true;
        }

        private void ListViewNodes_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem lvi = new ListViewItem(displayedNodes[e.ItemIndex].DisplayName.Split('\\').Last());
            lvi.Tag = displayedNodes[e.ItemIndex];
            lvi.ImageIndex = 0;
            e.Item = lvi;
        }

        private void ListViewNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewNodes.SelectedIndices.Count == 0) return;
            int index = listViewNodes.SelectedIndices[0];
            listViewNodes.VirtualListSize = 0;
            selectNode(allNodes.IndexOf(displayedNodes[index]));            
        }

        /// <summary>
        /// Display a list of nodes belonging to this node.
        /// </summary>
        /// <param name="nodeIdx"></param>
        public void selectNode(int nodeIdx)
        {
            buttonNavigateUp.Enabled = nodeIdx != 0;
            _noise = true;
            listViewNodes.BeginUpdate();
            selectedNodeIdx = nodeIdx;
            ///MessageBox.Show("dsp name " + allNodes[nodeIdx].DisplayName);
            string nodePath = allNodes[nodeIdx].DisplayName;
            displayedNodes.Clear();
            foreach(DisplayNode node in allNodes)
            {
                if (node.DisplayName.StartsWith(nodePath))
                {
                    if (node.DisplayName.Remove(0, nodePath.Length).Split('\\').Length == 2) displayedNodes.Add(node);
                }
            }
            labelFolderBreadcrumb.Text = allNodes[nodeIdx].DisplayName.Replace("\\", " > ");
            listViewNodes.VirtualListSize = displayedNodes.Count;
            listViewNodes.VirtualMode = true;
            listViewNodes.EndUpdate();
            buttonSelectFolder.Enabled = true;
        }

        public int GetSelectedNodeIdx()
        {
            return selectedNodeIdx;
        }

        private void buttonNavigateUp_Click(object sender, EventArgs e)
        {
            string currentDir = allNodes[selectedNodeIdx].DisplayName;
            if (!currentDir.Contains('\\')) return;
            int    removeCharLength = currentDir.Split('\\').Last().Length;
            currentDir = currentDir.Substring(0, currentDir.Length - removeCharLength - 1); // Remove the last chunk and the trailing slash.
            for (int n = 0; n < allNodes.Count; n++)
            {
                if (allNodes[n].DisplayName.Equals(currentDir))
                {
                    selectNode(n);
                    return;
                }
            }
            selectNode(1);
        }
    }
}
