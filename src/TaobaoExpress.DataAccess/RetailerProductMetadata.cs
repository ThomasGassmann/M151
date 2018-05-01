namespace TaobaoExpress.DataAccess
{
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(RetailerProductMetadata))]
    public partial class RetailerProduct
    {
    }

    public class RetailerProductMetadata
    {
        [Display(Name = "Product")]
        public long ProductId { get; set; }

        [Display(Name = "Retailer")]
        public long RetailerId { get; set; }

        [Display(Name = "Is Manufacturer?")]
        [Required]
        public bool IsManufacturer { get; set; }
    }
}
