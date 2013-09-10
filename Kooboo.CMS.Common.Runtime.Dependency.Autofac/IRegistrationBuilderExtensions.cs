#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Autofac;
using Autofac.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.CMS.Common.Runtime.Dependency.Autofac
{
    public static class IRegistrationBuilderExtensions
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> LifeStyle<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> rb, ComponentLifeStyle lifeStyle)
        {
            switch (lifeStyle)
            {
                case ComponentLifeStyle.Singleton:
                    rb = rb.SingleInstance();
                    break;
                case ComponentLifeStyle.InRequestScope:
                    rb = rb.InstancePerMatchingLifetimeScope("httpRequest");
                    break;
                case ComponentLifeStyle.Transient:
                default:
                    rb = rb.InstancePerDependency();
                    break;
            }

            return rb;
        }
        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TRegistrationStyle> WithParamterEx<TLimit, TReflectionActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TReflectionActivatorData, TRegistrationStyle> rb
            , params Parameter[] parameters)
            where TReflectionActivatorData : ReflectionActivatorData
        {
            if (parameters != null)
            {
                rb = rb.OnPreparing((arg) => arg.Parameters = parameters.Select(it => new NamedParameter(it.Name, it.ValueCallback())));
            }
            return rb;
        }
    }
}
