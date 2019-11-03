using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using DIOS.Common;
using Dersa.Interfaces;
using Dersa.Common;

namespace DersaStereotypes
{
    public class StereotypeUtil : IEntityComparerProvider
    {
        private static Hashtable _cmpTable = new Hashtable();

        public IComparer<ICompiledEntity> GetEntityComparer()
        {
            return new EntityComparer();
        }
        private class EntityComparer : IComparer<ICompiledEntity>
        {
            public int Compare(ICompiledEntity e1, ICompiledEntity e2)
            {
                if (e1.Id == e2.Id)
                    return 0;
                int res = 0;
                if (e1.Id < e2.Id)
                    res = -1;
                else
                    res = 1;
                string cmpid = e1.Id.ToString() + "_" + e2.Id.ToString();
                if (_cmpTable[cmpid] != null)
                    return (int)_cmpTable[cmpid];
                if ((e1 is DersaStereotypes.Entity || e1 is DersaStereotypes.Type) && (e2 is DersaStereotypes.Entity || e2 is DersaStereotypes.Type))
                {
                    //DersaStereotypes.Entity e1 = E1 as DersaStereotypes.Entity;
                    //DersaStereotypes.Entity e2 = E2 as DersaStereotypes.Entity;
                    if (e1 != null)
                    {
                        System.Collections.IList relations1 = e1.ARelations;
                        for (int i = 0; i < relations1.Count; i++)
                        {
                            ICompiledRelation rel = (ICompiledRelation)relations1[i];
                            if (rel.A.Id == rel.B.Id)
                                continue;
                            if (rel is DersaStereotypes.Relation)
                            {
                                DersaStereotypes.Relation Rel = rel as DersaStereotypes.Relation;
                                if (Rel.MakeFK)
                                {
                                    if (Rel.B.Id == e2.Id)
                                    {
                                        _cmpTable[cmpid] = 1;
                                        Logger.LogStatic(string.Format("e1 = {0}({1}), e2 = {2}({3}), result = 1", e1.Id, e1.Name, e2.Id, e2.Name));
                                        return 1;
                                    }
                                    else
                                    {
                                        int compareByEndB = Compare(rel.B, e2);
                                        if (compareByEndB > 0)
                                        {
                                            _cmpTable[cmpid] = 1;
                                            Logger.LogStatic(string.Format("e1 = {0}({1}), e2 = {2}({3}), result = 1", e1.Id, e1.Name, e2.Id, e2.Name));
                                            return 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (e2 != null)
                    {
                        System.Collections.IList relations2 = e2.ARelations;
                        for (int i = 0; i < relations2.Count; i++)
                        {
                            ICompiledRelation rel = (ICompiledRelation)relations2[i];
                            if (rel.A.Id == rel.B.Id)
                                continue;
                            if (rel is DersaStereotypes.Relation)
                            {
                                DersaStereotypes.Relation Rel = rel as DersaStereotypes.Relation;
                                if (Rel.MakeFK)
                                {
                                    if (Rel.B.Id == e1.Id)
                                    {
                                        _cmpTable[cmpid] = -1;
                                        Logger.LogStatic(string.Format("e1 = {0}({1}), e2 = {2}({3}), result = -1", e1.Id, e1.Name, e2.Id, e2.Name));
                                        return -1;
                                    }
                                    else
                                    {
                                        int compareByEndB = Compare(e1, rel.B);
                                        if (compareByEndB < 0)
                                        {
                                            _cmpTable[cmpid] = -1;
                                            Logger.LogStatic(string.Format("e1 = {0}({1}), e2 = {2}({3}), result = -1", e1.Id, e1.Name, e2.Id, e2.Name));
                                            return -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _cmpTable[cmpid] = res;
                    Logger.LogStatic(string.Format("e1 = {0}, e2 = {1}, result = {2}", e1.Name, e2.Name, res));
                    return res;
                }
                _cmpTable[cmpid] = res;
                Logger.LogStatic(string.Format("e1 = {0}, e2 = {1}, result = {2}", e1.Name, e2.Name, res));
                return res;
            }
        }
    }
}
