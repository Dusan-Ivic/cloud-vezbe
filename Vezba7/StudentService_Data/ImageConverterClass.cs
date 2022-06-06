using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentService_Data
{
    public class ImageConverterClass
    {
        public static Image ConvertImage(Image image)
        {
            return new Bitmap(image, new Size(20, 20));
        }
    }
}
