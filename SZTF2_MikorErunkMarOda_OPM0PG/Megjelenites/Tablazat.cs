using System;
using System.Collections.Generic;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megjelenites
{
    /// <summary>
    /// Megjelenítés táblázata.
    /// </summary>
    internal class Tablazat<T> : Lista<T>
    {
        /// <summary>
        /// Maximális elem egy oldalon;
        /// </summary>
        private int maxElem;

        /// <summary>
        /// Jelenlegi oldal indexe.
        /// </summary>
        public int JelenlegiOldal { get; private set; }
        
        /// <summary>
        /// Mennyi elem lehet maximum egy oldalon.
        /// <para> Ha a beállított érték kisebb, mint 1, akkor ArgumentOutOfRangeException-t dob. </para>
        /// </summary>
        public int MaxElemEgyOldalon
        {
            get => maxElem;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(MaxElemEgyOldalon));
                maxElem = value;
            }
        }
        /// <summary>
        /// Táblázat neve.
        /// </summary>
        public string Nev { get; }
        /// <summary>
        /// Táblázat oszlopai.
        /// </summary>
        public Lista<Oszlop<T>> Oszlopok { get; }
        /// <summary>
        /// Elkészít egy táblázatot.
        /// </summary>
        /// <param name="nev">Táblazat neve.</param>
        /// <param name="maxElemEgyOldalon">Mennyi elem lehet maximum egy oldalon.</param>
        public Tablazat(string nev, int maxElemEgyOldalon = 30)
        {
            Nev = nev;
            Oszlopok = new Lista<Oszlop<T>>();
            MaxElemEgyOldalon = maxElemEgyOldalon;
        }
        /// <summary>
        /// Elkészít egy táblázatot és feltölti a megadott kollekcióból.
        /// </summary>
        /// <param name="nev">Táblázat neve.</param>
        /// <param name="kollekcio">Kollekció, amiből feltölti az elemeket. </param>
        /// <param name="maxElemEgyOldalon">Mennyi elem lehet maximum egy oldalon.</param>

        public Tablazat(string nev, IOlvashatoKollekcio<T> kollekcio, int maxElemEgyOldalon = 30) : this(nev, maxElemEgyOldalon)
        {
            Nev = nev;
            foreach (var elem in kollekcio)
            {
                Hozzaad(elem);
            }
        }
      
        /// <summary>
        /// Előző oldalra lapoz, ha jelenlegi oldal nem a 0. oldalnál áll.
        /// </summary>
        /// <returns>Sikerült-e vissza lapozni.</returns>
        public bool ElozoOldal()
        {
            if (JelenlegiOldal > 0)
            {
                JelenlegiOldal--;
                return true;
            }
            else return false;
        }
       
        /// <summary>
        /// Következő oldalra lapoz, ha jelenlegi oldal nem az utolsó oldalnál áll.
        /// </summary>
        /// <returns>Sikerült-e tovább lapozni.</returns>
        public bool KovetkezoOldal()
        {
            if (JelenlegiOldal < OldalakDarab())
            {
                JelenlegiOldal++;
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Bejárja a jelenlegi oldalt.
        /// </summary>
        public IEnumerable<T> Oldal()
        {
            int kezdet = OldalKezdete();
            int veg = OldalVege();
            for (int i = kezdet; i < veg; i++)
            {
                yield return this[i];
            }
        }
        /// <summary>
        /// Megadja a táblázat oldalainak darabszámát.
        /// </summary>
        public int OldalakDarab()
        {
            int osztas = Math.DivRem(Darab, MaxElemEgyOldalon, out int maradek);
            return maradek == 0 ? osztas - 1 : osztas;
        }
        /// <summary>
        /// Megadja, hogy a teljes kollekciót nézve melyik indextől kezdődik a jelenlegi oldal.
        /// </summary>
        /// <returns></returns>
        public int OldalKezdete() => MaxElemEgyOldalon * JelenlegiOldal;
        /// <summary>
        /// Megadja, hogy a teljes kollekciót nézve melyik indexnél fejeződik be a jelenlegi oldal.
        /// </summary>
        /// <returns></returns>
        public int OldalVege() => Math.Clamp(OldalKezdete() + MaxElemEgyOldalon, 0, Darab);
       
        /// <summary>
        /// Táblázat szélessége.
        /// </summary>
        public int Szelesseg() => Oszlopok.Darab > 0 ? Oszlopok.Osszeg(o => o.Szelesseg) + Oszlopok.Darab - 1 : Nev.Length + 2;
    }
}