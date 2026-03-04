// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Infrastructure.Entities
{
    public class DashboardItem
    {
        public Guid Id { get; private set; }

        public Guid? UserId { get; private set; }

        public Guid? CustomerId { get; private set; }

        public int TemplateType { get; private set; }

        public ICollection<DashboardWidget>? Widgets { get; set; }

        public DashboardItem(Guid id, Guid? userId, Guid? customerId, int templateType)
        {
            Id = id;
            UserId = userId;
            CustomerId = customerId;
            TemplateType = templateType;
        }

        public void Update(Guid? userId, Guid? customerId, int templateType)
        {
            UserId = userId;
            CustomerId = customerId;
            TemplateType = templateType;
        }
    }
}