namespace SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok
{
    /// <summary>
    /// Kulcs-érték párokat reprezentáló struktúra. 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal struct Par<K, T>
    {
        /// <summary>
        /// Pár értéke.
        /// </summary>
        public T Ertek { get; }
        /// <summary>
        /// Pár kulcsa.
        /// </summary>
        public K Kulcs { get; }

        /// <summary>
        /// Elkészít egy kulcs-érték párt.
        /// </summary>
        /// <param name="kulcs"></param>
        /// <param name="ertek"></param>
        public Par(K kulcs, T ertek)
        {
            Kulcs = kulcs;
            Ertek = ertek;
        }
        /// <summary>
        /// Formátum: ({Kulcs};{Ertek})
        /// </summary>
        public override string ToString()
        {
            return $"({Kulcs};{Ertek})";
        }
    }
}