//using Microsoft.AspNetCore.Mvc;

//namespace mainProject.Controllers
//{
//    public class DetailsController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//        [HttpGet]
//        public async Task<IActionResult> Details(int jobId)
//        {

//            get by jobid
//            var jobsQuery = _context.Jobs
//              I
//                .AsQueryable();


//            var jobs = await jobsQuery.OrderByDescending(j => j.PostedDate)
//           .GroupJoin(
//          _context.Companies,
//          job => job.CompanyId,
//          company => company.Id.ToString(),
//          (job, companies) => new { job, companies }
//      )
//      .SelectMany(
//          x => x.companies.DefaultIfEmpty(),
//          (x, company) => new JobWithCompany
//          {
//              Job = x.job,
//              CompanyName = company != null
//                  ? company.CompanyName
//                  : "Company not listed"
//          })
//      .ToListAsync();
//            return View();
//        }
//        //POST: /ApplyJob/ApplyJob
//        //  Records that the logged-in user applied to a specific job

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize]
//        public async Task<IActionResult> ApplyJob(int jobId)
//        {
//            var job = await _context.Jobs.FindAsync(jobId);
//            if (job == null)
//            {
//                TempData["Error"] = "This job no longer exists.";
//                return RedirectToAction(nameof(Apply));
//            }

//            var userId = _userManager.GetUserId(User);

//            bool alreadyApplied = await _context.JobApplications
//                .AnyAsync(a => a.JobId == jobId && a.UserId == userId);

//            if (alreadyApplied)
//            {
//                TempData["Error"] = "You have already applied to this job.";
//                return RedirectToAction(nameof(Apply));
//            }

//            var application = new JobApplication
//            {
//                JobId = jobId,
//                UserId = userId,
//                AppliedDate = System.DateTime.Now,
//                Status = "Applied"
//            };

//        }
//    }
//}
