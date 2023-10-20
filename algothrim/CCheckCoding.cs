using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
//using HalconDotNet.HOperatorSet;
using static HalconDotNet.HOperatorSet;
using bottleWithHALCON.Tool;
using bottleWithHALCON.innerData;



namespace bottleWithHALCON.algothrim
{


	public struct s_pCoding
	{
		public bool bCharSingle;            //检测分隔符
		public bool bCharChn;              //字符检测
		public bool bDataOCR;              //日期识别
		public bool bGrayProjection;       //灰度投影分割
		public int iRecThreGray;            //日期识别灰度检测
		public bool bModelMatch;           //模板匹配检测
		public int iGray;                  //灰度值
		public float fScore;               //2015.1.21,增加匹配度,与s_pFindModel中的fScore有所不同，以s_pFindModel找到匹配模板后再于该值比较
		public s_pCreatModel spCreatModel; //建模参数
		public s_pFindModel spFindModel;   //模板匹配参数
										   //2015
		public bool bCharMatch;            //是否模板定位
		public int iCharEdge;              //字符对比度
		public int iCharWidth;             //字符宽度
		public int iCharHeiht;             //字符高度
		public int iCharGray;               //提取字符灰度
		public int iGrayMarkHeiht;          //灰度淹没高度
		public int iParititionCharWidth;      //拆分长度
		public int iParititionCharHeiht;      //拆分高度
		public int iParititionCharWidth2;
		public int iResultWidth;            //字符结果长度
		public int iResultHeiht;            //字符结果高度
		public int iResultNum1;              //字符结果个数第一行
		public int iResultNum2;              //字符结果个数第二行
		public int iRoiDeviationCenterX;            //ROI偏移X
		public int iIsHaveCapThresh;            //ROI偏移Y
		public int iRoiCharGary;                    //字符ROI区域灰度
		public int iIsHaveCapR1X;
		public int iIsHaveCapR1Y;
		public int iIsHaveCapR2X;
		public int iIsHaveCapR2Y;

		public double HeightL;   //字符高度
		public double WidthL;    //字符宽度

		public String sDataP;   //批次
		public int iPDataNum;    //批次所在行数
		public int iDataPone;    //批次所在首位
		public int iDataPtwo;    //批次所在末尾
		public int iDate;     //手动输入日期
		public bool bWriteDate;   //是否手动输入日期
		public bool bDataP;      //是否检测批次
		public int iCodeTyple;   //喷码类型

		public int iVertProjection;
		public int iCharSpace;
		public int iOpenWidth;

		public int iOneThreshEdge;         //边缘识别
		public int iOneNumEdge;
		public int iOneAreaEdge;
		public int ioneFuzzyArea;

		public int iTwoThreshEdge;         //边缘识别
		public int iTwoNumEdge;
		public int iTwoAreaEdge;
		public int iTwoFuzzyArea;

		public int iClipOneDistance;
		public int iClipTwoDistance;
		public int iClipThresh;

		public int iRaduis;
		public int iSelectArea;
		public float iCirculary;
		public int iErrAreaCap;//瓶盖异常区域
		public int iCharline;//字符行数
	};
	//内部参数，仅用于储存内部检测临时数据
	public struct s_tCoding
	//-----2015
	{
		public HTuple homMat2D;        //反射矩阵
		public HTuple homMat2DInv;     //逆反射矩阵
	};


	//图形参数
	public struct s_oCoding
	{
		//2015---
		public HTuple lLocModelID;      //定位模板ID

		public HObject regModelRectangle;  //模板距形区域
		public MyOri sCurrentOri;       //当前模板
		public HTuple lCharModelIDNumberDotPrint;       //字符数字模板ID
		public HTuple lCharModelIDNumberIndustrial;
		public HTuple lCharModelIDChar;     //字符字母模板ID
		public MyOri modelOri;         //坐标原点
		public HObject regModelRegion; //模板区域
		public HObject ROI;            //字符有效区域
		public MyOri sCharCurrentOri;  //当前匹配模板原点
		public HObject IsHaveCapRect;   //检测区域
		public HObject RoteImage;       //旋转图像
		public HObject CharRegion;   //选定单个字符

		s_oCoding()
		{
			lLocModelID = -1;
			lCharModelIDNumberDotPrint = -1;
			lCharModelIDNumberIndustrial = -1;
			lCharModelIDChar = -1;
		}
	};
	//调试中间结果图像（仅调试时使用，不同类型绘制不同颜色）
	public struct s_mCoding
	{
		public HObject regCheck;       //检测出的结果
		public HObject regCorrect;     //修正结果
		public HObject SegmentRect;     //显示中间变量
		public HObject ResultRect;      //显示识别结果
		public HObject CharSegment;    //字符分割
		public HObject CharRect;        //字符区域
		public HObject dynRect;        //字符动态分割结果
		public HObject OpenWidthRect;  //字符开运算结果
		public HObject LineRect;       //分行结果

		public int TempOneEdgeArea;
		public int TempOneEdgeNum;
		public int TempOneFuzzy;
		public int TempOneClip;
		public int TempOneParll;
		public String dispStr;   //显示字符
	};
	//他莫昔芬可治疗多巴胺激动剂抵抗性高泌乳血症患者的促性腺激素功能减退性性腺功能低下
	public class CCheckCoding:CommonCheck
    {
		public s_pCoding m_pCoding;    //数字参数
		public s_oCoding m_oCoding;    //图像参数
		public s_tCoding m_tCoding;    //临时参数
		public s_mCoding m_mCoding;    //中间结果（调试时使用）
		public CCheckCoding()
        {

        }
        ~CCheckCoding()
        {

        }

