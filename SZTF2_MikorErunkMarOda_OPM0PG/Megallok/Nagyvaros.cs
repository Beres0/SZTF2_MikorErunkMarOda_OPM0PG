using System;
namespace SZTF2_MikorErunkMarOda_OPM0PG.Megallok
{
    /// <summary>
    /// Nagyvarost reprezentáló osztály.
    /// </summary>
    internal class Nagyvaros : Megallo
    {
        /// <summary>
        /// Nagyvárosi kiterjedés maximuma.
        /// </summary>
        public const int KiterjedesMax = 10;
        /// <summary>
        /// Nagyvárosi kiterjedés minimuma.
        /// </summary>
        public const int KiterjedesMin = 4;
        /// <summary>
        /// Nagyvárosi sebességkorlát kiterjedésén belül.
        /// </summary>
        public const int SebessegKorlatNagyvaros = 30;
        /// <summary>
        /// Nagyváros kiterjedése.
        /// </summary>
        public int Kiterjedes { get; }
        /// <summary>
        /// Elkészít egy nagyvárost. Kiterjedését véletlenszerűen választja ki a nagyvárosi kiterjedés minimuma és maximuma között.
        /// <para>Következő szolgáltatást állítja be: Szolgaltatas.Toalett | Szolgaltatas.Nasi | Szolgaltatas.Kave </para>
        /// </summary>
        /// <param name="tavolsag">Megálló távolsága. Nem lehet kisebb, mint a nagyvárosi kiterjedés maximum, különben ArgumentOutOfRangeException-t dob. </param>
        /// <param name="random">Kiterjedés generálásához használt random</param>
        public Nagyvaros(int tavolsag, Random random) : base(tavolsag, Szolgaltatas.Toalett | Szolgaltatas.Nasi | Szolgaltatas.Kave)
        {
            if (tavolsag < KiterjedesMax)
            {
                throw new ArgumentOutOfRangeException(nameof(tavolsag));
            }
            Kiterjedes = random.Next(KiterjedesMin, KiterjedesMax + 1);
        }
        /// <summary>
        /// Kiterjedésén kívül az autópálya sebességkorlátja él, míg a kiterjédésén belül a nagyvárosi sebességkorlát.
        /// </summary>
        /// <param name="buszTavolsaga"></param>
        /// <returns></returns>
        public override int SebessegKorlat(float buszTavolsaga)
        {
            if (buszTavolsaga > Kiterjedes)
            {
                return base.SebessegKorlat(buszTavolsaga);
            }
            else return SebessegKorlatNagyvaros;
        }
        /// <summary>
        /// Formátum: {Type}-{Tavolsag}-{Kiterjedes}
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + $"-{Kiterjedes}";
        }
    }
}