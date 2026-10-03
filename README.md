# Dataverse Server-Side Examples

A focused C# portfolio of server-side extension patterns for Microsoft Dynamics 365 Customer Engagement and Microsoft Dataverse. The examples favor platform-native concepts, small classes, and reviewable code over application-specific architecture. All schema names and business rules are intentionally generic.

> **Development sample:** validate each example in a development or sandbox environment and adapt registration, schema names, security, and behavior before production use. This repository contains no deployment automation or environment configuration.

## Skills demonstrated

- Defensive `IPlugin` implementations for Create, Update, Delete, validation, images, shared variables, and tracing.
- Pipeline-aware changes that avoid an unnecessary `IOrganizationService.Update` or `Retrieve`.
- A compact `CodeActivity`, a custom process action handler, and a minimal `QueryExpression`.
- Friendly validation errors, least-data queries, safe type checks, and reusable attribute/image helpers.

## Structure

| Path | Purpose |
| --- | --- |
| `Plugins/` | Message-based plug-in examples, each focused on one platform concept. |
| `CustomWorkflowActivities/` | A simple input/calculation/output workflow activity. |
| `CustomActions/` | A handler for a generic custom process message. |
| `QueryExamples/` | A concise `QueryExpression` read example. |
| `Helpers/` | Small helpers for typed values, references, money, choices, GUIDs, and images. |

The optional SDK-style project targets .NET Framework 4.6.2, a commonly supported target for these SDK assemblies. Build and platform compatibility should always be checked against the target Dataverse environment.

## Plug-in execution pipeline

| Stage | Number | Typical use |
| --- | ---: | --- |
| **PreValidation** | 10 | Reject invalid requests early, before the core operation and usually before the database transaction. |
| **PreOperation** | 20 | Change the incoming `Target` within the transaction. Changes become part of the core operation without a second update. |
| **PostOperation** | 40 | React after the core operation. Synchronous steps still participate in the transaction; asynchronous steps run later. |

Synchronous steps execute in the caller's request and can return an immediate validation message, so they should be fast. Asynchronous PostOperation steps suit non-immediate follow-up work and do not block the original caller. Depth checks can help diagnose recursion, but correct filtering and avoiding updates to the triggering record are better first-line protections.

### Core context concepts

- **Target:** the input entity for Create/Update, containing only submitted attributes on Update. Delete supplies an `EntityReference`, not an `Entity`. Always verify the message, stage, input type, and logical name.
- **Pre/Post Images:** configured snapshots of selected columns before or after the operation. Images provide known values already in the execution context and are preferable to extra `Retrieve` calls. Keep their column lists minimal. Images are not available for every message/stage combination, so retrieve them safely by alias.
- **Filtering Attributes:** on an Update registration, list only attributes that should trigger the step. This checks whether a column was included in the request, not whether its value changed; compare `Target` against a Pre Image when actual change matters. Do not include the primary key as a filtering attribute.
- **SharedVariables:** a typed key/value collection for passing transient information between steps in the same execution pipeline. Check key existence and type before reading; parent context access may be necessary for nested operations.
- **Tracing:** `ITracingService` is the supported diagnostic channel. Include class/execution metadata such as message, stage, depth, primary entity, and correlation ID, but never trace secrets or sensitive field values.
- **Organization service:** obtain `IOrganizationService` from `IOrganizationServiceFactory` only when a data operation is required. Execute under the intended user, select minimal columns, and avoid redundant reads/writes.

## Registration reference

Aliases and selected image columns must match the code exactly.

| Example | Message | Stage | Mode | Entity | Filtering Attributes | Image |
| --- | --- | --- | --- | --- | --- | --- |
| `ValidationPlugin` | Create | PreValidation | Synchronous | `new_examplerecord` | N/A | None |
| `CreatePlugin` | Create | PreOperation | Synchronous | `new_examplerecord` | N/A | None |
| `UpdatePlugin` | Update | PreOperation | Synchronous | `new_examplerecord` | `new_status` | Pre Image `PreImage`: `new_status` |
| `DeletePlugin` | Delete | PreOperation | Synchronous | `new_examplerecord` | N/A | Pre Image `PreImage`: `new_isprocessed` |
| `PreImageExample` | Update | PreOperation | Synchronous | `new_examplerecord` | `new_status` | Pre Image `PreImage`: `new_status` |
| `PostImageExample` | Update | PostOperation | Synchronous | `new_examplerecord` | `new_isprocessed` | Post Image `PostImage`: `new_isprocessed` |
| `SharedVariablesExample` (writer) | Update | PreOperation | Synchronous | `new_examplerecord` | Choose relevant columns | None |
| `SharedVariablesExample` (reader) | Update | PostOperation | Synchronous | `new_examplerecord` | Same as writer | None |
| `TracingExample` | As needed | Appropriate stage | Synchronous or asynchronous | As needed | Minimal relevant columns | None |

The table is a design reference, not an importable registration file. Register the two `SharedVariablesExample` steps with matching execution order requirements. **Custom Actions** are first defined as custom process messages (or APIs) with request/response parameters, then the handler is registered on that message. **Custom Workflow Activities** are registered as workflow assemblies and selected/configured inside the process designer; neither follows ordinary entity-message registration exactly.

## Additional examples

### Custom Workflow Activity

`CalculateExampleActivity` uses `CodeActivityContext` to access `IWorkflowContext`, tracing, the service factory, and an organization service. Its `InArgument<decimal>` values produce an `OutArgument<decimal>` result. The calculation needs no record access; service creation is present solely to show the standard access pattern.

### Custom Action

`ProcessExampleActionPlugin` checks the placeholder message `new_ProcessExample`, validates `RecordId` and `InputValue`, and returns `ResultMessage` and `Success`. Replace the placeholder message only with a generic schema name created in the target solution. Expected validation errors stay friendly; unexpected technical details go to tracing rather than the user.

### QueryExpression

`QueryExpressionExample` requests only `new_name` and `new_totalamount`, filters on `new_isprocessed`, and limits results with `TopCount`. Real implementations should add deterministic ordering or paging when they need more than a small bounded result.

## Error handling, performance, and security

- Throw `InvalidPluginExecutionException` with an actionable, nontechnical message for expected business validation. Trace unexpected exceptions and expose only a generic user message.
- Keep synchronous execution short. Use filtering attributes, narrow image columns, minimal `ColumnSet` values, bounded queries, and no unnecessary `Retrieve`/`Update` calls.
- Never assume an attribute is present. Check context collections, aliases, types, nulls, and empty GUIDs.
- The calling or impersonated user's privileges apply to service operations. Follow least privilege, confirm ownership/team behavior, and avoid elevating execution without a documented reason.
- Never log secrets, personal data, confidential values, or full payloads. Correlation IDs and execution metadata are generally safer diagnostics.
- Use secure/unsecure step configuration for appropriate non-secret configuration; secrets belong in an approved secret-management mechanism, not source code or registration text.

## Local build

With a compatible .NET SDK and package access:

```bash
dotnet restore
dotnet build --no-restore
```

The resulting assembly still requires signing and registration/configuration appropriate to the target sandbox. No environment URL, credentials, or deployment pipeline is included.
