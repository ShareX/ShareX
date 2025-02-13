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

using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class AppVeyorUpdateChecker : UpdateChecker
    {
        public string Branch { get; set; } = "master";

        public override async Task CheckUpdateAsync()
        {
            try
            {
                AppVeyor appveyor = new AppVeyor()
                {
                    AccountName = "ShareX",
                    ProjectSlug = "sharex"
                };

                AppVeyorProject project = await appveyor.GetProjectByBranch(Branch);

                if (!project.build.status.Equals("success", StringComparison.OrdinalIgnoreCase) &&
                    !project.build.status.Equals("running", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Latest project build is not successful.");
                }

                AppVeyorProjectJob job = project.build.jobs.FirstOrDefault(x =>
                    x.name.Equals("Configuration: Release", StringComparison.OrdinalIgnoreCase) &&
                    x.osType.Equals("Windows", StringComparison.OrdinalIgnoreCase) &&
                    x.status.Equals("success", StringComparison.OrdinalIgnoreCase));

                if (job == null)
                {
                    throw new Exception("Unable to find successful release build.");
                }

                AppVeyorProjectArtifact[] artifacts = await appveyor.GetArtifacts(job.jobId);

                string deploymentName;

                if (IsPortable)
                {
                    deploymentName = "Portable";
                }
                else
                {
                    deploymentName = "Setup";
                }

                AppVeyorProjectArtifact artifact = artifacts.FirstOrDefault(x => x.name.Equals(deploymentName, StringComparison.OrdinalIgnoreCase));

                if (artifact == null)
                {
                    throw new Exception($"Unable to find \"{deploymentName}\" file.");
                }

                FileName = artifact.fileName;
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