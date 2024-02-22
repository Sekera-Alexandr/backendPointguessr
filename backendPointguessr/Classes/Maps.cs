namespace backendPointguessr.Classes
{
    /// <summary>
    /// Třída pro záznamy map
    /// </summary>
    public class Maps
    {
        public int PlaceID { get; set; }

        public string? Jmeno { get; set; }

        public string? ImageMapa { get; set; }

        public string? ImageMisto { get; set; }

        public int PoziceX { get; set; }

        public int PoziceY { get; set; }

        public int UserID { get; set; }
    }
}
