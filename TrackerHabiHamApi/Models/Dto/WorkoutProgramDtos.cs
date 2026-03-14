namespace TrackerHabiHamApi.Models.Dto;

public record ProgramListDto(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    List<ProgramExerciseDto> Exercises);

public record ProgramExerciseDto(
    int Id,
    int Order,
    string? Comment,
    ExerciseBriefDto Exercise);

public record ExerciseBriefDto(
    int Id,
    string Name,
    string? Description,
    MuscleGroupBriefDto? MuscleGroup);

public record MuscleGroupBriefDto(int Id, string Name);

/// <summary>Request body for POST /workouts. Program id required; date (defaults to today) and notes optional.</summary>
public record CreateWorkoutRequest(int WorkoutProgramId, DateOnly? Date = null, string? Notes = null);
