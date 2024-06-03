using Domain.Common;

namespace Domain.Entities
{
    public class SortByItem: BaseEntity
    {
        public string ItemName { get; set; }
        public string FieldName { get; set; }
    }
}
