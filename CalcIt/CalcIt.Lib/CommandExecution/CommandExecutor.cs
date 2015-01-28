// -----------------------------------------------------------------------
// <copyright file="CommandExecutor.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - CommandExecutor.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.CommandExecution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.Log;
    using CalcIt.Lib.NetworkAccess;
    using CalcIt.Protocol;
    using CalcIt.Protocol.Monitor;

    /// <summary>
    /// The command executor.
    /// </summary>
    /// <typeparam name="T">
    /// Type of class and ICalcItSession implemented.
    /// </typeparam>
    public class CommandExecutor<T>
        where T : class, ICalcItSession
    {
        /// <summary>
        /// The command queue.
        /// </summary>
        private Queue<T> commandQueue;

        /// <summary>
        /// Indicates if the instance is executing messages.
        /// </summary>
        private bool isExecuting;

        /// <summary>
        /// The calculation step sleep time.
        /// </summary>
        private TimeSpan taskWaitSleepTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutor{T}"/> class.
        /// </summary>
        public CommandExecutor()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the method provider.
        /// </summary>
        /// <value>
        /// The method provider.
        /// </value>
        public object MethodProvider { get; set; }

        /// <summary>
        /// Gets or sets the network access.
        /// </summary>
        /// <value>
        /// The network access.
        /// </value>
        public INetworkAccess<T> NetworkAccess { get; set; }

        /// <summary>
        /// Starts the executor.
        /// </summary>
        public void StartExecutor()
        {
            if (this.MethodProvider == null)
            {
                throw new InvalidOperationException("MethodProvider has to be initialized!");
            }

            if (this.NetworkAccess == null)
            {
                throw new InvalidOperationException("NetworkAccess has to be initialized!");
            }

            this.NetworkAccess.MessageReceived += (sender, e) => { this.commandQueue.Enqueue(e.Message); };

            this.isExecuting = true;

            Task.Run(() => this.RunExecutor());
        }

        /// <summary>
        /// Stops the executor.
        /// </summary>
        public void StopExecutor()
        {
            this.isExecuting = false;
        }

        /// <summary>
        /// Handles the tunnel message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void HandleTunneldMessage(T message)
        {
            this.commandQueue.Enqueue(message);
        }

        /// <summary>
        /// Finds the executor method.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="MethodInfo"/> found method.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Message Finder exception.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// MethodProvider has to be initialized!.
        /// </exception>
        private MethodInfo FindExecutorMethod(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (this.MethodProvider == null)
            {
                throw new InvalidOperationException("MethodProvider has to be initialized!");
            }

            MethodInfo executerMethod =
                this.MethodProvider.GetType()
                    .GetMethods()
                    .FirstOrDefault(
                        method =>
                        method.GetCustomAttributes(typeof(CommandHandlerAttribute), false)
                            .Any(attr => ((CommandHandlerAttribute)attr).MessageType == message.GetType()));

            return executerMethod;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.commandQueue = new Queue<T>();
            this.taskWaitSleepTime = new TimeSpan(0, 0, 0, 0, 100);
        }

        /// <summary>
        /// Runs the executor.
        /// </summary>
        private void RunExecutor()
        {
            while (this.isExecuting)
            {
                while (this.commandQueue.Count == 0)
                {
                    Thread.Sleep(this.taskWaitSleepTime);

                    if (!this.isExecuting)
                    {
                        return;
                    }
                }

                T message = this.commandQueue.Dequeue();

                MethodInfo executerMethod = this.FindExecutorMethod(message);

                // ReSharper disable once UseNullPropagation
                if (executerMethod != null)
                {
                    try
                    {
                        executerMethod.Invoke(this.MethodProvider, new object[] { message });
                    }
                    catch (Exception ex)
                    {
                        this.LogMessage(new LogMessage(ex));
                    }
                }
            }
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        private void LogMessage(LogMessage logMessage)
        {
            // ReSharper disable once UseNullPropagation
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(logMessage);
            }
        }
    }
}