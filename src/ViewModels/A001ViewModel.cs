using MetaFrm.Management.Razor.Models;
using MetaFrm.MVVM;

namespace MetaFrm.Management.Razor.ViewModels
{
    /// <summary>
    /// A001ViewModel
    /// </summary>
    public partial class A001ViewModel : BaseViewModel
    {
        /// <summary>
        /// SearchModel
        /// </summary>
        public SearchModel SearchModel { get; set; } = new();

        /// <summary>
        /// SelectResultModel
        /// </summary>
        public List<AssemblyModel> SelectResultModel { get; set; } = new List<AssemblyModel>();

        /// <summary>
        /// C001ViewModel
        /// </summary>
        public A001ViewModel()
        {

        }
    }
}