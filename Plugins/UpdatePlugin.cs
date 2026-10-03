using System;
using DataverseServerSideExamples.Helpers;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Plugins
{
    public sealed class UpdatePlugin : IPlugin
    {
        private const string PreImageAlias = "PreImage";

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            if (!string.Equals(context.MessageName, "Update", StringComparison.OrdinalIgnoreCase) || context.Stage != 20)
            {
                return;
            }

            Entity target;
            if (!context.InputParameters.TryGetValue("Target", out var value) || !(value is Entity) ||
                (target = (Entity)value).LogicalName != "new_examplerecord" || !target.Contains("new_status"))
            {
                return;
            }

            var preImage = DataverseHelpers.GetPreImage(context, PreImageAlias);
            if (preImage == null)
            {
                throw new InvalidPluginExecutionException("The update could not be evaluated because required context is unavailable.");
            }

            var previousStatus = DataverseHelpers.GetOptionSetValue(preImage, "new_status");
            var currentStatus = DataverseHelpers.GetOptionSetValue(target, "new_status");
            if (previousStatus == currentStatus)
            {
                return;
            }

            // Update Target in PreOperation rather than calling Update on the same record.
            target["new_isprocessed"] = currentStatus.HasValue;
        }
    }
}
