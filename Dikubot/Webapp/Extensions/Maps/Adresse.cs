namespace Dikubot.Webapp.Extensions.Maps
{
    public class Adresse
    {
        public string type { get; set; }
        public string tekst { get; set; }
        public string forslagstekst { get; set; }
        public int caretpos { get; set; }
        public AdresseData data { get; set; }
    }
	
    public class AdresseData
    {
        public string id { get; set; }
        public int status { get; set; }
        public int darstatus { get; set; }
        public string vejkode { get; set; }
        public string vejnavn { get; set; }
        public string adresseringsvejnavn { get; set; }
        public string husnr { get; set; }
        public object etage { get; set; }
        public object dør { get; set; }
        public object supplerendebynavn { get; set; }
        public string postnr { get; set; }
        public string postnrnavn { get; set; }
        public object stormodtagerpostnr { get; set; }
        public object stormodtagerpostnrnavn { get; set; }
        public string kommunekode { get; set; }
        public string adgangsadresseid { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public string href { get; set; }
        public string navn { get; set; }
    }
}