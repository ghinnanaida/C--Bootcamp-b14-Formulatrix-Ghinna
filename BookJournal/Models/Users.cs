using Microsoft.AspNetCore.Identity;

namespace BookJournal.Models
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<ProgressTracker> ProgressTrackers { get; set; } = new HashSet<ProgressTracker>();

    }
}