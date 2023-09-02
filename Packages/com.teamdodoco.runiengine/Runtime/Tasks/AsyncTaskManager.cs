#nullable enable
using RuniEngine.Booting;
using RuniEngine.Threading;

namespace RuniEngine.Tasks
{
    public static class AsyncTaskManager
    {
        /*public static event Action? asyncTaskAdd = null;
        public static event Action? asyncTaskChange = null;
        public static event Action? asyncTaskRemove = null;

        public static void AsyncTaskAddEventInvoke() => asyncTaskAdd?.Invoke();
        public static void AsyncTaskRemoveEventInvoke() => asyncTaskRemove?.Invoke();
        public static void AsyncTaskChangeEventInvoke() => asyncTaskChange?.Invoke();*/



        //public static List<AsyncTask> asyncTasks { get; } = new List<AsyncTask>();

        public static void AllAsyncTaskCancel()
        {
            NotMainThreadException.Exception();
            NotPlayModeException.Exception();

            /*for (int i = 0; i < asyncTasks.Count; i++)
            {
                AsyncTask asyncTask = asyncTasks[i];
                if (!asyncTask.cantCancel)
                {
                    asyncTask.Remove();
                    i--;
                }
            }*/
        }
    }
}
