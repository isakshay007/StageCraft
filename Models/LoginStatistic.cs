namespace StageCraft.Models
{
    public class LoginStatistic
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }  // Date of the login

        public int LoginCount { get; set; }  // Number of logins on that day
    }
}
