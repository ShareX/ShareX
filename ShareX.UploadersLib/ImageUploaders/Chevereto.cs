#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class CheveretoImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.Chevereto;

        public override Image ServiceImage => Resources.Chevereto;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.CheveretoUploader != null && !string.IsNullOrEmpty(config.CheveretoUploader.UploadURL) &&
                !string.IsNullOrEmpty(config.CheveretoUploader.APIKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Chevereto(config.CheveretoUploader)
            {
                DirectURL = config.CheveretoDirectURL
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpChevereto;
    }

    public sealed class Chevereto : ImageUploader
    {
        public static List<CheveretoUploader> Uploaders = new List<CheveretoUploader>()
        {
            new CheveretoUploader("http://www.ultraimg.com/api/1/upload", "3374fa58c672fcaad8dab979f7687397"),
            new CheveretoUploader("http://yukle.at/3/api/1/upload", "bf3f97649acf179ac17e2b3004fa44bc"),
            new CheveretoUploader("http://img.patifile.com/api/1/upload", "8320784a9b044510e8c723fb778fe3b7"),
            new CheveretoUploader("http://boltimg.com/api/1/upload", "8dfbcb7ab9b5258a90be7cf09e361894"),
            new CheveretoUploader("http://snapie.net/myapi/1/upload", "aff7bd5bf65b7e30b675a430049894b3"),
            new CheveretoUploader("http://picgur.org/api/1/upload", "0a65553c54cf72127d11281f96518469"),
            new CheveretoUploader("https://pixr.co/api/1/upload", "8fff10a8b0d2852c4167db53aa590e94"),
            new CheveretoUploader("https://sexr.co/api/1/upload", "46b9aa05ec994098c4b6f18b5eed5e36"),
            new CheveretoUploader("http://lightpics.net/api/1/upload", "7c6238e8f24c19454315d5dc812d4b93"),
            new CheveretoUploader("https://imgfly.me/api/1/upload", "c6133147592983996b65dda51ba70255"),
            new CheveretoUploader("http://imgpinas.com/api/1/upload", "7153eeee787ccbb4b01bea44ec0e699e"),
            new CheveretoUploader("http://imu.gr/api/1/upload", "a8e5fcfb79df9be675a6aa0a1541a89e"),
            new CheveretoUploader("http://www.upsieutoc.com/api/1/upload", "c692ca0925f8da5990e8c795602bf942"),
            new CheveretoUploader("http://www.storemypic.com/api/1/upload", "995269492c2a19902715d5cc3ed810fa"),
            new CheveretoUploader("https://i.tlthings.net/api/1/upload", "a7yk23ty0k13ralyh32p64hx22p7ek49tt"),
            new CheveretoUploader("https://picuza.com/api/1/upload", "f613c791e4fc79ada8ec629a9ac34d90"),
            new CheveretoUploader("https://bobblepic.com/api/1/upload", "5f3d45874194ad6a6e8c7400932b824f"),
            new CheveretoUploader("http://zimg.se/api/1/upload", "25a772decbbe4381378880c5712d4ae6"),
            new CheveretoUploader("http://forumbilder.com/api/1/upload", "7d67072c194531c3c9343f0e0eb48a54"),
            new CheveretoUploader("http://www.img-load.de/api/1/upload", "980ac733e6d272b5d7a6ee16afb753aa"),
            new CheveretoUploader("http://imgchr.com/api/1/upload", "a788873e81d83019f4807c87145ddf1f"),
            new CheveretoUploader("http://images.the-hive.fr/api/1/upload", "85c20c20105e5b741c11108498d5403b"),
            new CheveretoUploader("https://www.ezphotoshare.com/api/1/upload", "15388b54636e96e3e63e5e86d6b72271"),
            new CheveretoUploader("http://imgmax.com/api/1/upload", "1fa647781a3fb7aa91bd70eb4824a6c3"),
            new CheveretoUploader("http://upgfx.com/api/1/upload", "207faeeb604b9d25ec7a8d94029d4289"),
            new CheveretoUploader("http://frimge.com/api/1/upload", "6f71a24683cec81d14fc9ce73a0c7f60"),
            new CheveretoUploader("https://gifyu.com/api/1/upload", "9aa9c4dedd20aeb9a63e41676e061820"),
            new CheveretoUploader("http://wampi.ru/api/1/upload", "4ba6e1be69dc94c7c8b4039e277d18fc"),
            new CheveretoUploader("http://imgbros.com/api/1/upload", "58b234bafd8011b0afd3ea72cec0ba4f"),
            new CheveretoUploader("http://cuntuku.com/api/1/upload", "584bf3b4398f4e01f695cc0c50253110"),
            new CheveretoUploader("https://freshpic.xyz/api/1/upload", "465e94ae768c585e377314d322d690aa"),
            new CheveretoUploader("https://corgi.party/api/v3/sharex", "c91dbf81cbd8aa797a1d12e00822cfa7"),
            new CheveretoUploader("https://imgyukle.com/api/1/upload", "407289b6f603c950af54fbc79311b9b0"),
            new CheveretoUploader("https://imges.link/sharexapi/1/upload", "nCuBPgNYnHjheiyuXtnH77LrERQrLK44vDrY6HFG"),
            new CheveretoUploader("https://picsriver.com/api/1/upload", "cf4a1a08a577b6bcf2be2565918c00bd"),
            new CheveretoUploader("http://ap.imagensbrasil.org/api/1/upload", "9c9dfe77cd3bdbaa7220c6bbaf7452e7"),
            new CheveretoUploader("https://imgpile.com/api/1/upload", "4a2edc0efeb1596c662d69905674d025"),
            new CheveretoUploader("https://sekil.az/api/1/upload", "ef32f5599866c115b858d2246e2535ff"),
            new CheveretoUploader("http://imeggo.com/api/1/upload", "61b66b1ebe8b1dff4eaf3b371f150199")
        };

        public CheveretoUploader Uploader { get; private set; }

        public bool DirectURL { get; set; }

        public Chevereto(CheveretoUploader uploader)
        {
            Uploader = uploader;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("key", Uploader.APIKey);
            args.Add("format", "json");

            string url = URLHelpers.FixPrefix(Uploader.UploadURL);

            UploadResult result = SendRequestFile(url, stream, fileName, "source", args);

            if (result.IsSuccess)
            {
                CheveretoResponse response = JsonConvert.DeserializeObject<CheveretoResponse>(result.Response);

                if (response != null && response.Image != null)
                {
                    result.URL = DirectURL ? response.Image.URL : response.Image.URL_Viewer;

                    if (response.Image.Thumb != null)
                    {
                        result.ThumbnailURL = response.Image.Thumb.URL;
                    }
                }
            }

            return result;
        }

        public static string TestUploaders()
        {
            List<CheveretoTest> successful = new List<CheveretoTest>();
            List<CheveretoTest> failed = new List<CheveretoTest>();

            using (MemoryStream ms = new MemoryStream())
            {
                using (Image logo = ShareXResources.Logo)
                {
                    logo.Save(ms, ImageFormat.Png);
                }

                foreach (CheveretoUploader uploader in Uploaders)
                {
                    try
                    {
                        Chevereto chevereto = new Chevereto(uploader);
                        string filename = Helpers.GetRandomAlphanumeric(10) + ".png";

                        Stopwatch timer = Stopwatch.StartNew();
                        UploadResult result = chevereto.Upload(ms, filename);
                        long uploadTime = timer.ElapsedMilliseconds;

                        if (result != null && result.IsSuccess && !string.IsNullOrEmpty(result.URL))
                        {
                            successful.Add(new CheveretoTest { Name = uploader.ToString(), UploadTime = uploadTime });
                        }
                        else
                        {
                            failed.Add(new CheveretoTest { Name = uploader.ToString() });
                        }
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
                        failed.Add(new CheveretoTest { Name = uploader.ToString() });
                    }
                }
            }

            return string.Format("Successful uploads ({0}):\r\n\r\n{1}\r\n\r\nFailed uploads ({2}):\r\n\r\n{3}",
                successful.Count, string.Join("\r\n", successful.OrderBy(x => x.UploadTime)), failed.Count, string.Join("\r\n", failed));
        }

        private class CheveretoResponse
        {
            public CheveretoImage Image { get; set; }
        }

        private class CheveretoImage
        {
            public string URL { get; set; }
            public string URL_Viewer { get; set; }
            public CheveretoThumb Thumb { get; set; }
        }

        private class CheveretoThumb
        {
            public string URL { get; set; }
        }

        private class CheveretoTest
        {
            public string Name { get; set; }
            public long UploadTime { get; set; } = -1;

            public override string ToString()
            {
                if (UploadTime >= 0)
                {
                    return $"{Name} ({UploadTime}ms)";
                }

                return Name;
            }
        }
    }
}