using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BitMapEditor
{
    // Klasa przechowująca dane z nagłowka bitmapy, tablice bajtow pliku bmp oraz tablice pikseli pliku bmp
    internal class MyBitmapInfo
    {
        private int bmpWeight;

        // Wymiary x i y bitmapy
        private int xSize;

        private int ySize;

        // Ścieżka do pliku bmp
        private String bmpPath;

        // Offset (ilość bajtów poprzedzająca dane o obrazie)
        private int offset;

        // Ilość bitów koloru na jeden piksel
        private int pixelCount;

        // Konstruktor klasy
        public MyBitmapInfo(Image bitmap, int offset, int pixelCount, String path)
        {
            this.xSize = bitmap.Size.Width;
            this.ySize = bitmap.Size.Height;
            this.offset = offset;
            this.pixelCount = pixelCount;
            this.bmpPath = path;
            // szerokość * wysokość (ile mamy pikseli w obrazie) * 3 (bo jeden piksel to 3 składowe R G B)
            this.bmpWeight = this.offset + this.xSize * this.ySize * 3;
        }

        public int Weight
        {
            get
            {
                return bmpWeight;
            }
        }

        public String Path
        {
            get
            {
                return bmpPath;
            }
            set
            {
                bmpPath = value;
            }
        }

        public int SizeX
        {
            get
            {
                return xSize;
            }
            set
            {
                xSize = value;
            }
        }

        public int SizeY
        {
            get
            {
                return ySize;
            }
            set
            {
                ySize = value;
            }
        }

        public int Offset
        {
            get
            {
                return offset;
            }
        }

        public int BitPerPixel
        {
            get
            {
                return pixelCount;
            }
        }
    }
}