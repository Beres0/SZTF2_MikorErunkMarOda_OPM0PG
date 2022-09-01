using System;
using System.IO;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Tervezes
{
    /// <summary>
    /// UtvonalTervezo implementációjának működéséről állít össze statisztikát.
    /// </summary>
    internal class StatisztikaKeszito<T> where T:UtvonalTervezo
    {
        /// <summary>
        /// Példányosító függvény.
        /// </summary>
        private Fuggveny<Utazas, T> peldanyosito;
        /// <summary>
        /// Elkészít egy statisztika készítőt.
        /// </summary>
        /// <param name="peldanyosito">Példányosító függvény, amivel létrehozza a vizsgált útvonaltervező típust.</param>
        public StatisztikaKeszito(Fuggveny<Utazas,T> peldanyosito)
        {
            this.peldanyosito =peldanyosito;
        }
        /// <summary>
        /// Értesít, ha haladt az összehasonlítás.
        /// </summary>
        public event EsemenyKezelo<StatisztikaKeszito<T>, int> OsszehasonlitasHaladt;
        
        /// <summary>
        /// Értesít, ha haladt a statisztika.
        /// </summary>
        public event EsemenyKezelo<StatisztikaKeszito<T>, int> StatisztikaHaladt;
        /// <summary>
        /// Elkészíti a statisztikát.
        /// </summary>
        /// <param name="generalasokSzama">Hány generálás alapján készítse el.</param>
        /// <param name="generalo">Milyen parametérek alapján generáljon utazásokat az útvonaltervezőnek.</param>
        /// <returns></returns>
        public Statisztika Keszit(int generalasokSzama, Utazas.Generalo generalo)
        {
            LancoltLista<Utiterv> utitervek = new LancoltLista<Utiterv>();

            for (int i = 0; i < generalasokSzama; i++)
            {
                Utazas u = generalo.General();
                UtvonalTervezo tervezo = peldanyosito(u);
                Utiterv utiterv = null;
                try
                {
                    utiterv = tervezo.KeszitUtiterv();
                }
                catch (UtvonalTervezo.Kivetel)
                {
                }
                utitervek.Hozzaad(utiterv);
            }
            return new Statisztika(generalasokSzama, generalo, utitervek);
        }
        /// <summary>
        /// Több összehasonlítást végez a másik tervező tipusával és elmenti az erdeményeket egy fájlba.
        /// <para>Egy megállóról indulva minden összehasonlítás után növeli a megallók számát,
        /// míg el nem éri a generáló paraméterben megadott megállószámot.</para>
        /// </summary>
        /// <param name="utvonal">Fájl útvonala.</param>
        /// <param name="generalasokSzama">Egy iterációban mennyi generálás legyen.</param>
        /// <param name="megalloNoveles">Iteráció után mennyivel emelkedjen a megállók darabszáma</param>
        /// <param name="generalo">Generáló megállószáma itt az iterációk maximumát jelenti.</param>
        /// <returns>Statisztikák táblázata.</returns>
        public void OsszehasonlitoTablazat<T2>
            (string eleresiUt, StatisztikaKeszito<T2> masik, int generalasokSzama, int megalloNoveles, Utazas.Generalo generalo)
            where T2 : UtvonalTervezo
        {
            File.WriteAllText(eleresiUt, OsszehasonlitoTablazat(masik, generalasokSzama, megalloNoveles, generalo));
        }
        /// <summary>
        /// Több összehasonlítást végez a másik tervező tipusával és elmenti az erdeményeket egy szöveges pontosvesszővel tagolt táblázatba.
        /// <para>Egy megállóról indulva minden összehasonlítás után növeli a megallók számát,
        /// míg el nem éri a generáló paraméterben megadott megállószámot.</para>
        /// </summary>
        /// <param name="generalasokSzama">Egy iterációban mennyi generálás legyen.</param>
        /// <param name="megalloNoveles">Iteráció után mennyivel emelkedjen a megállók darabszáma</param>
        /// <param name="generalo">Generáló megállószáma itt az iterációk maximumát jelenti.</param>
        /// <returns>Statisztikák táblázata.</returns>
        public string OsszehasonlitoTablazat<T2> 
        ( StatisztikaKeszito<T2> masik, int generalasokSzama, int megalloNoveles, Utazas.Generalo generalo)
            where T2 : UtvonalTervezo
        {
            StringBuilder sb = new StringBuilder();
                sb.AppendLine(generalo.KeszitRekord(new Par<string,string>(nameof(generalasokSzama),generalasokSzama.ToString()),
                    new Par<string,string>(nameof(megalloNoveles),megalloNoveles.ToString())));
                sb.AppendLine();
                sb.AppendLine($";{typeof(T).Name};;;{typeof(T2).Name};;");
                sb.Append($"MegallokDarab;");
                sb.Append(Statisztika.Rekord.Fejlecek().Osszefuz(";") + ";");
                sb.AppendLine(Statisztika.Rekord.Fejlecek().Osszefuz(";"));

                for (int i = 1; i <= generalo.MegalloDarab; i += megalloNoveles)
                {
                    var p = new Utazas.Generalo(i, generalo.TavolsagMin, generalo.TavolsagMax, generalo.UtasDarab);
                    var eredmeny = Osszehasonlit(masik, generalasokSzama, p);
                    OsszehasonlitasHaladt?.Invoke(this, i);
                    sb.Append($"{i};");
                    sb.Append(Statisztika.Rekord.Ertekek(eredmeny.t1).Osszefuz(";") + ";");
                    sb.AppendLine(Statisztika.Rekord.Ertekek(eredmeny.t2).Osszefuz(";"));
                }
            return sb.ToString();
        }
        /// <summary>
        /// Több statisztikát készít egymás után és elmenti fájlba. 
        /// <para>Egy megállóról indulva minden elkészülés után növeli a megallók számát,
        /// míg el nem éri a generáló paraméterben megadott megállószámot.</para>
        /// </summary>
        /// <param name="utvonal">Fájl útvonala.</param>
        /// <param name="generalasokSzama">Egy iterációban mennyi generálás legyen.</param>
        /// <param name="megalloNoveles">Iteráció után mennyivel emelkedjen a megállók darabszáma</param>
        /// <param name="generalo">Generáló megállószáma itt az iterációk maximumát jelenti.</param>
        /// <returns>Statisztikák táblázata.</returns>
        public void StatisztikaTablazat(string eleresiUt,int generalasokSzama, int megalloNoveles, Utazas.Generalo generalo)
        {
            File.WriteAllText(eleresiUt, StatisztikaTablazat(generalasokSzama, megalloNoveles, generalo));
        }
        /// <summary>
        /// Több statisztikát készít egymás után és elmenti az erdeményeket egy szöveges pontosvesszővel tagolt táblázatba. 
        /// <para>Egy megállóról indulva minden elkészülés után növeli a megallók számát,
        /// míg el nem éri a generáló paraméterben megadott megállószámot.</para>
        /// </summary>
        /// <param name="generalasokSzama">Egy iterációban mennyi generálás legyen.</param>
        /// <param name="megalloNoveles">Iteráció után mennyivel emelkedjen a megállók darabszáma</param>
        /// <param name="generalo">Generáló megállószáma itt az iterációk maximumát jelenti.</param>
        /// <returns>Statisztikák táblázata.</returns>
        public string StatisztikaTablazat(int generalasokSzama, int megalloNoveles, Utazas.Generalo generalo)
        {
            StringBuilder sb = new StringBuilder();
                sb.AppendLine(generalo.KeszitRekord(new Par<string, string>(nameof(generalasokSzama), generalasokSzama.ToString()),
                   new Par<string, string>(nameof(megalloNoveles), megalloNoveles.ToString())));
                sb.AppendLine();

                sb.AppendLine(typeof(T).Name);
                sb.Append("MegallokDarab;");
                sb.AppendLine(Statisztika.Rekord.Fejlecek().Osszefuz(";"));
                for (int i = 1; i <= generalo.MegalloDarab; i += megalloNoveles)
                {
                    var p = new Utazas.Generalo
                    (i, generalo.TavolsagMin, generalo.TavolsagMax, generalo.UtasDarab,null,null);
                    var eredmeny = Keszit(generalasokSzama, p);
                    StatisztikaHaladt?.Invoke(this, i);
                    sb.Append($"{i};");
                    sb.AppendLine(Statisztika.Rekord.Ertekek(eredmeny).Osszefuz(";"));
                }
            return sb.ToString();
        }
        /// <summary>
        /// Összehasonlítja a tervezőtipusát a másik statisztika készítő tervezőtipusával.
        /// </summary>
        /// <param name="masik">Másik statisztika készítő</param>
        /// <param name="generalasokSzama">Hány generálásból álljon az összehasonlítás</param>
        /// <param name="generalo">Milyen parametérek alapján generáljon utazásokat az összehasonlításhoz.</param>
        /// <returns>t1 a saját statisztikája, t2 a másiké.  </returns>
        public (Statisztika t1, Statisztika t2) Osszehasonlit<T2>(StatisztikaKeszito<T2> masik, int generalasokSzama, Utazas.Generalo generalo)
            where T2:UtvonalTervezo
        {
            LancoltLista<Utiterv> utitervek1 = new LancoltLista<Utiterv>();
            LancoltLista<Utiterv> utitervek2 = new LancoltLista<Utiterv>();

            for (int i = 0; i < generalasokSzama; i++)
            {
                Utazas utazas = generalo.General();
                UtvonalTervezo tervezo1 = peldanyosito(utazas);
                UtvonalTervezo tervezo2 = masik.peldanyosito(utazas);
                Utiterv utiterv1 = null;
                Utiterv utiterv2 = null;

                try
                {
                    utiterv1 = tervezo1.KeszitUtiterv();
                    utiterv2 = tervezo2.KeszitUtiterv();
                }
                catch (UtvonalTervezo.Kivetel)
                {
                }
                utitervek1.Hozzaad(utiterv1);
                utitervek2.Hozzaad(utiterv2);
            }

            return (new Statisztika(generalasokSzama, generalo, utitervek1), new Statisztika(generalasokSzama, generalo, utitervek2));
        }
    }
}