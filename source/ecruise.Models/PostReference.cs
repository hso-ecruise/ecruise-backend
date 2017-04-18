using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class PostReference
        : IEquatable<PostReference>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostReference" /> class.
        /// </summary>
        /// <param name="id">The ressource's unique identifier (required)</param>
        /// <param name="url">URL to the ressource (required)</param>
        public PostReference(int id, string url)
        {
            if (id == 0)
                throw new ArgumentNullException(
                    nameof(id) + " is a required property for PostReference and cannot be zero");
            Id = id;
            Url = url ?? throw new ArgumentNullException(
                      nameof(url) + " is a required property for Maintenance and cannot be null");
        }

        /// <summary>
        /// The ressource's unique identifier 
        /// </summary>
        /// <value>The ressource's unique identifier</value>
        public int Id { get; }

        /// <summary>
        /// URL to the ressource 
        /// </summary>
        /// <value>URL to the ressource </value>
        public string Url { get; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PostReference {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Url: ").Append(Url).Append("\n");
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
            return Equals((PostReference)obj);
        }

        /// <summary>
        /// Returns true if PostReference instances are equal
        /// </summary>
        /// <param name="other">Instance of PostReference to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PostReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (Id == other.Id || Id.Equals(other.Id)) &&
                (Url == other.Url || Url.Equals(other.Url));
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

                hash = hash * 59 + Id.GetHashCode();
                hash = hash * 59 + Url.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(PostReference left, PostReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PostReference left, PostReference right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
