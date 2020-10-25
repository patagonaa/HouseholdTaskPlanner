using Dapper;
using TaskPlanner.Common.Db.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskPlanner.Common.Db
{
    public class ScheduledTaskRepository : SqlServerRepository, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(IOptions<DbConfiguration> options) : base(options)
        {
        }

        public async Task<bool> Delete(int id)
        {
            using (var connection = GetConnection())
            {
                var query = @"DELETE FROM ScheduledTask WHERE Id = @Id";
                return await connection.ExecuteAsync(query, new { Id = id }) > 0;
            }
        }

        public async Task DeleteForRecurringTask(int recurringTaskId)
        {
            using (var connection = GetConnection())
            {
                var query = @"DELETE FROM ScheduledTask WHERE RecurringTaskId = @RecurringTaskId";
                await connection.ExecuteAsync(query, new { RecurringTaskId = recurringTaskId });
            }
        }

        public async Task<ScheduledTask> Get(int id)
        {
            using (var connection = GetConnection())
            {
                var query = @"SELECT * FROM ScheduledTask WHERE Id = @Id";
                return await connection.QueryFirstAsync<ScheduledTask>(query, new { Id = id });
            }
        }

        public async Task<IList<ScheduledTaskViewModel>> GetAll()
        {
            using (var connection = GetConnection())
            {
                var query = @"
SELECT
    ScheduledTask.Id,
    ScheduledTask.Date,
    ScheduledTask.State,
    ISNULL(RecurringTask.Name, ScheduledTask.Name) AS Name,
    RecurringTask.Description,
    AssignedUser.Id AS AssignedUser
FROM ScheduledTask
LEFT JOIN [RecurringTask] ON RecurringTask.Id = ScheduledTask.RecurringTaskId
LEFT JOIN [User] AS AssignedUser ON AssignedUser.Id = ScheduledTask.AssignedUserId
ORDER BY
    ScheduledTask.Date ASC
";
                return (await connection.QueryAsync<ScheduledTaskViewModel>(query)).ToList();
            }
        }

        public async Task Insert(ScheduledTask scheduledTask)
        {
            using (var connection = GetConnection())
            {
                var query = @"INSERT INTO ScheduledTask (Date, RecurringTaskId, State, AssignedUserId, Name) VALUES (@Date, @RecurringTaskId, @State, @AssignedUserId, @Name)";
                await connection.ExecuteAsync(query, scheduledTask);
            }
        }

        public async Task SetAssignedUser(int id, int? userId)
        {
            using (var connection = GetConnection())
            {
                var query = @"UPDATE ScheduledTask SET AssignedUserId = @UserId WHERE Id = @Id";
                await connection.ExecuteAsync(query, new { Id = id, UserId = userId });
            }
        }

        public async Task SetState(int id, ScheduledTaskState state)
        {
            using (var connection = GetConnection())
            {
                var query = @"UPDATE ScheduledTask SET State = @State WHERE Id = @Id";
                await connection.ExecuteAsync(query, new { Id = id, State = state });
            }
        }
    }
}
