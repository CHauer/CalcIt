// -----------------------------------------------------------------------
// <copyright file="Logger.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - Logger.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CalcIt.Protocol.Monitor;

namespace CalcIt.Lib.Log
{
    /// <summary>
    /// 
    /// </summary>
    public class Logger : ILog
    {
        /// <summary>
        /// The log listeners
        /// </summary>
        private List<ILogListener> logListeners;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.LogMessages = new List<LogMessage>();
            this.logListeners = new List<ILogListener>();
        }

        /// <summary>
        /// Occurs when a message gets logged.
        /// </summary>
        public event EventHandler<LogMessage> MessageLogged;

        /// <summary>
        /// Gets the log messages.
        /// </summary>
        /// <value>
        /// The log messages.
        /// </value>
        public List<LogMessage> LogMessages { get; private set; }

        /// <summary>
        /// Gets the debug messages.
        /// </summary>
        /// <value>
        /// The debug messages.
        /// </value>
        public List<LogMessage> DebugMessages
        {
            get { return this.LogMessages.Where(lm => lm.IsDebug).ToList(); }
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddLogMessage(LogMessage message)
        {
            this.LogMessages.Add(message);

            OnMessageLogged(message);

            if (this.logListeners != null)
            {
                this.logListeners.ForEach(listener => listener.WriteLogMessage(message));
            }
        }

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void AddListener(ILogListener listener)
        {
            this.logListeners.Add(listener);
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void RemoveListener(ILogListener listener)
        {
            this.logListeners.Remove(listener);
        }

        /// <summary>
        /// Clears the listeners.
        /// </summary>
        public void ClearListeners()
        {
            this.logListeners.Clear();
        }

        /// <summary>
        /// Called when a message gets logged.
        /// </summary>
        /// <param name="e">The e.</param>
        protected virtual void OnMessageLogged(LogMessage e)
        {
            if (MessageLogged != null)
            {
                MessageLogged(this, e);
            }
        }
    }
}