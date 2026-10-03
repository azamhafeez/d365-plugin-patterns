using System;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Plugins
{
    public sealed class TracingExample : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracing.Trace(
                "TracingExample: message={0}; stage={1}; depth={2}; primaryEntity={3}; correlationId={4}",
                context.MessageName, context.Stage, context.Depth, context.PrimaryEntityName, context.CorrelationId);

            // Trace execution metadata only; do not include field values or other sensitive data.
        }
    }
}
