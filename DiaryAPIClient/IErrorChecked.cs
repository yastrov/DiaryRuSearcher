using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI
{
    interface IErrorChecked
    {
        Boolean CheckForError();
        String GetErrorAsString();
    }
}
