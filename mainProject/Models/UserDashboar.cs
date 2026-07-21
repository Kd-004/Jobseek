using JobPortal.Models;
using mainProject.Models;

namespace mainProject.ViewModels
{
    public class UserDashboard
    {
        public int AppliedJobs { get; set; }

        public int SavedJobs { get; set; }

        public int InterviewCalls { get; set; }

        public int ProfileCompletion { get; set; }

        public List<Job> RecommendedJobs { get; set; } = new();

        public List<JobApplication> RecentApplications { get; set; } = new();
    }
}