using System;
using Microsoft.Xrm.Sdk;

namespace DataverseServerSideExamples.Helpers
{
    /// <summary>Small, defensive helpers shared by the examples.</summary>
    public static class DataverseHelpers
    {
        public static T GetAttributeValueOrDefault<T>(Entity entity, string attributeName, T defaultValue = default(T))
        {
            if (entity == null || string.IsNullOrWhiteSpace(attributeName))
            {
                return defaultValue;
            }

            T value;
            return entity.TryGetAttribute(attributeName, out value) ? value : defaultValue;
        }

        public static bool TryGetAttribute<T>(this Entity entity, string attributeName, out T value)
        {
            value = default(T);
            object rawValue;
            if (entity == null || !entity.Attributes.TryGetValue(attributeName, out rawValue) || !(rawValue is T))
            {
                return false;
            }

            value = (T)rawValue;
            return true;
        }

        public static Guid? GetEntityReferenceId(Entity entity, string attributeName)
        {
            EntityReference reference;
            return entity.TryGetAttribute(attributeName, out reference) && reference.Id != Guid.Empty
                ? reference.Id
                : (Guid?)null;
        }

        public static decimal? GetMoneyValue(Entity entity, string attributeName)
        {
            Money money;
            return entity.TryGetAttribute(attributeName, out money) ? money.Value : (decimal?)null;
        }

        public static int? GetOptionSetValue(Entity entity, string attributeName)
        {
            OptionSetValue option;
            return entity.TryGetAttribute(attributeName, out option) ? option.Value : (int?)null;
        }

        public static Guid? NormalizeGuid(Guid? value)
        {
            return value.HasValue && value.Value != Guid.Empty ? value : null;
        }

        public static Entity GetPreImage(IPluginExecutionContext context, string alias)
        {
            return GetImage(context == null ? null : context.PreEntityImages, alias);
        }

        public static Entity GetPostImage(IPluginExecutionContext context, string alias)
        {
            return GetImage(context == null ? null : context.PostEntityImages, alias);
        }

        private static Entity GetImage(EntityImageCollection images, string alias)
        {
            Entity image;
            return images != null && !string.IsNullOrWhiteSpace(alias) && images.TryGetValue(alias, out image)
                ? image
                : null;
        }
    }
}
