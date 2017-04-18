using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class PostReference :  IEquatable<PostReference>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PostReference" /> class.
        /// </summary>
        /// <param name="id">The ressource's unique identifier .</param>
        /// <param name="url">URL to the ressource .</param>
        public PostReference(int? id, string url)
        {
            Id = id;
            Url = url;
        }

        /// <summary>
        /// The ressource&#39;s unique identifier 
        /// </summary>
        /// <value>The ressource&#39;s unique identifier </value>
        public int? Id { get; set; }
        /// <summary>
        /// URL to the ressource 
        /// </summary>
        /// <value>URL to the ressource </value>
        public string Url { get; set; }

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
                (
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other.Id)
                ) && 
                (
                    Url == other.Url ||
                    Url != null &&
                    Url.Equals(other.Url)
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
                if (Id != null)
                    hash = hash * 59 + Id.GetHashCode();
                if (Url != null)
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
