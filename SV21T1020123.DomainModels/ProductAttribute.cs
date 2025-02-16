namespace SV21T1020123.DomainModels
{
    public class ProductAttribute
    {
        public long AttributeID { get; set; }
        public int ProductID { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public string AttributeValue { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
