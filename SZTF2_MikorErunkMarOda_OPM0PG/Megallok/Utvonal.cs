using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megallok
{
    /// <summary>
    /// Megállók olvasható láncoltlistája.
    /// </summary>
    internal class Utvonal : OlvashatoLancoltLista<Megallo>
    {
        /// <summary>
        /// Útvonal hossza.
        /// </summary>
        public int Hossz { get; }
        /// <summary>
        /// Elkészíti az útvonalat.
        /// </summary>
        /// <param name="megallok">Megállók kollekciója</param>
        public Utvonal(IOlvashatoKollekcio<Megallo> megallok) : base(new LancoltLista<Megallo>(megallok))
        {
            Hossz = megallok.Osszeg((m) => m.Tavolsag);
        }
    }
}