using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Engine
{
    public class PlayerQuest:INotifyPropertyChanged
    {
        private Quest _details;
        private bool _iscompleted;
        public Quest Details { get { return _details; } set {
                _details = value; OnPropertyChanged("Details");
            } }
        public bool IsCompleted { get { return _iscompleted; } set {
                _iscompleted = value; OnPropertyChanged("IsCompleted");
            } }
        public string Name
        {
            get { return Details.Name; }
        }
        public PlayerQuest(Quest details)
        {
            Details = details;
            IsCompleted = false;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        
    }
}
