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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX;

/// <summary>What the capture should leave on the clipboard. Each goal stands for the tasks needed to reach it.</summary>
public enum AfterCaptureGoal
{
    CopyImage,
    CopyFile,
    CopyFilePath,
    CopyFolderPath,
    CopyUrl,
    LeaveClipboard
}

public readonly record struct AfterCaptureJobs(AfterCaptureTasks Capture, AfterUploadTasks Upload);

/// <summary>
/// Turns a goal plus the optional extras into task flags. Clipboard tasks overwrite each other when they run (the file tasks
/// are an else-if chain and the URL is copied last), so a goal keeps exactly one of them. Upload follows from the Copy URL goal
/// only: a task that merely has "copy URL" in its after upload tasks stays local.
/// </summary>
public static class AfterCaptureGoals
{
    /// <summary>Tasks that belong to a goal and are never optional extras.</summary>
    public const AfterCaptureTasks GoalCaptureTasks = AfterCaptureTasks.CopyImageToClipboard | AfterCaptureTasks.CopyFileToClipboard |
        AfterCaptureTasks.CopyFilePathToClipboard | AfterCaptureTasks.CopyFolderPathToClipboard | AfterCaptureTasks.UploadImageToHost;

    /// <summary>Goals this capture can reach: a recording has no image in memory, and upload may be turned off.</summary>
    public static IReadOnlyList<AfterCaptureGoal> GetAvailableGoals(bool hasImage, bool canUpload)
    {
        List<AfterCaptureGoal> goals = new();

        if (hasImage)
        {
            goals.Add(AfterCaptureGoal.CopyImage);
        }

        goals.Add(AfterCaptureGoal.CopyFile);
        goals.Add(AfterCaptureGoal.CopyFilePath);
        goals.Add(AfterCaptureGoal.CopyFolderPath);

        if (canUpload)
        {
            goals.Add(AfterCaptureGoal.CopyUrl);
        }

        goals.Add(AfterCaptureGoal.LeaveClipboard);
        return goals;
    }

    /// <summary>The goal the saved tasks already express, in the order the tasks would overwrite each other when run.</summary>
    public static AfterCaptureGoal InferGoal(AfterCaptureTasks capture, IReadOnlyCollection<AfterCaptureGoal> available)
    {
        AfterCaptureGoal goal = capture switch
        {
            _ when capture.HasFlag(AfterCaptureTasks.UploadImageToHost) => AfterCaptureGoal.CopyUrl,
            _ when capture.HasFlag(AfterCaptureTasks.CopyFileToClipboard) => AfterCaptureGoal.CopyFile,
            _ when capture.HasFlag(AfterCaptureTasks.CopyFilePathToClipboard) => AfterCaptureGoal.CopyFilePath,
            _ when capture.HasFlag(AfterCaptureTasks.CopyFolderPathToClipboard) => AfterCaptureGoal.CopyFolderPath,
            _ when capture.HasFlag(AfterCaptureTasks.CopyImageToClipboard) => AfterCaptureGoal.CopyImage,
            _ => AfterCaptureGoal.LeaveClipboard
        };

        return available.Contains(goal) ? goal : AfterCaptureGoal.LeaveClipboard;
    }

    public static bool NeedsSavedFile(AfterCaptureGoal goal) =>
        goal is AfterCaptureGoal.CopyFile or AfterCaptureGoal.CopyFilePath or AfterCaptureGoal.CopyFolderPath;

    /// <summary>The steps a goal cannot work without, in the order they run. Recordings are already saved.</summary>
    public static IReadOnlyList<Enum> GetRequiredTasks(AfterCaptureGoal goal, bool fileAlreadySaved)
    {
        List<Enum> tasks = new();

        if (NeedsSavedFile(goal) && !fileAlreadySaved)
        {
            tasks.Add(AfterCaptureTasks.SaveImageToFile);
        }

        switch (goal)
        {
            case AfterCaptureGoal.CopyImage:
                tasks.Add(AfterCaptureTasks.CopyImageToClipboard);
                break;
            case AfterCaptureGoal.CopyFile:
                tasks.Add(AfterCaptureTasks.CopyFileToClipboard);
                break;
            case AfterCaptureGoal.CopyFilePath:
                tasks.Add(AfterCaptureTasks.CopyFilePathToClipboard);
                break;
            case AfterCaptureGoal.CopyFolderPath:
                tasks.Add(AfterCaptureTasks.CopyFolderPathToClipboard);
                break;
            case AfterCaptureGoal.CopyUrl:
                tasks.Add(AfterCaptureTasks.UploadImageToHost);
                tasks.Add(AfterUploadTasks.CopyURLToClipboard);
                break;
        }

        return tasks;
    }

    /// <summary>Whether an optional task is offered for a goal.</summary>
    public static bool IsExtraAvailable(AfterCaptureTasks task, AfterCaptureGoal goal, bool fileAlreadySaved)
    {
        if (GoalCaptureTasks.HasFlag(task))
        {
            return false;
        }

        return task switch
        {
            // These work on the image in memory; a recording is already a saved file.
            AfterCaptureTasks.SaveImageToFile or AfterCaptureTasks.SaveImageToFileWithDialog
                or AfterCaptureTasks.SaveThumbnailImageToFile or AfterCaptureTasks.BeautifyImage or AfterCaptureTasks.AddImageEffects
                or AfterCaptureTasks.AnnotateImage or AfterCaptureTasks.OpenInPhotoEditor or AfterCaptureTasks.PinToScreen
                or AfterCaptureTasks.SendImageToPrinter when fileAlreadySaved => false,
            // Required for these goals, so it is not optional.
            AfterCaptureTasks.SaveImageToFile => !NeedsSavedFile(goal),
            // Deleting the file would leave the clipboard pointing at nothing.
            AfterCaptureTasks.DeleteFile => !NeedsSavedFile(goal),
            AfterCaptureTasks.ShowBeforeUploadWindow => goal == AfterCaptureGoal.CopyUrl,
            _ => true
        };
    }

    /// <summary>The tasks to run: what the goal needs, plus the extras that are offered for it.</summary>
    public static AfterCaptureJobs Resolve(AfterCaptureGoal goal, AfterCaptureTasks captureExtras, AfterUploadTasks uploadExtras, bool fileAlreadySaved)
    {
        AfterCaptureTasks capture = AfterCaptureTasks.None;

        foreach (AfterCaptureTasks task in Enum.GetValues<AfterCaptureTasks>())
        {
            if (task != AfterCaptureTasks.None && captureExtras.HasFlag(task) && IsExtraAvailable(task, goal, fileAlreadySaved))
            {
                capture |= task;
            }
        }

        AfterUploadTasks upload = uploadExtras;

        foreach (Enum task in GetRequiredTasks(goal, fileAlreadySaved))
        {
            if (task is AfterCaptureTasks captureTask)
            {
                capture |= captureTask;
            }
            else if (task is AfterUploadTasks uploadTask)
            {
                upload |= uploadTask;
            }
        }

        return new AfterCaptureJobs(capture, upload);
    }
}
