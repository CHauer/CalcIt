using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Lib.NetworkAccess.Events;
using CalcIt.Lib.NetworkAccess.Transform;
using CalcIt.Protocol;

namespace CalcIt.Lib.NetworkAccess.Udp
{
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using CalcIt.Lib.Log;
    using CalcIt.Protocol.Data;
    using CalcIt.Protocol.Endpoint;
    using CalcIt.Protocol.Monitor;

    public class UdpClientConnector<T> : INetworkClientConnector<T> where T : class, ICalcItSession, IMessageControl
    {
        /// <summary>
        /// The client.
        /// </summary>
        private UdpClient client;

        /// <summary>
        /// The reconnect receiver
        /// </summary>
        private UdpClient reconnectReceiver;

        /// <summary>
        /// Indicates if the  connector is receiving and sending.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The first receive.
        /// </summary>
        private bool firstReceiveHandled;

        /// <summary>
        /// The first send.
        /// </summary>
        private bool firstSendHandled;

        /// <summary>
        /// The current message number
        /// </summary>
        private int currentMessageNumber;

        /// <summary>
        /// The message send queue
        /// </summary>
        private Queue<T> messageSendQueue;

        /// <summary>
        /// The reconnect port
        /// </summary>
        private int reconnectPort;

        /// <summary>
        /// The session identifier
        /// </summary>
        private Guid sessionId;

        /// <summary>
        /// The calculation step sleep time.
        /// </summary>
        private TimeSpan taskWaitSleepTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientConnector{T}"/> class.
        /// </summary>
        public UdpClientConnector()
        {
            this.Initialize();
        }

        /// <summary>
        /// The message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Connection Settings are invalid or missing.</exception>
        public string Hostname
        {
            get
            {
                // ReSharper disable once MergeSequentialChecks
                if (this.ConnectionSettings == null || !(this.ConnectionSettings is IpConnectionEndpoint))
                {
                    throw new InvalidOperationException("Connection Settings are invalid or missing.");
                }

                return this.ConnectionSettings.Hostname;
            }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Connection Settings are invalid or missing.</exception>
        public int Port
        {
            get
            {
                if (this.ConnectionSettings == null || !(this.ConnectionSettings is IpConnectionEndpoint))
                {
                    throw new InvalidOperationException("Connection Settings are invalid or missing.");
                }

                return (this.ConnectionSettings as IpConnectionEndpoint).Port;
            }
        }

        /// <summary>
        /// Gets or sets the message transformer.
        /// </summary>
        public IMessageTransformer<T> MessageTransformer { get; set; }

        /// <summary>
        /// Gets a value indicating whether is connected.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public bool IsConnected
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return isRunning; }
        }

        /// <summary>
        /// Gets or sets the connection settings.
        /// </summary>
        /// <value>
        /// The connection settings.
        /// </value>
        public ConnectionEndpoint ConnectionSettings { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// The connect.
        /// </summary>
        public void Connect()
        {
            try
            {
                this.client = new UdpClient(this.Hostname, this.Port);
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
            }

            this.isRunning = true;

            Task.Run(() => this.RunMessageReceiver());
            Task.Run(() => this.RunMessageSender());
            Task.Run(() => this.RunReconnectReceiver());
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.isRunning = false;
            this.client.Close();
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public void Send(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.messageSendQueue.Enqueue(message);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.currentMessageNumber = 1;
            this.firstReceiveHandled = false;
            this.firstSendHandled = false;
            this.taskWaitSleepTime = new TimeSpan(0, 0, 0, 0, 100);
            this.messageSendQueue = new Queue<T>();
            this.reconnectPort = this.GetRandomReconnectPort();
        }

        /// <summary>
        /// Gets the random reconnect port between 10000 and 65535.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetRandomReconnectPort()
        {
            return new Random(DateTime.Now.Millisecond).Next(10000, 65535);
        }

        /// <summary>
        /// Runs the message receiver.
        /// </summary>
        private void RunMessageReceiver()
        {
            while (this.isRunning)
            {
                T message = null;

                try
                {
                    IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, this.Port);
                    var data = this.client.Receive(ref remoteEp);
                    message = this.MessageTransformer.TransformFrom(data);
                }
                catch (Exception ex)
                {
                    LogMessage(new LogMessage(ex));
                }

                if (message != null && message.SessionId != null)
                {
                    // first message received from server - session id set
                    if (!this.firstReceiveHandled)
                    {
                        this.firstReceiveHandled = true;
                        this.sessionId = message.SessionId.Value;
                    }

                    if (message.SessionId.Value.Equals(this.sessionId))
                    {
                        //message transmittion control
                        if (message.MessageNr == currentMessageNumber + 1)
                        {
                            currentMessageNumber++;
                            this.OnMessageReceived(new MessageReceivedEventArgs<T>(message, this.sessionId));
                        }
                        else
                        {
                            LogMessage(new LogMessage(LogMessageType.Debug, "Invalid message control number."));
                        }
                    }
                    else
                    {
                        LogMessage(new LogMessage(LogMessageType.Debug, "Invalid session id received."));
                    }
                }
            }
        }

        /// <summary>
        /// Runs the message sender.
        /// </summary>
        private void RunMessageSender()
        {
            while (isRunning)
            {
                while (this.messageSendQueue.Count == 0)
                {
                    Thread.Sleep(this.taskWaitSleepTime);
                }

                T message = this.messageSendQueue.Dequeue();

                if (!this.firstSendHandled)
                {
                    message.SessionId = null;
                    message.MessageNr = 1;
                    this.firstSendHandled = true;
                }
                else
                {
                    message.MessageNr = currentMessageNumber + 1;
                    currentMessageNumber++;
                }

                // reconnect endpoint
                message.ReconnectEndpoint = new IpConnectionEndpoint()
                {
                    Hostname = Dns.GetHostName(),
                    Port = this.reconnectPort
                };

                try
                {
                    byte[] buffer = this.MessageTransformer.TransformTo(message);
                    client.Send(buffer, buffer.Length);
                }
                catch (Exception ex)
                {
                    LogMessage(new LogMessage(ex));
                }
            }
        }

        /// <summary>
        /// Runs the reconnect listener.
        /// </summary>
        private void RunReconnectReceiver()
        {
            try
            {
                this.reconnectReceiver = new UdpClient(this.reconnectPort);
            }
            catch (Exception ex)
            {
                LogMessage(new LogMessage(ex));
            }

            while (this.isRunning)
            {
                IPEndPoint remoteEp = null;
                var data = this.reconnectReceiver.Receive(ref remoteEp);

                T message = null;

                try
                {
                    message = this.MessageTransformer.TransformFrom(data);
                }
                catch (Exception ex)
                {
                    LogMessage(new LogMessage(ex));
                }

                if (message != null && message.SessionId != null && message.SessionId.Value.Equals(this.sessionId))
                {
                    this.OnMessageReceived(new MessageReceivedEventArgs<T>(message, this.sessionId));
                }
                else
                {
                    LogMessage(new LogMessage(LogMessageType.Debug, "Wrong Session ID"));
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MessageReceived"/> event.
        /// </summary>
        /// <param name="args">
        /// The <see cref="MessageReceivedEventArgs{T}"/> instance containing the event data.
        /// </param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> args)
        {
            // ReSharper disable once UseNullPropagation
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, args);
            }
        }

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

    }
}