		public void readParam()
        {

        }

		public void writeParam()
        {

        }

		//需要油猴插件
		public void displayShape(HTuple lWindID)
        {
			//显示模板区域
			if (m_pCoding.bModelMatch)
			{
				//显示套冒区域
				
				SetColor(lWindID, "green");
				//disp_obj(m_oCoding.IsHaveCapRect,lWindID);
			}

			//显示调试图像
			//g_iUserPermit是一个全局变黄
			if (g_iUserPermit > 5)
			{
		

				SetColor(lWindID, "yellow");
				DispObj(m_mCoding.regCheck, lWindID);
				SetColor(lWindID, "blue");
				DispObj(m_mCoding.regCorrect, lWindID);


			}
			DispObj(m_mCoding.regCheck, lWindID);
			//显示识别结果
		}

		//定位
		public  s_CheckResult location(HObject hImg, bool bDebug/* = false*/, bool bUpdateOri/* = false*/, bool bCalib/* = false*/)
        {
			s_CheckResult sRsu = new s_CheckResult();
			HObject regCircle;
			HTuple lNum;
			HTuple tpRows, tpCols, OriAngle, scr;
			findModel(hImg, m_oCoding.lLocModelID,ref m_pCoding.spFindModel, out tpRows, out tpCols, out OriAngle, out scr);
			

			TupleLength(scr, out lNum);
			if (0 == lNum)
			{
				sRsu.iType = ERROR_CAPSTYPE;
				//未找到正确的定位目标!
				sRsu.strDescription = QObject::tr("Type of the caps is error,match falid!Score is 0");
				
				CopyObj(m_oCoding.regModelRectangle, out sRsu.regError, 1, -1);
				return sRsu;
			}
			else
			{

				m_oCoding.sCurrentOri.Row = tpRows[0].D();
				m_oCoding.sCurrentOri.Col = tpCols[0].D();
				m_oCoding.sCurrentOri.Angle = OriAngle[0].D();

			}

			if (bDebug)
			{


				GenCircle(out regCircle, m_oCoding.sCurrentOri.Row, m_oCoding.sCurrentOri.Col, 5);
				ConcatObj(m_mCoding.regCheck, regCircle, out m_mCoding.regCheck);

			}

			return sRsu;
		}

