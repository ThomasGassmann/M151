namespace TaobaoExpress.DataAccess
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ProductMetadata))]
    public partial class Product : ICreated, IUpdated, IIdentifiable
    {
    }

    public class ProductMetadata
    {
        [MinLength(5)]
        [Required]
        [Display(Name = "Product name")]
        public string Name { get; set; }

        [Display(Name = "Product image")]
        public byte[] Image { get; set; }

        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }
    }
}
