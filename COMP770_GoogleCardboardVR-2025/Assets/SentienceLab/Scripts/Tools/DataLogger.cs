﻿#region Copyright Information
// SentienceLab Unity Framework
// (C) SentienceLab (sentiencelab@aut.ac.nz), Auckland University of Technology, Auckland, New Zealand 
#endregion Copyright Information

using UnityEngine;
using System.IO;

namespace SentienceLab
{
	public interface IDataLogger
	{
		void Log(string _event, params object[] _data);
	}

	[AddComponentMenu("SentienceLab/Tools/Data Logger")]
	public class DataLogger : MonoBehaviour, IDataLogger
	{
		
		[Tooltip("Filename of the logfile\n(The string \"{TIMESTAMP}\" will be replaced by an actual timestamp)")]
		public string LogFilename = "Logfile_{TIMESTAMP}.txt";

		[Tooltip("Separator between values")]
		public string Separator = "\\t";

		public bool AlsoLogToConsole = false;


		public static IDataLogger Instance
		{
			get
			{
				if (m_instance == null && !m_instanceSearched)
				{
					m_instanceSearched = true;
					m_consoleLogger = new ConsoleLogger();
					m_instance = FindAnyObjectByType<DataLogger>();
					if (m_instance != null)
					{
						m_instance.OpenLogfile();
					}
					else
					{
						Debug.LogWarning("Could not find an instance of DataLogger. Using console logger only.");
					}
				}
				return (m_instance != null) ? m_instance : m_consoleLogger;
			}
		}


		public void Awake()
		{
			Start();
		}


		public void Start()
		{
			// force creation of instance and opening of file
			IDataLogger l = Instance;
			if (l != null) OpenLogfile();
		}


		protected void OpenLogfile()
		{
			if (m_writer == null && this.enabled)
			{
				try
				{
					// if required, build timestamped filename
					string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
					LogFilename = LogFilename.Replace("{TIMESTAMP}", timestamp);

#if !UNITY_EDITOR
					LogFilename = Application.persistentDataPath + "/" + LogFilename;
#endif
					// special case for TAB as separator
					if (Separator == "\\t")
					{
						Separator = "\t";
					}

					// open logfile and append
					m_writer = new StreamWriter(LogFilename, true);
					Log("start", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
				}
				catch (System.Exception e)
				{
					Debug.LogWarningFormat("Could not open logfile (Reason: {0})", e.ToString());
					this.enabled = false;
				}
			}
		}


		protected void CloseLogfile()
		{
			// close logfile
			try
			{
				if (m_writer != null)
				{
					Log("end", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
					m_writer.WriteLine();
					m_writer.Close();
					m_writer = null;
				}
			}
			catch (System.Exception e)
			{
				Debug.LogWarningFormat("Could not close logfile (Reason: {0})", e.ToString());
			}
		}


		public void Log(string _event)
		{
			Log(_event, new object[] { });
		}


		public void LogTransform(Transform _t)
		{
			Log("transform", 
				_t.gameObject.name,
				_t.position.x.ToString("F4"),
				_t.position.y.ToString("F4"),
				_t.position.z.ToString("F4"),
				_t.rotation.x.ToString("F4"),
				_t.rotation.y.ToString("F4"),
				_t.rotation.z.ToString("F4"),
				_t.rotation.w.ToString("F4"));
		}


		public void LogTransformPosition(Transform _t)
		{
			Log("transform",
				_t.gameObject.name,
				_t.position.x.ToString("F4"),
				_t.position.y.ToString("F4"),
				_t.position.z.ToString("F4"));
		}


		public void Log(string _event, params object[] _data)
		{
			if (m_writer != null)
			{
				m_writer.Write(string.Format("{0:0.000}", Time.unscaledTime));
				m_writer.Write(Separator);
				m_writer.Write(_event);
				foreach (object obj in _data)
				{
					m_writer.Write(Separator);
					m_writer.Write(obj.ToString());
				}
				m_writer.WriteLine();
				m_writer.Flush();
			}

			if (AlsoLogToConsole)
			{
				m_consoleLogger.Log(_event, _data);
			}
		}


		public void OnApplicationQuit()
		{
			CloseLogfile();
		}


		protected class ConsoleLogger : IDataLogger
		{
			public void Log(string _event, params object[] _data)
			{
				string output = string.Format("{0:0.000}", Time.unscaledTime) + '\t' + _event;
				foreach (object obj in _data)
				{
					output += '\t';
					output += obj.ToString();
				}
				Debug.Log(output);
			}
		}


		private static DataLogger   m_instance         = null;
		private static IDataLogger  m_consoleLogger    = null;
		private static bool         m_instanceSearched = false;
		private        StreamWriter m_writer;
	}
}