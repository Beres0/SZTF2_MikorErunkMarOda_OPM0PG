using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Személyeket igényük alapján csoportosító, olvasható lista.
    /// </summary>
    internal class SzemelyLista : OlvashatoCsoportositoLista<Igeny, Szemely>
    {
        /// <summary>
        /// Készít egy személylistát.
        /// </summary>
        /// <param name="szemelyek">Személyek kollekciója, amit csoportosít.</param>
        public SzemelyLista(IOlvashatoKollekcio<Szemely> szemelyek)
            : base(new CsoportositoLista<Igeny, Szemely>((s) => s.Igeny, szemelyek))
        { }
        /// <summary>
        /// Csoportokból elkészít egy kulcs-érték pár tömböt. 
        /// <para> Kulcs: személyek igénye.</para>
        /// <para> Érték: Igényhez tartozó csoport darabszáma.</para>
        /// </summary>
        /// <returns></returns>
        public Par<Igeny, int>[] IgenyPerDarab()
        {
            var igenyPerDarab = new Par<Igeny, int>[CsoportokDarab];
            int i = 0;
            foreach (var csoport in Csoportok())
            {
                igenyPerDarab[i++] = new Par<Igeny, int>(csoport.Elso.Igeny, csoport.Darab);
            }
            return igenyPerDarab;
        }
    }
}