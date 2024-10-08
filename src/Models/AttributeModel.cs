﻿using System.ComponentModel.DataAnnotations;

namespace MetaFrm.Management.Razor.Models
{
    /// <summary>
    /// AttributeModel
    /// </summary>
    public class AttributeModel
    {
        /// <summary>
        /// ATTRIBUTE_ID
        /// </summary>
        public int? ATTRIBUTE_ID { get; set; }

        /// <summary>
        /// ASSEMBLY_ID
        /// </summary>
        public int? ASSEMBLY_ID { get; set; }

        /// <summary>
        /// ATTRIBUTE_NAME
        /// </summary>
        [Required]
        [MinLength(1)]
        [Display(Name = "속성")]
        public string? ATTRIBUTE_NAME { get; set; }

        /// <summary>
        /// ATTRIBUTE_VALUE
        /// </summary>
        [Required]
        [Display(Name = "값")]
        public string? ATTRIBUTE_VALUE { get; set; }
    }
}