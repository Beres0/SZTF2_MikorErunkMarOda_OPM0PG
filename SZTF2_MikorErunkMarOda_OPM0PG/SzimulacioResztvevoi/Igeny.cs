using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Struktúra, amely egységbe zárja a szimuláció résztvevőinek szükségletét és ciklusát.
    /// </summary>
    internal struct Igeny
    {
        /// <summary>
        /// Igény ciklusa.
        /// </summary>
        public int Ciklus { get; }

        /// <summary>
        /// Leírja, hogy milyen szolgáltatásra van igény.
        /// </summary>
        public Szolgaltatas Szolgaltatas { get; }
        
        /// <summary>
        /// Elkészít egy igényt.
        /// </summary>
        /// <param name="megfeleloSzolgaltatas">Igény szolgáltatása</param>
        /// <param name="ciklus">Igény ciklusa</param>
        public Igeny(Szolgaltatas megfeleloSzolgaltatas, int ciklus)
        {
            Szolgaltatas = megfeleloSzolgaltatas;
            Ciklus = ciklus;
        }

        public bool Megfelel(Megallo megallo)
        {
            return (Szolgaltatas & megallo.Szolgaltatas) == Szolgaltatas;
        }
    }
}