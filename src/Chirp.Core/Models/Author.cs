using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core.Models
{
    /// <summary>
    /// Represents a user in the Chirp application.
    /// </summary>
    public record Author
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of cheeps authored by this user.
        /// </summary>
        public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    }
}