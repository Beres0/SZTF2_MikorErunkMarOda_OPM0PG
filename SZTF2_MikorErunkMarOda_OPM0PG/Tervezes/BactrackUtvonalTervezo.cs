namespace SZTF2_MikorErunkMarOda_OPM0PG.Tervezes
{
    /// <summary>
    /// Tervező, ami visszalépéses kereséssel keresi az útitervet. Mindig optimális megoldást nyújt.
    /// </summary>
    internal class BactrackUtvonalTervezo : UtvonalTervezo
    {
        /// <summary>
        /// Elmentett megoldás.
        /// </summary>
        private bool[] megoldas;

        /// <summary>
        /// Elmentett megoldás megállásainak száma. Ha nincs megoldás, akkor az értéke a megállók darabszáma+1-gyel egyenlő.
        /// </summary>
        private int min;

        /// <summary>
        /// Elkészít egy backtrack útvonal tervezőt
        /// </summary>
        /// <param name="utazas">Utázás, ami alapján megtervezi az útitervet.</param>
        public BactrackUtvonalTervezo(Utazas utazas) : base(utazas)
        {
            min = utazas.Utvonal.Darab + 1;
        }
        /// <summary>
        /// Betölti a státuszokba az elmentett megoldást.
        /// </summary>
        private void Betolt()
        {
            for (int i = 1; i < tervezoStatuszok.Length; i++)
            {
                tervezoStatuszok[i].Megall = megoldas[i];
            }
            FrissitStatuszok();
        }
        /// <summary>
        /// Akkor fogadja el a lehetséges részmegoldást, ha a megoldás szintjén nem fogyott el az üzemanyag
        /// és az utasok számlálója közül egyik sem telt le vagy ha valamelyik letelt, akkor megfelelő megálló esetén megáll-e a busz. 
        /// </summary>
        /// <param name="szint"></param>
        /// <returns></returns>
        private bool Elfogad(int szint)
        {
            TervezoStatusz parameter = tervezoStatuszok[szint];
            if (parameter.UzemanyagSzintMegalloElott <1) return false;
            int i = 0;

            while
                (i < igenyPerDarab.Length &&
                (parameter.SzamlalokMegalloElott[i] >0 ||
                (igenyPerDarab[i].Kulcs.Megfelel(parameter.Megallo) && parameter.Megall)))
            {
                i++;
            }

            return i >= igenyPerDarab.Length;
        }
        /// <summary>
        /// Elmenti a jelenlegi megoldást.
        /// </summary>
        private void Ment()
        {
            megoldas = new bool[tervezoStatuszok.Length];
            for (int i = 1; i < tervezoStatuszok.Length; i++)
            {
                megoldas[i] = tervezoStatuszok[i].Megall;
            }
        }

        /// <summary>
        /// Tervezt meghívja az első szinten, 0 darab megállással.
        /// Ha végére ért a tervezés és nincs megoldás, akkor Utvonal.Kivetelt dob.
        /// Ha van, akkor betolti a megoldást a státuszokba.
        /// <para> Általános visszalépéses keresésnek megfeleltetve:</para>
        /// <para> - Cél: legkevesebb megállóból álló útiterv</para>
        /// <para> - N- megállók száma</para>
        /// <para> - M[szint]: M[1...(N-1)] = 2, M[N] = 1  </para>
        /// <para> - R[szint]: R[1...(N-1)] = { Megáll(igaz); Nem áll meg(hamis) }, R[N] = { Megáll(igaz) }</para>
        /// <para> - Ft->Elfogad</para>
        /// <para> - szétválasztás/korlátozás: ha megállások száma a keresési ágon eléri az aktuális minimumot, nem folytatja tovább </para>
        /// <para>
        /// Futásidő komponensek(n: megállók száma, m: utastípusok száma):</para>
        /// <para> - FrissitStatuszok: n</para>
        /// <para> - Elfogad: m</para>
        /// <para> - Tervez[1-n]: 2^n*Elfogad*FrissitStatuszok</para>
        /// <para> Futásidő: Tervez[1-n]=2^n*m*n -> O(2^n*m*n) </para>
        /// </summary>
        protected override void Tervez()
        {
            Tervez(1, 0);
            if (megoldas != null)
            {
                Betolt();
            }
            else throw new Kivetel(tervezoStatuszok);
        }
        /// <summary>
        /// Addig ágaztatja tovább rekurzívan a visszalépéses keresést, amíg nem ért végére, van elfogadható részmegoldás és 
        /// a részmegoldások megállásainak száma kisebb, mint a jelenleg elmentett megoldásé.
        /// Előszőr beállítja hamisra a szinten a megállást, frissíti a státuszt és megvizsgálja-e Elfogad-dal, hogy megfelelő megoldás-e,
        /// utána meghívja a következő szinten saját magát ugyanannyi megállással.
        /// Ezt mégegyszer megteszi igaz értekkel is, annyi különbséggel, 
        /// hogy Elfogad mellett azt is megvizsgálja, hogy eggyel több megállással is kisebb-e, mint a minimum.
        /// Ha a végére ért és a megoldás elfogadható, kisebb, mint a jelenlegi minimum, akkor elmenti. 
        /// </summary>
        /// <param name="szint">Részmegoldás szintje.</param>
        /// <param name="megallasokSzama">Részmegoldás megállásainak száma.</param>
        private void Tervez(int szint, int megallasokSzama)
        {
            if (szint == tervezoStatuszok.Length - 1)
            {
                tervezoStatuszok[szint].Megall = true;
                megallasokSzama++;
                FrissitStatuszok(szint);
                if (Elfogad(szint)&&megallasokSzama<min)
                {
                        Ment();
                        min = megallasokSzama;
                }
            }
            else
            {
                tervezoStatuszok[szint].Megall = false;
                FrissitStatuszok(szint);
                if (Elfogad(szint))
                {
                    Tervez(szint + 1, megallasokSzama);
                }

                tervezoStatuszok[szint].Megall = true;
                megallasokSzama++;
                FrissitStatuszok(szint);
                if (Elfogad(szint)&&megallasokSzama<min)
                {
                    Tervez(szint + 1, megallasokSzama);
                }
            }
        }
    }
}