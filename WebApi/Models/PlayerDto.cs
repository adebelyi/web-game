using WebGame.Domain;

namespace WebApi.Models
{
    public class PlayerDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public PlayerDecision Decision { get; set; }
        public int Score { get; set; }
    }
}
