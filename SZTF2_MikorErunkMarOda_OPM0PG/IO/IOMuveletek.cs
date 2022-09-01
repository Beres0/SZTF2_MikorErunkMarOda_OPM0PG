using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using SZTF2_MikorErunkMarOda_OPM0PG.Kollekciok;
using SZTF2_MikorErunkMarOda_OPM0PG.Megjelenites;
using SZTF2_MikorErunkMarOda_OPM0PG.Tervezes;

namespace SZTF2_MikorErunkMarOda_OPM0PG
{
    /// <summary>
    /// IO műveletekhez nyújt segítséget.
    /// </summary>
    internal static class IOMuveletek
    {
        /// <summary>
        /// Forrásfájl mappája.
        /// </summary>
        public readonly static string Mappa = HivofelMappaja();
        /// <summary>
        /// Project neve.
        /// </summary>
        public readonly static string ProjectNev = Assembly.GetEntryAssembly().GetName().Name;
        /// <summary>
        /// Nevek mappa neve.
        /// </summary>
        public const string Nevek = "Nevek";
        /// <summary>
        /// Debug mappa neve,
        /// </summary>
        public const string Debug = "Debug";
        /// <summary>
        /// Statisztika mappa neve.
        /// </summary>
        public const string Statisztika = "Statisztika";
        /// <summary>
        /// Elérési útat készít a forrásfájl mappáján belül.
        /// </summary>
        /// <param name="eleresiUt">Elérési út további részei.</param>
        /// <returns>Elkészített elérési út.</returns>
        public static string KeszitMappanBeluliEleresiUtat(params string[] eleresiUt)
        {
            return Mappa +(eleresiUt.Length>0?"\\" + eleresiUt.Osszefuz("\\"):"");
        }

        /// <summary>
        /// Ha a project könyvtárban van az aktuális build, akkor átmásolja a Nevek mappa tartalmát a build gyökérkönyvtárába.
        /// </summary>
        public static void LetrehozNevekMappa()
        {
                if (LetezikProjectKonyvar())
                {
                    FileInfo[] forrasok =Almappa("Nevek").GetFiles("*.txt");
                    Directory.CreateDirectory("Nevek");
                    foreach (var fajl in forrasok)
                    {
                        fajl.CopyTo($"Nevek\\{fajl.Name}", true);
                    }
                }
        }
        /// <summary>
        /// Ha a project könyvtárban van az aktuális build, akkor átmásolja a Nevek mappát a build gyökérkönytárába.
        /// Ezután összegyűjti a benne található .txt fájlokat egy hashtáblában. 
        /// Kulcs a fájl neve kiterjesztés nélkül, érték pedig a fájlok soronkénti tartalma.
        /// </summary>
        public static HashTabla<string,string[]> BetoltNevek()
        {
            LetrehozNevekMappa();
            HashTabla<string, string[]> nevek = new HashTabla<string, string[]>(50);
            try
            {
                string[] fajlok = Directory.GetFiles("Nevek", "*txt");
                foreach (var fajl in fajlok)
                {
                    nevek.Hozzaad(Path.GetFileNameWithoutExtension(fajl), File.ReadAllLines(fajl));
                }
            }
            catch(IOException)
            {

            }
            catch(KulcsKivetel<string>)
            {

            }
            return nevek;
        }
        /// <summary>
        /// IOMuvelek.cs almappáját adja vissza.
        /// </summary>
        /// <param name="eleresiUt">További része az elérési útnak.</param>
        /// <returns>Almappa DirectoryInfoja. Ha nem talált, akkor null.</returns>
        public static DirectoryInfo Almappa(params string[] eleresiUt)
        {
            return new DirectoryInfo(KeszitMappanBeluliEleresiUtat(eleresiUt));
        }
        /// <summary>
        /// Hívófél mappájának elérési útja.
        /// </summary>
        private static string HivofelMappaja([CallerFilePath] string eleresiUt = null)
        {
            return new DirectoryInfo(eleresiUt).Parent.FullName;
        }
        /// <summary>
        /// Eldönti, hogy a build a project mappájában van-e.
        /// </summary>
        public static bool LetezikProjectKonyvar()
        {
            return KeresSzuloKonyvtar(Directory.GetCurrentDirectory(), ProjectNev)!=null;
        }
        /// <summary>
        /// Megadott elérési útnak megkeresi a felmenő mappáját. 
        /// </summary>
        /// <param name="eleresiUt">Ahonnan a keresés indul.</param>
        /// <param name="keresettMappaNeve">Mappa, amit keres</param>
        /// <returns>Keresett mappa Directoryinfoja. Ha nem talált, akkor null.</returns>
        private static DirectoryInfo KeresSzuloKonyvtar(string eleresiUt, string keresettMappaNeve)
        {
            DirectoryInfo szulo = Directory.GetParent(eleresiUt);
            if (szulo == null)
            {
                return null;
            }
            else if (szulo.Name == keresettMappaNeve)
            {
                return szulo;
            }
            else return KeresSzuloKonyvtar(szulo.FullName, keresettMappaNeve);
        }
    }
}