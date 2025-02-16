namespace SV21T1020123.DomainModels
{
    public class ProductPhoto
    {
        public long PhotoID { get; set; }
        public int ProductID { get; set; }
        public string Photo { get; set; } = string.Empty;
        public int DisplayOrder { get; set;}
        public string Description { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
    }
}
