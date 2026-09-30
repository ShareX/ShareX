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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ShareX.Destinations.Tests;

public class RoutingScenarioTests
{
    private readonly DestinationRoutingConfig config = new DestinationRoutingConfig();

    private DestinationInstance Add(string name, UploaderCategory category, string uploader, bool isDefault = true) =>
        config.Add(new DestinationInstance { Name = name, Category = category, Uploader = uploader, IsDefault = isDefault });

    private static RouteRequest Image(string extension) => new RouteRequest { Extension = extension, KnownFileType = PremadeFileTypes.Images };

    private static RouteRequest Text() => new RouteRequest { Extension = "txt", KnownFileType = PremadeFileTypes.Text };

    private static RouteRequest File(string extension) =>
        new RouteRequest { Extension = extension, ExcludedFileTypes = [PremadeFileTypes.Images, PremadeFileTypes.Text] };

    [Fact]
    public void ScenarioC_DefaultRoutesMatchTodaysBehaviour()
    {
        DestinationInstance imgur = Add("Imgur", UploaderCategory.Image, "Imgur");
        DestinationInstance pastebin = Add("Pastebin", UploaderCategory.Text, "Pastebin");
        DestinationInstance dropbox = Add("Dropbox", UploaderCategory.File, "Dropbox");
        List<DestinationRoute> routes =
        [
            new DestinationRoute(PremadeFileTypes.Images, imgur.Id),
            new DestinationRoute(PremadeFileTypes.Text, pastebin.Id),
            new DestinationRoute(PremadeFileTypes.OtherFiles, dropbox.Id)
        ];

        Assert.Same(imgur, RouteResolver.Resolve(config, routes, null, Image("png"))!.Instance);
        Assert.Same(pastebin, RouteResolver.Resolve(config, routes, null, Text())!.Instance);
        Assert.Same(dropbox, RouteResolver.Resolve(config, routes, null, File("zip"))!.Instance);
        // Videos have no route, so they go to Other files like every file did before.
        Assert.Same(dropbox, RouteResolver.Resolve(config, routes, null, File("mp4"))!.Instance);
        Assert.Empty(DestinationRoutes.Validate(config, routes));
    }

    [Fact]
    public void ScenarioA_CustomTypesWinOverPremadeTypes()
    {
        DestinationInstance imgur = Add("Imgur", UploaderCategory.Image, "Imgur");
        DestinationInstance tinypic = Add("TinyPic", UploaderCategory.Image, "Chevereto");
        DestinationInstance dropbox = Add("Dropbox", UploaderCategory.File, "Dropbox");
        DestinationInstance s3 = Add("Amazon S3", UploaderCategory.File, "AmazonS3");
        FileTypeDefinition png = config.AddCustomFileType("PNG", ["png"]);
        FileTypeDefinition jpg = config.AddCustomFileType("JPG", [".JPG", "jpeg"]);
        List<DestinationRoute> routes = new List<DestinationRoute> { new DestinationRoute(PremadeFileTypes.OtherFiles, s3.Id) };
        DestinationRoutes.Set(routes, png.Id, imgur.Id);
        DestinationRoutes.Set(routes, jpg.Id, tinypic.Id);
        DestinationRoutes.Set(routes, PremadeFileTypes.Videos, dropbox.Id);

        Assert.Equal(PremadeFileTypes.OtherFiles, routes[^1].FileTypeId);
        RouteMatch screenshot = RouteResolver.Resolve(config, routes, null, Image("png"))!;
        Assert.Same(imgur, screenshot.Instance);
        Assert.Equal("PNG \u2192 Imgur", screenshot.Description);
        Assert.Same(tinypic, RouteResolver.Resolve(config, routes, null, File("jpeg"))!.Instance);
        Assert.Same(dropbox, RouteResolver.Resolve(config, routes, null, File("mkv"))!.Instance);
        // GIF is an image but has no route of its own, so Other files catches it.
        Assert.Same(s3, RouteResolver.Resolve(config, routes, null, Image("gif"))!.Instance);
        Assert.Same(s3, RouteResolver.Resolve(config, routes, null, Text())!.Instance);
        Assert.Empty(DestinationRoutes.Validate(config, routes));
    }

