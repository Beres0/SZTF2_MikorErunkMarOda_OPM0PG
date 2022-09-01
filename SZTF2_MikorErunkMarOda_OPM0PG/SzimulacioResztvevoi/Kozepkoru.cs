using System;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Középkorút reprezentáló osztály.
    /// </summary>
    internal class Kozepkoru : Szemely
    {
        /// <summary>
        /// Középkprúak igénye: Toalett és kávé maximum 240 percenként.
        /// </summary>
        public static readonly Igeny KozepkoruIgeny = new Igeny(Szolgaltatas.Toalett | Szolgaltatas.Kave, 240);
        /// <summary>
        /// Leszálláskor "Kávézgat".
        /// </summary>
        protected override string LeszalltAllapot => "Kávézgat";
        /// <summary>
        /// Letelt számlálónál: "Meginna már egy kávét".
        /// </summary>
        protected override string LeteltSzamlaloAllapot => "Meginna már egy kávét";
        public override Igeny Igeny => KozepkoruIgeny;
        /// <summary>
        /// Elkészít egy középkorút.
        /// </summary>
        /// <param name="nev">Középkorú neve.</param>
        public Kozepkoru(string nev) : base(nev)
        { }
        /// <summary>
        /// Készít egy középkorút véletlenszerűen választott névvel.
        /// <para> Ha valamiért nem sikerült betölteni a középkorúakra jellemző neveket, akkor Szemely.Kivetelt dob.</para>
        /// </summary>
        /// <param name="random">Random, ami a nevet kiválasztja</param>
        public Kozepkoru(Random random) : base(random)
        {
        }
    }
}