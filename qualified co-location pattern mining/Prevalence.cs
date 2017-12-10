using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qualified_co_location_pattern_mining
{
    class Prevalence
    {
        SortedList<string, List<SortedSet<int>>> listT = new SortedList<string, List<SortedSet<int>>>();
        SortedList<string, List<SortedSet<int>>> listN = new SortedList<string, List<SortedSet<int>>>();
        //==============================================================================给定一阶模式为TypeInsList给定一阶T为实例邻居表INs
        
        //==================================================================生成二阶模式及二阶模式邻居
        public SortedList<string, List<SortedSet<int>>>TwoSize(SortedList<int, SortedSet<int>> INs, double min_prev, List<int> TypeCountList, List<int> TypeinsList)
        {
            SortedList<string, List<SortedSet<int>>> T = new SortedList<string, List<SortedSet<int>>>();
            List<string> listT2 = new List<string>();
            List<double> listPI = new List<double>();
            List<int> listFI = new List<int>();
            listT2.Add("0");//存放形如i+j=的二阶
            listPI.Add(0);//存放i+j二阶是否频繁
            listFI.Add(0);//存放对于每个i其在队列中的位置，作用s22
            int feature = TypeCountList.Count;
            //int sum = 0;
            //for (int i = 1; i < feature - 1; i++)//-----------------------初始化二阶队列
            //{
            //    for (int j = i + 1; j < feature; j++)
            //    {
            //        listT2.Add(i + "+" + j);
            //        listPI.Add(0.00);
            //    }

            //    sum += feature - 1 - i;
            //    listFI.Add(sum);
            //}
            //List<SortedSet<int>> listf2 = new List<SortedSet<int>>();

            //for (int i = 1; i < listT2.Count(); i++)//--------------------测试二阶
            //{
            //    List<SortedSet<int>> listtest = new List<SortedSet<int>>();
            //    int starneibori = 0;
            //    int sumi = 0;
            //    int sumj = 0;
            //    double prev1 = 0.00;
            //    double previ = 0.00;
            //    double prevj = 0.00;
            //    string[] a = listT2[i].Split('+');
            //    int fi = int.Parse(a[0].ToString());
            //    int fj = int.Parse(a[1].ToString());
            //    //直接检查fi的邻居

            for (int i = 1; i < feature - 1; i++)//-----------------------初始化二阶队列
            {

                //建立一个矩阵存放特征i的所有二阶邻居，行为特征i的所有实例个数，列为fi邻居特征个数
                List<List<SortedSet<int>>> matrixij = new List<List<SortedSet<int>>>();
                //初始化二阶矩阵
                List<SortedSet<int>> subm = new List<SortedSet<int>>();//存放fi的实例的某一特征的邻居实例
                for (int ii = 0; ii < TypeCountList[i] - TypeCountList[i - 1]; ii++)
                {
                    for (int jj = ii+1; jj < feature - i; jj++)
                    {

                        SortedSet<int> s = new SortedSet<int>() { };
                        subm.Add(s);
                    }
                    matrixij.Add(subm);
                }

                //赋值
                for (int ii = TypeCountList[i-1]+1; ii < TypeCountList[i]+1; ii++)
                {
                     foreach (var item in INs[ii])
                    {
                        matrixij[ii - TypeCountList[i - 1] - 1][TypeinsList[item] - 1].Add(item);
                    }                     
                    
                }

                //针对j列，即特征j,计算i+j的模式参与度



            }

      
            

            //    if (!INs.ContainsKey(ai))
            //    { break; }
            //    else
            //    {
            //        foreach (var item in INs[ai])//对于ai,遍历ai的实例,对于AI的每个实例
            //        {
            //            SortedList<int, HashSet<int>> dic1 = new SortedList<int, HashSet<int>>();
            //            dic1 = item. 
            //                item.Value; //int为aj特征 hash为aj实例                   
            //            if (dic1.ContainsKey(aj))
            //            {
            //                //ai的某个实例中的aj实例集合
            //                dicij.Add(item.Key.ToString(), dic1[aj]);
            //                hashj.UnionWith(dic1[aj]);
            //            }
            //        }
            //    }
            //    //计算频繁度
            //    sumj = hashj.Count();
            //    sumi = dicij.Count();
            //    previ = double.Parse(dicij.Keys.Count().ToString()) / double.Parse((s22[ai] - s22[ai - 1]).ToString());
            //    prevj = double.Parse(hashj.Count().ToString()) / double.Parse((s22[aj] - s22[aj - 1]).ToString());
            //    prev1 = Math.Min(previ, prevj);
            //    fs1[i] = 0;
            //    if (Math.Round(prev1, 3) > prev || Math.Round(prev1, 3) == prev)
            //    {
            //        StringBuilder prevkkk = new StringBuilder();
            //        StringBuilder kkk = new StringBuilder(2);
            //        kkk.Append(fs11[i] + "=");
            //        prevkkk.Append(Math.Round(previ, 2));
            //        prevkkk.Append(",");
            //        prevkkk.Append(+Math.Round(prevj, 2));
            //        kkk.Append(prevkkk);
            //        //   printf11.Add(kkk.ToString());
            //        prvevaluedic.Add(fs11[i].ToString(), prevkkk.ToString() + ",");
            //        dic2n.Add(fs11[i], dicij);
            //        listfn.Add(fs11[i]);
            //        if (Math.Abs(previ - prevj) > cds || Math.Abs(previ - prevj) == cds)//若含有主导特征
            //        {
            //            kkk.Append(fs11[i] + "  is a DFCP and");
            //            string[] h = fs11[i].Split('+');
            //            if (previ > prevj)
            //            {
            //                kkk.Append("the dominant feature is：" + h[0] + (previ - prevj));

            //            }
            //            else
            //            {
            //                kkk.Append("the dominant feature is：" + h[1] + (prevj - previ));
            //            }

            //        }
            //        printf11.Add(kkk.ToString());
            //    }
            //}
            ////   MessageBox.Show("二阶频繁模式生成！");
            //fnn.Add(dic2n);
            //dicstar.Clear();
            //sin.Clear();
            //dicstar.Clear();
            return T;
        }

    }
}
