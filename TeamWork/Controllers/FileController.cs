using STIM.Modules.Uploading;
using STIM.Modules.Downloading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STIM.Web;
using System.Data.Entity.Validation;
using STIM.Web.Security;
using STIM.Modules.Personalize;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace TeamWork.Controllers
{
    public class FileController : FileBaseController
    {
        //
        // GET: /File/

        private static string ICON_UPLOAD_PATH = "/Content/icons";
        private static string LOGIN_IMAGE_UPLOAD_PATH = "/Content/images";

        [AllowAnonymous]
        public ActionResult MenuIconUpload()
        {
            return SaveUploadFile(ICON_UPLOAD_PATH, false);
        }

        [AllowAnonymous]
        public ActionResult LoginImageUpload()
        {
            return SaveUploadFile(LOGIN_IMAGE_UPLOAD_PATH, false);
        }

        [HttpPost]
        public ActionResult UserProtraitUpload()
        {
            HttpUploadProcessor processor = new HttpUploadProcessor();
            processor.FileUploadedHandler = SaveUserProtrait;
            return SaveUploadFile(LOGIN_IMAGE_UPLOAD_PATH, processor, false);
        }

        #region 用户头像上传

        private void SaveUserProtrait(string filePath)
        {
            FileInfo imageFile = new FileInfo(filePath);
            if (imageFile.Exists)
            {
                IUserData user = UserContext.CurrentUser;
                PersonalizeSettings settings = user.PersonalizeSettings();

                int size = 128;
                using (Image smallImage = ZoomOut(ImageCut(Image.FromFile(imageFile.FullName)), size, size))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        smallImage.Save(ms, ImageFormat.Png);
                        settings.Portrait = ms.ToArray();
                    }
                }
                imageFile.Delete();
                user.UpdatePersonalize(settings);
            }
        }

        static Bitmap ImageCut(Image image)
        {
            if (image == null)
            {
                return null;
            }

            int w = image.Width;
            int h = image.Height;
            int top = 0;
            int left = 0;
            int width = 0;
            int height = 0;
            if (w > h)
            {
                left = (w - h) / 2;
                width = height = h;
            }
            else
            {
                top = (h - w) / 2;
                width = height = w;
            }

            Bitmap bmpOut = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmpOut))
            {
                g.DrawImage(image,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(left, top, width, height),
                    GraphicsUnit.Pixel);
                image.Dispose();
            }
            return bmpOut;
        }

        static Bitmap ZoomOut(Image image, int width, int height)
        {
            Bitmap newImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.Default;
                g.DrawImage(
                    image,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height),
                    GraphicsUnit.Pixel);
                image.Dispose();
            }
            return newImage;
        }

        #endregion

        //下载文件
        public ActionResult Download(string id)
        {
            //Guid fileId;
            //if (!Guid.TryParse(id, out fileId))
            //{
            //    throw new HttpException(404, "文件Id错误！");
            //}

            //FileUploadLogic fl = new FileUploadLogic();
            //UploadFile file = fl.GetFileById(fileId);
            //if (file == null)
            //{
            //    throw new HttpException(404, "文件信息不存在！");
            //}
            //FileInfo fileInfo = fl.GetFile(file.FilePath);
            //if (!fileInfo.Exists)
            //{
            //    throw new HttpException(404, "文件不存在！");
            //}

            //IHttpDownloadService downloader = new HttpSingleThreadDownloadService();
            //downloader.ResponseFile(Request, Response, fileInfo);

            return new EmptyResult();
        }


        #region Helpers

        protected ActionResult SaveUploadFile(string path, bool rename = true)
        {
            return SaveUploadFile(path, null, rename);
        }

        protected ActionResult SaveUploadFile(string path, HttpUploadProcessor processor, bool rename)
        {
            UploadEventArgs fileArg = null;

            if (processor == null)
                processor = new HttpUploadProcessor();
            if (processor.IsAjaxUpload(Request))
            {
                try
                {
                    fileArg = processor.SaveAjaxUploadFile(Request, path, rename);
                    return Json(fileArg);
                }
                catch (Exception e)
                {
                    return GetJsonErrorMessage(e);
                }
            }
            else
            {
                try
                {
                    fileArg = processor.SaveFormUploadFile(Request, ICON_UPLOAD_PATH, rename);
                }
                catch (Exception e)
                {
                    SetErrorMessageToViewBag(e);
                }
                return UploadResult(fileArg);
            }
        }

        #endregion
    }
}
