using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Classes
{
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public NotifyTaskCompletion()
        {

        }

        public NotifyTaskCompletion(Task<TResult> task)
        {
            initializeNewTask(task);
        }

        //----------------------------[ Properties ]------------------

        public Task<TResult> Task
        {
            get;
            set;
        }

        public Stack<Task<TResult>> TaskStack
        {
            get;
            set;
        }

        public TResult Result
        {
            get { return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult); }
        }

        public TResult ResultFromStack
        {
            get
            {
                var task = TaskStack.Pop();
                return (task != null && task.Status == TaskStatus.RanToCompletion) ? task.Result : default(TResult);
            }
        }

        public TaskStatus Status
        {
            get { return Task.Status; }
        }

        public bool IsCompleted
        {
            get { return Task.IsCompleted; }
        }

        public bool IsNotCompleted
        {
            get { return !Task.IsCompleted; }
        }

        public bool IsSuccessfullyCompleted
        {
            get { return Task.Status == TaskStatus.RanToCompletion; }
        }

        public bool IsCanceled
        {
            get { return Task.IsCanceled; }
        }

        public bool IsFaulted
        {
            get { return Task.IsFaulted; }
        }

        public AggregateException Exception
        {
            get { return Task.Exception; }
        }

        public Exception InnerException
        {
            get { return (Exception == null) ? null : Exception.InnerException; }
        }

        public string ErrorMessage
        {
            get { return (InnerException == null) ? null : InnerException.Message; }
        }


        //----------------------------[ Actions ]------------------

        public void initializeNewTask(Task<TResult> task)
        {

            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        public void initializeNewTask(Stack<Task<TResult>> taskStack)
        {
            TaskStack = taskStack;
            foreach (var task in taskStack)
            {
                if (!task.IsCompleted)
                {
                    var _ = WatchTaskAsync(task);
                }
            }

        }

        private async Task WatchTaskAsync(Task<TResult> task)
        {
            var propertyChanged = PropertyChanged;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));

            try
            {
                await task;
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                    Log.write(exception.Message, "ERR");
            }
            catch (Exception ex)
            {
                Log.write(ex.Message, "ERR");
            }


            if (propertyChanged == null)
                return;

            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this, new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }




    }

    //======================[ Pair ]=====================


    public class Pair<T1, T2>
    {
        public T1 PairID { get; set; }
        public T2 PairValue { get; set; }
    }

}
