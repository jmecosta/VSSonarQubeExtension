// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginLoader.cs" company="Copyright � 2014 Tekla Corporation. Tekla is a Trimble Company">
//     Copyright (C) 2013 [Jorge Costa, Jorge.Costa@tekla.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// --------------------------------------------------------------------------------------------------------------------
namespace VSSonarPlugins
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    ///     The local analyser.
    /// </summary>
    public class PluginLoader : IPluginLoader
    {
        #region Fields

        /// <summary>
        ///     The error data.
        /// </summary>
        private string errorData;

        /// <summary>
        ///     The plugins.
        /// </summary>
        [ImportMany]
        private IEnumerable<Lazy<IPlugin>> plugins;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get error data.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetErrorData()
        {
            return this.errorData;
        }

        /// <summary>
        /// The load plugins from folder.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ReadOnlyCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        public ReadOnlyCollection<IPlugin> LoadPluginsFromFolder(string folder)
        {
            var sensors = new AggregateCatalog();
            sensors.Catalogs.Add(new AssemblyCatalog(typeof(PluginLoader).Assembly));
            sensors.Catalogs.Add(new DirectoryCatalog(folder));
            var compositonContainer = new CompositionContainer(sensors);
            try
            {
                compositonContainer.ComposeParts(this);
                return new ReadOnlyCollection<IPlugin>(this.plugins.Select(plugin => plugin.Value).ToList());
            }
            catch (Exception compositionException)
            {
                if (compositionException is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = compositionException as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    foreach (var loaderException in loaderExceptions)
                    {
                        Debug.WriteLine(loaderException.Message + " " + loaderException.StackTrace);    
                    }
                    
                }

                this.errorData = compositionException.Message + " " + compositionException.StackTrace;
                Debug.WriteLine(compositionException.ToString());
                return null;
            }
        }

        #endregion
    }
}