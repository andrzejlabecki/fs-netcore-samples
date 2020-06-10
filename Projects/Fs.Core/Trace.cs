using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace AngularPOC.Core
{
    public class Trace
    {
        private static TraceSwitch traceSwitch = null;
        private static DailyTraceListener listener = null;
        private static string lastError	= null;
        private static string defaultObject = null;

        Trace()
        {
        }

        public static string getName()
        {
            return typeof(Trace).FullName;
        }

        public static TraceListener TraceListener
        {
            get
            {
                return listener;
            }
        }

        public static void Init(string DefaultObject, TraceLevel traceLevel, string FileName)
        {
            defaultObject = DefaultObject;
            traceSwitch = new TraceSwitch(getName(), "");
            traceSwitch.Level = traceLevel;

            TraceListenerCollection traceListenerCollection = System.Diagnostics.Trace.Listeners;
            if (traceListenerCollection != null)
            {
                listener = new DailyTraceListener(FileName);

                listener.TraceOutputOptions = TraceOptions.ProcessId|TraceOptions.ThreadId|TraceOptions.DateTime;

                traceListenerCollection.Add(listener);
            }
        }

        public static void Write(string Message, TraceLevel Level)
        {
            try
            {
                lastError = string.Empty;
                if (traceSwitch != null && traceSwitch.Level >= Level)
                {
                    TraceListenerCollection traceListenerCollection = System.Diagnostics.Trace.Listeners;

                    if (traceListenerCollection != null)
                    {
                        int listernersNo = traceListenerCollection.Count;

                        for (int i = 0; i < listernersNo; i++)
                        {
                            TraceListener listener = (TraceListener)traceListenerCollection[i];

                            if (listener != null)
                            {
                                StringBuilder sb = new StringBuilder();

                                DateTime date = DateTime.Now;

                                sb.Append(date.ToString("yyyy/MM/dd hh:mm:ss.fff tt")).Append(": ").Append(Message);

                                listener.WriteLine(sb.ToString());
                                listener.Flush();
                            }

                            if (traceSwitch.Level >= TraceLevel.Verbose)
                            {
                                TraceListener listenerDef = traceListenerCollection["Default"];

                                if (listenerDef != null)
                                {
                                    listenerDef.WriteLine(Message);
                                    listenerDef.Flush();
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
	        {
                lastError = ex.Message;
            }
        }

        public static void Write(string Object, string Subject, string Message, TraceLevel Level)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<").Append(Object).Append(">");
                sb.Append("<").Append(Subject).Append(">");
                sb.Append("<").Append(Message).Append(">");

                Trace.Write(sb.ToString(), Level);
            }
            catch
            {
            }
        }

        public static void Write(string Subject, string Message, TraceLevel Level)
        {
            Trace.Write(defaultObject, Subject, Message, Level);
        }

        public static string GetLastError()
        {
            return lastError;
        }
    }

    public class DailyTraceListener : TextWriterTraceListener
    {
        private const int FILE_ATTEMPT = 10;

        private DateTime _nextDateTime;

        private string _fileName;

        public DailyTraceListener(string fileName): base()
        {
            _fileName = fileName;
            InitializeListener();
            _nextDateTime = DateTime.Today.AddDays(1);
        }

        private void InitializeListener()
        {
            String fname = string.Empty;
            bool isopen = false;
            int cnt = 0;

            while (!isopen)
            {
                FileStream fs = null;
                fname = DailyPath(cnt);

                try
                {
                    // NOTE: This doesn't handle situations where file is opened for writing by another process but put into write shared mode, it will not throw an exception and won't show it as write locked
                    fs = System.IO.File.Open(fname, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); // If we can't open file for reading and writing then it's locked by another process for writing
                    isopen = true;
                }
                catch (System.Exception)
                {
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }

                cnt++;
                if (cnt >= FILE_ATTEMPT)
                    break;
            }

            base.Writer = new StreamWriter(fname, true);
        }

        public override void WriteLine(string value)
        {
            CheckDate();
            base.WriteLine(value);
            base.Flush();
        }

        public override void Write(string value)
        {
            CheckDate();
            base.Write(value);
            base.Flush();
        }

        private void CheckDate()
        {
            if (DateTime.Now > _nextDateTime)
            {
                _nextDateTime = DateTime.Today.AddDays(1);
                base.Writer.Flush();
                base.Writer.Close();
                InitializeListener();
            }

        }

        public override void Fail(string message)
        {
            CheckDate();
            base.Fail(message);
        }

        private string DailyPath(int cnt)
        {
            DateTime now = DateTime.Now;

            string fileNameWithDt = Path.GetFileNameWithoutExtension(_fileName);
            fileNameWithDt = fileNameWithDt.Replace("[YYYY]", string.Format("{0:0000}", now.Year));
            fileNameWithDt = fileNameWithDt.Replace("[MM]", string.Format("{0:00}", now.Month));
            fileNameWithDt = fileNameWithDt.Replace("[DD]", string.Format("{0:00}", now.Day) + ((cnt > 0) ? string.Format("_{0}", cnt) : string.Empty));
            fileNameWithDt = fileNameWithDt.Replace("[PI]", string.Format("{0}", Process.GetCurrentProcess().Id));

            fileNameWithDt = Path.Combine(Path.GetDirectoryName(_fileName),
            fileNameWithDt + Path.GetExtension(_fileName));

            return fileNameWithDt;
        }
    }
}
