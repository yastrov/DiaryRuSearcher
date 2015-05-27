using DiaryRuSearcher.StoreModel;

namespace DiaryRuSearcher.ViewsModels
{
    public class TagViewModel : BaseViewModel
    {
        public string TagId { get; set; }
        public string Description { get; set; }
        private bool _isSelected;
        public bool IsSelected {
            get { return _isSelected; }
            set {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public TagViewModel()
        {
            this.IsSelected = false;
        }
        public TagViewModel(TagStoreModel Model)
        {
            this.IsSelected = false;
            this.TagId = Model.TagId;
            this.Description = Model.Description;
        }
    }
}
