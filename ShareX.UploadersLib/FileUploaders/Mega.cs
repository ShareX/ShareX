#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using CG.Web.MegaApiClient;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class MegaFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Mega;

        public override Icon ServiceIcon => Resources.Mega;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.MegaAuthInfos != null && config.MegaAuthInfos.Email != null && config.MegaAuthInfos.Hash != null &&
                config.MegaAuthInfos.PasswordAesKey != null;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Mega(config.MegaAuthInfos?.GetMegaApiClientAuthInfos(), config.MegaParentNodeId);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpMega;
    }

    public sealed class Mega : FileUploader, IWebClient
    {
        // Pack all chunks in a single upload fragment
        // (by default, MegaApiClient splits files in 1MB fragments and do multiple uploads)
        // It allows to have a consistent upload progression in ShareX
        private const int UploadChunksPackSize = -1;

        private readonly MegaApiClient megaClient;
        private readonly MegaApiClient.AuthInfos authInfos;
        private readonly string parentNodeId;

        public Mega() : this(null, null)
        {
        }

        public Mega(MegaApiClient.AuthInfos authInfos) : this(authInfos, null)
        {
        }

        public Mega(MegaApiClient.AuthInfos authInfos, string parentNodeId)
        {
            AllowReportProgress = false;
            Options options = new Options(chunksPackSize: UploadChunksPackSize);
            megaClient = new MegaApiClient(options, this);
            this.authInfos = authInfos;
            this.parentNodeId = parentNodeId;
        }

        public bool TryLogin()
        {
            try
            {
                Login();
                return true;
            }
            catch (ApiException)
            {
                return false;
            }
        }

        private void Login()
        {
            if (authInfos == null)
            {
                megaClient.LoginAnonymous();
            }
            else
            {
                megaClient.Login(authInfos);
            }
        }

        internal IEnumerable<DisplayNode> GetDisplayNodes()
        {
            IEnumerable<INode> nodes = megaClient.GetNodes().Where(n => n.Type == NodeType.Directory || n.Type == NodeType.Root).ToArray();
            List<DisplayNode> displayNodes = new List<DisplayNode>();

            foreach (INode node in nodes)
            {
                displayNodes.Add(new DisplayNode(node, nodes));
            }

            displayNodes.Sort((x, y) => string.Compare(x.DisplayName, y.DisplayName, StringComparison.CurrentCultureIgnoreCase));
            displayNodes.Insert(0, DisplayNode.EmptyNode);

            return displayNodes;
        }

        public INode GetParentNode()
        {
            if (authInfos == null || parentNodeId == null)
            {
                return megaClient.GetNodes().SingleOrDefault(n => n.Type == NodeType.Root);
            }

            return megaClient.GetNodes().SingleOrDefault(n => n.Id == parentNodeId);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Login();

            INode createdNode = megaClient.Upload(stream, fileName, GetParentNode());

            UploadResult res = new UploadResult();
            res.IsURLExpected = true;
            res.URL = megaClient.GetDownloadLink(createdNode).ToString();

            return res;
        }

        #region IWebClient

        public Stream GetRequestRaw(Uri url)
        {
            throw new NotImplementedException();
        }

        public string PostRequestJson(Uri url, string jsonData)
        {
            return SendRequest(HttpMethod.POST, url.ToString(), jsonData, RequestHelpers.ContentTypeJSON);
        }

        public string PostRequestRaw(Uri url, Stream dataStream)
        {
            try
            {
                AllowReportProgress = true;
                return SendRequest(HttpMethod.POST, url.ToString(), dataStream, "application/octet-stream");
            }
            finally
            {
                AllowReportProgress = false;
            }
        }

        public Stream PostRequestRawAsStream(Uri url, Stream dataStream)
        {
            throw new NotImplementedException();
        }

        #endregion IWebClient

        internal class DisplayNode
        {
            public static readonly DisplayNode EmptyNode = new DisplayNode();

            private DisplayNode()
            {
                DisplayName = "[Select a folder]";
            }

            public DisplayNode(INode node, IEnumerable<INode> nodes)
            {
                Node = node;
                DisplayName = GenerateDisplayName(node, nodes);
            }

            public INode Node { get; private set; }

            public string DisplayName { get; private set; }

            private string GenerateDisplayName(INode node, IEnumerable<INode> nodes)
            {
                List<string> nodesTree = new List<string>();

                INode parent = node;
                do
                {
                    if (parent.Type == NodeType.Directory)
                    {
                        nodesTree.Add(parent.Name);
                    }
                    else
                    {
                        nodesTree.Add(parent.Type.ToString());
                    }

                    parent = nodes.FirstOrDefault(n => n.Id == parent.ParentId);
                }
                while (parent != null);

                nodesTree.Reverse();
                return string.Join(@"\", nodesTree.ToArray());
            }
        }
    }
}