using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ZelnyTrh.EF.DAL.Entities
{
    public class ApplicationUser : IdentityUser, IEntity<string>
    {
        public required string Name { get; set; } = string.Empty;

        // Identity-related navigation properties
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = [];
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; } = [];
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; } = [];
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = [];
        public virtual ICollection<IdentityRole> Roles { get; set; } = [];

        // Application-specific navigation properties
        public virtual ICollection<Orders> Orders { get; set; } = [];
        public virtual ICollection<Offers> Offer { get; set; } = [];
        public virtual ICollection<Reviews> Review { get; set; } = [];
        public virtual ICollection<SelfPickupRegistrations> SelfPickupRegistrations { get; set; } = [];
    }
}