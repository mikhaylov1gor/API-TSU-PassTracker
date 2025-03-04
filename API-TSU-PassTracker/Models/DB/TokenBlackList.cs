namespace API_TSU_PassTracker.Models.DB
{
    public class TokenBlackList
    {
        public Guid id { get; set; }
        public string token { get; set; }
        public DateTime expirationTime { get; set; } = DateTime.UtcNow;
    }
}
