using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalcIt.Lib.Server.Configuration;
using CalcIt.Lib.Server.Watcher;

namespace CalcIt.Lib.Server
{
    public class ServerManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerManager"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <exception cref="System.ArgumentNullException">configurationManager</exception>
        public ServerManager(IConfigurationManager<ServerConfiguration> configurationManager)
        {
            if (configurationManager == null)
            {
                throw new ArgumentNullException("configurationManager");
            }

            this.ConfigurationManager = configurationManager; 
            InitializeConfiguration();
        }

        private void InitializeConfiguration()
        {
            try
            {
                Configuration = ConfigurationManager.LoadConfiguration();
            }
            catch(Exception ex)
            {
                
            }

            
        }

        public IConfigurationManager<ServerConfiguration> ConfigurationManager { get; private set; }

        public ServerConfiguration Configuration { get; private set; }

        public List<GameClient> GameClients { get; private set; }

        public int StartTime { get; private set; }

        public int IsActiveServer { get; private set; }

        public void AddClient()
        {
            throw new System.NotImplementedException();
        }

        public void StartClientGame()
        {
            throw new System.NotImplementedException();
        }

        public void HandleAnswer()
        {
            throw new System.NotImplementedException();
        }

        public void HandleHighscoreRequest()
        {
            throw new System.NotImplementedException();
        }
    }
}
