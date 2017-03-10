using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingImageDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            BingDownloader bd = new BingDownloader("cn.bing.com", 0, 1);
            List<BingImage> images = bd.GetImage(0, 1);
            foreach (BingImage image in images)
            {
                Console.WriteLine(image.Messages[0].Text);
                foreach (Hotspot spot in image.Hotspots)
                {
                    Console.WriteLine(spot.Discription);
                }
                image.Image.Save("c:\\bingImage.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            Console.Read();
        }
    }
}
