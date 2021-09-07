﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GridTools
{
    static Vector3 Pos = new Vector3(0, 0);
    public static void AddGrids(Transform root, GroupBase data, Image obj)
    {
        float width = obj.rectTransform.rect.width;
        float height = obj.rectTransform.rect.height;
        var info = data.DataArray;
        int h_1 = data.H_count - 1;
        for (int i = 0; i < data.H_count; i++)
        {
            for (int j = 0; j < data.W_count; j++)
            {
                int _id = info[i,j];
                if (data.Isbg)
                {
                    if (data.Grid[i, j].Image == null)
                    {
                        var bg = Object.Instantiate(obj);
                        bg.transform.parent = root;
                        Pos.x = (j - data.W_count * 0.5f + 0.5f) * width;
                        Pos.y = (h_1 - i - data.H_count * 0.5f + 0.5f) * height;
                        bg.transform.localPosition = Pos;
                        data.Grid[i, j].IsUse = _id != 0;
                        data.Grid[i, j].Image = bg;
                        data.Grid[i, j].Status = _id;
#if UNITY_EDITOR
                        data.Grid[i, j].Text = bg.transform.Find("Text").GetComponent<Text>();
                        if (data.Grid[i, j].IsUse)
                            data.Grid[i, j].Text.text = i + "  " + j ;
                        else
                            data.Grid[i, j].Text.text = "";
#endif
                    }
                }
                else if (_id == 1)
                {
                    var bg = Object.Instantiate(obj);
                    bg.transform.parent = root;
                    Pos.x = (j - data.W_count * 0.5f + 0.5f) * width;
                    Pos.y = (h_1 - i - data.H_count * 0.5f + 0.5f) * height;
                    bg.transform.localPosition = Pos;
                }
            }
        }
    }

}