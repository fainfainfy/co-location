using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;



namespace qualified_co_location_pattern_mining
{
    public partial class Form : System.Windows.Forms.Form
    {
        public string inputfilepath = "";
        public static string outputfilepath = "";
        public static string outputname = "";
        public double prev = 0.00;
        public int range = 0;
        public double rand = 0;
        public double cds = 0;

        public List<Dictionary<string, Dictionary<string, HashSet<int>>>> fnn = new List<Dictionary<string, Dictionary<string, HashSet<int>>>>();//存放频繁模式表实例
        public List<string> printf = new List<string>();//频繁模式,形式为1+2+3{12-16-19;12-16-20;13-17-21}(0.5,0.4,0.6)=0.4    
        public List<string> printf11 = new List<string>();//频繁模式,形式为1+2+3{12-16-19;12-16-20;13-17-21}(0.5,0.4,0.6)=0.4    
                                                          // public List<List<string>> listfn = new List<List<string>>();//频繁模式,形式为1+2+3
        public List<string> listfn = new List<string>();//频繁模式,形式为1+2+3
                                                        // public List<List<string>> listout = new List<List<string>>();//频繁模式输出表实例1+2+3{1-2-3；2-3-4；}=0.2,0.4,0.4
                                                        //Dictionary<string, Dictionary<string, HashSet<string>>> dicstar = new Dictionary<string, Dictionary<string, HashSet<string>>>();//存放星星表
        Dictionary<int, Dictionary<int, Dictionary<int, HashSet<int>>>> dicstar = new Dictionary<int, Dictionary<int, Dictionary<int, HashSet<int>>>>();//存放星星表
        public Dictionary<string, string> fsnn = new Dictionary<string, string>();//物化
        public Dictionary<int, Instance> sin = new Dictionary<int, Instance>();////存放所有实例输入变量
        public Dictionary<string, string> prvevaluedic = new Dictionary<string, string>();//存放所有频繁模式的prev

        public List<string> begindataList = new List<string>();//s1 放的是有序的特征实例及其位置，s22放每个特征的个数，也是有序的
        public List<int> TypeCountList = new List<int>(); // TypeCountList 放的是特征对应的个数 如：[0]=0，[1]=5,[2]=7说明特征1有5个实例，特征2有2个实例 两个数组差为特征的实例范围
        public List<int> TypeInsList = new List<int>();   //TypeInsList 放的是实例对应的特征  如：[1]=1，[2]=1,[3]=2 说明实例1，2都是特征1的实例 实例3是特征2的实例

        public SortedList<int, SortedSet<int>> INs = new SortedList<int, SortedSet<int>>();//实例邻居集，int为实例号，set中是实例的所有邻居

        public List<string> star = new List<String>();
        public List<string> fs11 = new List<string>();//二阶频繁模式的布尔频繁值，与FS1一一对应 
        public List<double> fs1 = new List<double>();//二阶频繁模式及其参与率[0]=1+2：pri,prj=PI
        public List<string> fname = new List<String>();//特征编号与真实特征的映射 [1]=A [2]=B
        public List<int> fs2 = new List<int>();//存放对于每个i其在队列中的位置，作用s22


        public List<int> sizecounti = new List<int>();//每个特征可能生成的最大阶数
        public List<string> starneighbor = new List<string>();//某特征在星星中与其二阶频繁的特征集
        public List<double> fsn = new List<double>();
        public List<int> fsn2 = new List<int>();

        public Form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button_begin_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_prev.Text == "" || textBox_r.Text == "")
                { MessageBox.Show("此值不能为缺省，请重新输入！"); }
                else if (double.Parse(textBox_prev.Text) > 0.00 && double.Parse(textBox_prev.Text) < 1.00 && double.Parse(textBox_r.Text) > 0)
                {
                    prev = double.Parse(textBox_prev.Text);
                    rand = double.Parse(textBox_r.Text);
                    cds = double.Parse(textBox_CDS.Text);
                }
                else
                {
                    MessageBox.Show("此值不合法，请重新输入！");
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("输入错误");
            }
            beginfile();

