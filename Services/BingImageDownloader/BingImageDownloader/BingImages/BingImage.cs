using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace BingImageDownloader.BingImages
{
    [Serializable]
    [DataContract]
    public class BingImage
    {
        private DateTime _startDate;
        [DataMember]
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        private DateTime _fullStartDate;
        [DataMember]
        public DateTime FullStartDate
        {
            get { return _fullStartDate; }
            set { _fullStartDate = value; }
        }

        private DateTime _endDate;
        [DataMember]
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        private string _bingHost;
        [DataMember]
        public string BingHost
        {
            get { return _bingHost; }
            set { _bingHost = value; }
        }

        private string _imageUrl;
        [DataMember]
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        private string _urlBase;
        [DataMember]
        public string UrlBase
        {
            get { return _urlBase; }
            set { _urlBase = value; }
        }

        private string _copyright;
        [DataMember]
        public string Copyright
        {
            get { return _copyright; }
            set { _copyright = value; }
        }

        private string _copyrightLink;
        [DataMember]
        public string CopyrightLink
        {
            get { return _copyrightLink; }
            set { _copyrightLink = value; }
        }

        private string _drk;
        [DataMember]
        public string Drk
        {
            get { return _drk; }
            set { _drk = value; }
        }

        private string _top;
        [DataMember]
        public string Top
        {
            get { return _top; }
            set { _top = value; }
        }

        private string _bot;
        [DataMember]
        public string Bot
        {
            get { return _bot; }
            set { _bot = value; }
        }

        private List<Hotspot> _hotspots;
        /// <summary>
        /// 图片信息
        /// </summary>
        [DataMember]
        public List<Hotspot> Hotspots
        {
            get { return _hotspots; }
            set { _hotspots = value; }
        }

        private List<Message> _messages;
        /// <summary>
        /// 图片消息
        /// </summary>
        public List<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        private Bitmap _image;
        /// <summary>
        /// bing图片
        /// </summary>
        public Bitmap Image
        {
            get { return _image; }
            internal set { _image = value; }
        }

        public BingImage()
        {
            _hotspots = new List<Hotspot>(4);
            _messages = new List<Message>(3);
        }
    }
}
