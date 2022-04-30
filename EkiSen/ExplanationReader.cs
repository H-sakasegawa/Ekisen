﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NPOI.XSSF.UserModel;

/// <summary>
/// 説明用Excelファイルから画像データを管理提供するクラス
/// </summary>
class ExplanationReader
{
    public class ExplanationData
    {
        public ExplanationData(string _id, ExcelReader.PictureInfo _pictureInfo)
        {
            id = _id;
            pictureInfos.Add(_pictureInfo);
        }
        public void AddPictureInfo(ExcelReader.PictureInfo _pictureInfo)
        {
            pictureInfos.Add(_pictureInfo);
        }
        public int GetMaxWidth()
        {
            int maxW = 0;
            foreach (var inf in pictureInfos)
            {
                if (maxW < inf.width) maxW = inf.width;
            }
            return maxW;
        }
        public int GetMaxHeight()
        {
            int maxH = 0;
            foreach (var inf in pictureInfos)
            {
                if (maxH < inf.height) maxH = inf.height;
            }
            return maxH;

        }
        public string id;
        public List<ExcelReader.PictureInfo> pictureInfos = new List<ExcelReader.PictureInfo>();
    }

    public class ExplanationSheet
    {

        public void Clear()
        {
            dic.Clear();
        }

        //Key:項目キー名
        public Dictionary<string, ExplanationData> dic = new Dictionary<string, ExplanationData>();

    }

    public Dictionary<string, ExplanationSheet> dicSheet = new Dictionary<string, ExplanationSheet>();

 


    public int ReadExcel(string excelFilePath)
    {
        if (!File.Exists(excelFilePath))
        {
            return -1;
        }
        dicSheet.Clear();

        var workbook = ExcelReader.GetWorkbook(excelFilePath, "xlsx");

        int sheetNum =  workbook.NumberOfSheets;
        for(int iSheet=0; iSheet<sheetNum; iSheet++)
        {
            XSSFSheet sheet = (XSSFSheet)((XSSFWorkbook)workbook).GetSheetAt(iSheet);

            ExplanationSheet expSheet = new ExplanationSheet();
            dicSheet.Add(sheet.SheetName, expSheet);
            ReadSheet(expSheet, sheet);
        }

        return 0;
    }
    private int ReadSheet(ExplanationSheet expSheet, XSSFSheet sheet)
    { 

        List<ExcelReader.PictureInfo> lstCellInfos = ExcelReader.GetPicture(sheet);

        int iRow = 0;
        while (true)
        {

            //説明項目キー文字
            string sKeyItem = ExcelReader.CellValue(sheet, iRow, 0);
            if (string.IsNullOrEmpty(sKeyItem)) break;

            var sKeyAry = sKeyItem.Split('\n');
            //string Explanation = ExcelReader.CellValue(sheet, iRow, 1);

            List<ExcelReader.PictureInfo> pictureInfos = lstCellInfos.FindAll(x => x.row == iRow + 1)
                                                                     .OrderBy(x => x.col).ToList();
            foreach (var sKey in sKeyAry)
            {
                if (pictureInfos != null && pictureInfos.Count > 0)
                {
                    foreach (var info in pictureInfos)
                    {
                        if (expSheet.dic.ContainsKey(sKey))
                        {
                            expSheet.dic[sKey].AddPictureInfo(info);
                        }
                        else
                        {
                            expSheet.dic.Add(sKey, new ExplanationData(sKey, info));
                        }
                    }
                }
                else
                {
                    expSheet.dic.Add(sKey, new ExplanationData(sKey, null));
                }
            }
            iRow++;
        }

        return 0;

    }

    public void Clear()
    {
        foreach (var sheet in dicSheet)
        {
            sheet.Value.dic.Clear();
        }
    }

    public ExplanationData GetExplanation(string sheetName, string sKey)
    {
        if (!dicSheet.ContainsKey(sheetName)) return null;

        
        if (!dicSheet[sheetName].dic.ContainsKey(sKey)) return null;
        return dicSheet[sheetName].dic[sKey];
    }

    public List<string> GetExplanationKeys(string sheetName)
    {
        if (!dicSheet.ContainsKey(sheetName)) return null;
        return dicSheet[sheetName].dic.Keys.ToList();
    }
}
