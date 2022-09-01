using System;
using System.IO;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Tervezes
{
    /// <summary>
    /// UtvonalTervezo terméke.
    /// </summary>
    internal class Utiterv
    {
        /// <summary>
        /// Tervezőtől kapott státuszok.
        /// </summary>
        TervezoStatusz[] tervezoStatuszok;
        /// <summary>
        /// Tervezett üzemanyagfogyasztás az út során.  
        /// </summary>
        public float ElhasznaltUzemanyagOsszesen { get; private set; }
        /// <summary>
        /// Megállások helye és tervezett ideje.
        /// </summary>
        public OlvashatoHashTabla<Megallo,DateTime> Megallasok { get; private set; }
        /// <summary>
        /// Az útvonal hossza.
        /// </summary>
        public int TavOsszesen { get; private set; }
        /// <summary>
        /// Mennyi ideig tartott elkészíteni a tervezőjének.
        /// </summary>
        public TimeSpan TervezesHossza { get; }
        /// <summary>
        /// Tervezett utazási idő.
        /// </summary>
        public TimeSpan UtazasiIdoOsszesen { get; private set; }
        /// <summary>
        /// Elkészít egy útitervet.
        /// </summary>
        /// <param name="kiszamitasiIdo">Elkészítésének ideje.</param>
        /// <param name="tervezoStatuszok">Tervező státuszai.</param>
        public Utiterv(TimeSpan kiszamitasiIdo, TervezoStatusz[] tervezoStatuszok)
        {
            TervezesHossza = kiszamitasiIdo;
            this.tervezoStatuszok = tervezoStatuszok;
            Inicializal();
        }
        /// <summary>
        /// Inicializálja a tagjait a kapott státuszokból. 
        /// Kiszámítja az utazási időt, a távot, az elhasznált üzemanyagokat és közben elmenti a megállókat érkezéssel.
        /// </summary>
        private void Inicializal()
        {
            UtazasiIdoOsszesen = TimeSpan.Zero;
            TavOsszesen = 0;
            ElhasznaltUzemanyagOsszesen = 0;
            HashTabla<Megallo, DateTime> megallasok = new HashTabla<Megallo, DateTime>();
            for (int i = 1; i < tervezoStatuszok.Length; i++)
            {
                TervezoStatusz statusz = tervezoStatuszok[i];
                UtazasiIdoOsszesen += TimeSpan.FromMinutes(statusz.ElteltPercUtazassal);
                TavOsszesen += statusz.Megallo.Tavolsag;
                ElhasznaltUzemanyagOsszesen += statusz.ElhasznaltUzemanyag;
                if (statusz.Megall)
                {
                    megallasok.Hozzaad(new Par<Megallo,DateTime>(statusz.Megallo,statusz.ErkezesIdeje));
                    UtazasiIdoOsszesen += TimeSpan.FromMinutes(statusz.Leszallok);
                }
            }
            Megallasok = new OlvashatoHashTabla<Megallo, DateTime>(megallasok);
        }
        /// <summary>
        /// Elkészít egy pontosvesszővel tagolt szöveges táblázatot a tervezett adatokból.
        /// </summary>
        /// <returns>Szöveges táblazat.</returns>
        public string Tablazat()
        {
           return MegalloStatusz.Tablazat(tervezoStatuszok, 1, tervezoStatuszok.Length - 1);
        }
        /// <summary>
        /// Fájlba ír egy pontosvesszővel tagolt szöveges táblázatot a tervezett adatokból.
        /// </summary>
        /// <param name="utvonal">Fájl útvonala.</param>
        public void Tablazat(string eleresiUt)
        {
            File.WriteAllText(eleresiUt, Tablazat());
        }

    }
}