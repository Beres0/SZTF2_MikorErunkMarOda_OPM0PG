using System;
using System.Collections;
using System.Collections.Generic;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok
{
    /// <summary>
    /// Tömb alapú dinamikus gyüjtemény.
    /// <para>Tulajdonságok:</para>
    /// <para> - Keresés/Tartalmazás: Lineáris keresés. O (n)</para>
    /// <para> - Elérés sorrend/index szerint: Indexelhető O (1)</para>
    /// <para> - Hozzáadás: Hozzáadás a lista végén konstans, egyszerűen felülírja a soronkövetkező indexet,
    /// viszont, ha már elérte a kapacitása felsőhatárát, létrehoz egy kétszer akkor tömböt és átmasolja bele az elemeket.
    /// Tetszőleges helyre beszúráskor minden rákövetkező elemet arrébb másol. O (n)</para>
    /// <para> - Törlés: Ha negyed annyi vagy kevesebb elem van és nem kevesebb, mint az alapkapacitás, akkor negyedére csökkenti a tömb méretét.
    /// Sikeres lineáris keresést követően minden rákövetkező elemet előrébb másol eggyel. O(n)</para>  
    /// <para> - Tisztítás: Felülírja a használt tömböt egy üres, alapkapacitású tömbbel. O(1)</para>
    /// </summary>
    internal class Lista<T> : IIndexelhetoKollekcio<T>, IKollekcio<T>,IMasolhato<Lista<T>>,IOlvashatokent<T,OlvashatoLista<T>>
    {
        /// <summary>
        /// Lista belső tömbjének alapértelmezett mérete
        /// </summary>
        private const int AlapertelmezettKapacitas = 8;
        
        /// <summary>
        /// Tömb, amit a lista az elemek tárolására használ.
        /// </summary>
        private T[] elemek;
        public int Darab { get; private set; }
        /// <summary>
        /// Lista indexere.
        /// </summary>
        /// <param name="index"> Index értéke [0,Darab] között lehet, máskülönben IndexOutOfRangeExceptiont dob. </param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index >= Darab || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                return elemek[index];
            }
            set
            {
                if (index >= Darab || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                elemek[index] = value;
            }
        }
        /// <summary>
        /// Elkészít egy üres listát.
        /// </summary>
        public Lista()
        {
            elemek = new T[AlapertelmezettKapacitas];
        }
        /// <summary>
        /// Elkészít egy listát és feltölti elemekkel.
        /// </summary>
        /// <param name="kollekcio">Kollekció, amiből feltölti</param>
        public Lista(IOlvashatoKollekcio<T> kollekcio) : this()
        {
            foreach (var elem in kollekcio)
            {
                Hozzaad(elem);
            }
        }
        /// <summary>
        /// Kétszer akkorára növeli az elemek tömböt.
        /// </summary>
        private void Bovit()
        {
            T[] uj = new T[elemek.Length << 1];
            elemek.CopyTo(uj, 0);
            elemek = uj;
        }
        /// <summary>
        /// Negyed akkorára csökkenti az elemek tömböt.
        /// </summary>
        private void Szukit()
        {
            T[] uj = new T[elemek.Length >> 2];
            for (int i = 0; i < Darab; i++)
            {
                uj[i] = elemek[i];
            }
            elemek = uj;
        }
        /// <summary>
        /// Beszúr egy elemet tetszőleges indexre.
        /// </summary>
        /// <param name="elem">Beszúrandó elem.</param>
        /// <param name="index">Index értéke [0,Darab] között lehet, máskülönben IndexOutOfRangeExceptiont dob.</param>
        public void Beszur(T elem, int index)
        {
            if (index > Darab || index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            if (Darab >= elemek.Length)
            {
                Bovit();
            }

            for (int i = Darab - 1; i >= index; i--)
            {
                elemek[i + 1] = elemek[i];
            }

            elemek[index] = elem;
            Darab++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Darab; i++)
            {
                yield return elemek[i];
            }
        }
        /// <summary>
        /// Hozzáad egy elemet a lista végére.
        /// </summary>
        /// <param name="elem">Hozzáadandó elem.</param>
        public void Hozzaad(T elem)
        {
            if (Darab == elemek.Length)
            {
                Bovit();
            }

            elemek[Darab] = elem;

            Darab++;
        }
        /// <summary>
        /// Megkeresi az első P tulajdonságú elem indexét.
        /// </summary>
        /// <param name="predikatum">P tulajdonság</param>
        /// <param name="index">Elem indexe, ha nem volt sikeres a keresés a lista darabszámával tér vissza </param>
        /// <returns>Keresés sikeressége</returns>
        public bool Keres(Predikatum<T> predikatum, out int index)
        {
            int i = 0;
            while (i < Darab && !predikatum(elemek[i]))
            {
                i++;
            }

            index = i;
            return i < Darab;
        }
        /// <summary>
        /// Megkeresi az elem indexét.
        /// </summary>
        /// <param name="predikatum">Keresendő érték</param>
        /// <param name="index">Elem indexe, ha nem volt sikeres a keresés a lista darabszámával tér vissza </param>
        /// <returns>Keresés sikeressége</returns>
        public bool Keres(T ertek, out int index)
        {
            int i = 0;
            while (i < Darab && !ertek.Equals(elemek[i]))
            {
                i++;
            }

            index = i;
            return i < Darab;
        }

        public T[] MasolTombbe()
        {
            T[] tomb = new T[Darab];
            for (int i = 0; i < Darab; i++)
            {
                tomb[i] = elemek[i];
            }
            return tomb;
        }

        public Lista<T> Masol()
        {
            return new Lista<T>(this);
        }

        public OlvashatoLista<T> Olvashatokent()
        {
            return new OlvashatoLista<T>(this);
        }

        public bool Tartalmaz(T ertek)
        {
            return Keres(ertek, out int index);
        }

        public void Tisztit()
        {
            Darab = 0;
            elemek = new T[AlapertelmezettKapacitas];
        }

        public bool Torol(Predikatum<T> predikatum)
        {
            if (Keres(predikatum, out int index))
            {
                TorolAdottHelyen(index);
                return true;
            }
            else return false;
        }

        public bool Torol(T elem)
        {
            if (Keres(elem, out int index))
            {
                TorolAdottHelyen(index);
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Kitörli a kollekcióból a megadott indexen lévő elemet.
        /// </summary>
        /// <param name="index">Törlendő elem helye. A megadott index [0,Darab] között lehet, máskülönben IndexOutOfRangeExceptiont dob.</param>
        public void TorolAdottHelyen(int index)
        {
            if (index >= Darab || index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            Darab--;
            int kisebbMeret = elemek.Length >> 2;
            if (kisebbMeret >= Darab && kisebbMeret >= AlapertelmezettKapacitas)
            {
                Szukit();
            }

            for (int i = index; i < Darab - 1; i++)
            {
                elemek[i] = elemek[i + 1];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /// <summary>
    /// Lista<T> olvasható párja.
    /// </summary>
    internal class OlvashatoLista<T> : IIndexelhetoKollekcio<T>, IMasolhato<Lista<T>>
    {
        /// <summary>
        /// Lista, aminek olvasható felületéhez hozzáférést biztosít.
        /// </summary>
        protected Lista<T> lista;
        public int Darab => lista.Darab;
        /// <summary>
        /// Lista indexere.
        /// </summary>
        /// <param name="index"> Index értéke [0,Darab] között lehet, máskülönben IndexOutOfRangeExceptiont dob. </param>
        /// <returns></returns>
        public T this[int index] => lista[index];
        /// <summary>
        /// Elkészíti a lista olvaható párját.
        /// </summary>
        /// <param name="lista"></param>
        public OlvashatoLista(Lista<T> lista)
        {
            this.lista = lista;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return lista.GetEnumerator();
        }
        /// <summary>
        /// Megkeresi az elem indexét.
        /// </summary>
        /// <param name="predikatum">Keresendő érték</param>
        /// <param name="index">Elem indexe, ha nem volt sikeres a keresés a lista darabszámával tér vissza </param>
        /// <returns>Keresés sikeressége</returns>
        public bool Keres(T elem, out int index)
        {
            return lista.Keres(elem, out index);
        }
        /// <summary>
        /// Megkeresi az elem indexét.
        /// </summary>
        /// <param name="predikatum">Keresendő érték</param>
        /// <param name="index">Elem indexe, ha nem volt sikeres a keresés a lista darabszámával tér vissza </param>
        /// <returns>Keresés sikeressége</returns>
        public bool Keres(Predikatum<T> predikatum, out int index)
        {
            return lista.Keres(predikatum, out index);
        }

        public T[] MasolTombbe()
        {
            return lista.MasolTombbe();
        }

        public Lista<T> Masol()
        {
            return lista.Masol();
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