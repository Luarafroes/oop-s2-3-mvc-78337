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

            // Seed Admin User
            var adminUser = await CreateUser(userManager, "admin@vgc.com", "Admin@123", "Admin");

            // Seed Faculty Users
            var facultyUser1 = await CreateUser(userManager, "wenhao.fu@vgc.com", "Faculty@123", "Faculty");
            var facultyUser2 = await CreateUser(userManager, "rod.haanappel@vgc.com", "Faculty@123", "Faculty");
            var facultyUser3 = await CreateUser(userManager, "john.rowley@vgc.com", "Faculty@123", "Faculty");

            // Seed Student Users
            var studentUser1 = await CreateUser(userManager, "luara@vgc.com", "Student@123", "Student");
            var studentUser2 = await CreateUser(userManager, "student2@vgc.com", "Student@123", "Student");
            var studentUser3 = await CreateUser(userManager, "helder@vgc.com", "Student@123", "Student");
            var studentUser4 = await CreateUser(userManager, "wesley.oliveira@vgc.com", "Student@123", "Student");
            var studentUser5 = await CreateUser(userManager, "larissa.sousa@vgc.com", "Student@123", "Student");
            var studentUser6 = await CreateUser(userManager, "carlos.camargo@vgc.com", "Student@123", "Student");
            var studentUser7 = await CreateUser(userManager, "manar.ahkim@vgc.com", "Student@123", "Student");
            var studentUser8 = await CreateUser(userManager, "caio.pereira@vgc.com", "Student@123", "Student");

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
                    new Course { Name = "Programming Fundamentals", BranchId = 1, StartDate = DateTime.Now.AddMonths(-3), EndDate = DateTime.Now.AddMonths(9) },
                    new Course { Name = "Database Systems", BranchId = 1, StartDate = DateTime.Now.AddMonths(-3), EndDate = DateTime.Now.AddMonths(9) },
                    new Course { Name = "Operating Systems", BranchId = 2, StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(10) },
                    new Course { Name = "Artificial Intelligence", BranchId = 2, StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(10) },
                    new Course { Name = "Cloud Computing", BranchId = 3, StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(11) },
                    new Course { Name = "Cybersecurity", BranchId = 3, StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(11) },
                };
                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();
            }

            // Seed Faculty Profiles
            if (!context.FacultyProfiles.Any())
            {
                var facultyProfiles = new List<FacultyProfile>
                {
                    new FacultyProfile { IdentityUserId = facultyUser1!.Id, Name = "Dr. Wenhao Fu", Email = "wenhao.fu@vgc.com", Phone = "0851112233" },
                    new FacultyProfile { IdentityUserId = facultyUser2!.Id, Name = "Dr. Rod Haanappel", Email = "rod.haanappel@vgc.com", Phone = "0854445566" },
                    new FacultyProfile { IdentityUserId = facultyUser3!.Id, Name = "Dr. John Rowley", Email = "john.rowley@vgc.com", Phone = "0857778899" }
                };
                context.FacultyProfiles.AddRange(facultyProfiles);
                await context.SaveChangesAsync();
            }

            // Seed Faculty Course Assignments
            if (!context.FacultyCourseAssignments.Any())
            {
                var assignments = new List<FacultyCourseAssignment>
                {
                    // Dr. John Rowley teaches Programming and Database
                    new FacultyCourseAssignment { FacultyProfileId = 1, CourseId = 1 }, // Programming
                    new FacultyCourseAssignment { FacultyProfileId = 1, CourseId = 2 }, // Database
                    
                    // Dr. Rod Haanappel teaches Operating Systems and AI
                    new FacultyCourseAssignment { FacultyProfileId = 2, CourseId = 3 }, // Operating Systems
                    new FacultyCourseAssignment { FacultyProfileId = 2, CourseId = 4 }, // AI
                    
                    // Dr. Wenhao Fu teaches Cloud and Cybersecurity
                    new FacultyCourseAssignment { FacultyProfileId = 3, CourseId = 5 }, // Cloud Computing
                    new FacultyCourseAssignment { FacultyProfileId = 3, CourseId = 6 }, // Cybersecurity
                };
                context.FacultyCourseAssignments.AddRange(assignments);
                await context.SaveChangesAsync();
            }

            // Seed Student Profiles
            if (!context.StudentProfiles.Any())
            {
                var studentProfiles = new List<StudentProfile>
                {
                    new StudentProfile { IdentityUserId = studentUser1!.Id, Name = "Luara Froes", Email = "luara@vgc.com", Phone = "0861234567", Address = "10 Main St, Dublin", DateOfBirth = new DateTime(2000, 11, 5), StudentNumber = "VGC001" },
                    new StudentProfile { IdentityUserId = studentUser2!.Id, Name = "Bob Walsh", Email = "student2@vgc.com", Phone = "0867654321", Address = "22 High St, Cork", DateOfBirth = new DateTime(1999, 8, 20), StudentNumber = "VGC002" },
                    new StudentProfile { IdentityUserId = studentUser3!.Id, Name = "Helder Oliveira", Email = "helder@vgc.com", Phone = "0869876543", Address = "5 West St, Galway", DateOfBirth = new DateTime(2000, 3, 10), StudentNumber = "VGC003" },
                    new StudentProfile { IdentityUserId = studentUser4!.Id, Name = "Wesley Oliveira", Email = "wesley.oliveira@vgc.com", Phone = "0861122334", Address = "15 Park Ave, Dublin", DateOfBirth = new DateTime(2001, 6, 15), StudentNumber = "VGC004" },
                    new StudentProfile { IdentityUserId = studentUser5!.Id, Name = "Larissa Sousa", Email = "larissa.sousa@vgc.com", Phone = "0865566778", Address = "8 Church St, Cork", DateOfBirth = new DateTime(2002, 2, 22), StudentNumber = "VGC005" },
                    new StudentProfile { IdentityUserId = studentUser6!.Id, Name = "Carlos Camargo", Email = "carlos.camargo@vgc.com", Phone = "0869988776", Address = "42 Bridge St, Galway", DateOfBirth = new DateTime(2000, 9, 8), StudentNumber = "VGC006" },
                    new StudentProfile { IdentityUserId = studentUser7!.Id, Name = "Manar Ahkim", Email = "manar.ahkim@vgc.com", Phone = "0864433221", Address = "7 College Rd, Dublin", DateOfBirth = new DateTime(2001, 12, 3), StudentNumber = "VGC007" },
                    new StudentProfile { IdentityUserId = studentUser8!.Id, Name = "Caio Pereira", Email = "caio.pereira@vgc.com", Phone = "0868877665", Address = "23 Harbour St, Cork", DateOfBirth = new DateTime(2002, 5, 17), StudentNumber = "VGC008" }
                };
                context.StudentProfiles.AddRange(studentProfiles);
                await context.SaveChangesAsync();
            }

            // Seed Enrolments
            if (!context.CourseEnrolments.Any())
            {
                var enrolments = new List<CourseEnrolment>
                {
                    // Luara (ID 1) - Programming and Database
                    new CourseEnrolment { StudentProfileId = 1, CourseId = 1, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 1, CourseId = 2, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    
                    // Bob (ID 2) - Operating Systems and AI
                    new CourseEnrolment { StudentProfileId = 2, CourseId = 3, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 2, CourseId = 4, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    
                    // Helder (ID 3) - Cloud and Cybersecurity
                    new CourseEnrolment { StudentProfileId = 3, CourseId = 5, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 3, CourseId = 6, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    
                    // Wesley (ID 4) - Programming and AI
                    new CourseEnrolment { StudentProfileId = 4, CourseId = 1, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 4, CourseId = 4, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    
                    // Larissa (ID 5) - Database and Cloud
                    new CourseEnrolment { StudentProfileId = 5, CourseId = 2, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 5, CourseId = 5, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    
                    // Carlos (ID 6) - OS and Cybersecurity
                    new CourseEnrolment { StudentProfileId = 6, CourseId = 3, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 6, CourseId = 6, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    
                    // Manar (ID 7) - Programming, Database, AI
                    new CourseEnrolment { StudentProfileId = 7, CourseId = 1, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 7, CourseId = 2, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 7, CourseId = 4, EnrolDate = DateTime.Now.AddMonths(-2), Status = "Active" },
                    
                    // Caio (ID 8) - OS and Cloud
                    new CourseEnrolment { StudentProfileId = 8, CourseId = 3, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                    new CourseEnrolment { StudentProfileId = 8, CourseId = 5, EnrolDate = DateTime.Now.AddMonths(-1), Status = "Active" },
                };
                context.CourseEnrolments.AddRange(enrolments);
                await context.SaveChangesAsync();
            }

            // Seed Attendance Records
            if (!context.AttendanceRecords.Any())
            {
                var attendance = new List<AttendanceRecord>();
                var random = new Random();

                // Generate attendance for all enrolments (1-6 weeks)
                for (int enrolmentId = 1; enrolmentId <= 16; enrolmentId++)
                {
                    for (int week = 1; week <= 6; week++)
                    {
                        attendance.Add(new AttendanceRecord
                        {
                            CourseEnrolmentId = enrolmentId,
                            WeekNumber = week,
                            Date = DateTime.Now.AddDays(-(42 - (week * 7))),
                            Present = random.Next(0, 10) > 2 // 70-80% attendance
                        });
                    }
                }
                context.AttendanceRecords.AddRange(attendance);
                await context.SaveChangesAsync();
            }

            // Seed Assignments
            if (!context.Assignments.Any())
            {
                var assignments = new List<Assignment>
                {
                    // Programming assignments
                    new Assignment { CourseId = 1, Title = "Console App Project", MaxScore = 100, DueDate = DateTime.Now.AddDays(-30) },
                    new Assignment { CourseId = 1, Title = "OOP Final Project", MaxScore = 100, DueDate = DateTime.Now.AddDays(-10) },
                    
                    // Database assignments
                    new Assignment { CourseId = 2, Title = "SQL Query Assignment", MaxScore = 100, DueDate = DateTime.Now.AddDays(-25) },
                    new Assignment { CourseId = 2, Title = "Database Design Project", MaxScore = 100, DueDate = DateTime.Now.AddDays(-5) },
                    
                    // OS assignments
                    new Assignment { CourseId = 3, Title = "Process Scheduling Simulator", MaxScore = 100, DueDate = DateTime.Now.AddDays(-20) },
                    new Assignment { CourseId = 3, Title = "Memory Management Report", MaxScore = 100, DueDate = DateTime.Now.AddDays(-8) },
                    
                    // AI assignments
                    new Assignment { CourseId = 4, Title = "Search Algorithms Implementation", MaxScore = 100, DueDate = DateTime.Now.AddDays(-15) },
                    new Assignment { CourseId = 4, Title = "Neural Network Project", MaxScore = 100, DueDate = DateTime.Now.AddDays(-3) },
                    
                    // Cloud assignments
                    new Assignment { CourseId = 5, Title = "AWS Architecture Design", MaxScore = 100, DueDate = DateTime.Now.AddDays(-18) },
                    new Assignment { CourseId = 5, Title = "Serverless Application", MaxScore = 100, DueDate = DateTime.Now.AddDays(-2) },
                    
                    // Cybersecurity assignments
                    new Assignment { CourseId = 6, Title = "Security Audit Report", MaxScore = 100, DueDate = DateTime.Now.AddDays(-12) },
                    new Assignment { CourseId = 6, Title = "Penetration Testing Lab", MaxScore = 100, DueDate = DateTime.Now.AddDays(-1) },
                };
                context.Assignments.AddRange(assignments);
                await context.SaveChangesAsync();
            }

            // Seed Assignment Results
            if (!context.AssignmentResults.Any())
            {
                var results = new List<AssignmentResult>
                {
                    // Luara (ID 1) - Good grades
                    new AssignmentResult { AssignmentId = 1, StudentProfileId = 1, Score = 92, Feedback = "Excellent work! Great understanding of C# fundamentals." },
                    new AssignmentResult { AssignmentId = 2, StudentProfileId = 1, Score = 88, Feedback = "Very solid OOP design. Consider adding more comments." },
                    new AssignmentResult { AssignmentId = 3, StudentProfileId = 1, Score = 95, Feedback = "Outstanding SQL queries. Perfect optimization!" },
                    
                    // Bob (ID 2) - Average grades
                    new AssignmentResult { AssignmentId = 5, StudentProfileId = 2, Score = 72, Feedback = "Good effort, but process scheduling needs improvement." },
                    new AssignmentResult { AssignmentId = 7, StudentProfileId = 2, Score = 68, Feedback = "Search algorithms implemented, but optimization needed." },
                    
                    // Helder (ID 3) - Mixed grades
                    new AssignmentResult { AssignmentId = 9, StudentProfileId = 3, Score = 85, Feedback = "Well designed AWS architecture." },
                    new AssignmentResult { AssignmentId = 11, StudentProfileId = 3, Score = 45, Feedback = "Security audit lacks depth. Please review the OWASP guidelines." },
                    
                    // Wesley (ID 4) - Excellent grades
                    new AssignmentResult { AssignmentId = 1, StudentProfileId = 4, Score = 96, Feedback = "Exceptional! Best in class." },
                    new AssignmentResult { AssignmentId = 8, StudentProfileId = 4, Score = 94, Feedback = "Neural network implementation is impressive!" },
                    
                    // Larissa (ID 5) - Great grades
                    new AssignmentResult { AssignmentId = 4, StudentProfileId = 5, Score = 91, Feedback = "Beautiful database design. Normalization is perfect." },
                    new AssignmentResult { AssignmentId = 10, StudentProfileId = 5, Score = 89, Feedback = "Serverless app works flawlessly." },
                    
                    // Carlos (ID 6) - Average
                    new AssignmentResult { AssignmentId = 6, StudentProfileId = 6, Score = 74, Feedback = "Memory management report is decent." },
                    new AssignmentResult { AssignmentId = 12, StudentProfileId = 6, Score = 78, Feedback = "Good penetration testing attempt." },
                    
                    // Manar (ID 7) - Excellent all around
                    new AssignmentResult { AssignmentId = 1, StudentProfileId = 7, Score = 98, Feedback = "Outstanding! Perfect execution." },
                    new AssignmentResult { AssignmentId = 3, StudentProfileId = 7, Score = 96, Feedback = "SQL mastery demonstrated!" },
                    new AssignmentResult { AssignmentId = 7, StudentProfileId = 7, Score = 97, Feedback = "AI algorithms implemented brilliantly." },
                    
                    // Caio (ID 8) - Mixed grades
                    new AssignmentResult { AssignmentId = 5, StudentProfileId = 8, Score = 82, Feedback = "Good work on the scheduler." },
                    new AssignmentResult { AssignmentId = 10, StudentProfileId = 8, Score = 55, Feedback = "Serverless app needs revision. Function not triggering correctly." },
                };
                context.AssignmentResults.AddRange(results);
                await context.SaveChangesAsync();
            }

            // Seed Exams
            if (!context.Exams.Any())
            {
                var exams = new List<Exam>
                {
                    new Exam { CourseId = 1, Title = "Programming Final Exam", Date = DateTime.Now.AddDays(-18), MaxScore = 100, ResultsReleased = true },
                    new Exam { CourseId = 2, Title = "Database Midterm", Date = DateTime.Now.AddDays(-22), MaxScore = 100, ResultsReleased = true },
                    new Exam { CourseId = 3, Title = "OS Final Exam", Date = DateTime.Now.AddDays(-12), MaxScore = 100, ResultsReleased = false },
                    new Exam { CourseId = 4, Title = "AI Comprehensive Exam", Date = DateTime.Now.AddDays(-8), MaxScore = 100, ResultsReleased = false },
                    new Exam { CourseId = 5, Title = "Cloud Certification Prep", Date = DateTime.Now.AddDays(-5), MaxScore = 100, ResultsReleased = false },
                    new Exam { CourseId = 6, Title = "Security Exam", Date = DateTime.Now.AddDays(-3), MaxScore = 100, ResultsReleased = false },
                };
                context.Exams.AddRange(exams);
                await context.SaveChangesAsync();
            }

            // Seed Exam Results
            if (!context.ExamResults.Any())
            {
                var examResults = new List<ExamResult>
                {
                    // Released exams results
                    new ExamResult { ExamId = 1, StudentProfileId = 1, Score = 89, Grade = "B+" },
                    new ExamResult { ExamId = 1, StudentProfileId = 4, Score = 95, Grade = "A" },
                    new ExamResult { ExamId = 1, StudentProfileId = 7, Score = 97, Grade = "A+" },

                    new ExamResult { ExamId = 2, StudentProfileId = 1, Score = 92, Grade = "A-" },
                    new ExamResult { ExamId = 2, StudentProfileId = 5, Score = 88, Grade = "B+" },
                    new ExamResult { ExamId = 2, StudentProfileId = 7, Score = 94, Grade = "A" },
                    
                    // Not released yet - hidden from students
                    new ExamResult { ExamId = 3, StudentProfileId = 2, Score = 71, Grade = "C+" },
                    new ExamResult { ExamId = 3, StudentProfileId = 6, Score = 68, Grade = "C" },
                    new ExamResult { ExamId = 3, StudentProfileId = 8, Score = 79, Grade = "B-" },

                    new ExamResult { ExamId = 4, StudentProfileId = 2, Score = 65, Grade = "C" },
                    new ExamResult { ExamId = 4, StudentProfileId = 4, Score = 88, Grade = "B+" },
                    new ExamResult { ExamId = 4, StudentProfileId = 7, Score = 96, Grade = "A" },

                    new ExamResult { ExamId = 5, StudentProfileId = 3, Score = 82, Grade = "B" },
                    new ExamResult { ExamId = 5, StudentProfileId = 5, Score = 91, Grade = "A-" },
                    new ExamResult { ExamId = 5, StudentProfileId = 8, Score = 54, Grade = "F" },

                    new ExamResult { ExamId = 6, StudentProfileId = 3, Score = 76, Grade = "C+" },
                    new ExamResult { ExamId = 6, StudentProfileId = 6, Score = 82, Grade = "B" },
                    new ExamResult { ExamId = 6, StudentProfileId = 8, Score = 48, Grade = "F" },
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