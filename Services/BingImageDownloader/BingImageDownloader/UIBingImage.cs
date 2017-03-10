using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingImageDownloader
{
    public class UIBingImage
    {
        private System.Guid _bingImageId = Guid.Empty;

        public System.Guid BingImageId
        {
            get { return _bingImageId; }
            set
            {
                _bingImageId = value;
            }
        }

        private System.DateTime? _startDate = null;

        public System.DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
            }
        }

        private System.DateTime? _endDate = null;

        public System.DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
            }
        }

        private string _copyright = null;

        public string Copyright
        {
            get { return _copyright; }
            set
            {
                if (value != null)
                {
                    if (value.Length > 200)
                        throw new CharactersOverflowException("Enter too many characters!");
                }
                _copyright = value;
            }
        }

        private string _imageName = null;

        public string ImageName
        {
            get { return _imageName; }
            set
            {
                if (value != null)
                {
                    if (value.Length > 200)
                        throw new CharactersOverflowException("Enter too many characters!");
                }
                _imageName = value;
            }
        }

        private string _bingHost = null;

        public string BingHost
        {
            get { return _bingHost; }
            set
            {
                if (value != null)
                {
                    if (value.Length > 50)
                        throw new CharactersOverflowException("Enter too many characters!");
                }
                _bingHost = value;
            }
        }

        private string _imageUrl = null;

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                if (value != null)
                {
                    if (value.Length > 200)
                        throw new CharactersOverflowException("Enter too many characters!");
                }
                _imageUrl = value;
            }
        }

        private byte[] _imageData = null;

        public byte[] ImageData
        {
            get { return _imageData; }
            set
            {
                _imageData = value;
            }
        }

        private string _hotspots = null;

        public string Hotspots
        {
            get { return _hotspots; }
            set
            {
                if (value != null)
                {
                    if (value.Length > 2000)
                        throw new CharactersOverflowException("Enter too many characters!");
                }
                _hotspots = value;
            }
        }

        private string _messages = null;

        public string Messages
        {
            get { return _messages; }
            set
            {
                if (value != null)
                {
                    if (value.Length > 2000)
                        throw new CharactersOverflowException("Enter too many characters!");
                }
                _messages = value;
            }
        }
    }
}
