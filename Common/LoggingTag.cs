using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CG_2IV05.Common
{
	public class LoggingTag
	{
		private static ConcurrentDictionary<int, Stack<string>> _loggingContexts =
			new ConcurrentDictionary<int, Stack<string>>();

		public static void Pop()
		{
			CreateStackIfNeccesary();
			if (_loggingContexts[CurrentContextID].Count > 1)
			{
				_loggingContexts[CurrentContextID].Pop();
			}
		}

		public static void Push(string context)
		{
			CreateStackIfNeccesary();
			_loggingContexts[CurrentContextID].Push(context);
		}

		public static string CurrentContext
		{
			get
			{
				CreateStackIfNeccesary();
				return _loggingContexts[CurrentContextID].Peek();
			}
		}

		private static int CurrentContextID
		{
			get { return Thread.CurrentThread.ManagedThreadId; }
		}

		private static void CreateStackIfNeccesary()
		{
			if (!_loggingContexts.ContainsKey(CurrentContextID))
			{
				_loggingContexts[CurrentContextID] = new Stack<string>();
				_loggingContexts[CurrentContextID].Push(CurrentContextID.ToString());
			}
		}
	}
}
