using System;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Megjelenites
{
    /// <summary>
    /// Szimuláció megjelenítéséért felelős osztály.
    /// </summary>
    internal class SzimulacioMegjelenito
    {
        /// <summary>
        /// Táblázatok szegélyei.
        /// </summary>
        internal static class Szegely
        {
            public const char AlsoT = '╩';
            public const char BalAlso = '╚';
            public const char BalFelso = '╔';
            public const char BalT = '╠';
            public const char FelsoT = '╦';
            public const char Fuggoleges = '║';
            public const char Horizontalis = '═';
            public const char JobbAlso = '╝';
            public const char JobbFelso = '╗';
            public const char JobbT = '╣';
            public const char Kereszt = '╬';
        }
       /// <summary>
       /// Táblázatok oldalainak maximális eleme.
       /// </summary>
        private int maxElemEgyOldalon;

        /// <summary>
        /// StringBuilder amivel felépíti a kinézetet.
        /// </summary>
        private StringBuilder sb;

        /// <summary>
        /// Szimuláció busza.
        /// </summary>
        private Busz busz;

        /// <summary>
        /// Útvonal táblázata.
        /// </summary>
        public Tablazat<Megallo> Megallok { get; }

        /// <summary>
        /// Személyek táblázata.
        /// </summary>
        public Tablazat<Szemely> Szemelyek { get; }
        /// <summary>
        /// Szimuláció parancsai.
        /// </summary>
        HashLista<Parancs> Parancsok { get; }
        /// <summary>
        /// Elkészít egy szimuláció megjelenítőt.
        /// </summary>
        /// <param name="maxElemEgyOldalon">Táblazatok maximális eleme egy oldalon-</param>
        /// <param name="parancsok">Szimuláció parancsai.</param>
        public SzimulacioMegjelenito(int maxElemEgyOldalon,HashLista<Parancs> parancsok)
        {
            sb = new StringBuilder();
            this.maxElemEgyOldalon = maxElemEgyOldalon;
            Parancsok = parancsok;
            Megallok = new Tablazat<Megallo>("Útvonal",maxElemEgyOldalon);
            MegallokOszlopaitHozzaad();
            Szemelyek = new Tablazat<Szemely>("Személyek",maxElemEgyOldalon);
            SzemelyekOszlopaitHozzaad();
        }
        /// <summary>
        /// Betölti a buszt a megjelenítéshez.
        /// </summary>
        /// <param name="busz">Szimuláció busza.</param>
        public void Betolt(Busz busz)
        {
            this.busz = busz;
            Megallok.Tisztit();
            foreach (var megallo in busz.Utazas.Utvonal)
            {
                Megallok.Hozzaad(megallo);
            }

            Szemelyek.Tisztit();
            foreach (var szemely in busz.Utazas.Szemelyek)
            {
                Szemelyek.Hozzaad(szemely);
            }
        }

        /// <summary>
        /// Busz szegélyezett adata.
        /// <para>||{busz.adat} ... ||</para>
        /// </summary>
        /// <param name="kivalaszto">Kiválasztja a busz adatát.</param>
        /// <param name="teljesSzelesseg">Táblázat teljes szélessége</param>
        /// <returns></returns>
        private string BuszSzegelyezettAdata(Fuggveny<Busz, string> kivalaszto, int teljesSzelesseg)
        {
            return Szegely.Fuggoleges + kivalaszto(busz).Igazit(Igazitas.Bal, teljesSzelesseg) + Szegely.Fuggoleges;
        }

        /// <summary>
        /// Busz szegélyezett adatai.
        /// <para>||{busz.adat} ... ||</para>
        /// <para>||{busz.adat} ... ||</para>
        /// <para>||{busz.adat} ... ||</para>
        /// ...
        /// </summary>
        /// <param name="teljesSzelesseg">Szimuláció megjlenítésének teljes szélessége</param>
        private void BuszSzegelyezettAdatai(int teljesSzelesseg)
        {
            SorokKoztiSzegely(Megallok, Szegely.BalT, Szegely.Horizontalis);
            SorokKoztiSzegely(Szemelyek, Szegely.AlsoT, Szegely.Horizontalis, Szegely.JobbT);
            sb.AppendLine();
            sb.AppendLine(BuszSzegelyezettAdata((b) => $"Sebesség: {busz.Sebesseg}", teljesSzelesseg));
            sb.AppendLine(BuszSzegelyezettAdata((b) => $"Üzemanyag: {busz.UzemanyagSzint}", teljesSzelesseg));
            sb.AppendLine(BuszSzegelyezettAdata((b) => $"Megtett táv: {busz.OsszesMegtettTav}", teljesSzelesseg));
            sb.AppendLine(BuszSzegelyezettAdata((b) => $"Távolság a következő megállótól: {busz.TavolsagAKovetkezotol}", teljesSzelesseg));
            SorokKoztiSzegely(Megallok, Szegely.BalAlso, Szegely.Horizontalis);
            SorokKoztiSzegely(Szemelyek, Szegely.Horizontalis, Szegely.Horizontalis, Szegely.JobbAlso);
            sb.AppendLine();
        }
        /// <summary>
        /// Felépíti a parancsok leírását.
        /// </summary>
        /// <param name="teljesSzelesseg">Szimuláció megjelenítésének teljes szélessége</param>
        private void IranyitastEpit(int teljesSzelesseg)
        {
            int hossz = 0;
            string sp = new string(' ', 10);
            foreach (var parancs in Parancsok)
            {
                string leiras = parancs.Leiras + sp;
                if (hossz + leiras.Length > teljesSzelesseg)
                {
                    sb.AppendLine("\n");
                    hossz = 0;
                }
                hossz += leiras.Length;
                sb.Append(leiras);
            }
        }
        /// <summary>
        /// Hozzádja a megállók táblázathoz a következő oszlopokat:
        /// <para>"Busz": busz pozíció, 4-es szélesség</para>
        /// <para>"Érkezési idő": busz érkezési ideje a megállóhoz, 15-ös szélesség</para>
        /// <para>"Megálló": megálló típusa, 15-ös szélesség</para>
        /// </summary>
        private void MegallokOszlopaitHozzaad()
        {
            Megallok.Oszlopok.Hozzaad(new Oszlop<Megallo>("Busz")
                                     .AdatBeallit((m) => m == busz.Feletart ? "B" : " ")
                                     .SzelessegBeallit(4)
                                     .IgazitasBeallit(Igazitas.Kozep)
            );

            Megallok.Oszlopok.Hozzaad(new Oszlop<Megallo>("Érkezési idő")
                                      .AdatBeallit((m) =>
                                      busz.Utiterv.Megallasok.Keres(m, out DateTime erkezes) ?
                                      erkezes.ToString("HH:mm:ss") : "")
                                      .SzelessegBeallit(15)
                                      .IgazitasBeallit(Igazitas.Kozep)
           );
            Megallok.Oszlopok.Hozzaad(new Oszlop<Megallo>("Megálló")
                                 .AdatBeallit((m) => m.GetType().Name)
                                 .SzelessegBeallit(15)
           );
        }
        /// <summary>
        /// Megjeleníti a szimulációt
        /// </summary>
        public void Megjelenites()
        {
            Console.Clear();
            int szelesseg = Megallok.Szelesseg() + Szemelyek.Szelesseg() + 1;
            
            SzimulacioFejlecetEpit(szelesseg);
            TablazatFejleceketEpit();
            OszlopFejlecekEpit();
            TablazatRekordokatEpit();
            OldalInformaciotEpit();
            BuszSzegelyezettAdatai(szelesseg);
            IranyitastEpit(szelesseg);
            Console.WriteLine(sb.ToString());
            sb.Clear();
        }
        /// <summary>
        /// Felepiti a megállók- és személyek táblázat oldal információit.
        /// </summary>
        private void OldalInformaciotEpit()
        {
            SorokKoztiSzegely(Megallok, Szegely.BalT, Szegely.AlsoT);
            SorokKoztiSzegely(Szemelyek, Szegely.Kereszt, Szegely.AlsoT, Szegely.JobbT);
            sb.AppendLine();
            SzegelyezettTablazatInformacio(Megallok);
            sb.Remove(sb.Length - 1, 1);
            SzegelyezettTablazatInformacio(Szemelyek);
            sb.AppendLine();
        }
       /// <summary>
       /// Felépíti a megállók és a személyek táblázatának az oszlop fejléceit.
       /// </summary>
        private void OszlopFejlecekEpit()
        {
            SorokKoztiSzegely(Megallok, Szegely.BalT, Szegely.FelsoT);
            SorokKoztiSzegely(Szemelyek, Szegely.Kereszt, Szegely.FelsoT, Szegely.JobbT);
            sb.AppendLine();
            SzegelyezettOszlopFejlecek(Megallok);
            sb.Remove(sb.Length - 1, 1);
            SzegelyezettOszlopFejlecek(Szemelyek);
            sb.AppendLine();
            SorokKoztiSzegely(Megallok, Szegely.BalT, Szegely.Kereszt);
            SorokKoztiSzegely(Szemelyek, Szegely.Kereszt, Szegely.Kereszt, Szegely.JobbT);
            sb.AppendLine();
        }

        /// <summary>
        /// Sorok közti szegély.
        /// <para>
        /// {kezdő} === {elválasztó} === {elválasztó} ... =={utolsó}
        /// </para>
        /// </summary>
        /// <param name="kezdo">Szegély nyitó karaktere.</param>
        /// <param name="elvalaszto">Szegély elválasztó karaktere.</param>
        /// <param name="utolso">Szegély záró karakter.</param>
        private void SorokKoztiSzegely<T>(Tablazat<T> tablazat, char? kezdo = null, char? elvalaszto = null, char? utolso = null)
        {
            sb.Append(kezdo);
            foreach (var oszlop in tablazat.Oszlopok)
            {
                for (int i = 0; i < oszlop.Szelesseg; i++)
                {
                    sb.Append(Szegely.Horizontalis);
                }
                sb.Append(elvalaszto);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(utolso);
        }

        /// <summary>
        /// Elem adatai szegéllyel elválasztva.
        /// <para>
        /// || {adat} || {adat} || ... ||
        /// </para>
        /// </summary>
        private void SzegelyezettAdatok<T>(Tablazat<T> tablazat, T elem)
        {
            SzegelyezettSor(tablazat, (o) => o.Adat(elem));
        }

        /// <summary>
        /// Oszlopok fejlécei szegéllyel elválasztva.
        /// <para>
        /// || {fejléc} || {fejléc} || ... ||
        /// </para>
        /// </summary>
        private void SzegelyezettOszlopFejlecek<T>(Tablazat<T> tablazat)
        {
             SzegelyezettSor(tablazat, (o) => o.Fejlec());
        }
      
        /// <summary>
        /// ||{kivalaszto}||{kivalaszto}||...[oszlopok darab] ||
        /// </summary>
        private void SzegelyezettSor<T>(Tablazat<T> tablazat, Fuggveny<Oszlop<T>, string> kivalaszto)
        {
           
                sb.Append(Szegely.Fuggoleges);
                foreach (var oszlop in tablazat.Oszlopok)
                {
                    sb.Append(kivalaszto(oszlop));
                    sb.Append(Szegely.Fuggoleges);
                }
          
        }

        /// <summary>
        /// <para>Információ a táblázat elemszámáról és a jelenlegi oldal pozíciójáról.</para>
        /// <para>|| {Darab} db ... {JelenlegiOldal+1}/{OldalakDarab+1}||</para>
        /// </summary>
        /// <returns></returns>
        private void SzegelyezettTablazatInformacio<T>(Tablazat<T> tablazat)
        {
            sb.Append(Szegely.Fuggoleges);
            string darab = $" {tablazat.Darab} db";
            string oldal = $"{tablazat.JelenlegiOldal + 1}/{tablazat.OldalakDarab() + 1} ";
            sb.Append(darab.Igazit(oldal, tablazat.Szelesseg()));
            sb.Append(Szegely.Fuggoleges);
        }

        /// <summary>
        /// || ... || ... || ... ||
        /// </summary>
        /// <returns></returns>
        private void SzegelyezettUresSor<T>(Tablazat<T> tablazat)
        {
            SzegelyezettSor(tablazat, (o) => o.Ures);
        }
        /// <summary>
        /// Hozzádja a személyek táblázathoz a következő oszlopokat:
        /// <para>"Név": személyek neve, 20-as szélesség</para>
        /// <para>"Típus": személyek típusa, 15-ös szélesség</para>
        /// <para>"Számláló":személyek számlálója, 10-es szélesség</para>
        /// <para>"Állapot": személyek állapota, 20-as szélesség</para>
        /// </summary>
        private void SzemelyekOszlopaitHozzaad()
        {
            Szemelyek.Oszlopok.Hozzaad(new Oszlop<Szemely>("Név")
                                       .AdatBeallit((s) => s.Nev)
                                       .SzelessegBeallit(20)
            );

            Szemelyek.Oszlopok.Hozzaad(new Oszlop<Szemely>("Típus")
                                      .AdatBeallit((s) => s.GetType().Name)
                                      .SzelessegBeallit(15)
            );
            Szemelyek.Oszlopok.Hozzaad(new Oszlop<Szemely>("Számlaló")
                                      .AdatBeallit((s) => s.Szamlalo.ToString())
                                      .SzelessegBeallit(10)
                                      .IgazitasBeallit(Igazitas.Jobb)
            );
            Szemelyek.Oszlopok.Hozzaad(new Oszlop<Szemely>("Állapot")
                                .AdatBeallit((s) => s.Allapot)
                                .SzelessegBeallit(20)
                                .IgazitasBeallit(Igazitas.Kozep)
                              );
        }

        /// <summary>
        /// Felépíti a szimuláció fejlécét.
        /// </summary>
        /// <param name="teljesSzelesseg"></param>
        private void SzimulacioFejlecetEpit(int teljesSzelesseg)
        {
            SorokKoztiSzegely(Megallok, Szegely.BalFelso, Szegely.Horizontalis);
            SorokKoztiSzegely(Szemelyek, Szegely.Horizontalis, Szegely.Horizontalis, Szegely.JobbFelso);

            sb.AppendLine();

            string ido = busz.Utazas.Ido.PontosIdo.ToString("yyyy.MM.dd  HH:mm");
            string nev = "Mikor érünk már oda?";

            sb.AppendLine(Szegely.Fuggoleges + ido.Igazit(nev, teljesSzelesseg) + Szegely.Fuggoleges);
        }
        /// <summary>
        /// Felépíti a megállók- és személyek táblázatának fejléceit.
        /// </summary>
        private void TablazatFejleceketEpit()
        {
            SorokKoztiSzegely(Megallok, Szegely.BalT, Szegely.Horizontalis);
            SorokKoztiSzegely(Szemelyek, Szegely.FelsoT, Szegely.Horizontalis, Szegely.JobbT);
            sb.AppendLine();

            TablazatFejlec(Megallok);
            sb.Remove(sb.Length - 1, 1);
            TablazatFejlec(Szemelyek);
            sb.AppendLine();

        }
        /// <summary>
        ///  Felépíti a megállók- és személyek táblázatának rekordjait.
        /// </summary>
        private void TablazatRekordokatEpit()
        {
            int kezdM = Megallok.OldalKezdete();
            int kezdS = Szemelyek.OldalKezdete();
            int vegM = Megallok.OldalVege();
            int vegS = Szemelyek.OldalVege();
            for (int i = 0; i < maxElemEgyOldalon; i++)
            {
                if (kezdM < vegM)
                {
                    SzegelyezettAdatok(Megallok, Megallok[kezdM++]);
                }
                else
                {
                    SzegelyezettUresSor(Megallok);
                }
                sb.Remove(sb.Length - 1, 1);
                if (kezdS < vegS)
                {
                    SzegelyezettAdatok(Szemelyek, Szemelyek[kezdS++]);
                    sb.AppendLine();
                }
                else
                {
                    SzegelyezettUresSor(Szemelyek);
                    sb.AppendLine();
                }
            }
        }

        /// <summary>
        /// Táblázat fejléce.
        /// <para>
        /// ||...{táblázat neve}...||
        /// </para>
        /// </summary>
        public void TablazatFejlec<T>(Tablazat<T> tablazat)
        {
            sb.Append(Szegely.Fuggoleges);
            sb.Append(tablazat.Nev.Igazit(Igazitas.Kozep, tablazat.Szelesseg()));
            sb.Append(Szegely.Fuggoleges);
        }
    }
}