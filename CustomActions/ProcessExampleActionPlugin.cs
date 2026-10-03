using System;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.CustomActions
{
    public sealed class ProcessExampleActionPlugin : IPlugin
    {
        // Replace this placeholder with the unique name of the unbound custom process message.
        private const string MessageName = "new_ProcessExample";

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (!string.Equals(context.MessageName, MessageName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            tracing.Trace("ProcessExampleActionPlugin started. CorrelationId={0}", context.CorrelationId);
            try
            {
                if (!context.InputParameters.TryGetValue("RecordId", out var recordIdValue) ||
                    !(recordIdValue is Guid recordId) || recordId == Guid.Empty)
                {
                    throw new InvalidPluginExecutionException("RecordId must contain a valid record identifier.");
                }

                if (!context.InputParameters.TryGetValue("InputValue", out var inputValueObject) ||
                    !(inputValueObject is string inputValue) || string.IsNullOrWhiteSpace(inputValue))
                {
                    throw new InvalidPluginExecutionException("InputValue is required.");
                }

                // This transformation needs no data operation, so no IOrganizationService is created.
                context.OutputParameters["ResultMessage"] = "The request was processed successfully.";
                context.OutputParameters["Success"] = true;
                tracing.Trace("ProcessExampleActionPlugin completed for correlation {0}.", context.CorrelationId);
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception exception)
            {
                tracing.Trace("ProcessExampleActionPlugin failed: {0}", exception);
                throw new InvalidPluginExecutionException("The request could not be processed. Please try again or contact an administrator.");
            }
        }
    }
}
