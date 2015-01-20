// -----------------------------------------------------------------------
// <copyright file="IConfigurationLoader.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - IConfigurationLoader.cs</summary>
// -----------------------------------------------------------------------

namespace CalcIt.Lib.Server.Configuration
{
    /// <summary>
    /// The Configuration loader interface.
    /// </summary>
    public interface IConfigurationManager<T>
    {
        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns></returns>
        T LoadConfiguration();

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        void SaveConfiguration(T configuration);
    }
}