    [Fact]
    public void ScenarioB_DuplicatedInstancesKeepTheirOwnSettings()
    {
        DestinationInstance s3 = Add("Amazon S3", UploaderCategory.File, "AmazonS3");
        s3.Settings = new JObject { ["AmazonS3Settings"] = new JObject { ["Bucket"] = "media", ["SecretAccessKey"] = "secret" } };
        DestinationInstance r2 = config.Duplicate(s3, "R2 Videos");
        r2.Settings!["AmazonS3Settings"]!["Bucket"] = "videos";
        DestinationInstance dropbox = Add("Dropbox", UploaderCategory.File, "Dropbox");
        List<DestinationRoute> routes =
        [
            new DestinationRoute(PremadeFileTypes.Images, s3.Id),
            new DestinationRoute(PremadeFileTypes.Videos, r2.Id),
            new DestinationRoute(PremadeFileTypes.OtherFiles, dropbox.Id)
        ];

        Assert.Equal("media", (string?)s3.Settings["AmazonS3Settings"]!["Bucket"]);
        Assert.Equal("secret", (string?)r2.Settings["AmazonS3Settings"]!["SecretAccessKey"]);
        Assert.Same(s3, RouteResolver.Resolve(config, routes, null, Image("png"))!.Instance);
        Assert.Same(r2, RouteResolver.Resolve(config, routes, null, File("webm"))!.Instance);
        Assert.Same(dropbox, RouteResolver.Resolve(config, routes, null, File("pdf"))!.Instance);
    }

    [Fact]
    public void TaskOverride_ReplacesOnlyItsFileType()
    {
        DestinationInstance imgur = Add("Imgur", UploaderCategory.Image, "Imgur");
        DestinationInstance dropbox = Add("Dropbox", UploaderCategory.File, "Dropbox");
        DestinationInstance r2 = Add("R2 Videos", UploaderCategory.File, "AmazonS3", isDefault: false);
        List<DestinationRoute> defaults = [new DestinationRoute(PremadeFileTypes.Images, imgur.Id), new DestinationRoute(PremadeFileTypes.OtherFiles, dropbox.Id)];
        List<DestinationRoute> recording = [new DestinationRoute(PremadeFileTypes.Videos, r2.Id)];

        RouteMatch video = RouteResolver.Resolve(config, defaults, recording, File("mp4"))!;
        RouteMatch image = RouteResolver.Resolve(config, defaults, recording, Image("png"))!;

        Assert.Same(r2, video.Instance);
        Assert.True(video.IsOverride);
        Assert.Same(imgur, image.Instance);
        Assert.False(image.IsOverride);
    }

    [Fact]
    public void BrokenOverride_FallsBackToDefaultRoute()
    {
        DestinationInstance imgur = Add("Imgur", UploaderCategory.Image, "Imgur");
        DestinationInstance dropbox = Add("Dropbox", UploaderCategory.File, "Dropbox");
        List<DestinationRoute> defaults = [new DestinationRoute(PremadeFileTypes.Images, imgur.Id), new DestinationRoute(PremadeFileTypes.OtherFiles, dropbox.Id)];

        RouteMatch match = RouteResolver.Resolve(config, defaults, [new DestinationRoute(PremadeFileTypes.Images, Guid.NewGuid())], Image("png"))!;

        Assert.Same(imgur, match.Instance);
        Assert.False(match.IsOverride);
    }

    [Fact]
    public void RemovedInstance_FallsThroughToOtherFiles()
    {
        DestinationInstance extra = Add("S3 Media", UploaderCategory.File, "AmazonS3", isDefault: false);
        DestinationInstance dropbox = Add("Dropbox", UploaderCategory.File, "Dropbox");
        List<DestinationRoute> routes = [new DestinationRoute(PremadeFileTypes.Images, extra.Id), new DestinationRoute(PremadeFileTypes.OtherFiles, dropbox.Id)];

        Assert.True(config.Remove(extra));

        Assert.Same(dropbox, RouteResolver.Resolve(config, routes, null, Image("png"))!.Instance);
        Assert.Single(DestinationRoutes.Validate(config, routes));
    }

    [Fact]
    public void KnownFileType_TakesPrecedenceOverExtensionLookup()
    {
        DestinationInstance imgur = Add("Imgur", UploaderCategory.Image, "Imgur");
        DestinationInstance pastebin = Add("Pastebin", UploaderCategory.Text, "Pastebin");
        DestinationInstance dropbox = Add("Dropbox", UploaderCategory.File, "Dropbox");
        List<DestinationRoute> routes =
        [
            new DestinationRoute(PremadeFileTypes.Images, imgur.Id),
            new DestinationRoute(PremadeFileTypes.Text, pastebin.Id),
            new DestinationRoute(PremadeFileTypes.OtherFiles, dropbox.Id)
        ];

        // A user can add "md" to the task's text extensions. ShareX then classifies the file as text before routing.
        Assert.Same(pastebin, RouteResolver.Resolve(config, routes, null, new RouteRequest { Extension = "xyz", KnownFileType = PremadeFileTypes.Text })!.Instance);
        // And a file ShareX classified as a plain file never lands on the image route by extension.
        Assert.Same(dropbox, RouteResolver.Resolve(config, routes, null, File("png"))!.Instance);
        // Without any hint the premade lists decide.
        Assert.Same(imgur, RouteResolver.Resolve(config, routes, null, RouteRequest.ForFileName("/tmp/a.PNG"))!.Instance);
    }

