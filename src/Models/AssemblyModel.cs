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
        [Display(Name = "네임스페이스")]
        public string? NAMESPACE { get; set; }

        /// <summary>
        /// DllFile
        /// </summary>
        [Display(Name = "DLL")]
        public Microsoft.AspNetCore.Components.Forms.IBrowserFile? DllFile { get; set; }

        /// <summary>
        /// FILE_DATE
        /// </summary>
        public DateTime? FILE_DATE { get; set; }

        /// <summary>
        /// VERSION
        /// </summary>
        [Display(Name = "버전")]
        public string? VERSION { get; set; }

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