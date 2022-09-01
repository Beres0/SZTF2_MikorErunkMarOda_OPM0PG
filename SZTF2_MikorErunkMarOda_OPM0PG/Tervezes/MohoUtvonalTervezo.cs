using System;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Tervezes
{
    /// <summary>
    /// Tervező, ami mohó elven keresi az útitervet. Nem mindig nyújt optimális megoldást.
    /// </summary>
    internal class MohoUtvonalTervezo : UtvonalTervezo
    {
        /// <summary>
        /// Elkészít egy mohó útvonaltervezőt.
        /// </summary>
        /// <param name="utazas"></param>
        public MohoUtvonalTervezo(Utazas utazas) : base(utazas)
        {
        }
        /// <summary>
        /// Megkeresi az első olyan megállót, ahol a valamelyik személytipusnak letelt a számlálója
        /// és az a megálló éppen megfelel az igényének.
        /// Ha van ilyen, akkor a státusz megállást beállítja igazra és frissíti a többi státuszt, majd ugyanígy folytatja a keresést a tömb végéig.
        /// </summary>
        private void KeresMegfeleloMegallokat()
        {
            for (int i = 1; i < tervezoStatuszok.Length; i++)
            {
                TervezoStatusz statusz = tervezoStatuszok[i];
                int j = 0;
                while (j < igenyPerDarab.Length &&
                       (statusz.SzamlalokMegalloElott[j] >0 ||
                       !igenyPerDarab[j].Kulcs.Megfelel(statusz.Megallo)))
                {
                    j++;
                }

                if (j < igenyPerDarab.Length)
                {
                    statusz.Megall = true;
                    FrissitStatuszok(i);
                }
            }
        }
        /// <summary>
        /// Megkeresi az első olyan státuszt  az útvonalon, ahol elfogyott az üzemanyag.
        /// Közben azt is számontartja, hogy melyik volt az utolsó megálló, ahol tankolni lehet. 
        /// Ha talált alacsony üzemanyagú státuszt és tankolási lehetőséget is talált,
        /// akkor tankolási lehetőségnél beállítja a státusz megállását igazra,
        /// frissíti az utána lévő státuszokat, ezután törli az utolsó tankolási lehetőséget és folytatja a keresést, 
        /// amíg a végére nem ért. Abban az esetben, ha alacsony üzemanyagszint ellenére nem talált szóba jöhető megállót,
        /// akkor idő előtt abbahagyja a keresést, mert az útvonal nem lehetséges és visszatér hamis értékkel.
        /// </summary>
        /// <returns></returns>
        private bool KeresTankolasiLehetosegeket()
        {
            int nincs = -1;
            int utolso = nincs;
            int i = 1;
            bool folytat = true;
            while (i < tervezoStatuszok.Length && folytat)
            {
                while (i < tervezoStatuszok.Length && tervezoStatuszok[i].UzemanyagSzintMegalloElott > 0)
                {
                    if (tervezoStatuszok[i].Megallo.Szolgaltatas.VanIlyen(Szolgaltatas.Tankolas))
                    {
                        utolso = i;
                    }
                    i++;
                }

                folytat = i < tervezoStatuszok.Length && utolso != nincs;
                if (folytat)
                {
                    tervezoStatuszok[utolso].Megall = true;
                    FrissitStatuszok(utolso);
                    utolso = nincs;
                }
            }

            return i >= tervezoStatuszok.Length;
        }


        /// <summary>
        /// Előszőr frissíti az összes státuszt, hogy valós értékeket mutassanak az elemek,
        /// majd beállítja/megvizsgálja a tankolással rendelkező megállókat és ha a busz nem fogy ki az üzemanyagból,
        /// akkor a személyek igényeinek megfelelően is végig vizsgálja az útvonalat.
        /// Ha KeresTankolasiLehetosegek metódus eredménye hamis, akkor UtvonalTervezo.Kivetelt dob, mert az útvonal nem lehetséges.
        /// <para>
        /// Futásidő komponensek(n: megállók száma, m: utastípusok száma):
        /// </para>
        /// <para> - FrissitStatuszok: n</para>
        /// <para> - KeresTankolasiLehetosegeket: n*FrissitStatuszok</para>
        /// <para> - KeresMegfeleloMegallokat: n*(m+FrissitStatuszok)</para>
        /// <para> Futásidő: FrissitStatuszok+KeresTankolasiLehetosegeket+KeresMegfeleloMegallokat=n+n*n+n*(m+n) -> O(n*(n+m))</para>
        /// </summary>
        protected override void Tervez()
        {
            FrissitStatuszok();
            if (KeresTankolasiLehetosegeket())
            {
                KeresMegfeleloMegallokat();
                tervezoStatuszok[tervezoStatuszok.Length - 1].Megall = true;
                FrissitStatuszok();
            }
            else
            {
                throw new Kivetel(tervezoStatuszok);
            }
        }
    }
}