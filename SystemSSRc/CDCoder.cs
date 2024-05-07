using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cdShifrC_
{
    internal class CDCoder
    {
        private int Size;

        private char[] Stream;

        public CDCoder(int size, char[] stream)
        {
            Size = size;
            Stream = stream;
        }

        public override string ToString()
        {
            string Str = string.Empty;

            for (int i = 0; i < Size; i++)
            {
                Str += Stream[i].ToString();
            }

            return Str;
        }

        public char[] Code()
        {

            for (int i = 0; i < Size; i++)
            {
                int newPlace = Math.Abs(i % Size);
                Stream[i] = (char)(Stream[i] - newPlace);
                Console.WriteLine(newPlace);
                Swap<char>(ref Stream[i], ref Stream[newPlace]);
            }

            return Stream;
        }

        public char[] Decode()
        {
            for (int i = Size - 1; i >= 0; i--)
            {
                int oldPlace = Math.Abs(i % Size);
                Swap<char>(ref Stream[i], ref Stream[oldPlace]);
                Stream[i] = (char)(Stream[i] + oldPlace);
                Console.WriteLine(oldPlace);
            }

            return Stream;
        }

        void Swap<T>(ref T v1, ref T v2) { T v3 = v1; v1 = v2; v2 = v3; }
    }

}
