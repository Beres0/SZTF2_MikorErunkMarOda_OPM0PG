namespace SZTF2_MikorErunkMarOda_OPM0PG
{
    /// <summary>
    /// Generikus delegált bool visszatérési értékkel.
    /// </summary>
    /// <param name="ertek"></param>
    /// <returns>Igaz-e a P tulajdonság az aktuális értékre.</returns>
    internal delegate bool Predikatum<T>(T ertek);
    /// <summary>
    /// Paraméter nélküli void delegált.
    /// </summary>
    public delegate void Akcio();
    /// <summary>
    /// Egy paraméterrel rendelkező generikus void delegált.
    /// </summary>
    public delegate void Akcio<T1>(T1 parameter);

    /// <summary>
    /// Két paraméterrel rendelkező generikus void delegált.
    /// </summary>
    public delegate void Akcio<T1, T2>(T1 p1, T2 p2);
    /// <summary>
    /// Eseménykezeléshez használt argumentum nélküli generikus void delegált.
    /// </summary>
    /// <param name="kuldo">Esemény küldője.</param>
    public delegate void EsemenyKezelo<TKuldo>(TKuldo kuldo);
    /// <summary>
    /// Eseménykezeléshez használt argumentummal rendelkező generikus void delegált.
    /// </summary>
    /// <param name="kuldo">Esemény küldője.</param>
    /// <param name="argumentum">Esemény argumentuma.</param>
    public delegate void EsemenyKezelo<TKuldo, TArgumentum>(TKuldo kuldo, TArgumentum argumentum);
    /// <summary>
    /// Paraméter nélküli generikus delegált visszatérési értékkel.
    /// </summary>
    /// <returns>Eredmény.</returns>
    public delegate TEredmeny Fuggveny<TEredmeny>();
    /// <summary>
    /// Egy paraméterrel rendelkező generikus delegált visszatérési értékkel.
    /// </summary>
    /// <returns>Eredmény.</returns>
    public delegate TEredmeny Fuggveny<T1, TEredmeny>(T1 parameter);
    /// <summary>
    /// Két paraméterrel rendelkező generikus delegált visszatérési értékkel.
    /// </summary>
    /// <returns>Eredmény.</returns>
    public delegate TEredmeny Fuggveny<T1, T2, TEredmeny>(T1 p1,T2 p2);
}