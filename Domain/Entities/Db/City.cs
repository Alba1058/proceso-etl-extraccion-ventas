namespace Domain.Entities.Db
{
    public class City
    {
        public int CityID { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int CountryID { get; set; }
    }
}
