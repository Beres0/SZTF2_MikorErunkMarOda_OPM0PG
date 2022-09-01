using System;
using System.IO;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Buszon utazó személyeket reprezentáló osztály.
    /// </summary>
    internal abstract class Szemely : ISzamlalo
    {
        /// <summary>
        /// Kivétel, ami akkor dobódik, ha a konkrét személytípusok nevei betöltése problémába ütközött.
        /// </summary>
        public class Kivetel:KulcsKivetel<string>
        {
            /// <summary>
            /// Elkészíti a kivételt.
            /// </summary>
            /// <param name="tipusNev">Szemely konkrét típusának neve</param>
            public Kivetel(string tipusNev) : base(tipusNev, $"{tipusNev} neveit nem sikerült betölteni!")
            {
            }
        }
        /// <summary>
        /// Generáláshoz használt példányosító függvények.
        /// </summary>
        private static Fuggveny<Random,Szemely>[] peldanyosito = {
                (r)=>new Nagymama(r),
                (r)=>new SorozoFiatal(r),
                (r)=>new Kozepkoru(r),
                (r)=>new Gyerek(r)
        };
        /// <summary>
        /// Nevek tömbje személyek
        /// ///típusa szerint.
        /// </summary>
        private static readonly HashTabla<string, string[]> nevek =IOMuveletek.BetoltNevek();
        
        /// <summary>
        /// Állapot megállásnál. Értéke körülményektől változik.
        /// </summary>
        public string allasAllapot;
        /// <summary>
        /// Alapértelmezett állapot, amikor leszállt egy megállónál a buszról: "Pihen".
        /// </summary>
        protected virtual string LeszalltAllapot => "Pihen";
        /// <summary>
        /// Állapot, amikor letelt a számlálója.
        /// </summary>
        protected abstract string LeteltSzamlaloAllapot { get; }
        /// <summary>
        /// Állapot, amikor a busz a következő megálló felé tart: "Utazik".
        /// </summary>
        protected virtual string UtazoAllapot => "Utazik";

        /// <summary>
        /// Alapértelmezett állapot, amikor nem száll le egy megállóról, mert nem felelt meg neki: "Várakozik".
        /// </summary>
        protected virtual string VarakozoAllapot => "Várakozik";
        public string Allapot
        {
            get
            {
                if (Busz == null)
                {
                    return VarakozoAllapot;
                }
                else if (Szamlalo <= 0)
                {
                    return LeteltSzamlaloAllapot;
                }
                else if (Busz.Sebesseg > 0)
                {
                    return UtazoAllapot;
                }
                else return allasAllapot;
            }
        }
        /// <summary>
        /// Busz, amin a személy tartózkodik.
        /// </summary>
        public Busz Busz { get; private set; }
        public abstract Igeny Igeny { get; }
        /// <summary>
        /// Személy neve.
        /// </summary>
        public string Nev { get; }
        public float Szamlalo { get; protected set; }

        /// <summary>
        /// Esemény, ami akkor váltódik ki, ha a személy megfelelőnek talált egy megállót és leszállt.
        /// </summary>
        public event EsemenyKezelo<Szemely> IdeiglenesenLeszallt;
        /// <summary>
        /// Beállítja a kezdőértékeket
        /// </summary>
        private Szemely()
        {
            allasAllapot = VarakozoAllapot;
            Szamlalo = Igeny.Ciklus;
        }

        /// <summary>
        /// Beállítja a személy nevét.
        /// </summary>
        /// <param name="nev">Személy neve.</param>
        public Szemely(string nev):this()
        {
            Nev = nev;
        }
        /// <summary>
        /// Beállít egy generált nevet.
        /// <para> Ha valamiért nem sikerült betölteni a típusra jellemző neveket, akkor Szemely.Kivetelt dob.</para>
        /// </summary>
        /// <param name="random">Random, ami a nevet kiválasztja</param>
        public Szemely(Random random) : this()
        {
            string tipus = GetType().Name;
            try
            {

                Nev=random.Kivalaszt(nevek["Vezeteknevek"])+" "+random.Kivalaszt(nevek[tipus]);
            }
            catch (KulcsKivetel<string>)
            {
                throw new Kivetel(tipus);
            }
        }
        /// <summary>
        /// Busz.Megallt eseményre iratkozik fel.
        /// <para> Megfelelő megálló esetén visszatölti a számlálóját,
        /// állapotát leszállt állapotra változtatja és elsüti az IdeiglenesenLeszallt eseményét.
        /// Ha nem felelt meg neki a megálló, akkor várakozó állapotot állítja be.</para>
        /// </summary>
        /// <param name="kuldo">Busz, amin utazik.</param>
        /// <param name="argumentum">Megálló, ahol a busz megállt.</param>
        private void Busz_Megallt(Busz kuldo, Megallo argumentum)
        {
            if (Igeny.Megfelel(argumentum))
            {
                Szamlalo = Igeny.Ciklus;
                allasAllapot = LeszalltAllapot;
                IdeiglenesenLeszallt?.Invoke(this);
            }
            else allasAllapot = VarakozoAllapot;
        }
        /// <summary>
        /// Busz.ElerteAVegallomast eseményre iratkozik fel.
        /// <para>Állapotát leszálltra változtatja és leiratkozik az összes eseményről, amire feliratkozott felszálláskor.</para>
        /// </summary>
        /// <param name="kuldo"></param>
        protected void Busz_ElerteAVegallomast(Busz kuldo)
        {
            allasAllapot = "Leszállt";
            Leiratkozasok(kuldo);
        }
        /// <summary>
        /// Következő eseményekre iratkozik fel azonos nevű metódusaival:
        /// <para> Busz.Megallt. </para>
        /// <para> Busz.Utazas.Ido.EltelEgyPerc </para>
        /// <para> Busz.ElerteAVegallomast </para>
        /// </summary>
        /// <param name="busz"> Busz, amire a személy felszállt. </param>
        protected virtual void Feliratkozasok(Busz busz)
        {
            busz.Megallt += Busz_Megallt;
            busz.Utazas.Ido.ElteltEgyPerc += Ido_ElteltEgyPerc;
            busz.ElerteAVegallomast += Busz_ElerteAVegallomast;
        }
        /// <summary>
        /// Ido.ElteltEgyPerc eseményre iratkozik fel.
        /// <para> Percenként csökkenti a számlálót. </para>
        /// </summary>
        /// <param name="ido">Busz ideje.</param>
        protected virtual void Ido_ElteltEgyPerc(Ido ido)
        {
            Szamlalo--;
        }
        /// <summary>
        /// Következő eseményekről iratkozik le azonos nevű metódusaival:
        /// <para> Busz.Megallt. </para>
        /// <para> Busz.Utazas.Ido.EltelEgyPerc </para>
        /// <para> Busz.ElerteAVegallomast </para>
        /// </summary>
        /// <param name="busz">Busz, amiről a személy leszállt.</param>
        protected virtual void Leiratkozasok(Busz busz)
        {
            busz.Megallt -= Busz_Megallt;
            busz.Utazas.Ido.ElteltEgyPerc -= Ido_ElteltEgyPerc;
            busz.ElerteAVegallomast += Busz_ElerteAVegallomast;
        }

        /// <summary>
        /// Generál n darab utast kettő sofőrrel.
        /// </summary>
        /// <param name="random">Random, amivel a személyeket generálja.</param>
        /// <param name="utasDarab">Utasok száma.</param>
        /// <returns>Generált személyek láncoltlistája.</returns>
        public static LancoltLista<Szemely> General(Random random, int utasDarab)
        {
            LancoltLista<Szemely> szemelyek = new LancoltLista<Szemely>();
            for (int i = 0; i < Sofor.SoforDarab; i++)
            {
                szemelyek.Hozzaad(new Sofor(random));
            }
            for (int i = 0; i < utasDarab; i++)
            {
                szemelyek.Hozzaad(random.Kivalaszt(peldanyosito)(random));
            }
            return szemelyek;
        }
        /// <summary>
        /// Ha nem szállt már fel egy másik buszra, akkor felszáll. Feliratkozik a busz eseményeire.
        /// </summary>
        /// <param name="busz">Busz, amire felszáll.</param>
        public void Felszall(Busz busz)
        {
            if (Busz == null)
            {
                Busz = busz;

                Feliratkozasok(busz);
            }
        }
    }
}