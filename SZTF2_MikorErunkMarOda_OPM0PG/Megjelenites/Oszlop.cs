using System;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megjelenites
{
    /// <summary>
    /// Megjelenítés táblázatának oszlopa
    /// </summary>
    internal class Oszlop<T>
    {
        /// <summary>
        /// Adat formátuma.
        /// </summary>
        private Fuggveny<T, string> adat;
        /// <summary>
        /// Oszlop szélessége.
        /// </summary>
        private int szelesseg;
        /// <summary>
        /// Oszlop igazítása.
        /// </summary>
        public Igazitas Igazitas { get; private set; }
        /// <summary>
        /// Oszlop neve.
        /// </summary>
        public string Nev { get; }
        /// <summary>
        /// Oszlop szélessége
        /// </summary>
        public int Szelesseg
        {
            get => szelesseg;
            private set
            {
                szelesseg = value;
                Ures = new string(' ', szelesseg);
            }
        }
        /// <summary>
        /// Azonos szélességű üres karakterlánc.
        /// </summary>
        public string Ures { get; private set; }
        /// <summary>
        /// Elkészít egy oszlopot.
        /// </summary>
        /// <param name="nev">Oszlop neve.</param>
        public Oszlop(string nev)
        {
            Nev = nev;
            Szelesseg = nev.Length + 2;
            Igazitas = Igazitas.Bal;
            adat = (t) => t.ToString();
        }
        /// <summary>
        /// Visszaadja az elem adatát igazított szövegként 
        /// </summary>
        /// <param name="elem">Elem, amiből előáll az adat</param>
        public string Adat(T elem)
        {
            string str = adat(elem);
            if (str.Length > Szelesseg)
            {
                str = str.Substring(0, Szelesseg);
            }
            return str.Igazit(Igazitas, Szelesseg);
        }
        /// <summary>
        /// Beállítja, hogy az oszlop milyen módon nyerje ki az elemből az adatot.
        /// </summary>
        /// <param name="adat">Szöveggé alakító függvény.</param>
        public Oszlop<T> AdatBeallit(Fuggveny<T, string> adat)
        {
            if (adat is null)
            {
                throw new ArgumentNullException(nameof(adat));
            }
            this.adat = adat;
            return this;
        }
        /// <summary>
        /// Visszaadja az elem adatát.
        /// </summary>
        /// <param name="elem"></param>
        public string AdatIgazitasNelkul(T elem)
        {
            return adat(elem);
        }
        /// <summary>
        /// Visszaadja az oszlop nevét igazított szövegként.
        /// </summary>
        public string Fejlec()
        {
            string str = Nev;
            if (str.Length > Szelesseg)
            {
                str = str.Substring(0, Szelesseg);
            }
            return str.Igazit(Igazitas, Szelesseg);
        }
        /// <summary>
        /// Beállítja az oszlop igazítását.
        /// </summary>
        /// <param name="igazitas">Igazítás típusa.</param>
        public Oszlop<T> IgazitasBeallit(Igazitas igazitas)
        {
            Igazitas = igazitas;
            return this;
        }
        /// <summary>
        /// Beállítja az oszlop szélességét.
        /// </summary>
        /// <param name="szelesseg">Beállított szélesség. Ha az érték kisebb-egyenlő 0, ArgumentOutOfRangeExceptiont dob.</param>
        public Oszlop<T> SzelessegBeallit(int szelesseg)
        {
            if (szelesseg <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(szelesseg));
            }
            Szelesseg = szelesseg;
            return this;
        }
    }
}