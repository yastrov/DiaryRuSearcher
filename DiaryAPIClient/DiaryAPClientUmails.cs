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

        public List<UmailUnit> UmailsGet(string folder, Int64 from)
        {
            List<UmailUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "umail.get");
            nvc.Add("sid", _sid);
            nvc.Add("folder", folder.Trim());
            if (from != 0)
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
                System.Threading.Thread.Sleep(this.timeoutBetweenRequests);
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
                System.Threading.Thread.Sleep(this.timeoutBetweenRequests);
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

        public async Task<List<UmailUnit>> UmailsGetAsync(string folder, Int64 from)
        {
            List<UmailUnit> result;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("method", "umail.get");
            nvc.Add("sid", _sid);
            nvc.Add("folder", folder.Trim());
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
            Int64 i = 0;
            List<UmailUnit> r;
            while (i < folderUnit.Count)
            {
                r = UmailsGet(folderUnit.Folderid.Trim(), i);
                var count = r.Count();
                if (count == 0)
                    break;
                result.AddRange(r);
                System.Threading.Thread.Sleep(this.timeoutBetweenRequests);
                i += Convert.ToInt64(r.Count());
            }
            return result;
        }

        public async Task AllUmailsGetInFolderProcessingAsync(UmailFolderUnit folderUnit,
                                                            IUmailProcessor processor,
                                                            IProgress<Double> onProgressPercentChanged,
                                                            CancellationToken cancellationToken)
        {
            Int64 i = 0;
            List<UmailUnit> r;
            while (i < folderUnit.Count)
            {
                r = await UmailsGetAsync(folderUnit.Folderid.Trim(), i);
                var count = r.Count();
                if (count == 0)
                    break;
                foreach (var umail in r)
                    processor.ProcessUmail(umail);
                System.Threading.Thread.Sleep(this.timeoutBetweenRequests);
                cancellationToken.ThrowIfCancellationRequested();
                i += Convert.ToInt64(r.Count());
                onProgressPercentChanged.Report(i / folderUnit.Count);
            }
        }
        public async Task AllUmailsGetInAllFoldersProcessingAsync(IUmailProcessor processor,
                                                       IProgress<Double> onProgressPercentChanged,
                                                        CancellationToken cancellationToken)
        {
            var r = UmailGetFolders();
            foreach (var folder in r)
            {
                if (folder.Count == 0) continue;
                await AllUmailsGetInFolderProcessingAsync(folder, processor, onProgressPercentChanged, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        #endregion
    }
}
