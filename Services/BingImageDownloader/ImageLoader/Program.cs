using BingImageDownloader;
using BingImageDownloader.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            //Log.Get().Info("Hi~~");

            BingImageManager imageManager = new BingImageManager("cn.bing.com", "China");
            Console.WriteLine(imageManager.ContainsTodayBingImage());
            Console.ReadLine();
            Console.WriteLine(imageManager.SaveTodayBingImage());
            //UIBingImage image = imageManager.GetLastBingImage();
            //Console.WriteLine(image.ImageName);
            //using (FileStream fs = new FileStream("D:\\" + image.ImageName + ".jpg", FileMode.Create))
            //{
            //    fs.Write(image.ImageData, 0, image.ImageData.Length);
            //}

        }
    }
}
