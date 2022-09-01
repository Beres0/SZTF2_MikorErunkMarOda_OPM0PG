using System;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megjelenites
{
    /// <summary>
    /// Szimuláció ebből épiti fel a szöveges táblázatát.
    /// </summary>
    class SzimulacioStatusz : IMegalloStatusz
    {
        /// <summary>
        /// Rekord, ami az IMegallo interface RekordOszlop attributúmmal ellátott tulajdonságainak fejlécét és lekerdéző függvényeit tárolja.
        /// </summary>
        public static Rekord<IMegalloStatusz> Rekord => MegalloStatusz.Rekord;

        public Megallo Megallo { get; }
        public int[] SzamlalokMegalloElott { get;  }
        public Par<Igeny, int>[] IgenyPerDarab { get;}
        public DateTime ErkezesIdeje { get;set; }
        public virtual int Leszallok { get;set; }
        public bool Megall { get; set; }

        public float UzemanyagSzintMegalloElott { get; set; }

        /// <summary>
        /// Elkészít egy szimuláció státuszt.
        /// </summary>
        /// <param name="megallo">Státuszhoz tartozó megálló</param>
        /// <param name="igenyPerDarab">Személyek darabszáma igényük szerint</param>
        public SzimulacioStatusz(Megallo megallo, Par<Igeny, int>[] igenyPerDarab)
        {
            Megallo = megallo;
            SzamlalokMegalloElott = new int[igenyPerDarab.Length];
            IgenyPerDarab = igenyPerDarab;
        }
    }
}