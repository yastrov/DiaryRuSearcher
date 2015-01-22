using DiaryAPI.JSONResponseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI.Interfaces
{
    public interface ICommentProcessor
    {
        void ProcessComment(CommentUnit comment);
    }
}
