namespace ECM7.Migrator.NAnt.Loggers
{
	using System;
	using global::NAnt.Core;
	using log4net;
	using log4net.Core;

	using Level = global::NAnt.Core.Level;

	/// <summary>
	/// NAnt task logger for the migration mediator
	/// </summary>
	public class TaskLogger : ILog
	{
		/// <summary>
		/// ���� NAnt, ������� �������� � ��������
		/// </summary>
		private readonly Task task;

		/// <summary>
		/// �������������
		/// </summary>
		/// <param name="task">���� NAnt, ������� �������� � ��������</param>
		public TaskLogger(Task task)
		{
			Require.IsNotNull(task, "�� ����� ������� task ��� NAnt");
			this.task = task;
		}

		#region Implementation of ILoggerWrapper

		public ILogger Logger
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region Implementation of ILog

		public void Debug(object message)
		{
			task.Log(Level.Debug, message.ToString());
		}

		public void Debug(object message, Exception exception)
		{
			task.Log(Level.Debug, message.ToString());
		}

		public void DebugFormat(string format, params object[] args)
		{
			task.Log(Level.Debug, format, args);
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			task.Log(Level.Debug, format, args);
		}

		public void Info(object message)
		{
			throw new NotImplementedException();
		}

		public void Info(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Warn(object message)
		{
			throw new NotImplementedException();
		}

		public void Warn(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Error(object message)
		{
			throw new NotImplementedException();
		}

		public void Error(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Fatal(object message)
		{
			throw new NotImplementedException();
		}

		public void Fatal(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public bool IsDebugEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsInfoEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsWarnEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsErrorEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsFatalEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}