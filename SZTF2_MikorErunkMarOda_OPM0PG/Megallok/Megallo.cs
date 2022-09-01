using System;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megallok
{
    /// <summary>
    /// Konkrét megallók ősosztálya.
    /// </summary>
    internal abstract class Megallo
    {
        /// <summary>
        /// Generáláshoz használt peldányosító függvények.
        /// </summary>
        private static Fuggveny<int,Random, Megallo>[] peldanyositok = {
                (t,r) => new BenzinKut(t),
                (t,r) => new PihenoHely(t),
                (t,r) => new McDonalds(t),
                (t,r) => new Nagyvaros(t,r),
        };
        /// <summary>
        /// Az autópálya sebességkorlátja km/h-ban.
        /// </summary>
        public const int SebessegKorlatAutopalya = 90;
        /// <summary>
        /// Távolság az előző megállótól
        /// </summary>
        public int Tavolsag { get; }
        /// <summary>
        /// Ilyen szolgáltatás/szolgáltatásokkal rendelkezik a megálló.
        /// </summary>
        public Szolgaltatas Szolgaltatas { get; }
        /// <summary>
        /// Beállítja a kezdőértékeket.
        /// </summary>
        /// <param name="tavolsag">Megálló távolsága. 0 vagy kisebb érték esetén ArgumentOuOfRangeExceptiont dob.  </param>
        /// <param name="szolgaltatas"> Megálló szolgáltatása.</param>
        public Megallo(int tavolsag, Szolgaltatas szolgaltatas)
        {
            if (tavolsag <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tavolsag));
            }

            Tavolsag = tavolsag;
            Szolgaltatas = szolgaltatas;
        }
        /// <summary>
        /// Legenerál n darab megállót.
        /// </summary>
        /// <param name="random">Random, amit generáláshoz használ.</param>
        /// <param name="tavolsagMin"> Generált távolságok alsóhatára. </param>
        /// <param name="tavolsagMax"> Generált távolság felsőhatára. </param>
        /// <param name="darab"> Ennyi darab megállót generál le.</param>
        /// <returns>Generált megállók láncoltlistája</returns>
        public static LancoltLista<Megallo> General(Random random, int tavolsagMin, int tavolsagMax, int darab)
        {
            LancoltLista<Megallo> megallok = new LancoltLista<Megallo>();
            for (int i = 0; i < darab; i++)
            {
                megallok.Hozzaad(random.Kivalaszt(peldanyositok)(random.Next(tavolsagMin, tavolsagMax + 1),random));
            }
            return megallok;
        }
        /// <summary>
        /// Megadja a sebességkorlátot a busz megállótól való távolsága függvényében. Alapértelmezetten a SebessegKorlatAutopalya értékével tér vissza.
        /// </summary>
        /// <param name="buszTavolsaga"> Busz távolsága a megállótól.</param>
        /// <returns></returns>
        public virtual int SebessegKorlat(float buszTavolsaga)
        {
            return SebessegKorlatAutopalya;
        }
        /// <summary>
        /// "Formátum: {Type}-{Tavolsag}"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name + "-" + Tavolsag;
        }
    }
}