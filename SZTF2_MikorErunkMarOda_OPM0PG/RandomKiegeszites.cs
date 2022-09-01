using System;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;

namespace SZTF2_MikorErunkMarOda_OPM0PG
{
    /// <summary>
    /// Random osztály kiegészítése.
    /// </summary>
    internal static class RandomKiegeszites
    {
        /// <summary>
        /// Kiválaszt véletlenszerúen egy elemet az adott tömbből.
        /// </summary>
        /// <param name="random">Kiegészítés randomja.</param>
        /// <param name="tomb">Tömb, amiből kiválaszt.</param>
        /// <returns>Kiválaszott elem.</returns>
        public static T Kivalaszt<T>(this Random random, T[] tomb)
        {
            return tomb[random.Next(tomb.Length)];
        }

        /// <summary>
        /// Kiválaszt véletlenszerúen egy elemet az adott kollekcióból.
        /// </summary>
        /// <param name="random">Kiegészítés randomja.</param>
        /// <param name="tomb">Kollekció, amiből kiválaszt.</param>
        /// <returns>Kiválaszott elem.</returns>
        public static T Kivalaszt<T>(this Random random, IIndexelhetoKollekcio<T> kollekcio)
        {
            return kollekcio[random.Next(kollekcio.Darab)];
        }
    }
}