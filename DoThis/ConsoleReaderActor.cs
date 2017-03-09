using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {

        public const string StartCommand = "start";
        public const string ExitCommand = "exit";


        protected override void OnReceive(object message)
        {

            var inputText = String.Empty;
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
                inputText = @"D:\temp\testtail.txt";
            }
            else
            {
                inputText = Console.ReadLine();
            }

            if (String.Equals(inputText, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the entire actor system (allows the process to exit)
                Context.System.Terminate();
            }
            else
            {
                Context.ActorSelection("akka://MyActorSystem/user/validationActor").Tell(inputText);
            }


        }

        #region Internal method
        private void DoPrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.\n");
        }
        #endregion

    }
}