using System;
using System.Diagnostics;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Tervezes
{
    /// <summary>
    /// Útvonal megtervezésért felelős osztály.
    /// </summary>
    internal abstract class UtvonalTervezo
    {
        /// <summary>
        /// Kivétel, amit akkor dob az útvonaltervező,
        /// ha nem talált lehetséges útitervet, mert a busz minden esetben kifogy az üzemanyagból.
        /// </summary>
        public class Kivetel:Exception
        {
            /// <summary>
            /// Tervező státuszai
            /// </summary>
            public TervezoStatusz[] Statuszok { get; }
            /// <summary>
            /// Beállítja a kiváltó tervező státuszait. 
            /// </summary>
            /// <param name="statuszok">Kiváltó tervező státuszai</param>
            public Kivetel(TervezoStatusz[] statuszok):base("Útvonal nem lehetséges!")
            {
                Statuszok = statuszok;
            }
        }
        /// <summary>
        /// Elkészített útiterv.
        /// </summary>
        private Utiterv utiterv;
        /// <summary>
        /// Buszon utazó személyek darabszáma igény szerint csoportosítva.
        /// </summary>
        protected Par<Igeny, int>[] igenyPerDarab;
        
        /// <summary>
        /// Tervező státuszokból állő tömb. Ebben tartja nyílván az adatokat tervezéskor.
        /// </summary>
        protected TervezoStatusz[] tervezoStatuszok;
        /// <summary>
        /// Utázás, ami alapján megtervezi az útitervet.
        /// </summary>
        protected Utazas utazas;

        /// <summary>
        /// Incializálja az útvonaltervezőt.
        /// </summary>
        /// <param name="utazas">Utázás, ami alapján megtervezi az útitervet.</param>
        public UtvonalTervezo(Utazas utazas)
        {
            Incializal(utazas);
        }

        /// <summary>
        /// Utazásból inicializálja a státuszokat és az igényPerDarabot.
        /// <para>Státuszok eggyel több elemet tartalmaz, mint az utazás útvonala, 
        /// mert a 0. indexén egy strázsa elem áll. 
        /// Erre azért van szükség, hogy ne kelljen a legelső elemnél bonyolult elágazásokkal lekezelni a frissítést.</para>
        /// </summary>
        /// <param name="utazas">Utázás, ami alapján megtervezi az útitervet.</param>
        protected virtual void Incializal(Utazas utazas)
        {
            this.utazas = utazas;
            igenyPerDarab = utazas.Szemelyek.IgenyPerDarab();
            tervezoStatuszok = new TervezoStatusz[utazas.Utvonal.Darab + 1];
            int i = 0;
            tervezoStatuszok[i++] = TervezoStatusz.StrazsaElem(utazas.Ido.IndulasIdeje, igenyPerDarab);
            foreach (var megallo in utazas.Utvonal)
            {
                tervezoStatuszok[i++] = new TervezoStatusz(megallo, igenyPerDarab);
            }
        }
        /// <summary>
        /// Frissíti az adott indexen lévő státuszt az azt megelőzővel.
        /// </summary>
        /// <param name="index">Index, ahol frissíteni fogja a státuszt.</param>
        protected void FrissitStatusz(int index)
        {
            tervezoStatuszok[index].Frissit(tervezoStatuszok[index - 1]);
        }
        /// <summary>
        /// Frissíti az összes státuszt a strázsa elem kivételével.
        /// </summary>
        protected void FrissitStatuszok()
        {
            FrissitStatuszok(1, tervezoStatuszok.Length - 1);
        }
        /// <summary>
        /// Frissíti a státuszokat egy megadott határon belül.
        /// </summary>
        /// <param name="mettol">Mettől frissítsen.</param>
        /// <param name="meddig">Meddig firssítsen.</param>
        protected void FrissitStatuszok(int mettol, int meddig)
        {
            for (int i = mettol; i <= meddig; i++)
            {
                FrissitStatusz(i);
            }
        }
        /// <summary>
        /// Frissíti a státuszokat egy megadott indextől.
        /// </summary>
        /// <param name="mettol">Mettől frissítsen.</param>
        protected void FrissitStatuszok(int mettol)
        {
            FrissitStatuszok(mettol, tervezoStatuszok.Length - 1);
        }
        /// <summary>
        /// Tervezés metódusa. Ezt fogják implementálni a leszármazottak.
        /// </summary>
        protected abstract void Tervez();

        /// <summary>
        /// Elkészíti az útitervet, ha még nincs elkészítve és visszatér vele. 
        /// <para>Ha nem találta lehetséges útitervet, akkor UtvonalTervezo.Kivetelt dob.</para>
        /// </summary>
        public Utiterv KeszitUtiterv()
        {
            if (utiterv == null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Tervez();
                sw.Stop();

                utiterv = new Utiterv(sw.Elapsed, tervezoStatuszok);
            }

            return utiterv;
        }
    }
}