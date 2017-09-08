#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2015-2016 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;
using System;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using ShareX.UploadersLib.OtherServices;

namespace ShareX.UploadersLib.FileUploaders
{
    public class ChuangYiBaoFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.ChuangYiBao;

        public override Icon ServiceIcon => Resources.ChuangYiBao;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.ChuangYiBaoSettings != null && !string.IsNullOrEmpty(config.ChuangYiBaoSettings.Username) && !string.IsNullOrEmpty(config.ChuangYiBaoSettings.Password) && !string.IsNullOrEmpty(config.ChuangYiBaoSettings.Auth_token) ;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new ChuangYiBaoUploader(APIKeys.ChuangYiBaoKey, config);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpChuangYiBao;
    }

    public sealed class ChuangYiBaoUploader : FileUploader
    {
        public const int TYPE_DOCUMENT = 1;// 文件
        public const int TYPE_PIC = 2; // 图片
        public const int TYPE_AUDIO = 3;// 声音
        public const int TYPE_VIDEO = 4;// 视频
        public const int TYPE_TXT = 6; // 文本
        public const int TYPE_PICS = 7; // 多图片
        public const int TYPE_EML = 8; //  邮件

        private string URLAPI = "";
        private string EndPoint = "";

        public const string RegisterURL = "/business/personal/register.do";
        public const string ResetPasswordURL = "/business/personal/forgetPwd.do";

        public ChuangYiBaoOptions Config { get; set; }

        private string APIKey;

        public ChuangYiBaoUploader(string developerKey, UploadersConfig config)
        {
            APIKey = developerKey;
            Config = config.ChuangYiBaoSettings;
            URLAPI = config.ChuangYiBaoDomain;
            EndPoint = config.ChuangYiBaoEndPoint;
        }
        private static char IntToChar(int x)
        {
            if (x < 10) return (char)(x + '0');
            return (char)(x - 10 + 'a');
        }

        /* - - - - - - - - - - - - - - - - - - - - - - - -  
         * Stream 和 byte[] 之间的转换 
         * - - - - - - - - - - - - - - - - - - - - - - - */
        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        private static string BytesToString(byte[] b)
        {
            char[] res = new char[b.Length * 2];
            for (int i = 0; i < b.Length; ++i)
            {
                res[2 * i] = IntToChar(b[i] >> 4);
                res[2 * i + 1] = IntToChar(b[i] & 0xf);
            }
            return new string(res);
        }

        public string GetMd5(string signatureStr)
        {
            byte[] signatureBytes = Encoding.ASCII.GetBytes(signatureStr);
            return getMD5(signatureBytes);
        }

        private string getMD5(byte[] signatureBytes)
        {
            MD5 md5gen = MD5.Create();
            byte[] md5Bytes = md5gen.ComputeHash(signatureBytes);
            return BytesToString(md5Bytes);
        }

        /**
         * 获取阿里云的配置信息
         *
         * @param responseHandler
         */
        public ChuangYiBaoOssStorage getOSSInfo()
        {
            ChuangYiBaoOssStorage bucket = null;
            String url = "/publicPhoneAction!getSystemConfig.do";
            Dictionary<string, string> args = new Dictionary<string, string>();
            string response = SendRequest(HttpMethod.POST, URLAPI + url, args,prepareHeader());
            if (!string.IsNullOrEmpty(response))
            {
                ChuangYiBaoOssStorageResponse resp = JsonConvert.DeserializeObject<ChuangYiBaoOssStorageResponse>(response);

                if (resp != null && resp.data != null && !string.IsNullOrEmpty(resp.data.bucketName))
                {
                    bucket = resp.data;
                }
            }
            return bucket;
        }

        /**
         * 获取阿里云的Token
         */
        public ChuangYiBaoTokenResponse getOSSToken()
        {
            ChuangYiBaoTokenResponse token = null;
            String url = "/sts/stsAction!getToken.do";
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("userName", Config.Username);
            args.Add("password", GetMd5(ChuangYiBaoTripleDESCryptoService.Decrypt(Config.Password)));
            String uuid = Guid.NewGuid().ToString();//PhoneInfoUtil.getDeviceId(App.getAppContext());
            args.Add("uuid", uuid);
            string response = SendRequest(HttpMethod.GET,URLAPI+url, args,prepareHeader());
            if (!string.IsNullOrEmpty(response))
            {
                ChuangYiBaoTokenResponse resp = JsonConvert.DeserializeObject<ChuangYiBaoTokenResponse> (response);

                if (resp != null && !string.IsNullOrEmpty(resp.securityToken))
                {
                    token = resp;
                }
            }

            return token;
        }

        /**
         * 判断空间大小
         * @param fileSize
         */
        public ChuangYiBaoResponse checkSpaceSize(float fileSize)
        {
            ChuangYiBaoResponse resp = null;
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("fileSize", fileSize.ToString());  //文件大小

            String url = "/cspaceAction!spaceOverplus.do";
            string response = SendRequest(HttpMethod.POST, URLAPI + url, args, prepareHeader());
            if (!string.IsNullOrEmpty(response))
            {
                resp = JsonConvert.DeserializeObject<ChuangYiBaoResponse>(response);
            }
            else
            {
                resp = new ChuangYiBaoResponse();
                resp.status = -1;
                resp.msg = "判断空间大小出错";
            }

            return resp;
        }

        private String putOSSObject(Stream stream, string fileName, ChuangYiBaoOssStorage oss, ChuangYiBaoTokenResponse token)
        {

            // 创建ClientConfiguration实例
            ClientConfiguration conf = new ClientConfiguration();

            // 配置使用Cname
            conf.IsCname = false;

            // 设置网络参数
            //我们可以用ClientConfiguration设置一些网络参数：
            //conf.MaxErrorRetry = 3;     //设置请求发生错误时最大的重试次数
            //conf.ConnectionTimeout = 300;  //设置连接超时时间
            conf.SetCustomEpochTicks(oss.timestamp);        //设置自定义基准时间

            /// <summary>
            /// 由用户指定的OSS访问地址、、阿里云颁发的AccessKeyId/AccessKeySecret、客户端配置
            /// 构造一个新的OssClient实例。
            /// </summary>
            /// <param name="endpoint">OSS的访问地址。</param>
            /// <param name="accessKeyId">OSS的访问ID。</param>
            /// <param name="accessKeySecret">OSS的访问密钥。</param>
            /// <param name="conf">客户端配置。</param>
            OssClient client = new OssClient(new Uri(EndPoint), token.accessKeyId, token.accessKeySecret, token.securityToken, conf);

            ObjectMetadata objectMeta = new ObjectMetadata();
            objectMeta.ContentLength = stream.Length;
            // 可以在metadata中标记文件类型
            objectMeta.ContentType = "";
            // 对object进行服务器端加密，目前服务器端只支持x-oss-server-side-encryption加密
            objectMeta.AddHeader("x-oss-server-side-encryption", "AES256");

            String filekey = "windows/" + Config.Auth_token + "/" + DateTime.Now.ToString("yyyyMMdd") + "/" + fileName;
            PutObjectResult putObjectResult = client.PutObject(oss.bucketName, filekey, stream, objectMeta);

            return filekey;
        }

        /**
        * 登陆
        *
        * @param userName
        * @param pwd
        * @param responseHandler
        */
        public bool GetAccessToken()
        {
            string URLAccessToken = URLAPI + "/usermgrAction!login.do";
            if (!string.IsNullOrEmpty(Config.Username) && !string.IsNullOrEmpty(Config.Password))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("user_name", Config.Username);
                args.Add("pwd", GetMd5(ChuangYiBaoTripleDESCryptoService.Decrypt(Config.Password)));

                string response = SendRequest(HttpMethod.POST, URLAccessToken, args);

                if (!string.IsNullOrEmpty(response))
                {
                    ChuangYiBaoLoginResponse resp = JsonConvert.DeserializeObject<ChuangYiBaoLoginResponse>(response);

                    if (resp != null && resp.data != null && !string.IsNullOrEmpty(resp.data.id))
                    {
                        Config.Auth_token = resp.data.id;
                        return true;
                    }
                }
            }

            return false;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            ChuangYiBaoOssStorage oss = getOSSInfo();

            ChuangYiBaoTokenResponse token = getOSSToken();

            float fileSize = (float)stream.Length / 1024 / 1024;
            ChuangYiBaoResponse hasSpace = checkSpaceSize(fileSize);

            UploadResult result = null;
            string fileKey = "";
            int fileType = 0;
            if (hasSpace.status == 1)
            {
                fileKey = putOSSObject(stream, fileName, oss, token);

                fileType = GetFileType(fileName);

                if (Config.IsEvidence)
                {
                    result = addEvidence(stream, fileName, fileKey, fileType, fileSize);
                }
                else
                {
                    result = addCreation(stream, fileName, fileKey, fileType, fileSize);
                }
                
            }
            else
            {
                result = new UploadResult();
                result.IsSuccess = false;
                result.Errors = new List<string>();
                result.Errors.Add(hasSpace.msg);
            }
            if (!string.IsNullOrEmpty(result.Response))
            {
                JToken jsonResponse = JToken.Parse(result.Response);

                if (jsonResponse != null)
                {
                    bool isSuccess = jsonResponse["status"].Value<int>() == 1;

                    if (isSuccess)
                    {
                        ChuangYiBaoUploadResult uploadResult = jsonResponse["data"].ToObject<ChuangYiBaoUploadResult>();

                        if (uploadResult != null && uploadResult.id != null && !string.IsNullOrEmpty(uploadResult.id) )
                        {
                            if(fileType == -2)
                            {
                                result.URL = "http://" + oss.cybstorage + "/" + fileKey;

                                StringBuilder sbThumbnailURL = new StringBuilder("http://" + oss.imgstorage + "/" + fileKey);
                                sbThumbnailURL.Append("?");
                                sbThumbnailURL.Append("OSSAccessKeyId="+token.accessKeyId);
                                sbThumbnailURL.Append("&");
                                sbThumbnailURL.Append("Expires=" + token.expiration);
                                sbThumbnailURL.Append("&");
                                sbThumbnailURL.Append("Signature=" + "f3vPCvzZ0yFk0X9+H1tD8s40+ug=");
                                sbThumbnailURL.Append("&");
                                sbThumbnailURL.Append("security-token=" + token.securityToken);
 
                                result.ThumbnailURL = sbThumbnailURL.ToString();
                            }
                            else
                            {
                                if (Config.IsEvidence)
                                {
                                    //business/personal/evidence/v_list.do
                                    result.URL = URLAPI + string.Format("/business/personal/evidence/v_info.do?id={0}", uploadResult.id);
                                    result.ThumbnailURL = URLAPI + string.Format("business/personal/evidence/v_info.do?id={0}", uploadResult.id);
                                }
                                else
                                {
                                    result.URL = URLAPI + string.Format("/business/personal/creation/v_info.do?id={0}", uploadResult.id);
                                    result.ThumbnailURL = URLAPI + string.Format("/business/personal/creation/v_info.do?id={0}", uploadResult.id);
                                }
                            }
                        }
                    }
                    else
                    {
                        ChuangYiBaoErrorInfo errorInfo = jsonResponse["msg"].ToObject<ChuangYiBaoErrorInfo>();
                        Errors.Add(errorInfo.ToString());
                    }
                }
            }

            return result;
        }

        private ChuangYiBaoUploadResult getCreation(string id)
        {
            ChuangYiBaoUploadResult resp = null;
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("id", id);  //创意ID

            String url = "/cspaceAction!spaceOverplus.do";
            string response = SendRequest(HttpMethod.POST, URLAPI + url, args, prepareHeader());
            if (!string.IsNullOrEmpty(response))
            {

            }
            else
            {

            }

            return resp;
        }

        private UploadResult addCreation(Stream stream, string fileName, string fileKey,int fileType,float fileSize)
        {
            UploadResult result;
            string url = "/ccreationAction!addCreation.do";
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            arguments.Add("seconds", "0");
            arguments.Add("secretstate", "2");
            arguments.Add("name", fileName.Remove(fileName.LastIndexOf(".")));
            arguments.Add("location", fileKey);
            arguments.Add("userid", Config.Auth_token);
            arguments.Add("type", fileType.ToString());
            arguments.Add("size", fileSize.ToString());
            arguments.Add("md5", getMD5(StreamToBytes(stream)));

            result = UploadData(stream, URLAPI + url, fileName, "file", arguments, prepareHeader());
            return result;
        }

        private UploadResult addEvidence(Stream stream, string fileName, string fileKey, int fileType, float fileSize)
        {
            UploadResult result;
            string url = "/business/personal/evidence/o_add.do";
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            arguments.Add("state", "0");
            arguments.Add("recordDate", "0");
            arguments.Add("localtion", fileKey);
            arguments.Add("fileType", fileType.ToString());
            arguments.Add("fileSize", fileSize.ToString());
            arguments.Add("fileMd5", getMD5(StreamToBytes(stream)));
            arguments.Add("evidenceName", fileName.Remove(fileName.LastIndexOf(".")));
            arguments.Add("czFlag", "1");

            result = UploadData(stream, URLAPI + url, fileName, "file", arguments, prepareHeader());
            return result;
        }

        private String GetFileExtension(String fileName)
        {
            return fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf("."));
        }

        private int GetFileType(String fileName)
        {
            int fileType;
            String fileExtension = GetFileExtension(fileName);
            if (".pdf" == fileExtension || ".doc" == fileExtension || ".docx" == fileExtension)
            {
                fileType = TYPE_DOCUMENT;
            }
            else if (".jpg" == fileExtension || ".jpeg" == fileExtension || ".png" == fileExtension || ".gif" == fileExtension)
            {
                fileType = TYPE_PIC;
            }
            else if (".mp3" == fileExtension || ".amr" == fileExtension)
            {
                fileType = TYPE_AUDIO;
            }
            else if (".mp4" == fileExtension || ".avi" == fileExtension || ".rmvb" == fileExtension || ".avi" == fileExtension)
            {
                fileType = TYPE_VIDEO;
            }
            else if(".text" == fileExtension || ".txt" == fileExtension)
            {
                fileType = TYPE_TXT;
            }
            else
            {
                fileType = -1;
            }

            return fileType;
        }

        private NameValueCollection prepareHeader()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("channel", "yuanchuangyunandroid");
            headers.Add("productId", "2.1.6");
            headers.Add("UserAgent", "android");
            headers.Add("userId", Config.Auth_token);
            return headers;
        }

        public class ChuangYiBaoErrorInfo
        {
            public int error_code { get; set; }
            public string error_message { get; set; }

            public override string ToString()
            {
                return string.Format("Error message: {0}\r\nError code: {1}", error_message, error_code);
            }
        }

        public class ChuangYiBaoResponse
        {
            public int status { get; set; }
            public String msg { get; set; }
        }

        public class ChuangYiBaoTokenResponse : ChuangYiBaoResponse
        {
            public long expiration { get; set; }
            public string securityToken { get; set; }
            public string accessKeyId { get; set; }
            public string accessKeySecret { get; set; }
        }

        public class ChuangYiBaoOssStorageResponse : ChuangYiBaoResponse
        {
            public ChuangYiBaoOssStorage data { get; set; }
        }

        public class ChuangYiBaoOssStorage
        {
            public long timestamp { get; set; }
            public string imgstorage { get; set; }
            public string cybstorage { get; set; }
            public string bucketName { get; set; }
    }

        public class ChuangYiBaoLoginResponse : ChuangYiBaoResponse
        {
            public ChuangYiBaoLogin data{ get; set; }
        }

        public class ChuangYiBaoLogin
        {
            public string auth_token { get; set; }
            public string id { get; set; }
            public string email { get; set; }
            public string userName { get; set; }
            public ChuangYiBaoUserAvatar avatar { get; set; }
            public string membership { get; set; }
            public string membership_item_number { get; set; }
            public string membership_cookie { get; set; }
        }

        public class ChuangYiBaoUser
        {
            public bool is_owner { get; set; }
            public int cache_version { get; set; }
            public string username { get; set; }
            public string description { get; set; }
            public int creation_date { get; set; }
            public string location { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public ChuangYiBaoUserAvatar Avatar { get; set; }
        }

        public class ChuangYiBaoUserAvatar
        {
            public int image_id { get; set; }
            public int server { get; set; }
            public string filename { get; set; }
        }

        public class ChuangYiBaoUploadResponse : ChuangYiBaoResponse
        {
            public ChuangYiBaoUploadResult data { get; set; }
        }

        public class ChuangYiBaoUploadResult
        {
            public string cryptstate { get; set; }
            public string ctime { get; set; }
            public string etime { get; set; }
            public string folderName { get; set; }
            public string folderid { get; set; }
            public string foldername { get; set; }
            public string id { get; set; }
            public string location { get; set; }
            public string locationAll { get; set; }
            public string md5 { get; set; }
            public string name { get; set; }
            public string paytype { get; set; }
            public string privatekey { get; set; }
            public string remark { get; set; }
            public string rtime { get; set; }
            public string seconds { get; set; }
            public string secretstate { get; set; }
            public string size { get; set; }
            public string state { get; set; }
            public string storageCard { get; set; }
            public string thumbnail { get; set; }
            public string thumbnailOri { get; set; }
            public string time_stamp { get; set; }
            public string time_stamp_public { get; set; }
            public string type { get; set; }
            public string userid { get; set; }
            public string utime { get; set; }
            public List<ChuangYiBaoImage> images { get; set; }
        }

        public class ChuangYiBaoImage
        {
            public string id { get; set; }
            public int server { get; set; }
            public int bucket { get; set; }
            public string lp_hash { get; set; }
            public string filename { get; set; }
            public string original_filename { get; set; }
            public string direct_link { get; set; }
            public object title { get; set; }
            public object description { get; set; }
            public List<string> tags { get; set; }
            public int likes { get; set; }
            public bool liked { get; set; }
            public int views { get; set; }
            public int comments_count { get; set; }
            public bool comments_disabled { get; set; }
            public int filter { get; set; }
            public int filesize { get; set; }
            public int creation_date { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public bool @public { get; set; }
            public bool is_owner { get; set; }
            public ChuangYiBaoUser owner { get; set; }
            public List<ChuangYiBaoImage> next_images { get; set; }
            public List<ChuangYiBaoImage> prev_images { get; set; }
            public object related_images { get; set; }
        }
    }

    public class ChuangYiBaoOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEvidence { get; set; }
        [JsonIgnore]
        public string Auth_token { get; set; }
        public int ThumbnailWidth { get; set; } = 256;
        public int ThumbnailHeight { get; set; }
    }
}