using System.Reflection;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok
{
    using System.Collections;
    using System.Collections.Generic;
    /// <summary>
    /// Megadott kulcskiválasztó függvény alapján csoportosítja az elemeket/hozz létre ekvivalencia osztályokat láncoltlisták listájában.
    /// Olyan tulajdonságot érdemes kiválasztani, ami futás során nem változik, különben ellentmondásos állapotba kerülhet a gyüjtemény.
    /// <para>Tulajdonságok sima láncoltlistához viszonyítva:</para>
    /// <para> - Keresés/Tartalmazás: Keresés átlagban gyorsabb. O (n) </para>
    /// <para> - Elérés sorrend/index szerint: Csoportok hozzáadási sorrendje befolyásolja, de ettől függetlenül ugyanolyan. O (n)</para>
    /// <para> - Hozzáadás: Rosszabb, ha minden elem külön csoportot képez, akár lineárissá is fajulhat a láncoltlista konstans hozzáadásához képest.  O (n) </para>
    /// <para> - Törlés: Keresés átlagosan gyorsabb-> törlés átlagosan gyorsabb. O(n) </para>  
    /// <para> -Tisztítás: Null-ra állítja a referenciákat. O(1)</para>
    /// </summary>
    internal class CsoportositoLista<K, T> : IKollekcio<T>,IMasolhato<CsoportositoLista<K,T>>,IOlvashatokent<T,OlvashatoCsoportositoLista<K,T>>
    {
        /// <summary>
        /// Belső Csoport struktúra
        /// </summary>
        private struct Csoport
        {
            /// <summary>
            /// Csoport írható listája. Csoportosító lista nem ad hozzáférést hozzá a külvilágnak,
            /// hogy ne sérüljön a kollekció csoportosító tulajdonsága.
            /// </summary>
            public LancoltLista<T> Irhato { get; }
            /// <summary>
            /// Csoport kulcsa
            /// </summary>
            public K Kulcs { get; }
            /// <summary>
            /// Csoport olvasható listája. Ezen keresztül fogják tudni a kliensek olvasni a csoportok elemeit.
            /// </summary>
            public OlvashatoLancoltLista<T> Olvashato { get; }
            /// <summary>
            /// Elkészít egy csoportot
            /// </summary>
            /// <param name="kulcs"></param>
            public Csoport(K kulcs)
            {
                Kulcs = kulcs;
                Irhato = new LancoltLista<T>();
                Olvashato =Irhato.Olvashatokent();
            }
        }
        /// <summary>
        /// Csoportok láncoltlistája.
        /// </summary>
        private LancoltLista<Csoport> csoportok;
        /// <summary>
        /// Csoportosítás módja.
        /// </summary>
        private Fuggveny<T, K> csoportositoKulcs;
        /// <summary>
        /// Kollekcióban található csoportok darabszáma.
        /// </summary>
        public int CsoportokDarab => csoportok.Darab;

        public int Darab => csoportok.Osszeg((cs) => cs.Olvashato.Darab);

        /// <summary>
        /// Inicializálja a csoportokat.
        /// </summary>
        private CsoportositoLista()
        {
            csoportok = new LancoltLista<Csoport>();
        }
        /// <summary>
        /// Elkészít egy csoportosító listát és feltölti elemekkel.
        /// </summary>
        /// <param name="csoportositoKulcs">Csoportosítás módja.</param>
        /// <param name="elemek">Hozzá kívánt adni elemek kollekciója.</param>
        public CsoportositoLista(Fuggveny<T, K> csoportositoKulcs, IOlvashatoKollekcio<T> elemek) : this(csoportositoKulcs)
        {
            foreach (var elem in elemek)
            {
                Hozzaad(elem);
            }
        }
        /// <summary>
        /// Elkészít egy üres csoportosító listát.
        /// </summary>
        /// <param name="csoportositoKulcs"></param>
        public CsoportositoLista(Fuggveny<T, K> csoportositoKulcs) : this()
        {
            this.csoportositoKulcs = csoportositoKulcs;
        }

        /// <summary>
        /// Megkeresi kulcs alapján a csoportot.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <param name="csoport">Keresett csoport. Ha nem talált ilyen kulcsot, értéke default.</param>
        /// <returns>Keresés sikeressége</returns>
        private bool KeresCsoport(K kulcs, out Csoport csoport)
        {
            return csoportok.Keres((cs) => cs.Kulcs.Equals(kulcs), out csoport);
        }
        /// <summary>
        /// Bejárja a csoportok kollekcióját.
        /// </summary>
        public IEnumerable<OlvashatoLancoltLista<T>> Csoportok()
        {
            foreach (var csoport in csoportok)
            {
                yield return csoport.Olvashato;
            }
        }
        /// <summary>
        /// Bejárja a csoportok kulcsait.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<K> CsoportositoKulcsok()
        {
            foreach (var csoport in csoportok)
            {
                yield return csoport.Kulcs;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var csoport in csoportok)
            {
                foreach (var elem in csoport.Olvashato)
                {
                    yield return elem;
                }
            }
        }
        /// <summary>
        /// Hozzáadja az elemet a megfelelő csoporthoz. Ha még nincs ilyen csoport, létrehoz neki egyet.
        /// </summary>
        /// <param name="ertek"></param>
        public void Hozzaad(T ertek)
        {
            K kulcs = csoportositoKulcs(ertek);

            if (!KeresCsoport(kulcs, out Csoport csoport))
            {
                csoport = new Csoport(kulcs);
                csoportok.Hozzaad(csoport);
            }

            csoport.Irhato.Hozzaad(ertek);
        }
    
        /// <summary>
        /// Megkeresi az érték csoportját.
        /// </summary>
        /// <param name="ertek"></param>
        /// <param name="csoport">Keresett csoport.Ha nem található meg az érték egyik csoportban sem, akkor az értéke default.</param>
        /// <returns>Keresés sikeressége</returns>
        public bool KeresCsoport(T ertek, out OlvashatoLancoltLista<T> csoport)
        {
            return KeresCsoport(csoportositoKulcs(ertek), out csoport);
        }
        /// <summary>
        /// Megkeresi a csoportot kulcs alapján.
        /// </summary>
        /// <param name="csoport">Keresett csoporot. Ha nincs ilyen kulcsú csoport, akkor az értéke default. </param>
        /// <returns></returns>
        public bool KeresCsoport(K kulcs, out OlvashatoLancoltLista<T> csoport)
        {
            csoport = default;
            if (KeresCsoport(kulcs, out Csoport cs))
            {
                csoport = cs.Olvashato;
                return true;
            }
            else return false;
        }
        public T[] MasolTombbe()
        {
            int i = 0;
            T[] tomb = new T[Darab];
            foreach (var csoport in csoportok)
            {
                foreach (var elem in csoport.Olvashato)
                {
                    tomb[i++] = elem;
                }
            }
            return tomb;
        }

        public CsoportositoLista<K, T> Masol()
        {
            CsoportositoLista<K, T> csoportosito = new CsoportositoLista<K, T>(csoportositoKulcs);
            csoportosito.csoportok = csoportok.Masol();
            return csoportosito;
        }

        public OlvashatoCsoportositoLista<K, T> Olvashatokent()
        {
            return new OlvashatoCsoportositoLista<K, T>(this);
        }

        public bool Tartalmaz(T ertek)
        {
            return KeresCsoport(csoportositoKulcs(ertek), out Csoport csoport) && csoport.Olvashato.Tartalmaz(ertek);
        }

        public void Tisztit()
        {
            csoportok.Tisztit();
        }

        public bool Torol(T ertek)
        {
            return KeresCsoport(csoportositoKulcs(ertek), out Csoport csoport) && csoport.Irhato.Torol(ertek);
        }

        public bool Torol(Predikatum<T> predikatum)
        {
            foreach (var csoport in csoportok)
            {
                if (csoport.Irhato.Torol(predikatum))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Kitörli az csoportot, amibe az érték beleillik.
        /// </summary>
        /// <param name="ertek"></param>
        /// <returns>Történt-e törlés.</returns>
        public bool TorolCsoport(T ertek)
        {
            return TorolCsoport(csoportositoKulcs(ertek));
        }
        /// <summary>
        /// Kitörli a csoportot a kulcsa alapján.
        /// </summary>
        /// <param name="ertek"></param>
        /// <returns>Történt-e törlés.</returns>
        public bool TorolCsoport(K kulcs)
        {
            return csoportok.Torol((cs) => cs.Kulcs.Equals(kulcs));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /// <summary>
    /// Csoportosító lista olvasható párja.
    /// </summary>
    internal class OlvashatoCsoportositoLista<K, T> : IOlvashatoKollekcio<T>
    {
        /// <summary>
        /// Csoportosító lista, aminek olvasható felületéhez hozzáférést biztosít.
        /// </summary>
        private CsoportositoLista<K, T> csoportosito;
        /// <summary>
        /// Kollekcióban található csoportok darabszáma.
        /// </summary>
        public int CsoportokDarab => csoportosito.CsoportokDarab;

        public int Darab => csoportosito.Darab;
        /// <summary>
        /// Elkészíti a csoportosító lista olvasható párját.
        /// </summary>
        /// <param name="csoportosito">Csoportosító lista, amiből elkészül</param>
        public OlvashatoCsoportositoLista(CsoportositoLista<K, T> csoportosito)
        {
            this.csoportosito = csoportosito;
        }
        /// <summary>
        /// Bejárja a csoportok kollekcióját.
        /// </summary>
        public IEnumerable<OlvashatoLancoltLista<T>> Csoportok()
        {
            return csoportosito.Csoportok();
        }
        /// <summary>
        /// Bejárja a csoportok kulcsait.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<K> CsoportositoKulcsok()
        {
            return csoportosito.CsoportositoKulcsok();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return csoportosito.GetEnumerator();
        }

        public bool Keres(Predikatum<T> predikatum, out T ertek)
        {
            return csoportosito.Keres(predikatum, out ertek);
        }
           /// <summary>
        /// Bejárja a csoportok kulcsait.
        /// </summary>
        /// <returns></returns>
        public bool KeresCsoport(T ertek, out OlvashatoLancoltLista<T> csoport)
        {
            return csoportosito.KeresCsoport(ertek, out csoport);
        }
        /// <summary>
        /// Megkeresi kulcs alapján a csoportot.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <param name="csoport">Keresett csoport. Ha nem talált ilyen kulcsot, értéke default.</param>
        /// <returns>Keresés sikeressége</returns>
        public bool KeresCsoport(K kulcs, out OlvashatoLancoltLista<T> csoport)
        {
            return csoportosito.KeresCsoport(kulcs, out csoport);
        }

        public T[] MasolTombbe()
        {
            return csoportosito.MasolTombbe();
        }

        public CsoportositoLista<K, T> Masol()
        {
            return csoportosito.Masol();
        }

        public bool Tartalmaz(T ertek)
        {
            return csoportosito.Tartalmaz(ertek);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return csoportosito.GetEnumerator();
        }
    }
}