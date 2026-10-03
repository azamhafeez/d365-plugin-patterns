using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace DataverseServerSideExamples.CustomWorkflowActivities
{
    public sealed class CalculateExampleActivity : CodeActivity
    {
        [Input("Input Amount")]
        [RequiredArgument]
        public InArgument<decimal> InputAmount { get; set; }

        [Input("Multiplier")]
        [Default("1")]
        public InArgument<decimal> Multiplier { get; set; }

        [Output("Calculated Amount")]
        public OutArgument<decimal> CalculatedAmount { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            var workflowContext = executionContext.GetExtension<IWorkflowContext>();
            var tracing = executionContext.GetExtension<ITracingService>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            var result = InputAmount.Get(executionContext) * Multiplier.Get(executionContext);
            CalculatedAmount.Set(executionContext, result);

            // The service shows the supported access pattern; this calculation needs no data operation.
            tracing.Trace("CalculateExampleActivity completed for workflow correlation {0}.", workflowContext.CorrelationId);
            GC.KeepAlive(service);
        }
    }
}