    [Fact]
    public void NoOtherFilesRoute_ResolvesToNull()
    {
        Assert.Null(RouteResolver.Resolve(config, [], null, File("zip")));
        Assert.Contains("one Other files route", DestinationRoutes.Validate(config, [])[0]);
    }
}

public class CompatibilityTests
{
    private readonly DestinationRoutingConfig config = new DestinationRoutingConfig();

    [Fact]
    public void ImageUploaders_AcceptImagesAndImageOnlyCustomTypes()
    {
        DestinationInstance imgur = config.Add(new DestinationInstance { Name = "Imgur", Category = UploaderCategory.Image, Uploader = "Imgur" });
        FileTypeDefinition png = config.AddCustomFileType("PNG", ["png"]);
        FileTypeDefinition mixed = config.AddCustomFileType("Mixed", ["png", "zip"]);

        Assert.True(config.IsCompatible(imgur, PremadeFileTypes.Get(PremadeFileTypes.Images)!));
        Assert.True(config.IsCompatible(imgur, png));
        Assert.False(config.IsCompatible(imgur, mixed));
        Assert.False(config.IsCompatible(imgur, PremadeFileTypes.Get(PremadeFileTypes.Videos)!));
        Assert.False(config.IsCompatible(imgur, PremadeFileTypes.Get(PremadeFileTypes.OtherFiles)!));
    }

    [Fact]
    public void FileUploaders_AcceptEverything()
    {
        DestinationInstance s3 = config.Add(new DestinationInstance { Name = "S3", Category = UploaderCategory.File, Uploader = "AmazonS3" });

        Assert.All(config.GetFileTypes(), type => Assert.True(config.IsCompatible(s3, type)));
    }

    [Fact]
    public void AcceptedFileTypes_CanBeNarrowed()
    {
        DestinationInstance custom = config.Add(new DestinationInstance
        {
            Name = "My host",
            Category = UploaderCategory.File,
            Uploader = "CustomFileUploader",
            AccountIndex = 0,
            AcceptedFileTypes = [PremadeFileTypes.Videos]
        });

        Assert.Single(config.GetCompatibleInstances(PremadeFileTypes.Get(PremadeFileTypes.Videos)!));
        Assert.Empty(config.GetCompatibleInstances(PremadeFileTypes.Get(PremadeFileTypes.Archives)!));
        Assert.Contains($"{custom.Name} does not accept Other files.", DestinationRoutes.Validate(config, [new DestinationRoute(PremadeFileTypes.OtherFiles, custom.Id)]));
    }

    [Fact]
    public void RouteToIncompatibleInstance_IsReported()
    {
        DestinationInstance pastebin = config.Add(new DestinationInstance { Name = "Pastebin", Category = UploaderCategory.Text, Uploader = "Pastebin" });

        IReadOnlyList<string> errors = DestinationRoutes.Validate(config, [new DestinationRoute(PremadeFileTypes.Images, pastebin.Id)], isDefaultTable: false);

        Assert.Equal(["Pastebin does not accept Images."], errors);
    }
}

public class EditingTests
{
    [Fact]
    public void Duplicate_DeepClonesAndInsertsAfterOriginal()
    {
        DestinationRoutingConfig config = new DestinationRoutingConfig();
        DestinationInstance first = config.Add(new DestinationInstance { Name = "Amazon S3", Category = UploaderCategory.File, Uploader = "AmazonS3", IsDefault = true,
            Settings = new JObject { ["AmazonS3Settings"] = new JObject { ["Bucket"] = "a" } } });
        DestinationInstance last = config.Add(new DestinationInstance { Name = "Dropbox", Category = UploaderCategory.File, Uploader = "Dropbox", IsDefault = true });

        DestinationInstance copy = config.Duplicate(first);
        copy.Settings!["AmazonS3Settings"]!["Bucket"] = "b";

        Assert.Equal(["Amazon S3", "Amazon S3 (2)", "Dropbox"], config.Instances.Select(i => i.Name));
        Assert.NotEqual(first.Id, copy.Id);
        Assert.False(copy.IsDefault);
        Assert.True(copy.IsSameUploader(first));
        Assert.Equal("a", (string?)first.Settings!["AmazonS3Settings"]!["Bucket"]);
        Assert.Equal("Amazon S3 (3)", config.Duplicate(first).Name);
        Assert.NotSame(last, copy);
    }

