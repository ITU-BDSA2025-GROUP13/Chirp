using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Domain
{
    /// <summary>
    /// Represents a message (cheep) posted by a user in the Chirp application.
    /// </summary>
    public record Cheep
    {
        /// <summary>
        /// Gets or sets the unique identifier for the message.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheepId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who authored this message.
        /// </summary>
        public required int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the user who authored this message.
        /// </summary>
        [ForeignKey("AuthorId")]
        public required Author? Author { get; set; }

        /// <summary>
        /// Gets or sets the text content of the message.
        /// </summary>
        public required string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the publication date of the message.
        /// </summary>
        public required DateTime TimeStamp { get; set; }
    }
}