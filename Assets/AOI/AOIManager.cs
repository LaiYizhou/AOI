using System;
using System.Collections.Generic;
using System.Text;
using AOI.Define;
using UnityEngine;

namespace AOI
{
    /// <summary>
    /// the Manager of AOI LogicEntities
    /// </summary>

    public class AOIManager
    {
        private string _name;
        private LogicEntity _dummyHead;
        private LogicEntity _dummyTail;
        
        public LogicEntity DummyHead => this. _dummyHead;
        public LogicEntity DummyTail => this. _dummyTail;
        
        public AOIManager(string name)
        {
            this._name = name;
            
            this._dummyHead = CreateLogicEntity(ConstDefine.ID_NAN);
            this._dummyTail = CreateLogicEntity(ConstDefine.ID_NAN);

            this._dummyHead.XNext = this._dummyTail;
            this._dummyHead.YNext = this._dummyTail;
            this._dummyTail.XPre = this._dummyHead;
            this._dummyTail.YPre = this._dummyHead;
        }

        public void Destroy()
        {
            this._name = null;
            this._dummyHead = null;
            this._dummyTail = null;
        }
        
        public LogicEntity CreateLogicEntity(int id, int layer = ConstDefine.ALL_ONE, float radius = 0)
        {
            var entity = new LogicEntity(this, id, layer, radius);
            return entity;
        }

        public void DestroyLogicEntity(LogicEntity entity)
        {
            entity.XPre.XNext = entity.XNext;
            entity.XNext.XPre = entity.XPre;
            entity.YPre.YNext = entity.YNext;
            entity.YNext.YPre = entity.YPre;
            
            entity.Destroy();
        }

        public IList<LogicEntity> GetLogicEntitiesInRangeByCenter(Vector2 centerPos, float range,
            int layer = ConstDefine.ALL_ONE, int maxCount = int.MaxValue, bool isSort = false)
        {
            IList<LogicEntity> res = new List<LogicEntity>();
            LogicEntity head = _dummyHead;
            bool isEnter = false;
            while (head != _dummyTail)
            {
                if (IsEntityInLinkedListRange(head, centerPos, range))
                {
                    isEnter = true;
                    if (IsEntityInRange(head, centerPos, range))
                        res.Add(head);
                }
                else
                {
                    if (isEnter)
                        break;
                }
                
                head = head.XNext;
            }

            return res;
        }

        public string DebugShow()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("X-Axis");
            var cur = _dummyHead.XNext;
            while (cur != _dummyTail)
            {
                sb.Append(cur).Append("->");
                cur = cur.XNext;
            }
            sb.Append("null\n");
                
            sb.AppendLine("Y-Axis");
            cur = _dummyHead.YNext;
            while (cur != _dummyTail)
            {
                sb.Append(cur).Append("->");
                cur = cur.YNext;
            }
            sb.Append("null\n");

            string res = sb.ToString();
            Debug.LogError(res);
            return res;
        }

        private bool IsEntityInRange(LogicEntity entity, Vector2 center, float radius)
        {
            float sqrDis = (entity.GetPos() - center).SqrMagnitude();
            return sqrDis <= radius * radius;
        }

        private bool IsEntityInLinkedListRange(LogicEntity entity, Vector2 center, float radius)
        {
            if (entity == this._dummyTail || entity == this._dummyHead)
                return false;

            if (!entity.IsActive)
                return false;

            return Math.Abs(entity.GetPos().x - center.x) <= radius;
        }
        
    }
}