    [Fact]
    public void DefaultInstances_CannotBeRemoved()
    {
        DestinationRoutingConfig config = new DestinationRoutingConfig();
        DestinationInstance imgur = config.Add(new DestinationInstance { Name = "Imgur", Category = UploaderCategory.Image, Uploader = "Imgur", IsDefault = true });

        Assert.False(config.Remove(imgur));
        Assert.Throws<ArgumentException>(() => config.Rename(imgur, " "));
        config.Rename(imgur, " Imgur anonymous ");
        Assert.Equal("Imgur anonymous", imgur.Name);
    }

    [Fact]
    public void CustomFileTypes_NormaliseExtensions()
    {
        DestinationRoutingConfig config = new DestinationRoutingConfig();

        FileTypeDefinition jpg = config.AddCustomFileType("JPG", ["*.JPG, .jpeg; jpg"]);

        Assert.Equal(["jpg", "jpeg"], jpg.Extensions);
        Assert.Throws<ArgumentException>(() => config.AddCustomFileType("Empty", [" "]));
        Assert.Throws<ArgumentException>(() => config.AddCustomFileType("", ["png"]));
    }

    [Fact]
    public void RemoveFileType_RemovesRoutesEverywhere()
    {
        DestinationRoutingConfig config = new DestinationRoutingConfig();
        FileTypeDefinition png = config.AddCustomFileType("PNG", ["png"]);
        List<DestinationRoute> defaults = [new DestinationRoute(png.Id, Guid.NewGuid()), new DestinationRoute(PremadeFileTypes.OtherFiles, Guid.NewGuid())];
        List<DestinationRoute> task = [new DestinationRoute(png.Id, Guid.NewGuid())];

        DestinationRoutes.RemoveFileType(config, png.Id, defaults, task, null);

        Assert.Empty(config.CustomFileTypes);
        Assert.Single(defaults);
        Assert.Empty(task);
        Assert.Throws<ArgumentException>(() => DestinationRoutes.RemoveFileType(config, PremadeFileTypes.Images));
    }

    [Fact]
    public void OtherFilesRoute_CannotBeRemovedFromDefaultTable()
    {
        List<DestinationRoute> routes = [new DestinationRoute(PremadeFileTypes.OtherFiles, Guid.NewGuid())];

        Assert.False(DestinationRoutes.Remove(routes, PremadeFileTypes.OtherFiles));
        Assert.True(DestinationRoutes.Remove(routes, PremadeFileTypes.OtherFiles, isDefaultTable: false));
    }

    [Fact]
    public void FileTypes_ListPremadeThenCustomThenOtherFiles()
    {
        DestinationRoutingConfig config = new DestinationRoutingConfig();
        config.AddCustomFileType("PNG", ["png"]);

        List<string> names = config.GetFileTypes().Select(type => type.Name).ToList();

        Assert.Equal("Images", names[0]);
        Assert.Equal("PNG", names[^2]);
        Assert.Equal("Other files", names[^1]);
    }

    [Theory]
    [InlineData("shot.png", "png")]
    [InlineData(@"C:\a.b\file", null)]
    [InlineData("archive.tar.GZ", "gz")]
    [InlineData("noext.", null)]
    [InlineData(null, null)]
    public void GetExtension_ReadsLastExtension(string? fileName, string? expected)
    {
        Assert.Equal(expected, RouteRequest.GetExtension(fileName));
    }

    [Fact]
    public void PremadeFileTypes_DoNotShareExtensions()
    {
        List<string> all = PremadeFileTypes.All.SelectMany(type => type.Extensions).ToList();

        Assert.Equal(all.Count, all.Distinct().Count());
    }

    [Fact]
    public void Config_RoundTripsThroughJson()
    {
        DestinationRoutingConfig config = new DestinationRoutingConfig();
        DestinationInstance s3 = config.Add(new DestinationInstance { Name = "S3", Category = UploaderCategory.File, Uploader = "AmazonS3", AccountIndex = 2,
            AcceptedFileTypes = [PremadeFileTypes.Videos], Settings = new JObject { ["AmazonS3Settings"] = new JObject { ["Bucket"] = "x" } } });
        config.AddCustomFileType("PNG", ["png"]);

        DestinationRoutingConfig copy = JsonConvert.DeserializeObject<DestinationRoutingConfig>(JsonConvert.SerializeObject(config))!;

        DestinationInstance read = Assert.Single(copy.Instances);
        Assert.Equal(s3.Id, read.Id);
        Assert.Equal(2, read.AccountIndex);
        Assert.Equal([PremadeFileTypes.Videos], read.AcceptedFileTypes);
        Assert.Equal("x", (string?)read.Settings!["AmazonS3Settings"]!["Bucket"]);
        Assert.Equal("PNG", Assert.Single(copy.CustomFileTypes).Name);
    }
}
