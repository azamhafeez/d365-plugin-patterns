using System;
using DataverseServerSideExamples.Helpers;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Plugins
{
    public sealed class DeletePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            if (!string.Equals(context.MessageName, "Delete", StringComparison.OrdinalIgnoreCase) || context.Stage != 20)
            {
                return;
            }

            EntityReference target;
            if (!context.InputParameters.TryGetValue("Target", out var value) || !(value is EntityReference) ||
                (target = (EntityReference)value).LogicalName != "new_examplerecord")
            {
                return;
            }

            var preImage = DataverseHelpers.GetPreImage(context, "PreImage");
            if (preImage == null)
            {
                throw new InvalidPluginExecutionException("The record cannot be deleted because required validation information is unavailable.");
            }

            if (preImage.GetAttributeValue<bool>("new_isprocessed"))
            {
                throw new InvalidPluginExecutionException("Processed records cannot be deleted.");
            }
        }
    }
}
