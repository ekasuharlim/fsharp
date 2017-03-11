using System;
﻿using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // initialize MyActorSystem
            MyActorSystem = ActorSystem.Create("MyActorSystem");


            //PrintInstructions();

            // time to make your first actors!
            //YOU NEED TO FILL IN HERE
            // make consoleWriterActor using these props: Props.Create(() => new ConsoleWriterActor())
            // make consoleReaderActor using these props: Props.Create(() => new ConsoleReaderActor(consoleWriterActor))
            //var consoleWriter = MyActorSystem.ActorOf(Props.Create
            //    (() => new ConsoleWriterActor()));
            //var consoleReader = MyActorSystem.ActorOf(Props.Create
            //    (() => new ConsoleReaderActor(consoleWriter)));

            
            Props consoleWriterProps = Props.Create(() => new ConsoleWriterActor());
            IActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps,
                "consoleWriterActor");
            Props tailCoordinatorProp = Props.Create(() => new TailCoordinatorActor());
            IActorRef tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProp,
                "tailCoordinatorActor");

            Props validationProp = Props.Create(() => new FileValidatorActor(consoleWriterActor));
            IActorRef validationActor = MyActorSystem.ActorOf(validationProp,
                   "validationActor");

            Props consoleReaderProps = Props.Create(() => new ConsoleReaderActor());
            IActorRef consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps,
                "consoleReaderActor");
            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }

    }
    #endregion
}
