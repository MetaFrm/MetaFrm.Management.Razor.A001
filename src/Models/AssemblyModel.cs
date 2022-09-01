using System.ComponentModel.DataAnnotations;

namespace MetaFrm.Management.Razor.Models
{
    /// <summary>
    /// AssemblyModel
    /// </summary>
    public class AssemblyModel
    {
        /// <summary>
        /// ASSEMBLY_ID
        /// </summary>
        public int? ASSEMBLY_ID { get; set; }

        /// <summary>
        /// NAMESPACE
        /// </summary>
        [Required]
        [MinLength(10)]
        [Display(Name = "Namespace")]
        public string? NAMESPACE { get; set; }

        /// <summary>
        /// DllFile
        /// </summary>
        [Display(Name = "Dll File")]
        public Microsoft.AspNetCore.Components.Forms.IBrowserFile? DllFile { get; set; }

        /// <summary>
        /// FILE_DATE
        /// </summary>
        public DateTime? FILE_DATE { get; set; }

        /// <summary>
        /// VERSION
        /// </summary>
        [Display(Name = "Version")]
        public string? VERSION { get; set; }

        /// <summary>
        /// PLATFORM_ID
        /// </summary>
        [Required]
        [Display(Name = "Platform")]
        public int? PLATFORM_ID { get; set; }
        /// <summary>
        /// PLATFORM_DESC
        /// </summary>
        [Display(Name = "Platform")]
        public string? PLATFORM_DESC { get; set; }

        /// <summary>
        /// NICKNAME
        /// </summary>
        public string? NICKNAME { get; set; }

        /// <summary>
        /// Attributes
        /// </summary>
        public List<AttributeModel> Attributes { get; set; } = new();
    }
}