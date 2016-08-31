﻿using System;
using LightInject;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration;
using Umbraco.Core.Dictionary;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Plugins;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Strings;
using Umbraco.Core.Sync;
using Umbraco.Core._Legacy.PackageActions;

namespace Umbraco.Core.DependencyInjection
{
    // this class is here to support the transition from singletons and resolvers to injection,
    // by providing a static access to singleton services - it is initialized once with a service
    // container, in CoreBootManager.
    // obviously, this is some sort of service locator anti-pattern. ideally, it should not exist.
    // practically... time will tell.
    public static class Current
    {
        private static ServiceContainer _container;

        internal static ServiceContainer Container
        {
            get
            {
                if (_container == null) throw new Exception("No container has been set.");
                return _container;
            }
            set // ok to set - don't be stupid
            {
                if (_container != null) throw new Exception("A container has already been set.");
                _container = value;
            }
        }

        internal static bool HasContainer => _container != null;

        // for UNIT TESTS exclusively!
        internal static void Reset()
        {
            _container = null;

            _shortStringHelper = null;
            _logger = null;
            _profiler = null;
            _profilingLogger = null;
            _applicationContext = null;

            Resetted?.Invoke(null, EventArgs.Empty);
        }

        internal static event EventHandler Resetted;

        #region Getters

        // fixme - refactor
        // some of our tests want to *set* the current application context and bypass the container
        // so for the time being we support it, however we should fix our tests

        private static ApplicationContext _applicationContext;

        public static ApplicationContext ApplicationContext
        {
            get { return _applicationContext ?? (_applicationContext = Container.GetInstance<ApplicationContext>()); }
            set { _applicationContext = value; }
        }

        public static PluginManager PluginManager
            => Container.GetInstance<PluginManager>();

        public static UrlSegmentProviderCollection UrlSegmentProviders
            => Container.GetInstance<UrlSegmentProviderCollection>();

        public static CacheRefresherCollection CacheRefreshers
            => Container.GetInstance<CacheRefresherCollection>();

        public static PropertyEditorCollection PropertyEditors
            => Container.GetInstance<PropertyEditorCollection>();

        public static ParameterEditorCollection ParameterEditors
            => Container.GetInstance<ParameterEditorCollection>();

        internal static ValidatorCollection Validators
            => Container.GetInstance<ValidatorCollection>();

        internal static PackageActionCollection PackageActions
            => Container.GetInstance<PackageActionCollection>();

        internal static PropertyValueConverterCollection PropertyValueConverters
            => Container.GetInstance<PropertyValueConverterCollection>();

        internal static IPublishedContentModelFactory PublishedContentModelFactory
            => Container.GetInstance<IPublishedContentModelFactory>();

        public static IServerMessenger ServerMessenger
            => Container.GetInstance<IServerMessenger>();

        public static IServerRegistrar ServerRegistrar
            => Container.GetInstance<IServerRegistrar>();

        public static ICultureDictionaryFactory CultureDictionaryFactory
            => Container.GetInstance<ICultureDictionaryFactory>();

        // fixme - refactor
        // we don't want Umbraco to die because the container has not been properly initialized,
        // for some too-important things such as IShortStringHelper or loggers, so if it's not
        // registered we setup a default one. We should really refactor our tests so that it does
        // not happen, but hey...

        private static IShortStringHelper _shortStringHelper;

        public static IShortStringHelper ShortStringHelper
            => _shortStringHelper ?? (_shortStringHelper = _container?.TryGetInstance<IShortStringHelper>() 
                ?? new DefaultShortStringHelper(new DefaultShortStringHelperConfig().WithDefault(UmbracoConfig.For.UmbracoSettings())));

        private static ILogger _logger;
        private static IProfiler _profiler;
        private static ProfilingLogger _profilingLogger;

        public static ILogger Logger
            => _logger ?? (_logger = _container?.TryGetInstance<ILogger>() 
                ?? new DebugDiagnosticsLogger());

        public static IProfiler Profiler
            => _profiler ?? (_profiler = _container?.TryGetInstance<IProfiler>() 
                ?? new LogProfiler(Logger));

        public static ProfilingLogger ProfilingLogger
            => _profilingLogger ?? (_profilingLogger = _container?.TryGetInstance<ProfilingLogger>())
               ?? new ProfilingLogger(Logger, Profiler);

        #endregion
    }
}
