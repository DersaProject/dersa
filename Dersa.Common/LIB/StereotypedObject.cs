using System;
using Dersa.Interfaces;

namespace Dersa.Common
{
    public abstract class StereotypedObject :/* BaseClass,*/ IStereotypedObject
    {
        public StereotypedObject()
        {
        }
        public StereotypedObject(DersaSqlManager sm)
        {
            _SM = sm;
        }
        protected int _id = -1;
        public int Id
        {
            get
            {
                return _id;
            }
        }
        protected string _name = "name is undefined";
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private DersaSqlManager _SM;
        protected DersaSqlManager SM
        {
            get
            {
                return _SM;
            }
        }
        protected string _stereotype_name;
        public string StereotypeName
        {
            get
            {
                return _stereotype_name;
            }
        }
        public int Rank
        {
            get
            {
                return 0;
            }
        }
        public ICompiled GetCompiledInstance()
        {
            return DersaUtil.CreateInstance(this, this.SM);
            //return null;
        }
        string IStereotypedObject.StereotypeName
        {
            get
            {
                return StereotypeName;
            }
        }
        public/* override*/ int CompareTo(Object y)
        {
            return 0;
        }
    }
}