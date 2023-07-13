namespace Skynet.Domain.Cheaters
{
    public class CheaterListWrapper
    {
        public DateTime LastUpdated { get; set; }
        public string UpdatedBy { get; set; }
        public int Lenght
        {
            get
            {
                return cheaters.Count;
            }
        }
        public List<Cheater> cheaters { get; set; }
        public CheaterListWrapper()
        {

        }
    }
}
