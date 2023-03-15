using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace BitMapEditor
{
    internal class MyBitmap
    {
        // Zmienne do przechowania pierwotnej bitmapy i aktualnej (przetworzonej) bitmapy
        private Bitmap preBitmap;

        private Bitmap curBitmap;

        // Zmienna do przechowania informacji o bitmapie
        private MyBitmapInfo bitmapInfo;

        // Pusta tablica jednowymiarowa bajtów [wartości od 0 do 255]
        private byte[] byteArray;

        //Pusta tablica dwuwymiarowa pikseli bez dodatkowych bajtow w postaci nagłówka pliku i nagłówka obrazu
        //- rozmiar wiersza (Width * 3 (24 bit));
        private byte[,] pixelArray;

        // Konstruktor klasy
        public MyBitmap(Image image, String path)
        {
            // Konwersja na bitmapę 24bitową
            this.preBitmap = convertTo24bpp(image);
            this.curBitmap = convertTo24bpp(image);
            // Funkcja przerabiająca obraz na tablicę bajtow (byte array będzie dalej puste jeśli curBitmap nie istnieje,
            // w przeciwnym wypadku w tablicy znajdzie się strumień bitmapy)
            this.byteArray = toByteArray(curBitmap, ImageFormat.Bmp);
            // Do zmiennej bitmapInfo trafiają informacje nt. szerokości i wysokości obrazu,
            // offsetu (ilość bajtów poprzedzająca dane o obrazie),
            // ilości bitów koloru na jeden piksel, ścieżki obrazu
            this.bitmapInfo = new MyBitmapInfo(image, byteArray[10], byteArray[28], path);
            Console.WriteLine(byteArray[10] + " oraz " + byteArray[28] + "oraz" + byteArray.Length);
            // Funkcja konwertująca tablicę
            this.pixelArray = convertArray(this.byteArray, image.Width, image.Height);
        }

        // Konwersja bitmapy na 24bitową
        public static Bitmap convertTo24bpp(Image img)
        {
            // Format ma 24 bity na piksel, odpowiednio 8 bitów na każdy z kolorów: czerwony, zielony, niebieski
            Bitmap bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            // Tworzymy obiekt typu Graphics do modyfikacji
            Graphics gr = Graphics.FromImage(bmp);
            // Rysujemy nową bitmapę bazującą na oryginalnej
            // (0,0 - współrzędne x,y lewego górnego rogu prostokąta;
            // img.width, img.height - szerokość i wysokość)
            gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }

        // Przepisanie tablicy pikseli na ktorej operuje funkcja asemblerowa i przerobienie jej na bitmape.
        public void finalizeAssemblerFunc()
        {
            this.preBitmap = this.curBitmap;
            Bitmap bmp = createBitmapFromPixelArray(this.pixelArray, this.bitmapInfo.SizeX, this.bitmapInfo.SizeY);
            this.curBitmap = convertTo24bpp(bmp);
        }

        public void finalizeAssemblerFuncSharp(byte[,] resultArray)
        {
            this.preBitmap = curBitmap;
            this.pixelArray = (byte[,])resultArray.Clone();
            Bitmap bmp = createBitmapFromPixelArray(this.pixelArray, this.bitmapInfo.SizeX, this.bitmapInfo.SizeY);
            this.curBitmap = convertTo24bpp(bmp);
        }

        // Funkcja przerabia obraz na tablicę bajtow
        public byte[] toByteArray(Image image, ImageFormat format)
        {
            // Jeżeli obraz istnieje
            if (image != null)
            {
                // Używamy MemoryStream jako źródła zapasowego dla danych
                MemoryStream ms = new MemoryStream();
                // Zapisujemy podany obraz w strumieniu ms, w formacie format
                image.Save(ms, format);
                // Zwracamy zawartośc strumienia do tablicy bajtów
                return ms.ToArray();
            }
            // Jeżeli nie istnieje, zwracamy null (tablica dalej jest pusta)
            return null;
        }

        // Konwersja tablicy z pełnymi danymi do tablicy samych danych dotyczących kolorów obrazu
        public byte[,] convertArray(byte[] Input, int sizeX, int sizeY)
        {
            int width = ((Input.Length - 54) / sizeY); // 134 - 54 = 80; 80/5 = 16
            int additional = width - (sizeX * 3); //16 - 5*3 = 1
            int line = 0;
            byte[,] Output = new byte[sizeY, width - additional]; // 5, 15

            // Do tablicy Output wkładamy wartości będące danymi obrazka (same informacje dot. składowych pikseli)
            // Od wartości od której zaczynają się dane obrazka, do końca wartości w tablicy, skaczemy co bloki podzielne
            // przez 4
            for (int i = 54; i < Input.Length; i += width)
            {
                for (int j = 0; j < width - additional; j++) //0, 15, +1
                {
                    Output[line, j] = Input[i + j];
                }
                line++;
            }
            // Zwracamy tablicę zawierającą same dane obrazu (bez informacji o strukturze bitmapy)
            return Output;
        }

        // Funkcja tworzy bitmape z tablicy bajtow;
        public Bitmap createBitmapFromByteArray(byte[] pixelArray)
        {
            return new Bitmap(Bitmap.FromStream(new MemoryStream(pixelArray)));
        }

        // Funkcja tworzy bitmape z tablicy pikseli;
        public Bitmap createBitmapFromPixelArray(byte[,] Input, int sizeX, int sizeY)
        {
            int width = ((byteArray.Length - 54) / sizeY);
            int additional = width - (sizeX * 3);
            byte[] additionalArray = new byte[additional];
            byte[] tmpByteArray = new byte[byteArray.Length];
            Bitmap bitmap = new Bitmap(sizeX, sizeY);
            Array.Copy(byteArray, tmpByteArray, bitmapInfo.Offset);

            int i = bitmapInfo.Offset;
            for (int k = 0; k < sizeY; k++)
            {
                for (int n = 0; n < sizeX * 3; n++)
                {
                    tmpByteArray[i] = Input[k, n];
                    i++;
                }
                additionalArray.CopyTo(tmpByteArray, i);
                i += additional;
            }
            byteArray = (byte[])tmpByteArray.Clone();
            return createBitmapFromByteArray(byteArray);
        }

        // Nadpisanie byteArray i pixelArray;
        internal void updateArrays()
        {
            this.byteArray = this.toByteArray(this.curBitmap, ImageFormat.Bmp);
            this.pixelArray = this.convertArray(this.byteArray, this.bitmapInfo.SizeX, this.bitmapInfo.SizeY);
        }

        // Gettery i settery
        public Bitmap PreviousBitmap
        {
            get
            {
                return preBitmap;
            }
            set
            {
                preBitmap = value;
            }
        }

        public Bitmap CurrentBitmap
        {
            get
            {
                return curBitmap;
            }
            set
            {
                curBitmap = value;
            }
        }

        public MyBitmapInfo BitmapInfo
        {
            get
            {
                return bitmapInfo;
            }
            set
            {
                bitmapInfo = value;
            }
        }

        public byte[,] PixelArray
        {
            get
            {
                return pixelArray;
            }
            set
            {
                pixelArray = value;
            }
        }

        public byte[] ByteArray
        {
            get
            {
                return byteArray;
            }
            set
            {
                byteArray = value;
            }
        }
    }
}