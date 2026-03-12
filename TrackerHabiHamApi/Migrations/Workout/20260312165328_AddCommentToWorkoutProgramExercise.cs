using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackerHabiHamApi.Migrations.Workout
{
    /// <inheritdoc />
    public partial class AddCommentToWorkoutProgramExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "WorkoutProgramExercises",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "WorkoutProgramExercises");
        }
    }
}
