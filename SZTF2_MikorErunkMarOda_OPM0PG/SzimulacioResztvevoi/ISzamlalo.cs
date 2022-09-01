namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Felület a szimuláció aktív résztvevőinek.
    /// </summary>
    internal interface ISzamlalo
    {
        /// <summary>
        /// Szöveges információ az állapotáról.
        /// </summary>
        string Allapot { get; }
        /// <summary>
        /// Milyen igénnyel rendelkezik.
        /// </summary>
        Igeny Igeny { get; }
        /// <summary>
        /// Hol tart a számlálója.
        /// </summary>
        float Szamlalo { get; }
    }
}