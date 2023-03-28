using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using RequiredAttribute = Microsoft.Build.Framework.RequiredAttribute;

namespace Lab5.Models
{        
    public enum Question
    {
        Earth, Computer
    }
    public class Prediction
    {
        [Required]
        public int PredictionID { get; set; }

        [Required]
        public string FileName { get; set; }

        [Url]
        [Required]
        public string Url { get; set; }

        [Required]
        public Question Question
        {
            get; set;
        }

        public static implicit operator int(Prediction v)
        {
            throw new NotImplementedException();
        }
    }
}
