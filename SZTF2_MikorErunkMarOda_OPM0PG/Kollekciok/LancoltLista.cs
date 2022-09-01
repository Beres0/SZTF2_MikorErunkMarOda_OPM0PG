using System.Collections;
using System.Collections.Generic;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok
{
    /// <summary>
    /// Egyszeresen láncolt lista.
    /// <para>Tulajdonságok:</para>
    /// <para> - Keresés/Tartalmazás: Lineáris keresés. O (n)</para>
    /// <para> - Elérés sorrend/index szerint: Nem indexelhető, ezért O (n)</para>
    /// <para> - Hozzáadás: Fejéről és a farkáról is tárol referenciát, így a hozzáadás elejére-végére O (1)</para>
    /// <para> - Tisztítás: Null-ra állítja a referenciákat. O (1)</para>       
    /// </summary>
    internal class LancoltLista<T> : IKollekcio<T>, IMasolhato<LancoltLista<T>>, IOlvashatokent<T,OlvashatoLancoltLista<T>>
    {
        /// <summary>
        /// Belső osztály amivel össze lehet láncolni az objektumokat.
        /// </summary>
        private class Elem
        {
            /// <summary>
            /// Az elemben tárolt érték.
            /// </summary>
            public T Ertek { get; set; }
            /// <summary>
            /// Következő elem referenciája.
            /// </summary>
            public Elem Kovetkezo { get; set; }
            /// <summary>
            /// Elkészít egy elemet.
            /// </summary>
            /// <param name="ertek">Tárolandó érték.</param>
            public Elem(T ertek)
            {
                Ertek = ertek;
            }
        }
        /// <summary>
        /// Lista első eleme.
        /// </summary>
        private Elem farok;
        /// <summary>
        /// Lista utolsó eleme.
        /// </summary>
        private Elem fej;
        public int Darab { get; private set; }
        /// <summary>
        /// Lista első eleme.
        /// </summary>
        public T Elso => fej.Ertek;
        /// <summary>
        /// Lista utolsó eleme.
        /// </summary>
        public T Utolso => farok.Ertek;

        /// <summary>
        /// Elkészít egy üres láncoltlistát.
        /// </summary>
        public LancoltLista()
        { }
        /// <summary>
        /// Elkészít egy láncoltlistát és feltölti a paraméterben átadott elemekkel.
        /// </summary>
        /// <param name="kollekcio">Kollekció, amiből feltöltjük a listát.</param>
        public LancoltLista(IOlvashatoKollekcio<T> kollekcio) : this()
        {
            foreach (var elem in kollekcio)
            {
                Hozzaad(elem);
            }
        }

       /// <summary>
       /// Hozzáad egy elemet a lista végére.
       /// </summary>
       /// <param name="elem"></param>
        private void Hozzaad(Elem elem)
        {
            if (farok == null)
            {
                fej = elem;
            }
            else
            {
                farok.Kovetkezo = elem;
            }
            farok = elem;
            Darab++;
        }
        /// <summary>
        ///  Megkeresi az első elemet, ami tartalmazza az étéket.
        /// </summary>
        /// <param name="ertek">Keresett érték.</param>
        /// <param name="elem">Elem, ami tárolja az értéket. Ha nem talált ilyen elemet, akkor az értéke default</param>
        /// <returns>Keresés sikeressége.</returns>
        private bool Keres(T ertek, out Elem elem)
        {
            return Keres((e) => e.Equals(ertek), out elem);
        }
        /// <summary>
        ///  Megkeresi az első elemet, ami tartalmaz P tulajdonságú értéket.
        /// </summary>
        /// <param name="ertek">P tulajdonság.</param>
        /// <param name="elem">Elem, ami tárolja a P tulajdonságú értéket. Ha nem talált ilyen elemet, akkor az értéke default</param>
        /// <returns>Keresés sikeressége.</returns>
        private bool Keres(Predikatum<T> predikatum, out Elem elem)
        {
            elem = fej;
            while (elem != null && !predikatum(elem.Ertek))
            {
                elem = elem.Kovetkezo;
            }

            return elem != null;
        }

        /// <summary>
        /// Megkeresi érték alapján az első elemet és ha talált, akkor a tartalmát kicseréli egy másik értékre.
        /// </summary>
        /// <param name="cserelendo">Cserélendő érték.</param>
        /// <param name="uj">Elem új értéke</param>
        /// <returns>Csere sikeressége.</returns>
        public bool Csere(T cserelendo, T uj)
        {
            return Csere((e) => e.Equals(cserelendo), uj);
        }
        /// <summary>
        /// Kicseréli az első P tulajdonságú értéket.
        /// </summary>
        /// <param name="predikatum">P tulajdonság.</param>
        /// <param name="ertek">Új érték.</param>
        /// <returns>Történt-e csere.</returns>
        public bool Csere(Predikatum<T> predikatum, T ertek)
        {
            if (Keres(predikatum, out Elem elem))
            {
                elem.Ertek = ertek;
                return true;
            }
            else return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Elem elem = fej;
            while (elem != null)
            {
                yield return elem.Ertek;
                elem = elem.Kovetkezo;
            }
        }
        /// <summary>
        /// Hozzáad egy elemet a lista végére.
        /// </summary>
        /// <param name="ertek">Hozzáadandó érték</param>
        public void Hozzaad(T ertek)
        {
            Elem elem = new Elem(ertek);
            Hozzaad(elem);
        }
        /// <summary>
        /// Hozzáad egy elemet a lista elejére.
        /// </summary>
        /// <param name="ertek">Hozzáadandó érték</param>
        public void HozzaadElejere(T ertek)
        {
            Elem elem = new Elem(ertek);
            if(fej==null)
            {
                farok = elem;
            }

            elem.Kovetkezo = fej;
            fej = elem;
            Darab++;
        }
        public T[] MasolTombbe()
        {
            T[] tomb = new T[Darab];
            int i = 0;
            foreach (var elem in this)
            {
                tomb[i++] = elem;
            }

            return tomb;
        }
        public LancoltLista<T> Masol()
        {
            return new LancoltLista<T>(this);
        }
        public OlvashatoLancoltLista<T> Olvashatokent()
        {
            return new OlvashatoLancoltLista<T>(this);
        }

        public bool Tartalmaz(T ertek)
        {
            return Keres(ertek, out Elem elem);
        }

        public virtual void Tisztit()
        {
            fej = null;
            farok = null;
            Darab = 0;
        }

        public bool Torol(Predikatum<T> predikatum)
        {
            Elem elem = fej;
            Elem elozo = null;
            while (elem != null && !predikatum(elem.Ertek))
            {
                elozo = elem;
                elem = elem.Kovetkezo;
            }

            if (elem != null && predikatum(elem.Ertek))
            {
                if (elozo == null)
                {
                    fej = fej.Kovetkezo;
                }
                else
                {
                    elozo.Kovetkezo = elem.Kovetkezo;
                }

                if (elem == farok)
                {
                    farok = elozo;
                }

                Darab--;
                return true;
            }
            else return false;
        }

        public bool Torol(T ertek)
        {
            return Torol((e) => e.Equals(ertek));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /// <summary>
    /// LancoltLista<T> olvasható párja.
    /// </summary>
    internal class OlvashatoLancoltLista<T> : IOlvashatoKollekcio<T>,IMasolhato<LancoltLista<T>>
    {
        /// <summary>
        /// Lista, aminek olvasható felületéhez hozzáférést biztosít.
        /// </summary>
        protected LancoltLista<T> lista;
        public int Darab => lista.Darab;
        /// <summary>
        /// Lista első eleme.
        /// </summary>
        public T Elso => lista.Elso;
        /// <summary>
        /// Lista utolsó eleme.
        /// </summary>
        public T Utolso => lista.Utolso;
        /// <summary>
        /// Elkészíti a lista olvasható párját.
        /// </summary>
        /// <param name="lista">Lista, amiből elkészül.</param>
        public OlvashatoLancoltLista(LancoltLista<T> lista)
        {
            this.lista = lista;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return lista.GetEnumerator();
        }

        public T[] MasolTombbe()
        {
            return lista.MasolTombbe();
        }

        public LancoltLista<T> Masol()
        {
            return new LancoltLista<T>(this);
        }

        public bool Tartalmaz(T ertek)
        {
            return lista.Tartalmaz(ertek);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lista.GetEnumerator();
        }
    }
}