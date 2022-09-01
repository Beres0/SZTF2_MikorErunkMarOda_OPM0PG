using System;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megjelenites
{
    /// <summary>
    /// Progam irányításának testreszabásáért felel
    /// </summary>
    internal class Parancs
    {
        /// <summary>
        /// Mi történjen billentyű lenyomásakor
        /// </summary>
        private Akcio parancs;
        /// <summary>
        ///<para>Kulcs: Parancs billentyú módosítója</para>
        ///<para>Érték: Parancs billentyűje.</para>
        /// </summary>
        public Par<ConsoleModifiers,ConsoleKey> Billentyu { get; }
        /// <summary>
        /// Parancs szöveges leírása.
        /// </summary>
        public string Leiras { get; }
        /// <summary>
        /// Elkészít egy parancsot.
        /// </summary>
        /// <param name="billentyu">Parancs billentyűje.</param>
        /// <param name="leiras">Parancs leírása.</param>
        /// <param name="parancs">Parancs művelete.</param>
        public Parancs(Par<ConsoleModifiers,ConsoleKey> billentyu, string leiras, Akcio parancs)
        {
            Billentyu = billentyu;
            Leiras = leiras;
            this.parancs = parancs;
        }
        /// <summary>
        /// Végrehajta a parancsot.
        /// </summary>
        public void Vegrehajt()
        {
            parancs();
        }
    }
}