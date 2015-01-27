using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Protocol;
using CalcIt.Protocol.Server;

namespace CalcIt.Lib.CommandExecution
{
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.Log;
    using CalcIt.Protocol.Monitor;

    public class CommandExecutor<T> where T : class, ICalcItSession
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
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.commandQueue = new Queue<T>();
            this.taskWaitSleepTime = new TimeSpan(0, 0, 0, 0, 100);
        }

        /// <summary>
        /// Gets or sets the network access.
        /// </summary>
        /// <value>
        /// The network access.
        /// </value>
        public INetworkAccess<T> NetworkAccess { get; set; }

        /// <summary>
        /// Gets or sets the method provider.
        /// </summary>
        /// <value>
        /// The method provider.
        /// </value>
        public object MethodProvider { get; set; }

        /// <summary>
        /// Finds the executor method.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">message</exception>
        /// <exception cref="System.InvalidOperationException">MethodProvider has to be initialized!</exception>
        private MethodInfo FindExecutorMethod(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (MethodProvider == null)
            {
                throw new InvalidOperationException("MethodProvider has to be initialized!");
            }

            MethodInfo executerMethod = MethodProvider.GetType().GetMethods()
                                       .FirstOrDefault(method => method.GetCustomAttributes(typeof(CommandHandlerAttribute), false)
                                                                       .Any(attr => ((CommandHandlerAttribute)attr).MessageType == message.GetType()));

            return executerMethod;
        }

        /// <summary>
        /// Starts the executor.
        /// </summary>
        public void StartExecutor()
        {
            if (MethodProvider == null)
            {
                throw new InvalidOperationException("MethodProvider has to be initialized!");
            }

            if (NetworkAccess == null)
            {
                throw new InvalidOperationException("NetworkAccess has to be initialized!");
            }

            NetworkAccess.MessageReceived += (sender, e) => { commandQueue.Enqueue(e.Message); };

            isExecuting = true;

            Task.Run(() => RunExecutor());
        }

        /// <summary>
        /// Stops the executor.
        /// </summary>
        public void StopExecutor()
        {
            isExecuting = false;
        }

        /// <summary>
        /// Runs the executor.
        /// </summary>
        private void RunExecutor()
        {
            while (isExecuting)
            {
                while (commandQueue.Count == 0)
                {
                    Thread.Sleep(taskWaitSleepTime);

                    if (!isExecuting)
                    {
                        return;
                    }
                }

                T message = commandQueue.Dequeue();

                MethodInfo executerMethod = FindExecutorMethod(message);

                // ReSharper disable once UseNullPropagation
                if (executerMethod != null)
                {
                    try
                    {
                        executerMethod.Invoke(MethodProvider, new object[] { message });
                    }
                    catch (Exception ex)
                    {
                        LogMessage(new LogMessage(ex));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        private void LogMessage(LogMessage logMessage)
        {
            // ReSharper disable once UseNullPropagation
            if (Logger != null)
            {
                Logger.AddLogMessage(logMessage);
            }
        }

        /// <summary>
        /// Handles the tunneld message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void HandleTunneldMessage(T message)
        {
            commandQueue.Enqueue(message);
        }
    }
}
