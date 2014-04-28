﻿using System;
using System.Threading.Tasks;
using Nimbus.DependencyResolution;
using Nimbus.Handlers;
using Nimbus.Interceptors.Inbound;
using Nimbus.Logger;

namespace Nimbus.Infrastructure.Events
{
    internal class CompetingEventMessageDispatcher : EventMessageDispatcher
    {
        public CompetingEventMessageDispatcher(IDependencyResolver dependencyResolver,
                                               IBrokeredMessageFactory brokeredMessageFactory,
                                               IInboundInterceptorFactory inboundInterceptorFactory,
                                               Type handlerType,
                                               IClock clock,
                                               Type eventType)
            : base(dependencyResolver, brokeredMessageFactory, inboundInterceptorFactory, handlerType, clock, eventType, new NullLogger())
        {
        }

        protected override object CreateHandlerFromScope<TBusEvent>(IDependencyResolverScope scope, TBusEvent busEvent)
        {
            var handler = scope.Resolve<IHandleCompetingEvent<TBusEvent>>(HandlerType.FullName);
            return handler;
        }

        protected override Task DispatchToHandleMethod<TBusEvent>(TBusEvent busEvent, object handler)
        {
            var genericHandler = (IHandleCompetingEvent<TBusEvent>) handler;
            var handlerTask = genericHandler.Handle(busEvent);
            return handlerTask;
        }
    }
}