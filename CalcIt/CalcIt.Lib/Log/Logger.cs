// -----------------------------------------------------------------------
// <copyright file="Logger.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - Logger.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Log
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The Logger implementation of Log interface.
    /// </summary>
    public class Logger : ILog
    {
        /// <summary>
        /// The log listeners.
        /// </summary>
        private List<ILogListener> logListeners;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger()
        {
            this.Initialize();
        }

        /// <summary>
        /// Occurs when a message gets logged.
        /// </summary>
        public event EventHandler<LogMessage> MessageLogged;

        /// <summary>
        /// Gets the debug messages.
        /// </summary>
        /// <value>
        /// The debug messages.
        /// </value>
        public List<LogMessage> DebugMessages
        {
            get
            {
                return this.LogMessages.Where(lm => lm.IsDebug).ToList();
            }
        }

        /// <summary>
        /// Gets the log messages.
        /// </summary>
        /// <value>
        /// The log messages.
        /// </value>
        public List<LogMessage> LogMessages { get; private set; }

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="listener">
        /// The listener.
        /// </param>
        public void AddListener(ILogListener listener)
        {
            this.logListeners.Add(listener);
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void AddLogMessage(LogMessage message)
        {
            this.LogMessages.Add(message);

            this.OnMessageLogged(message);

            if (this.logListeners != null)
            {
                Parallel.ForEach(this.logListeners, listener => listener.WriteLogMessage(message));
            }
        }

        /// <summary>
        /// Clears the listeners.
        /// </summary>
        public void ClearListeners()
        {
            this.logListeners.Clear();
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="listener">
        /// The listener.
        /// </param>
        public void RemoveListener(ILogListener listener)
        {
            this.logListeners.Remove(listener);
        }

        /// <summary>
        /// Called when a message gets logged.
        /// </summary>
        /// <param name="e">
        /// The e parameter.
        /// </param>
        protected virtual void OnMessageLogged(LogMessage e)
        {
            if (this.MessageLogged != null)
            {
                this.MessageLogged(this, e);
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.LogMessages = new List<LogMessage>();
            this.logListeners = new List<ILogListener>();
        }
    }
}