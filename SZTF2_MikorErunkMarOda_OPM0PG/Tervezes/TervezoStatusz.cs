using System;
using System.Collections.Generic;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megallok;
using SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi;

namespace SZTF2_MikorErunkMarOda_OPM0PG.Tervezes
{
    /// <summary>
    /// Tervező ebben tárolja a megállónként az adatokat.
    /// </summary>
    internal class TervezoStatusz : IMegalloStatusz
    {
        /// <summary>
        /// Rekord, ami az IMegallo interface RekordOszlop attributúmmal ellátott tulajdonságainak fejlécét és lekerdéző függvényeit tárolja.
        /// </summary>
        public static Rekord<IMegalloStatusz> Rekord => MegalloStatusz.Rekord;
        /// <summary>
        /// Mennyien szállnának le, ha megállna a busz.
        /// </summary>
        private int leszallnanak;
      


        public Par<Igeny, int>[] IgenyPerDarab { get; }
        public DateTime ErkezesIdeje { get; private set; }
        /// <summary>
        /// Előző megállótól elhasznált üzemanyag.
        /// </summary>
        public float ElhasznaltUzemanyag { get; private set; }
        /// <summary>
        /// Előző megállótól utazással eltelt percek.
        /// </summary>
        public int ElteltPercUtazassal { get; private set; }
        public int Leszallok => Megall ? leszallnanak : 0;
        public bool Megall { get; set; }
        public Megallo Megallo { get; }
        public int[] SzamlalokMegalloElott { get; }
        /// <summary>
        /// Személytipusok számlálói megálló elhagyásakor.
        /// </summary>
        public IEnumerable<int> SzamlalokMegalloUtan
        {
            get
            {
                for (int i = 0; i < SzamlalokMegalloElott.Length; i++)
                {
                    yield return SzamlaloMegalloUtan(i);
                }
            }
        }
        
        public float UzemanyagSzintMegalloElott { get; private set; }
     
        public float UzemanyagSzintMegalloUtan =>Megallo==null||(Megall&&Busz.BuszIgeny.Megfelel(Megallo))?
               Busz.BuszIgeny.Ciklus : UzemanyagSzintMegalloElott;

        /// <summary>
        /// Incializálja a strázsa elemet.
        /// </summary>
        /// <param name="indulas">Utazás kezdete.</param>
        /// <param name="igenyPerDarab">Személyek darabszáma igényük szerint. </param>
        private TervezoStatusz(DateTime indulas, Par<Igeny, int>[] igenyPerDarab)
        {
            ErkezesIdeje = indulas;
            IgenyPerDarab = igenyPerDarab;
        }
        /// <summary>
        /// Elkészít egy tervező státuszt.
        /// </summary>
        /// <param name="megallo">Státuszhoz tartozó megálló</param>
        /// <param name="igenyPerDarab">Személyek darabszáma igényük szerint</param>
        public TervezoStatusz(Megallo megallo, Par<Igeny, int>[] igenyPerDarab)
        {
            Megallo = megallo;
            SzamlalokMegalloElott = new int[igenyPerDarab.Length];
            IgenyPerDarab = igenyPerDarab;
            KiszamolLeszallnanak();
            KiszamolUt();
        }
        /// <summary>
        /// Kiszámolja hány embernek felelne meg a megálló, ha megáll a busz.
        /// </summary>
        private void KiszamolLeszallnanak()
        {
            for (int i = 0; i < IgenyPerDarab.Length; i++)
            {
                if (IgenyPerDarab[i].Kulcs.Megfelel(Megallo))
                {
                    leszallnanak += IgenyPerDarab[i].Ertek;
                }
            }
        }
        /// <summary>
        /// Kiszámolja mennyi idő és üzemanyagba kerülne eljutni az előző megállótól ebbe a megállóba.
        /// </summary>
        private void KiszamolUt()
        {
            float megtettTav = Megallo.Tavolsag;
            while (megtettTav > 0)
            {
                float tav = Megallo.SebessegKorlat(megtettTav) / 60f;
                megtettTav -= tav;
                ElteltPercUtazassal++;
                ElhasznaltUzemanyag += tav / Busz.FogyasztasPerKm;
            }

        }
        /// <summary>
        /// Elkészít egy strázsaelemet. 
        /// </summary>
        /// <param name="indulas">Utazás kezdete</param>
        /// <param name="igenyPerDarab">Személyek darabszáma igényük szerint. </param>
        /// <returns>Strázsa elem.</returns>
        public static TervezoStatusz StrazsaElem(DateTime indulas, Par<Igeny, int>[] igenyPerDarab)
        {
            return new TervezoStatusz(indulas,igenyPerDarab);
        }
        /// <summary>
        /// Frissíti az előző státusz értékei alapján a számlálókat, üzemanyagszintet és az érkezési időt.
        /// </summary>
        /// <param name="elozo">Előző státusz.</param>
        public void Frissit(TervezoStatusz elozo)
        {
            UzemanyagSzintMegalloElott = elozo.UzemanyagSzintMegalloUtan - ElhasznaltUzemanyag;
            for (int i = 0; i < SzamlalokMegalloElott.Length; i++)
            {
                SzamlalokMegalloElott[i] = elozo.SzamlaloMegalloUtan(i) - ElteltPercUtazassal;
            }
            ErkezesIdeje = elozo.ErkezesIdeje +TimeSpan.FromMinutes(elozo.Leszallok+ElteltPercUtazassal);
        }
        /// <summary>
        /// Személytípus számlálója a megálló elhagyásakor.
        /// </summary>
        /// <param name="igenyTipusIndex"></param>
        /// <returns></returns>
        public int SzamlaloMegalloUtan(int szemelyTipusIndex)
        {
            return (Megallo==null||Megall&&IgenyPerDarab[szemelyTipusIndex].Kulcs.Megfelel(Megallo)? 
                IgenyPerDarab[szemelyTipusIndex].Kulcs.Ciklus : SzamlalokMegalloElott[szemelyTipusIndex]) -Leszallok;
        }

    }
}