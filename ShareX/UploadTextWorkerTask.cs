using ShareX.UploadersLib;
using ShareX.UploadersLib.TextUploaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareX
{
    public class UploadTextWorkerTask : WorkerTask
    {
        public UploadTextWorkerTask(TaskSettings taskSettings) : base(taskSettings)
        {
        }

        public override UploadResult PerformTask(Stream stream, string fileName)
        {
            TextUploader textUploader = null;

            switch (Info.TaskSettings.TextDestination)
            {
                case TextDestination.Pastebin:
                    PastebinSettings settings = Program.UploadersConfig.PastebinSettings;
                    if (string.IsNullOrEmpty(settings.TextFormat))
                    {
                        settings.TextFormat = Info.TaskSettings.AdvancedSettings.TextFormat;
                    }
                    textUploader = new Pastebin(APIKeys.PastebinKey, settings);
                    break;
                case TextDestination.Paste2:
                    textUploader = new Paste2(new Paste2Settings { TextFormat = Info.TaskSettings.AdvancedSettings.TextFormat });
                    break;
                case TextDestination.Slexy:
                    textUploader = new Slexy(new SlexySettings { TextFormat = Info.TaskSettings.AdvancedSettings.TextFormat });
                    break;
                case TextDestination.Pastee:
                    textUploader = new Pastee { Lexer = Info.TaskSettings.AdvancedSettings.TextFormat };
                    break;
                case TextDestination.Paste_ee:
                    textUploader = new Paste_ee(Program.UploadersConfig.Paste_eeUserAPIKey);
                    break;
                case TextDestination.Gist:
                    textUploader = Program.UploadersConfig.GistAnonymousLogin ? new Gist(Program.UploadersConfig.GistPublishPublic) :
                        new Gist(Program.UploadersConfig.GistPublishPublic, Program.UploadersConfig.GistOAuth2Info);
                    break;
                case TextDestination.Upaste:
                    textUploader = new Upaste(Program.UploadersConfig.UpasteUserKey)
                    {
                        IsPublic = Program.UploadersConfig.UpasteIsPublic
                    };
                    break;
                case TextDestination.Hastebin:
                    textUploader = new Hastebin()
                    {
                        CustomDomain = Program.UploadersConfig.HastebinCustomDomain,
                        SyntaxHighlighting = Program.UploadersConfig.HastebinSyntaxHighlighting
                    };
                    break;
                case TextDestination.OneTimeSecret:
                    textUploader = new OneTimeSecret()
                    {
                        API_KEY = Program.UploadersConfig.OneTimeSecretAPIKey,
                        API_USERNAME = Program.UploadersConfig.OneTimeSecretAPIUsername
                    };
                    break;
                case TextDestination.CustomTextUploader:
                    CustomUploaderItem customUploader = GetCustomUploader(Program.UploadersConfig.CustomTextUploaderSelected);
                    if (customUploader != null)
                    {
                        textUploader = new CustomTextUploader(customUploader);
                    }
                    break;
            }

            if (textUploader != null)
            {
                PrepareUploader(textUploader);
                return textUploader.UploadText(stream, fileName);
            }

            return null;
        }
    }
}