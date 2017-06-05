using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using MimeKit;


namespace ecruise.Models
{
    public class Customer
        : IEquatable<Customer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Customer" /> class.
        /// </summary>
        /// <param name="customerId">CustomerId (required)</param>
        /// <param name="email">Email (required)</param>
        /// <param name="chipCardUid">ChipCardUid</param>
        /// <param name="phoneNumber">PhoneNumber (required)</param>
        /// <param name="firstName">FirstName (required)</param>
        /// <param name="lastName">LastName (required)</param>
        /// <param name="country">Country (required)</param>
        /// <param name="city">City (required)</param>
        /// <param name="zipCode">ZipCode (required)</param>
        /// <param name="street">Street (required)</param>
        /// <param name="houseNumber">HouseNumber (required)</param>
        /// <param name="addressExtraLine">Extra line for the user's address. Can contain various detail information about the user's address. (required)</param>
        /// <param name="activated">True if the user has activated his account by clicking on the link in the activation email.</param>
        /// <param name="verified">True if the user has verified his account at our head-quarter by bringing us his driver's license.</param>
        public Customer(uint customerId, string email, string phoneNumber, string chipCardUid, string firstName,
            string lastName, string country, string city, uint zipCode, string street, string houseNumber,
            string addressExtraLine, bool activated = false, bool verified = false)
        {
            CustomerId = customerId;
            Email = email;
            PhoneNumber = phoneNumber;
            ChipCardUid = chipCardUid;
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
        /// Gets or Sets a CustomerId
        /// </summary>
        [Required, Range(0, uint.MaxValue)]
        public uint CustomerId { get; }

        /// <summary>
        /// Gets or Sets Email
        /// </summary>
        [Required, StringLength(64), EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or Sets PhoneNumber
        /// </summary>
        [StringLength(16)]
        public string ChipCardUid { get; set; }

        /// <summary>
        /// Gets or Sets PhoneNumber
        /// </summary>
        [Required, StringLength(32)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or Sets FirstName
        /// </summary>
        [Required, StringLength(48)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or Sets LastName
        /// </summary>
        [Required, StringLength(48)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or Sets Country
        /// </summary>
        [Required, StringLength(2, MinimumLength = 2)]
        public string Country { get; set; }

        /// <summary>
        /// Gets or Sets City
        /// </summary>
        [Required, StringLength(64)]
        public string City { get; set; }

        /// <summary>
        /// Gets or Sets ZipCode
        /// </summary>
        [Required, Range(1001, 99999)]
        public uint ZipCode { get; set; }

        /// <summary>
        /// Gets or Sets Street
        /// </summary>
        [Required, StringLength(128)]
        public string Street { get; set; }

        /// <summary>
        /// Gets or Sets HouseNumber
        /// </summary>
        [Required, StringLength(8)]
        public string HouseNumber { get; set; }

        /// <summary>
        /// Extra line for the user's address. Can contain various detail information about the user's address. 
        /// </summary>
        /// <value>Extra line for the user's address. Can contain various detail information about the user's address. </value>
        [StringLength(64)]
        public string AddressExtraLine { get; set; }

        /// <summary>
        /// True if the user has activated his account by clicking on the link in the activation email. 
        /// </summary>
        /// <value>True if the user has activated his account by clicking on the link in the activation email. </value>
        public bool Activated { get; set; }

        /// <summary>
        /// True if the user has verified his account at our head-quarter by bringing us his driver's license. 
        /// </summary>
        /// <value>True if the user has verified his account at our head-quarter by bringing us his driver's license.</value>
        public bool Verified { get; set; }

        /// <summary>
        /// Sends a mail with the specified subject and body to the customer
        /// </summary>
        /// <param name="subject">The subject for the mail</param>
        /// <param name="body">The body for the mail</param>
        /// <returns>True if mail could be sent, else false</returns>
        public async Task<bool> SendMail(string subject, string body)
        {
            // return false if no mail server configured
            if (Environment.GetEnvironmentVariable("EMAIL_SERVER") == null)
                return false;

            // Create the message
            MimeMessage mail = new MimeMessage();

            // set mail header fields
            mail.From.Add(new MailboxAddress("eCruise Information", "info@ecruise.me"));
            mail.To.Add(new MailboxAddress($"{FirstName} {LastName}", $"{Email}"));
            mail.Subject = subject;
            mail.Body = new TextPart("html")
            {
                Text = body
            };

            // create the smtp client
            using (var smtpClient = new SmtpClient())
            {
                // connect to the server
                smtpClient.Connect(
                    Environment.GetEnvironmentVariable("EMAIL_SERVER"),
                    ushort.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")),
                    false
                );

                // don't use oauth2 authentification
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");

                // login using user and password
                smtpClient.Authenticate(Environment.GetEnvironmentVariable("EMAIL_USER"),
                    Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));

                // Send the mail
                await smtpClient.SendAsync(mail);

                // Gracefully disconnect from mail server
                smtpClient.Disconnect(true);
            }

            return true;
        }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Customer {\n");
            sb.Append("  CustomerId: ").Append(CustomerId).Append("\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            sb.Append("  PhoneNumber: ").Append(PhoneNumber).Append("\n");
            sb.Append("  ChipCardUid: ").Append(ChipCardUid).Append("\n");
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
                (CustomerId == other.CustomerId || CustomerId.Equals(other.CustomerId)) &&
                (Email == other.Email || Email.Equals(other.Email)) &&
                (PhoneNumber == other.PhoneNumber || PhoneNumber.Equals(other.PhoneNumber)) &&
                (ChipCardUid == other.ChipCardUid || ChipCardUid.Equals(other.ChipCardUid)) &&
                (FirstName == other.FirstName || FirstName.Equals(other.FirstName)) &&
                (LastName == other.LastName || LastName.Equals(other.LastName)) &&
                (Country == other.Country || Country.Equals(other.Country)) &&
                (City == other.City || City.Equals(other.City)) &&
                (ZipCode == other.ZipCode || ZipCode.Equals(other.ZipCode)) &&
                (Street == other.Street || Street.Equals(other.Street)) &&
                (HouseNumber == other.HouseNumber || HouseNumber.Equals(other.HouseNumber)) &&
                (AddressExtraLine == other.AddressExtraLine || AddressExtraLine.Equals(other.AddressExtraLine)) &&
                (Activated == other.Activated || Activated.Equals(other.Activated)) &&
                (Verified == other.Verified || Verified.Equals(other.Verified));
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

                hash = hash * 59 + CustomerId.GetHashCode();

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

    public class Address
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Address" /> class.
        /// </summary>
        /// <param name="country">Country (required)</param>
        /// <param name="city">City (required)</param>
        /// <param name="zipCode">ZipCode (required)</param>
        /// <param name="street">Street (required)</param>
        /// <param name="houseNumber">HouseNumber (required)</param>
        /// <param name="adressExtraLine">AdressExtraLine</param>
        public Address(string country, string city, uint zipCode, string street, string houseNumber,
            string adressExtraLine)
        {
            Country = country;
            City = city;
            ZipCode = zipCode;
            Street = street;
            HouseNumber = houseNumber;
            AdressExtraLine = adressExtraLine;
        }

        [Required, StringLength(2, MinimumLength = 2)]
        public string Country { get; }

        [Required, StringLength(64)]
        public string City { get; }

        [Required, Range(1001, 99999)]
        public uint ZipCode { get; }

        [Required, StringLength(128)]
        public string Street { get; }

        [Required, StringLength(8)]
        public string HouseNumber { get; }

        public string AdressExtraLine { get; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Address {\n");
            sb.Append("  Country: ").Append(Country).Append("\n");
            sb.Append("  City: ").Append(City).Append("\n");
            sb.Append("  ZipCode: ").Append(ZipCode).Append("\n");
            sb.Append("  Street: ").Append(Street).Append("\n");
            sb.Append("  HouseNumber: ").Append(HouseNumber).Append("\n");
            sb.Append("  AdressExtraLine: ").Append(AdressExtraLine).Append("\n");
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
            return Equals((Address)obj);
        }

        /// <summary>
        /// Returns true if Address instances are equal
        /// </summary>
        /// <param name="other">Instance of Address to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (Country == other.Country || Country.Equals(other.Country)) &&
                (City == other.City || City.Equals(other.City)) &&
                (ZipCode == other.ZipCode || ZipCode.Equals(other.ZipCode)) &&
                (Street == other.Street || Street.Equals(other.Street)) &&
                (HouseNumber == other.HouseNumber || HouseNumber.Equals(other.HouseNumber)) &&
                (AdressExtraLine == other.AdressExtraLine || AdressExtraLine.Equals(other.AdressExtraLine));
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

                hash = hash * 59 + Country.GetHashCode();
                hash = hash * 59 + City.GetHashCode();
                hash = hash * 59 + ZipCode.GetHashCode();
                hash = hash * 59 + Street.GetHashCode();
                hash = hash * 59 + HouseNumber.GetHashCode();
                hash = hash * 59 + AdressExtraLine.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Address left, Address right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Address left, Address right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
