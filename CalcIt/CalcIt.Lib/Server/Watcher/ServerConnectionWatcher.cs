using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.CommandExecution;
using CalcIt.Lib.NetworkAccess;
using CalcIt.Protocol.Server;

namespace CalcIt.Lib.Server.Watcher
{
    public class ServerConnectionWatcher
    {

        public ServerConnectionWatcher(ServerConfiguration configuration)
        {
            this.Configuration = configuration;

            IntializeServerConnections(); 
        }

        public List<INetworkAccess<CalcItServerMessage>> ServerConnections { get; private set; }

        public CommandExecutor<CalcItServerMessage> ServerCommandExecutor { get; set; }

        public ServerConfiguration Configuration { get; private set; }

        public event EventHandler AllConnectionsLost;

        public event EventHandler ServerStatesSynced;

        private void IntializeServerConnections()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void RunWatcher()
        {
            throw new System.NotImplementedException();
        }
    }
}
