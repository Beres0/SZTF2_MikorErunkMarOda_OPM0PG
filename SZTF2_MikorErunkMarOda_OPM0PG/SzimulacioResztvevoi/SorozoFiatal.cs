using System;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Söröző fiatalt reprezentáló osztály.
    /// </summary>
    internal class SorozoFiatal : Szemely
    {
        /// <summary>
        /// Sőrőző fiatalok igénye: Toalett maximum 120 percenként.
        /// </summary>
        public static readonly Igeny SorozoFiatalIgeny = new Igeny(Szolgaltatas.Toalett, 120);
        /// <summary>
        /// Leszálláskor "Könnyít magán".
        /// </summary>
        protected override string LeszalltAllapot => "Könnyít magán";
        /// <summary>
        /// Letelt számlálókor "Lassan bepisil".
        /// </summary>
        protected override string LeteltSzamlaloAllapot => "Lassan bepisil";
        public override Igeny Igeny => SorozoFiatalIgeny;
        /// <summary>
        /// Elkészít egy söröző fiatalt.
        /// </summary>
        /// <param name="nev">Söröző fiatal neve.</param>
        public SorozoFiatal(string nev) : base(nev)
        { }
        /// <summary>
        /// Készít egy söröző fiatalt véletlenszerűen választott névvel.
        /// <para> Ha valamiért nem sikerült betölteni a söröző fiatalokra jellemző neveket, akkor Szemely.Kivetelt dob.</para>
        /// </summary>
        /// <param name="random">Random, ami a nevet kiválasztja</param>
        public SorozoFiatal(Random random) : base(random)
        {
        }
    }
}