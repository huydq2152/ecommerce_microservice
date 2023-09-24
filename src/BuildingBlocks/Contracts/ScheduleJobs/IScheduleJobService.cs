using System.Linq.Expressions;

namespace Contracts.ScheduleJobs;

public interface IScheduleJobService
{
    #region Fire And Forget

    string Enqueue(Expression<Action> functionCall);
    string Enqueue<T>(Expression<Action<T>> functionCall);

    #endregion

    #region Delayded job

    string Schedule(Expression<Action> functionCall, TimeSpan delay);
    string Schedule<T>(Expression<Action<T>> functionCall, TimeSpan delay);

    string Schedule(Expression<Action> functionCall, DateTimeOffset enqueueAt);

    #endregion

    #region Continuos jobs

    string ContinueQueueWith(string parentJobId, Expression<Action> functionCall);

    #endregion

    bool Delete(string jobId);

    bool Requeue(string jobId);
}