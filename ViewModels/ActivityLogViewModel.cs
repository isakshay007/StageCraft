using System;

namespace StageCraft.ViewModels
{
    public class ActivityLogViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
