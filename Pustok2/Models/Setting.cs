using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [StringLength(maximumLength:50)]
        public string SupportPhone { get; set; }
        [StringLength(maximumLength: 50)]
        public string ContactPhone { get; set; }
        [StringLength(maximumLength: 100)]
        public string Address { get; set; }
        [StringLength(maximumLength: 100)]
        public string HeaderLogo { get; set; }
        [StringLength(maximumLength: 100)]
        public string FooterLogo { get; set; }
        [StringLength(maximumLength: 100)]
        public string Email { get; set; }
        [StringLength(maximumLength: 100)]
        public string PromotionTitle { get; set; }
        [StringLength(maximumLength: 100)]
        public string PromotionSubtitle { get; set; }
        [StringLength(maximumLength: 100)]
        public string PromotionBtnText { get; set; }
        [StringLength(maximumLength: 100)]
        public string PromotionRedirectURL { get; set; }
        [StringLength(maximumLength: 100)]
        public string PromotionBgImage { get; set; }

    }
}
