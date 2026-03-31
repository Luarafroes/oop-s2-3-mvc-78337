using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Entities;
using VgcCollege.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Admin Dashboard
        public async Task<IActionResult> Index()
        {
            var branchCount = await _context.Branches.CountAsync();
            var courseCount = await _context.Courses.CountAsync();
            var studentCount = await _context.StudentProfiles.CountAsync();
            var facultyCount = await _context.FacultyProfiles.CountAsync();

            ViewBag.BranchCount = branchCount;
            ViewBag.CourseCount = courseCount;
            ViewBag.StudentCount = studentCount;
            ViewBag.FacultyCount = facultyCount;

            return View();
        }

        // ==================== BRANCHES ====================
        public async Task<IActionResult> Branches()
        {
            var branches = await _context.Branches
                .Include(b => b.Courses)
                .ToListAsync();
            return View(branches);
        }

        public IActionResult CreateBranch()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBranch(Branch branch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Branches.Add(branch);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Branch created: {BranchName} by {User}", branch.Name, User.Identity?.Name);
                    return RedirectToAction(nameof(Branches));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating branch {BranchName}", branch.Name);
                    ModelState.AddModelError("", "An error occurred while creating the branch.");
                }
            }
            return View(branch);
        }

        public async Task<IActionResult> EditBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null) return NotFound();
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBranch(int id, Branch branch)
        {
            if (id != branch.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Branch updated: {BranchName} by {User}", branch.Name, User.Identity?.Name);
                    return RedirectToAction(nameof(Branches));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating branch {BranchId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the branch.");
                }
            }
            return View(branch);
        }

        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null) return NotFound();
            return View(branch);
        }

        [HttpPost, ActionName("DeleteBranch")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranchConfirmed(int id)
        {
            try
            {
                var branch = await _context.Branches.FindAsync(id);
                if (branch != null)
                {
                    _context.Branches.Remove(branch);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Branch deleted: {BranchName} by {User}", branch.Name, User.Identity?.Name);
                }
                return RedirectToAction(nameof(Branches));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting branch {BranchId}", id);
                TempData["Error"] = "Cannot delete branch that has courses. Remove courses first.";
                return RedirectToAction(nameof(Branches));
            }
        }

        // ==================== COURSES ====================
        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.Enrolments)
                .ToListAsync();
            return View(courses);
        }

        public IActionResult CreateCourse()
        {
            ViewBag.Branches = new SelectList(_context.Branches, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Course created: {CourseName} by {User}", course.Name, User.Identity?.Name);
                    return RedirectToAction(nameof(Courses));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating course {CourseName}", course.Name);
                    ModelState.AddModelError("", "An error occurred while creating the course.");
                }
            }
            ViewBag.Branches = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            return View(course);
        }

        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            ViewBag.Branches = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Course updated: {CourseName} by {User}", course.Name, User.Identity?.Name);
                    return RedirectToAction(nameof(Courses));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating course {CourseId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the course.");
                }
            }
            ViewBag.Branches = new SelectList(_context.Branches, "Id", "Name", course.BranchId);
            return View(course);
        }

        // ==================== STUDENTS ====================
        public async Task<IActionResult> Students()
        {
            var students = await _context.StudentProfiles.ToListAsync();
            return View(students);
        }

        public IActionResult CreateStudent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(StudentProfile student, string password, string email)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create Identity user first
                    var user = new IdentityUser { UserName = email, Email = email };
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Student");
                        student.IdentityUserId = user.Id;
                        student.Email = email;
                        _context.StudentProfiles.Add(student);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Student created: {StudentName} ({StudentNumber}) by {User}",
                            student.Name, student.StudentNumber, User.Identity?.Name);
                        return RedirectToAction(nameof(Students));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating student {StudentName}", student.Name);
                    ModelState.AddModelError("", "An error occurred while creating the student.");
                }
            }
            return View(student);
        }

        // ==================== FACULTY ====================
        public async Task<IActionResult> Faculty()
        {
            var faculty = await _context.FacultyProfiles
                .Include(f => f.CourseAssignments)
                .ThenInclude(fca => fca.Course)
                .ToListAsync();
            return View(faculty);
        }

        public IActionResult CreateFaculty()
        {
            ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFaculty(FacultyProfile faculty, string password, string email, List<int>? selectedCourses)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create Identity user
                    var user = new IdentityUser { UserName = email, Email = email };
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Faculty");
                        faculty.IdentityUserId = user.Id;
                        faculty.Email = email;
                        _context.FacultyProfiles.Add(faculty);
                        await _context.SaveChangesAsync();

                        // Add course assignments
                        if (selectedCourses != null && selectedCourses.Any())
                        {
                            foreach (var courseId in selectedCourses)
                            {
                                _context.FacultyCourseAssignments.Add(new FacultyCourseAssignment
                                {
                                    FacultyProfileId = faculty.Id,
                                    CourseId = courseId
                                });
                            }
                            await _context.SaveChangesAsync();
                        }

                        _logger.LogInformation("Faculty created: {FacultyName} by {User}",
                            faculty.Name, User.Identity?.Name);
                        return RedirectToAction(nameof(Faculty));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating faculty {FacultyName}", faculty.Name);
                    ModelState.AddModelError("", "An error occurred while creating the faculty member.");
                }
            }
            ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
            return View(faculty);
        }

        // ==================== EXAM RESULTS RELEASE ====================
        public async Task<IActionResult> ExamResults()
        {
            var exams = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Results)
                .ToListAsync();
            return View(exams);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleExamRelease(int examId)
        {
            try
            {
                var exam = await _context.Exams.FindAsync(examId);
                if (exam != null)
                {
                    exam.ResultsReleased = !exam.ResultsReleased;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Exam {ExamId} results release toggled to {Status} by {User}",
                        examId, exam.ResultsReleased, User.Identity?.Name);
                }
                return RedirectToAction(nameof(ExamResults));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling exam release for {ExamId}", examId);
                TempData["Error"] = "An error occurred while toggling exam results.";
                return RedirectToAction(nameof(ExamResults));
            }
        }
    }
}