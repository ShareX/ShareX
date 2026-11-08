#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using Xunit;

namespace ShareX.Tests;

public sealed class AfterCaptureGoalTests
{
    private static readonly IReadOnlyList<AfterCaptureGoal> AllGoals = AfterCaptureGoals.GetAvailableGoals(hasImage: true, canUpload: true);

    [Fact]
    public void ShareXDefaultsStayLocalEvenThoughCopyUrlIsAnAfterUploadTask()
    {
        AfterCaptureGoal goal = AfterCaptureGoals.InferGoal(AfterCaptureTasks.CopyImageToClipboard | AfterCaptureTasks.SaveImageToFile, AllGoals);
        AfterCaptureJobs jobs = AfterCaptureGoals.Resolve(goal, AfterCaptureTasks.SaveImageToFile, AfterUploadTasks.CopyURLToClipboard, false);

        Assert.Equal(AfterCaptureGoal.CopyImage, goal);
        Assert.False(jobs.Capture.HasFlag(AfterCaptureTasks.UploadImageToHost));
        Assert.True(jobs.Capture.HasFlag(AfterCaptureTasks.SaveImageToFile));
    }

    [Theory]
    [InlineData(AfterCaptureTasks.UploadImageToHost | AfterCaptureTasks.CopyImageToClipboard, AfterCaptureGoal.CopyUrl)]
    [InlineData(AfterCaptureTasks.CopyFileToClipboard | AfterCaptureTasks.CopyFilePathToClipboard, AfterCaptureGoal.CopyFile)]
    [InlineData(AfterCaptureTasks.CopyFilePathToClipboard | AfterCaptureTasks.CopyImageToClipboard, AfterCaptureGoal.CopyFilePath)]
    [InlineData(AfterCaptureTasks.CopyFolderPathToClipboard, AfterCaptureGoal.CopyFolderPath)]
    [InlineData(AfterCaptureTasks.SaveImageToFile, AfterCaptureGoal.None)]
    [InlineData(AfterCaptureTasks.None, AfterCaptureGoal.None)]
    public void InferGoal_MatchesWhatWouldEndUpOnTheClipboard(AfterCaptureTasks capture, AfterCaptureGoal expected)
    {
        Assert.Equal(expected, AfterCaptureGoals.InferGoal(capture, AllGoals));
    }

    [Theory]
    [InlineData(AfterCaptureGoal.CopyImage, AfterCaptureTasks.CopyImageToClipboard)]
    [InlineData(AfterCaptureGoal.CopyFile, AfterCaptureTasks.CopyFileToClipboard)]
    [InlineData(AfterCaptureGoal.CopyFilePath, AfterCaptureTasks.CopyFilePathToClipboard)]
    [InlineData(AfterCaptureGoal.CopyFolderPath, AfterCaptureTasks.CopyFolderPathToClipboard)]
    [InlineData(AfterCaptureGoal.CopyUrl, AfterCaptureTasks.UploadImageToHost)]
    public void Resolve_KeepsExactlyOneGoalTask(AfterCaptureGoal goal, AfterCaptureTasks expected)
    {
        AfterCaptureJobs jobs = AfterCaptureGoals.Resolve(goal, AfterCaptureGoals.GoalCaptureTasks, AfterUploadTasks.None, false);

        Assert.Equal(expected, jobs.Capture & AfterCaptureGoals.GoalCaptureTasks);
    }

    [Fact]
    public void NoGoalCopiesAndUploadsNothing()
    {
        AfterCaptureJobs jobs = AfterCaptureGoals.Resolve(AfterCaptureGoal.None, AfterCaptureGoals.GoalCaptureTasks, AfterUploadTasks.None, false);

        Assert.Equal(AfterCaptureTasks.None, jobs.Capture);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void NoGoalIsNeverOfferedAsAChoice(bool hasImage, bool canUpload)
    {
        Assert.DoesNotContain(AfterCaptureGoal.None, AfterCaptureGoals.GetAvailableGoals(hasImage, canUpload));
    }

    [Fact]
    public void FileGoalsSaveTheCaptureAndNeverDeleteIt()
    {
        AfterCaptureJobs jobs = AfterCaptureGoals.Resolve(AfterCaptureGoal.CopyFilePath, AfterCaptureTasks.DeleteFile, AfterUploadTasks.None, false);

        Assert.True(jobs.Capture.HasFlag(AfterCaptureTasks.SaveImageToFile));
        Assert.False(jobs.Capture.HasFlag(AfterCaptureTasks.DeleteFile));
    }

    [Fact]
    public void CopyUrlAddsTheUploadAndKeepsItsExtras()
    {
        AfterCaptureJobs jobs = AfterCaptureGoals.Resolve(AfterCaptureGoal.CopyUrl, AfterCaptureTasks.DeleteFile | AfterCaptureTasks.ShowBeforeUploadWindow,
            AfterUploadTasks.UseURLShortener, false);

        Assert.True(jobs.Capture.HasFlag(AfterCaptureTasks.UploadImageToHost));
        Assert.True(jobs.Capture.HasFlag(AfterCaptureTasks.DeleteFile));
        Assert.True(jobs.Capture.HasFlag(AfterCaptureTasks.ShowBeforeUploadWindow));
        Assert.Equal(AfterUploadTasks.CopyURLToClipboard | AfterUploadTasks.UseURLShortener, jobs.Upload);
    }

    [Fact]
    public void RecordingsHaveNoCopyImageGoalAndNoImageTasks()
    {
        IReadOnlyList<AfterCaptureGoal> goals = AfterCaptureGoals.GetAvailableGoals(hasImage: false, canUpload: true);
        AfterCaptureJobs jobs = AfterCaptureGoals.Resolve(AfterCaptureGoal.CopyFilePath, AfterCaptureTasks.PinToScreen, AfterUploadTasks.None, true);

        Assert.DoesNotContain(AfterCaptureGoal.CopyImage, goals);
        Assert.Equal(AfterCaptureGoal.None, AfterCaptureGoals.InferGoal(AfterCaptureTasks.CopyImageToClipboard, goals));
        Assert.Equal(AfterCaptureTasks.CopyFilePathToClipboard, jobs.Capture);
    }

    [Fact]
    public void DisabledUploadRemovesTheCopyUrlGoal()
    {
        IReadOnlyList<AfterCaptureGoal> goals = AfterCaptureGoals.GetAvailableGoals(hasImage: true, canUpload: false);

        Assert.DoesNotContain(AfterCaptureGoal.CopyUrl, goals);
        Assert.Equal(AfterCaptureGoal.None, AfterCaptureGoals.InferGoal(AfterCaptureTasks.UploadImageToHost, goals));
    }
}
