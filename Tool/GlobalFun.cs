using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//这个代码需要仔细研究，必须明确g_fPreHour
namespace bottleWithHALCON.Tool
{
  public  class GlobalFun

    {

		public float g_fPreHour;
	   public GlobalFun()
        {

        }

        ~GlobalFun()
        {

        }

        public String GetCharRun(String strCharRoot)
        {
			if (strCharRoot == "")
			{
				return "";
			}
			String strCharRun = "";
			for (int i = 0; i < strCharRoot.Length; ++i)
			{
				char ch = strCharRoot[i];				//这里有两个问题1.c#
				switch (ch)
				{
					case '<':
						{
							i++;
							while ('>' != (ch = strCharRoot[i]))
							{
								strCharRun += ch;
								i++;
								if (i == strCharRoot.Length)		//Length
								{
									break;
								}
							}
						}
						break;
					default:
						strCharRun += ch;
						break;
				}
			}
			return strCharRun;
		}

		public String GetCharGenerate(String strCharRun)
        {

			DateTime DT = System.DateTime.Now;
			if (strCharRun == "")
			{
				return "";
			}
			String str;
			if (DT.Hour < g_fPreHour)				//这里是计算时间,获取小时
			{

				//QTime的作用是计算时间，只计算
				//str = QDateTime::currentDateTime().addDays(-1).toString("yyyyMMdd");
				str = DT.AddDays(-1).ToString("yyyyMMdd");
			}
			else
			{
				//str = QDateTime::currentDateTime().toString("yyyyMMdd");
				str = DT.ToString("yyyyMMdd");                  //获取当前时间并转为yyyyMMdd格式

			}
			//查找替换#date#
			return strCharRun.Replace("#date#", str);
		}

		public String GetCharRule(String strCharRoot)
        {
			DateTime DT = System.DateTime.Now;
			if (strCharRoot == "")
			{
				return "";
			}
			//查找替换#date#
			//#在正则表达式起什么作用
			//#在正则表达式中没有具体含义，属于常规字符
			String strTemp = strCharRoot.Replace("#date#", DT.ToString("yyyyMMdd"));
			String strCharRule = "";
			for (int i = 0; i < strCharRoot.Length; ++i)
			{
				
				char ch = strCharRoot[i];
				switch (ch)				
				{
					case '<'://不识别均为*
						{
							i++;
							while ('>' != (ch = strCharRoot[i]))
							{
								strCharRule += "*";
								i++;
								if (i == strCharRoot.Length)
								{
									break;
								}
							}
						}
						break;
					default:
						{
							if (ch >= '0' && ch <= '9')
							{
								strCharRule += "0";
							}
							else
							{
								if (ch >= 'a' && ch <= 'z')
								{
									strCharRule += "a";
								}
								else
								{
									if (ch >= 'A' && ch <= 'Z')
									{
										strCharRule += "A";
									}
									else
									{
										strCharRule += "*";
									}
								}
							}
						}
						break;
				}
			}
			return strCharRule;
		}
	}
}
