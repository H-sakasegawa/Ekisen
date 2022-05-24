using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EkiSen
{
    public partial class rad3Pen : Form
    {
        enum JouKake
        {
            kake = 0,   //下卦
            jouke       //上卦
        }

        class BookInf
        {

            public string name;
            public ExplanationReader.ExplanationSheet data;

            public BookInf(string name, ExplanationReader.ExplanationSheet data)
            {
                this.name = name;
                this.data = data;
            }
            public override string ToString()
            {
                return name;
            }
        }

        class TestInf
        {
            public TestInf(int ekiSuKake, int ekiSuJouke, int mark)
            {
                TableMng tblMng = TableMng.GetTblManage();

                hachiKeKake = tblMng.hachiKeTbl.GetHachiKeByEkisu(ekiSuKake);
                hachiKeJouke = tblMng.hachiKeTbl.GetHachiKeByEkisu(ekiSuJouke);
                this.mark = mark;

                //裏卦
                int value = ~hachiKeKake.hachiKe & 0b111;
                hachiKeUraKake = tblMng.hachiKeTbl.GetHachiKe(value);

                value = ~hachiKeJouke.hachiKe & 0b111;
                hachiKeUraJouke = tblMng.hachiKeTbl.GetHachiKe(value);

                //伏卦
                hachiKeHusiKake = hachiKeKake.Clone();
                hachiKeHusiJouke = hachiKeJouke.Clone();

                int[] HusiKakeItems = new int[] { hachiKeKake.hachiKe, hachiKeJouke.hachiKe };

                //　TryValue[] 1,2 3 | 4,5,6
                //             0,1,2 | 3,4,5
                //             -------------- 
                // idx           0   |   1     (TryValue[2] - 1) / 3
                int idx = (mark - 1) / 3;

                // bit         0,1,2 | 0,1,2    (TryValue[2] - 1) % 3
                int bit = (mark - 1) % 3;

                value = 0x0001 << bit;
                int newHachiKe = HusiKakeItems[idx] ^ value;
                //置き換えた八朴で置き換え
                if(idx==0) hachiKeHusiKake = tblMng.hachiKeTbl.GetHachiKe(newHachiKe);
                else hachiKeHusiJouke = tblMng.hachiKeTbl.GetHachiKe(newHachiKe);


            }
            public HachiKe hachiKeKake;
            public HachiKe hachiKeJouke;
            public int mark;
            //UraKake
            public HachiKe hachiKeUraKake;
            public HachiKe hachiKeUraJouke;
            //HusiKake
            public HachiKe hachiKeHusiKake;
            public HachiKe hachiKeHusiJouke;


        }

        TableMng tblMng = TableMng.GetTblManage();
        int TryNum = 0;
        int[] HachiKeValue = new int[2];
        int[] HachiKeValueHusiKake = new int[2];
        int[] HachiKeValueUraKake = new int[2];
        List<int> lstKakeMark = new List<int>();
        List<TestInf> lstTest= new List<TestInf>();

        int curPageNo = 0;
        Image curImage = null;
        ExplanationReader.ExplanationData curMainData = null;
        bool bDispExplanation = true;

        public rad3Pen()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            picExplanation1.Dock = DockStyle.Fill;
            picExplanation2.Dock = DockStyle.Fill;
            txtExplanation1.Dock = DockStyle.Fill;
            txtExplanation2.Dock = DockStyle.Fill;

            picExplanation1.BorderStyle = BorderStyle.None;
            picExplanation2.BorderStyle = BorderStyle.None;
            txtExplanation1.BorderStyle = BorderStyle.None;
            txtExplanation2.BorderStyle = BorderStyle.None;


            //拡大縮小で縦横比率を維持
            picExplanation1.SizeMode = PictureBoxSizeMode.Zoom;
            picExplanation2.SizeMode = PictureBoxSizeMode.Zoom;


            this.MinimumSize = new Size(this.Size.Width, this.Size.Height);


        }


        /// <summary>
        /// シートの画像データ項目に設定されている画像数を取得
        /// </summary>
        /// <param name="sheetName">シート名</param>
        /// <param name="pictureID">データ項目識別</param>
        /// <returns></returns>
        public int PictureNum(string sheetName, int pictureID)
        {
            return 0;
        }

        /// <summary>
        /// シートの画像データ項目に設定されている画像から
        /// 指定されたIndexの画像をピクチャーボックスに描画
        /// </summary>
        /// <param name="sheetName">シート名</param>
        /// <param name="pictureID">データ項目識別</param>
        /// <param name="pictureIndex">表示画像Index</param>
        /// <param name="pic">画像表示先PictureBox</param>
        /// <returns></returns>
        public int DispPicture(string sheetName, int pictureID, int pictureIndex, PictureBox pic)
        {

            return 0;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            lstKakeMark.Clear();
            lblTestCnt.Text = "";

            if (rad3PenZei.Checked)
            {
                //３編筮
                int randValue = SanPenZei();
                if (TryNum >= 3)
                {
                    DispSenboku();
                    //卦名（かめい） 表示
                    DispKamei();

                    lblEkisu.Text += string.Format("-{0}", lstKakeMark[0]);

                    TryNum = 0;
                    lstKakeMark.Clear();
                    bDispExplanation = true;
                }
                else
                {
                    ClearDisp();
                    //ランダム易数表示
                    if (TryNum == 1)
                    {
                        lblEkisu.Text = string.Format("{0}", randValue);
                    }
                    else
                    {
                        if (TryNum == 2) lblEkisu.Text += string.Format("-{0}", randValue);
                    }
                }

            }
            else
            {
                TryNum = 0;
                //６編筮
                RokuPenZei();
                DispSenboku();
                //卦名（かめい） 表示
                DispKamei();

            }

        }


        /// <summary>
        /// ３編筮
        /// </summary>
        private int SanPenZei()
        {
            int Sheed = (int)DateTime.Now.Ticks;
            Random rand = new Random(Sheed);

            if(TryNum==0)
            {
                ClearPanel(panel2);
                ClearPanel(panel3);

            }
            int ekiSu = 0;
            if (TryNum < 2)
            {
                ekiSu = rand.Next(1, 9); //1以上、9未満

                HachiKeValue[TryNum] = tblMng.hachiKeTbl.GetHachiKeByEkisu(ekiSu).hachiKe;
                //裏卦(全ビット反転）
                HachiKeValueUraKake[TryNum] = ~HachiKeValue[TryNum] & 0b111;
            }
            else
            {
                ekiSu = rand.Next(1, 7);
                lstKakeMark.Add(ekiSu); //1以上、7未満
            }

            TryNum++;

            //3回目は易数ではありません。
            return ekiSu;
        }
        /// <summary>
        /// ６編筮
        /// </summary>
        private void RokuPenZei()
        {
            int Sheed = (int)DateTime.Now.Ticks;
            Random rand = new Random(Sheed);
            Array.Clear(HachiKeValue, 0, HachiKeValue.Length);

            for (int idx = 0; idx < 2; idx++)
            {
                for (int i = 0; i < 3; i++)
                {
                    int ekisu = rand.Next(1, 9); //1以上、9未満
                    var hachiboku = tblMng.hachiKeTbl.GetHachiKeByEkisu(ekisu);

                    int bit = (int)hachiboku.inyou << i;
                    HachiKeValue[idx] |= bit;
                }
            }
            //裏卦(全ビット反転）
            for (int i = 0; i < 2; i++)
            {
                HachiKeValueUraKake[i] = ~HachiKeValue[i] & 0b111;
            }
        }
        /// <summary>
        /// Reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            TryNum = 0;
            lstTest.Clear();
            lblTestCnt.Text = "";
            lblEkisu.Text = "";
            ClearDisp();

        }
        private void ClearDisp()
        {
            ClearPanel(panel2);
            ClearPanel(panel3);
            lblKamei.Text = "";
            lblKamei2.Text = "";

            ClearExplanation();
        }

        private void ClearExplanation()
        {
            lblPage.Text = string.Format("0/0");
            lstBooks.Items.Clear();
            picExplanation1.Image = null;
            picExplanation2.Image = null;
            txtExplanation1.Text = "";
            txtExplanation2.Text = "";
        }

        private void ClearPanel(Panel panel)
        {
            foreach (PictureBox pic in panel.Controls)
            {
                pic.Dispose();
            }
            panel.Controls.Clear();

        }


        //卦名（かめい） 表示
        private void DispKamei()
        {
            var sike = tblMng.rokujuYonKeTbl.GetRokujuYonSike(HachiKeValue[(int)JouKake.jouke], HachiKeValue[(int)JouKake.kake]);
            if (sike == null) return;

            lblKamei.Text = string.Format("{0}：{1}", sike.no, sike.name);

            if (radHusiKake.Checked)
            {
                sike = tblMng.rokujuYonKeTbl.GetRokujuYonSike(HachiKeValueHusiKake[(int)JouKake.jouke], HachiKeValueHusiKake[(int)JouKake.kake]);
            }else
            {
                sike = tblMng.rokujuYonKeTbl.GetRokujuYonSike(HachiKeValueUraKake[(int)JouKake.jouke], HachiKeValueUraKake[(int)JouKake.kake]);
            }
            if (sike == null) return;
            lblKamei2.Text = string.Format("{0}：{1}", sike.no, sike.name);
        }
        private void DispSenboku()
        {

 


            DispSenbokuPanel(panel2, HachiKeValue, lstKakeMark);

            if (radHusiKake.Checked)
            {
                //伏卦（乱数６で決まった項目を反転）
                Array.Copy(HachiKeValue, HachiKeValueHusiKake, HachiKeValueHusiKake.Length);
                foreach (var mark in lstKakeMark)
                {
                    //　TryValue[] 1,2 3 | 4,5,6
                    //             0,1,2 | 3,4,5
                    //             -------------- 
                    // idx           0   |   1     (TryValue[2] - 1) / 3
                    int idx = (mark - 1) / 3;

                    // bit         0,1,2 | 0,1,2    (TryValue[2] - 1) % 3
                    int bit = (mark - 1) % 3;

                    int value = 0x0001 << bit;
                    HachiKeValueHusiKake[idx] = HachiKeValue[idx] ^ value;

                }
                DispSenbokuPanel(panel3, HachiKeValueHusiKake, null, false);//伏卦
            }
            else if (radUraKake.Checked)
            {
                DispSenbokuPanel(panel3, HachiKeValueUraKake, null, false);//裏卦
            }

            //説明資料読み込み
            LoadExplanation();
            //説明表示
            ShowExplanation();
        }
        private void DispSenbokuPanel(Panel panel, int[] HachiBokuValue, List<int> lstMark, bool bMark = true)
        {
            HachiKe hachiBokuKaKe = tblMng.hachiKeTbl.GetHachiKe(HachiBokuValue[(int)JouKake.kake]);
            HachiKe hachiBokuJouKe = tblMng.hachiKeTbl.GetHachiKe(HachiBokuValue[(int)JouKake.jouke]);
 
            DispSenbokuPanel(panel, hachiBokuKaKe, hachiBokuJouKe, lstMark, bMark);
        }

        private void DispSenbokuPanel(Panel panel, int kaKe, int JouKe, int mark, bool bMark = true)
        {
            int[] hachiBoku = new int[2];
            HachiKe hachiBokuKaKe = tblMng.hachiKeTbl.GetHachiKeByEkisu(kaKe);
            HachiKe hachiBokuJouKe = tblMng.hachiKeTbl.GetHachiKeByEkisu(JouKe);

            List<int> lstMark = new List<int>();
            lstMark.Add(mark);
            DispSenbokuPanel(panel, hachiBokuKaKe, hachiBokuJouKe, lstMark, bMark);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="hachiBokuKaKe">//下卦</param>
        /// <param name="hachiBokuJouKe">//上卦</param>
        /// <param name="mark"></param>
        /// <param name="bMark"></param>
        private void DispSenbokuPanel(Panel panel, HachiKe hachiBokuKaKe, HachiKe hachiBokuJouKe, List<int> lstMark, bool bMark = true)
        {
            //List<PictureBox>[] lstPictures = new List<PictureBox>[2];
            HachiKe[] hachiBoku = new HachiKe[] { hachiBokuKaKe, hachiBokuJouKe };

            ClearPanel(panel);

            PictureBox[] pictures = new PictureBox[6];

            int top = panel.Height - 30;
            int iMark = 0;
            for (int iHachi = 0; iHachi < 2; iHachi++)
            {
                if (iHachi == 1)
                {
                    top -= 10;
                }
                int bit = 0x01;
                for (int i = 0; i < 3; i++)
                {
                    iMark++;
                    PictureBox targetPic;
                    if ((hachiBoku[iHachi].hachiKe & bit << i) == 0)
                    {
                        targetPic = picSenbokuIn;
                    }
                    else
                    {
                        targetPic = picSenbokuYou;
                    }

                    PictureBox pic = CopyPic(targetPic);
                    top -= pic.Height;
                    pic.Left = 10;
                    pic.Top = top;
                    panel.Controls.Add(pic);

                    if (bMark && lstMark!=null)
                    {
                        if (lstMark.Contains(iMark))
                        {
                            PictureBox pic2 = CopyPic(picMark);
                            panel.Controls.Add(pic2);
                            pic2.Left = pic.Left + pic.Width;
                            pic2.Top = pic.Top + (pic.Height - picMark.Height) / 2;

                        }
                    }
                    top -= 5;
                }
            }
            PictureBox CopyPic(PictureBox targetPic)
            {
                PictureBox pic = new PictureBox();
                pic.Image = targetPic.Image;
                pic.Width = targetPic.Width;
                pic.Height = targetPic.Height;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                pic.Visible = true;

                return pic;
            }


        }

        //伏卦チェックボックス
        private void radHusiKake_CheckedChanged(object sender, EventArgs e)
        {
            if (!radHusiKake.Checked) return;
            DispSenbokuPanel(panel3, HachiKeValueHusiKake, null, false);
            //卦名（かめい） 表示
            DispKamei();
            LoadExplanation();

        }
        //裏卦チェック簿kk巣
        private void radUraKake_CheckedChanged(object sender, EventArgs e)
        {
            if (!radUraKake.Checked) return;

            DispSenbokuPanel(panel3, HachiKeValueUraKake, null, false);
            //卦名（かめい） 表示
            DispKamei();
            LoadExplanation();
        }



        private void LoadExplanation()
        {
            RokujuSike sike;
            int hachike_jouke;
            int hachike_kake;

            if (radExplanation1.Checked)
            {
                hachike_jouke = HachiKeValue[(int)JouKake.jouke];
                hachike_kake = HachiKeValue[(int)JouKake.kake];
            }
            else
            {
                if (radHusiKake.Checked)
                {
                    hachike_jouke = HachiKeValueHusiKake[(int)JouKake.jouke];
                    hachike_kake = HachiKeValueHusiKake[(int)JouKake.kake];
                }
                else
                {
                    hachike_jouke = HachiKeValueUraKake[(int)JouKake.jouke];
                    hachike_kake = HachiKeValueUraKake[(int)JouKake.kake];
                }
            }

            sike = tblMng.rokujuYonKeTbl.GetRokujuYonSike(hachike_jouke, hachike_kake);

            LoadExplanation(sike.no);
        }
        private void LoadExplanation(int sikeNo)
        {
            sikeNo = 6;
            string fileName = string.Format(@"Data\64K{0:00}.xlsx", sikeNo);
            //Exeファイルパス
            string exePath = Path.GetDirectoryName(Application.ExecutablePath);

            string excelPath = Path.Combine(exePath, fileName);

            ExplanationReader er = new ExplanationReader();
            er.ReadExcel(excelPath);

            lstBooks.Items.Clear();
            foreach (var sheet in er.dicSheet)
            {
                int idx = lstBooks.Items.Add(new BookInf(sheet.Key, sheet.Value));
            }
            if(lstBooks.Items.Count>0)
            {
                lstBooks.SelectedIndex = 0;
            }
        }

        private void lstBooks_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowExplanation();
        }


        private void  ShowExplanation()
        {
            //メイン説明文表示
            ShowMainPage(1);

            //爻辞の説明
            ShowSubPage();
        }



        private void ShowMainPage(int pageNo)
        {
            BookInf book = (BookInf)lstBooks.SelectedItem;
            if (book == null)
            {
                ClearExplanation();
                return;
            }
            picExplanation1.Visible = false;
            txtExplanation1.Visible = false;

            curMainData = book.data.dic["メイン"];
            if (curMainData.pictureInfos.Count > 0)
            {
                ShowPage(book, curMainData, pageNo, picExplanation1);
            }
            else
            {
                ShowPage(book, curMainData, pageNo, txtExplanation1);
            }
        }

        private void ShowSubPage()
        {
            BookInf book = (BookInf)lstBooks.SelectedItem;
            if (book == null) return;
            if (lstKakeMark.Count <= 0) return;
            int value = lstKakeMark[0];
            if (value > 0)
            {
                picExplanation2.Visible = false;
                txtExplanation2.Visible = false;

                if (book.data.dic.ContainsKey(value.ToString()))
                {
                    var data = book.data.dic[value.ToString()];
                    if (data.pictureInfos.Count > 0)
                    {
                        ShowPage(book, data, 1, picExplanation2);
                    }
                    else
                    {
                        ShowPage(book, data, 1, txtExplanation2);
                    }

                    
                }else
                {
                    picExplanation2.Image = null;
                    txtExplanation2.Text = "";
                }
            }
        }

        private void ShowPage(BookInf book, ExplanationReader.ExplanationData curData, int pageNo, PictureBox pic)
        {
            pic.Image = null;
            if (curMainData == null) return;

            pic.Visible = true;

            if (pageNo > curData.pictureInfos.Count) return;
            if (curData.pictureInfos[pageNo - 1] == null) return;
            curPageNo = pageNo;

            if (curPageNo >= 0)
            {
                ImageConverter imgconv = new ImageConverter();
                curImage = (Image)imgconv.ConvertFrom(curData.pictureInfos[pageNo - 1].pictureData.Data);
                pic.Image = curImage;

                lblPage.Text = string.Format("{0}/{1}", curPageNo, curData.pictureInfos.Count);
            }
            else
            {
                pic.Image = null;
                lblPage.Text = string.Format("{0}/{1}", 0, 0);
            }
        }

        private void ShowPage(BookInf book, ExplanationReader.ExplanationData curData, int pageNo, TextBox txtBox)
        {
            txtBox.Text = "";
            if (curMainData == null) return;

            txtBox.Visible = true;

            if (pageNo > curData.textInfos.Count) return;
            if (curData.textInfos[pageNo - 1] == null) return;
            curPageNo = pageNo;

            if (curPageNo >= 0)
            {
                txtBox.Text = curData.textInfos[pageNo-1];

                lblPage.Text = string.Format("{0}/{1}", curPageNo, curData.textInfos.Count);
            }
            else
            {
                txtBox.Text = "";
                lblPage.Text = string.Format("{0}/{1}", 0, 0);
            }
        }

        private void PageDown()
        {
            if (curMainData == null || curPageNo <= 1) return;

            ShowMainPage(curPageNo - 1);
        }
        private void PageUp()
        {
            if (curMainData == null) return;

            if (curMainData.pictureInfos.Count > 0)
            {
                if (curPageNo >= curMainData.pictureInfos.Count) return;
            }
            else
            {
                if (curPageNo >= curMainData.textInfos.Count) return;
            }
            ShowMainPage(curPageNo + 1);
        }

        // "<"ボタン
        private void button7_Click(object sender, EventArgs e)
        {
            PageDown();
        }

        // ">"ボタン
        private void button5_Click(object sender, EventArgs e)
        {
            PageUp();
        }

        // "|<"ボタン
        private void button4_Click(object sender, EventArgs e)
        {
            ShowMainPage(1);
        }

        // ">|"ボタン
        private void button6_Click(object sender, EventArgs e)
        {
            ShowMainPage(curMainData.pictureInfos.Count);

        }
        /// <summary>
        /// 学習用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            lblEkisu.Text = "";
            if (lstTest.Count==0)
            {
                //テストデータ作成
                for (int iKake = 1; iKake <= 8; iKake++)
                {
                    for (int iJouKe = 1; iJouKe <= 8; iJouKe++)
                    {
                        for (int iMark = 1; iMark <= 6; iMark++)
                        {
                            lstTest.Add(new TestInf(iKake, iJouKe, iMark));


                        }
                    }
                }
            }

            //学習用ボタン押下時は、毎回説明を消去
            ClearExplanation();
            bDispExplanation = false;

            int Sheed = (int)DateTime.Now.Ticks;
            Random rand = new Random(Sheed);

            int testNo = rand.Next(1, lstTest.Count);
            TestInf inf = lstTest[testNo - 1];

            lstTest.RemoveAt(testNo - 1);

            HachiKeValue[(int)JouKake.kake] = inf.hachiKeKake.hachiKe;
            HachiKeValue[(int)JouKake.jouke] = inf.hachiKeJouke.hachiKe;
            //伏卦
            HachiKeValueHusiKake[(int)JouKake.kake] = inf.hachiKeHusiKake.hachiKe;
            HachiKeValueHusiKake[(int)JouKake.jouke] = inf.hachiKeHusiJouke.hachiKe;
            //裏卦
            HachiKeValueUraKake[(int)JouKake.kake] = inf.hachiKeUraKake.hachiKe;
            HachiKeValueUraKake[(int)JouKake.jouke] = inf.hachiKeUraJouke.hachiKe;

            lstKakeMark.Clear();

            lstKakeMark.Add(inf.mark);
            DispSenbokuPanel(panel2, HachiKeValue, lstKakeMark, true);

            if (radHusiKake.Checked)
            {
                DispSenbokuPanel(panel3, HachiKeValueHusiKake, null, false);//伏卦
            }
            else
            {
                DispSenbokuPanel(panel3, HachiKeValueUraKake, null, false);//裏卦
            }
            //卦名（かめい） 表示
            DispKamei();

            lblTestCnt.Text = string.Format("{0}/{1}", 384 - lstTest.Count, 384);
            lblEkisu.Text = string.Format("{0}-{1}-{2}", inf.hachiKeKake.ekiSu, inf.hachiKeJouke.ekiSu, inf.mark);

        }

        private void button8_Click(object sender, EventArgs e)
        {
            bDispExplanation = true;

            //説明資料読み込み
            LoadExplanation();
            //説明表示
            ShowExplanation();

        }
        //説明表示（八卦）
        private void radExplanation1_CheckedChanged(object sender, EventArgs e)
        {
            if (!radExplanation1.Checked) return;
            if (!bDispExplanation) return;
            button8_Click(null, null);
        }

        //説明表示（伏卦、裏卦）
        private void radExplanation2_CheckedChanged(object sender, EventArgs e)
        {
            if (!radExplanation2.Checked) return;
            if (!bDispExplanation) return;
            button8_Click(null, null);
        }
    }
}
