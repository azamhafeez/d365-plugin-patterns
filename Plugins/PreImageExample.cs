using System;
using DataverseServerSideExamples.Helpers;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Plugins
{
    public sealed class PreImageExample : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if (!string.Equals(context.MessageName, "Update", StringComparison.OrdinalIgnoreCase)) return;

            var image = DataverseHelpers.GetPreImage(context, "PreImage");
            if (image == null)
            {
                tracing.Trace("PreImageExample: configured PreImage was not available.");
                return;
            }

            tracing.Trace("PreImageExample: previous status was available={0}.", image.Contains("new_status"));
        }
    }
}
