using BingImageDownloader.BingImages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;


namespace BingImageDownloader
{
    public class BingImageManager
    {
        private BingDownloader downloader;

        public BingImageManager()
        {
        }

        public BingImageManager(string host, string countryName)
        {
            downloader = new BingDownloader(host);
            downloader.CountryName = countryName;
        }

        public BingDownloader Downloader
        {
            get
            {
                return downloader;
            }
        }

        protected DbConnection Connection
        {
            get
            {
                return new SqlConnection(ConfigurationManager.ConnectionStrings["BingImage"].ConnectionString);
            }
        }

        protected void Open(DbConnection connection)
        {
            if (connection != null && connection.State != ConnectionState.Open)
                connection.Open();
        }

        protected void Close(DbConnection connection)
        {
            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Close();
        }

        public bool ContainsTodayBingImage()
        {
            using (DbConnection conn = Connection)
            {
                Open(conn);
                DbCommand command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(1) FROM [sys_LoginImages] WHERE [EndDate] = @EndDate";
                command.CommandType = CommandType.Text;

                DbParameter param = new SqlParameter("@EndDate", DbType.DateTime);
                param.Value = DateTime.Now.ToString("yyyy-MM-dd");
                command.Parameters.Add(param);

                int val = (int)command.ExecuteScalar();
                return val > 0;
            }
        }

        public bool SaveTodayBingImage()
        {
            List<BingImage> images = null;
            try
            {
                images = downloader.GetImage(1, 1);
                if (images.Count == 0)
                    return false;
                BingImage image = images[0];
                UIBingImage uiImage = BingImageToUIBingImage(image);
                using (DbConnection conn = Connection)
                {
                    conn.Open();
                    DbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO [sys_LoginImages] VALUES (@BingImageId, @StartDate, @EndDate, @Copyright, @ImageName, @BingHost, @ImageUrl, @ImageData, @Hotspots, @Messages)";
                    cmd.CommandType = CommandType.Text;

                    DbParameter param = new SqlParameter("@BingImageId", DbType.Guid);
                    param.Value = uiImage.BingImageId;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@StartDate", DbType.DateTime);
                    param.Value = uiImage.StartDate;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@EndDate", DbType.DateTime);
                    param.Value = uiImage.EndDate;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@Copyright", DbType.String);
                    param.Value = uiImage.Copyright;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@ImageName", DbType.String);
                    param.Value = uiImage.ImageName;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@BingHost", DbType.String);
                    param.Value = uiImage.BingHost;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@ImageUrl", DbType.String);
                    param.Value = uiImage.ImageUrl;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@ImageData", DbType.Binary);
                    param.Value = uiImage.ImageData;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@Hotspots", DbType.String);
                    param.Value = uiImage.Hotspots;
                    cmd.Parameters.Add(param);
                    param = new SqlParameter("@Messages", DbType.String);
                    param.Value = uiImage.Messages;
                    cmd.Parameters.Add(param);

                    int val = cmd.ExecuteNonQuery();
                    return val == 1;
                }
            } 
            catch(Exception e)
            {
                Logging.Log.Get().Current.Error("保存出现错误", e);
                return false;
            }
        }

        public UIBingImage GetLastBingImage()
        {
            using (DbConnection conn = Connection)
            {
                UIBingImage uiImage = null;
                conn.Open();
                DbCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP(1) * FROM [sys_LoginImages] ORDER BY [StartDate] DESC";
                cmd.CommandType = CommandType.Text;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        uiImage = new UIBingImage();
                        uiImage.BingImageId = reader.GetGuid(0);
                        uiImage.StartDate = Convert.ToDateTime(reader[1]);
                        uiImage.EndDate = Convert.ToDateTime(reader[2]);
                        uiImage.Copyright = reader.GetString(3);
                        uiImage.ImageName = reader.GetString(4);
                        uiImage.BingHost = reader.GetString(5);
                        uiImage.ImageUrl = reader.GetString(6);
                        uiImage.ImageData = (byte[])reader.GetValue(7);
                        uiImage.Hotspots = reader.GetString(8);
                        uiImage.Messages = reader.GetString(9);
                    }
                }
                return uiImage;
            }
        }

        #region Helpers

        UIBingImage BingImageToUIBingImage(BingImage image)
        {
            UIBingImage uiImage = new UIBingImage();
            uiImage.BingImageId = Guid.NewGuid();
            uiImage.StartDate = image.StartDate;
            uiImage.EndDate = image.EndDate;
            uiImage.Copyright = image.Copyright;
            uiImage.ImageName = FormatImageName(image.Copyright);
            uiImage.BingHost = downloader.BingHost;
            uiImage.ImageUrl = image.ImageUrl;
            
            MemoryStream imageStream = new MemoryStream();
            image.Image.Save(imageStream, ImageFormat.Jpeg);
            uiImage.ImageData = imageStream.ToArray();
            imageStream.Close();

            uiImage.Hotspots = JsonConvert.SerializeObject(image.Hotspots);
            uiImage.Messages = JsonConvert.SerializeObject(image.Messages);

            return uiImage;
        }

        string FormatImageName(string name)
        {
            int len = name.IndexOf("(©");
            if (len == -1)
                len = name.IndexOf("(");
            if (len == -1)
                len = name.Length;
            if (len > 0)
            {
                return name.Substring(0, len);
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion
    }
}
