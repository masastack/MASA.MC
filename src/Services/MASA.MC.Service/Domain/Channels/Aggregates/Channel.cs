using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
using MASA.MC.Contracts.Admin.Enums.Channels;
using MASA.MC.Service.Admin.Infrastructure.Extensions;

namespace MASA.MC.Service.Admin.Domain.Channels.Aggregates
{
    public class Channel : AuditAggregateRoot<Guid, Guid?>
    {
        public virtual string DisplayName { get; protected set; } = string.Empty;

        public virtual ChannelType Type { get; protected set; }

        public virtual bool IsStatic { get; protected set; }

        public virtual Dictionary<string, string> ExtraProperties { get; protected set; } = new();

        public Channel()
        {
        }

        public Channel(
            string displayName,
            ChannelType type,
            Dictionary<string, string> extraProperties,
            bool isStatic = true)
        {
            DisplayName = displayName;
            Type = type;
            IsStatic = isStatic;
            foreach (var p in extraProperties)
            {
                SetDataValue(p.Key, p.Value);
            }
        }

        public string GetDataValue(string name)
        {
            return ExtraProperties?.GetOrDefault(name)??string.Empty;
        }

        public void SetDataValue(string name, string value)
        {
            ExtraProperties[name] = value;
        }
    }
    
}
