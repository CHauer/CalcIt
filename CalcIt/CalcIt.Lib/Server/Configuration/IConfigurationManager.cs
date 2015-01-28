// -----------------------------------------------------------------------
// <copyright file="IConfigurationManager.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - IConfigurationManager.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Server.Configuration
{
    /// <summary>
    /// The Configuration loader interface.
    /// </summary>
    /// <typeparam name="T">
    /// Type of class and ICalcItSession implemented.
    /// </typeparam>
    public interface IConfigurationManager<T>
    {
        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="T"/> type.
        /// </returns>
        T LoadConfiguration();

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        void SaveConfiguration(T configuration);
    }
}