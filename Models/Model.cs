using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VehicleQuotes.Models
{
    [Index(propertyName: nameof(Name), additionalPropertyNames: nameof(MakeID), IsUnique = true)]
    public class Model
    {
        public int ID { get; set; }
        public string Name { get; set; }

        // Foreign key for Make to form a Many-to-One relationship
        public int MakeID { get; set; }

        public Make Make { get; set; }

        // Foreign key of Model is stored in ModelStyle to form a One-to-Many relationship
        public ICollection<ModelStyle> ModelStyles { get; set; }
    }
}