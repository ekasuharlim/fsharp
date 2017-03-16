using System;
using Akka.Actor;
using System.Diagnostics;
using System.Collections.Generic;


namespace ChartApp.Actors
{
    public class PerformanceCounterActor : ReceiveActor
    {
        private readonly string _seriesName;
        private readonly Func<PerformanceCounter> _performanceCounterGenerator;
        private PerformanceCounter _counter;

        private readonly HashSet<IActorRef> _subscriptions;
        private readonly ICancelable _cancelPublishing;

        public PerformanceCounterActor(string seriesName,
            Func<PerformanceCounter> perfCounterGenerator)
        {
            _seriesName = seriesName;
            _performanceCounterGenerator = perfCounterGenerator;
            _subscriptions = new HashSet<IActorRef>();
            _cancelPublishing = new Cancelable(Context.System.Scheduler);
            Receive<GatherMetrics>(gm => 
            {
                var metric = new Metric(_seriesName, _counter.NextValue());
                foreach (var sub in _subscriptions) sub.Tell(metric);
            });

            Receive<SubscribeCounter>(sc =>
            {
                _subscriptions.Add(sc.Subscriber);
            });

            Receive<UnsubscribeCounter>(us =>
            {
                _subscriptions.Remove(us.Subscriber);
            });

        }

        #region Actor lifecycle methods
        protected override void PreStart()
        {
            _counter = _performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromMilliseconds(250),
                TimeSpan.FromMilliseconds(250),
                Self,
                new GatherMetrics(),
                Self,
                _cancelPublishing
                );

        }

        protected override void PostStop()
        {
            try
            {
                _cancelPublishing.Cancel();
                _counter.Dispose();
            }
            catch
            {
            }
            finally
            {
                base.PostStop();
            }

        }
        #endregion

    }
}
