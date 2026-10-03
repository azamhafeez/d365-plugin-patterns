using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace DataverseServerSideExamples.CustomWorkflowActivities
{
    public sealed class CalculateExampleActivity : CodeActivity
    {
        [Input("Example Record")]
        [RequiredArgument]
        [ReferenceTarget("new_examplerecord")]
        public InArgument<EntityReference> ExampleRecord { get; set; }

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

            var recordReference = ExampleRecord.Get(executionContext);
            if (recordReference == null || recordReference.Id == Guid.Empty ||
                recordReference.LogicalName != "new_examplerecord")
            {
                throw new InvalidPluginExecutionException("Please select a valid example record.");
            }

            var record = service.Retrieve(
                recordReference.LogicalName,
                recordReference.Id,
                new ColumnSet("new_totalamount"));
            var totalAmount = record.GetAttributeValue<Money>("new_totalamount")?.Value ?? 0m;
            var result = totalAmount * Multiplier.Get(executionContext);
            CalculatedAmount.Set(executionContext, result);

            tracing.Trace("CalculateExampleActivity completed for workflow correlation {0}.", workflowContext.CorrelationId);
        }
    }
}
