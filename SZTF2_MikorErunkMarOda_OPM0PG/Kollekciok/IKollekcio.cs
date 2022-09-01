using System.Collections.Generic;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok
{

    /// <summary>
    /// Felület az objektum másolhatóságához.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IMasolhato<T>
    {
        /// <summary>
        /// Elkészíti a másolatot.
        /// </summary>
        T Masol();
    }
    /// <summary>
    /// Felület a kollekcióknak, amivel elő állítják az olvasható párjukat. 
    /// </summary>
    interface IOlvashatokent<T,TOlvashato>  where TOlvashato:IOlvashatoKollekcio<T>
    {
        /// <summary>
        /// Létrehozza a kollekciót olvashatóként.
        /// </summary>
        TOlvashato Olvashatokent();
    }
    /// <summary>
    /// IOlvashatoKollekcio-t egészíti ki metódusokkal.
    /// </summary>
    internal static class KollekcioKiegeszites
    {
        /// <summary>
        /// Elemek egy kiválasztott adattagja alapján kiszámolja az átlagot.
        /// </summary>
        /// <param name="kollekcio">Kiegészítés kollekciója.</param>
        /// <param name="kivalaszto">Kiválasztja, hogy mi alapján átlagoljon a függvény.</param>
        /// <returns>Kiszámított átlag.</returns>
        public static double Atlag<T>(this IOlvashatoKollekcio<T> kollekcio, Fuggveny<T, int> kivalaszto)
        {
            double szum = kollekcio.Osszeg(kivalaszto);
            return szum / kollekcio.Darab;
        }
        /// <summary>
        /// Elemek egy kiválasztott adattagja alapján kiszámolja az átlagot.
        /// </summary>
        /// <param name="kollekcio">Kiegészítés kollekciója.</param>
        /// <param name="kivalaszto">Kiválasztja, hogy mi alapján átlagoljon a függvény.</param>
        /// <returns>Kiszámított átlag.</returns>
        public static double Atlag<T>(this IOlvashatoKollekcio<T> kollekcio, Fuggveny<T, double> kivalaszto)
        {
            double szum = kollekcio.Osszeg(kivalaszto);
            return szum / kollekcio.Darab;
        }
        /// <summary>
        /// Megszámolja hány P tulajdonságú elem található a kollekcióban.
        /// </summary>
        /// <param name="kollekcio">Kiegészítés kollekciója.</param>
        /// <param name="predikatum">P tulajdonság.</param>
        /// <returns>P tulajdonságú elemek darabszáma.</returns>
        public static int Darab<T>(this IOlvashatoKollekcio<T> kollekcio, Predikatum<T> predikatum)
        {
            int darab = 0;
            foreach (var elem in kollekcio)
            {
                if (predikatum(elem))
                {
                    darab++;
                }
            }
            return darab;
        }
        /// <summary>
        /// Lineárisan megkeresi P tulajdonságú elem első előfordulását. 
        /// </summary>
        /// <param name="kollekcio">Kiegészítés célpontja.</param>
        /// <param name="predikatum">P tulajdonság</param>
        /// <param name="ertek">P tulajdonság első előfordulása.
        /// Ha nincs ilyen elem a kollekcióban, akkor default értékkel tér vissza </param>
        /// <returns>Keresés eredménye.</returns>
        public static bool Keres<T>(this IOlvashatoKollekcio<T> kollekcio, Predikatum<T> predikatum, out T ertek)
        {
            ertek = default;
            foreach (var elem in kollekcio)
            {
                if (predikatum(elem))
                {
                    ertek = elem;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Eldönti,hogy a kollekcióban az összes elem P tulajdonsággal rendelkezik-e
        /// </summary>
        /// <param name="kollekcio">Kiegészített kollekció.</param>
        /// <param name="predikatum">P tulajdonság</param>
        /// <returns>Keresés sikeressége.</returns>
        public static bool Mindegyik<T>(this IOlvashatoKollekcio<T> kollekcio, Predikatum<T> predikatum)
        {
            return !Keres(kollekcio, (t) => !predikatum(t), out T ertek);
        }
        /// <summary>
        /// Elemeket összegzi egy kiválasztott adattagja alapján.
        /// </summary>
        /// <param name="kollekcio">Kiegészítés kollekciója.</param>
        /// <param name="kivalaszto">Kiválasztja, hogy mi alapján összegezzen a függvény.</param>
        /// <returns>Kiszámított összeg.</returns>
        public static int Osszeg<T>(this IOlvashatoKollekcio<T> kollekcio, Fuggveny<T, int> kivalaszto)
        {
            int szum = 0;
            foreach (var elem in kollekcio)
            {
                szum += kivalaszto(elem);
            }
            return szum;
        }
        /// <summary>
        /// Elemeket összegzi egy kiválasztott adattagja alapján.
        /// </summary>
        /// <param name="kollekcio">Kiegészítés kollekciója.</param>
        /// <param name="kivalaszto">Kiválasztja, hogy mi alapján összegezzen a függvény.</param>
        /// <returns>Kiszámított összeg.</returns>
        public static double Osszeg<T>(this IOlvashatoKollekcio<T> kollekcio, Fuggveny<T, double> kivalaszto)
        {
            double szum = 0;
            foreach (var elem in kollekcio)
            {
                szum += kivalaszto(elem);
            }
            return szum;
        }
        /// <summary>
        /// Kollekció elemeit egy kiválasztott szöveges reprezentáció alapján összefűzi egy karakterlánncá.
        /// </summary>
        /// <param name="kollekcio">Kiegészített kollekció.</param>
        /// <param name="kivalaszto">Kiválasztja az elem szöveges reprezentációját </param>
        /// <param name="elvalaszto">Elemeket ezzel választja el egymástól a karakterláncban.</param>
        /// <returns>Összefűzött karakterlánc</returns>
        public static string Osszefuz<T>(this IOlvashatoKollekcio<T> kollekcio, Fuggveny<T,string> kivalaszto,string elvalaszto)
        {
            return Szoveg.Osszefuz(kollekcio, kivalaszto, elvalaszto);
        }
        /// <summary>
        /// Összefűz egy szöveges kollekciót.
        /// </summary>
        /// <param name="kollekcio">Kiegészített kollekció.</param>
        /// <param name="elvalaszto">Elemeket ezzel választja el egymástól a karakterláncban.</param>
        /// <returns>Összefűzött karakterlánc</returns>
        public static string Osszefuz(this IOlvashatoKollekcio<string> kollekcio, string elvalaszto)
        {
            return Szoveg.Osszefuz(kollekcio, elvalaszto);
        }
        /// <summary>
        /// Eldönti, hogy üres vagy null-e a kollekció.
        /// </summary>
        /// <param name="kollekcio">Kiegészített kollekció.</param>
        public static bool Ures<T>(this IOlvashatoKollekcio<T> kollekcio)
        {
            return kollekcio == null || kollekcio.Darab == 0;
        }
        /// <summary>
        /// Eldönti, hogy van-e P tulajdonságú elem a kollekcióban.
        /// </summary>
        /// <param name="kollekcio">Kiegészített kollekció.</param>
        public static bool Van<T>(this IOlvashatoKollekcio<T> kollekcio, Predikatum<T> predikatum)
        {
            return Keres(kollekcio, predikatum, out T ertek);
        }
    }
    /// <summary>
    /// Bővíti az IOlvashatoKollekcio felületét olvasható indexeléssel.
    /// </summary>
    internal interface IIndexelhetoKollekcio<T> : IOlvashatoKollekcio<T>
    {
        /// <summary>
        /// </summary>
        /// <returns>Indexen található elem.</returns>
        T this[int index] { get; }
    }
    /// <summary>
    /// Bővíti az IOlvashatoKollekció felületét írható műveletekkel.
    /// </summary>
    internal interface IKollekcio<T> : IOlvashatoKollekcio<T>
    {
        /// <summary>
        /// Hozzáad egy elemet a kollekcióhoz.
        /// </summary>
        /// <param name="ertek">Hozzáadandó érték.</param>
        void Hozzaad(T ertek);
        /// <summary>
        /// Kiüríti a kollekciót.
        /// </summary>
        void Tisztit();
       /// <summary>
       /// Törli az elemet, ha megtalálható a kollekcióban.
       /// </summary>
       /// <param name="ertek"></param>
       /// <returns>Történt-e törlés.</returns>
        bool Torol(T ertek);
        /// <summary>
        /// Törli az első P tulajdonságú elemet a kollekcióban, ha talált olyat.
        /// </summary>
        /// <param name="predikatum">Keresett P tulajdonság.</param>
        /// <returns>Történt-e törlés.</returns>
        bool Torol(Predikatum<T> predikatum);
    }
    /// <summary>
    /// Olvasható generikus kollekciók általános alapfelületét írja elő.
    /// </summary>
    internal interface IOlvashatoKollekcio<T> : IEnumerable<T>
    {
        /// <summary>
        /// Kollekcióban található elemek darabszáma.
        /// </summary>
        int Darab { get; }
        /// <summary>
        /// Belemásolja egy új tömbbe a kollekció tartalmát.
        /// </summary>
        T[] MasolTombbe();
        /// <summary>
        /// Eldönti, hogy az adott elemet tartalmazza-e a kollekció.
        /// </summary>
        bool Tartalmaz(T ertek);
        
    }
}