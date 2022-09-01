using System;

namespace SZTF2_MikorErunkMarOda_OPM0PG.SzimulacioResztvevoi
{
    /// <summary>
    /// Időt reprezentáló osztály, a szimuláció vezérlője.
    /// </summary>
    internal class Ido
    {
        /// <summary>
        /// Mikor kezdődött az utazás.
        /// </summary>
        public DateTime IndulasIdeje { get; private set; }
        /// <summary>
        /// Mennyi időnél tart jelenleg.
        /// </summary>
        public DateTime PontosIdo { get; private set; }

        /// <summary>
        /// Értesíti a feliratkozókat, hogy eltelt egy perc
        /// </summary>
        public event EsemenyKezelo<Ido> ElteltEgyPerc;
        /// <summary>
        /// Elkészít egy időt.
        /// </summary>
        /// <param name="indulasIdeje">Mikor kezdődött az utazás.</param>
        public Ido(DateTime indulasIdeje)
        {
            IndulasIdeje = indulasIdeje;
            PontosIdo = indulasIdeje;
        }
        /// <summary>
        /// Elkészít egy időt a jelenlegi dátummal.
        /// </summary>
        public Ido() : this(DateTime.Now)
        {
        }
        /// <summary>
        /// Elsüti az ElteltEgyPerc eseményt percszer. 
        /// </summary>
        /// <param name="perc">Hány perc teljen el.</param>
        public void Eltelt(int perc)
        {
            PontosIdo += TimeSpan.FromMinutes(perc);
            for (int i = 0; i < perc; i++)
            {
                ElteltEgyPerc?.Invoke(this);
            }
        }
    }
}