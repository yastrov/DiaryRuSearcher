using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryRuSearcher.ViewsModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected static DiaryDataBase _diaryDataBase;
        public static void SetDataBase(DiaryDataBase dbase)
        {
            _diaryDataBase = dbase;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
