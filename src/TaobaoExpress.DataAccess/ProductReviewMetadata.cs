namespace TaobaoExpress.DataAccess
{
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ProductReviewMetadata))]
    public partial class ProductReview : ICreated, IIdentifiable
    {
    }

    public class ProductReviewMetadata
    {
        [EmailAddress]
        [Required]
        [Display(Name = "Email Address")]
        public string UserEmail { get; set; }

        [MinLength(10)]
        [Required]
        public string Text { get; set; }

        [Required]
        [Range(1, 5)]
        public int Review { get; set; }
    }
}
