using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class SkinEntity
    {
        public SkinEntity(string ActiveSkinName,string ActiveSvgPaletteName) {
            this.ActiveSkinName = ActiveSkinName;
            this.ActiveSvgPaletteName = ActiveSvgPaletteName;
        }
        public SkinEntity()
        {

        }
        public string ActiveSkinName { get; set; } = "Basic";

        public string ActiveSvgPaletteName { get; set; } = "Office White";
    }
}
