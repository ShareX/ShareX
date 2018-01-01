#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.HelpersLib
{
    public class AppVeyorUpdateChecker : UpdateChecker
    {
        public override void CheckUpdate()
        {
            try
            {
                AppVeyor appveyor = new AppVeyor()
                {
                    AccountName = "ShareX",
                    ProjectSlug = "sharex",
                    Proxy = Proxy
                };

                AppVeyorProject project = appveyor.GetProjectByBranch("master");

                if (!project.build.status.Equals("success", StringComparison.InvariantCultureIgnoreCase) &&
                    !project.build.status.Equals("running", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception("Latest project build is not successful.");
                }

                AppVeyorProjectJob job = project.build.jobs.FirstOrDefault(x =>
                    x.name.Equals("Configuration: Release", StringComparison.InvariantCultureIgnoreCase) &&
                    x.osType.Equals("Windows", StringComparison.InvariantCultureIgnoreCase) &&
                    x.status.Equals("success", StringComparison.InvariantCultureIgnoreCase));

                if (job == null)
                {
                    throw new Exception("Unable to find successful release build.");
                }

                AppVeyorProjectArtifact[] artifacts = appveyor.GetArtifacts(job.jobId);

                AppVeyorProjectArtifact artifact = artifacts.FirstOrDefault(x => x.name.Equals("Setup", StringComparison.InvariantCultureIgnoreCase));

                if (artifact == null)
                {
                    throw new Exception("Unable to find setup file.");
                }

                Filename = artifact.fileName;
                DownloadURL = appveyor.GetArtifactDownloadURL(job.jobId, artifact.fileName);
                if (Version.TryParse(project.build.version, out Version version))
                {
                    LatestVersion = version;
                }
                RefreshStatus();
                Status = UpdateStatus.UpdateAvailable;
                return;
            }
            catch (Exception e)
            {
                e.ShowError();
            }

            Status = UpdateStatus.UpdateCheckFailed;
        }
    }
}