using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Entities;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<FacultyController> _logger;

        public FacultyController(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<FacultyController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Faculty Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);

            if (faculty == null)
            {
                _logger.LogWarning("Faculty profile not found for user {UserId}", user.Id);
                return RedirectToAction("AccessDenied", "Home");
            }

            // Get courses this faculty teaches
            var courses = await _context.FacultyCourseAssignments
                .Include(fca => fca.Course)
                .Where(fca => fca.FacultyProfileId == faculty.Id)
                .Select(fca => fca.Course)
                .ToListAsync();

            // Get total students across all their courses
            var studentIds = await _context.CourseEnrolments
                .Where(e => courses.Select(c => c.Id).Contains(e.CourseId) && e.Status == "Active")
                .Select(e => e.StudentProfileId)
                .Distinct()
                .ToListAsync();

            // Get pending assignments that need grading
            var pendingGrading = await _context.AssignmentResults
                .Include(ar => ar.Assignment)
                .Where(ar => ar.Assignment.CourseId != null &&
                            courses.Select(c => c.Id).Contains(ar.Assignment.CourseId) &&
                            ar.Score == 0 && ar.Feedback == "")
                .CountAsync();

            ViewBag.Faculty = faculty;
            ViewBag.TotalCourses = courses.Count;
            ViewBag.TotalStudents = studentIds.Count;
            ViewBag.PendingGrading = pendingGrading;

            return View();
        }

        // GET: My Students
        public async Task<IActionResult> MyStudents()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);

            if (faculty == null) return RedirectToAction("AccessDenied", "Home");

            // Get courses this faculty teaches
            var courseIds = await _context.FacultyCourseAssignments
                .Where(fca => fca.FacultyProfileId == faculty.Id)
                .Select(fca => fca.CourseId)
                .ToListAsync();

            // Get all active students enrolled in those courses
            var students = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .Where(e => courseIds.Contains(e.CourseId) && e.Status == "Active")
                .Select(e => e.StudentProfile)
                .Distinct()
                .ToListAsync();

            // Get their courses
            var studentCourses = await _context.CourseEnrolments
                .Include(e => e.Course)
                .Where(e => courseIds.Contains(e.CourseId) && e.Status == "Active")
                .GroupBy(e => e.StudentProfileId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(e => e.Course).ToList());

            ViewBag.StudentCourses = studentCourses;
            ViewBag.Faculty = faculty;

            return View(students);
        }

        // GET: Student Details (with grades)
        public async Task<IActionResult> StudentDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);

            if (faculty == null) return RedirectToAction("AccessDenied", "Home");

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            // Verify this student is enrolled in a course this faculty teaches
            var courseIds = await _context.FacultyCourseAssignments
                .Where(fca => fca.FacultyProfileId == faculty.Id)
                .Select(fca => fca.CourseId)
                .ToListAsync();

            var isInMyCourse = await _context.CourseEnrolments
                .AnyAsync(e => e.StudentProfileId == id &&
                              courseIds.Contains(e.CourseId));

            if (!isInMyCourse)
            {
                _logger.LogWarning("Faculty {FacultyId} attempted to access student {StudentId} not in their courses", faculty.Id, id);
                return RedirectToAction("AccessDenied", "Home");
            }

            // Get student's enrolments
            var enrolments = await _context.CourseEnrolments
                .Include(e => e.Course)
                .Where(e => e.StudentProfileId == id && e.Status == "Active")
                .ToListAsync();

            // Get assignment results
            var assignmentResults = await _context.AssignmentResults
                .Include(ar => ar.Assignment)
                .ThenInclude(a => a.Course)
                .Where(ar => ar.StudentProfileId == id)
                .ToListAsync();

            // Get exam results
            var examResults = await _context.ExamResults
                .Include(er => er.Exam)
                .ThenInclude(e => e.Course)
                .Where(er => er.StudentProfileId == id)
                .ToListAsync();

            // Get attendance records
            var attendance = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                .Where(a => a.CourseEnrolment.StudentProfileId == id)
                .ToListAsync();

            ViewBag.Enrolments = enrolments;
            ViewBag.AssignmentResults = assignmentResults;
            ViewBag.ExamResults = examResults;
            ViewBag.Attendance = attendance;

            return View(student);
        }

        // GET: Enter/Edit Grades
        public async Task<IActionResult> EnterGrades(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);

            if (faculty == null) return RedirectToAction("AccessDenied", "Home");

            // Verify faculty teaches this course
            var teachesCourse = await _context.FacultyCourseAssignments
                .AnyAsync(fca => fca.FacultyProfileId == faculty.Id && fca.CourseId == courseId);

            if (!teachesCourse)
            {
                _logger.LogWarning("Faculty {FacultyId} attempted to enter grades for course {CourseId} they don't teach", faculty.Id, courseId);
                return RedirectToAction("AccessDenied", "Home");
            }

            var course = await _context.Courses
                .Include(c => c.Assignments)
                .Include(c => c.Exams)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return NotFound();

            var students = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Where(e => e.CourseId == courseId && e.Status == "Active")
                .Select(e => e.StudentProfile)
                .ToListAsync();

            ViewBag.Course = course;
            ViewBag.Students = students;

            return View();
        }

        // POST: Save Assignment Grade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAssignmentGrade(int assignmentId, int studentId, int score, string feedback)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var faculty = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);

                if (faculty == null) return Unauthorized();

                var assignment = await _context.Assignments
                    .Include(a => a.Course)
                    .FirstOrDefaultAsync(a => a.Id == assignmentId);

                if (assignment == null) return NotFound();

                // Verify faculty teaches this course
                var teachesCourse = await _context.FacultyCourseAssignments
                    .AnyAsync(fca => fca.FacultyProfileId == faculty.Id && fca.CourseId == assignment.CourseId);

                if (!teachesCourse) return Unauthorized();

                var result = await _context.AssignmentResults
                    .FirstOrDefaultAsync(r => r.AssignmentId == assignmentId && r.StudentProfileId == studentId);

                if (result == null)
                {
                    result = new AssignmentResult
                    {
                        AssignmentId = assignmentId,
                        StudentProfileId = studentId,
                        Score = score,
                        Feedback = feedback ?? string.Empty
                    };
                    _context.AssignmentResults.Add(result);
                    _logger.LogInformation("Assignment grade added: Assignment {AssignmentId}, Student {StudentId}, Score {Score} by {User}",
                        assignmentId, studentId, score, User.Identity?.Name);
                }
                else
                {
                    result.Score = score;
                    result.Feedback = feedback ?? string.Empty;
                    _context.Update(result);
                    _logger.LogInformation("Assignment grade updated: Assignment {AssignmentId}, Student {StudentId}, Score {Score} by {User}",
                        assignmentId, studentId, score, User.Identity?.Name);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving assignment grade for Assignment {AssignmentId}, Student {StudentId}",
                    assignmentId, studentId);
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Save Exam Grade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveExamGrade(int examId, int studentId, int score, string grade)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                var faculty = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);

                if (faculty == null) return Unauthorized();

                var exam = await _context.Exams
                    .Include(e => e.Course)
                    .FirstOrDefaultAsync(e => e.Id == examId);

                if (exam == null) return NotFound();

                // Verify faculty teaches this course
                var teachesCourse = await _context.FacultyCourseAssignments
                    .AnyAsync(fca => fca.FacultyProfileId == faculty.Id && fca.CourseId == exam.CourseId);

                if (!teachesCourse) return Unauthorized();

                var result = await _context.ExamResults
                    .FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentProfileId == studentId);

                if (result == null)
                {
                    result = new ExamResult
                    {
                        ExamId = examId,
                        StudentProfileId = studentId,
                        Score = score,
                        Grade = grade ?? string.Empty
                    };
                    _context.ExamResults.Add(result);
                    _logger.LogInformation("Exam grade added: Exam {ExamId}, Student {StudentId}, Score {Score} by {User}",
                        examId, studentId, score, User.Identity?.Name);
                }
                else
                {
                    result.Score = score;
                    result.Grade = grade ?? string.Empty;
                    _context.Update(result);
                    _logger.LogInformation("Exam grade updated: Exam {ExamId}, Student {StudentId}, Score {Score} by {User}",
                        examId, studentId, score, User.Identity?.Name);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving exam grade for Exam {ExamId}, Student {StudentId}",
                    examId, studentId);
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Assignments list
        public async Task<IActionResult> Assignments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);

            if (faculty == null) return RedirectToAction("AccessDenied", "Home");

            var courseIds = await _context.FacultyCourseAssignments
                .Where(fca => fca.FacultyProfileId == faculty.Id)
                .Select(fca => fca.CourseId)
                .ToListAsync();

            var assignments = await _context.Assignments
                .Include(a => a.Course)
                .Where(a => courseIds.Contains(a.CourseId))
                .OrderByDescending(a => a.DueDate)
                .ToListAsync();

            return View(assignments);
        }
    }
}