//Blizzless Project 2022
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
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
			MinimumLevel = minLevel;
			MaximumLevel = maxLevel;
			IncludeTimeStamps = includeTimeStamps;
			_fileName = fileName;
			_fileTimestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
			_filePath = $"{LogConfig.Instance.LoggingRoot}/{_fileTimestamp}/{_fileName}";
			_fileIndex = 0;

			if (!Directory.Exists(LogConfig.Instance.LoggingRoot)) // create logging directory if it does not exist yet.
				Directory.CreateDirectory(LogConfig.Instance.LoggingRoot);

			if (!Directory.Exists($"{LogConfig.Instance.LoggingRoot}/{_fileTimestamp}")) // create logging directory if it does not exist yet.
				Directory.CreateDirectory($"{LogConfig.Instance.LoggingRoot}/{_fileTimestamp}");

			_fileStream = new FileStream(_filePath, reset ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read); // init the file stream.
			_logStream = new StreamWriter(_fileStream) { AutoFlush = true }; // init the stream writer.
			TaskQueue = new ConcurrentQueue<Action>();
			LoggerThread = new Thread(CheckQueue) { Name = "Logger", IsBackground = true };
			LoggerThread.Start();
		}

		public void CheckQueue()
		{
			while (true)
			{
				if (TaskQueue.TryDequeue(out var action))
					action.Invoke();

				Thread.Sleep(1);
			}
		}

		/// <summary>
		/// Replace the colors from AnsiColor so they do not appear in the log file.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private string NoColors(string message) => Regex.Replace(message, @"\$\[\[?[\w\W\d\s_\-\/]+\]\]?\$", "");

		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		public override void LogMessage(Logger.Level level, string logger, string message)
		{
			TaskQueue.Enqueue(() =>
			{
				message = NoColors(message);
				var timeStamp = IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";
				if (!_disposed) // make sure we're not disposed.
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
					_logStream.WriteLine(level > Logger.Level.ChatMessage
						? $"{timeStamp}[{level.ToString(),5}] [{logger}]: {message}"
						: $"{timeStamp}{message}");
				}
			});
		}

		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		/// <param name="exception">Exception to be included with log message.</param>
		public override void LogException(Logger.Level level, string logger, string message, Exception exception)
		{
			TaskQueue.Enqueue(() =>
			{
				message = NoColors(message);
				var timeStamp = IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";
				if (!_disposed) // make sure we're not disposed.
				{
					_logStream.WriteLine(
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
			GC.SuppressFinalize(this); // Take object out the finalization queue to prevent finalization code for it from executing (~FileTarget).
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return; // if already disposed, just return

			if (disposing) // only dispose managed resources if we're called from directly or in-directly from user code.
			{
				_logStream.Close();
				_logStream.Dispose();
				_fileStream.Close();
				_fileStream.Dispose();
			}

			_logStream = null;
			_fileStream = null;

			_disposed = true;
		}

		~FileTarget() { Dispose(false); } // finalizer called by the runtime. we should only dispose unmanaged objects and should NOT reference managed ones.

		#endregion
	}
}
