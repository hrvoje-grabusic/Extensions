using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace Kooboo.Modules.XLIFF.Models
{
    public class TextContentImportModel : Kooboo.CMS.Web.Models.ImportModel
    {
        [DisplayName("Data format")]
        public string Formatter { get; set; }

        //[Required(ErrorMessage = "Required")]
        ////[RegularExpression(".+\\.(zip)$", ErrorMessage = "Required a zip file.")]
        //[UIHint("File")]
        ////[Description("choose a .xlf file")]
        //public virtual HttpPostedFileWrapper File { get; set; }
        [Required(ErrorMessage = "Required")]
        [UIHint("File")]
        [Description("choose a .xlf file")]
        public override HttpPostedFileWrapper File
        {
            get
            {
                return base.File;
            }
            set
            {
                base.File = value;
            }
        }

        [Required]
        public string FolderName { get; set; }

        public ITextContentFormater TextContentExporter
        {
            get
            {
                return Kooboo.CMS.Common.Runtime.EngineContext.Current.Resolve<ITextContentFormater>(this.Formatter.ToLower());
            }
        }
    }
}
