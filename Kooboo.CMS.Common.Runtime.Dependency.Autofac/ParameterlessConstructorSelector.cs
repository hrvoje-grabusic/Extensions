using Autofac.Core.Activators.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.CMS.Common.Runtime.Dependency.Autofac
{
    internal class ParameterlessConstructorSelector : IConstructorSelector
    {
        #region Implementation of IConstructorSelector

        /// <summary>
        /// Selects the best constructor from the available constructors.
        /// </summary>
        /// <param name="constructorBindings">Available constructors.</param>
        /// <returns>
        /// The best constructor.
        /// </returns>
        public ConstructorParameterBinding SelectConstructorBinding(ConstructorParameterBinding[] constructorBindings)
        {
            return constructorBindings.First();
        }

        #endregion
    }
}
