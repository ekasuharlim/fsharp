﻿using System;
using System.Collections.Generic;
using Akka.Actor;
using System.IO;

namespace WinTail
{
    public class FileObserver : IDisposable
    {
        private readonly IActorRef _tailActor;
        private readonly string _absoluteFilePath;
        private FileSystemWatcher _watcher;
        private readonly string _fileDir;
        private readonly string _fileNameOnly;

        public FileObserver(IActorRef tailActor, string absoluteFilePath)
        {
            _tailActor = tailActor;
            _absoluteFilePath = absoluteFilePath;
            _fileDir = Path.GetDirectoryName(absoluteFilePath);
            _fileNameOnly = Path.GetFileName(absoluteFilePath);
        }

        public void Start()
        {
            _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly);
            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            _watcher.Changed += _watcher_Changed;
            _watcher.Error += _watcher_Error;
            _watcher.EnableRaisingEvents = true;

        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            _tailActor.Tell(new TailActor.FileError(_fileNameOnly,
                e.GetException().Message), ActorRefs.NoSender);

        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
             if(e.ChangeType == WatcherChangeTypes.Changed)
            {
                _tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
            }
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}