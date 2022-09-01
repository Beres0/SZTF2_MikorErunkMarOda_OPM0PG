using System;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.Tervezes;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Sofőrt reprezentáló osztály.
    /// </summary>
    internal class Sofor : Szemely
    {
        /// <summary>
        /// Ennyi sofőr tartkózkadhat a buszon.
        /// </summary>
        public const int SoforDarab = 2;
        /// <summary>
        /// Sofőrök igénye: Bármilyen megállónál társávál cserélne maximum 240 percenként.
        /// </summary>
        public static readonly Igeny SoforIgeny = new Igeny(Szolgaltatas.Semmi, 240);
       
        /// <summary>
        /// Letelt számlálókor "Cserélne".
        /// </summary>
        protected override string LeteltSzamlaloAllapot => "Cserélne";
        /// <summary>
        /// Ha ő vezető, akkor "Vezet", egyébként "Utazik".
        /// </summary>
        protected override string UtazoAllapot => Busz.VezetoSofor == this ? "Vezet" : base.UtazoAllapot;
        public override Igeny Igeny => SoforIgeny;
        /// <summary>
        /// Mennyi ideig áll a megállónál.
        /// </summary>
        public int Varakozas { get; private set; }

        /// <summary>
        /// Esemény, ami akkor váltódik ki, ha vezet és sebességet változtat.
        /// </summary>
        public event EsemenyKezelo<Sofor, int> SebessegetValtoztat;
        /// <summary>
        /// Elkészít egy sofőrt.
        /// </summary>
        /// <param name="nev">Sofőr neve.</param>
        public Sofor(string nev) : base(nev)
        {
        }
        /// <summary>
        /// Készít egy sofőrt véletlenszerűen választott névvel.
        /// <para> Ha valamiért nem sikerült betölteni a sofőrökre jellemző neveket, akkor Szemely.Kivetelt dob.</para>
        /// </summary>
        /// <param name="random">Random, ami a nevet kiválasztja</param>
        public Sofor(Random random):base(random)
        {

        }
        /// <summary>
        /// Busz.ElereAMegallot eseményre iratkozik fel.
        /// <para> Ha vezet és a busz elért egy olyan megállót, ahol az útiterv szerint meg kell állni, akkor megállítja a buszt.</para>
        /// </summary>
        /// <param name="kuldo">Busz, amin tarkózkodik</param>
        /// <param name="argumentum">Megálló, amite elért a busz.</param>
        private void Busz_ElerteAMegallot(Busz kuldo, Megallo argumentum)
        {
            if (Busz.VezetoSofor == this)
            {
                if (kuldo.Utiterv.Megallasok.TartalmazKulcs(argumentum))
                {
                    SebessegetValtoztat?.Invoke(this, 0);
                }
            }
        }
        /// <summary>
        /// Busz.SzemelyIdeiglenesenLeszallt eseményre iratkozik fel.
        /// <para> Figyeli a leszálló utasokat és növeli a megállónál a várakozás idejét.</para>
        /// </summary>
        /// <param name="kuldo">Busz, amin tarkózkodik.</param>
        /// <param name="argumentum">Személy, aki leszállt.</param>
        private void Busz_SzemelyIdeiglenesenLeszallt(Busz kuldo, Szemely argumentum)
        {
            Varakozas++;
        }
        /// <summary>
        /// Következő eseményekre iratkozik fel azonos nevű metódusaival:
        /// <para> Busz.Megallt. </para>
        /// <para> Busz.Utazas.Ido.ElteltEgyPerc </para>
        /// <para> Busz.ElerteAVegallomast </para>
        /// <para> Busz.SzemelyIdeiglesenLeszallt </para>
        /// <para> Busz.ElerteAMegallot </para>
        /// </summary>
        /// <param name="busz"> Busz, amire a sofőr felszállt. </param>
        protected override void Feliratkozasok(Busz busz)
        {
            base.Feliratkozasok(busz);
            busz.SzemelyIdeiglenesenLeszallt += Busz_SzemelyIdeiglenesenLeszallt;
            busz.ElerteAMegallot += Busz_ElerteAMegallot;
        }
        /// <summary>
        /// Busz.Utazas.Ido.Eltelt eseményre iratkozik fel.
        /// <para> Percenként csökkenti a számlálót. </para>
        /// <para> Ha még várakozik a megállónál és a várakozás nagyobb, mint 1, akkor percenként csökkenti.
        /// Különben ha vezet, akkor lekérdezi a megállótól a sebességkorlátot és ha a busz sebessége nem egyezik vele,
        /// megváltoztatja a sebességét.</para>
        /// </summary>
        /// <param name="ido">Busz ideje.</param>
        protected override void Ido_ElteltEgyPerc(Ido ido)
        {
            base.Ido_ElteltEgyPerc(ido);
            if (Varakozas >1)
            {
                Varakozas--;
            }
            else if (Busz.VezetoSofor == this)
            {
                int sebessegHatar = Busz.Feletart.SebessegKorlat(Busz.TavolsagAKovetkezotol);
                if (sebessegHatar != Busz.Sebesseg)
                {
                    SebessegetValtoztat?.Invoke(this, sebessegHatar);
                }
            }
        }
        /// <summary>
        /// Következő eseményekrről iratkozik le azonos nevű metódusaival:
        /// <para> Busz.Megallt. </para>
        /// <para> Busz.Utazas.Ido.ElteltEgyPerc </para>
        /// <para> Busz.ElerteAVegallomast </para>
        /// <para> Busz.SzemelyIdeiglesenLeszallt </para>
        /// <para> Busz.ElerteAMegallot </para>
        /// </summary>
        /// <param name="busz"> Busz, amiről a sofőr leszállt. </param>
        protected override void Leiratkozasok(Busz busz)
        {
            base.Leiratkozasok(busz);
            busz.SzemelyIdeiglenesenLeszallt += Busz_SzemelyIdeiglenesenLeszallt;
            busz.ElerteAMegallot += Busz_ElerteAMegallot;
        }
        /// <summary>
        /// 30 megálló felett MohoUtvonalTervezot, 30 vagy kevesebb megállónál BacktrackUtvonaltervezot készít.
        /// </summary>
        /// <param name="utazas"> Utazás, ami alapján az útiterv készül.</param>
        public Utiterv KeszitUtiterv(Utazas utazas)
        {
            Utiterv utiterv = utazas.Utvonal.Darab > 30 ?
                      new MohoUtvonalTervezo(utazas).KeszitUtiterv() : new BactrackUtvonalTervezo(utazas).KeszitUtiterv();
            return utiterv;
        }
    }
}