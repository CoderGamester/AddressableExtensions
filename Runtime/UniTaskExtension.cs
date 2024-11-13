using Cysharp.Threading.Tasks;

namespace GameLovers.AssetsImporter
{
    /// <summary>
    /// Extension methods for UniTask class
    /// </summary>
    public static class UniTaskExtension
    {
        /// <summary>
        /// Gets the result of a UniTask synchronously.
        /// This method blocks the calling thread until the task completes.
        /// </summary>
        /// <typeparam name="T">The type of the result of the task.</typeparam>
        /// <param name="task">The task to get the result of.</param>
        /// <returns>The result of the task.</returns>
        /// <remarks>
        /// This method is provided for convenience when you need to get the result of a task synchronously.
        /// However, it is generally recommended to use asynchronous programming patterns instead of synchronous ones.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The task has not completed yet.</exception>
        /// <exception cref="AggregateException">The task completed in a Faulted state.</exception>
        public static T GetResult<T>(this UniTask<T> task) => task.GetAwaiter().GetResult();
    }
}