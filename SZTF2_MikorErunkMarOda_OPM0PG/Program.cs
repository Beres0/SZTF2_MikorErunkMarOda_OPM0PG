using SZTF2_MikorErunkMarOda_OPM0PG.
    Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megjelenites;
using SZTF2_MikorErunkMarOda_OPM0PG.Tervezes;
using System;
using System.Collections.Generic;
using System.Collections;
using SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi;
using System.IO;

namespace SZTF2_MikorErunkMarOda_OPM0PG
{
    internal class Program
    {
        /// <summary>
        /// Fájlba írja a moho statisztikát IOMuveletek\Statisztika\moho.csv elérési útvonalon.
        /// <para>Generálási paramétérek: </para>
        /// <para> - generálások száma=20 </para>
        /// <para> - megalloNoveles=10: </para>
        /// <para> - megalloDarab=1001 </para>
        /// <para> - tavolsagMin=10 </para>
        /// <para> - tavolsagMax=40 </para>
        /// <para> - utasDarab=40 </para>
        /// </summary>
        private static void MohoStatisztikaTablazat()
        {
            StatisztikaKeszito<MohoUtvonalTervezo> moho =
               new StatisztikaKeszito<MohoUtvonalTervezo>((u) => new MohoUtvonalTervezo(u));

            string eleresiUt = IOMuveletek.KeszitMappanBeluliEleresiUtat(IOMuveletek.Statisztika, "moho.csv");
            int generalasokSzama = 20; int megalloNoveles = 10;
            Utazas.Generalo generalo = new Utazas.Generalo(megalloDarab: 1001, tavolsagMin: 10, tavolsagMax: 40, utasDarab: 40);
            
            void haladt(StatisztikaKeszito<MohoUtvonalTervezo> s, int i)
            {
                Console.WriteLine($"{eleresiUt} készül\n{i}/{generalo.MegalloDarab}");
            }

            moho.StatisztikaHaladt += haladt;
            moho.StatisztikaTablazat(eleresiUt, generalasokSzama, megalloNoveles, generalo);
            moho.StatisztikaHaladt -= haladt;
            Console.WriteLine("Kész!");
        }
        /// <summary>
        /// Fájlba írja a moho és backtrack összehasonlítást IOMuveletek\Statisztika\osszehasonlitas.csv elérési útvonalon.
        /// <para> Generálási paramétérek: </para>
        /// <para> - generálások száma=5 </para>
        /// <para> - megalloNoveles=1 </para>
        /// <para> - megalloDarab=45 </para>
        /// <para> - tavolsagMin=10 </para>
        /// <para> - tavolsagMax=40 </para>
        /// <para> - utasDarab=40 </para>
        /// </summary>
        private static void MohoBacktrackOsszehasonlitoTablazat()
        {
            StatisztikaKeszito<MohoUtvonalTervezo> moho =
                new StatisztikaKeszito<MohoUtvonalTervezo>((u) => new MohoUtvonalTervezo(u));
            StatisztikaKeszito<BactrackUtvonalTervezo> backtrack =
                new StatisztikaKeszito<BactrackUtvonalTervezo>((u) => new BactrackUtvonalTervezo(u));

            string eleresiUt = IOMuveletek.KeszitMappanBeluliEleresiUtat(IOMuveletek.Statisztika, "osszehasonlitas.csv");
            int generalasokSzama = 5; int megalloNoveles = 1;
            Utazas.Generalo generalo = new Utazas.Generalo(megalloDarab: 45, tavolsagMin: 10, tavolsagMax: 40, utasDarab: 40);

            void haladt(StatisztikaKeszito<MohoUtvonalTervezo> s, int i)
            {
                Console.WriteLine($"{eleresiUt} készül\n{i}/{generalo.MegalloDarab}");
            };
           

            moho.OsszehasonlitasHaladt += haladt;
            moho.OsszehasonlitoTablazat(eleresiUt, backtrack, generalasokSzama, megalloNoveles, generalo);
            moho.OsszehasonlitasHaladt -= haladt;
            Console.WriteLine("Kész!");
        }
        /// <summary>
        /// Fájlba írja két véletlenszerűen generált utazás szimulációjának táblázatát.
        /// <para> Elérési út: IOMuveletek\Debug\szim1.csv</para>
        /// <para> Generálási paramétérek: </para>
        /// <para> - megalloDarab=30 </para>
        /// <para> - tavolsagMin=10 </para>
        /// <para> - tavolsagMax=40 </para>
        /// <para> - utasDarab=40 </para>
        /// <para> Elérési út: IOMuveletek\Debug\szim2.csv</para>
        /// <para> Generálási paramétérek: </para>
        /// <para> - megalloDarab=80 </para>
        /// <para> - tavolsagMin=10 </para>
        /// <para> - tavolsagMax=40 </para>
        /// <para> - utasDarab=20 </para>
        /// </summary>
        public static void SzimulacioTablazatok()
        {
            Szimulacio szim = new Szimulacio();
            Utazas.Generalo gen1 = new Utazas.Generalo(megalloDarab: 30, tavolsagMin: 10, tavolsagMax: 40, utasDarab: 40);
            Utazas.Generalo gen2 = new Utazas.Generalo(megalloDarab: 80, tavolsagMin: 10, tavolsagMax: 30, utasDarab: 20);
            string eleresiUt1 = IOMuveletek.KeszitMappanBeluliEleresiUtat(IOMuveletek.Debug, "szim1.csv");
            string eleresiUt2 = IOMuveletek.KeszitMappanBeluliEleresiUtat(IOMuveletek.Debug, "szim2.csv");

            szim.Tablazat(eleresiUt1,gen1.General());
            szim.Tablazat(eleresiUt2,gen2.General());
            Console.WriteLine(eleresiUt1+" kész!");
            Console.WriteLine(eleresiUt2+" kész!");

        }
        /// <summary>
        /// Elkészíti a következő fájlokat:
        ///  <para>IOMuveletek\Statisztika\moho.csv</para>
        ///  <para>IOMuveletek\Statisztika\osszehasonlitas.csv</para>
        ///  <para>IOMuveletek\Debug\szim1.csv</para>
        ///  <para>IOMuveletek\Debug\szim2.csv</para>
        /// </summary>
        private static void OsszesFajl()
        {

            MohoStatisztikaTablazat();
            MohoBacktrackOsszehasonlitoTablazat();
            SzimulacioTablazatok();
        }
        /// <summary>
        /// Elindítja a szimulációt.
        /// </summary>
        private static void SzimulaciotIndit()
        {
            new Szimulacio().Fut();
        }
     
        private static void Main(string[] args)
        {
            SzimulaciotIndit();
        }
    }
}