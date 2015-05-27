
namespace DiaryRuSearcher.StoreModel
{
    public class TagStoreModel
    {
        [SQLite.PrimaryKey]
        [SQLite.Unique]
        public string TagId { get; set; }
        public string Description { get; set; }

        public TagStoreModel()
        {
            ;
        }

        public TagStoreModel(string Id, string Description)
        {
            this.TagId = Id;
            this.Description = Description;
        }

        public TagStoreModel(TagStoreModel Model)
        {
            this.TagId = Model.TagId;
            this.Description = Model.Description;
        }
    }
}

