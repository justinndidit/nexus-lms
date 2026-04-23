using Microsoft.EntityFrameworkCore;

namespace webApi.Data.Seeders;

public static class RoleSeeder
{
    private static readonly List<string> Roles =
    [
        // Administrative
        "system_admin",
        "registrar_officer",
        "exam_officer",

        // Faculty/Department Leadership
        "dean_of_faculty",
        "sub_dean",
        "head_of_department",
        "department_exam_officer",

        // Academic Staff
        "professor",
        "lecturer",
        "lecturer_in_charge",
        "guest_lecturer",
        "teaching_assistant",

        // Students
        "student",
        "course_rep",
        "post_graduate",
    ];

    public static async Task SeedAsync(LMSApiApplicationContext db)
    {
        if (await db.Roles.AnyAsync()) return;
        var existingRoles = await db.Roles
            .Select(r => r.RoleName)
            .ToListAsync();

        var newRoles = Roles
            .Where(r => !existingRoles.Contains(r))
            .Select(r => new Role(r))
            .ToList();

        if (newRoles.Count == 0) return;

        await db.Roles.AddRangeAsync(newRoles);
        await db.SaveChangesAsync();
    }
}