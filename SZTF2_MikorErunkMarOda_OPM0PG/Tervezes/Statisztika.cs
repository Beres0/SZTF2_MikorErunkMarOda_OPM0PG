using System.Collections.Generic;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Tervezes
{
    /// <summary>
    /// StatisztikaKeszito terméke.
    /// </summary>
    internal class Statisztika
    {
     /// /// <summary>
     /// Rekord, ami a Statisztika RekordOszlop attributúmmal ellátott tulajdonságainak fejlécét és lekerdéző függvényeit tárolja.
     /// </summary>
        public static readonly Rekord<Statisztika> Rekord = Rekord<Statisztika>.Letrehoz();
        /// <summary>
        /// Mennyi generálásából készült.
        /// </summary>
        public int GeneralasokSzama { get; }
        /// <summary>
        /// Generálások közül mennyi volt lehetséges a tervező szerint.
        /// </summary>
        [RekordOszlop]
        public int LehetsegesDarab { get; }
        /// <summary>
        /// Megállások átlaga.
        /// </summary>
        [RekordOszlop]
        public double MegallasokAtlag { get; }
        /// <summary>
        /// Mennyi másodperc alatt készültek el az útitervek átlagban.
        /// </summary>
        [RekordOszlop]
        public double FutasIdoAtlagMs { get; }
        /// <summary>
        /// Generáló, amelyik a bemeneti utazásokat generálta. 
        /// </summary>
        public Utazas.Generalo Generalo { get; }


        /// <summary>
        /// Elkészít egy Statisztikát.
        /// </summary>
        /// <param name="generalasokSzama">Mennyi generálásból készült</param>
        /// <param name="generalo">Generáló, amelyik a bemeneti utazásokat generálta. </param>
        /// <param name="utitervek">Tervező által elkészített útitervek.</param>
        public Statisztika(int generalasokSzama, Utazas.Generalo generalo, LancoltLista<Utiterv> utitervek)
        {
            Generalo = generalo;
            GeneralasokSzama = generalasokSzama;
            double mp = 0;
            double megallasokSzama = 0;
            foreach (var utiterv in utitervek)
            {
                if (utiterv != null)
                {
                    LehetsegesDarab++;
                    megallasokSzama += utiterv.Megallasok.Darab;
                    mp += utiterv.TervezesHossza.TotalSeconds;
                }
            }
            MegallasokAtlag = megallasokSzama / LehetsegesDarab;
            FutasIdoAtlagMs = mp / LehetsegesDarab;
        }
    }
}