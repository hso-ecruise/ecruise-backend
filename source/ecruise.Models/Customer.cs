using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Customer : IEquatable<Customer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Customer" /> class.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="phoneNumber">PhoneNumber.</param>
        /// <param name="firstName">FirstName.</param>
        /// <param name="lastName">LastName.</param>
        /// <param name="country">Country.</param>
        /// <param name="city">City.</param>
        /// <param name="zipCode">ZipCode.</param>
        /// <param name="street">Street.</param>
        /// <param name="houseNumber">HouseNumber.</param>
        /// <param name="addressExtraLine">Extra line for the user's address. Can contain various detail information about the user&#39;s address. .</param>
        /// <param name="activated">True if the user has activated his account by clicking on the link in the activation email. .</param>
        /// <param name="verified">True if the user has verified his account at our head-quarter by bringing us his driver&#39;s license. .</param>
        public Customer(string email, string phoneNumber, string firstName, string lastName, string country,
            string city, int? zipCode, string street, string houseNumber, string addressExtraLine, bool? activated,
            bool? verified)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            FirstName = firstName;
            LastName = lastName;
            Country = country;
            City = city;
            ZipCode = zipCode;
            Street = street;
            HouseNumber = houseNumber;
            AddressExtraLine = addressExtraLine;
            Activated = activated;
            Verified = verified;
        }

        /// <summary>
        /// Gets or Sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or Sets PhoneNumber
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or Sets FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or Sets LastName
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or Sets Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or Sets City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or Sets ZipCode
        /// </summary>
        public int? ZipCode { get; set; }

        /// <summary>
        /// Gets or Sets Street
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or Sets HouseNumber
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// Extra line for the user&#39;s address. Can contain various    detail information about the user&#39;s address. 
        /// </summary>
        /// <value>Extra line for the user&#39;s address. Can contain various    detail information about the user&#39;s address. </value>
        public string AddressExtraLine { get; set; }

        /// <summary>
        /// True if the user has activated his account by clicking on the   link in the activation email. 
        /// </summary>
        /// <value>True if the user has activated his account by clicking on the   link in the activation email. </value>
        public bool? Activated { get; set; }

        /// <summary>
        /// True if the user has verified his account at our head-quarter   by bringing us his driver&#39;s license. 
        /// </summary>
        /// <value>True if the user has verified his account at our head-quarter   by bringing us his driver&#39;s license. </value>
        public bool? Verified { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Customer {\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            sb.Append("  PhoneNumber: ").Append(PhoneNumber).Append("\n");
            sb.Append("  FirstName: ").Append(FirstName).Append("\n");
            sb.Append("  LastName: ").Append(LastName).Append("\n");
            sb.Append("  Country: ").Append(Country).Append("\n");
            sb.Append("  City: ").Append(City).Append("\n");
            sb.Append("  ZipCode: ").Append(ZipCode).Append("\n");
            sb.Append("  Street: ").Append(Street).Append("\n");
            sb.Append("  HouseNumber: ").Append(HouseNumber).Append("\n");
            sb.Append("  AddressExtraLine: ").Append(AddressExtraLine).Append("\n");
            sb.Append("  Activated: ").Append(Activated).Append("\n");
            sb.Append("  Verified: ").Append(Verified).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Customer)obj);
        }

        /// <summary>
        /// Returns true if Customer instances are equal
        /// </summary>
        /// <param name="other">Instance of Customer to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Customer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    Email == other.Email ||
                    Email != null &&
                    Email.Equals(other.Email)
                ) &&
                (
                    PhoneNumber == other.PhoneNumber ||
                    PhoneNumber != null &&
                    PhoneNumber.Equals(other.PhoneNumber)
                ) &&
                (
                    FirstName == other.FirstName ||
                    FirstName != null &&
                    FirstName.Equals(other.FirstName)
                ) &&
                (
                    LastName == other.LastName ||
                    LastName != null &&
                    LastName.Equals(other.LastName)
                ) &&
                (
                    Country == other.Country ||
                    Country != null &&
                    Country.Equals(other.Country)
                ) &&
                (
                    City == other.City ||
                    City != null &&
                    City.Equals(other.City)
                ) &&
                (
                    ZipCode == other.ZipCode ||
                    ZipCode != null &&
                    ZipCode.Equals(other.ZipCode)
                ) &&
                (
                    Street == other.Street ||
                    Street != null &&
                    Street.Equals(other.Street)
                ) &&
                (
                    HouseNumber == other.HouseNumber ||
                    HouseNumber != null &&
                    HouseNumber.Equals(other.HouseNumber)
                ) &&
                (
                    AddressExtraLine == other.AddressExtraLine ||
                    AddressExtraLine != null &&
                    AddressExtraLine.Equals(other.AddressExtraLine)
                ) &&
                (
                    Activated == other.Activated ||
                    Activated != null &&
                    Activated.Equals(other.Activated)
                ) &&
                (
                    Verified == other.Verified ||
                    Verified != null &&
                    Verified.Equals(other.Verified)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 41;

                if (Email != null)
                    hash = hash * 59 + Email.GetHashCode();
                if (PhoneNumber != null)
                    hash = hash * 59 + PhoneNumber.GetHashCode();
                if (FirstName != null)
                    hash = hash * 59 + FirstName.GetHashCode();
                if (LastName != null)
                    hash = hash * 59 + LastName.GetHashCode();
                if (Country != null)
                    hash = hash * 59 + Country.GetHashCode();
                if (City != null)
                    hash = hash * 59 + City.GetHashCode();
                if (ZipCode != null)
                    hash = hash * 59 + ZipCode.GetHashCode();
                if (Street != null)
                    hash = hash * 59 + Street.GetHashCode();
                if (HouseNumber != null)
                    hash = hash * 59 + HouseNumber.GetHashCode();
                if (AddressExtraLine != null)
                    hash = hash * 59 + AddressExtraLine.GetHashCode();
                if (Activated != null)
                    hash = hash * 59 + Activated.GetHashCode();
                if (Verified != null)
                    hash = hash * 59 + Verified.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Customer left, Customer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Customer left, Customer right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
