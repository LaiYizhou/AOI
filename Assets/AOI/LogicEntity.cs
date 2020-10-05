using System;
using System.Collections.Generic;
using AOI.Define;
using UnityEngine;

namespace AOI
{
    /// <summary>
    /// the Entity in AOI BlackBox
    /// </summary>

    public class LogicEntity
    {
        private readonly AOIManager _aoiManager;
        
        private readonly int _id;
        
        //todo
        private readonly int _layer;
        private readonly float _radius;

        private float _x;
        private float _y;

        public LogicEntity XPre;
        public LogicEntity XNext;
        public LogicEntity YPre;
        public LogicEntity YNext;

        public bool IsActive => !(this.XPre == null && this.XNext == null && this.YPre == null && this.YNext == null);
        
        public override string ToString()
        {
            return $" [{this._id} ({this._x}, {this._y})] ";
        }

        public LogicEntity(AOIManager aoiManager, int id, int layer, float radius)
        {
            _aoiManager = aoiManager;
            
            this._id = id;
            this._layer = layer;
            this._radius = radius;

            this._x = ConstDefine.FLOAT_NAN;
            this._y = ConstDefine.FLOAT_NAN;

            this.XPre = null;
            this.XNext = null;
            this.YPre = null;
            this.YNext = null;
        }

        public void Destroy()
        {
            this._x = ConstDefine.FLOAT_NAN;
            this._y = ConstDefine.FLOAT_NAN;

            this.XPre = null;
            this.XNext = null;
            this.YPre = null;
            this.YNext = null;
        }
        
        public int GetId()
        {
            return this._id;
        }

        public Vector2 GetPos()
        {
            return new Vector2(_x, _y);
        }

        public void SetPos(Vector2 pos)
        {
            SetPos(pos.x, pos.y);
        }
        
        public void SetPos(float x, float y)
        {
            if (IsActive)
            {
                if (CheckNeedRefresh(x, y))
                {
                    int xRes = GetRefreshXDir(x);
                    int yRes = GetRefreshYDir(y); 
                    if (xRes != 0 || yRes != 0)
                    {
                        this._x = x;
                        this._y = y;
                        if (xRes != 0)
                            RefreshXPos(xRes);
                        if (yRes != 0)
                            RefreshYPos(yRes);
                    }
                }
            }
            else
            {
                this._x = x;
                this._y = y;
                InsertXPos();
                InsertYPos();
            }
        }

        public IList<LogicEntity> GetLogicEntityInRange(float range, int type = ConstDefine.ALL_ONE,
            int maxCount = int.MaxValue, bool isSort = false)
        {
            IList<LogicEntity> res = new List<LogicEntity>();
            var cur = this.XNext;
            while (cur != _aoiManager.DummyTail)
            {
                if (IsEntityInMyLinkedListRange(cur, range))
                {
                    if (IsEntityInMyRange(cur, range))
                    {
                        res.Add(cur);
                    }

                    cur = cur.XNext;
                }
                else
                {
                    break;
                }
            }
            
            cur = this.XPre;
            while (cur != _aoiManager.DummyHead)
            {
                if (IsEntityInMyLinkedListRange(cur, range))
                {
                    if (IsEntityInMyRange(cur, range))
                    {
                        res.Add(cur);
                    }

                    cur = cur.XPre;
                }
                else
                {
                    break;
                }
            }

            return res;
        }
        
        private bool CheckNeedRefresh(float x, float y)
        {
            if (Math.Abs(x - this._x) < ConstDefine.EPSILON && Math.Abs(y - this._y) < ConstDefine.EPSILON)
                return false;

            return true;
        }

        private int GetRefreshXDir(float x)
        {
            if (this.XPre == this._aoiManager.DummyHead && this.XNext == this._aoiManager.DummyTail)
                return 0;
            
            float preX;
            float nextX;
            
            if (this.XPre != this._aoiManager.DummyHead)
            {
                preX = this.XPre._x;
                return x >= preX ? 0 : -1;
            }
            
            if (this.XNext != this._aoiManager.DummyTail)
            {
                nextX = this.XNext._x;
                return x <= nextX ? 0 : 1;
            }
            
            preX = this.XPre._x;
            nextX = this.XNext._x;
            if (x >= preX && x <= nextX)
                return 0;
            else if (x < preX)
                return -1;
            else
                return 1;
        }

