using Microsoft.AspNetCore.Mvc;
using TrackerHabiHamApi.Models;
using TrackerHabiHamApi.Models.Dto;
using TrackerHabiHamApi.Services;

namespace TrackerHabiHamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;
        private readonly IMuscleGroupService _muscleGroupService;
        private readonly IExerciseService _exerciseService;
        private readonly IWorkoutProgramService _workoutProgramService;
        private readonly IWorkoutSessionService _workoutSessionService;

        public WorkoutController(
            IWorkoutService workoutService,
            IMuscleGroupService muscleGroupService,
            IExerciseService exerciseService,
            IWorkoutProgramService workoutProgramService,
            IWorkoutSessionService workoutSessionService)
        {
            _workoutService = workoutService;
            _muscleGroupService = muscleGroupService;
            _exerciseService = exerciseService;
            _workoutProgramService = workoutProgramService;
            _workoutSessionService = workoutSessionService;
        }

        [HttpGet("check")]
        public async Task<ActionResult> CheckConnectionAsync(CancellationToken ct = default)
        {
            try
            {
                var isConnected = await _workoutService.CheckConnectionAsync(ct);
                return isConnected
                    ? Ok(new { connected = true, message = "PostgreSQL connection is healthy." })
                    : StatusCode(503, new { connected = false, message = "PostgreSQL connection failed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { connected = false, message = ex.Message });
            }
        }

        #region MuscleGroups
        [HttpGet("muscle-groups")]
        public async Task<ActionResult<IEnumerable<MuscleGroup>>> GetMuscleGroups(CancellationToken ct = default)
        {
            var items = await _muscleGroupService.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("muscle-groups/{id:int}")]
        public async Task<ActionResult<MuscleGroup>> GetMuscleGroup(int id, CancellationToken ct = default)
        {
            var item = await _muscleGroupService.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }
        #endregion

        #region Exercises
        [HttpGet("exercises")]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises([FromQuery] int? muscleGroupId, CancellationToken ct = default)
        {
            var items = await _exerciseService.GetAllAsync(muscleGroupId, ct);
            return Ok(items);
        }

        [HttpGet("exercises/{id:int}")]
        public async Task<ActionResult<Exercise>> GetExercise(int id, CancellationToken ct = default)
        {
            var item = await _exerciseService.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("exercises")]
        public async Task<ActionResult<Exercise>> CreateExercise([FromBody] Exercise exercise, CancellationToken ct = default)
        {
            var created = await _exerciseService.CreateAsync(exercise, ct);
            return CreatedAtAction(nameof(GetExercise), new { id = created.Id }, created);
        }

        [HttpPut("exercises/{id:int}")]
        public async Task<ActionResult<Exercise>> UpdateExercise(int id, [FromBody] Exercise exercise, CancellationToken ct = default)
        {
            var updated = await _exerciseService.UpdateAsync(id, exercise, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("exercises/{id:int}")]
        public async Task<ActionResult> DeleteExercise(int id, CancellationToken ct = default)
        {
            var deleted = await _exerciseService.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
        #endregion

        #region WorkoutPrograms
        [HttpGet("programs")]
        public async Task<ActionResult<IEnumerable<ProgramListDto>>> GetPrograms(CancellationToken ct = default)
        {
            var items = await _workoutProgramService.GetProgramsListAsync(ct);
            return Ok(items);
        }

        [HttpGet("programs/{id:int}")]
        public async Task<ActionResult<WorkoutProgram>> GetProgram(int id, CancellationToken ct = default)
        {
            var item = await _workoutProgramService.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("programs")]
        public async Task<ActionResult<WorkoutProgram>> CreateProgram([FromBody] WorkoutProgram program, CancellationToken ct = default)
        {
            var created = await _workoutProgramService.CreateAsync(program, ct);
            return CreatedAtAction(nameof(GetProgram), new { id = created.Id }, created);
        }

        [HttpPut("programs/{id:int}")]
        public async Task<ActionResult<WorkoutProgram>> UpdateProgram(int id, [FromBody] WorkoutProgram program, CancellationToken ct = default)
        {
            var updated = await _workoutProgramService.UpdateAsync(id, program, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("programs/{id:int}")]
        public async Task<ActionResult> DeleteProgram(int id, CancellationToken ct = default)
        {
            var deleted = await _workoutProgramService.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("programs/{id:int}/exercises")]
        public async Task<ActionResult<WorkoutProgramExercise>> AddExerciseToProgram(int id, [FromQuery] int exerciseId, [FromQuery] int order, [FromQuery] string? comment = null, CancellationToken ct = default)
        {
            var item = await _workoutProgramService.AddExerciseAsync(id, exerciseId, order, comment, ct);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpDelete("programs/{programId:int}/exercises/{programExerciseId:int}")]
        public async Task<ActionResult> RemoveExerciseFromProgram(int programId, int programExerciseId, CancellationToken ct = default)
        {
            var deleted = await _workoutProgramService.RemoveExerciseAsync(programId, programExerciseId, ct);
            return deleted ? NoContent() : NotFound();
        }
        #endregion

        #region Workouts (sessions)
        [HttpGet("workouts")]
        public async Task<ActionResult<IEnumerable<Workout>>> GetWorkouts(
            [FromQuery] int? programId,
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to,
            CancellationToken ct = default)
        {
            var items = await _workoutSessionService.GetAllAsync(programId, from, to, ct);
            return Ok(items);
        }

        [HttpGet("workouts/{id:int}")]
        public async Task<ActionResult<Workout>> GetWorkout(int id, CancellationToken ct = default)
        {
            var item = await _workoutSessionService.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("workouts")]
        public async Task<ActionResult<Workout>> CreateWorkout([FromBody] CreateWorkoutRequest request, CancellationToken ct = default)
        {
            var created = await _workoutSessionService.CreateFromProgramAsync(request, ct);
            if (created == null) return NotFound();
            return CreatedAtAction(nameof(GetWorkout), new { id = created.Id }, created);
        }

        [HttpPut("workouts/{id:int}")]
        public async Task<ActionResult<Workout>> UpdateWorkout(int id, [FromBody] Workout workout, CancellationToken ct = default)
        {
            var updated = await _workoutSessionService.UpdateAsync(id, workout, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("workouts/{id:int}")]
        public async Task<ActionResult> DeleteWorkout(int id, CancellationToken ct = default)
        {
            var deleted = await _workoutSessionService.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("workouts/{id:int}/sets")]
        public async Task<ActionResult<Set>> AddSet(int id, [FromBody] Set set, CancellationToken ct = default)
        {
            var created = await _workoutSessionService.AddSetAsync(id, set, ct);
            return created == null ? NotFound() : Ok(created);
        }

        [HttpPut("workouts/{workoutId:int}/sets/{setId:int}")]
        public async Task<ActionResult<Set>> UpdateSet(int workoutId, int setId, [FromBody] Set set, CancellationToken ct = default)
        {
            var updated = await _workoutSessionService.UpdateSetAsync(workoutId, setId, set, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("workouts/{workoutId:int}/sets/{setId:int}")]
        public async Task<ActionResult> RemoveSet(int workoutId, int setId, CancellationToken ct = default)
        {
            var deleted = await _workoutSessionService.RemoveSetAsync(workoutId, setId, ct);
            return deleted ? NoContent() : NotFound();
        }
        #endregion
    }
}
