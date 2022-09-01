using System;

namespace SZTF2_MikorErunkMarOda_OPM0PG
{/// <summary>
/// Szolgaltatas enum tipus kiegészítő osztálya.
/// </summary>
    public static class SzolgaltatasKiegeszites
    {
        /// <summary>
        /// Eldönti, hogy a szolgáltatás tartalmaz-e ilyen értéket.
        /// </summary>
        /// <param name="mt">Kiegészítés célpontja</param>
        /// <param name="szolgaltatas">Keresett szolgáltatás.</param>
        public static bool VanIlyen(this Szolgaltatas mt, Szolgaltatas szolgaltatas)
        {
            return (mt & szolgaltatas) == szolgaltatas;
        }
    }
    /// <summary>
    /// Enum tipus, ami Flags attributummal van díszitve, így különböző kombinációkát képes tárolni. Értéke a megálló szolgáltatását/szolgáltatásait jelzi.
    /// </summary>
    [Flags]
    public enum Szolgaltatas
    {
        Semmi = 0, Tankolas = 1, Toalett = 2, Kave = 4, Nasi = 8
    }
}