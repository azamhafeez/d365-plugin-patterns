using System;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Plugins
{
    public sealed class CreatePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracing.Trace("CreatePlugin: message={0}, stage={1}", context.MessageName, context.Stage);

            if (!string.Equals(context.MessageName, "Create", StringComparison.OrdinalIgnoreCase) || context.Stage != 20)
            {
                return;
            }

            Entity target;
            if (!context.InputParameters.TryGetValue("Target", out var value) || !(value is Entity) ||
                (target = (Entity)value).LogicalName != "new_examplerecord")
            {
                return;
            }

            if (!target.Attributes.Contains("new_isprocessed"))
            {
                // PreOperation changes are included in the platform create; no Update call is needed.
                target["new_isprocessed"] = false;
            }
        }
    }
}