            Thread thread = new Thread(() =>
            {

                InDatasouce F = new DataIO();
                List<string> inputlist = new List<string>();
                inputlist = F.inputdata(inputfilepath);
                //  ------------------------------------------------------------------------------------获得初始数据
                string data = System.DateTime.Today.DayOfYear.ToString() + "shuchu" + System.DateTime.Now.Minute.ToString();
                outputname = outputfilepath + "\\" + data + ".txt";
                //=============================================================================计算程序运行时间
                Stopwatch timecost = new Stopwatch();
                timecost.Start();
                beginfile();
                timecost.Stop();
                TimeSpan ts2 = timecost.Elapsed;
                Console.WriteLine("Stopwatch总共花费{0}ms.", ts2.TotalMilliseconds);
                MessageBox.Show("计算完毕，共花费" + ts2.TotalMilliseconds + "ms,为您输出结果到txt");

            });
            thread.Start();//启动线程
            thread.IsBackground = true;//后台运行
                                       //   MessageBox.Show("输出成功！");
            this.Close();
            MessageBox.Show("输出成功！");
        }


        public void beginfile()
        {
            InDatasouce F = new DataIO();
            List<string> inputlist = new List<string>();
            inputlist = F.inputdata(inputfilepath);
            List<double> ax = new List<double>();
            List<double> ay = new List<double>();
            //  ------------------------------------------------------------------------------------获得初始数据
            #region
            // MessageBox.Show("开始读入数据");
            for (int i = 0; i < inputlist.Count; i++)//获得初始数据
            {
                string[] inputa = inputlist[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                //  string s = inputa[1].ToString() + "." + inputa[0].ToString() + "(" + System.Convert.ToDouble(inputa[2]) + "," + System.Convert.ToDouble(inputa[3]) + ")";
                StringBuilder s = new StringBuilder();
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCodea = (int)asciiEncoding.GetBytes(inputa[1].ToString())[0];
                s.Append(intAsciiCodea.ToString());
                s.Append(".");
                s.Append(inputa[0].ToString());
                s.Append("(");
                s.Append(System.Convert.ToDouble(inputa[2]));
                s.Append(",");
                s.Append(System.Convert.ToDouble(inputa[3]));
                begindataList.Add(s.ToString());
                ax.Add(System.Convert.ToDouble(inputa[2]));
                ay.Add(System.Convert.ToDouble(inputa[3]));
            }
            MessageBox.Show("数据读取完毕，进入物化阶段");
            //字典序排序

            //-----------------------------------------------------------------------------------------全局变量赋值
            begindataList.Sort();//begindataList 放的是有序的特征实例及其位置，TypeCountList放每个特征的个数，也是有序的
            begindataList.Insert(0, "begin");
            int k = 1;
            int s3no = 1;
            begindataList.Add("over");//begindataList有一个over结尾，避免无法将其中的最后一个特征加不进TypeCountList的情况
            // s2.Add("0");
            TypeCountList.Add(0);
            TypeInsList.Add(0);
            TypeInsList.Add(1);
            DataGrid dg = new DataGrid();//=========================================================datagrad对象
            dg.instancelized(begindataList);           
            for (int i = 1; i < begindataList.Count - 1; i++)
            {
                string[] a = begindataList[i].Split('.');
                string[] b = begindataList[i + 1].Split('.');

                if (!b[0].Equals(a[0]))
                {
                    s3no++;
                    TypeInsList.Add(s3no);//TypeInsList放的是实例对应的特征  如：[1]=1，[2]=1,[3]=2 说明实例1，2都是特征1的实例 实例3是特征2的实例
                    TypeCountList.Add(k);//TypeCountList放的是特征对应的个数 如：[0]=0，[1]=5,[2]=7说明特征1有5个实例，特征2有2个实例 两个数组差为特征的实例范围
                    k++;
                }
                else
                {
                    TypeInsList.Add(s3no);
                    k++;
                }
            }

            #endregion
            //--------------------------------------------------------------------------------------物化            
            int maxx = (int)ax.Max() / ((int)rand);//获得最大行数
            int maxy = (int)ay.Max() / ((int)rand);//获得最大列数
            //dg.Grid(maxx, maxy, (int)rand, dg.instancelized(begindataList));//格化
            //--------------------------------------------------------------------------------------建立实例邻居表
            INs=dg.InstanceNeighbor(maxx, maxy, (int)rand, dg.Grid(maxx, maxy, (int)rand, dg.instancelized(begindataList)), dg.instancelized(begindataList));
            //--------------------------------------------------------------------------------------产生模式 



        }


        private void button_datasource_Click(object sender, EventArgs e)//选择输入文件
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "txt文件|*.txt|所有文件|*.*";
                openFileDialog.ShowDialog();
                this.Text = openFileDialog.FileName;
                //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\   
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FilterIndex = 1;
                inputfilepath = this.Text.ToString();
                label_inputpath.Text = "input：" + this.Text.ToString();
            }
            catch { MessageBox.Show("please choose an input file"); }
        }

        private void button_scan_Click(object sender, EventArgs e)//选择文件输出位置
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.Text = path.SelectedPath;
            label_outputpath.Text = path.SelectedPath;
          //  outputfilepath = textBox_file.Text.ToString();
        }

        private void buttonTest_Click(object sender, EventArgs e)//测试
        {
            #region
            /*
            string str = "a.1(2.5,4.3)";
            stringinstance o=new stringinstance();
            o.splitinstance(".","(",",",")",str);
            string m = "";
            for (int i = 0; i < o.splitinstance(".", "(", ",", ")", str).count; i++)
            {
                 
                 m=o.splitinstance(".", "(", ",", ")", str)[i]; //----这就是你要的结果
                messagebox.show(m);
            }
            */
            #endregion//测试字符串分割实例

            //StringInstance s = new StringInstance();
            //string str = "12,13,15,16";
            //string[] ti = { "," };

            //for (int i = 0; i < s.SplitString1(str, ',').Count; i++)
            //{
            //    MessageBox.Show(s.SplitString1(str, ',')[i]);
            //}

            List<SortedSet<int>> list = new List<SortedSet<int>>();//装实例表
            List<SortedSet<int>> listPA = new List<SortedSet<int>>();//装PA包括的实例
            //List<SortedSet<int>> listline = new List<SortedSet<int>>();
            List<int> listline = new List<int>();//装每个PA包括的行号

            SortedSet<int> ssin1 = new SortedSet<int>() { 1, 4, 7 };
            SortedSet<int> ssin2 = new SortedSet<int>() { 1, 4, 8 };
            SortedSet<int> ssin3 = new SortedSet<int>() { 2, 5, 9 };
            SortedSet<int> ssin4 = new SortedSet<int>() { 3, 6, 9 };
            SortedSet<int> ssin5 = new SortedSet<int>() { };
            list.Add(ssin1); list.Add(ssin2); list.Add(ssin3); list.Add(ssin4); list.Add(ssin5);

            //首先赋值第一行初始化
            //listPA.Add(list[0]);//PA初始化            
            // listline.Add(0);//行号初始化
            //for (int i = 1; i < list.Count(); i++)//讲实例表分到不同的PA中
            //{
            //    int j = listPA.Count - 1;
            //    SortedSet<int> tmpset = new SortedSet<int>();
            //    if (listPA[j].Overlaps(list[i]))
            //    {
            //        listPA[j].UnionWith(list[i]);
            //    }
            //    else
            //    {
            //        listline.Add(i);
            //        listPA.Add(list[i]);
            //    }
            //}

            Occupation oc = new Occupation();
            //oc.PA(list);
            //for (int i = 0; i < oc.PA(list).Count(); i++)//讲实例表分到不同的PA中
            //{
            //    StringBuilder m = new StringBuilder();
            //    foreach (var item in oc.PA(list)[i])
            //    {
            //        m.Append(item.ToString()); m.Append(",");
            //    }
            //    MessageBox.Show(m.ToString());
            //}
            //=========================================================================公共邻域

            List<SortedSet<int>> listcn = new List<SortedSet<int>>();//装实例邻居表
            List<SortedSet<int>> listCA = new List<SortedSet<int>>();//装CA包括的实例

            SortedSet<int> csin1 = new SortedSet<int>() { 1, 4, 7, 10, 11, 14 };
            SortedSet<int> csin2 = new SortedSet<int>() { 1, 4, 8, 10, 14 };
            SortedSet<int> csin3 = new SortedSet<int>() { 2, 5, 9, 12, 15 };
            SortedSet<int> csin4 = new SortedSet<int>() { 3, 6, 9, 13, 16 };
            listcn.Add(csin1); listcn.Add(csin2); listcn.Add(csin3); listcn.Add(csin4);

            //List<int> line = new List<int>();

            //foreach (var item in oc.PA(list)[oc.PA(list).Count() - 1])//取出listline
            //{
            //    line.Add(item);
            //}
            //for (int i = 0; i < line.Count()-1; i++)
            //{
            //    SortedSet<int> tmpset = new SortedSet<int>();
            //    tmpset = listcn[line[i]];
            //    for (int j = line[i]+1; j < line[i + 1]; j++)
            //    {
            //        tmpset.UnionWith(listcn[j]);
            //    }
            //    listCA.Add(tmpset);
            //}
            //listPA = oc.PA(list);
            //listCA = oc.CA(oc.PA(list)[oc.PA(list).Count() - 1], listcn);
            //for (int i = 0; i < listCA.Count(); i++)//讲实例表分到不同的PA中
            //{
            //    StringBuilder m1 = new StringBuilder();
            //    foreach (var item in listCA[i])
            //    {
            //        m1.Append(item);
            //        m1.Append(",");
            //    }
            //    MessageBox.Show(m1.ToString());
            //}
            double index = oc.OccupationIndex(oc.CA(oc.PA(list)[oc.PA(list).Count() - 1], listcn), oc.PA(list));
            MessageBox.Show(index.ToString());

            //for (int i = 0; i < listline.Count(); i++)//讲实例表分到不同的PA中
            //{

            //    MessageBox.Show(listline[i].ToString());
            //}
        }

    }
}