		public s_CheckResult check(HObject srcImg, bool bDebug/* = false*/)
		{
			s_CheckResult sRsu;

			//字符检测
			HTuple ConnectCharNum1, ConnectCharNum2;
			HObject select1, CharTr1, CharThresh1, ConnectChar1, smallrect1, select2, CharTr2, CharThresh2, ConnectChar2, smallrect2;
			HTuple CharR11, CharC11, CharR12, CharC12, AR11, AC11, CharArea1, CharR21, CharC21, CharR22, CharC22, AR21, AC21, CharArea2;

			HTuple centerR = -1, centerC = -1, TempSLC2 = -1;


			//模糊识别
			HObject RedufuzzyArea1, ThresfuzzyArea1, RedufuzzyArea2, ThresfuzzyArea2, clipStdRegionstemp2;

			HTuple fuzzyAreaValue1 = -1, fuzzyAreaValue2 = -1;
			HTuple hv_Class = -1, RowPaller1 = -1, RowPaller2 = -1, hv_Scrot2 = -1, hv_Scrot1 = -1;

			//上下缺字检测
			HObject ClipRegionClosing1, clipStdRegions1, ClipRegionClosing2, clipStdRegions2;
			HObject ThresClipArea1, ThresClipArea2, SortclipStdRegions2, SortclipStdRegions1;
			HTuple ClipRow11 = -1, ClipRow12 = -1, ClipRow21 = -1, ClipRow22 = -1, ClipC2 = -1, ClipC1 = -1;

			HTuple ClipNum1, ClipStand1, ClipNum2, ClipStand2;
			char str1;
			double positionlong = 0;
			int slc2 = 0, slc1 = 0;
			String str = "", Restr1, Restr2, RestrP;
			double k2long = 0, k1long = 0;

			//整行检测

			HTuple lnum1 = 0, lBnum1 = 0, lnum2 = 0, lBnum2 = 0, singlenun = 0, Scharlinenum = 0;
			HObject SegLine2, SegLine1, CharLine, RoteImage, transaffine1, transaffine2, SCharLine, bigeraffine1, SortSegLine1, SortSegLine2;
			HObject SelectedRegions2, bigeraffine2, OCRRegionClosing, OCRRegion, SelOCRRegionClosing;
			//识别字符生产日期
			HObject RecAdd1, GrayRecgRegion1, ThresRedufuzzyArea1;
			HTuple R3A1 = -1, C3A1 = -1, R3A2 = -1, C3A2 = -1, CharArea = -1, CharCenterR = -1, CharCenterC = -1, RRC = -1, RCC = -1;//膨胀后中心点
			int CharLong = 0;
			HObject objsinglenum;

			//初始化显示列表数值
			m_mCoding.dispStr = "";
			//用于检测单个字符标志位
			int flageERR1 = 0;
			int flageERR2 = 0;

			if (bDebug)
			{



				GenEmptyObj(out m_mCoding.regCheck);
				GenEmptyObj(out m_mCoding.regCorrect);
			}
			//参数配置大部分比较简单，但是应该明确相机组有什么用，蔡明添加
			//下一步还是明确天恒检测软件的界面，蔡明添加

			try
			{
				Restr1 = "";
				Restr2 = "";
				sRsu = SegementMatch(srcImg, bDebug, out SegLine1, out SegLine2, out CharLine, out RoteImage);

				//分割结果出现异常后，直接退出
				if (sRsu.iType > 0)
				{
					return sRsu;
				}
				//-----------第一行分割结果------------
				lnum1 = 0;
				lBnum1 = 0;

				Connection(SegLine1, out SegLine1);
				//write_region(SegLine1,"E:\\resh1.reg");

				SortRegion(SegLine1, out SortSegLine1, "upper_left", "true", "column");
				//字符长度、宽度-------
				//select_shape (SortSegLine1, &transaffine1,HTuple("width").Concat("height"), "or",HTuple(m_pCoding.WidthL/2).Concat(m_pCoding.iResultHeiht), HTuple(999999).Concat(999999));//25,65
				//	select_shape(SortSegLine1, &transaffine1, HTuple("height"), "and", HTuple(m_pCoding.iResultHeiht), HTuple(999999));//25,65
				SelectShape(SortSegLine1, out transaffine1, "height", "and", m_pCoding.iResultHeiht, 999999);

				if (bDebug)
				{

					ConcatObj(m_mCoding.SegmentRect, transaffine1, out m_mCoding.SegmentRect);
				}

				CountObj(transaffine1, out lnum1);
				//字符过长的NG
				//select_shape(SortSegLine1, &bigeraffine1,HTuple("width").Concat("height"), "and",HTuple(m_pCoding.iResultWidth).Concat(120), HTuple(999999).Concat(999999));
				//count_obj(bigeraffine1,&lBnum1);
				//write_region(transaffine1,"F:\\E盘\\项目管理-2015\\灌装相关项目\\厨邦酱油瓶盖\\demo\\transaffine1.reg"); 

				if ((lnum1 < m_pCoding.iResultNum1) || (lBnum1 > 0) || (lnum1 > m_pCoding.iResultNum1))  //NG produce
				{
					//NG produce
					//字体第一行过长
					sRsu.iType = ERROR_MOUTHBROKEN;
					//未找到字符区域，请检测字符宽高设置是否合适!
					sRsu.strDescription = QObject::tr("Char region isn't found,Please examine whether the value of charWidth and charHeight is appropriate or not!");

					CopyObj(transaffine1, out sRsu.regError, 1, -1);

				}

				//-------------------------------------------------------------------------------------------------------------------------
				//第二行分割结果
				//
				if (m_pCoding.iCharline == 2)
				{

					Connection(SegLine2, out SegLine2);
					SortRegion(SegLine2, out SortSegLine2, "upper_left", "true", "column");
					lnum2 = 0;
					lBnum2 = 0;

					TempSLC2 = 0;
					//select_shape (SortSegLine2, &transaffine2,HTuple("width").Concat("height"), "or",HTuple(m_pCoding.WidthL/2).Concat(m_pCoding.iResultHeiht), HTuple(999999).Concat(999999));//25,65

					SelectShape(SortSegLine2, out transaffine2, "height", "and", m_pCoding.iResultHeiht, 999999);
					if (bDebug)
					{

						ConcatObj(m_mCoding.SegmentRect, transaffine2, out m_mCoding.SegmentRect);
					}

					CountObj(transaffine2, out lnum2);
					//	字符过大报错						  								  
					//select_shape(SelectedRegions2, &bigeraffine2,HTuple("width").Concat("height"), "and",HTuple(m_pCoding.iResultWidth).Concat(120), HTuple(999999).Concat(999999));								
					//count_obj(bigeraffine2,&lBnum2);

					//write_region(transaffine2,"F:\\E盘\\项目管理-2015\\灌装相关项目\\厨邦酱油瓶盖\\demo\\transaffine2.reg"); 
					if ((lnum2 < m_pCoding.iResultNum2) || (lBnum2 > 0) || (lnum2 > m_pCoding.iResultNum2))  //NG produce
					{
						//NG produce

						sRsu.iType = ERROR_MOUTHOUTTERRINGEX;
						//未找到字符区域，请检测字符宽高设置是否合适!
						sRsu.strDescription = QObject::tr("Char region isn't found,Please examine whether the value of charWidth and charHeight is appropriate or not!");
						CopyObj(transaffine2, out sRsu.regError, 1, -1);
					}
				}


				if (!sRsu.iType)
				//int m=0;
				//if(m)
				{ //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@++++++++++++++++

					flageERR1 = 0;
					if (m_pCoding.bDataOCR && m_pCoding.iClipThresh == 1)
					{
						for (int k1 = m_pCoding.iClipOneDistance; k1 <= m_pCoding.iClipTwoDistance; k1++)
						{
							//分割好后，抠出来每一个小字，进行边界、模糊、断裂检测


							GenEmptyRegion(out OCRRegionClosing);
							GenEmptyRegion(out OCRRegion);
							SelectObj(transaffine1, out select1, k1);


							GenEmptyRegion(out RecAdd1);
							SmallestRectangle1(select1, out R3A1, out C3A1, out R3A2, out C3A2);
							R3A1 = R3A1 - 1;
							R3A2 = R3A2 + 1;
							//判断字符宽度，是1还是7
							//扩大字符，方便识别位置
							C3A2 = C3A2;
							C3A1 = C3A1;


							GenRectangle1(out RecAdd1, R3A1, C3A1, R3A2, C3A2);
							//write_region(RecAdd1,"F:\\E盘\\项目管理-2015\\灌装相关项目\\厨邦酱油瓶盖\\demo\\RecAdd1.reg");
							//intersection(RecAdd1,m_mCoding.CharSegment,&SelOCRRegionClosing);

							ReduceDomain(RoteImage, RecAdd1, out RedufuzzyArea1);
							MeanImage(RedufuzzyArea1, out ThresRedufuzzyArea1, 10, 10);
							DynThreshold(RedufuzzyArea1, ThresRedufuzzyArea1, out SelOCRRegionClosing, (m_pCoding.iRecThreGray + m_pCoding.iCharEdge) / 2, "dark");


							if (m_pCoding.iCodeTyple == 0)

								DoOcrSingleClassMlp(SelOCRRegionClosing, RoteImage, m_oCoding.lCharModelIDNumberDotPrint, 1, out hv_Class, out hv_Scrot1);
							if (m_pCoding.iCodeTyple == 1)
								DoOcrSingleClassMlp(SelOCRRegionClosing, RoteImage, m_oCoding.lCharModelIDNumberIndustrial, 1, out hv_Class, out hv_Scrot1);
							//write_region(SelOCRRegionClosing,"E://1.reg");
							double soce = hv_Scrot1[0].D;
							str = hv_Class[0].S;
							Restr1 += str;
						}
					}
					if (m_pCoding.bDataP && m_pCoding.iPDataNum == 1)
					{
						for (int k1 = m_pCoding.iDataPone; k1 <= m_pCoding.iDataPtwo; k1++)
						{
							//分割好后，抠出来每一个小字，进行边界、模糊、断裂检测



							GenEmptyRegion(out OCRRegionClosing);
							GenEmptyRegion(out OCRRegion);
							SelectObj(transaffine1, out select1, k1);


							GenEmptyRegion(out RecAdd1);
							SmallestRectangle1(select1, out R3A1, out C3A1, out R3A2, out C3A2);
							R3A1 = R3A1 - 1;
							R3A2 = R3A2 + 1;
							//判断字符宽度，是1还是7
							//扩大字符，方便识别位置
							C3A2 = C3A2;
							C3A1 = C3A1;


							GenRectangle1(out RecAdd1, R3A1, C3A1, R3A2, C3A2);
							//write_region(RecAdd1,"F:\\E盘\\项目管理-2015\\灌装相关项目\\厨邦酱油瓶盖\\demo\\RecAdd1.reg");
							//intersection(RecAdd1,m_mCoding.CharSegment,&SelOCRRegionClosing);
						


							ReduceDomain(RoteImage, RecAdd1, out RedufuzzyArea1);
							MeanImage(RedufuzzyArea1, out ThresRedufuzzyArea1, 10, 10);
							DynThreshold(RedufuzzyArea1, ThresRedufuzzyArea1, out SelOCRRegionClosing, (m_pCoding.iRecThreGray + m_pCoding.iCharEdge) / 2, "dark");
							DoOcrSingleClassMlp(SelOCRRegionClosing, RoteImage, m_oCoding.lCharModelIDChar, 1, out hv_Class, out hv_Scrot1);
							//write_region(SelOCRRegionClosing,"E://1.reg");
							double soce = hv_Scrot1[0].D();
							str = hv_Class[0].S();
							if (m_pCoding.iClipThresh == 1)
							{

								RestrP += str;
							}

						}
					}
					//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@第二行开始@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
					if (!flageERR1 && m_pCoding.iCharline == 2)
					{
						//上面正确往下//小字z符第二行
						flageERR2 = 0;

						if (m_pCoding.bDataOCR && m_pCoding.iClipThresh == 2)
						{
							for (int k2 = m_pCoding.iClipOneDistance; k2 <= m_pCoding.iClipTwoDistance; k2++)
							{
								

								GenEmptyRegion(out OCRRegionClosing);
								GenEmptyRegion(out OCRRegion);
								SelectObj(transaffine2, out select2, k2);
								SmallestRectangle1(select2, out CharR21, out CharC21, out CharR22, out CharC22);

								CharR21 = CharR21 - 1;
								CharR22 = CharR22 + 1;

								GenRectangle1(out select2, CharR21, CharC21, CharR22, CharC22);
								//intersection(select2,m_mCoding.CharSegment,&OCRRegionClosing);
								
								ReduceDomain(RoteImage, select2, out RedufuzzyArea2);
								MeanImage(RedufuzzyArea2, out ThresfuzzyArea2, 10, 10);
								DynThreshold(RedufuzzyArea2, ThresfuzzyArea2, out OCRRegionClosing, (m_pCoding.iRecThreGray + m_pCoding.iCharEdge) / 2, "dark");
								if (m_pCoding.iCodeTyple == 0)

									DoOcrSingleClassMlp(OCRRegionClosing, RoteImage, m_oCoding.lCharModelIDNumberDotPrint, 1, out hv_Class, out hv_Scrot1);
								if (m_pCoding.iCodeTyple == 1)

									DoOcrSingleClassMlp(OCRRegionClosing, RoteImage, m_oCoding.lCharModelIDNumberIndustrial, 1, out hv_Class, out hv_Scrot1);

								//}
								str = hv_Class[0].S;
								str1 = *(str.data());

								Restr2 += str;
							}
						}
						if (m_pCoding.bDataP && m_pCoding.iPDataNum == 2)
						{

							for (int k1 = m_pCoding.iDataPone; k1 <= m_pCoding.iDataPtwo; k1++)
							{
								//分割好后，抠出来每一个小字，进行边界、模糊、断裂检测


								GenEmptyRegion(out OCRRegionClosing);
								GenEmptyRegion(out OCRRegion);
								SelectObj(transaffine2, out select2, k1);




								GenEmptyRegion(out RecAdd1);
								SmallestRectangle1(select2, out R3A1, out C3A1, out R3A2, out C3A2);
								R3A1 = R3A1 - 1;
								R3A2 = R3A2 + 1;
								//判断字符宽度，是1还是7
								//扩大字符，方便识别位置
								C3A2 = C3A2;
								C3A1 = C3A1;


								GenRectangle1(out RecAdd1, R3A1, C3A1, R3A2, C3A2);
								//write_region(RecAdd1,"F:\\E盘\\项目管理-2015\\灌装相关项目\\厨邦酱油瓶盖\\demo\\RecAdd1.reg");
								//intersection(RecAdd1,m_mCoding.CharSegment,&SelOCRRegionClosing);

								ReduceDomain(RoteImage, RecAdd1, out RedufuzzyArea1);
								MeanImage(RedufuzzyArea1, out ThresRedufuzzyArea1, 10, 10);
								DynThreshold(RedufuzzyArea1, ThresRedufuzzyArea1, out SelOCRRegionClosing, (m_pCoding.iRecThreGray + m_pCoding.iCharEdge) / 2, "dark");

								DoOcrSingleClassMlp(SelOCRRegionClosing, RoteImage, m_oCoding.lCharModelIDChar, 1, out hv_Class, out hv_Scrot1);


								//write_region(SelOCRRegionClosing,"E://1.reg");
								double soce = hv_Scrot1[0].D;
								str = hv_Class[0].S;
								if (m_pCoding.iClipThresh == 1)
								{

									RestrP += str;
								}

							}
						}
						//第二行开始检测结束	 

					}//if flageERR2

					//判断日期是否正确
					if ((!sRsu.iType) && (m_pCoding.bDataOCR))
					{
						String strdiff = "";

						int t = m_pCoding.iClipTwoDistance - m_pCoding.iClipOneDistance + 1;
						//手动输入日期
						if (m_pCoding.bWriteDate)
						{
							strdiff = String::number(m_pCoding.iDate);
							strdiff = strdiff.left(t);
						}

						//获取系统时间判断是否正确
						else
						{
							str = QDateTime::currentDateTime().toString("yyyyMMdd");
							strdiff = str.left(t);
							// strdiff2 = QDateTime::currentDateTime().toString("yyyyMMdd")+"8";
							// strdiff1 = QDateTime::currentDateTime().toString("yyyyMMdd")+"9";
							// if((Restr1!=str)||(Restr1!=strdiff1)||(Restr1!=strdiff2))
						}

						if (m_pCoding.iClipThresh == 1)
							m_mCoding.dispStr = Restr1;

						if (m_pCoding.iClipThresh == 2)
							m_mCoding.dispStr = Restr2;

						sRsu.strDescription = (m_mCoding.dispStr);
						if (Restr1 != strdiff && m_pCoding.iClipThresh == 1)
						{
							sRsu.iType = ERROR_BOTTLECOLOR;

							CopyObj(transaffine1, out sRsu.regError, 1, -1);
							return sRsu;
						}
						if (Restr2 != strdiff && m_pCoding.iClipThresh == 2)
						{
							sRsu.iType = ERROR_BOTTLECOLOR;

							CopyObj(transaffine1, out sRsu.regError, 1, -1);
							return sRsu;
						}
					}

					if ((!sRsu.iType) && m_pCoding.bDataP)
					{
						QString strdiffP = "";
						strdiffP = m_pCoding.sDataP;
						m_mCoding.dispStr = m_mCoding.dispStr + RestrP;
						sRsu.strDescription = QString(m_mCoding.dispStr);
						if (RestrP != strdiffP)
						{
							sRsu.iType = ERROR_MOUTHINNERRING;
							if (m_pCoding.iPDataNum == 1)

								CopyObj(transaffine1, out sRsu.regError, 1, -1);
							if (m_pCoding.iPDataNum == 2)

								CopyObj(transaffine2, out sRsu.regError, 1, -1);
							return sRsu;
						}
					}
				}//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@++++++++++++

				return sRsu;
            }
			catch (HException &e)
			{
				//Halcon异常		
				QString tempStr, strTime, imgFilePath;
				QTime tCurTime;
				tempStr = e.message;
				tempStr.remove(0, 20);
				sRsu.iType = ERROR_EXCEPTION;
				sRsu.strDescription = QObject::tr("CCheckCoding::check,") + tempStr;
				//2013.12.13 保存异常图像和模板	
				//WriteLog(sRsu.strDescription,AbnormityLog);

				extern QString g_strAppPath;
				imgFilePath = g_strAppPath + "AbnormalImages\\";
				QDir saveDir;
				if (!saveDir.exists(imgFilePath))
				{
					saveDir.mkpath(imgFilePath);
				}
				imgFilePath += QString("%1.bmp").arg(QTime::currentTime().toString("hhmmss.zzz"));//存图
																								  //write_image(srcImg, "bmp", 0, imgFilePath);	
				return sRsu;

			}



			}//这个反花括号和谁匹配呢

		


	}
		public s_CheckResult SegementMatch(HObject srcImg, bool bDebug, out HObject SegLine1, out HObject SegLine2, out HObject CharLine, out HObject RoteImage)
		{
			HTuple IsHaveRow = -1, IsHaveCol = -1, IsHaveRadus = -1;
			s_CheckResult sRsu = new s_CheckResult();
			GenEmptyRegion(out m_oCoding.RoteImage);
			GenEmptyRegion(out SegLine1);
			GenEmptyRegion(out SegLine2);
			GenEmptyObj(out m_mCoding.CharSegment);


			HTuple hv_Row1, hv_Column1, hv_Row2, hv_Column2;


			if (bDebug)
			{


				GenEmptyObj(out m_mCoding.CharRect);
				GenEmptyObj(out m_mCoding.dynRect);
				GenEmptyObj(out m_mCoding.SegmentRect);
				GenEmptyObj(out m_mCoding.ResultRect);

				GenEmptyObj(out m_mCoding.OpenWidthRect);
				GenEmptyObj(out m_mCoding.LineRect);

			}
			HObject srcImgRect;

			try
			{
				if (m_pCoding.bModelMatch)
				{
					HObject reduceImg, meanImg, affImg, selctregPrompt, rotateImgGray;
					HObject regPrompt, regThresh, regTemp, regChar, S2Regions;

					 

					HTuple lNum;
					HTuple tpCharRsu1;
					HTuple tpCharRsu2;
					//模板匹配
					if (m_oCoding.lLocModelID < 0)
					{
						sRsu.iType = ERROR_CAPSTYPE;
						//无效的匹配模板
						sRsu.strDescription = QObject::tr("Invalid match model!");
						
						GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
						return sRsu;
					}
				

					//提取瓶盖区域
					Threshold(srcImg, out regThresh, m_pCoding.iGray, 255);
					Connection(regThresh, out regPrompt);
					SelectShape(regPrompt, out selctregPrompt, "area", "and", 10000, 99999999);
					FillUp(selctregPrompt, out regTemp);
					OpeningCircle(regTemp, out regTemp, 33.5);
					Connection(regTemp, out regTemp);
					SelectShape(regTemp, out regTemp, "circularity", "and", 0.5, 1);
					SelectShapeStd(regTemp, out regThresh, "max_area", 70);
					CountObj(regThresh, out lNum);
					if (lNum == 0)
					{
						sRsu.iType = ERROR_NOCAPS;
						//未找到瓶盖，请检测灰度值设置是否合适!
						sRsu.strDescription = QObject::tr("The bottle caps isn't found,Please examine whether gray is appropriate or not!");
						
						CountObj(regPrompt, out lNum);

						if (lNum > 0)
						{
							
							CopyObj(regPrompt, out sRsu.regError, 1, -1);
						}
						else
						{
							
							GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
						}
						
						return sRsu;
					}


					//模板定位
					HTuple tpRows = -1, tpCols = -1, tpPhis = -1, tpScores = -1, tempeAreaValue = -1;
					

					ReduceDomain(srcImg, regThresh, out reduceImg);
					SmallestCircle(regThresh, out IsHaveRow, out IsHaveCol, out IsHaveRadus);
					findModel(reduceImg, m_oCoding.lLocModelID, ref m_pCoding.spFindModel, out tpRows, out tpCols, out tpPhis, out tpScores);
					if (0 == lNum)
					{
						sRsu.iType = ERROR_CAPSTYPE;
						//未找到正确的定位目标!
						sRsu.strDescription = QObject::tr("Type of the caps is error,match falid!Score is 0");
						
						CopyObj(m_oCoding.regModelRectangle, out sRsu.regError, 1, -1);
						return sRsu;
					}
					else
					{
						if (tpScores[0].D < m_pCoding.fScore)
						{
							//-----------------2015---???????????????????????????????????
							sRsu.iType = ERROR_CAPSTYPE;
							//匹配度设置不当!
							sRsu.strDescription = QObject::tr("Type of the caps is error,Please examine whether score is appropriate or not!Score is %1").arg(tpScores[0].D());
							HTuple tpRadius;
							

							TupleGenConst(lNum, 10, out tpRadius);
							GenCircle(out sRsu.regError, tpRows, tpCols, tpRadius);
							return sRsu;
						}
						if (tpScores[0].D >= m_pCoding.fScore)
						{//--
							HObject R1, RegionFillUp, ConnecR1, selectR1, tempR1, Cir, I1, ImageMean, RegThresh;

							///----------------------2015--- 
							//仿射变换矩阵
							

							VectorAngleToRigid(tpRows[0].D, tpCols[0].D, tpPhis[0].D,
								m_oCoding.modelOri.Row, m_oCoding.modelOri.Col, m_oCoding.modelOri.Angle, out m_tCoding.homMat2D);

							//trans_ROI
						
							AffineTransImage(reduceImg,out RoteImage, m_tCoding.homMat2D, "constant", "false");
							AffineTransImage(reduceImg, out m_oCoding.RoteImage, m_tCoding.homMat2D, "constant", "false");
							HObject charReg;
							//copy_image(srcImg,&m_oCoding.RoteImage);
							
							SmallestRectangle1(m_oCoding.IsHaveCapRect, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
							//gen_rectangle1(&charReg,hv_Row1,tpCols[0].D()-m_pCoding.iGrayMarkHeiht,hv_Row2,hv_Column2);
						

							GenRectangle1(out charReg, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
							ReduceDomain(RoteImage, charReg,out RoteImage);
						}

					}
                }
                else
                {
					HTuple StartAngle = -45, EndAngle = 45, kangle = -1, kan = -1, OrientationAngle = -1;
					text_line_orientation(srcImg, srcImg, 90, StartAngle.Rad(), EndAngle.Rad(), out OrientationAngle);


					TextLineOrientation(srcImg, srcImg, 90, StartAngle.Rad(), EndAngle.Rad(), out OrientationAngle);
					kangle = OrientationAngle.Deg();


					if (kangle > 40)
					{
						kan = 90 - kangle + 180;
					}

					else
					{
						kan = -kangle;
					}

					

					RotateImage(srcImg, out m_oCoding.RoteImage, kan, "constant");
					RotateImage(srcImg,out  RoteImage, kan, "constant");
					ReduceDomain(RoteImage, m_oCoding.IsHaveCapRect,out RoteImage);
				}
				HObject Region, ConnectedRegions, ObjectSelected;
				HTuple RegionArea, RegionMax, RegionIndices;
				HTuple hv_Row11, hv_Column11, hv_Row21, hv_Column21;
		


				Threshold(RoteImage, out Region, m_pCoding.iRoiCharGary, 255);
				Connection(Region, out ConnectedRegions);
				AreaCenter(ConnectedRegions, out RegionArea,out  hv_Row11, out hv_Column11);
				TupleMax(RegionArea, out RegionMax);
				TupleFind(RegionArea, RegionMax, out RegionIndices);
				SelectObj(ConnectedRegions, out ObjectSelected, RegionIndices + 1);
				FillUp(ObjectSelected, out ObjectSelected);
				InnerRectangle1(ObjectSelected, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
				double regWidth = hv_Column2[0].D - hv_Column1[0].D;
				HObject Rectangle, ImageReduced, RingRectThresh;
				HObject RegThresh, RegThreshFillUp, RegThreshDifference, RegImageMean;
				HObject RegionDynThresh, RegionIntersection;
				GenRectangle1(out Rectangle, hv_Row1 + 10, hv_Column1, hv_Row2 - 10, hv_Column2 - 10);

				if (bDebug)
				{
					
					ConcatObj(m_mCoding.CharRect, Rectangle, out m_mCoding.CharRect);
				}
				
				ReduceDomain(RoteImage, Rectangle, out ImageReduced);

				if (0)
				{
					
					HTuple RingNum;

					HTuple row11, row12,col1,col2;
					/*	fill_up(ObjectSelected,&RingRectThresh);
						opening_rectangle1(RingRectThresh,&RingRectThresh,regWidth/3,2);
						connection(RingRectThresh,&RingRectThresh);*/



					OpeningRectangle1(ObjectSelected, out RingRectThresh, 5, 2);
					FillUp(RingRectThresh, out RingRectThresh);
					Connection(RingRectThresh, out RingRectThresh);
					AreaCenter(RingRectThresh, out RegionArea, out row11, out col1);
					TupleMax(RegionArea, out RegionMax);
					TupleFind(RegionArea, RegionMax, out RegionIndices);
					SelectObj(RingRectThresh, out ObjectSelected, RegionIndices + 1);
					SmallestRectangle1(ObjectSelected,out row11,out col1,out row12,out col2);
					if (row12 - row11 < (hv_Row2 - hv_Row1) * 5 / 6)
					{
						sRsu.iType = ERROR_MOUTHOUTTERRING;
						
						CopyObj(RingRectThresh, out sRsu.regError, 1, -1);
						return sRsu;
					}
				}

				

				Threshold(ImageReduced, out RegThresh, m_pCoding.iCharGray, 255);
				FillUp(RegThresh, out RegThreshFillUp);
				Difference(RegThreshFillUp, RegThresh, out RegThreshDifference);
				MeanImage(ImageReduced, out RegImageMean, 10, 10);
				DynThreshold(RoteImage, RegImageMean, out RegionDynThresh, m_pCoding.iCharEdge, "dark");
				Union2(RegionDynThresh, RegThreshDifference, out RegionIntersection);
				if (bDebug)
				{
					

					ConcatObj(m_mCoding.dynRect, RegionDynThresh, out m_mCoding.dynRect);
					ConcatObj(m_mCoding.ResultRect, RegThreshDifference, out m_mCoding.ResultRect);

				}
				HObject regLine1, regLine2, regLine3, SelectedRegions1, SelectedRegions2;
				

				GenEmptyRegion(out regLine1);
				GenEmptyRegion(out regLine2);
				GenEmptyRegion(out SelectedRegions1);
				GenEmptyRegion(out SelectedRegions2);

				HTuple IsHaveRow = -1, IsHaveCol = -1, IsHaveRadus = -1;
				//分割后整合矩形框
				HObject I1, threshI1, UtempSeg1, UtempSeg2, Seg2obj, Seg1obj, tempSeg1, tempSeg2;
				HTuple tempSeg2num, tempSeg1num, standPaller1 = 0, standPaller2 = 0;

				HTuple centerR = -1, centerC = -1;
				HTuple SLC2 = -1, SLC1 = -1;
				int slc2 = 0, slc1 = 0;
				QString str;
				//--------------------------------------------------
				HObject ImageReducedOK, IntersectClsoing, RegionTrans1, R1OK, R2OK, P1, P2, RegionTrans2;
				HObject CRIntersectClsoing, SUp, Sdown, Rect1, Rect2, SCR1OK, CR1OK;
				HTuple hv_LineArea, RLeft = new  HTuple(), CLeft = new HTuple(), RRight = new HTuple(), CRight = new HTuple(), CenterR = new HTuple(), ChnStart = HTuple();
				HTuple Tstart = -1, Tend = -1;
				HTuple Number = 0;
				reduce_domain(*RoteImage, RegionIntersection, &ImageReducedOK);




				
				closing_rectangle1(RegionIntersection, &IntersectClsoing, regWidth / 8, m_pCoding.iResultWidth);
				opening_rectangle1(IntersectClsoing, &IntersectClsoing, 4, 1);
				//write_region(IntersectClsoing,"E://.reg");
				connection(IntersectClsoing, &CRIntersectClsoing);
				area_center(CRIntersectClsoing, &hv_LineArea, NULL, NULL);
				tuple_max(hv_LineArea, &RegionMax);
				tuple_find(hv_LineArea, RegionMax, &RegionIndices);
				select_obj(CRIntersectClsoing, &ObjectSelected, RegionIndices + 1);
				smallest_rectangle1(ObjectSelected, &hv_Row1, &hv_Column1, &hv_Row2, &hv_Column2);
				m_pCoding.HeightL = hv_Row2[0].D() - hv_Row1[0].D();
				select_shape(CRIntersectClsoing, &CRIntersectClsoing, "height", "and", m_pCoding.iResultHeiht, 99999);
				//select_shape(CRIntersectClsoing,&CRIntersectClsoing,"height", "and", m_pCoding.iParititionCharHeiht-5, 99999);
				select_shape(CRIntersectClsoing, &CRIntersectClsoing, "width", "and", m_pCoding.HeightL / 2, 99999);

				//*******保证上下都能分割,进行单排横向联接*******************
				ClosingRectangle1(RegionIntersection, &IntersectClsoing, regWidth / 8, m_pCoding.iResultWidth);
				OpeningRectangle1(IntersectClsoing, out IntersectClsoing, 4, 1);
				//write_region(IntersectClsoing,"E://.reg");
				Connection(IntersectClsoing, out CRIntersectClsoing);
				AreaCenter(CRIntersectClsoing, &hv_LineArea, NULL, NULL);
				TupleMax(hv_LineArea, out RegionMax);
				TupleFind(hv_LineArea, RegionMax, out RegionIndices);
				SelectObj(CRIntersectClsoing, out ObjectSelected, RegionIndices + 1);
				SmallestRectangle1(ObjectSelected, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
				m_pCoding.HeightL = hv_Row2[0].D - hv_Row1[0].D;
				SelectShape(CRIntersectClsoing, out CRIntersectClsoing, "height", "and", m_pCoding.iResultHeiht, 99999);
				//select_shape(CRIntersectClsoing,&CRIntersectClsoing,"height", "and", m_pCoding.iParititionCharHeiht-5, 99999);
				SelectShape(CRIntersectClsoing, out CRIntersectClsoing, "width", "and", m_pCoding.HeightL / 2, 99999);

				if (bDebug)
				{
					
					ConcatObj(m_mCoding.LineRect, CRIntersectClsoing, out m_mCoding.LineRect);

				}
				area_center(CRIntersectClsoing, NULL, &CenterR, NULL);
				tuple_sort(CenterR, &CenterR);

				AreaCenter();
				TupleSort(CenterR, out CenterR);
				//write_region(CRIntersectClsoing,"E://reg.reg");
				switch (m_pCoding.iCharline)
				{
					case 1:

						Tstart = CenterR[0];
						Tend = CenterR[0].D() + 10;
						select_shape(CRIntersectClsoing, &SUp, "row", "and", Tstart, Tend);

						count_obj(SUp, &Number);
						//-------------//
						union1(SUp, &SUp);

						shape_trans(SUp, &Rect1, "convex");

						SelectShape(CRIntersectClsoing, out SUp, "row", "and", Tstart, Tend);
						CountObj(SUp, out Number);
						ShapeTrans(SUp, out Rect1, "convex");

						break;
					case 2:
						//***第一行******
						
						CountObj(CRIntersectClsoing, out Number);
						if (Number == 1)
						{
							smallest_rectangle1(CRIntersectClsoing, &hv_Row1, &hv_Column1, &hv_Row2, &hv_Column2);
							m_pCoding.HeightL = m_pCoding.HeightL / 2;
							gen_rectangle1(&Rect1, hv_Row1, hv_Column1, hv_Row1 + m_pCoding.HeightL, hv_Column2);
							gen_rectangle1(&Rect2, hv_Row2 - m_pCoding.HeightL, hv_Column1, hv_Row2, hv_Column2);
							break;
						}
						Tstart = CenterR[0];
						Tend = CenterR[0].D() + m_pCoding.iResultHeiht;
						select_shape(CRIntersectClsoing, &SUp, "row", "and", Tstart, Tend);

						//-------------//
						union1(SUp, &SUp);
						//*****第二行********
						Tstart = CenterR[1].D() - 5;
						Tend = CenterR[1].D() + m_pCoding.iResultHeiht;
						select_shape(CRIntersectClsoing, &Sdown, "row", "and", Tstart, Tend);
						union1(Sdown, &Sdown);

						shape_trans(SUp, &Rect1, "convex");
						shape_trans(Sdown, &Rect2, "convex");
						break;

						//default:
						//// sort_region (regChar, regLine1, "upper_left", "true", "column");
						// break;
				}
			}
			}
		}

	}
}
	
