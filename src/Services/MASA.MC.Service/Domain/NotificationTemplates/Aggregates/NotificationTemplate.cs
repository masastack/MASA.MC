using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
using MASA.MC.Contracts.Admin.Enums.NotificationTemplates;

namespace MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates
{
    public class NotificationTemplate : AuditAggregateRoot<Guid, Guid?>
    {
        public virtual Guid ChannelId { get; protected set; }
        public virtual string Code { get; protected set; }
        public virtual string DisplayName { get; protected set; }
        public virtual string Content { get; protected set; }
        public virtual string TemplateId { get; protected set; }
        public virtual NotificationTemplateStatus Status { get; protected set; }
        public virtual bool IsStatic { get; protected set; }
        public virtual ICollection<NotificationTemplateItem> Items { get; protected set; }
        public NotificationTemplate()
        {
        }
        public NotificationTemplate(
            string code,
            string displayName,
            string content,
            List<NotificationTemplateItem> items,
            NotificationTemplateStatus status= NotificationTemplateStatus.Normal,
            bool isStatic = true)
        {
            Code = code;
            Status= status;
            IsStatic = isStatic;

            SetContent(displayName, content);

            Items = items ?? new List<NotificationTemplateItem>();
        }
        public void AddOrUpdateItem(string key, string displayText, string description, bool isStatic = false)
        {
            var existingItem = Items.SingleOrDefault(item => item.Key == key);

            if (existingItem == null)
            {
                Items.Add(new NotificationTemplateItem(Id, key, displayText, description, isStatic));
            }
            else
            {
                existingItem.SetContent(displayText, description);
            }
        }

        public void SetContent(string displayName, string content)
        {
            DisplayName = displayName;
            Content = content;
        }
    }
}
