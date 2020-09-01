using Dapper;
using HouseholdTaskPlanner.Common.Db.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.Common.Db
{
    public class RecurringTaskRepository : SqlServerRepository, IRecurringTaskRepository
    {
        public RecurringTaskRepository(IOptions<DbConfiguration> options)
            : base(options)
        {
        }

        public async Task<IList<RecurringTask>> GetAll()
        {
            using (var connection = GetConnection())
            {
                var results = await connection.QueryAsync<RecurringTask>("SELECT * FROM [dbo].[RecurringTask]");
                return results.ToList();
            }
        }

        public async Task<RecurringTask> Get(int id)
        {
            using (var connection = GetConnection())
            {
                return await connection.QueryFirstAsync<RecurringTask>("SELECT * FROM [dbo].[RecurringTask] WHERE Id = @id", new { id = id });
            }
        }

        public async Task Insert(RecurringTask task)
        {
            using (var connection = GetConnection())
            {
                var sqlStr = @"
    INSERT INTO [dbo].[RecurringTask]
        ([Name],[Description],[IntervalDays])
        VALUES (@Name, @Description, @IntervalDays)";

                await connection.ExecuteAsync(sqlStr, task);
            }
        }

        public async Task<bool> Update(RecurringTask task)
        {
            using (var connection = GetConnection())
            {
                var sqlStr = @"UPDATE [dbo].[RecurringTask] SET [Name] = @Name, [Description] = @Description, [IntervalDays] = @IntervalDays WHERE Id = @id";

                return await connection.ExecuteAsync(sqlStr, task) > 0;
            }
        }

        public async Task<IList<RecurringTask>> GetUnscheduled()
        {
            using (var connection = GetConnection())
            {
                var results = await connection.QueryAsync<RecurringTask>(@"
SELECT
    RecurringTask.*
FROM [dbo].[RecurringTask]
WHERE
    NOT EXISTS (SELECT ScheduledTask.Id FROM ScheduledTask WHERE ScheduledTask.RecurringTaskId = RecurringTask.Id AND ScheduledTask.State = @State)
", new { State = (int)ScheduledTaskState.Todo });
                return results.ToList();
            }
        }
    }
}
