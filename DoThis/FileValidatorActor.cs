using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.IO;

namespace WinTail
{
    public class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;
        private readonly IActorRef _tailCoordinatorActor;

        public FileValidatorActor(IActorRef consoleWriterActror, 
            IActorRef tailCoordinatorActor)
        {
            _consoleWriterActor = consoleWriterActror;
            _tailCoordinatorActor = tailCoordinatorActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as String;
            if (string.IsNullOrEmpty(msg))
            {
                _consoleWriterActor.Tell(new Messages.NullInputError("Input is blank"));
                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                var valid = IsFileUri(msg);
                if (valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess(
                        String.Format("Start tailing {0}", msg)));
                    _tailCoordinatorActor.Tell(
                        new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.InputError(
                        String.Format("{0} is not a valid uri",msg)));
                    Sender.Tell(new Messages.ContinueProcessing());

                }
            }
        }

        private static bool IsFileUri(string path)
        {
            return File.Exists(path);
        }

    }
}
