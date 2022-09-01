using System;
using System.Collections.Generic;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi;

namespace SZTF2_MikorErunkMarOda_OPM0PG
{
    /// <summary>
    /// Szimulált utazás kezdőparamétereit egységbezáró osztály.
    /// </summary>
    internal class Utazas
    {
        /// <summary>
        /// Képes utazásokat generálni a paraméterei szerint.
        /// </summary>
        public class Generalo
        {
            /// <summary>
            /// Rekord, ami a Generalo RekordOszlop attribútummal ellátott tulajdonságainak fejlécét és lekerdéző függvényeit tárolja.
            /// </summary>
            public static readonly Rekord<Generalo> Rekord = Rekord<Generalo>.Letrehoz();

            /// <summary>
            /// Indulás ideje.
            /// <para>RekordOszlop attribútummal van jelölve.</para>
            /// </summary>
            [RekordOszlop]
            public DateTime IndulasIdeje { get; }

            /// <summary>
            /// Generált megállók darabszáma.
            /// <para>RekordOszlop attribútummal van jelölve.</para>
            /// </summary>
            [RekordOszlop]
            public int MegalloDarab { get; }
            /// <summary>
            /// Generált megállók maximum távolsága.
            /// <para>RekordOszlop attribútummal van jelölve.</para>
            /// </summary>
            [RekordOszlop]
            public int TavolsagMax { get; }

            /// <summary>
            /// Generált megállók minimum távolsága.
            /// <para>RekordOszlop attribútummal van jelölve.</para>
            /// </summary>
            [RekordOszlop]
            public int TavolsagMin { get; }
            /// <summary>
            /// Utasok száma.
            /// <para>RekordOszlop attribútummal van jelölve.</para>
            /// </summary>
        
            [RekordOszlop]
            public int UtasDarab { get; }

            /// <summary>
            /// Generáláshoz használt random seedje.
            /// <para>RekordOszlop attribútummal van jelölve.</para>
            /// </summary>
            [RekordOszlop]
            public int? Seed { get; }
            /// <summary>
            /// Elkészít egy utazás generálót.
            /// </summary>
            /// <param name="megalloDarab">Generált megállók darabszáma.</param>
            /// <param name="tavolsagMin">Generált megállók minimum távolsága.</param>
            /// <param name="tavolsagMax">Generált megállók maximum távolsága.</param>
            /// <param name="utasDarab">Utasok száma.</param>
            /// <param name="indulas">Indulás ideje. Ha nem kap értéket, a jelenlegi időt állítja be.</param>
            /// <param name="seed">Generáláshoz használt random seedje</param>
            public Generalo(int megalloDarab, int tavolsagMin, int tavolsagMax, int utasDarab, DateTime? indulas=null,int? seed=null)
            {
                IndulasIdeje = indulas.HasValue ? indulas.Value : DateTime.Now;
                Seed = seed;
                MegalloDarab = megalloDarab;
                TavolsagMin = tavolsagMin;
                TavolsagMax = tavolsagMax;
                UtasDarab = utasDarab;
            }

            /// <summary>
            /// Generál egy utazást a paraméterei szerint.
            /// </summary>
            /// <returns>Generált utazás.</returns>
            public Utazas General()
            {
                Random random = Seed.HasValue?new Random(Seed.Value):new Random();
                LancoltLista<Megallo> megallok = Megallo.General(random, TavolsagMin, TavolsagMax, MegalloDarab);
                LancoltLista<Szemely> szemelyek = Szemely.General(random, UtasDarab);
                return new Utazas(IndulasIdeje, megallok, szemelyek);
            }
            /// <summary>
            /// Több utazást generál a paraméterei szerint.
            /// </summary>
            /// <returns>Generált utazások láncoltlistája.</returns>

            public LancoltLista<Utazas> General(int darab)
            {
                Random random = Seed.HasValue ? new Random(Seed.Value) : new Random();
                LancoltLista<Utazas> utazasok = new LancoltLista<Utazas>();
                for (int i = 0; i < darab; i++)
                {
                    LancoltLista<Megallo> megallok = Megallo.General(random, TavolsagMin, TavolsagMax, MegalloDarab);
                    LancoltLista<Szemely> szemelyek = Szemely.General(random, UtasDarab);
                    utazasok.Hozzaad(new Utazas(IndulasIdeje, megallok, szemelyek));
                }
                return utazasok;
            }

            /// <summary>
            /// Elkészít egy pontosvesszővel tagolt szöveges rekordot az értékeiből. Lehet hozzáadni extra
            /// adatokat is, ha a kliens úgy kívánja.
            /// </summary>
            /// <returns>Szöveges rekord.</returns>
            public string KeszitRekord(params Par<string,string>[] extraAdatok)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Rekord.Fejlecek().Osszefuz(";"));
                sb.Append(";");
                sb.AppendLine(extraAdatok.Osszefuz((par) => par.Kulcs, ";"));
                sb.Append(Rekord.Ertekek(this).Osszefuz(";"));
                sb.Append(";");
                sb.AppendLine(extraAdatok.Osszefuz((par) => par.Ertek, ";"));
                return sb.ToString();
            }
        }
        /// <summary>
        /// Utazás ideje.
        /// </summary>
        public Ido Ido { get; }
        /// <summary>
        /// Utazó személyek.
        /// </summary>
        public SzemelyLista Szemelyek { get; }
        /// <summary>
        /// Utazás útvonala.
        /// </summary>
        public Utvonal Utvonal { get; }
        /// <summary>
        /// Elkészít egy utazást.
        /// </summary>
        /// <param name="indulas">Indulás időpontja.</param>
        /// <param name="utvonal">Az útvonal.</param>
        /// <param name="szemelyek">Buszon utazó személyek</param>
        public Utazas(DateTime indulas,IOlvashatoKollekcio<Megallo> utvonal, IOlvashatoKollekcio<Szemely> szemelyek)
        {
            Ido = new Ido(indulas);
            Utvonal = new Utvonal(utvonal);
            Szemelyek = new SzemelyLista(szemelyek);
        }
    }
}