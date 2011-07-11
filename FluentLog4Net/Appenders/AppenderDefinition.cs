﻿using System.Collections.Generic;

using FluentLog4Net.Configuration;
using FluentLog4Net.Helpers;

using log4net.Appender;
using log4net.Core;

namespace FluentLog4Net.Appenders
{
    /// <summary>
    /// The base class for all fluent appender definitions.
    /// </summary>
    public abstract class AppenderDefinition<T> : IAppenderDefinition where T : AppenderDefinition<T>
    {
        private Level _threshold;
        private readonly LayoutConfiguration<T>  _layout;
        private readonly List<FilterConfiguration<T>> _filters;
        private readonly ErrorHandlerConfiguration<T> _errorHandler;

        protected AppenderDefinition()
        {
            _layout = new LayoutConfiguration<T>((T)this);
            _filters = new List<FilterConfiguration<T>>();
            _errorHandler = new ErrorHandlerConfiguration<T>((T)this);
        }

        public T At(Level threshold)
        {
            _threshold = threshold;
            return (T)this;
        }

        public LayoutConfiguration<T> Format
        {
            get { return _layout; }
        }

        public FilterConfiguration<T> Apply
        {
            get { return _filters.AddItem(new FilterConfiguration<T>((T)this)); }
        }

        public ErrorHandlerConfiguration<T> HandleErrors
        {
            get { return _errorHandler; }
        }

        protected abstract AppenderSkeleton CreateAppender();

        IAppender IAppenderDefinition.CreateAppender()
        {
            var appender = CreateAppender();
            appender.Threshold = _threshold;

            _layout.ApplyTo(appender);
            
            foreach(var filter in _filters)
                filter.ApplyTo(appender);

            _errorHandler.ApplyTo(appender);

            return appender;
        }
    }
}