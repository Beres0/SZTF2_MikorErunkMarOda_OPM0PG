using System;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Nagymamát reprezentáló osztály.
    /// </summary>
    internal class Nagymama : Szemely
    {
        /// <summary>
        /// Nagymamák igénye:Bármilyen megállónál maximum 180 percenként meg szeretné mozgatni magát.
        /// </summary>
        public static readonly Igeny NagymamaIgeny = new Igeny(Szolgaltatas.Semmi, 180);
        /// <summary>
        /// Leszálláskor "Megmozgatja magát".
        /// </summary>
        protected override string LeszalltAllapot => "Megmozgatja magát";
       /// <summary>
       /// Letelt számlálókor "Megfájdult a dereka".
       /// </summary>
        protected override string LeteltSzamlaloAllapot => "Megfájdult a dereka";
        public override Igeny Igeny => NagymamaIgeny;

        /// <summary>
        /// Elkészít egy nagymamát.
        /// </summary>
        /// <param name="nev">Nagymama neve.</param>
        public Nagymama(string nev) : base(nev)
        { }
        /// <summary>
        /// Készít egy nagymamát véletlenszerűen választott névvel.
        /// <para> Ha valamiért nem sikerült betölteni a nagymamákra jellemző neveket, akkor Szemely.Kivetelt dob.</para>
        /// </summary>
        /// <param name="random">Random, ami a nevet kiválasztja</param>
        public Nagymama(Random random) : base(random)
        {
        }
    }
}