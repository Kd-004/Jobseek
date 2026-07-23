using mainProject.Models;

namespace mainProject.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int ActiveJobPostings { get; set; }
        public int TotalApplications { get; set; }
        public int ShortlistedCandidates { get; set; }
        public int RejectedCandidates { get; set; }

        public List<Job> RecentJobPostings { get; set; } = new();
        public List<JobApplication> RecentApplicants { get; set; } = new();
    }
}