        private int GetRefreshYDir(float y)
        {
            if (this.YPre == this._aoiManager.DummyHead && this.YNext == this._aoiManager.DummyTail)
                return 0;
            
            float preY;
            float nextY;
            
            if (this.YPre != this._aoiManager.DummyHead)
            {
                preY = this.YPre._y;
                return y >= preY ? 0 : -1;
            }
            
            if (this.YNext != this._aoiManager.DummyTail)
            {
                nextY = this.YNext._y;
                return y <= nextY ? 0 : 1;
            }
            
            preY = this.YPre._y;
            nextY = this.YNext._y;
            if (y >= preY && y <= nextY)
                return 0;
            else if (y < preY)
                return -1;
            else
                return 1;
        }


        private void RefreshXPos(int dir)
        {
            if (dir > 0)
            {
                LogicEntity next = this.XNext;
                while (next != this._aoiManager.DummyTail && next.XNext != this._aoiManager.DummyTail && next.XNext._x < this._x)
                {
                    next = next.XNext;
                }

                var oldpre = this.XPre;
                var newPre = next;
                var newNext = newPre.XNext;

                oldpre.XNext = this.XNext;
                this.XNext.XPre = oldpre;

                newPre.XNext = this;
                this.XPre = newPre;
                this.XNext = newNext;
                if (newNext != this._aoiManager.DummyTail)
                    newNext.XPre = this;
            }
            else
            {
                LogicEntity pre = this.XPre;
                while (pre != this._aoiManager.DummyHead && pre.XPre != this._aoiManager.DummyHead && pre.XPre._x > this._x)
                {
                    pre = pre.XPre;
                }

                var oldNext = this.XNext;
                var newNext = pre;
                var newPre = pre.XPre;

                oldNext.XPre = this.XPre;
                this.XPre.XNext = oldNext;

                newNext.XNext = this;
                this.XNext = newNext;
                this.XPre = newPre;
                if (newPre != this._aoiManager.DummyHead)
                    newPre.XNext = this;
            }
        }
        
        private void RefreshYPos(int dir)
        {
            if (dir > 0)
            {
                LogicEntity next = this.YNext;
                while (next != this._aoiManager.DummyTail && next.YNext != this._aoiManager.DummyTail && next.YNext._y < this._y)
                {
                    next = next.YNext;
                }

                var oldpre = this.YPre;
                var newPre = next;
                var newNext = newPre.YNext;

                oldpre.YNext = this.YNext;
                this.YNext.YPre = oldpre;

                newPre.YNext = this;
                this.YPre = newPre;
                this.YNext = newNext;
                if (newNext != this._aoiManager.DummyTail)
                    newNext.YPre = this;
            }
            else
            {
                LogicEntity pre = this.YPre;
                while (pre != this._aoiManager.DummyHead && pre.YPre != this._aoiManager.DummyHead && pre.YPre._y > this._y)
                {
                    pre = pre.YPre;
                }

                var oldNext = this.YNext;
                var newNext = pre;
                var newPre = pre.YPre;

                oldNext.YPre = this.YPre;
                this.YPre.YNext = oldNext;

                newNext.YNext = this;
                this.YNext = newNext;
                this.YPre = newPre;
                if (newPre != this._aoiManager.DummyHead)
                    newPre.YNext = this;
            }
        }
        
        private void InsertXPos()
        {
            var head = this._aoiManager.DummyHead;
            var next = head.XNext;
            while (next != this._aoiManager.DummyTail && next._x < this._x)
            {
                head = next;
                next = head.XNext;
            }

            head.XNext = this;
            this.XPre = head;
            this.XNext = next;
            next.XPre = this;
        }
        
        private void InsertYPos()
        {
            var head = this._aoiManager.DummyHead;
            var next = head.YNext;
            while (next != this._aoiManager.DummyTail && next._y < this._y)
            {
                head = next;
                next = head.YNext;
            }

            head.YNext = this;
            this.YPre = head;
            this.YNext = next;
            next.YPre = this;
        }
        
        private bool IsEntityInMyRange(LogicEntity entity, float radius)
        {
            float sqrDis = (entity.GetPos() - this.GetPos()).SqrMagnitude();
            return sqrDis <= radius * radius;
        }

        private bool IsEntityInMyLinkedListRange(LogicEntity entity, float radius)
        {
            if (entity == this._aoiManager.DummyHead || entity == this._aoiManager.DummyTail)
                return false;

            if (!entity.IsActive)
                return false;

            return Math.Abs(entity.GetPos().x - this._x) <= radius;
        }
        
    }
}
