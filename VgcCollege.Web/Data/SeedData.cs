using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Entities;

namespace VgcCollege.Web.Data
{
    public static class SeedData
    {
        public static async Task InitialiseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Seed Roles
            string[] roles = { "Admin", "Faculty", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Users
            var adminUser = await CreateUser(userManager, "admin@vgc.com", "Admin@123", "Admin");
            var facultyUser1 = await CreateUser(userManager, "faculty1@vgc.com", "Faculty@123", "Faculty");
            var facultyUser2 = await CreateUser(userManager, "faculty2@vgc.com", "Faculty@123", "Faculty");
            var studentUser1 = await CreateUser(userManager, "student1@vgc.com", "Student@123", "Student");
            var studentUser2 = await CreateUser(userManager, "student2@vgc.com", "Student@123", "Student");
            var studentUser3 = await CreateUser(userManager, "student3@vgc.com", "Student@123", "Student");

            // Seed Branches
            if (!context.Branches.Any())
            {
                var branches = new List<Branch>
                {
                    new Branch { Name = "Dublin Campus", Address = "1 O'Connell St, Dublin" },
                    new Branch { Name = "Cork Campus", Address = "12 Patrick St, Cork" },
                    new Branch { Name = "Galway Campus", Address = "5 Shop St, Galway" }
                };
                context.Branches.AddRange(branches);
                await context.SaveChangesAsync();
            }

            // Seed Courses
            if (!context.Courses.Any())
            {
                var courses = new List<Course>
                {
                    new Course { Name = "Software Development", BranchId = 1, StartDate = DateTime.Now.AddMonths(-3), EndDate = DateTime.Now.AddMonths(9) },
                    new Course { Name = "Data Science", BranchId = 1, StartDate = DateTime.Now.AddMonths(-3), EndDate = DateTime.Now.AddMonths(9) },
                    new Course { Name = "Cybersecurity", BranchId = 2, StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(10) },
                    new Course { Name = "Cloud Computing", BranchId = 3, StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(11) },
                };
                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();
            }

            // Seed Faculty Profiles
            if (!context.FacultyProfiles.Any())
            {
                var facultyProfiles = new List<FacultyProfile>
                {
                    new FacultyProfile { IdentityUserId = facultyUser1!.Id, Name = "Dr. John Murphy", Email = "faculty1@vgc.com", Phone = "0851234567" },
                    new FacultyProfile { IdentityUserId = facultyUser2!.Id, Name = "Dr. Sarah Kelly", Email = "faculty2@vgc.com", Phone = "0857654321" }
                };
                context.FacultyProfiles.AddRange(facultyProfiles);
                await context.SaveChangesAsync();
            }

            // Seed Faculty Course Assignments
            if (!context.FacultyCourseAssignments.Any())
            {
                var assignments = new List<FacultyCourseAssignment>
                {
                    new FacultyCourseAssignment { FacultyProfileId = 1, CourseId = 1 },
                    new FacultyCourseAssignment { FacultyProfileId = 1, CourseId = 2 },
                    new FacultyCourseAssignment { FacultyProfileId = 2, CourseId = 3 },
                    new FacultyCourseAssignment { FacultyProfileId = 2, CourseId = 4 },
                };
                context.FacultyCourseAssignments.AddRange(assignments);
                await context.SaveChangesAsync();
            }

            // Seed Student Profiles
            if (!context.StudentProfiles.Any())
            {
                var studentProfiles = new List<StudentProfile>
                {
                    new StudentProfile { IdentityUserId = studentUser1!.Id, Name = "Alice Ryan", Email = "student1@vgc.com", Phone = "0861234567", Address = "10 Main St, Dublin", DateOfBirth = new DateTime(2000, 5, 15), StudentNumber = "VGC001" },
                    new StudentProfile { IdentityUserId = studentUser2!.Id, Name = "Bob Walsh", Email = "student2@vgc.com", Phone = "0867654321", Address = "22 High St, Cork", DateOfBirth = new DateTime(1999, 8, 20), StudentNumber = "VGC002" },
                    new StudentProfile { IdentityUserId = studentUser3!.Id, Name = "Carol Byrne", Email = "student3@vgc.com", Phone = "0869876543", Address = "5 West St, Galway", DateOfBirth = new DateTime(2001, 3, 10), StudentNumber = "VGC003" }
                };
                context.StudentProfiles.AddRange(studentProfiles);
                await context.SaveChangesAsync();
            }

            // Seed Enrolments
            if (!context.CourseEnrolments.Any())
            {
                var enrolments = new List<CourseEnrolment>
                {
                    new CourseEnrolment { StudentProfileId = 1, CourseId = 1, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 1, CourseId = 2, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 2, CourseId = 3, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 3, CourseId = 4, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                };
                context.CourseEnrolments.AddRange(enrolments);
                await context.SaveChangesAsync();
            }

            // Seed Attendance Records
            if (!context.AttendanceRecords.Any())
            {
                var attendance = new List<AttendanceRecord>
                {
                    new AttendanceRecord { CourseEnrolmentId = 1, WeekNumber = 1, Date = DateTime.Now.AddDays(-14), Present = true },
                    new AttendanceRecord { CourseEnrolmentId = 1, WeekNumber = 2, Date = DateTime.Now.AddDays(-7), Present = true },
                    new AttendanceRecord { CourseEnrolmentId = 1, WeekNumber = 3, Date = DateTime.Now, Present = false },
                    new AttendanceRecord { CourseEnrolmentId = 2, WeekNumber = 1, Date = DateTime.Now.AddDays(-14), Present = true },
                    new AttendanceRecord { CourseEnrolmentId = 2, WeekNumber = 2, Date = DateTime.Now.AddDays(-7), Present = false },
                    new AttendanceRecord { CourseEnrolmentId = 3, WeekNumber = 1, Date = DateTime.Now.AddDays(-7), Present = true },
                    new AttendanceRecord { CourseEnrolmentId = 4, WeekNumber = 1, Date = DateTime.Now.AddDays(-7), Present = true },
                };
                context.AttendanceRecords.AddRange(attendance);
                await context.SaveChangesAsync();
            }

            // Seed Assignments
            if (!context.Assignments.Any())
            {
                var assignments = new List<Assignment>
                {
                    new Assignment { CourseId = 1, Title = "CA1 - Console App", MaxScore = 100, DueDate = DateTime.Now.AddDays(-30) },
                    new Assignment { CourseId = 1, Title = "CA2 - OOP Project", MaxScore = 100, DueDate = DateTime.Now.AddDays(-10) },
                    new Assignment { CourseId = 2, Title = "Data Analysis Report", MaxScore = 100, DueDate = DateTime.Now.AddDays(-20) },
                    new Assignment { CourseId = 3, Title = "Security Audit", MaxScore = 100, DueDate = DateTime.Now.AddDays(-15) },
                    new Assignment { CourseId = 4, Title = "Cloud Architecture Design", MaxScore = 100, DueDate = DateTime.Now.AddDays(-5) },
                };
                context.Assignments.AddRange(assignments);
                await context.SaveChangesAsync();
            }

            // Seed Assignment Results
            if (!context.AssignmentResults.Any())
            {
                var results = new List<AssignmentResult>
                {
                    new AssignmentResult { AssignmentId = 1, StudentProfileId = 1, Score = 78, Feedback = "Good work, needs more comments." },
                    new AssignmentResult { AssignmentId = 2, StudentProfileId = 1, Score = 85, Feedback = "Excellent OOP implementation." },
                    new AssignmentResult { AssignmentId = 3, StudentProfileId = 1, Score = 90, Feedback = "Outstanding analysis." },
                    new AssignmentResult { AssignmentId = 4, StudentProfileId = 2, Score = 72, Feedback = "Solid understanding shown." },
                    new AssignmentResult { AssignmentId = 5, StudentProfileId = 3, Score = 88, Feedback = "Well structured design." },
                };
                context.AssignmentResults.AddRange(results);
                await context.SaveChangesAsync();
            }

            // Seed Exams
            if (!context.Exams.Any())
            {
                var exams = new List<Exam>
                {
                    new Exam { CourseId = 1, Title = "Semester 1 Exam", Date = DateTime.Now.AddDays(-20), MaxScore = 100, ResultsReleased = true },
                    new Exam { CourseId = 2, Title = "Data Science Midterm", Date = DateTime.Now.AddDays(-15), MaxScore = 100, ResultsReleased = false },
                    new Exam { CourseId = 3, Title = "Security Fundamentals Exam", Date = DateTime.Now.AddDays(-10), MaxScore = 100, ResultsReleased = true },
                    new Exam { CourseId = 4, Title = "Cloud Platforms Exam", Date = DateTime.Now.AddDays(-5), MaxScore = 100, ResultsReleased = false },
                };
                context.Exams.AddRange(exams);
                await context.SaveChangesAsync();
            }

            // Seed Exam Results
            if (!context.ExamResults.Any())
            {
                var examResults = new List<ExamResult>
                {
                    new ExamResult { ExamId = 1, StudentProfileId = 1, Score = 82, Grade = "B" },
                    new ExamResult { ExamId = 2, StudentProfileId = 1, Score = 91, Grade = "A" },
                    new ExamResult { ExamId = 3, StudentProfileId = 2, Score = 75, Grade = "B" },
                    new ExamResult { ExamId = 4, StudentProfileId = 3, Score = 68, Grade = "C" },
                };
                context.ExamResults.AddRange(examResults);
                await context.SaveChangesAsync();
            }
        }

        private static async Task<IdentityUser?> CreateUser(
            UserManager<IdentityUser> userManager,
            string email, string password, string role)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null) return existing;

            var user = new IdentityUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                return user;
            }
            return null;
        }
    }
}