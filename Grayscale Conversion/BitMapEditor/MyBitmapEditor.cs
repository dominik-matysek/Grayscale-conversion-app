using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace BitMapEditor
{
    // Klasa zawierajaca metody odpowiedzialne za edytowanie bitmap;
    internal class MyBitmapEditor
    {
        private const int MAX = 256;

        public MyBitmapEditor()
        {
        }

        // Konwersja do skali szarości
        internal void grayScale(MyBitmap myBitmap)
        {
            myBitmap.PreviousBitmap = (Bitmap)myBitmap.CurrentBitmap.Clone();
            //Wykorzystanie modelu YUV //Srednia skladowych
            const float rMod = 0.299f; //rMod = 0.333f;
            const float gMod = 0.587f; //gMod = 0.333f;
            const float bMod = 0.114f; //bMod = 0.333f;
            Graphics g = Graphics.FromImage(myBitmap.CurrentBitmap);

            ColorMatrix colorMatrix = new ColorMatrix(new[]
            {
                new[] {rMod, rMod, rMod, 0, 1},
                new[] {gMod, gMod, gMod, 0, 1},
                new[] {bMod, bMod, bMod, 0, 1},
                new[] {0.0f, 0.0f, 0.0f, 1, 1},
                new[] {0.0f, 0.0f, 0.0f, 0, 1}
            });

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            int x = myBitmap.BitmapInfo.SizeX;
            int y = myBitmap.BitmapInfo.SizeY;
            g.DrawImage(myBitmap.CurrentBitmap, new Rectangle(0, 0, x, y), 0, 0, x, y, GraphicsUnit.Pixel, attributes);
            g.Dispose();
        }

        // Cofniecie ostatniej operacji;
        internal void goBack(MyBitmap myBitmap)
        {
            Bitmap tmp = (Bitmap)myBitmap.PreviousBitmap.Clone();
            myBitmap.CurrentBitmap = myBitmap.PreviousBitmap;
            myBitmap.PreviousBitmap = tmp;
            myBitmap.updateArrays();
        }

        // Klasa odpowiedzialna za funckje edytujace napisane w Asemblerze;
        public class UnsafeNativeMethods
        {
            [DllImport("BitMapEditorDLL.dll")]
            public static extern int Dodaj(int a, int b);

            [DllImport("BitMapEditorDLL.dll")]
            public static extern void GreyASM(IntPtr byteArray, int sizeX, int sizeY);

            public static void InitAsmFunction()
            {
                byte[,] pixelArray = new byte[9, 9];
                byte[,] resultArray = new byte[9, 9];
                IntPtr a0 = Marshal.UnsafeAddrOfPinnedArrayElement(pixelArray, 0);
                IntPtr a1 = Marshal.UnsafeAddrOfPinnedArrayElement(resultArray, 0);
                GreyASM(a0, 3, 3);
            }
        }
    }
}