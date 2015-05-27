using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryRuSearcher.StoreModel
{
    public class TagRelationStoreModel
    {
        public string TagId { get; set; }
        public string PostId { get; set; }
        public TagRelationStoreModel()
        {
            ;
        }
        public TagRelationStoreModel(string TagId, string PostId)
        {
            this.TagId = TagId;
            this.PostId = PostId;
        }
    }
}
