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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace ShareX.Platform.MacOS;

/// <summary>
/// A Finder Quick Action (an Automator service in ~/Library/Services) that runs ShareX with the selected files. It appears in Finder's
/// context menu under Quick Actions and Services, which is how applications without a Finder extension add entries there.
/// </summary>
internal static class FinderQuickAction
{
    /// <summary>The bundle folder name, from the entry's stable identifier, for example "ShareX.workflow".</summary>
    public static string GetBundleName(ShellMenuEntry entry) =>
        new string(entry.Id.Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' or '.' ? c : '_').ToArray()) + ".workflow";

    /// <summary>Runs ShareX in the background with the selected files as arguments; ShareX forwards them to a running instance.</summary>
    public static string CreateCommand(ShellMenuEntry entry)
    {
        StringBuilder command = new StringBuilder(ShellQuote(entry.ExecutablePath));

        foreach (string argument in entry.Arguments)
        {
            command.Append(' ').Append(ShellQuote(argument));
        }

        return command.Append(" \"$@\" >/dev/null 2>&1 &").ToString();
    }

    public static string CreateInfoPlist(ShellMenuEntry entry) => $"""
        <?xml version="1.0" encoding="UTF-8"?>
        <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
        <plist version="1.0">
        <dict>
        	<key>NSServices</key>
        	<array>
        		<dict>
        			<key>NSMenuItem</key>
        			<dict>
        				<key>default</key>
        				<string>{Escape(entry.Label)}</string>
        			</dict>
        			<key>NSMessage</key>
        			<string>runWorkflowAsService</string>
        			<key>NSRequiredContext</key>
        			<dict>
        				<key>NSApplicationIdentifier</key>
        				<string>com.apple.finder</string>
        			</dict>
        			<key>NSSendFileTypes</key>
        			<array>
        				<string>{(entry.Target == ShellMenuTarget.Images ? "public.image" : "public.item")}</string>
        			</array>
        		</dict>
        	</array>
        </dict>
        </plist>

        """;

    public static string CreateDocument(ShellMenuEntry entry)
    {
        string input = entry.Target == ShellMenuTarget.Images ? "com.apple.Automator.fileSystemObject.image" : "com.apple.Automator.fileSystemObject";

        return $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
            <plist version="1.0">
            <dict>
            	<key>AMApplicationBuild</key>
            	<string>523</string>
            	<key>AMApplicationVersion</key>
            	<string>2.10</string>
            	<key>AMDocumentVersion</key>
            	<string>2</string>
            	<key>actions</key>
            	<array>
            		<dict>
            			<key>action</key>
            			<dict>
            				<key>AMAccepts</key>
            				<dict>
            					<key>Container</key>
            					<string>List</string>
            					<key>Optional</key>
            					<true/>
            					<key>Types</key>
            					<array>
            						<string>com.apple.cocoa.string</string>
            					</array>
            				</dict>
            				<key>AMActionVersion</key>
            				<string>2.0.3</string>
            				<key>AMApplication</key>
            				<array>
            					<string>Automator</string>
            				</array>
            				<key>AMParameterProperties</key>
            				<dict>
            					<key>COMMAND_STRING</key>
            					<dict/>
            					<key>CheckedForUserDefaultShell</key>
            					<dict/>
            					<key>inputMethod</key>
            					<dict/>
            					<key>shell</key>
            					<dict/>
            					<key>source</key>
            					<dict/>
            				</dict>
            				<key>AMProvides</key>
            				<dict>
            					<key>Container</key>
            					<string>List</string>
            					<key>Types</key>
            					<array>
            						<string>com.apple.cocoa.string</string>
            					</array>
            				</dict>
            				<key>ActionBundlePath</key>
            				<string>/System/Library/Automator/Run Shell Script.action</string>
            				<key>ActionName</key>
            				<string>Run Shell Script</string>
            				<key>ActionParameters</key>
            				<dict>
            					<key>COMMAND_STRING</key>
            					<string>{Escape(CreateCommand(entry))}</string>
            					<key>CheckedForUserDefaultShell</key>
            					<true/>
            					<key>inputMethod</key>
            					<integer>1</integer>
            					<key>shell</key>
            					<string>/bin/sh</string>
            					<key>source</key>
            					<string></string>
            				</dict>
            				<key>BundleIdentifier</key>
            				<string>com.apple.RunShellScript</string>
            				<key>CFBundleVersion</key>
            				<string>2.0.3</string>
            				<key>CanShowSelectedItemsWhenRun</key>
            				<false/>
            				<key>CanShowWhenRun</key>
            				<true/>
            				<key>Category</key>
            				<array>
            					<string>AMCategoryUtilities</string>
            				</array>
            				<key>Class Name</key>
            				<string>RunShellScriptAction</string>
            				<key>InputUUID</key>
            				<string>{Uuid(entry, "input")}</string>
            				<key>OutputUUID</key>
            				<string>{Uuid(entry, "output")}</string>
            				<key>UUID</key>
            				<string>{Uuid(entry, "action")}</string>
            				<key>UnlocalizedApplications</key>
            				<array>
            					<string>Automator</string>
            				</array>
            				<key>isViewVisible</key>
            				<integer>1</integer>
            			</dict>
            			<key>isViewVisible</key>
            			<integer>1</integer>
            		</dict>
            	</array>
            	<key>connectors</key>
            	<dict/>
            	<key>workflowMetaData</key>
            	<dict>
            		<key>applicationBundleID</key>
            		<string>com.apple.finder</string>
            		<key>applicationPath</key>
            		<string>/System/Library/CoreServices/Finder.app</string>
            		<key>inputTypeIdentifier</key>
            		<string>{input}</string>
            		<key>outputTypeIdentifier</key>
            		<string>com.apple.Automator.nothing</string>
            		<key>presentationMode</key>
            		<integer>15</integer>
            		<key>processesInput</key>
            		<false/>
            		<key>serviceApplicationBundleID</key>
            		<string>com.apple.finder</string>
            		<key>serviceApplicationPath</key>
            		<string>/System/Library/CoreServices/Finder.app</string>
            		<key>serviceInputTypeIdentifier</key>
            		<string>{input}</string>
            		<key>serviceOutputTypeIdentifier</key>
            		<string>com.apple.Automator.nothing</string>
            		<key>serviceProcessesInput</key>
            		<false/>
            		<key>systemImageName</key>
            		<string>NSActionTemplate</string>
            		<key>useAutomaticInputType</key>
            		<false/>
            		<key>workflowTypeIdentifier</key>
            		<string>com.apple.Automator.servicesMenu</string>
            	</dict>
            </dict>
            </plist>

            """;
    }

    internal static string ShellQuote(string value) => "'" + value.Replace("'", "'\\''", StringComparison.Ordinal) + "'";

    private static string Escape(string value) => SecurityElement.Escape(value) ?? "";

    /// <summary>A stable UUID per entry and role, so rewriting a Quick Action does not change it.</summary>
    private static string Uuid(ShellMenuEntry entry, string role)
    {
        byte[] hash = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("ShareX/" + entry.Id + "/" + role));
        return new Guid(hash.AsSpan(0, 16)).ToString().ToUpperInvariant();
    }
}
