using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkiSen
{

    public class RokujuSike
    {
        public RokujuSike(int no, string name, int jouBoku, int kaBoku)
        {
            this.no = no;
            this.name = name;
            this.jouBoku = jouBoku;
            this.kaBoku = kaBoku;
        }
        public int no;
        public string name;
        public int jouBoku; //上卦
        public int kaBoku; //下卦

    }
    public class HachiKe
    {
        public enum InYou
        {
            In = 0,
            You
        }
        public HachiKe(int hachiBoku, string shouZou, string imi, string people, string houi, int ekiSu, InYou inyou)
        {
            this.hachiKe = hachiBoku;
            this.shouZou = shouZou;
            this.imi = imi;
            this.people = people;
            this.houi = houi;
            this.ekiSu = ekiSu;
            this.inyou = inyou;
        }

        public HachiKe Clone()
        {
            return (HachiKe)MemberwiseClone();
        }
        public int hachiKe;     //八卦
        public string shouZou;  //正象
        public string imi;      //意味
        public string people;   //人
        public string houi;     //方位
        public int ekiSu;       //易数
        public InYou inyou;     //陰陽

    }

    class RokujuYonBokuMatrix
    {

    }

    public class TableMng
    {
        static TableMng tblMng = new TableMng();
        public static TableMng GetTblManage()
        {
            return tblMng;
        }

        public class RokujuSikeTbl
        {
            public Dictionary<int, RokujuSike> dicRokujuuSike = null;

            public RokujuSike GetRokujuSike( int sikeNo )
            {
                if (!dicRokujuuSike.ContainsKey(sikeNo) ) return null;
                return dicRokujuuSike[sikeNo];
            }
        }
        public RokujuSikeTbl rokujuSikeTbl = new RokujuSikeTbl();


        public class HachiKeTbl
        {
            public Dictionary<int, HachiKe> dicHachiKe = null;

            public HachiKe GetHachiKe(int hachiboku)
            {
                return dicHachiKe.First(x => x.Value.hachiKe == hachiboku).Value;
            }
            public HachiKe GetHachiKeByEkisu(int ekisu)
            {
                return dicHachiKe.First(x => x.Value.ekiSu == ekisu).Value;
            }
          


        }
        public HachiKeTbl hachiKeTbl = new HachiKeTbl();

        public class RokujuYonKeTbl
        {
            /// <summary>
            /// 主キー
            /// </summary>
            public int[] joKe;
            public Dictionary<int, int[]> dicRokujuYonBoku;

            public RokujuSike GetRokujuYonSike(int jouKe, int kake)
            {
                var item = dicRokujuYonBoku[kake];

                int index = Array.IndexOf(joKe, jouKe);

                var sikeNo = item[index];

                return tblMng.rokujuSikeTbl.GetRokujuSike(sikeNo);

            }
        }
        public RokujuYonKeTbl rokujuYonKeTbl = new RokujuYonKeTbl();

        public TableMng()
        {
            rokujuSikeTbl.dicRokujuuSike = new Dictionary<int, RokujuSike>()
            {
                {1  ,new RokujuSike(1  ,"乾為天"    ,1  ,1)},
                {2  ,new RokujuSike(2  ,"坤為地"    ,8  ,8)},
                {3  ,new RokujuSike(3  ,"水雷屯"    ,6  ,4)},
                {4  ,new RokujuSike(4  ,"山水蒙"    ,7  ,6)},
                {5  ,new RokujuSike(5  ,"水天需"    ,6  ,1)},
                {6  ,new RokujuSike(6  ,"天水訟"    ,1  ,6)},
                {7  ,new RokujuSike(7  ,"地水師"   ,8  ,6)},
                {8  ,new RokujuSike(8  ,"水地比"    ,6  ,8)},
                {9  ,new RokujuSike(9  ,"風天小畜"   ,5  ,1)},
                {10 ,new RokujuSike(10 ,"天沢履"    ,1  ,2)},
                {11 ,new RokujuSike(11 ,"地天泰"    ,8  ,1)},
                {12 ,new RokujuSike(12 ,"天地否"    ,1  ,8)},
                {13 ,new RokujuSike(13 ,"天火同人"   ,1  ,3)},
                {14 ,new RokujuSike(14 ,"火天大有"   ,3  ,1)},
                {15 ,new RokujuSike(15 ,"地山謙"    ,8  ,7)},
                {16 ,new RokujuSike(16 ,"雷地予"    ,4  ,8)},
                {17 ,new RokujuSike(17 ,"沢雷随"    ,2  ,4)},
                {18 ,new RokujuSike(18 ,"山風蟲"    ,7  ,5)},
                {19 ,new RokujuSike(19 ,"地沢臨"    ,8  ,2)},
                {20 ,new RokujuSike(20 ,"風地観"    ,5  ,8)},
                {21 ,new RokujuSike(21 ,"火雷噬ごう" ,3 ,4) },
                {22 ,new RokujuSike(22 ,"山火賁"    ,7  ,3)},
                {23 ,new RokujuSike(23 ,"山地剥"   ,7  ,8)},
                {24 ,new RokujuSike(24 ,"地雷復"    ,8  ,4)},
                {25 ,new RokujuSike(25 ,"天雷无妄"   ,1  ,4)},
                {26 ,new RokujuSike(26 ,"山天大畜"   ,7  ,1)},
                {27 ,new RokujuSike(27 ,"山雷頤"    ,7  ,4)},
                {28 ,new RokujuSike(28 ,"沢風大過"   ,2  ,5)},
                {29 ,new RokujuSike(29 ,"坎為水"    ,6  ,6)},
                {30 ,new RokujuSike(30 ,"離為火"    ,3  ,3)},
                {31 ,new RokujuSike(31 ,"沢山咸"    ,2  ,7)},
                {32 ,new RokujuSike(32 ,"雷風恒"    ,4  ,5)},
                {33 ,new RokujuSike(33 ,"天山遯"    ,1  ,7)},
                {34 ,new RokujuSike(34 ,"雷天大壮"   ,4  ,1)},
                {35 ,new RokujuSike(35 ,"火地晋"    ,3  ,8)},
                {36 ,new RokujuSike(36 ,"地火明夷"   ,8  ,3)},
                {37 ,new RokujuSike(37 ,"風火家人"   ,5  ,3)},
                {38 ,new RokujuSike(38 ,"火沢けい"   ,3  ,2)},
                {39 ,new RokujuSike(39 ,"水山けん"   ,6  ,7)},
                {40 ,new RokujuSike(40 ,"雷水解"    ,4  ,6)},
                {41 ,new RokujuSike(41 ,"山沢損"    ,7  ,2)},
                {42 ,new RokujuSike(42 ,"風雷益"    ,5  ,4)},
                {43 ,new RokujuSike(43 ,"沢天夬"    ,2  ,1)},
                {44 ,new RokujuSike(44 ,"天風こう"   ,1  ,5)},
                {45 ,new RokujuSike(45 ,"沢地萃"    ,2  ,8)},
                {46 ,new RokujuSike(46 ,"地風升"    ,8  ,5)},
                {47 ,new RokujuSike(47 ,"沢水困"    ,2  ,6)},
                {48 ,new RokujuSike(48 ,"水風井"    ,6  ,5)},
                {49 ,new RokujuSike(49 ,"沢火革"    ,2  ,3)},
                {50 ,new RokujuSike(50 ,"火風鼎"    ,3  ,4)},
                {51 ,new RokujuSike(51 ,"震為雷"    ,4  ,4)},
                {52 ,new RokujuSike(52 ,"艮為山"    ,7  ,7)},
                {53 ,new RokujuSike(53 ,"風山漸"    ,5  ,7)},
                {54 ,new RokujuSike(54 ,"雷沢帰妹"   ,4  ,2)},
                {55 ,new RokujuSike(55 ,"雷火豊"    ,4  ,3)},
                {56 ,new RokujuSike(56 ,"火山旅"    ,3  ,7)},
                {57 ,new RokujuSike(57 ,"巽為風"    ,5  ,5)},
                {58 ,new RokujuSike(58 ,"兌為沢"    ,2  ,2)},
                {59 ,new RokujuSike(59 ,"風水渙"    ,5  ,6)},
                {60 ,new RokujuSike(60 ,"水沢節"    ,6  ,2)},
                {61 ,new RokujuSike(61 ,"風沢中孚"   ,5  ,2)},
                {62 ,new RokujuSike(62 ,"雷山小過"   ,4  ,7)},
                {63 ,new RokujuSike(63 ,"水火既済"   ,6  ,3)},
                {64 ,new RokujuSike(64 ,"火水未済"   ,3  ,6)},
            };

            hachiKeTbl.dicHachiKe = new Dictionary<int, HachiKe>()
            {
                {0b111, new HachiKe(0b111, "天","健全",  "父",    "西北", 1, HachiKe.InYou.You)},
                {0b011, new HachiKe(0b011, "沢","喜ぶ",  "少女",  "西",   2, HachiKe.InYou.In)},
                {0b101, new HachiKe(0b101, "火","美しい","中女",  "南",   3, HachiKe.InYou.In)},
                {0b001, new HachiKe(0b001, "雷","動く",  "長男",  "東",   4, HachiKe.InYou.You)},
                {0b110, new HachiKe(0b110, "風","伏入",  "長女",  "東南", 5, HachiKe.InYou.In)},
                {0b010, new HachiKe(0b010, "水","陥る",  "中男",  "北",   6, HachiKe.InYou.You)},
                {0b100, new HachiKe(0b100, "山","止まる","少男",  "東北", 7, HachiKe.InYou.You)},
                {0b000, new HachiKe(0b000, "地","柔順",  "母",    "南西", 8, HachiKe.InYou.In)},


            };

            rokujuYonKeTbl = new RokujuYonKeTbl();
            rokujuYonKeTbl.joKe = new int[]{0b000, 0b100, 0b010, 0b110, 0b001, 0b101, 0b011, 0b111};
            rokujuYonKeTbl.dicRokujuYonBoku = new Dictionary<int, int[]>()
            {
                { 0b111,new int[]{11,26, 5, 9,34,14,43, 1 } },
                { 0b011,new int[]{19,41,60,61,54,38,58,10 } },
                { 0b101,new int[]{36,22,63,37,55,30,49,13 } },
                { 0b001,new int[]{24,27, 3,42,51,21,17,25 } },
                { 0b110,new int[]{46,18,48,57,32,50,28,44 } },
                { 0b010,new int[]{ 7, 4,29,59,40,64,47, 6 } },
                { 0b100,new int[]{15,52,39,53,62,56,31,33 } },
                { 0b000,new int[]{ 2,23, 8,20,16,35,45,12 } },

            };
        }
    }
}
