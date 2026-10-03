using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DataverseServerSideExamples.QueryExamples
{
    public static class QueryExpressionExample
    {
        public static EntityCollection GetUnprocessedRecords(IOrganizationService service)
        {
            var query = new QueryExpression("new_examplerecord")
            {
                ColumnSet = new ColumnSet("new_name", "new_totalamount"),
                TopCount = 50
            };
            query.Criteria.AddCondition(new ConditionExpression("new_isprocessed", ConditionOperator.Equal, false));

            return service.RetrieveMultiple(query);
        }
    }
}
