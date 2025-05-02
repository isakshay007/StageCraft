namespace StageCraft.ViewModels
{
    public class LoginStatisticsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalManagers { get; set; }
        //  For bar chart data
        public List<string> RoleNames { get; set; } = new List<string> { "Admins", "Production Managers", "Users" };
        public List<int> RoleCounts { get; set; } = new();
    }
}