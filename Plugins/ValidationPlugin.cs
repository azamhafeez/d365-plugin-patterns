using System;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Plugins
{
    public sealed class ValidationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            if (!string.Equals(context.MessageName, "Create", StringComparison.OrdinalIgnoreCase) || context.Stage != 10)
            {
                return;
            }

            if (!context.InputParameters.TryGetValue("Target", out var value) || !(value is Entity target) ||
                target.LogicalName != "new_examplerecord")
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(target.GetAttributeValue<string>("new_name")))
            {
                throw new InvalidPluginExecutionException("Please provide a name before saving the record.");
            }

            if (target.GetAttributeValue<bool>("new_isprocessed") &&
                target.GetAttributeValue<EntityReference>("new_relatedrecordid") == null)
            {
                throw new InvalidPluginExecutionException("Please select a related record before marking this record as processed.");
            }
        }
    }
}
