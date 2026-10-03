using System;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Plugins
{
    /// <summary>Register in PreOperation to write and PostOperation to read.</summary>
    public sealed class SharedVariablesExample : IPlugin
    {
        private const string VariableName = "ExampleWasValidated";

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if (!string.Equals(context.MessageName, "Update", StringComparison.OrdinalIgnoreCase)) return;

            if (context.Stage == 20)
            {
                context.SharedVariables[VariableName] = true;
                return;
            }

            object value;
            if (context.Stage == 40 && context.SharedVariables.TryGetValue(VariableName, out value) && value is bool)
            {
                tracing.Trace("SharedVariablesExample: validation result was available={0}.", (bool)value);
            }
        }
    }
}
