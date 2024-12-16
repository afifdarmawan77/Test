using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using System.Security.Cryptography;
using DOT_Test.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace DOT_Test.Libraries
{
    public class Helper : IHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataProtector dataProtector;
        public IConfiguration Configuration { get; }

        public Helper(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IDataProtectionProvider provider, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            dataProtector = provider.CreateProtector(GetType().FullName);
            Configuration = configuration;
        }

        public ApplicationUser GetUser(ClaimsPrincipal User)
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            return user;
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal User)
        {
            var email = User.Identities.First().Claims.FirstOrDefault()?.Value;
            return (!string.IsNullOrEmpty(email)) ? await _context.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Email == email) : null;
        }

        public async Task<string> GetRoleUser(ClaimsPrincipal User)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }


        public string QueryProtect(string QueryString)
        {
            return dataProtector.Protect(QueryString);
        }

        public string QueryUnprotect(string QueryString)
        {
            return dataProtector.Unprotect(QueryString);
        }

        public string QueryEncode(string QueryString)
        {
            return HttpUtility.UrlEncode(QueryString);
        }

        public string QueryDecode(string EncodedQueryString)
        {
            return HttpUtility.UrlDecode(EncodedQueryString);
        }

        public string SerializeJson(dynamic data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public dynamic DeserializeJson(string stringJson)
        {
            return JsonConvert.DeserializeObject(stringJson);
        }

        public string Nl2Br(string src)
        {
            return src?.Replace("\r\n", "<br />").Replace("\n", "<br />");
        }

        public async Task<string> UploadImage(int id, IFormFile file, string directory)
        {
            var url = string.Empty;

            try
            {
                if (file != null && file.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], directory, id.ToString());

                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }

                    string fileName = $"{Path.GetRandomFileName().Replace(".", string.Empty)}.{file.ContentType.Split('/')[1]}";

                    url = Path.Combine(Configuration["UploadFolder"], directory, id.ToString(), fileName).Replace("\\", "/");

                    fileName = Path.Combine(path, fileName);
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch
            {
                url = string.Empty;
            }

            return url;
        }

        public async Task<string> UploadImageAvatar(string userid, IFormFile file)
        {
            var url = string.Empty;

            try
            {
                if (file != null && file.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], "Users", userid);

                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }

                    string fileName = $"{Path.GetRandomFileName().Replace(".", string.Empty)}.{file.ContentType.Split('/')[1]}";

                    url = Path.Combine(Configuration["UploadFolder"], "Users", userid, fileName).Replace("\\", "/");

                    fileName = Path.Combine(path, fileName);
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch
            {
                url = string.Empty;
            }

            return url;
        }

        public bool RemoveImage(int id, string file, string directory)
        {
            try
            {
                if (!string.IsNullOrEmpty(file))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], directory, id.ToString());
                    var replacer = "/";
                    if (Path.DirectorySeparatorChar == '\\')
                    {
                        replacer = "\\";
                    }
                    var fileName = Path.Combine(path, file.Replace("/", replacer).Split(Path.DirectorySeparatorChar).Last());
                    if (System.IO.File.Exists(fileName)) System.IO.File.Delete(fileName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveImageAvatar(string userid, string file)
        {
            try
            {
                if (!string.IsNullOrEmpty(file))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], "Users", userid);
                    var replacer = "/";
                    if (Path.DirectorySeparatorChar == '\\')
                    {
                        replacer = "\\";
                    }
                    var fileName = Path.Combine(path, file.Replace("/", replacer).Split(Path.DirectorySeparatorChar).Last());
                    if (System.IO.File.Exists(fileName)) System.IO.File.Delete(fileName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendNotification(string _title, string id, string deviceId, string clickAction = "DayOff", string body = "Update Cuti")
        {
            bool result = true;
            try
            {
                string serverKey = "AAAAE6XzPhI:APA91bFcnhF7VR4aYE0F_FEnmp0XYR44G0O-vzTJTkoUodhAxcPKPFdMxeKfRZtmbvaNs1Sfs0AovHGLt0Zu7gT9g2hfTwdEBwcSLe7EMUBT3aHJAzyu_Cb43GpCnpLJNut2kNOGkJnW";
                string senderId = "84388560402";
                var requestUri = "https://fcm.googleapis.com/fcm/send";
                WebRequest webRequest = WebRequest.Create(requestUri);
                webRequest.Method = "POST";
                webRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                webRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                webRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    priority = "high",
                    notification = new
                    {
                        title = _title,
                        body = body,
                        click_action = clickAction,
                        show_in_foreground = "True",
                        icon = "myicon",
                        sound = "mysound",
                        android_channel_id = "cnid"
                    },
                    data = new
                    {
                        title = _title,
                        body = id,
                        click_action = clickAction,
                        show_in_foreground = "True",
                        icon = "myicon",
                        sound = "mysound",
                        android_channel_id = "cnid"
                    }

                };
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                webRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse webResponse = webRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = webResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public string changeFormatDate(String dateString)
        {

            DateTime parsedDateTime;
            string formattedDate = "";
            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out parsedDateTime))
            {
                formattedDate = parsedDateTime.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                Console.WriteLine("Parsing failed");
            }

            return formattedDate;

        }

        public string ConvertFileToBase64(String path)
        {

            string base64String = "";

            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        base64String = Convert.ToBase64String(imageBytes);
                        return base64String;
                    }
                }
            }
            catch (Exception e)
            {
                return base64String;
            }

            return base64String;

        }

        public string CopyFileTo(string SourceId, string DestinationId, string SourceUrl, string TemplatePath, bool IsTemporary)
        {
            try
            {
                if (!string.IsNullOrEmpty(SourceUrl))
                {
                    var fileName = Path.GetFileName(SourceUrl);

                    var sourcePath = "";
                    if (!IsTemporary)
                    {
                        sourcePath = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"],
                                            TemplatePath, "_temp", SourceId);
                    }
                    else
                    {
                        sourcePath = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"],
                                            TemplatePath, SourceId);
                    }

                    var sourceFileNameWithPath = Path.Combine(sourcePath, fileName);

                    var destinationPath = "";
                    if (IsTemporary)
                    {
                        destinationPath = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], TemplatePath,
                                                        "_temp", DestinationId);
                    }
                    else
                    {
                        destinationPath = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], TemplatePath,
                                                        DestinationId);
                    }

                    if (!Directory.Exists(destinationPath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(destinationPath);
                    }

                    var destinationFileNameWithPath = Path.Combine(destinationPath, fileName);

                    System.IO.File.Copy(sourceFileNameWithPath, destinationFileNameWithPath, true);

                    var url = "";
                    if (IsTemporary)
                    {
                        url = Path.Combine(Configuration["UploadFolder"], TemplatePath, "_temp", DestinationId, fileName).Replace("\\", "/");
                    }
                    else
                    {
                        url = Path.Combine(Configuration["UploadFolder"], TemplatePath, DestinationId, fileName).Replace("\\", "/");
                    }

                    return url;
                }

                return SourceUrl;
            }
            catch (Exception ex)
            {
                return SourceUrl;
            }
        }

        public bool RemoveFile(string UserID, string FilePath, string TemplatePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], TemplatePath, UserID.ToString());
                    var replacer = "/";
                    if (Path.DirectorySeparatorChar == '\\')
                    {
                        replacer = "\\";
                    }
                    var fileName = Path.Combine(path, FilePath.Replace("/", replacer).Split(Path.DirectorySeparatorChar).Last());
                    if (System.IO.File.Exists(fileName)) System.IO.File.Delete(fileName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GenericUploadFile(string Id, IFormFile File, string ResultUrl, string TemplatePath, bool IsTemporary)
        {
            try
            {
                var url = ResultUrl;

                if (File != null && File.Length > 0)
                {
                    var path = "";

                    if (IsTemporary)
                    {
                        path = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], TemplatePath, "_temp", Id);
                    }
                    else
                    {
                        path = Path.Combine(Directory.GetCurrentDirectory(), Configuration["StaticFolder"], TemplatePath, Id);
                    }

                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }

                    var extension = File.ContentType.Split('/')[1];
                    if (File.ContentType.Contains("vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                    {
                        extension = "xlsx";
                    }
                    if (File.ContentType.Contains("vnd.openxmlformats-officedocument.wordprocessingml.document"))
                    {
                        extension = "docx";
                    }
                    if (File.ContentType.Contains("vnd.ms-excel"))
                    {
                        extension = "xls";
                    }
                    if (File.ContentType.Contains("msword"))
                    {
                        extension = "doc";
                    }

                    //string fileNameOri = Path.GetFileNameWithoutExtension(ResultFile.FileName);

                    string fileName = "";
                    if (TemplatePath.ToLower() == "document")
                    {
                        fileName = $"{DateTime.Now.ToString("yyyyMMddHHmm")}_{Path.GetFileNameWithoutExtension(File.FileName)}.{extension}";
                    }

                    if (TemplatePath.ToLower() == "users")
                    {
                        fileName = $"{Path.GetRandomFileName().Replace(".", string.Empty)}.{extension}";
                    }

                    var fileNameWithPath = Path.Combine(path, fileName);

                    using (var fileStream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await File.CopyToAsync(fileStream);
                    }

                    if (IsTemporary)
                    {
                        url = Path.Combine(Configuration["UploadFolder"], TemplatePath, "_temp", Id, fileName).Replace("\\", "/");
                    }
                    else
                    {
                        url = Path.Combine(Configuration["UploadFolder"], TemplatePath, Id, fileName).Replace("\\", "/");
                    }


                }

                return url;
            }
            catch (Exception ex)
            {
                return ResultUrl;
                //throw new Exception(ex.Message);
            }
        }



        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary.ToLower());
        }

        public string DatetimeConverter(DateTime date)
        {
            DateTime datetimeNow = date;
            DateTime today = datetimeNow;
            var dayName = datetimeNow.ToString("dddd");
            //extract the month
            int daysInMonth = today.Day;
            DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
            //days of week starts by default as Sunday = 0
            int firstDayOfMonth = (int)firstOfMonth.DayOfWeek;
            int weeksInMonth = (int)Math.Ceiling((firstDayOfMonth + daysInMonth) / 7.0);

            string namaHari = dayName + weeksInMonth;
            return namaHari;
        }

    }
}
