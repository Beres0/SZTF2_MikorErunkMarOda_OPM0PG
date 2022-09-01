using System;
using System.Collections;
using System.Collections.Generic;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok
{
    /// <summary>
    /// Kulcs-érték párokat tároló gyors elérésű adatszerkezet, ami láncoltlisták tömbjét használ elem tárolásra és túlcsordulási területként.
    /// <para>Tulajdonságok:</para>
    /// <para> - Keresés/Tartalmazás: Konstans lehet kulcs alapján az elérés,
    /// de ha sok külcsütközés van, sőt egyenesen mindegyik egy indexre képződik le, 
    /// akkor legrosszabb esetben egy láncolt listában kell lineárisan keresni. O (1) / O (n)</para>
    /// <para> - Elérés sorrend/index szerint:  Nem értelmezhető, a kulcsok nem követhető sorrendben képződnek le. </para>
    /// <para> - Hozzáadás: Konstans, kedvezőtlen kulcs leképződés esetén lineáris. O (1) /O (n) </para>
    /// <para> - Törlés: Konstans, legrosszabb esetben linearis O (1) /O (n)</para>  
    /// <para> -Tisztítás: Végigjárja a méretével megegyezű elemű láncolt listákból álló tömbjét és ha tartalmaz elemet, meghívja a tisztít metódusát. O(méret)</para>
    /// </summary>
    internal class HashTabla<K, T> : IKollekcio<Par<K, T>>, IMasolhato<HashTabla<K,T>>,IOlvashatokent<Par<K,T>,OlvashatoHashTabla<K,T>>
    {
        /// <summary>
        /// Listák tömb alapértelmezett mérete.
        /// </summary>
        private const int AlapertelmezettMeret = 100;
        /// <summary>
        /// Ebben tárolja az elemeit a kollekció.
        /// </summary>
        private LancoltLista<Par<K, T>>[] listak;
        public int Darab { get; private set; }
        /// <summary>
        /// Inicializáláskor megadott méret.
        /// </summary>
        public int Meret { get; }
        /// <summary>
        /// <para> get: Ha megadott kulcs nem található KulcsKivetel-t dob.</para>
        /// <para> set: Hozzáadja az új elemet vagy kicseréli, ha van már ilyen kulcs a kollekcióban.</para>
        /// </summary>
        /// <param name="kulcs"></param>
        /// <returns>Kulcshoz tartozó érték</returns>
        public T this[K kulcs]
        {
            get
            {
                if (Keres(kulcs, out T ertek))
                {
                    return ertek;
                }
                else throw KulcsKivetel<K>.KulcsNemTalalhato(kulcs);
            }
            set
            {
                LancoltLista<Par<K, T>> lista = KeresHashAlapjan(kulcs);
                Par<K, T> par = new Par<K, T>(kulcs, value);
                if (!lista.Csere((p) => p.Kulcs.Equals(kulcs), par))
                {
                    lista.Hozzaad(par);
                }
            }
        }
        /// <summary>
        /// Elkészít egy hashtáblát az alapértelmezett mérettel.
        /// </summary>
        public HashTabla() : this(AlapertelmezettMeret)
        { }
        /// <summary>
        /// Elkészít egy hashtáblát tetszőleges mérettel.
        /// </summary>
        /// <param name="meret">Hashtábla mérete.</param>
        public HashTabla(int meret)
        {
            Meret = meret;
            listak = new LancoltLista<Par<K, T>>[meret];

            for (int i = 0; i < meret; i++)
            {
                listak[i] = new LancoltLista<Par<K, T>>();
            }
        }
        /// <summary>
        /// Leképzett helyen megnézi a listát és megkeresi benne a kulcsot, majd visszatér az értékkel, ha sikeres volt a keresés.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <param name="lista">Leképzett helyen lévő lista.</param>
        /// <param name="ertek">Ha a keresés nem volt sikeres, akkor defaulttal tér vissza.</param>
        /// <returns>Keresés sikeressége</returns>
        private bool Keres(K kulcs, out LancoltLista<Par<K, T>> lista, out T ertek)
        {
            ertek = default;
            lista = KeresHashAlapjan(kulcs);
            if (lista.Keres((p) => p.Kulcs.Equals(kulcs), out Par<K, T> p))
            {
                ertek = p.Ertek;
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Kulcs abszolút hashértékét osztja maradékosan a létrehozásakor megadott méretével, így rendeli a kulcsot bizonyos indexhez. 
        /// </summary>
        /// <param name="kulcs">Leképzendő kulcs.</param>
        /// <returns>Leképzett index.</returns>
        private int KulcsLekepzes(K kulcs)
        {
            return Math.Abs(kulcs.GetHashCode() % Meret);
        }
        /// <summary>
        /// Leképezi a kulcsot indexre és visszatér az indexen lévő láncoltlista referenciájával.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <returns>Leképzett kulcs helyén lévő láncoltlista</returns>
        private LancoltLista<Par<K, T>> KeresHashAlapjan(K kulcs)
        {
            return listak[KulcsLekepzes(kulcs)];
        }

        public IEnumerator<Par<K, T>> GetEnumerator()
        {
            foreach (var lista in listak)
            {
                if (lista.Darab > 0)
                {
                    foreach (var par in lista)
                    {
                        yield return par;
                    }
                }
            }
        }

        /// <summary>
        /// Hozzáad egy kulcs-érték párt a kollekcióhoz. Ha már található ilyen a kulcs a kollekcióban KulcsKivetel-t dob.
        /// </summary>
        public void Hozzaad(K kulcs, T ertek)
        {
            Hozzaad(new Par<K, T>(kulcs, ertek));
        }
        /// <summary>
        /// Hozzáad egy kulcs-érték párt a kollekcióhoz. Ha már található ilyen a kulcs a kollekcióban KulcsKivetel-t dob.
        /// </summary>
        public void Hozzaad(Par<K, T> par)
        {
            if (!HozzaadniProbal(par))
            {
                throw KulcsKivetel<K>.VanMarIlyenKulcs(par.Kulcs);
            }
        }
        /// <summary>
        /// Megpróbál hozzáadni egy kulcs-érték párt a kollekcióhoz.
        /// </summary>
        /// <returns> Történt-e hozzáadás</returns>
        public bool HozzaadniProbal(K kulcs, T ertek)
        {
            return HozzaadniProbal(new Par<K, T>(kulcs, ertek));
        }
        /// <summary>
        /// Megpróbál hozzáadni egy kulcs-érték párt a kollekcióhoz.
        /// </summary>
        /// <returns>Történt-e hozzáadás</returns>
        public bool HozzaadniProbal(Par<K, T> par)
        {
            if (!Keres(par.Kulcs, out LancoltLista<Par<K, T>> lista, out T ertek))
            {
                lista.Hozzaad(par);
                Darab++;
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Megkeresi kulcs alapján az elemet.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <param name="ertek">Ha nem járt sikerrel a keresés default értékkel tér vissza.</param>
        /// <returns>Keresés sikeressége</returns>
        public bool Keres(K kulcs, out T ertek)
        {
            return Keres(kulcs, out var lista, out ertek);
        }

        public Par<K, T>[] MasolTombbe()
        {
            Par<K, T>[] tomb = new Par<K, T>[Darab];
            int i = 0;
            foreach (var par in this)
            {
                tomb[i++] = par;
            }
            return tomb;
        }

        public HashTabla<K, T> Masol()
        {
            HashTabla<K, T> tabla = new HashTabla<K, T>(Meret);
            foreach (var lista in listak)
            {
                foreach (var par in lista)
                {
                    tabla.Hozzaad(par);
                }
            }
            return tabla;
        }

        public OlvashatoHashTabla<K, T> Olvashatokent()
        {
            return new OlvashatoHashTabla<K, T>(this);
        }
        public bool Tartalmaz(Par<K, T> par)
        {
            return KeresHashAlapjan(par.Kulcs).Tartalmaz(par);
        }
        /// <summary>
        /// Eldönti, hogy van-e ilyen kulcs a kollekcióban. 
        /// </summary>
        /// <param name="kulcs"></param>
        public bool TartalmazKulcs(K kulcs)
        {
            return Keres(kulcs, out T ertek);
        }
     
        public void Tisztit()
        {
            foreach (var lista in listak)
            {
                if (Darab > 0)
                {
                    lista.Tisztit();
                }
            }
        }
        /// <summary>
        /// Törli kulcs alapján a kulcs-érték párt, ha megtalálható a kollekcióban.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <returns>Történt-e törlés.</returns>
        public bool Torol(K kulcs)
        {
            return KeresHashAlapjan(kulcs).Torol((p) => kulcs.Equals(p.Kulcs));
        }
        /// <summary>
        /// Törli a kulcs-érték párt, ha megtalálható a kollekcióban
        /// </summary>
        /// <returns>Történt-e törlés.</returns>
        public bool Torol(Par<K, T> par)
        {
            return KeresHashAlapjan(par.Kulcs).Torol(par);
        }

        public bool Torol(Predikatum<Par<K, T>> predikatum)
        {
            foreach (var par in this)
            {
                if (predikatum(par))
                {
                    return Torol(par);
                }
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /// <summary>
    /// Kulcs alapú kollekciók dobják bizonyos esetekben.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public class KulcsKivetel<K> : Exception
    {
        /// <summary>
        /// Kivételt kiváltó kulcs.
        /// </summary>
        public K Kulcs { get; }
        /// <summary>
        /// Elkészíti a kivételt.
        /// </summary>
        /// <param name="kulcs">Kivételt kiváltó kulcs</param>
        /// <param name="message">Kivétel üzenete</param>
        public KulcsKivetel(K kulcs, string message) : base(message)
        {
            Kulcs = kulcs;
        }
        /// <summary>
        /// Ha a kulcs nem található a kollekcióban.
        /// </summary>
        /// <returns>Kivétel</returns>
        public static KulcsKivetel<K> KulcsNemTalalhato(K kulcs)
        {
            return new KulcsKivetel<K>(kulcs, "Kulcs nem található!");
        }
        /// <summary>
        /// Ha a kulcs nem egyedi.
        /// </summary>
        /// <returns>Kivétel</returns>
        public static KulcsKivetel<K> VanMarIlyenKulcs(K kulcs)
        {
            return new KulcsKivetel<K>(kulcs, "Van már ilyen kulcs!");
        }
    }
    /// <summary>
    /// Hashtábla olvasható párja.
    /// </summary>
    internal class OlvashatoHashTabla<K, T> : IOlvashatoKollekcio<Par<K, T>>, IMasolhato<HashTabla<K,T>>
    {
        /// <summary>
        /// Hashtábla, aminek olvasható felületéhez hozzáférést biztosít.
        /// </summary>
        protected HashTabla<K, T> tabla;
        public int Darab => tabla.Darab;
        /// <summary>
        /// Inicializáláskor megadott méret.
        /// </summary>
        public int Meret => tabla.Meret;
        /// <summary>
        /// <para> get: Ha megadott kulcs nem található KulcsKivetel-t dob.</para>
        /// <para> set: Hozzáadja az új elemet vagy kicseréli, ha van már ilyen kulcs a kollekcióban.</para>
        /// </summary>
        /// <param name="kulcs"></param>
        /// <returns>Kulcs alapján megtalált érték</returns>
        public T this[K kulcs] => tabla[kulcs];

        public OlvashatoHashTabla(HashTabla<K, T> tabla)
        {
            this.tabla = tabla;
        }

        public IEnumerator<Par<K, T>> GetEnumerator()
        {
            return tabla.GetEnumerator();
        }

        public bool Keres(K kulcs, out T ertek)
        {
            return tabla.Keres(kulcs, out ertek);
        }

        public Par<K, T>[] MasolTombbe()
        {
            return tabla.MasolTombbe();
        }

        public HashTabla<K, T> Masol()
        {
            return tabla.Masol();
        }

        public bool Tartalmaz(Par<K, T> par)
        {
            return tabla.Tartalmaz(par);
        }
        /// <summary>
        /// Eldönti, hogy van-e ilyen kulcs a kollekcióban. 
        /// </summary>
        /// <param name="kulcs"></param>
        public bool TartalmazKulcs(K kulcs)
        {
            return tabla.TartalmazKulcs(kulcs);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tabla.GetEnumerator();
        }
    }
}