using System;
using System.Collections.Generic;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megallok
{
    /// <summary>
    /// Interfész, ami statisztikai, tervezői célal fontosabb adatokat tárol el megállónként.
    /// </summary>
    interface IMegalloStatusz
    {
        /// <summary>
        /// Buszon utazó személyek darabszáma igény szerint csoportosítva.
        /// </summary>
        Par<Igeny, int>[] IgenyPerDarab { get; }
        /// <summary>
        /// Buszon utazó személyek számlálója érkezés pillanata előtt.
        /// </summary>
        int[] SzamlalokMegalloElott { get; }
        /// <summary>
        /// Státusz megállója.
        /// </summary>
        [RekordOszlop]
        Megallo Megallo { get; }
        /// <summary>
        /// Megáll-e a busz a megállónál.
        /// <para>RekordOszlop attribútummal van jelölve.</para>
        /// </summary>
        [RekordOszlop]
        bool Megall { get; }
        /// <summary>
        /// Érkezés időpontja.
        /// <para>RekordOszlop attribútummal van jelölve.</para>
        /// </summary>
        [RekordOszlop]
        DateTime ErkezesIdeje { get; }
        /// <summary>
        /// Busz üzemanyagszintje érkezés pillanata előtt.
        /// <para>RekordOszlop attribútummal van jelölve.</para>
        /// </summary>
        [RekordOszlop]
        float UzemanyagSzintMegalloElott { get; }
        /// <summary>
        /// Mennyi személy száll le a megállónál.
        /// <para>RekordOszlop attribútummal van jelölve.</para>
        /// </summary>
        [RekordOszlop]
        int Leszallok { get; }
    }
    /// <summary>
    /// IMegalloStatusz interfész segédosztálya, ami a tömbje táblázatba írásáért/írhatóságáért felel.
    /// </summary>
    static class MegalloStatusz
    {
        /// <summary>
        /// Rekord, ami az IMegallo interface RekordOszlop attributúmmal ellátott tulajdonságainak fejlécét és lekerdéző függvényeit tárolja.
        /// </summary>
        public static readonly Rekord<IMegalloStatusz> Rekord = Rekord<IMegalloStatusz>.Letrehoz();
        /// <summary>
        /// Elkészít egy pontosvesszővel tagolt szöveges táblázatot IMegalloStatuszok tömbjéből.
        /// </summary>
        /// <param name="megallok">Táblazatba írandó megállók.</param>
        /// <param name="mettol">Hanyadik megállótól írja a táblázatot.</param>
        /// <param name="meddig">Hanyadik megállóig írja a táblázatot.</param>
        /// <returns>Szöveges táblazat.</returns>
        public static string Tablazat(IMegalloStatusz[] megallok, int mettol, int meddig)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Rekord<IMegalloStatusz>.Oszlop oszlop in Rekord)
            {
                sb.Append(oszlop.Fejlec);
                for (int i = mettol; i <= meddig; i++)
                {
                    sb.Append(";" + oszlop.Ertek(megallok[i]));
                }
                sb.AppendLine();
            }
            for (int i = 0; i < megallok[mettol].IgenyPerDarab.Length; i++)
            {
                Par<Igeny, int> par = megallok[mettol].IgenyPerDarab[i];
                sb.Append($"{ par.Kulcs.Ciklus} {par.Kulcs.Szolgaltatas} {par.Ertek}");
                for (int j = mettol; j <= meddig; j++)
                {
                    sb.Append(";" + megallok[j].SzamlalokMegalloElott[i]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}