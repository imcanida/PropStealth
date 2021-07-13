using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropStealth.Models
{
    [System.Serializable]
    public class AssetToLoad
    {
        public AssetToLoad() { }
        public AssetToLoad(string assetBundle, string assetPrefab)
        {
            this.assetBundle = assetBundle;
            this.assetPrefab = assetPrefab;
        }

        public string assetBundle { get; set; }
        public string assetPrefab { get; set; }
    }
}
