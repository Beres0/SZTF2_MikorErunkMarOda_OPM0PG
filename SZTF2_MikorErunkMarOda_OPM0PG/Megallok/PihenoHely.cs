namespace SZTF2_MikorErunkMarOda_OPM0PG.Megallok
{
    /// <summary>
    /// Pihenőhelyet reprezentáló osztály.
    /// </summary>
    internal class PihenoHely : Megallo
    {
        /// <summary>
        /// Elkészít egy pihenőhelyet.
        /// Következő szolgáltatást állítja be: MegalloTualjdonsag.Toalett
        /// </summary>
        /// <param name="tavolsag">Megálló távolsága. 0 vagy kisebb érték esetén ArgumentOuOfRangeExceptiont dob.  </param>
        public PihenoHely(int tavolsag) : base(tavolsag, Szolgaltatas.Toalett)
        { }
    }
}