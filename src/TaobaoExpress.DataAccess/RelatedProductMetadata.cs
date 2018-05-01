namespace TaobaoExpress.DataAccess
{
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(RelatedProductMetadata))]
    public partial class RelatedProduct
    {
    }

    public class RelatedProductMetadata
    {
        [Display(Name = "Related Product")]
        public long RelatedProductId { get; set; }

        [Display(Name = "Is Substitute Product?")]
        public bool IsSubstitute { get; set; }
    }
}
