using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BingImageDownloader.BingImages
{
    [Serializable]
    [DataContract]
    public class Hotspot
    {
        private string _discription;
        /// <summary>
        /// 描述信息
        /// </summary>
        [DataMember]
        public string Discription
        {
            get { return _discription; }
            set { _discription = value; }
        }

        private string _link;
        /// <summary>
        /// 搜索链接
        /// </summary>
        [DataMember]
        public string Link
        {
            get { return _link; }
            set { _link = value; }
        }

        private string _query;
        /// <summary>
        /// 问题信息
        /// </summary>
        [DataMember]
        public string Query
        {
            get { return _query; }
            set { _query = value; }
        }

        private int _locationX;
        /// <summary>
        /// 对应图片左上角的位置
        /// </summary>
        [DataMember]
        public int LocationX
        {
            get { return _locationX; }
            set { _locationX = value; }
        }

        private int _locationY;
        /// <summary>
        /// 对应图片右上角的位置
        /// </summary>
        [DataMember]
        public int LocationY
        {
            get { return _locationY; }
            set { _locationY = value; }
        }
    }
}
