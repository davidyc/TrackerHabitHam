using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Data
{
    public static class WorkoutDbContextSeeder
    {
        private static readonly string[] MuscleGroupNames =
        {
            "Грудь", "Спина", "Плечи", "Бицепс", "Трицепс",
            "Предплечья", "Пресс", "Квадрицепс", "Бицепс бедра",
            "Ягодицы", "Икры", "Кардио", "Другое"
        };

        public static async Task SeedMuscleGroupsAsync(WorkoutDbContext context, CancellationToken ct = default)
        {
            if (await context.MuscleGroups.AnyAsync(ct))
                return;

            foreach (var name in MuscleGroupNames)
            {
                context.MuscleGroups.Add(new MuscleGroup { Name = name });
            }
            await context.SaveChangesAsync(ct);
        }
    }
}
