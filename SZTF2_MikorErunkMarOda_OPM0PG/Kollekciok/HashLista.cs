using System.Collections;
using System.Collections.Generic;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok
{
    /// <summary>
    /// Kétszeresen oda-vissza láncolt lista, annyi extrával,
    /// hogy tartalmaz egy hashtáblát és egy kulcskiválasztó delegáltat,
    /// ötvözve ezzel a láncolt lista sorrendiségét a hasítótábla konstans elérésével.
    /// Kiválasztott kulccsal konstans időben el lehet érni a láncolt listában mind az őt tartalmazó Elem-et, mind magát a példányt.
    /// Hátránya, hogy „jelentős” többlet tárhellyel jár.
    /// <para>Tulajdonságok:</para>
    /// <para> - Keresés/Tartalmazás: Konstans lehet kulcs alapján az elérés, de ha sok külcsütközés van,
    /// sőt egyenesen mindegyik egy indexre képződik le,
    /// akkor legrosszabb esetben egy láncolt listában kell lineárisan keresni. O(1)/O(n)</para>
    /// <para> - Elérés sorrend/index szerint: Nem indexelhető, ezért O(n)</para>
    /// <para> - Hozzáadás: Oda-vissza láncolt, kedvező esetben konstans O(1)/O(n)</para>
    /// <para> - Tisztítás: Végigjárja a méretével megegyezű elemű láncolt listákból álló tömbjét és ha tartalmaz elemet,
    /// meghívja a tisztít metódusát. O(méret)</para>
    /// </summary>
    internal class HashLista<T> : IKollekcio<T>, IMasolhato<HashLista<T>>, IOlvashatokent<T, OlvashatoHashLista<T>>
    {
        /// <summary>
        /// Belső oszály amivel oda-vissza lehet láncol az objektumokat
        /// </summary>
        private class Elem
        {
            /// <summary>
            /// Előző elem referenciája.
            /// </summary>
            public Elem Elozo { get; set; }

            /// <summary>
            /// Az elemben tárolt érték.
            /// </summary>
            public T Ertek { get; set; }

            /// <summary>
            /// Következő elem referenciája.
            /// </summary>
            public Elem Kovetkezo { get; set; }

            /// <summary>
            /// Elkészít egy elemet
            /// </summary>
            /// <param name="ertek">Tárolandó érték.</param>
            public Elem(T ertek)
            {
                Ertek = ertek;
            }
        }

        /// <summary>
        /// Hashlista utolsó eleme.
        /// </summary>
        private Elem farok;

        /// <summary>
        /// Hashlista első eleme
        /// </summary>
        private Elem fej;

        /// <summary>
        /// Segéd hashtábla, amiben tárolja az elemeket.
        /// </summary>
        private HashTabla<object, Elem> hash;

        /// <summary>
        /// Kulcskiválasztó delegált.
        /// </summary>
        private Fuggveny<T, object> kulcsKivalaszto;

        public int Darab => hash.Darab;

        /// <summary>
        /// Hashlista első eleme
        /// </summary>
        public T Elso => fej.Ertek;

        /// <summary>
        /// Inicializáláskor megadott méret.
        /// </summary>
        public int Meret => hash.Meret;

        /// <summary>
        /// Hashlista utolsó eleme.
        /// </summary>
        public T Utolso => farok.Ertek;

        /// <summary>
        /// Ha megadott kulcs nem található KulcsKivetel-t dob.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <returns>Kulcshoz tartozó érték</returns>
        public T this[object kulcs] => hash[kulcs].Ertek;

        /// <summary>
        /// Elkészit egy hashlistát alapértelmezett mérettel.
        /// </summary>
        /// <param name="kulcs">Függvény, amellyel kiválasztjuk az elemből az azonosítására szolgáló kulcsot </param>
        public HashLista(Fuggveny<T, object> kulcs)
        {
            kulcsKivalaszto = kulcs;
            hash = new HashTabla<object, Elem>();
        }

        /// <summary>
        /// Elkészít egy hashlistát.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <param name="meret">Hashlista mérete</param>
        public HashLista(Fuggveny<T, object> kulcs, int meret)
        {
            kulcsKivalaszto = kulcs;
            hash = new HashTabla<object, Elem>(meret);
        }

        /// <summary>
        /// Hozzáad egy elemet a lista végére. Ha a kiválasztott kulcs nem egyedi, akkor KulcsKivetel-t dob.
        /// </summary>
        private void Hozzaad(Elem elem)
        {
            hash.Hozzaad(kulcsKivalaszto(elem.Ertek), elem);
            HozzaadElem(elem);
        }

        /// <summary>
        /// Hozzáad egy elemet a lista elejére. Ha a kiválasztott kulcs nem egyedi, akkor KulcsKivetel-t dob.
        /// </summary>
        /// <param name="ertek">Hozzáadandó érték</param>
        private void HozzaadElejere(Elem elem)
        {
            hash.Hozzaad(kulcsKivalaszto(elem.Ertek), elem);
            if (fej == null)
            {
                farok = elem;
            }

            elem.Kovetkezo = fej;
            fej = elem;
        }

        /// <summary>
        /// Hozzáad egy elemet a csak a láncoltlistához. Beállítja faroknak és ha ő az első, akkor fejnek is.
        /// </summary>
        /// <param name="elem">Hozzaadando elem.</param>
        private void HozzaadElem(Elem elem)
        {
            if (farok == null)
            {
                fej = elem;
            }
            else
            {
                farok.Kovetkezo = elem;
                elem.Elozo = farok;
            }
            farok = elem;
        }

        /// <summary>
        /// Megpróbál hozzáadni egy elemet a lista végére.
        /// </summary>
        /// <param name="ertek">Hozzáadandó elem</param>
        /// <returns>Történt-e hozzáadás.</returns>
        private bool HozzaadniProbal(Elem elem)
        {
            if (hash.HozzaadniProbal(kulcsKivalaszto(elem.Ertek), elem))
            {
                HozzaadElem(elem);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Hashtáblában megkeresi az értékből kiválasztott kulcs alapján az elemet.
        /// </summary>
        /// <param name="ertek">Keresendő érték</param>
        /// <param name="elem">Ha nincs ilyen elem, akkor értéke default.</param>
        /// <returns>Keresés sikeressége</returns>
        private bool Keres(T ertek, out Elem elem)
        {
            return hash.Keres(kulcsKivalaszto(ertek), out elem);
        }

        /// <summary>
        /// Kicseréli, ha van ugyanolyan kulcsú elem a kollekcióban.
        /// </summary>
        /// <param name="cserelendo">Régi elem</param>
        /// <param name="uj">Új elem</param>
        /// <returns>Történt-e csere.</returns>
        public bool Csere(T cserelendo, T uj)
        {
            object kulcs = kulcsKivalaszto(cserelendo);
            if (hash.Keres(kulcs, out Elem elem))
            {
                hash.Torol(kulcs);
                elem.Ertek = uj;
                hash.Hozzaad(kulcsKivalaszto(uj), elem);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Megpróbálja elérni a kollekcióban az előtte álló értéket.
        /// </summary>
        /// <param name="ertek">Vizsgált érték</param>
        /// <param name="elozo">Előző érték.
        /// Ha a vizsgált érték előtt nem áll elem vagy nem található meg a kollekcióban, akkor defaultként tér vissza </param>
        /// <returns>Elérés sikeressége</returns>
        public bool Elozo(T ertek, out T elozo)
        {
            elozo = default;
            if (Keres(ertek, out Elem elem) && elem.Elozo != null)
            {
                elozo = elem.Elozo.Ertek;
                return true;
            }
            return false;
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
        /// Hozzáad egy értéket a lista végére. Ha a kiválasztott kulcs nem egyedi, akkor KulcsKivetel-t dob.
        /// </summary>
        /// <param name="ertek"></param>
        public void Hozzaad(T ertek)
        {
            Hozzaad(new Elem(ertek));
        }

        /// <summary>
        /// Hozzáad egy értéket a lista elejére. Ha a kiválasztott kulcs nem egyedi, akkor KulcsKivetel-t dob.
        /// </summary>
        /// <param name="ertek">Hozzáadandó érték</param>
        public void HozzaadElejere(T ertek)
        {
            HozzaadElejere(new Elem(ertek));
        }

        /// <summary>
        /// Megpróbál hozzáadni egy értéket a lista végére.
        /// </summary>
        /// <returns> Történt-e hozzáadás</returns>
        public bool HozzaadniProbal(T ertek)
        {
            return HozzaadniProbal(new Elem(ertek));
        }

        /// <summary>
        /// Megpróbál hozzáadni egy elemet a lista elejére.
        /// </summary>
        /// <param name="ertek">Hozzáadandó elem</param>
        /// <returns>Történt-e hozzáadás.</returns>
        public bool HozzaadniProbalElejere(T ertek)
        {
            Elem elem = new Elem(ertek);
            if (hash.HozzaadniProbal(kulcsKivalaszto(ertek), elem))
            {
                HozzaadElejere(elem);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Kulcs alapján megkeresi az értéket.
        /// </summary>
        /// <returns>Keresés sikeressége</returns>
        public bool Keres(object kulcs, out T ertek)
        {
            ertek = default;
            if (hash.Keres(kulcs, out Elem elem))
            {
                ertek = elem.Ertek;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Megpróbálja elérni a kollekcióban az utána következő értéket.
        /// </summary>
        /// <param name="ertek">Vizsgált érték</param>
        /// <param name="kovetkezo">Következő érték.
        /// Ha a vizsgált érték után nem áll elem vagy nem található meg a kollekcióban, akkor defaultként tér vissza </param>
        /// <returns>Elérés sikeressége</returns>
        public bool Kovetkezo(T ertek, out T kovetkezo)
        {
            kovetkezo = default;
            if (Keres(ertek, out Elem elem) && elem.Kovetkezo != null)
            {
                kovetkezo = elem.Kovetkezo.Ertek;
                return true;
            }
            return false;
        }

        public HashLista<T> Masol()
        {
            HashLista<T> hashLista = new HashLista<T>(kulcsKivalaszto, hash.Meret);
            foreach (var ertek in this)
            {
                hashLista.Hozzaad(ertek);
            }
            return hashLista;
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

        public OlvashatoHashLista<T> Olvashatokent()
        {
            return new OlvashatoHashLista<T>(this);
        }

        public bool Tartalmaz(T ertek)
        {
            return hash.TartalmazKulcs(kulcsKivalaszto(ertek));
        }

        /// <summary>
        /// Eldönti, hogy ilyen kulccsal rendelkező elemet tartalmaz-e a kollekció.
        /// </summary>
        public bool TartalmazKulcs(object kulcs)
        {
            return hash.TartalmazKulcs(kulcs);
        }

        public void Tisztit()
        {
            fej = null;
            farok = null;
            hash.Tisztit();
        }

        public bool Torol(T ertek)
        {
            if (Keres(ertek, out Elem elem))
            {
                if (Darab == 1)
                {
                    fej = null;
                    farok = null;
                }
                else
                {
                    if (elem != fej)
                    {
                        elem.Elozo.Kovetkezo = elem.Kovetkezo;
                    }
                    else
                    {
                        fej = elem.Kovetkezo;
                    }

                    if (elem != farok)
                    {
                        elem.Kovetkezo.Elozo = elem.Elozo;
                    }
                    else
                    {
                        farok = elem.Elozo;
                    }
                }

                hash.Torol(kulcsKivalaszto(ertek));
                return true;
            }
            else return false;
        }

        public bool Torol(Predikatum<T> predikatum)
        {
            foreach (var ertek in this)
            {
                if (predikatum(ertek))
                {
                    return Torol(ertek);
                }
            }

            return false;
        }

        /// <summary>
        /// Bejárja visszafele a kollekciót.
        /// </summary>
        public IEnumerable<T> Visszafele()
        {
            Elem elem = farok;
            while (elem != null)
            {
                yield return elem.Ertek;
                elem = elem.Elozo;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Hashlista olvasható párja.
    /// </summary>
    internal class OlvashatoHashLista<T> : IOlvashatoKollekcio<T>, IMasolhato<HashLista<T>>
    {
        /// <summary>
        /// Hashlista, aminek olvasható felületéhez hozzáférést biztosít.
        /// </summary>
        private HashLista<T> hashLista;

        public int Darab => hashLista.Darab;

        /// <summary>
        /// Inicializáláskor megadott méret.
        /// </summary>
        public int Meret => hashLista.Meret;

        /// <summary>
        /// Ha megadott kulcs nem található KulcsKivetel-t dob.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <returns>Kulcshoz tartozó érték</returns>public T this[object kulcs]=>hashLista[kulcs];
        public OlvashatoHashLista(HashLista<T> hashLista)
        {
            this.hashLista = hashLista;
        }

        /// <summary>
        /// Megpróbálja elérni a kollekcióban az előtte álló értéket.
        /// </summary>
        /// <param name="ertek">Vizsgált érték</param>
        /// <param name="elozo">Előző érték.
        /// Ha a vizsgált érték előtt nem áll elem vagy nem található meg a kollekcióban, akkor defaultként tér vissza </param>
        /// <returns>Elérés sikeressége</returns>

        public bool Elozo(T ertek, out T elozo)
        {
            return hashLista.Elozo(ertek, out elozo);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return hashLista.GetEnumerator();
        }

        /// <summary>
        /// Kulcs alapján megkeresi az értéket.
        /// </summary>
        /// <returns>Keresés sikeressége</returns>

        public bool Keres(object kulcs, out T ertek)
        {
            return hashLista.Keres(kulcs, out ertek);
        }

        /// <summary>
        /// Megpróbálja elérni a kollekcióban az utána következő értéket.
        /// </summary>
        /// <param name="ertek">Vizsgált érték</param>
        /// <param name="kovetkezo">Következő érték.
        /// Ha a vizsgált érték után nem áll elem vagy nem található meg a kollekcióban, akkor defaultként tér vissza </param>
        /// <returns>Elérés sikeressége</returns>
        public bool Kovetkezo(T ertek, out T kovetkezo)
        {
            return hashLista.Kovetkezo(ertek, out kovetkezo);
        }

        public HashLista<T> Masol()
        {
            return hashLista.Masol();
        }

        public T[] MasolTombbe()
        {
            return hashLista.MasolTombbe();
        }

        public bool Tartalmaz(T ertek)
        {
            return hashLista.Tartalmaz(ertek);
        }

        /// <summary>
        /// Eldönti, hogy ilyen kulccsal rendelkező elemet tartalmaz-e a kollekció.
        /// </summary>
        public bool TartalmazKulcs(object kulcs)
        {
            return hashLista.TartalmazKulcs(kulcs);
        }

        /// <summary>
        /// Bejárja visszafele a kollekciót.
        /// </summary>
        public IEnumerable<T> Visszafele()
        {
            return hashLista.Visszafele();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hashLista.GetEnumerator();
        }
    }
}