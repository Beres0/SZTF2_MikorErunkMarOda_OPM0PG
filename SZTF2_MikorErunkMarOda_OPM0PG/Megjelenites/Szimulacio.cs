using System;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi;
using SZTF2_MikorErunkMarOda_OPM0PG.Tervezes;
using System.IO;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megjelenites
{
    /// <summary>
    /// Szimulálja a busz utazását.
    /// </summary>
    internal class Szimulacio
    {
        /// <summary>
        /// Szimuláció busza.
        /// </summary>
        private Busz busz;
        
        /// <summary>
        /// Szimuláció parancsai.
        /// </summary>
        private HashLista<Parancs> parancsok;

        /// <summary>
        /// Segédváltozó, amivel meg lehet szakítani az aktuláis szimuláció futását.
        /// </summary>
        private bool vissza;
        /// <summary>
        /// Szimuláció megjelenítése.
        /// </summary>
        SzimulacioMegjelenito megjelenito;
        /// <summary>
        /// Elkészít egy szimulációt.
        /// </summary>
        public Szimulacio()
        {
            parancsok = new HashLista<Parancs>((i) => i.Billentyu);
            ParancsokatHozzad();
            megjelenito = new SzimulacioMegjelenito(20,parancsok);
        }
        /// <summary>
        /// Bekér egy számot addig a felhasználótól, amíg nem ad érvenyes bemenetet.
        /// </summary>
        /// <param name="nev">Bekért adat neve.</param>
        /// <param name="min">Szám alsóhatára.</param>
        /// <param name="max">Szám felsőhatára.</param>
        /// <returns>Bekért adat számként.</returns>
        private int Bekeres(string nev, int min, int max)
        {
            string input;
            int eredmeny;
            bool ervenyes;
            do
            {
                Console.Write(nev + $"[{min},{max}]: ");
                input = Console.ReadLine();
                ervenyes = int.TryParse(input, out eredmeny) && eredmeny >= min && eredmeny <= max;
                if (!ervenyes)
                {
                    Console.WriteLine($"Érvenytelen! {min} és {max} közötti számot adj meg");
                }
            } while (!ervenyes);
            return eredmeny;
        }
   
        /// <summary>
        /// Feldolgozza a lenyomott billentyűket szimuláció futásakor.
        /// </summary>
        /// <param name="billentyu">Lenyomott billentyű.</param>
        private void Iranyit(ConsoleKeyInfo billentyu)
        {
            if (parancsok.Keres(new Par<ConsoleModifiers,ConsoleKey>(billentyu.Modifiers,billentyu.Key),
                out Parancs parancs))
            {
                parancs.Vegrehajt();
            }
        }
        /// <summary>
        /// Hozzáadja a következő parancsokat:
        /// <para>Q: 30 perc eltelik.</para>
        /// <para>W: 20 perc eltelik.</para>
        /// <para>E: 10 perc eltelik.</para>
        /// <para>R: 5 perc eltelik.</para>
        /// <para>T: 1 perc eltelik.</para>
        /// <para>balra kurzor: megállók előző oldalára lapoz.</para>
        /// <para>jobbra kurzor: megállók következő oldalára lapoz.</para>
        /// <para>CTRL-balra kurzor: személyek előző oldalára lapoz.</para>
        /// <para>CTRL-jobbra kurzor: személyek következő oldalára lapoz.</para>
        /// <para>ESC: visszalép a bekéréshez.</para>
        /// </summary>
        private void ParancsokatHozzad()
        {
            ParancsotHozzaad(0, ConsoleKey.Q, "Q: 30 perc", () => busz.Utazas.Ido.Eltelt(30));
            ParancsotHozzaad(0,ConsoleKey.W, "W: 20 perc", () => busz.Utazas.Ido.Eltelt(20));
            ParancsotHozzaad(0, ConsoleKey.E, "E: 10 perc", () => busz.Utazas.Ido.Eltelt(10));
            ParancsotHozzaad(0, ConsoleKey.R, "R: 5 perc", () => busz.Utazas.Ido.Eltelt(5));
            ParancsotHozzaad(0, ConsoleKey.T, "T: 1 perc", () => busz.Utazas.Ido.Eltelt(1));
            ParancsotHozzaad(0,ConsoleKey.LeftArrow, "<- Útvonal", () => megjelenito.Megallok.ElozoOldal());
            ParancsotHozzaad(0, ConsoleKey.RightArrow, "-> Útvonal", () => megjelenito.Megallok.KovetkezoOldal());
            ParancsotHozzaad(ConsoleModifiers.Control,ConsoleKey.LeftArrow, "CTRL+<- Személyek", () => megjelenito.Szemelyek.ElozoOldal());
            ParancsotHozzaad(ConsoleModifiers.Control,ConsoleKey.RightArrow, "CTRL+-> Személyek", () => megjelenito.Szemelyek.KovetkezoOldal());
            ParancsotHozzaad(0,ConsoleKey.Escape, "ESC vissza", () => vissza = true);
        }
        /// <summary>
        /// Hozzáad egy parancsot.
        /// </summary>
        /// <param name="modosito">Módosító billentyű.</param>
        /// <param name="billentyu">Billentyű.</param>
        /// <param name="leiras">Parancs szöveges leírása.</param>
        /// <param name="akcio">Parancs művelete.</param>
        private void ParancsotHozzaad(ConsoleModifiers modosito, ConsoleKey billentyu, string leiras, Akcio akcio)
        {
            parancsok.Hozzaad(new Parancs(new Par<ConsoleModifiers, ConsoleKey>(modosito,billentyu), leiras, akcio));
        }

        /// <summary>
        /// Bekéri a következő adatokat:
        /// <para>Generált megállók száma, 1 és 1000 között.</para>
        /// <para>Generált megállók minimális távolsága 10 és 50 között.</para>
        /// <para>Generált megállók maximális távolsága {minimalis} és 50 között.</para>
        /// <para>Generált utasok száma 0 és 100 között.</para>
        /// <para> Lekezeli, ha valamelyik utas nevét nem lehet betölteni vagy
        /// ha a generált útvonal nem lehetséges a tervező szerint. Ilyen esetben null a visszatérési érték.</para>
        /// </summary>
        /// <returns>Busz a bekért adatok alapján.</returns>
        private Busz Bekeresek()
        {
            Busz busz = null;
            try
            {
                do
                {
                    int megallok = Bekeres("Generált megállók száma", 1, 1000);
                    int tavMin = Bekeres("Generált megállók minimum távolsága", 10, 50);
                    int tavMax = Bekeres("Generált megállók maximum távolsága", tavMin, 50);
                    int utasok = Bekeres("Generált utasok száma", 0, 100);

                    busz = new Busz(new Utazas.Generalo(megallok, tavMin, tavMax, utasok).General());
                } while (busz == null);
            }
            catch(UtvonalTervezo.Kivetel ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            catch(KulcsKivetel<string> ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return busz;
        }
        /// <summary>
        /// Szimulációkat indít igenleges válaszok esetén.
        /// <para>Megkérdezi előszőr, hogy szeretne egy szimulációt a felhasználó. 
        /// Ha igen, akkor bekéri a busz adatait és ha nem lépett fel hiba a létrehozása közben,
        /// akkor elindítja.</para>
        /// <para>Szimuláció addig tart, 
        /// amíg a busz el nem éri az utolsó megállót vagy a felhasználó meg nem szakítja.</para> 
        /// </summary>
        public void Fut()
        {
            while (SzimulacioIndul())
            {
                vissza = false;
                busz=Bekeresek();
                if (busz != null)
                {
                    megjelenito.Betolt(busz);
                    void elerteAVegallomast(Busz b)
                    {
                        vissza = true;
                    }
                    busz.ElerteAVegallomast += elerteAVegallomast;
                    while (!vissza)
                    {
                        megjelenito.Megjelenites();
                        Iranyit(Console.ReadKey());
                    }
                    Console.Clear();
                    if (vissza)
                    {
                        Console.WriteLine("Az utazás véget ért!");
                        Console.ReadLine();
                        busz.ElerteAVegallomast -= elerteAVegallomast;
                    }
                }
            }
        }

        /// <summary>
        /// Szimulációt indít a megadott utazás alapján.
        /// <para>Szimuláció addig tart, 
        /// amíg a busz el nem éri az utolsó megállót vagy a felhasználó meg nem szakítja.</para> 
        /// </summary>
        /// <param name="utazas">Utazás, ami alapján létrehozza a szimulációt.</param>
        public void Lefuttat(Utazas utazas)
        {
            busz = new Busz(utazas);
            megjelenito.Betolt(busz);
            bool vege = false;
            
            EsemenyKezelo<Busz> vegallomas = (b) =>
            {
                vege = true;
            };

            busz.ElerteAVegallomast += vegallomas;
            while (!vege)
            {
                megjelenito.Megjelenites();
                Iranyit(Console.ReadKey());
            }
            busz.ElerteAVegallomast -= vegallomas;
            busz = null;
        }
 
        /// <summary>
        /// Lefuttat egy szimulációt és rögzíti megállónként az adatait egy pontosvesszővel tagolt táblázatban.
        /// </summary>
        /// <param name="utazas">Sziimuláció utazása.</param>
        /// <returns>Elkészült táblázat.</returns> 
        public string Tablazat(Utazas utazas)
        {
            Busz busz = new Busz(utazas);
            bool vege = false;
            var igenyPerDarab = busz.Utazas.Szemelyek.IgenyPerDarab();


            SzimulacioStatusz[] megallok = new SzimulacioStatusz[utazas.Utvonal.Darab];

            void elerteAMegallot(Busz b, Megallo m)
            {

                SzimulacioStatusz megallo = new SzimulacioStatusz(m, igenyPerDarab)
                {
                    ErkezesIdeje = b.Utazas.Ido.PontosIdo,
                    Megall = b.Utiterv.Megallasok.TartalmazKulcs(m),
                    UzemanyagSzintMegalloElott = b.UzemanyagSzint
                };
                int i = 0;

                foreach (var csoport in b.Utazas.Szemelyek.Csoportok())
                {
                    megallo.SzamlalokMegalloElott[i++] = (int)csoport.Elso.Szamlalo;
                    if (megallo.Megall)
                    {
                        megallo.Leszallok += csoport.Elso.Igeny.Megfelel(m) ? csoport.Darab : 0;
                    }
                }
                megallok[b.ElertMegallok] = megallo;
            }
          
            void elerteAVegallomast(Busz b)
            {
                vege = true;
            }
            busz.ElerteAMegallot += elerteAMegallot;
            busz.ElerteAVegallomast += elerteAVegallomast;

            while (!vege)
            {
                busz.Utazas.Ido.Eltelt(1);
            }
            busz.ElerteAMegallot -= elerteAMegallot;
            busz.ElerteAVegallomast -= elerteAVegallomast;

            return "Utiterv\r\n" + busz.Utiterv.Tablazat()+"Szimulacio\r\n" +MegalloStatusz.Tablazat(megallok, 0, megallok.Length - 1);
        }
        /// <summary>
        /// Lefuttat egy szimulációt és rögzíti megállónként az adatait egy pontosvesszővel tagolt táblázatban,
        /// majd fájlba írja.
        /// </summary>
        /// <param name="utvonal">Fájl útvonala.</param>
        /// <param name="utazas">Szimuláció utazása.</param>
        /// <returns>Elkészült táblázat.</returns> 
        public void Tablazat(string eleresiUt, Utazas utazas)
        {
            File.WriteAllText(eleresiUt, Tablazat(utazas));
        }
        /// <summary>
        /// Megkérdezi a felhasználót, hogy szeretne-e egy szimulációt.
        /// <para>Inputban nem különböztet meg a kis- és nagybetüt. </para>
        /// <para> Igazzal tér vissza a következő inputok esetén: "i","igen" </para> 
        /// <para> Hamissal tér vissza a következő inputok esetén: "n","nem" </para> 
        /// <para> Más esetben megkérdezi újra. </para> 
        /// </summary>
        private bool SzimulacioIndul()
        {
            string input;
            do
            {
                Console.WriteLine("Szeretnél egy szimulációt? [I]gen [N]em");
                input = Console.ReadLine().ToLower();
            } while (!(input == "i" || input == "igen" || input == "n" || input == "nem"));

            return input == "i" || input == "igen";
        }
    }
}