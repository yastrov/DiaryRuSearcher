using DiaryAPI.Interfaces;
using DiaryAPI.JSONResponseClasses;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public partial class DiaryAPIClient
    {
        public List<UmailFolderUnit> UmailGetFolders()
        {
            List<UmailFolderUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "umail.get_folders");
            nvc.Add("sid", _sid);
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = this._Request(postString, "GET", null))
            {
                var r = GetObjectFromJson<UmailFoldersJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                result = r.GetFolders();
            }
            return result;
        }

        public List<UmailUnit> UmailsGet(string folder, int from)
        {
            List<UmailUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "umail.get");
            nvc.Add("sid", _sid);
            nvc.Add("folder", folder);
            if(from != 0)
            nvc.Add("from", from.ToString());
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = this._Request(postString, "GET", null))
            {
                var r = GetObjectFromJson<UmailJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                result = r.GetUmails();
            }
            return result;
        }

        public List<UmailUnit> AllUmailsGetInFolder(UmailFolderUnit folderUnit)
        {
            List<UmailUnit> result = new List<UmailUnit>();
            int i = 0;
            List<UmailUnit> r;
            while (i < folderUnit.Count)
            {
                r = UmailsGet(folderUnit.Folderid.Trim(), i);
                result.AddRange(r);
                System.Threading.Thread.Sleep(1000);
                //i += Convert.ToUInt64(r.Count());
                i += r.Count();
            }
            return result;
        }

        public List<UmailUnit> AllUmailsGetInAllFolders()
        {
            List<UmailUnit> result = new List<UmailUnit>();
            foreach (var folder in UmailGetFolders())
            {
                result.AddRange(AllUmailsGetInFolder(folder));
            }
            return result;
        }

        #region Procession
        public void AllUmailsGetInFolderProcessing(UmailFolderUnit folderUnit, IUmailProcessor processor)
        {
            int i = 0;
            List<UmailUnit> r;
            while (i < folderUnit.Count)
            {
                r = UmailsGet(folderUnit.Name, i);
                foreach (var umail in r)
                    processor.ProcessUmail(umail);
                System.Threading.Thread.Sleep(1000);
                //i += Convert.ToUInt64(r.Count());
                i += r.Count();
            }
        }
        public void AllUmailsGetInAllFoldersProcessing(IUmailProcessor processor)
        {
            foreach (var folder in UmailGetFolders())
            {
                try
                {
                    if (folder.Count == 0) continue;
                    AllUmailsGetInFolderProcessing(folder, processor);
                }
                catch (DiaryAPIClientException ex)
                {
                    Console.WriteLine(ex.ToString()); 
                }
            }
        }
        #endregion

        #region Async
        public async Task<List<UmailFolderUnit>> UmailGetFoldersAsync()
        {
            List<UmailFolderUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "umail.get_folders");
            nvc.Add("sid", _sid);
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = await this._RequestAsync(postString, "GET", null))
            {
                var r = GetObjectFromJson<UmailFoldersJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                result = r.GetFolders();
            }
            return result;
        }

        public async Task<List<UmailUnit>> UmailsGetAsync(string folder, int from)
        {
            List<UmailUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "umail.get");
            nvc.Add("sid", _sid);
            nvc.Add("folder", folder);
            if (from != 0)
                nvc.Add("from", from.ToString());
            string postString = NameValueCollectionToUrlString(nvc);
            using (HttpWebResponse response = await this._RequestAsync(postString, "GET", null))
            {
                var r = GetObjectFromJson<UmailJSONResponse>(response);
                if (r.CheckForError()) throw new DiaryAPIClientException(r.Error);
                result = r.GetUmails();
            }
            return result;
        }

        public List<UmailUnit> AllUmailsGetInFolderAsync(UmailFolderUnit folderUnit)
        {
            List<UmailUnit> result = new List<UmailUnit>();
            int i = 0;
            List<UmailUnit> r;
            while (i < folderUnit.Count)
            {
                r = UmailsGet(folderUnit.Folderid.Trim(), i);
                result.AddRange(r);
                System.Threading.Thread.Sleep(1000);
                //i += Convert.ToUInt64(r.Count());
                i += r.Count();
            }
            return result;
        }

        public async Task AllUmailsGetInFolderProcessingAsync(UmailFolderUnit folderUnit, IUmailProcessor processor, CancellationToken cancellationToken)
        {
            int i = 0;
            List<UmailUnit> r;
            while (i < folderUnit.Count)
            {
                r = await UmailsGetAsync(folderUnit.Name, i);
                foreach (var umail in r)
                    processor.ProcessUmail(umail);
                System.Threading.Thread.Sleep(1000);
                cancellationToken.ThrowIfCancellationRequested();
                //i += Convert.ToUInt64(r.Count());
                i += r.Count();
            }
        }
        public async Task AllUmailsGetInAllFoldersProcessingAsync(IUmailProcessor processor, IProgress<int> onProgressPercentChanged,CancellationToken cancellationToken)
        {
            var r = UmailGetFolders();
            int all_mails = 0, current = 0;
            foreach (var f in r)
                all_mails += f.Count;
            foreach (var folder in r)
            {
                try
                {
                    if (folder.Count == 0) continue;
                    current += folder.Count;
                    await AllUmailsGetInFolderProcessingAsync(folder, processor, cancellationToken);
                    onProgressPercentChanged.Report(current/all_mails);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (DiaryAPIClientException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        #endregion
    }
}
