namespace SZTF2_MikorErunkMarOda_OPM0PG.Megallok
{
    /// <summary>
    /// McDonaldst reprezentáló osztály.
    /// </summary>
    internal class McDonalds : Megallo
    {
        /// <summary>
        /// Elkészít egy McDonalds-t.
        /// Következő szolgáltatást állítja be: Szolgaltatas.Toalett | Szolgaltatas.Kave | Szolgaltatas.Nasi
        /// </summary>
        /// <param name="tavolsag">Megálló távolsága. 0 vagy kisebb érték esetén ArgumentOuOfRangeExceptiont dob. </param>
        public McDonalds(int tavolsag) : base(tavolsag,
            Szolgaltatas.Toalett | Szolgaltatas.Kave | Szolgaltatas.Nasi)
        { }
    }
}