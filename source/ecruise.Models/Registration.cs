using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Registration : IEquatable<Registration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Registration" /> class.
        /// </summary>
        /// <param name="email">Email (required).</param>
        /// <param name="password">Password (required).</param>
        /// <param name="firstName">FirstName (required).</param>
        /// <param name="lastName">LastName (required).</param>
        public Registration(string email, string password, string firstName, string lastName)
        {
            Email = email ?? throw new ArgumentNullException("Email is a required property for Registration and cannot be null");
            Password = password ?? throw new ArgumentNullException("Password is a required property for Registration and cannot be null");
            FirstName = firstName ?? throw new ArgumentNullException("FirstName is a required property for Registration and cannot be null");
            LastName = lastName ?? throw new ArgumentNullException("LastName is a required property for Registration and cannot be null");
        }

        /// <summary>
        /// Gets or Sets Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or Sets Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or Sets FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or Sets LastName
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Registration {\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            sb.Append("  Password: ").Append(Password).Append("\n");
            sb.Append("  FirstName: ").Append(FirstName).Append("\n");
            sb.Append("  LastName: ").Append(LastName).Append("\n");
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
            return Equals((Registration)obj);
        }

        /// <summary>
        /// Returns true if Registration instances are equal
        /// </summary>
        /// <param name="other">Instance of Registration to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Registration other)
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
                    Password == other.Password ||
                    Password != null &&
                    Password.Equals(other.Password)
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
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (Email != null)
                    hash = hash * 59 + Email.GetHashCode();
                if (Password != null)
                    hash = hash * 59 + Password.GetHashCode();
                if (FirstName != null)
                    hash = hash * 59 + FirstName.GetHashCode();
                if (LastName != null)
                    hash = hash * 59 + LastName.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Registration left, Registration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Registration left, Registration right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
