//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Concurrent;
//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using System.Threading;

namespace DiIiS_NA.Core.Logging
{
	public class FileTarget : LogTarget, IDisposable
	{
		private readonly string _fileName; 
		private readonly string _filePath; 
		private readonly string _fileTimestamp; 
		private FileStream _fileStream; 
		private StreamWriter _logStream; 
		private int _fileIndex; 
		private ConcurrentQueue<Action> TaskQueue;
		private Thread LoggerThread;

		/// <param name="fileName">Filename of the logfile.</param>
		/// <param name="minLevel">Minimum level of messages to emit</param>
		/// <param name="maxLevel">Maximum level of messages to emit</param>
		/// <param name="includeTimeStamps">Include timestamps in log?</param>
		/// <param name="reset">Reset log file on application startup?</param>
		public FileTarget(string fileName, Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps, bool reset = false)
		{
			this.MinimumLevel = minLevel;
			this.MaximumLevel = maxLevel;
			this.IncludeTimeStamps = includeTimeStamps;
			this._fileName = fileName;
			this._fileTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
			this._filePath = string.Format("{0}/{1}/{2}", LogConfig.Instance.LoggingRoot, this._fileTimestamp, _fileName);
			this._fileIndex = 0;

			if (!Directory.Exists(LogConfig.Instance.LoggingRoot)) // create logging directory if it does not exist yet.
				Directory.CreateDirectory(LogConfig.Instance.LoggingRoot);

			if (!Directory.Exists(string.Format("{0}/{1}", LogConfig.Instance.LoggingRoot, this._fileTimestamp))) // create logging directory if it does not exist yet.
				Directory.CreateDirectory(string.Format("{0}/{1}", LogConfig.Instance.LoggingRoot, this._fileTimestamp));

			this._fileStream = new FileStream(_filePath, reset ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read); // init the file stream.
			this._logStream = new StreamWriter(this._fileStream) { AutoFlush = true }; // init the stream writer.
			this.TaskQueue = new ConcurrentQueue<Action>();
			this.LoggerThread = new Thread(this.CheckQueue) { Name = "Logger", IsBackground = true };
			this.LoggerThread.Start();
		}

		public void CheckQueue()
		{
			while (true)
			{
				Action action = null;
				if (this.TaskQueue.TryDequeue(out action))
					action.Invoke();

				Thread.Sleep(1);
			}
		}

		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		public override void LogMessage(Logger.Level level, string logger, string message)
		{
			this.TaskQueue.Enqueue(() =>
			{
				var timeStamp = this.IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";
				if (!this._disposed) // make sure we're not disposed.
				{
					/*
					if (this._fileStream.Length >= 20971520) //20 MB limit
					{
						//System.IO.File.Move(this._filePath, string.Format("{0}.{1}", this._filePath, this._fileIndex));
						//this._fileIndex++;
						this._fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.Read); // init the file stream.
						this._logStream = new StreamWriter(this._fileStream) { AutoFlush = true }; // init the stream writer.
					}
					//*/
					if (level > Logger.Level.ChatMessage)
						this._logStream.WriteLine(string.Format("{0}[{1}] [{2}]: {3}", timeStamp, level.ToString().PadLeft(5), logger, message));
					else
						this._logStream.WriteLine(string.Format("{0}{1}", timeStamp, message));
				}
			});
		}

		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		/// <param name="exception">Exception to be included with log message.</param>
		public override void LogException(Logger.Level level, string logger, string message, Exception exception)
		{
			this.TaskQueue.Enqueue(() =>
			{
				var timeStamp = this.IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";
				if (!this._disposed) // make sure we're not disposed.
				{
					this._logStream.WriteLine(
						$"{timeStamp}[{level.ToString(),5}] [{logger}]: {message} - [Exception] {exception}");
				}
			});
		}

		#region de-ctor

		// IDisposable pattern: http://msdn.microsoft.com/en-us/library/fs2xkftw%28VS.80%29.aspx

		private bool _disposed = false;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this); // Take object out the finalization queue to prevent finalization code for it from executing a second time.
		}

		private void Dispose(bool disposing)
		{
			if (this._disposed) return; // if already disposed, just return

			if (disposing) // only dispose managed resources if we're called from directly or in-directly from user code.
			{
				this._logStream.Close();
				this._logStream.Dispose();
				this._fileStream.Close();
				this._fileStream.Dispose();
			}

			this._logStream = null;
			this._fileStream = null;

			_disposed = true;
		}

		~FileTarget() { Dispose(false); } // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones.

		#endregion
	}
}
