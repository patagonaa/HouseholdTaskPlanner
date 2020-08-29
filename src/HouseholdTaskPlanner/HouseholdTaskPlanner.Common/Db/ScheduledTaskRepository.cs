﻿using Dapper;
using HouseholdTaskPlanner.Common.Db.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.Common.Db
{
    public class ScheduledTaskRepository : SqlServerRepository, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(IOptions<DbConfiguration> options) : base(options)
        {
        }

        public async Task<ScheduledTask> Get(int id)
        {
            using (var connection = GetConnection())
            {
                var query = @"SELECT * FROM ScheduledTask";
                return await connection.QueryFirstAsync<ScheduledTask>(query, new { Id = id });
            }
        }

        public async Task<IList<ScheduledTaskViewModel>> GetList()
        {
            using (var connection = GetConnection())
            {
                var query = @"
SELECT
    ScheduledTask.Id,
    ScheduledTask.Date,
    ISNULL(RecurringTask.Name, ScheduledTask.Name) AS Name,
    RecurringTask.Description,
    AssignedUser.Id AS AssignedUser
FROM ScheduledTask
LEFT JOIN [RecurringTask] ON RecurringTask.Id = ScheduledTask.RecurringTaskId
LEFT JOIN [User] AS AssignedUser ON AssignedUser.Id = ScheduledTask.AssignedUserId
WHERE
    ScheduledTask.State = @State
ORDER BY
    ScheduledTask.Date ASC
";
                return (await connection.QueryAsync<ScheduledTaskViewModel>(query, new { State = (int)ScheduledTaskState.Todo })).ToList();
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
