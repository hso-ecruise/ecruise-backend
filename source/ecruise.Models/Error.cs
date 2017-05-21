using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Error
        : IEquatable<Error>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error" /> class.
        /// </summary>
        /// <param name="code">Unique error code (required)</param>
        /// <param name="message">Basic error message (required)</param>
        /// <param name="description">Detailed error message (required)</param>
        public Error(int code, string message, string description)
        {
            Code = code;
            Message = message;
            Description = description;
        }

        /// <summary>
        /// Unique error code
        /// </summary>
        /// <value>Unique error code</value>
        [Range(int.MinValue, int.MaxValue)]
        public int Code { get; }

        /// <summary>
        /// Basic error message
        /// </summary>
        /// <value>Basic error message</value>
        [Required]
        public string Message { get; }

        /// <summary>
        /// Detailed error message
        /// </summary>
        /// <value>Detailed error message</value>
        [Required]
        public string Description { get; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Error {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
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
            return Equals((Error)obj);
        }

        /// <summary>
        /// Returns true if Error instances are equal
        /// </summary>
        /// <param name="other">Instance of Error to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Error other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (Code == other.Code || Code.Equals(other.Code)) &&
                (Message == other.Message || Message.Equals(other.Message)) &&
                (Description == other.Description || Description.Equals(other.Description));
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

                hash = hash * 59 + Code.GetHashCode();
                hash = hash * 59 + Message.GetHashCode();
                hash = hash * 59 + Description.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Error left, Error right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Error left, Error right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
