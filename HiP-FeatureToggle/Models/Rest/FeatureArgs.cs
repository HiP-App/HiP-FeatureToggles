﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Rest
{
    public class FeatureArgs
    {
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// ID of the parent feature, or -1 if this is a root feature.
        /// </summary>
        [DefaultValue(-1)]
        public int Parent { get; set; }
    }
}
