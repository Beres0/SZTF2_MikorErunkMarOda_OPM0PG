using System.Collections.Generic;
using System.Text;
namespace SZTF2_MikorErunkMarOda_OPM0PG
{
    /// <summary>
    /// Szőveges műveleteket végző kiegészítő metódusokat ad a string és az IEnumerable{T} osztályhoz.
    /// </summary>
    internal static class Szoveg
    {
        /// <summary>
        /// StringBuilder, ami az összefűzést végzi.
        /// </summary>
        private static readonly StringBuilder osszefuzo=new StringBuilder();
        /// <summary>
        /// Elemeket fűz össze egyetlen karakterlánccá.
        /// </summary>
        /// <param name="elemek">Kiegészített gyüjtemény</param>
        /// <param name="kivalaszto">Az elem szöveges reprezentációja.</param>
        /// <param name="elvalaszto">Elválasztó, amit beszúr az elemek közé.</param>
        /// <returns>Összefűzött szöveg</returns>
        public static string Osszefuz<T>(this IEnumerable<T> elemek, Fuggveny<T, string> kivalaszto, string elvalaszto)
        {
            osszefuzo.Clear();

            var enumerator = elemek.GetEnumerator();
            if (enumerator.MoveNext())
            {
                osszefuzo.Append(kivalaszto(enumerator.Current));
                while (enumerator.MoveNext())
                {
                    osszefuzo.Append(elvalaszto);
                    osszefuzo.Append(kivalaszto(enumerator.Current));
                }
            }
            return osszefuzo.ToString();
        }
        /// <summary>
        /// Szövegeket fűz össze egyetlen karakterlánccá.
        /// </summary>
        /// <param name="elemek">Kiegészített gyüjtemény</param>
        /// <param name="elvalaszto">Elválasztó, amit beszúr a szövegek közé.</param>
        /// <returns>Összefűzött szöveg</returns>
        public static string Osszefuz(this IEnumerable<string> szovegek,string elvalaszto)
        {
            return Osszefuz(szovegek, (s) => s, elvalaszto);
        }
        /// <summary>
        /// Szöveget balra, a másik szöveget jobbra igazítja egy bizonyos szélességen.
        /// </summary>
        /// <param name="str">Balra igazítandó kiegészített szöveg.</param>
        /// <param name="masik">Jobbra igazítandó másik szöveg.</param>
        /// <param name="szelesseg">Igazítás szélessége.</param>
        /// <returns>Igazított szöveg</returns>
        public static string Igazit(this string str, string masik, int szelesseg)
        {
            bool paratlan = szelesseg % 2 != 0;
            szelesseg /= 2;
            return str.Igazit(Igazitas.Bal, szelesseg) + masik.Igazit(Igazitas.Jobb, paratlan ? szelesseg + 1 : szelesseg);
        }
        /// <summary>
        /// Szöveget igazít bizonyos szélességen.
        /// </summary>
        /// <param name="str">Kiegészített szöveg.</param>
        /// <param name="igazitas">Igazítás módja.</param>
        /// <param name="szelesseg">Igazítás szélessége.</param>
        /// <returns>Igazított szöveg.</returns>
        public static string Igazit(this string str, Igazitas igazitas, int szelesseg)
        {
            switch (igazitas)
            {
                case Igazitas.Bal: return str.PadRight(szelesseg);
                case Igazitas.Jobb: return str.PadLeft(szelesseg);
                case Igazitas.Kozep:
                    int kulonbseg = szelesseg - str.Length;
                    string pad1 = new string(' ', kulonbseg / 2);
                    if (kulonbseg % 2 == 0)
                    {
                        return pad1 + str + pad1;
                    }
                    else
                    {
                        string pad2 = new string(' ', kulonbseg / 2 + 1);
                        return pad1 + str + pad2;
                    }
                default: return null;
            }
        }
    }
    /// <summary>
    /// Szöveges igazítás módjai.
    /// </summary>
    internal enum Igazitas
    {
        Bal, Jobb, Kozep
    }
}