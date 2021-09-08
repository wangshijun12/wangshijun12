﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGroupMgr : MonoBehaviour
{
    public Image preproot;
    public Image defgrid;
    public Image mingrid;
    public Image usegrid;
    public Image swgrid;
    public Image draggrid;
    public Dictionary<int, int> Postox { get; } = new Dictionary<int, int>()
    {
        [-270] = 0,
        [-210] = 1,
        [-150] = 2,
        [-90] = 3,
        [-30] = 4,
        [30] = 5,
        [90] = 6,
        [150] = 7,
        [210] = 8,
        [270] = 9,
    };
    public Dictionary<int, int> Postoy { get; } = new Dictionary<int, int>()
    {
        [-270] = 9,
        [-210] = 8,
        [-150] = 7,
        [-90] = 6,
        [-30] = 5,
        [30] = 4,
        [90] = 3,
        [150] = 2,
        [210] = 1,
        [270] = 0,
    };
    List<GridData> swGridList = new List<GridData>();//临时展示在面包上的格子
    public GridGroup_Ground gridGroup_Ground;//主面板数据
    public static GridGroupMgr Inst;
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
        preproot = ResourceMgr.Inst.LoadRes<Image>("Prefab/addgridbg");
        mingrid = ResourceMgr.Inst.LoadRes<Image>("Prefab/blockmin");
        defgrid = ResourceMgr.Inst.LoadRes<Image>("Prefab/blockno");
        usegrid = ResourceMgr.Inst.LoadRes<Image>("Prefab/block");
        swgrid = ResourceMgr.Inst.LoadRes<Image>("Prefab/blocksw");
        draggrid = ResourceMgr.Inst.LoadRes<Image>("Prefab/blockdrag");
        GameGloab.Sprites["usegrid"] = usegrid.sprite;
        GameGloab.Sprites["mingrid"] = mingrid.sprite;
        GameGloab.Sprites["defgrid"] = defgrid.sprite;
        GameGloab.Sprites["swgrid"] = swgrid.sprite;
        GameGloab.Sprites["draggrid"] = draggrid.sprite;

        gridGroup_Ground = new GridGroup_Ground();
    }
    /// <summary>
    /// 还原临时显示的grid
    /// </summary>
    public void RevertswGrid()
    {
        foreach (var v in swGridList)
        {
            v.Revert();
        }
        swGridList.Clear();
    }
    /// <summary>
    /// 检测现在移动到的位置能不能放
    /// </summary>
    public void CheckAvailable(Vector2 _pos)
    {
        Vector2 pos = _pos;
        var gdata = DragingGridMgr.Inst.gridData;
        var alldata = gridGroup_Ground;
        if (M_math.Even(gdata.H_count))
            pos.y += 30;
        if (M_math.Even(gdata.W_count))
            pos.x -= 30;

        //根据 pos 计算出 i j 对应的grid
        int w = OutGridPos(pos.x);
        if (!Postox.ContainsKey(w))
        {
            RevertswGrid();
            return;//超出 不处理
        }
        int h = OutGridPos(pos.y);
        if (!Postoy.ContainsKey(h))
        {
            RevertswGrid();
            return;//超出 不处理
        }
        int h_index = Postoy[h];//h:w  坐标
        int w_index = Postox[w];
        RevertswGrid();//先清理再筛选
        if (CanAddPrep(gdata, alldata, h_index, w_index, true))
        {
            foreach (var v in swGridList)
            {
                v.Status = 2;
            }
        }
        else
        {
            RevertswGrid();
        }
    }

    /// <summary>
    /// 判断GridGroup_prep能不能放
    /// </summary>
    /// <param name="gdata"></param>
    /// <param name="alldata"></param>
    /// <param name="h_index"></param>
    /// <param name="w_index"></param>
    /// <param name="isadd">是否处理swgrid列表</param>
    /// <returns></returns>
    private bool CanAddPrep(GroupBase gdata, GridGroup_Ground alldata, int h_index, int w_index, bool isadd = false)
    {
        bool h_even = M_math.Even(gdata.H_count);
        bool w_even = M_math.Even(gdata.W_count);
        int all_maxh = alldata.H_count - 1;
        int all_maxw = alldata.W_count - 1;
        int h_ban = h_index - ((int)(gdata.H_count * 0.5f));
        int w_ban = w_index - ((int)(gdata.W_count * 0.5f));
        int add_h = h_even ? 1 : 0;
        int add_w = w_even ? 1 : 0;
        //当前选中的位置 根据拖动出来的展开获取需要处理的grid
        int all_h;
        int all_w;
        for (int i = 0; i < gdata.H_count; i++)
        {
            for (int j = 0; j < gdata.W_count; j++)
            {
                //将gdata ij的位置 与alldata的_i_j对应起来
                all_h = h_ban + i + add_h;
                all_w = w_ban + j + add_w;
                if (all_h < 0 || all_h > all_maxh || all_w < 0 || all_w > all_maxw)
                {
                    return false; //超出边界
                }
                if (gdata.Grid[i, j].IsUse)
                {
                    if (alldata.Grid[all_h, all_w].IsUse)
                    {
                        return false;//若gdata有数据 alldata也有数据 说明不能放
                    }
                    else if (isadd)
                    {
                        swGridList.Add(alldata.Grid[all_h, all_w]);
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 根据坐标的值 装换成最靠近的规整坐标的值
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int OutGridPos(float index)
    {
        //x -270 到 270 为0到9   y 270 到 -270 为0到9
        //x    : -270  -210  -150  -90  -30   30   90   150   210   270
        //30倍数   -9    -7    -5   -3   -1    1   3     5     7     9
        //        0       1     2    3    4    5    6    7    8      9   
        // 坐标数除30 得到奇数向下取整  偶数向上取整
        float num = index / 30;//30倍数
        int p_n = num > 0 ? 1 : -1;//正负值
        float num_abs = M_math.Abs(num);
        int endind = 0;
        if (M_math.Even((int)num_abs))
            endind = (int)(30 * p_n * Math.Ceiling(num_abs));//向上取整
        else
            endind = (int)(30 * p_n * (float)Math.Floor(num_abs));//向下取整
        if (M_math.Abs(endind - index) < 28)
            return endind;
        else
            return 0;
    }
}