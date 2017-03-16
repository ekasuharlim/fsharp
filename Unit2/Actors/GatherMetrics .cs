using System;
using Akka.Actor;

namespace ChartApp.Actors
{
    #region Reporting

    public class GatherMetrics
    {
 
    }

    public class Metric
    {
        public Metric(string series, float counterValue)
        {
            CounterValue = counterValue;
            Series = series;
        }

        public string Series { get; private set; }

        public float CounterValue { get; private set; }
    }
    #endregion

    #region Performance Counter Management

    /// <summary>
    /// All types of counters supported by this example
    /// </summary>
    public enum CounterType
    {
        Cpu,
        Memory,
        Disk
    }

    public class SubscribeCounter
    {
        public SubscribeCounter(CounterType counterType,IActorRef subscriber)
        {
            Subscriber = subscriber;
            Counter = counterType;
        }

        public CounterType Counter { get; set; }

        public IActorRef Subscriber { get; set; }

    }

    public class UnsubscribeCounter
    {
        public UnsubscribeCounter(CounterType counter, IActorRef subscriber)
        {
            Subscriber = subscriber;
            Counter = counter;
        }

        public CounterType Counter { get; private set; }

        public IActorRef Subscriber { get; private set; }
    }

    #endregion
}
