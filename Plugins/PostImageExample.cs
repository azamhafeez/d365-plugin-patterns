using System;
using DataverseServerSideExamples.Helpers;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Plugins
{
    public sealed class PostImageExample : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if (!string.Equals(context.MessageName, "Update", StringComparison.OrdinalIgnoreCase) || context.Stage != 40) return;

            var image = DataverseHelpers.GetPostImage(context, "PostImage");
            if (image == null)
            {
                tracing.Trace("PostImageExample: configured PostImage was not available.");
                return;
            }

            tracing.Trace("PostImageExample: resulting processed flag was available={0}.", image.Contains("new_isprocessed"));
        }
    }
}
