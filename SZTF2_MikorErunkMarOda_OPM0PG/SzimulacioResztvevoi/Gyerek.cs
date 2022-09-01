using System;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Gyereket reprezentáló osztály.
    /// </summary>
    internal class Gyerek : Szemely
    {
        /// <summary>
        /// Gyerekek igénye: nasi maximum 300 percenként.
        /// </summary>
        public static readonly Igeny GyerekIgeny = new Igeny(Szolgaltatas.Nasi, 300);
        /// <summary>
        /// Leszálláskor "Nasizik".
        /// </summary>
        protected override string LeszalltAllapot => "Nasizik";
        /// <summary>
        /// Letelt számlálókor "Éhes".
        /// </summary>
        protected override string LeteltSzamlaloAllapot => "Éhes";
        public override Igeny Igeny => GyerekIgeny;
        /// <summary>
        /// Elkészít egy gyereket.
        /// </summary>
        /// <param name="nev">Gyerek neve.</param>
        public Gyerek(string nev) : base(nev)
        { }
        /// <summary>
        /// Készít egy gyereket véletlenszerűen választott névvel.
        /// <para> Ha valamiért nem sikerült betölteni a gyerekekre jellemző neveket, akkor Szemely.Kivetelt dob.</para>
        /// </summary>
        /// <param name="random">Random, ami a nevet kiválasztja</param>
        public Gyerek(Random random) : base(random)
        {
        }
    }
}