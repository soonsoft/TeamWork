using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.Xml.Linq;

namespace BingImageDownloader.BingImages
{
    public class BingDownloader
    {
        private const string BING_ARCHIVE = "http://{0}/HPImageArchive.aspx?idx={1}&n={2}";

        private string _countryName;
        /// <summary>
        /// bing网站对应国家的名称
        /// </summary>
        public string CountryName
        {
            get { return _countryName; }
            set { _countryName = value; }
        }

        private string _bingHost;
        /// <summary>
        /// bing首页的地址
        /// </summary>
        public string BingHost
        {
            get { return _bingHost; }
            set { _bingHost = value; }
        }

        private string _bingImageUrl;
        /// <summary>
        /// bing首页图片的地址
        /// </summary>
        public string BingImageUrl
        {
            get { return _bingImageUrl; }
            set { _bingImageUrl = value; }
        }

        public BingDownloader()
        {
        }

        public BingDownloader(string bingUrl)
        {
            _bingHost = bingUrl;
        }

        private void CheckDayNum(ref int dayNum)
        {
            //获取哪一天的图片，默认是0（今天），最多只能是13天
            if (dayNum < 0) dayNum = 0;
            else if (dayNum > 13) dayNum = 13;
        }

        private void CheckImageNum(ref int imgNum)
        {
            //需要获取图片的数量最多只能获取8张
            if (imgNum < 1) imgNum = 1;
            else if (imgNum > 8) imgNum = 8;
        }

        public List<BingImage> GetImage()
        {
            return GetImage(0, 1);
        }

        /// <summary>
        /// 获取bing图片
        /// </summary>
        /// <param name="dayNum">从哪一天开始获取图片，0表示今天，1表示昨天以此类推</param>
        /// <param name="imgNum">获取图片的数量</param>
        /// <returns></returns>
        public List<BingImage> GetImage(int dayNum, int imgNum)
        {
            CheckDayNum(ref dayNum);
            CheckImageNum(ref imgNum);

            string url = String.Format(BING_ARCHIVE, _bingHost, dayNum, imgNum);

            List<BingImage> images = RequestBing(url);
            string imageUrl = "http://{0}/{1}";
            string imageUrlFHD = "http://{0}/{1}_1920x1080.jpg";
            foreach (BingImage image in images)
            {
                image.BingHost = _bingHost;
                url = String.Format(imageUrlFHD, _bingHost, image.UrlBase);
                RequestBingImage(url, image);
                if(image.Image == null)
                {
                    url = String.Format(imageUrl, _bingHost, image.ImageUrl);
                    RequestBingImage(url, image);
                }
            }
            return images;
        }

        static List<BingImage> RequestBing(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                XDocument dom = XDocument.Load(response.GetResponseStream());
                response.Close();
                response = null;

                return BingInfoFactory.LoadXMLToImages(dom);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }

        static void RequestBingImage(string url, BingImage image)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApplicationException("url未找到图片");
                }
                image.Image = new Bitmap(response.GetResponseStream());
            }
            catch
            {
                image = null;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }
    }
}
