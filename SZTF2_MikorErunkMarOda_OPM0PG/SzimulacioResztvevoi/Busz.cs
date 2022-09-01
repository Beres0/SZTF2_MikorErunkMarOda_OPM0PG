using System;
using System.Collections.Generic;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.Tervezes;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Egy buszt reprezentáló osztály és a szimuláció modellje.
    /// Rajta keresztül kommunikál esemény vezérelten a szimuláció összes résztvevője.
    /// </summary>
    internal class Busz : ISzamlalo
    {
        /// <summary>
        /// Kivétel, amit Busz dob bizonyos esetekben.
        /// </summary>
        public class Kivetel : Exception
        {
            /// <summary>
            /// Busz, ami a kivételt dobta.
            /// </summary>
            public Busz Busz { get; }
         
            /// <summary>
            /// Elkészít egy busz kivételt.
            /// </summary>
            /// <param name="busz">Busz, ami kiváltotta.</param>
            /// <param name="message">Busz üzenete.</param>
            public Kivetel(Busz busz,string message) : base(message)
            {
                Busz = busz;
            }
        }
        /// <summary>
        /// Segéd változó, ami segít a sofőrök cseréjében.
        /// </summary>
        private Sofor csere;

        /// <summary>
        /// Megállók enumerátora. Vele halad végig az útvonalon.
        /// </summary>
        private IEnumerator<Megallo> feletart;
        /// <summary>
        /// Fogyasztás kilométerenként: 100 km / 30 liter.
        /// </summary>
        public const float FogyasztasPerKm = 100 / 30f;

        /// <summary>
        /// Buszok igénye: Tankolásra van igénye maximum 150 literenként.
        /// </summary>
        public static readonly Igeny BuszIgeny = new Igeny(Szolgaltatas.Tankolas, 150);
        /// <summary>
        /// Az a sofőr, amelyik vezeti a buszt.
        /// </summary>
        public Sofor VezetoSofor { get; private set; }
        /// <summary>
        /// Szöveges információ az állapotáról.
        /// <para> Ha a sebessége nagyobb, mint 0: "Halad".</para>
        /// <para> Máskülönben: "Áll".</para>
        /// </summary>
        public string Allapot => Sebesseg > 0 ? "Halad" : "Áll";
      
        /// <summary>
        /// Elért megállók száma.
        /// </summary>
        public int ElertMegallok { get; private set; }
        /// <summary>
        /// Megálló, ami felé tart éppen.
        /// </summary>
        public Megallo Feletart => feletart.Current;
        public Igeny Igeny => BuszIgeny;
        /// <summary>
        /// Utazás során megtett táv.
        /// </summary>
        public float OsszesMegtettTav { get; private set; }
        /// <summary>
        /// Busz nem vezető sofőre.
        /// </summary>
        public Sofor KeszenletiSofor { get; private set; }
        /// <summary>
        /// Busz sebessége km/órában.
        /// </summary>
        public int Sebesseg { get; private set; }
        /// <summary>
        /// Aktuális távolsága a következő megállótól.
        /// </summary>
        public float TavolsagAKovetkezotol { get; private set; }
        /// <summary>
        /// Szimulált utazás.
        /// </summary>
        public Utazas Utazas { get; }
        /// <summary>
        /// Sofőrök útiterve.
        /// </summary>
        public Utiterv Utiterv { get; private set; }
       /// <summary>
       /// Üzemanyag szintje, ami egyben a számlálója is.
       /// </summary>
        public float UzemanyagSzint { get; private set; }
        float ISzamlalo.Szamlalo => UzemanyagSzint;

        /// <summary>
        /// Esemény, ami akkor váltódik ki, ha a busz elérte a megállót.
        /// </summary>
        public event EsemenyKezelo<Busz, Megallo> ElerteAMegallot;

        /// <summary>
        /// Esemény, ami akkor váltódik ki, ha a busz elérte a végállomást.
        /// </summary>
        public event EsemenyKezelo<Busz> ElerteAVegallomast;
        /// <summary>
        /// Esemény, ami akkor váltódik ki, ha a busz megállt a megállónál.
        /// </summary>
        public event EsemenyKezelo<Busz, Megallo> Megallt;
        /// <summary>
        /// Esemény, ami akkor váltódik ki, ha egy személy leszállt a buszról valamelyik megállónál.
        /// </summary>
        public event EsemenyKezelo<Busz, Szemely> SzemelyIdeiglenesenLeszallt;
        /// <summary>
        /// Elkészíti a buszt. 
        /// <para>Beállítja az útvonal első megállóját és ellenőrzi a személylistát. Ha kettőnél több sofőrt talál, Busz.Kivetelt dob.</para>
        /// </summary>
        /// <param name="utazas">Szimulálni kívánt utazás.</param>
        public Busz(Utazas utazas)
        {
            Utazas = utazas;
            feletart = utazas.Utvonal.GetEnumerator();
            feletart.MoveNext();
            TavolsagAKovetkezotol = Feletart.Tavolsag;

            Utazas.Ido.ElteltEgyPerc += Ido_ElteltEgyPerc;
            UzemanyagSzint = BuszIgeny.Ciklus;
            Ellenoriz();
            SzemelyekFelszallnak();
        }
        /// <summary>
        /// Elhagyja a megállót. Feletart következőjére lép, ha van még megálló az útvonalon.
        /// </summary>
        private void ElhagytaAMegallot()
        {
            if (feletart.MoveNext())
            {
                TavolsagAKovetkezotol = Feletart.Tavolsag;
            }
        }
        /// <summary>
        /// Kikeresi utazási személyei közül a sofőrök csoportját. Ha több van belőlük, mint 2, akkor Busz.Kivételt.Dob.
        /// </summary>
        private void Ellenoriz()
        {
            if (Utazas.Szemelyek.KeresCsoport(Sofor.SoforIgeny, out OlvashatoLancoltLista<Szemely> soforok))
            {
                if (soforok.Darab != Sofor.SoforDarab)
                {
                    throw new Kivetel(this, $"{Sofor.SoforDarab} soförnek kell lennie a buszon! Jelenleg ennyi van: {soforok.Darab}");
                }
            }
        }
        /// <summary>
        /// Elérte a megállót. 
        /// <para> Ha a sebesség 0, akkor megáll és megpróbál tankolni, ha lehetséges. 
        /// Ha az utolsó megállónál állt meg, értesíti a feliratkozókat</para>
        /// </summary>
        /// <param name="megallo"></param>
        private void Elerte(Megallo megallo)
        {
            ElerteAMegallot?.Invoke(this, Feletart);
            ElertMegallok++;
            
            if (Sebesseg == 0)
            {
                TavolsagAKovetkezotol = 0;
                if (Igeny.Megfelel(megallo))
                {
                    UzemanyagSzint = Igeny.Ciklus;
                }
                Megallt?.Invoke(this, megallo);


                if (Utazas.Utvonal.Utolso == Feletart)
                {
                    UtazasVegereErt();
                }
            }
        }
        /// <summary>
        /// Elérte az utolsó megállót. Értesíti a feliratkozókat és leiratkozik a személyek eseményeiről.
        /// </summary>
        private void UtazasVegereErt()
        {
            ElerteAVegallomast?.Invoke(this);
            foreach (var szemely in Utazas.Szemelyek)
            {
                szemely.IdeiglenesenLeszallt -= Szemely_IdeiglenesenLeszallt;
                if (szemely is Sofor sofor)
                {
                    sofor.SebessegetValtoztat -= Sofor_SebessegetValtoztat;
                }
            }
        }
        /// <summary>
        /// Busz halad. 
        /// <para>Kiszámolja, mekkora a sebessége km/percben és eszerint növeli a megtett távot, 
        /// illetve csökkenti az üzemanyagot és a következő megállótól a távolságot.</para>
        /// </summary>
        private void Halad()
        {
            float sebessegPercben = Sebesseg / 60f;
            UzemanyagSzint -= sebessegPercben / FogyasztasPerKm;
            TavolsagAKovetkezotol -= sebessegPercben;
            OsszesMegtettTav += sebessegPercben;
        }
        /// <summary>
        /// Ido.ElteltEgyPerc eseményre iratkozik fel.
        /// <para> Ha kifogyott az üzemanyagból busz kivételt dob.</para>
        /// <para> Ha van elég üzemanyag és nem áll a busz, tovább halad az úton.
        /// Értesíti a feliratkozókat, ha elérte a megállót. </para>
        /// <para>Ha érkezés után is halad a busz, akkor elhagyja és beállítja a következőt.</para>
        /// </summary>
        /// <param name="ido">Idő, ami kiváltotta.</param>
        private void Ido_ElteltEgyPerc(Ido ido)
        {
            if(UzemanyagSzint<=0)
            {
                throw new Kivetel(this, "Elfogyott az üzemanyag!");
            }
            else if (Sebesseg > 0)
            {
                Halad();
                if (TavolsagAKovetkezotol <= 0)
                {
                    Elerte(Feletart);
                    if (Sebesseg > 0)
                    {
                        ElhagytaAMegallot();
                    }
                }
            }
        }
        /// <summary>
        /// Sofor.SebessegetValtoztat eseményre iratkozik fel.
        /// <para> Ha a küldött sebesség nagyobb egyelnő nullával beállítja az értékét.</para>
        /// <para>Ha a busz egy megállónál állt és a sebességváltoztás hatására elindult, akkor elhagyja a megállót.</para>
        /// </summary>
        /// <param name="kuldo"></param>
        /// <param name="argumentum"></param>
        private void Sofor_SebessegetValtoztat(Sofor kuldo, int argumentum)
        {
            if (argumentum >= 0)
            {
                if (Sebesseg == 0 && argumentum > 0 && TavolsagAKovetkezotol == 0)
                {
                    ElhagytaAMegallot();
                }
                Sebesseg = argumentum;
            }
        }
        /// <summary>
        /// Megcseréli a vezető és a készenléti sofőrt, ha még nem történt meg.
        /// </summary>
        private void SofortCserel(Sofor sofor)
        {
            if (csere == null)
            {
                csere = sofor;
            }
            else
            {
                if (sofor == KeszenletiSofor)
                {
                    VezetoSofor = sofor;
                    KeszenletiSofor = csere;
                }
                else
                {
                    KeszenletiSofor = sofor;
                    VezetoSofor = csere;
                }
                csere = null;
            }
        }
        /// <summary>
        /// Szemely.IdeiglenesenLeszallt eseményre iratkozik fel.
        /// <para>Továbbítja saját eseményként a feliratkozóinak.</para>
        /// <para>Ha a személy sofőr, megcseréli a társával.</para>
        /// </summary>
        /// <param name="kuldo"></param>
        /// <param name="argumentum"></param>
        private void Szemely_IdeiglenesenLeszallt(Szemely kuldo)
        {
            if (kuldo is Sofor sofor)
            {
                SofortCserel(sofor);
            }

            SzemelyIdeiglenesenLeszallt?.Invoke(this, kuldo);
        }
        /// <summary>
        /// Felszállnak a személyek a buszra.
        /// <para>
        /// Feliratkozik a személyek IdeigleneseLeszallt eseményére. 
        /// Ha a személy sofőr, feliratkozik a SebessegetValtoztat eseményére is és ha még nem foglalt a vezető pozíció,
        /// akkor beállítja őt és elkéri tőle az útitervet. 
        /// <para>Végül megkéri a személyeket, hogy ők is végezzék el a kapcsolatfelvételhez szükséges dolgaikat.</para>
        /// </para>
        /// </summary>
        private void SzemelyekFelszallnak()
        {
            foreach (var szemely in Utazas.Szemelyek)
            {
                szemely.IdeiglenesenLeszallt += Szemely_IdeiglenesenLeszallt;

                if (szemely is Sofor sofor)
                {
                    sofor.SebessegetValtoztat += Sofor_SebessegetValtoztat;
                    if (VezetoSofor == null)
                    {
                        VezetoSofor = sofor;
                        Utiterv = sofor.KeszitUtiterv(Utazas);
                    }
                    else if (KeszenletiSofor == null)
                    {
                        KeszenletiSofor = sofor;
                    }
                }
                szemely.Felszall(this);
            }
        }
    }
}