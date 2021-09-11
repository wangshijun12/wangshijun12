﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PrepAddGridGroup : MonoBehaviour
{
    public Transform Root;
    public bool IsUse;
    [SerializeField]
    GridGroup_MinPrep gridData;
    // Start is called before the first frame update
    void Start()
    {
        EventTriggerListener.Get(gameObject).onDown = OnPointerDown;
        EventTriggerListener.Get(gameObject).onUp = OnPointerUp;
    }
     public void SetUse()
    {
        IsUse = true;
        //所有的子类消失
        PoolMgr.Recycle(gridData);
        if (GridGroupMgr.Inst.IsOverPrep())
        {
            GridGroupMgr.Inst.RefreshPrepGridGroup();
        }
    }
    
    public void SetGridData(GridGroup_MinPrep v)
    {
        gridData = v;
    }
    public void OnPointerUp(GameObject eventData)
    {
        if (IsUse)
        {
            return;
        }
        DragingGridMgr.Inst.SetDragUp(this);
        Debug.Log("OnPointerUp   " + transform.name);

        if (GridGroupMgr.Inst.RefreshMainGrid())//如果当前可以放置 刷新主面板显示
        {
            //////////////////////////////////////刷新主面板显示时候执行该操作 GridGroupMgr.Inst.RevertswGrid();//还原预览的格子
            //////////////////////////////////////刷新主面板显示时候执行该操作 GridGroupMgr.Inst.ClearGrid(); //如果有可以销毁的 实现销毁并添加积分
            SetUse();//设置当前待放入的group为使用过了

        }
        else
        {
            //使用失败 返回到指定位置
        }

        //待放格子区 检测是否可以放置 不能放的变灰 无法使用
    }
    public void OnPointerDown(GameObject eventData)
    {
        if (IsUse )
        {
            return;
        }
        DragingGridMgr.Inst.SetDragDown(gridData);
        Debug.Log("OnPointerDown   " + transform.name);
    }
    public void Reset()
    {
        IsUse = false;
    }
}
