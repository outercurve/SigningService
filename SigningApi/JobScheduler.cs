using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SigningServiceBase;

namespace Outercurve.SigningApi
{
    public class JobScheduler : IDisposable, IDependency
    {
        readonly BlockingCollection<Job> _jobs = new BlockingCollection<Job>();
        private Task _jobsTask;
        
        public JobScheduler()
        {
            _jobsTask = Task.Factory.StartNew(() =>
                {
                    foreach (var j in _jobs.GetConsumingEnumerable())
                    {
                        
                        RunTask(j).Wait();
                    }
                });
        }

       
        private async Task RunTask(Job j)
        {
            j.PrefixedAction();
            try
            {
                var t = Task.Factory.StartNew(j.Action);
                if (await Task.WhenAny(t, Task.Delay(TimeSpan.FromMinutes(20))) == t && !t.IsFaulted)
                {
                    j.PostFixedAction();
                }
                else
                {
                    // we timedout
                    j.FailedAction();
                }
            }
            catch
            {
                j.FailedAction();
            }
        }


        public void Add(Job job)
        {
           _jobs.Add(job);
        }

        public void Dispose()
        {
            _jobs.CompleteAdding();
        }
    }
}