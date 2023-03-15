using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BitMapEditor
{
    internal class TimeResult
    {
        public TimeResult(String strImpl, int second, int milisecond, int sizeX, int sizeY)
        {
            this.strImpl = strImpl;
            this.second = second;
            this.milisecond = milisecond;
            this.strMilis = convertMilis(milisecond);
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }

        private String strImpl;
        public String strMilis { get; set; }
        private int second;
        private int milisecond;
        private int sizeX;
        private int sizeY;

        public static String convertMilis(int milisecond)
        {
            String strMilis = "00";
            if (milisecond > 0)
            {
                if (milisecond < 100)
                {
                    strMilis = ((float)milisecond / 1000).ToString();
                    return strMilis.Substring(2);
                }
                else
                    strMilis = milisecond.ToString();
            }
            return strMilis;
        }

        // Gettery i settery

        public String Implementation
        {
            get
            {
                return strImpl;
            }
        }

        public int Seconds
        {
            get
            {
                return second;
            }
        }

        public int Miliseconds
        {
            get
            {
                return milisecond;
            }
        }

        public int Width
        {
            get
            {
                return sizeX;
            }
        }

        public int Height
        {
            get
            {
                return sizeY;
            }
        }
    }
}