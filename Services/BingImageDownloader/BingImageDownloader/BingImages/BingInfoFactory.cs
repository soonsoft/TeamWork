using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace BingImageDownloader.BingImages
{
    public static class BingInfoFactory
    {
        private static Regex queryTextRegex = new Regex(@"q=([^&]*)");

        public static List<BingImage> LoadXMLToImages(XDocument dom)
        {
            XElement image = dom.Element("images");
            var images = image.Elements("image");
            List<BingImage> bingImages = new List<BingImage>();
            foreach (var item in images)
            {
                bingImages.Add(CreateBingImage(item));
            }
            return bingImages;
        }

        public static BingImage CreateBingImage(XElement image)
        {
            BingImage bingImg = new BingImage();

            // 获取图片的基本属性
            bingImg.StartDate = DateTime.ParseExact(image.Element("startdate").Value, "yyyyMMdd", null);
            bingImg.FullStartDate = DateTime.ParseExact(image.Element("fullstartdate").Value, "yyyyMMddHHmm", null);
            bingImg.EndDate = DateTime.ParseExact(image.Element("enddate").Value, "yyyyMMdd", null);
            bingImg.ImageUrl = image.Element("url").Value.Trim('/');
            bingImg.UrlBase = image.Element("urlBase").Value.Trim('/');
            bingImg.Copyright = image.Element("copyright").Value;
            bingImg.Drk = image.Element("drk").Value;
            bingImg.Top = image.Element("top").Value;
            bingImg.Bot = image.Element("bot").Value;

            // 获取图片热点信息
            XElement element = image.Element("hotspots");
            if(element != null)
            {
                var hotSports = element.Elements("hotspot");
                foreach (var hotSport in hotSports)
                {
                    bingImg.Hotspots.Add(CreateHotspot(hotSport));
                }
            }

            // 获取图片相关信息
            element = image.Element("messages");
            if(element != null)
            {
                var messages = element.Elements("message");
                foreach (var message in messages)
                {
                    bingImg.Messages.Add(CreateMessage(message));
                }
            }

            return bingImg;
        }

        public static Hotspot CreateHotspot(XElement hotspot)
        {
            Hotspot hs = new Hotspot();

            hs.Discription = hotspot.Element("desc").Value;
            hs.Link = hotspot.Element("link").Value;
            if (!String.IsNullOrEmpty(hs.Link))
            {
                hs.Link = queryTextRegex.Replace(hs.Link, EncodeQueryTextInLink);
            }
            hs.Query = hotspot.Element("query").Value;
            hs.LocationX = Convert.ToInt32(hotspot.Element("LocX").Value);
            hs.LocationY = Convert.ToInt32(hotspot.Element("LocY").Value);

            return hs;
        }

        public static Message CreateMessage(XElement message)
        {
            Message msg = new Message();

            msg.Title = message.Element("msgtitle").Value;
            msg.Link = message.Element("msglink").Value;
            msg.Text = message.Element("msgtext").Value;

            return msg;
        }

        static string EncodeQueryTextInLink(Match match)
        {
            string[] textArr = match.Value.Split('=');
            string text = null;
            if (textArr.Length >= 2)
            {
                text = textArr[1];
            }
            if (String.IsNullOrEmpty(text))
            {
                return match.Value;
            }
            return String.Concat("q=", HttpUtility.UrlEncode(text));
        }
    }
}
