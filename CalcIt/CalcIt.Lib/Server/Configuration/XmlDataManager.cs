// -----------------------------------------------------------------------
// <copyright file="XmlDataManager.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.Lib - XmlDataManager.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.Lib.Server.Configuration
{
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// The  XmlSerializer - implements ConfigurationSaver and Loader interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class XmlConfigurationSerializer<T> : IConfigurationManager<T>
        where T : class
    {
        /// <summary>
        /// The serializer.
        /// </summary>
        private XmlSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlConfigurationSerializer{T}" /> class.
        /// </summary>
        public XmlConfigurationSerializer()
        {
            this.serializer = new XmlSerializer(typeof(T));
        }

        /// <summary>
        /// Gets or sets the configuration file.
        /// </summary>
        /// <value>
        /// The configuration file.
        /// </value>
        public string ConfigurationFile { get; set; }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T LoadConfiguration()
        {
            using (var file = File.OpenRead(this.ConfigurationFile))
            {
                return (T)this.serializer.Deserialize(file);
            }
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public void SaveConfiguration(T configuration)
        {
            using (var file = File.Open(this.ConfigurationFile, FileMode.Create, FileAccess.Write))
            {
                this.serializer.Serialize(file, configuration);
                file.Flush();
            }
        }
    }
}