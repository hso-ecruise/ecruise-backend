using System.ComponentModel.DataAnnotations;

namespace ecruise.Models
{
    public class Registration
        : Customer
    {
        public Registration(uint customerId, string password, string email, string phoneNumber, string chipCardUid,
            string firstName, string lastName, string country, string city, uint? zipCode, string street,
            string houseNumber, string addressExtraLine, bool activated = false, bool verified = false)
            : base(customerId, email, phoneNumber, chipCardUid, firstName, lastName, country, city, zipCode, street,
                houseNumber, addressExtraLine, activated, verified)
        {
            Password = password;
        }

        /// <summary>
        ///     Gets Password
        /// </summary>
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 8,
            ErrorMessage = "The field Password must be a string a minimum length of '8'.")]
        public string Password { get; }
    }
}
