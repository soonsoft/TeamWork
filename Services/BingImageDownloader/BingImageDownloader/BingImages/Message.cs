using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BingImageDownloader.BingImages
{
    [Serializable]
    [DataContract]
    public class Message
    {
        private string _title;
        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _link;
        /// <summary>
        /// 信息链接
        /// </summary>
        [DataMember]
        public string Link
        {
            get { return _link; }
            set { _link = value; }
        }

        private string _text;
        /// <summary>
        /// 信息文本描述
        /// </summary>
        [DataMember]
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}
