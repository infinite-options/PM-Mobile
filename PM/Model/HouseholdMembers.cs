using System;
using System.ComponentModel;

namespace PM.Model
{
    public class HouseholdMembers : INotifyPropertyChanged
    {
        string memTitle;
        string memName;
        string memAge;
        string memRelation;

        public event PropertyChangedEventHandler PropertyChanged;

        public string MemberTitle
        {
            set
            {
                if (memTitle != value)
                {
                    memTitle = value;
                    OnPropertyChanged("MemberTitle");
                }
            }
            get
            {
                return memTitle;
            }

        }

        public string MemberName
        {
            set
            {
                if (memName != value)
                {
                    memName = value;
                    OnPropertyChanged("MemberName");
                }
            }
            get
            {
                return memName;
            }

        }

        public string MemberAge
        {
            set
            {
                if (memAge != value)
                {
                    memAge = value;
                    OnPropertyChanged("MemberAge");
                }
            }
            get
            {
                return memAge;
            }

        }

        public string MemberRelationship
        {
            set
            {
                if (memRelation != value)
                {
                    memRelation = value;
                    OnPropertyChanged("MemberRelationship");
                }
            }
            get
            {
                return memRelation;
            }

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class HouseholdComp : INotifyPropertyChanged
    {
        string memTitle;
        string memName;
        string memSSN;
        string memAge;
        string memRelation;
        string memdob;

        public event PropertyChangedEventHandler PropertyChanged;

        public string MemberSSN
        {
            set
            {
                if (memSSN != value)
                {
                    memSSN = value;
                    OnPropertyChanged("MemberSSN");
                }
            }
            get
            {
                return memSSN;
            }

        }

        public string MemberDOB
        {
            set
            {
                if (memdob != value)
                {
                    memdob = value;
                    OnPropertyChanged("MemberDOB");
                }
            }
            get
            {
                return memdob;
            }

        }

        public string MemberTitle
        {
            set
            {
                if (memTitle != value)
                {
                    memTitle = value;
                    OnPropertyChanged("MemberTitle");
                }
            }
            get
            {
                return memTitle;
            }

        }

        public string MemberName
        {
            set
            {
                if (memName != value)
                {
                    memName = value;
                    OnPropertyChanged("MemberName");
                }
            }
            get
            {
                return memName;
            }

        }

        public string MemberAge
        {
            set
            {
                if (memAge != value)
                {
                    memAge = value;
                    OnPropertyChanged("MemberAge");
                }
            }
            get
            {
                return memAge;
            }

        }

        public string MemberRelationship
        {
            set
            {
                if (memRelation != value)
                {
                    memRelation = value;
                    OnPropertyChanged("MemberRelationship");
                }
            }
            get
            {
                return memRelation;
            }

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
