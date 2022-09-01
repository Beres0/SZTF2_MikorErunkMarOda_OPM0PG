using System;
using System.Reflection;

using System.Collections.Generic;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using System.Runtime.CompilerServices;

namespace SZTF2_MikorErunkMarOda_OPM0PG
{
    /// <summary>
    /// Attribútum, amivel meg lehet jelölni kigyűjtésre az osztály tulajdonságait.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    class RekordOszlopAttribute : Attribute
    {
        /// <summary>
        /// Rekord fejléce.
        /// </summary>
        public string Fejlec { get; }
        /// <summary>
        /// Beállítja a fejléc nevét. Alapértelmezetten a tulajdonság neve.
        /// </summary>
        public RekordOszlopAttribute([CallerMemberName] string fejlec = null)
        {
            Fejlec = fejlec;
        }
    }

    /// <summary>
    /// Csak olvasható kollekció. Szöveges formában tárolja a típus RekordOszlop attribútummal ellátott tulajdonságait.
    /// </summary>
    class Rekord<T> : OlvashatoHashLista<Rekord<T>.Oszlop>
    {
        /// <summary>
        /// Megadott generikus típusból reflectionnel kigyűjti a RekordOszloppal rendelkező tulajdonságokat és
        /// eltárolja a lekérdező függvényüket alapértelmezett szöveggé alakítva. Kollekcio az oszlop fejlécét kezeli kulcsként,
        /// ezért ha a tulajdonság fejlécének egyéni név lett megadva, ügyelni kell,
        /// hogy tipuson belül egyedi legyen.  
        /// <para>Kliensnek még ad lehetőséget a módosításra lezárás előtt, 
        /// például ha összetett típus szeretne eltárolni vagy ha bizonyos oszlopnál egyéni formátumot szeretne megjeleníteni.</para>
        /// </summary>
        /// <param name="modositasok">Módosítások létrehozás előtt.</param>
        /// <returns>Elkészített rekord</returns>
        public static Rekord<T> Letrehoz(Akcio<HashLista<Oszlop>> modositasok = null)
        {
            HashLista<Oszlop> lista = new HashLista<Oszlop>((f) => f.Fejlec);
            Type tipus = typeof(T);
            PropertyInfo[] tulajdonsagok = tipus.GetProperties();
            foreach (PropertyInfo tulajdonsag in tulajdonsagok)
            {
                RekordOszlopAttribute attributum = tulajdonsag.GetCustomAttribute<RekordOszlopAttribute>();
                if (attributum != null)
                {
                    lista.Hozzaad(new Oszlop(attributum.Fejlec, (o) => tulajdonsag.GetValue(o)?.ToString()));
                }
            }
            modositasok?.Invoke(lista);
            return new Rekord<T>(lista);
        }
        /// <summary>
        /// Belső osztály, amibe el lehet tárolni a tulajdonság kiolvasott nevét és a szöveggé alakított lekérdező függvényét.
        /// </summary>
        public class Oszlop
        {
            /// <summary>
            /// Tulajdonság fejléce.
            /// </summary>
            public string Fejlec { get; }
            /// <summary>
            /// Tulajdonság lekérdező függvény szövegként.
            /// </summary>
            Fuggveny<object, string> ertek;
            /// <summary>
            /// Elkészít egy oszlopot
            /// </summary>
            /// <param name="fejlec">Tulajdonság fejléce.</param>
            /// <param name="ertek">Tulajdonság lekérdező függvénye szövegkén<./param>
            public Oszlop(string fejlec, Fuggveny<object, string> ertek)
            {
                Fejlec = fejlec;
                this.ertek = ertek;
            }
            public string Ertek(T peldany)
            {
                return ertek(peldany);
            }

        }
        /// <summary>
        /// Továbbadja a kapott hashlistát az ősének.
        /// </summary>
        private Rekord(HashLista<Oszlop> rekord) : base(rekord)
        {
        }
        /// <summary>
        /// Bejárja a tipus fejléceit.
        /// </summary>
        public IEnumerable<string> Fejlecek()
        {
            foreach (var adat in this)
            {
                yield return adat.Fejlec;
            }
        }
        /// <summary>
        /// Bejárja az átadott példány értékeit.
        /// </summary>
        ///<param name="peldany"> Átadott példány. </param>
        public IEnumerable<string> Ertekek(T peldany)
        {
            foreach (var adat in this)
            {
                yield return adat.Ertek(peldany);
            }
        }
    }
}