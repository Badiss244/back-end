using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Domain
{
    public class ImageHelper
    {
        public static byte[] ConvertResourceImageToByteArray(Image img)
        {
            if (img == null)
                throw new ArgumentNullException(nameof(img));

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
