using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryRuSearcher.ViewsModels
{
    public class UmailFolderViewModel
    {
        public string Name { get; set; }
        public Int64 Count { get; set; }
        public string Folderid { get; set; }

        public UmailFolderViewModel(DiaryAPI.JSONResponseClasses.UmailFolderUnit folder)
        {
            this.Name = folder.Name;
            this.Count = folder.Count;
            this.Folderid = folder.Folderid.Replace("#", String.Empty);
        }
    }
}
