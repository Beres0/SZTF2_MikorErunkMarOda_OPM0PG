namespace SZTF2_MikorErunkMarOda_OPM0PG.Megallok
{
   /// <summary>
   /// Benzinkutat reprezentáló osztály
   /// </summary>
    internal class BenzinKut : Megallo
    {

        /// <summary>
        /// Elkészít egy benzinkutat.
        /// Következő szolgáltatást állítja be: Szolgaltatas.Tankolas | Szolgaltatas.Toalett | Szolgaltatas.Kave
        /// </summary>
        /// <param name="tavolsag">Megálló távolsága. 0 vagy kisebb érték esetén ArgumentOuOfRangeExceptiont dob. </param>
        public BenzinKut(int tavolsag) : base(tavolsag,
            Szolgaltatas.Tankolas | Szolgaltatas.Toalett | Szolgaltatas.Kave)
        { }
    }
}