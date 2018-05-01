namespace TaobaoExpress.DataAccess
{
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(RetailerMetadata))]
    public partial class Retailer : ICreated, IUpdated, IIdentifiable
    {
    }

    public class RetailerMetadata
    {
        [Required(ErrorMessage = "The name is required")]
        [MinLength(5, ErrorMessage = "Retailer names must be at least 5 characters long")]
        public string Name { get; set; }
    }
}
