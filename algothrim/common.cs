using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HalconDotNet;
//using HalconDotNet.HOperatorSet;
using static HalconDotNet.HOperatorSet;
using bottleWithHALCON.innerData;
using static System.Math;
namespace bottleWithHALCON.algothrim
{
	//c#中 strcut成员确实要加public
	public struct s_My_Ori
	{
		float Row;
		float Col;
		float Angle;
		public s_My_Ori(float row = 0,float col = 0, float angle = 0)
        {
			Row = row;
			Col = col;
			Angle = angle;
		}


	}

	public struct s_pCreatModel
	{

		public int iNumLevel;      //****金字塔级数（该值越大匹配耗时越短）
								   //模板角度，一般根据对称性来进行设置，若模板图像不对称需设幅度为360度，若为正方形，可设幅度为90度，长方形为180度
		public float fPhiStart;    //起始角度（弧度，范围-3.14到3.14）
		public float fPhiExtent;   //角度幅度（弧度，范围0到6.28）
		public float fPhiStep;     //角度步长
								   //**优化算法参数,第一个参数，依次更优化，点数多时可使用.'none'1, 'point_reduction_low'1/2, 'point_reduction_medium'1/3, 'point_reduction_high'1/4）
								   //*优化算法参数,第二个参数，'pregeneration', 'no_pregeneration' ，预分配内存对速度影响不大，反而耗费内存，不推荐使用
		public String strOptim;   //**优化算法（降低边缘点数可提速,依次更优化.)
								  //极性，'use_polarity'，使用该参数即可，一般模板与目标极性相同
		public int iEdge;          //对比度，用于提取模板，有三种参数模式[min,max,size],目前只用一个参数
		public int iMinEdge;       //最小对比度，排除无效边缘干扰
	}

	public struct s_pFindModel
	{
		public double fPhiStart;    //**起始角度（弧度，范围-3.14到3.14）
		public double fPhiExtent;   //**角度幅度（弧度，范围0到6.28）
		public double fScore;       //***匹配度，该值越小匹配越耗时，范围[0，1]，推荐值（0.3-0.7）
		public int nNumMatchs; //匹配个数，0为所有匹配，指定个数，则选择匹配度最高的
		public double fMaxOverlap;  //允许的最大重叠面积，范围[0，1]，无格挡可设为0
								   //**亚像素精度影响检测速度和匹配结果（*角度*），若精度要求不高，可使用'interpolation'，综合精度和速度可使用'least_squares'
		public String strSubPixel;//**亚像素精度。
		public double fGreediness;  //****用于定位加速，该值越大速度越快，但可能导致无法找到匹配目标，推荐（0.7-0.9）

	public s_pFindModel(double fphiStart = -3.14, double fphiExtent = 6.29, double fscore = 0.3, int nnumMatchs = 0, double fmaxOverlap = 0, String strsubPixel = "least_squares", double fgreediness = 0.7)
        {
			fPhiStart = fphiStart;
			fPhiExtent = fphiExtent;
			fScore = fscore;
			nNumMatchs = nnumMatchs;
			fMaxOverlap = fmaxOverlap;
			strSubPixel = strsubPixel;
			fGreediness = fgreediness;

		}
	}


	public struct s_pPMToolPara
	{
		public s_pCreatModel spCreatModel;
		public s_pFindModel spFindModel;
	}

	public struct s_pOCRPublicPara
	{
		public HTuple hNumberModelID;       //数字字模库
		public HTuple hLowerCaseModelID;    //小写字母字模库
		public HTuple hUpperCaseModelID;    //大写字母字模库

		public int iLineCount;     //行数
		public int iRelation1;     //与|或
		public int iSegEdge1;      //对比度1
		public int iSegGray1;      //灰度1
		public int iRelation2;     //与|或
		public int iSegGray2;      //对比度2
		public int iSegEdge2;      //灰度2
		public double dVarStdDevScale;//Var阈值参数
		public int iVarAbsGray;

		public int iCharSpace;     //字符间隔
		public int iCharWidth;     //字符宽度
		public int iCharHeight;    //字符高度
		public float fScore;       //匹配度

		public int iPosMethod;     //查找字符位置方法0、记忆1、自动
		public	s_pOCRPublicPara(HTuple NumberModelID, HTuple LowerCaseModelID, HTuple UpperCaseModelID, int LineCount = 1, int Relation1 = 0, int SegEdge1 = 10, int SegGray1 = 30,int Relation2 = 0, int SegGray2 =60 , int SegEdge2 = 20, double VarStdDevScale = 0.8, int VarAbsGray = 10, int CharSpace = 0, int CharWidth = 10, int CharHeight = 20,float Score = (float)0.7, int PosMethod = 0)
        {
			hNumberModelID = NumberModelID;
			hLowerCaseModelID = LowerCaseModelID;
			hUpperCaseModelID = UpperCaseModelID;
			iLineCount = LineCount;
			iRelation1 = Relation1;
			iSegEdge1 = SegEdge1;
			iSegGray1 = SegGray1;
			iRelation2 = Relation2;
			iSegGray2 = SegGray2;
			iSegEdge2 = SegEdge2;
			dVarStdDevScale = VarStdDevScale;
			iVarAbsGray = VarAbsGray;
			iCharSpace = CharSpace;
			iCharWidth = CharWidth;
			iCharHeight = CharHeight;
			fScore = Score;
			iPosMethod = PosMethod; 
		}
	}
	//float和double区别

	public struct s_pOCRLinePara
	{

		public int iCheckMethod;       //检测方法：0、粗略，仅看单个字符有无 1、字符识别
								//* 解析字符串获取参数，不进行保存
								//字符规则
								//int iCharRule;				//0：字符有一定的生成规则，数字字母等有其固定位置；1：字符无生成规则，字母数字等无序。
								//QString strCharRoot;		//源字符:数字 + [小写字母] + {大写字母} + #动态信息# --修改字符规则后，重启系统有效
		public String strCharRoot;        //源字符:检测字符 + <不检测字符> + #动态信息# --修改字符规则后，重启系统有效
		public String strCharRun;         //*运行字符-含预编译字符（#），运行时实时替换
		public String strCharGenerate;    //*生成字符-以当时字符完全替换,检测是实际使用,对比检测对错
		public String strCharRule;        //*字符规则-识别时按此规则用相应的字模库；0-数字；a-小写字母；A-大写字母；*-不进行识别
		public s_pOCRLinePara(int checkMethod = 0,String CharRoot="", String CharRun = "", String CharGenerate = "", String CharRule="")
		{
			iCheckMethod = checkMethod;
			strCharRule = CharRule;
			strCharRoot = CharRoot;
			strCharRun = CharRun;
			strCharGenerate = CharGenerate;
		}
	}
	public struct s_pOCRErrorInfo
	{
		public String strErrorChar;   //错误字符，1个，检测到错误即退出
		public HObject regErrorChar;   //错误区域，1个，检测到错误即退出
		public int iType;              //错误字符类型
	}











	public struct s_pOCRPara
	{
	public	s_pOCRPublicPara spOCRPublicPara;
	public s_pOCRErrorInfo spOCRErrorInfo;
	public s_pOCRLinePara spOCRPara1;
	public s_pOCRLinePara spOCRPara2;
	public s_pOCRLinePara spOCRPara3;

	
	}


	public struct s_pCapsPara
	{
		//找瓶盖和安全环
		public int iPolar;             //极性 0：黑色盖子，白色安全环缺陷，1：白色盖子,黑色安全环缺陷
		public int iCapsGray;          //灰度值
		public float fClosingScale;    //闭合比例系数，合并断开的安全环和瓶盖，纵向闭合，尺度为fClosingScale×高度，标定时，闭合尺度默认为20（小）

		public bool bSafeRing;         //安全环
		public bool bHorClosing;       //水平闭合，不检测安全环中间缺陷
		public int iSafeRingEdge;
		public int iSafeRingGray;
		public int iSafeRingWidth;     //安全环缺口宽度
		public int iSafeRingHeight;    //安全环缺口高度
		public int iSafeRingArea;      //安全环面积

		public bool bCapsPos;          //瓶盖高低、角度、大小
		public int iHighCapsMin;       //高盖
		public int iHighCapsMax;
		public int iPickCapsLineHeight;//提取瓶盖角度线，所切割的高度
		public int iPickCapsArea;      //面积
		public float fCapsPhiMin;      //角度
		public float fCapsPhiMax;
	}

	//预处理区域参数
	public struct s_pROIPara
	{
	public	float fRoiRatio;
	public int nClosingWH;
	public int nGapWH;
	public s_pROIPara(float RoiRatio, int ClosingWH,int GapWH)
        {
			fRoiRatio = (float)0.85;
			nClosingWH = 30;
			nGapWH = 50;
		}

	}

	//线定位参数
	public struct s_LineLocPara
	{
	public	int iRow1;
		public int iCol1;
		public int iRow2;
		public int iCol2;
		public int iDistance;

	}
	//c#默认初始化结构体成员为0
	public struct s_LineLocParaEx
	{
		public s_LineLocPara sLineOuter;
		public s_LineLocPara sLineInner;
	}


	//原点
	public struct s_MyOri
	{
		public	double dOriRow;
		public double dOriCol;
		public double dOriPhi;
	}

	public partial class CommonCheck
	{
		public CommonCheck()
		{

		}
		~CommonCheck()
		{

		}

		private List<int> m_vCansOutWardErrorType;  //1罐壁
		private List<int> m_vCansBotFontErrorType;  //2罐底
		private List<int> m_vBottleCapsErrorType;   //3瓶盖
		private List<int> m_vBottleMorphErrorType;  //4异形瓶
		private List<int> m_vBottleMouthExErrorType;    //5口外沿
		private List<int> m_vFullLevelPosErrorType; //6液位 
		private List<int> m_vFullCapsErrorType;     //7满瓶瓶盖
		private List<int> m_vFaceLabelErrorType;        //8平面标签
		private List<int> m_vCircleLabelErrorType;  //9圆形标签(老干妈)
		private List<int> m_vCodingErrorType;       //10喷码检测


		private short m_iShowMode;          //显示模式（0：比例显示;1：填充显示）
		private bool m_bShowCheckID;        //是否显示检测ID
		private short m_iCheckID;           //用于显示检测ID
		//private Rect m_rtImageShow;        //图像显示区域
	//	private CMyShape* m_pMyShape;       //绘制图像

		private e_Status m_eStatus;         //状态
		private e_DrawStatus m_eDrawStatus; //绘制状态
		private short m_iDrawShape;         //绘制形状

		private float m_fHorScale;          //水平比例
		private float m_fVerScale;          //垂直比例
		private int m_iArrowScale;          //箭头大小

		private int m_iFontHeight;          //字体高度
		private int m_iFontLeftSpace;       //字体左侧间隔
		private int m_iFontRightSpace;      //字体右侧间隔
		private int m_iFontVerSpace;        //字体纵向间隔
		s_ImgWidgetShowInfo m_sImgWidgetShowInfo;   //界面显示信息


		public double PI = 3.1415926;                   //PI是弧度制
														//引用参数就是；读取时和传值参数一样，写入时会改变原变量


		public int g_iUserPermit = 6;
		public void makeModel(HObject srcImg, HObject regModel, s_pCreatModel sModel, out HTuple lLocModelID, int iMethod = 0)   //c++中的const参数和引用搭配使用时表示引用参数内容不可被修改
		{

			HObject reducedImg;
			HObject xldModel;


			ReduceDomain(srcImg, regModel, out reducedImg);
			switch (iMethod)
			{
				case 0:
					CreateShapeModel(reducedImg, sModel.iNumLevel, sModel.fPhiStart, sModel.fPhiExtent, sModel.fPhiStep, sModel.strOptim, "use_polarity", sModel.iEdge, sModel.iMinEdge, out lLocModelID);



					break;
				case 1:

					EdgesSubPix(reducedImg, out xldModel, "canny", 1, sModel.iEdge, 40);
					CreateShapeModelXld(xldModel, sModel.iNumLevel, sModel.fPhiStart, sModel.fPhiExtent, sModel.fPhiStep, sModel.strOptim, "use_polarity", sModel.iMinEdge, out lLocModelID);
					break;
				default:


					CreateShapeModel(reducedImg, sModel.iNumLevel, sModel.fPhiStart, sModel.fPhiExtent, sModel.fPhiStep, sModel.strOptim, "use_polarity", sModel.iEdge, sModel.iMinEdge, out lLocModelID);
					break;
			}
			lLocModelID = new HTuple();
			lLocModelID.Dispose();

		}
		//研究c#指针，c#中结构体的指针，普通类型变量的指针
		//研究c#out,out用于在函数中返回多个值
		//要在c#中明确哪些类型是传值,哪些类型是传引用
		//要学会在c#中如何将函数定义和函数实现分开
		//Dispose的使用方法是先定义，然后使用Dispose最后为变量赋值
		//findModel寻找模板
		public void findModel(HObject Img, HTuple ModelID, ref s_pFindModel sModel, out HTuple Row, out HTuple Column,
		out HTuple Angle, out HTuple Score)
		{

			FindShapeModel(Img, ModelID, sModel.fPhiStart, sModel.fPhiExtent, sModel.fScore, sModel.nNumMatchs,
		sModel.fMaxOverlap, sModel.strSubPixel, 0, sModel.fGreediness, out Row, out Column, out Angle, out Score);

		}
		//*功能：获取一个边缘点，返回边缘点的个数，找到点后判断污点干扰；nThresh边缘提取阈值，nLength边缘长度
		//findEdgePointSingleEx函数完成,2023.6.26
		public int findEdgePointSingleEx(HObject Image, HObject LineSeg,
		out HTuple RowPt, out HTuple ColPt, int nThresh, int nLength, int nEdge = 10, int nDirect = (int)e_LineDirect.L2R, int nType = 0,
		bool bMean = false)
		{
			int iCount = findEdgePointSingle(Image, LineSeg, out RowPt, out ColPt, nEdge, nDirect, nType, bMean, true);
			if (iCount < 2)
			{
				return iCount;
			}
			HObject imgDomain, imgSobel, regThresh;
			HObject regRect;
			GenEmptyObj(out imgDomain);
			GenEmptyObj(out imgSobel);
			GenEmptyObj(out regThresh);
			GenEmptyObj(out regRect);
			int nWidthEx = 50, nHeightEx = 10;
			int nTempRow, nTempCol;
			HTuple numTemp;
			int i;


			for (i = 0; i < iCount; ++i)
			{
				//如何为halcon函数传递类型变量而不是HTuple
				//将普通类型变量转为HTuple?

				switch (nDirect)
				{
					case (int)e_LineDirect.L2R:
						//	gen_rectangle1(&regRect, (*RowPt)[i].I() - nWidthEx, (*ColPt)[i].I() - nHeightEx, (*RowPt)[i].I() + nWidthEx, (*ColPt)[i].I() + nHeightEx);
						GenRectangle1(out regRect, (HTuple)(RowPt[i].I - nWidthEx), (HTuple)(ColPt[i].I - nHeightEx), (HTuple)(RowPt[i].I + nWidthEx), (HTuple)(ColPt[i].I + nHeightEx));
						break;
					case (int)e_LineDirect.R2L:
						GenRectangle1(out regRect, (HTuple)(RowPt[i].I - nWidthEx), (HTuple)(ColPt[i].I - nHeightEx), (HTuple)(RowPt[i].I + nWidthEx), (HTuple)(ColPt[i].I + nHeightEx));
						break;
					case (int)e_LineDirect.T2B:
						//	gen_rectangle1(&regRect, (*RowPt)[i].I() - nHeightEx, (*ColPt)[i].I() - nWidthEx, (*RowPt)[i].I() + nHeightEx, (*ColPt)[i].I() + nWidthEx);

						GenRectangle1(out regRect, (HTuple)(RowPt[i].I - nHeightEx), (HTuple)(ColPt[i].I - nWidthEx), (HTuple)(RowPt[i].I + nHeightEx), (HTuple)(ColPt[i].I + nWidthEx));
						break;
					case (int)e_LineDirect.B2T:
						//gen_rectangle1(&regRect, (*RowPt)[i].I() - nHeightEx, (*ColPt)[i].I() - nWidthEx, (*RowPt)[i].I() + nHeightEx, (*ColPt)[i].I() + nWidthEx);
						GenRectangle1(out regRect, (HTuple)(RowPt[i].I - nHeightEx), (HTuple)(ColPt[i].I - nWidthEx), (HTuple)(RowPt[i].I + nHeightEx), (HTuple)(ColPt[i].I + nWidthEx));

						break;
					default:

						break;
				}


				ReduceDomain(Image, regRect, out imgDomain);
				SobelAmp(imgDomain, out imgSobel, "sum_abs", 3);
				Threshold(imgSobel, out regThresh, nThresh, 255);
				Connection(regThresh, out regThresh);
				ClosingRectangle1(regThresh, out regThresh, 3, 3);
				SelectShape(regThresh, out regThresh, (new HTuple("height")).TupleConcat(
		  "width"), "or", (new HTuple(nLength)).TupleConcat(nLength), (new HTuple(99999)).TupleConcat(
		  99999));
				CountObj(regThresh, out numTemp);
				if (numTemp[0].I > 0)
				{
					nTempRow = RowPt[i].I;
					nTempCol = ColPt[i].I;
					RowPt[0] = (HTuple)nTempRow;
					ColPt[0] = (HTuple)nTempCol;
					return 1;

				}
				//	SelectShape(regThresh,out regThresh, (HTuple)("height").Concat("width"), "or", (HTuple)(nLength).Concat(nLength), (HTuple)(99999).Concat(99999));
			}
			return 0;
		}


		//返回结果在
		//findEdgePointSingle函数完成 2023.6.26
		unsafe public int findEdgePointSingle(HObject Image, HObject LineSeg,
	out HTuple RowPt, out HTuple ColPt, int nEdge = 10, int nDirect = (int)e_LineDirect.L2R, int nType = 0,
		bool bMean = false, bool bAllPoint = false)
		{

			RowPt = new HTuple();
			ColPt = new HTuple();
			RowPt.Dispose();
			ColPt.Dispose();

			HObject ImageReduce, ImageGauss, NewLine;
			HTuple Row, Col, GrayValue;
			int nGrayDiff1, nGrayDiff2, nGrayDiff3, nCount, nLength, diff;         //原代码是long 型，现在改成int型

			HTuple nImgWidth, nImgHeight;
			HTuple dPhi;
			GenEmptyObj(out ImageReduce);
			GenEmptyObj(out ImageGauss);
			GenEmptyObj(out NewLine);
			Row = new HTuple();
			Col = new HTuple();
			GrayValue = new HTuple();
			dPhi = new HTuple();
			Row.Dispose();
			Col.Dispose();
			GrayValue.Dispose();
			dPhi.Dispose();

			nCount = 0;

			//get_region_points(LineSeg, &Row, &Col);                 //get_region_points算子的作用是什么?
			nLength = Col.Length;
			if (nLength < 4)
			{
				return nCount;
			}

			GetRegionPoints(LineSeg, out Row, out Col);             //获得线型区域的点
			nLength = Col.TupleIsNumber();
			if (nLength < 4)
			{
				return nCount;
			}
			diff = Col[0].I - Col[nLength - 1].I;
			//orientation_region(LineSeg, &dPhi);
			OrientationRegion(LineSeg, out dPhi);
			TupleFabs(dPhi, out dPhi);                             //求绝对值
			switch (nDirect)
			{
				case (int)e_LineDirect.L2R:
					if (diff > 0)
					{


						TupleInverse(Row, out Row);
						TupleSort(Col, out Col);
					}
					break;
				case (int)e_LineDirect.R2L:
					if (diff < 0)
					{

						TupleInverse(Row, out Row);
						TupleInverse(Col, out Col);
					}
					else if (diff > 0 && dPhi < PI / 4)
					{


						TupleSort(Col, out Col);
						TupleInverse(Col, out Col);
					}
					break;
				case (int)e_LineDirect.T2B:
					if (diff > 0 && dPhi < PI / 4)
					{

						TupleSort(Col, out Col);
						TupleInverse(Col, out Col);

					}
					break;
				case (int)e_LineDirect.B2T:
					if (diff < 0)
					{

						TupleInverse(Row, out Row);
						TupleInverse(Col, out Col);
					}
					else
					{


						TupleInverse(Row, out Row);
						TupleSort(Col, out Col);



					}
					break;
				default:
					break;
			}

			HObject ImgMedian, ImgDomain, RegInter;
			HTuple nNum;

			GenEmptyObj(out ImgMedian);
			GenEmptyObj(out ImgDomain);
			GenEmptyObj(out RegInter);
			GetDomain(Image, out ImgDomain);
			Intersection(LineSeg, ImgDomain, out RegInter);
			Connection(RegInter, out RegInter);
			SelectShape(RegInter, out RegInter, "area", "and", 1, 99999999);
			CountObj(RegInter, out nNum);

			if (nNum[0].I == 0)
			{
				return nCount;
			}
			ReduceDomain(Image, LineSeg, out ImgMedian);
			if (bMean)
			{
				HObject regDilation, imgMean;
				DilationCircle(LineSeg, out regDilation, 3.5);
				ReduceDomain(Image, regDilation, out imgMean);
				MeanImage(imgMean, out imgMean, 3, 3);
				ReduceDomain(imgMean, LineSeg, out ImgMedian);
			}

			MedianImage(ImgMedian, out ImgMedian, "circle", 5, "continued");

			HTuple ptr, type;
			ptr = new HTuple();
			type = new HTuple();
			byte* pointer = null;
			ptr.Dispose();
			type.Dispose();
			int i, x, y;
			//第二个输出参数 poiter包含什么信息?pointer是一个指向图像数据块的指针，每个字节是8位因为需要明确c#中如何访问指针型数据

			GetImagePointer1(ImgMedian, out ptr, out type, out nImgWidth, out nImgHeight);
			pointer = (byte*)ptr[0].L;                          //将长整型强制转为byte*指针数据
																//pointer
																//c#整数和指针可以相互转换
																//byte* ptr = (byte *)pointer[0].L;		
																//需要将明确byte[]首地址是什么格式
																//计算机是一个内存对应一个字节地址，64位计算机一个地址的长度是64位
																//指针总是指向变量的第一个字节
																//因此这里需要指向图像数据字节数组第一个元素的指针，因此需要byte*还是long *?不论什么类型指针，在内存中都是64位，只是不同类型指针加1时所加的字节数不同
																//每一个内存单元都有一个地址
																//内存单元 = 8b一个int型数据由4个内存单元组成，一共4B 32b，理论上这四个内存单元每一个都有一个地址，但是用指针指向时只有储存第一个存储单元的地址即可。任何类型的指针都是指向该类型变量的首地址，由于指针类型已确定，因此编译器能够确定数据所占字节数，从而编译时会安排相应的指令访问该地址中的数据。所以指针 + 1，代表其储存的地址直接略过连续的几个存储单元（int类型指针则直接把连续的4个字节看成整体，指针加一则一次性直接跳过4个），变为下一个相同类型数据的首储存单元地址。


			for (i = 0; i < nLength; ++i)               //这个nLength需要c#中修改
			{
				x = Col[i].I;
				y = Row[i].I;
				if (x < nImgWidth[0].I && y < nImgHeight[0].I)            //如果在区域内
				{
					GrayValue[i] = (HTuple)pointer[y * nImgWidth + x];  //获取矩阵内某点的像素值 //这里必须看如何通指针访问数组元素 //GrayValue的值类型全是byte型,但是这里全转为HTuple型
				}
				else
				{
					GrayValue[i] = 0;
				}
			}

			if (nType == 0)//灰度由高到低
			{
				for (i = 0; i < nLength - 5; ++i)
				{
					nGrayDiff1 = GrayValue[i].I - GrayValue[i + 3].I;
					nGrayDiff2 = GrayValue[i + 1].I - GrayValue[i + 4].I;
					nGrayDiff3 = GrayValue[i + 2].I - GrayValue[i + 5].I;
					if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)         //找到亮点
					{
						RowPt[nCount] = Row[i + 3];
						ColPt[nCount] = Col[i + 3];
						++nCount;
						//找所有点继续
						if (!bAllPoint)
						{
							break;
						}
					}

				}
			}
			else if (nType == 1)                //灰度从低到高
			{
				for (i = 0; i < nLength - 5; ++i)
				{
					nGrayDiff1 = GrayValue[i + 3].I - GrayValue[i].I;
					nGrayDiff2 = GrayValue[i + 4].I - GrayValue[i + 1].I;
					nGrayDiff3 = GrayValue[i + 5].I - GrayValue[i + 2].I;
					if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)
					{
						RowPt[nCount] = Row[i + 3];
						ColPt[nCount] = Col[i + 3];
						++nCount;
						//找所有点继续
						if (!bAllPoint)
						{
							break;
						}
					}
				}
			}
			return nCount;
		}

		unsafe public int findEdgePointDouble(HObject Image, HObject LineSeg,
			out HTuple RowPt, out HTuple ColPt, int nEdge = 10, int nDirect = (int)e_LineDirect.L2R, int nType = 0, bool bMean = false)
		{









			// 清空目的数组
			RowPt = new HTuple();
			ColPt = new HTuple();
			HObject ImageReduce, ImageGauss, NewLine;
			HTuple Row, Col, GrayValue;
			int nGrayDiff1, nGrayDiff2, nGrayDiff3, nCount, nLength, diff;         //原代码是long 型，现在改成int型
			HTuple nImgWidth, nImgHeight;
			HTuple dPhi;
			GenEmptyObj(out ImageReduce);
			GenEmptyObj(out ImageGauss);
			GenEmptyObj(out NewLine);
			Row = new HTuple();
			Col = new HTuple();
			GrayValue = new HTuple();
			dPhi = new HTuple();
			Row.Dispose();
			Col.Dispose();
			RowPt.Dispose();
			ColPt.Dispose();
			GrayValue.Dispose();
			dPhi.Dispose();


			// 边界点的数目
			nCount = 0;

			nLength = Col.Length;
			if (nLength < 4)
			{
				return nCount;
			}

			GetRegionPoints(LineSeg, out Row, out Col);             //获得线型区域的点
			nLength = Col.TupleIsNumber();
			if (nLength < 4)
			{
				return nCount;
			}
			diff = Col[0].I - Col[nLength - 1].I;
			//orientation_region(LineSeg, &dPhi);
			OrientationRegion(LineSeg, out dPhi);
			TupleFabs(dPhi, out dPhi);                             //求绝对值


			switch (nDirect)
			{
				case (int)e_LineDirect.L2R:
					if (diff > 0)
					{


						TupleInverse(Row, out Row);
						TupleSort(Col, out Col);
					}
					break;
				case (int)e_LineDirect.R2L:
					if (diff < 0)
					{

						TupleInverse(Row, out Row);
						TupleInverse(Col, out Col);
					}
					else if (diff > 0 && dPhi < PI / 4)
					{


						TupleSort(Col, out Col);
						TupleInverse(Col, out Col);
					}
					break;
				case (int)e_LineDirect.T2B:
					if (diff > 0 && dPhi < PI / 4)
					{

						TupleSort(Col, out Col);
						TupleInverse(Col, out Col);

					}
					break;
				case (int)e_LineDirect.B2T:
					if (diff < 0)
					{

						TupleInverse(Row, out Row);
						TupleInverse(Col, out Col);
					}
					else
					{


						TupleInverse(Row, out Row);
						TupleSort(Col, out Col);



					}
					break;
				default:
					break;
			}

			HObject ImgMedian, ImgDomain, RegInter;
			HTuple nNum;

			GenEmptyObj(out ImgMedian);
			GenEmptyObj(out ImgDomain);
			GenEmptyObj(out RegInter);
			GetDomain(Image, out ImgDomain);
			Intersection(LineSeg, ImgDomain, out RegInter);
			Connection(RegInter, out RegInter);
			SelectShape(RegInter, out RegInter, "area", "and", 1, 99999999);
			CountObj(RegInter, out nNum);

			if (nNum[0].I == 0)
			{
				return nCount;
			}

			ReduceDomain(Image, LineSeg, out ImgMedian);
			if (bMean)
			{
				HObject regDilation, imgMean;
				DilationCircle(LineSeg, out regDilation, 3.5);
				ReduceDomain(Image, regDilation, out imgMean);
				MeanImage(imgMean, out imgMean, 3, 3);
				ReduceDomain(imgMean, LineSeg, out ImgMedian);
			}

			MedianImage(ImgMedian, out ImgMedian, "circle", 5, "continued");

			HTuple ptr, type;
			ptr = new HTuple();
			type = new HTuple();
			byte* pointer = null;
			ptr.Dispose();
			type.Dispose();
			int i, j, x, y;
			//第二个输出参数 poiter包含什么信息?pointer是一个指向图像数据块的指针，每个字节是8位因为需要明确c#中如何访问指针型数据

			GetImagePointer1(ImgMedian, out ptr, out type, out nImgWidth, out nImgHeight);
			pointer = (byte*)ptr[0].L;                          //将长整型强制转为byte*指针数据
																//pointer
																//c#整数和指针可以相互转换
																//byte* ptr = (byte *)pointer[0].L;		
																//需要将明确byte[]首地址是什么格式
																//计算机是一个内存对应一个字节地址，64位计算机一个地址的长度是64位
																//指针总是指向变量的第一个字节
																//因此这里需要指向图像数据字节数组第一个元素的指针，因此需要byte*还是long *?不论什么类型指针，在内存中都是64位，只是不同类型指针加1时所加的字节数不同
																//每一个内存单元都有一个地址
																//内存单元 = 8b一个int型数据由4个内存单元组成，一共4B 32b，理论上这四个内存单元每一个都有一个地址，但是用指针指向时只有储存第一个存储单元的地址即可。任何类型的指针都是指向该类型变量的首地址，由于指针类型已确定，因此编译器能够确定数据所占字节数，从而编译时会安排相应的指令访问该地址中的数据。所以指针 + 1，代表其储存的地址直接略过连续的几个存储单元（int类型指针则直接把连续的4个字节看成整体，指针加一则一次性直接跳过4个），变为下一个相同类型数据的首储存单元地址。


			for (i = 0; i < nLength; ++i)               //这个nLength需要c#中修改
			{
				x = Col[i].I;
				y = Row[i].I;
				if (x < nImgWidth[0].I && y < nImgHeight[0].I)            //如果在区域内
				{
					GrayValue[i] = (HTuple)pointer[y * nImgWidth + x];  //获取矩阵内某点的像素值 //这里必须看如何通指针访问数组元素 //GrayValue的值类型全是byte型,但是这里全转为HTuple型
				}
				else
				{
					GrayValue[i] = 0;
				}
			}



			if (nType == 0)//灰度由高到低
			{
				for (i = 0; i < nLength - 5; ++i)
				{
					nGrayDiff1 = GrayValue[i].I - GrayValue[i + 3].I;
					nGrayDiff2 = GrayValue[i + 1].I - GrayValue[i + 4].I;
					nGrayDiff3 = GrayValue[i + 2].I - GrayValue[i + 5].I;
					if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)
					{
						RowPt[nCount] = Row[i + 3];
						ColPt[nCount] = Col[i + 3];
						++nCount;
						break;
					}
				}

				if (nCount > 0)
				{
					for (j = nLength - 1; j > i + 8; --j)
					{
						nGrayDiff1 = GrayValue[j].I - GrayValue[j - 3].I;
						nGrayDiff2 = GrayValue[j - 1].I - GrayValue[j - 4].I;
						nGrayDiff3 = GrayValue[j - 2].I - GrayValue[j - 5].I;
						if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)
						{
							RowPt[nCount] = Row[j - 3];
							ColPt[nCount] = Col[j - 3];

							++nCount;
							break;
						}
					}
				}
			}
			else if (nType == 1)
			{
				for (i = 0; i < nLength - 5; ++i)
				{
					nGrayDiff1 = GrayValue[i + 3].I - GrayValue[i].I;
					nGrayDiff2 = GrayValue[i + 4].I - GrayValue[i + 1].I;
					nGrayDiff3 = GrayValue[i + 5].I - GrayValue[i + 2].I;
					if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)//不是边缘时，差值不大于nEdge;找到边缘时，差值才大于nEdge
					{
						RowPt[nCount] = Row[i + 3];
						ColPt[nCount] = Col[i + 3];
						++nCount;
						break;
					}
				}

				if (nCount > 0)
				{
					for (j = nLength - 1; j > i + 8; --j)
					{
						nGrayDiff1 = GrayValue[j - 3].I - GrayValue[j].I;
						nGrayDiff2 = GrayValue[j - 4].I - GrayValue[j - 1].I;
						nGrayDiff3 = GrayValue[j - 5].I - GrayValue[j - 2].I;
						if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)
						{
							RowPt[nCount] = Row[j - 3];
							ColPt[nCount] = Col[j - 3];

							++nCount;
							break;
						}
					}
				}
			}
			return nCount;
		}


		//RowPt和ColPt就是引用
		/// <summary>
		///连按两下注释
		/// </summary>
		/// <param name="Image"></param>
		/// <param name="LineSeg"></param>
		/// <param name="RowPt"></param>
		/// <param name="ColPt"></param>
		/// <param name="nEdge"></param>
		/// <param name="fRange"></param>
		/// <param name="oldWidth"></param>
		/// <param name="nType"></param>
		/// <param name="bMean"></param>
		/// <returns></returns>
		//testEdgePointDouble函数的功能还不明确
		public int testEdgePointDouble(HObject Image, HObject LineSeg,
		HTuple RowPt, HTuple ColPt, int nEdge, float fRange,
		float oldWidth, int nType = 0, bool bMean = false)
		{
			float newWidth;
			float range = fRange;//宽度浮动范围比例	
			int newPtCol, newPtRow;
			int tempColL, tempColR;
			HTuple row1, row2, numTemp, col1, col2;

			HObject tempLineL, tempLineR, concatLines, newLine;
			HTuple rPtTemp, cPtTemp;
			row1 = new HTuple();
			row2 = new HTuple();
			col1 = new HTuple();
			col2 = new HTuple();
			numTemp = new HTuple();
			row1.Dispose();
			row2.Dispose();
			col1.Dispose();
			col2.Dispose();
			GenEmptyObj(out tempLineL);
			GenEmptyObj(out tempLineR);
			GenEmptyObj(out concatLines);
			GenEmptyObj(out newLine);
			newWidth = ColPt[1].I - ColPt[0].I;
			if (Abs(newWidth - oldWidth) / (oldWidth + 0.0001) < range)
				return 0;

			SmallestRectangle1(LineSeg, out row1, out col1, out row2, out col2);
			tempColL = (int)(ColPt[0].I + oldWidth * (1 - range / 2));
			tempColR = (int)(ColPt[0].I + oldWidth * (1 + range / 2));
			//oldwidth和newwidth分别代表什么?


			GenRegionLine(out tempLineL, row1.I - 5, tempColL, row2.I + 5, tempColL);
			GenRegionLine(out tempLineR, row1.I - 5, tempColR, row2.I + 5, tempColR);
			ConcatObj(tempLineL, tempLineR, out concatLines);
			Union1(concatLines, out concatLines);
			Difference(LineSeg, concatLines, out newLine);
			Connection(newLine, out newLine);
			SelectShape(newLine, out newLine, "area", "and", 1, 9999);
			CountObj(newLine, out numTemp);
			if (numTemp.I != 3)
				return 0;



			SortRegion(newLine, out newLine, "upper_left", "true", "column");       //必须看一下sortRegion的作用是什么
			SelectObj(newLine, out newLine, 2);
			int nRet = findEdgePointSingle(Image, newLine, out rPtTemp, out cPtTemp, nEdge, (int)e_LineDirect.R2L, nType, bMean);


			if (nRet == 1)
			{
				newPtCol = cPtTemp[0].I;
				newPtRow = rPtTemp[0].I;
				if (Abs(newPtCol - ColPt[0].I - oldWidth) / oldWidth < range)
				{
					ColPt[1] = newPtCol;
					RowPt[1] = newPtRow;
					return 0;
				}
			}

			tempColL = (int)(ColPt[1].I - oldWidth * (1 + range / 2));              //tempColL意义是什么?
			tempColR = (int)(ColPt[1].I - oldWidth * (1 - range / 2));


			GenRegionLine(out tempLineL, row1.I - 5, tempColL, row2.I + 5, tempColL);
			GenRegionLine(out tempLineR, row1.I - 5, tempColR, row2.I + 5, tempColR);

			ConcatObj(tempLineL, tempLineR, out concatLines);
			Union1(concatLines, out concatLines);
			Difference(LineSeg, concatLines, out newLine);
			Connection(newLine, out newLine);
			SelectShape(newLine, out newLine, "area", "and", 1, 9999);

			CountObj(newLine, out numTemp);
			if (numTemp.I != 3)
			{
				return 0;
			}
			SortRegion(newLine, out newLine, "upper_left", "true", "column");
			SelectObj(newLine, out newLine, 2);
			nRet = findEdgePointSingle(Image, newLine, out rPtTemp, out cPtTemp, nEdge, (int)e_LineDirect.L2R, nType, bMean);

			if (nRet == 1)
			{
				newPtCol = cPtTemp[0].I;
				newPtRow = rPtTemp[0].I;
				if (Abs(ColPt[1].I - newPtCol - oldWidth) / oldWidth < range)
				{
					//这里可以直接修改ColPt，因为c#对像传的是引用
					ColPt[0] = newPtCol;
					RowPt[0] = newPtRow;
					return 0;
				}
			}
			if (newWidth > oldWidth)
			{
				return 1;//左右大于实际
			}
			else
				return 2;//左右小于实际

		}

		//*功能：自动判连瓶，返回是否找到正确边缘点
		public bool findEdgePointDoubleEx(HObject Image, HObject LineSeg,
		out HTuple RowPt, out HTuple ColPt, int nEdge, float fRange,
		float oldWidth, int nType = 0/*=0*/, bool bMean = false/*=FALSE*/, bool bRedress = true/* = true*/)
		{
			int iPointNum = 0;
			RowPt = new HTuple();
			ColPt = new HTuple();
			RowPt.Dispose();
			ColPt.Dispose();

			if (findEdgePointDouble(Image, LineSeg, out RowPt, out ColPt, nEdge, (int)e_LineDirect.L2R, nType, bMean) < 2)
			{
				return false;
			}

			if (fRange > 0 && oldWidth > 0 && bRedress)//自动纠错
			{
				//这里*RowPt表示访问值，testEdgePointDouble的参数是引用，这里使用了*RowPt表示传给函数一个引用
				iPointNum = testEdgePointDouble(Image, LineSeg, RowPt, ColPt, nEdge, fRange, oldWidth, nType, bMean);
				if (iPointNum != 0)/*左右点都不正确时,缩放line重定位*/
				{

					HObject zoomLine;
					if (iPointNum == 1)//左右大于实际
					{

						//	*RowPt = HTuple();
						//	*ColPt = HTuple();   这两句代码是否有实际意义?目的是不是为了清空变量?
						GenRegionLine(out zoomLine, RowPt[0], ColPt[0], RowPt[1], ColPt[1]);
						if (findEdgePointDouble(Image, zoomLine, out RowPt, out ColPt, nEdge, (int)e_LineDirect.L2R, nType, bMean) < 2)
						{
							return false;
						}
						if (testEdgePointDouble(Image, zoomLine, RowPt, ColPt, nEdge, fRange, oldWidth, nType, bMean) != 0)
						{
							return false;
						}
					}
					else if (iPointNum == 2)//左右小于实际
					{
						int MoveDist = 30;//左右扩展的长度
						HTuple Phi;
						double rowLeft, colLeft, rowRight, colRight;


						LineOrientation(RowPt[0], ColPt[0], RowPt[1], ColPt[1], out Phi);
						rowLeft = RowPt[0].D + MoveDist * Sin(Phi.D);
						colLeft = ColPt[0].D - MoveDist * Cos(Phi.D);
						rowRight = RowPt[1].D - MoveDist * Sin(Phi.D);
						colRight = ColPt[1].D - MoveDist * Cos(Phi.D);
						GenRegionLine(out zoomLine, rowLeft, colLeft, rowRight, colRight);

						if (findEdgePointDouble(Image, zoomLine, out RowPt, out ColPt, nEdge, (int)e_LineDirect.L2R, nType, bMean) < 2)
						{
							return false;
						}
						if (testEdgePointDouble(Image, zoomLine, RowPt, ColPt, nEdge, fRange, oldWidth, nType, bMean) != 0)
						{
							return false;
						}
					}
				}
			}
			return false;

		}

		public s_CheckResult findBulbLine(HObject hImg, HObject regBulbLing, out HTuple RowPt, out HTuple ColPt)
		{
			s_CheckResult sRsu = new s_CheckResult();
			HObject imgReduce, imgMedian;
			HObject regMedian, regRect;
			HObject xldSub, xldLeft, xldRight;
			HTuple lNum;

			GenEmptyObj(out imgReduce);
			GenEmptyObj(out imgMedian);
			GenEmptyObj(out regMedian);
			GenEmptyObj(out regRect);
			GenEmptyObj(out xldSub);
			GenEmptyObj(out xldLeft);
			GenEmptyObj(out xldRight);

			RowPt = new HTuple();
			ColPt = new HTuple();

			HTuple Row1, Col1, Row2, Column2, LeftRow, LeftCol, RightRow, RightCol;
			HTuple SelLeftRow, SelLeftCol, SelRightRow, SelRightCol;
			HTuple tpMin, tpIndices;
			Row1 = new HTuple();
			Col1 = new HTuple();
			Row2 = new HTuple();
			Column2 = new HTuple();
			LeftRow = new HTuple();
			LeftCol = new HTuple();
			RightRow = new HTuple();
			RightCol = new HTuple();
			SelLeftRow = new HTuple();
			SelLeftCol = new HTuple();
			SelRightRow = new HTuple();
			SelRightCol = new HTuple();
			tpMin = new HTuple();
			tpIndices = new HTuple();
			Row1.Dispose();
			Col1.Dispose();
			Row2.Dispose();
			Column2.Dispose();
			LeftRow.Dispose();
			LeftCol.Dispose();
			RightRow.Dispose();
			RightCol.Dispose();
			SelLeftRow.Dispose();
			SelLeftCol.Dispose();
			SelRightRow.Dispose();
			SelRightCol.Dispose();
			tpMin.Dispose();





			ReduceDomain(hImg, regBulbLing, out imgReduce);
			MedianRect(imgReduce, out regMedian, 1, 30);



			//有没有必要换成HTuple再传给Halcon函数?没必要，可以为halcon函数传递原始类型
			SmallestRectangle1(regMedian, out Row1, out Col1, out Row2, out Column2);
			GenRectangle1(out regRect, Row1[0].I + 2, Col1 + 2, Row2 - 2, Column2 - 2);
			ReduceDomain(hImg, regRect, out imgReduce);
			EdgesSubPix(imgReduce, out xldSub, "canny", 1, 10, 20);
			SelectShapeXld(xldSub, out xldSub, "height", "and", 40, 99999999);
			SortContoursXld(xldSub, out xldSub, "upper_left", "true", "column");
			CountObj(xldSub, out lNum);

			if (lNum < 2)
			{



				sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);
				CopyObj(regRect, out sRsu.regError, 1, -2);
				return sRsu;
			}


			//左侧点


			SelectObj(xldSub, out xldLeft, 1);
			SelectObj(xldSub, out xldRight, lNum);

			GetContourXld(xldLeft, out LeftRow, out LeftCol);
			TupleMin(LeftCol, out tpMin);
			TupleFind(LeftCol, tpMin, out tpIndices);
			TupleSelect(LeftRow, tpIndices, out SelLeftRow);


			TupleSelect(LeftCol, tpIndices, out SelLeftCol);



			tpIndices = 0;

			GetContourXld(xldRight, out RightRow, out RightCol);
			TupleMax(RightCol, out tpMin);
			TupleFind(RightCol, tpMin, out tpIndices);
			TupleSelect(RightRow, tpIndices, out SelRightRow);
			TupleSelect(RightCol, tpIndices, out SelRightCol);

			RowPt[0] = SelLeftRow[0];
			ColPt[0] = SelLeftCol[0];
			RowPt[1] = SelRightRow[0];
			ColPt[1] = SelRightCol[0];
			return sRsu;

		}
		//寻找pet环
		public s_CheckResult findPetRing(HObject hImg, HObject regPetRing, out HTuple RowPt, out HTuple ColPt,
		int iGray, int iBackground = 1/* = 1*/)
		{
			s_CheckResult sRsu = new s_CheckResult();
			HObject ImageReduced, Regions;
			HObject ROILeft, ROIRight;
			HTuple dRow1, dCol1, dRow2, dCol2;
			HTuple num = 0, Area, AreaAll;
			HTuple rowCenter, colCenter;
			double LinePhi;
			int i;
			//HTuple类型的输出参数在输入函数前，需定义 HTuple RowPt;
			RowPt = new HTuple();
			ColPt = new HTuple();
			RowPt.Dispose();
			ColPt.Dispose();
			dRow1 = new HTuple();
			dCol1 = new HTuple();
			dRow2 = new HTuple();
			dCol2 = new HTuple();
			Area = new HTuple();
			AreaAll = new HTuple();
			dRow1.Dispose();
			dCol1.Dispose();
			dRow2.Dispose();
			dCol2.Dispose();
			Area.Dispose();
			AreaAll.Dispose();

			//输入参数可以用基本类型，输出参数必须是Htuple

			ReduceDomain(hImg, regPetRing, out ImageReduced);

			if (0 == iBackground)
			{
				Threshold(ImageReduced, out Regions, iGray, 255);

			}
			else
			{
				Threshold(ImageReduced, out Regions, 0, iGray);
			}



			Connection(Regions, out Regions);
			SelectShapeStd(Regions, out Regions, "max_area", 70);
			SmallestRectangle1(Regions, out dRow1, out dCol1, out dRow2, out dCol2);



			GenRectangle1(out ROILeft, dRow1, dCol1, dRow2, (dCol1 + dCol2) / 2);
			ReduceDomain(hImg, ROILeft, out ImageReduced);
			if (0 == iBackground)
			{
				Threshold(ImageReduced, out Regions, iGray, 255);
			}
			else
			{
				Threshold(ImageReduced, out Regions, 0, iGray);
			}

			Connection(Regions, out Regions);
			SelectShapeStd(Regions, out Regions, "max_area", 70);
			AreaCenter(Regions, out Area, out rowCenter, out colCenter);
			AreaCenter(ROILeft, out AreaAll, out rowCenter, out colCenter);

		


			if (Area < 50 || (AreaAll - Area) < 50)		//HTuple可以直接和基础类型运算
            {
				sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);

				if(Area > 0)
                {
					CopyObj(Regions,out sRsu.regError, 1, -1);
                }
                else
                {
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
                }

				sRsu.strDescription = "Not found left locate point!";
				return sRsu;
			}


			HTuple LeftRows, LeftColumns;
			int LeftColumnMin, LeftRowsMax;
			HTuple Indices, Length, Rows1;
			//获得轮廓点
			

			GetRegionConvex(Regions, out LeftRows,out  LeftColumns);
			TupleLength(LeftColumns, out Length);
			
			if(Length == 0)
            {
				sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);
				if(Area > 0)
                {
					CopyObj(Regions,out sRsu.regError, 1, -1);
                }
                else
                {
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
                }
				sRsu.strDescription = "Not found left locate point!";
				return sRsu;

			}
			else
			{

				//TupleSortIndex起了什么作用?

				TupleSortIndex(LeftColumns,out Indices);
				LeftColumnMin = LeftColumns[Indices[0].I];
				LeftRowsMax = LeftRows[Indices[0].I];
				if(Length > 1)
                {
					for(i = 0; i < Length; ++i)
                    {
						if (LeftColumns[Indices[i].I].I - LeftColumns[Indices[0].I].I < 4)
						{
							if (LeftRowsMax < LeftRows[Indices[i].I].I)
							{
								LeftRowsMax = LeftRows[Indices[i].I].I;
								LeftColumnMin = LeftColumns[Indices[i].I].I;
							}
						}
						else
							break;
                    }

                }
			}
			
			//当HTuple与基础类型做运算时，结果是什么类型?
			GenRectangle1(out ROIRight, dRow1, (dCol1 + dCol2) / 2, dRow2, dCol2);
			ReduceDomain(hImg,ROIRight,out ImageReduced);
			if(0 == iBackground)
            {
				Threshold(ImageReduced,out Regions,iGray,255);

            }
            else
            {
				Threshold(ImageReduced,out Regions, 0, iGray);
            }
			//检测黑点可以先找到种子像素，然后获取图像的指针，利用种子像素向外拓展(利用点生成区域，再检测区域的特征)
			Connection(Regions,out Regions);
			SelectShapeStd(Regions,out Regions,"max_area",70);
			

			AreaCenter(Regions,out Area,out rowCenter,out colCenter);
			AreaCenter(ROIRight,out AreaAll,out rowCenter,out colCenter);
			if((Area< 50) || (AreaAll - Area) < 50)
            {
				sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);
				if(Area > 0)
                {
					CopyObj(Regions, out sRsu.regError, 1, -1);
                }
                else
                {
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
                }
				sRsu.strDescription = "Not found right locate point!";
				return sRsu;
			}


			HTuple RightRows, RightColumns;
			//c#变量的初始化怎么做?
			int RightColumnMax = 0, RightRowsMax = 0;//c#中不初始化变量会报错，这里要注意初始化为零是否正确
			HTuple Rows3;
	

			GetRegionConvex(Regions,out RightRows,out RightColumns);
			TupleLength(RightColumns,out Length);

			if(Length == 0)
            {
				sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);
				if(Area > 0)
                {
					CopyObj(Regions,out sRsu.regError, 1, -1);
                }
                else
                {
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
                }
			}
		

            else
            {
				TupleSortIndex(RightColumns,out Indices);
				RightColumnMax = RightColumns[Indices[Length[0].I - 1].I].I;
				RightRowsMax = RightRows[Indices[Length[0].I - 1].I].I;
				if(Length > 1)
                {
					for(i = 1; i < Length; ++i)
                    {
						if (RightColumns[Indices[Length[0].I - 1].I].I - RightColumns[Indices[Length[0].I - 1 - i].I].I < 4)
						{
							if (RightRowsMax < RightRows[Indices[Length[0].I - 1 - i].I])
							{
								RightColumnMax = RightColumns[Indices[Length[0] - 1 - i].I];
								RightRowsMax = RightRows[Indices[Length[0].I - 1 - i].I];
							}
						}
						else
							break;
                    }
                }
            }
			//e_DefectType e_DefectType
			//如果基准线角度不对直接返回
			if (Abs(LeftRowsMax - RightRowsMax) > 60)
			{
				sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
				if (Area > 0)
				{
					//copy_obj(Regions, &sRsu.regError, 1, -1);
					CopyObj(Regions,out sRsu.regError,1,-1);
				}
				else
				{
					
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
				}
				return sRsu;
			}

			

			RowPt[0] = LeftRowsMax;
			RowPt[1] = RightRowsMax;
			ColPt[0] = LeftColumnMin;
			ColPt[1] = RightColumnMax;
			return sRsu;
		}

		public bool genValidROI(HObject imgSrc,ref s_pROIPara roiPara, HObject ROI, out HObject validROI)
        {
			
			
			//决定HLong怎么用之前，需要明确HLong型变量用在了哪里，这个函数接受了什么类型参数
			//gen_empty_obj(validROI);


			HTuple nRow1, nCol1, nRow2, nCol2;
			HObject imgReduced;
			HObject PartionRegion, RegionBin, ConnectedRegions, SelectedRegions;
			nRow1 = new HTuple();
			nCol1 = new HTuple();
			nRow2 = new HTuple();
			nCol2 = new HTuple();
			nRow1.Dispose();
			nCol1.Dispose();
			nRow2.Dispose();
			nCol2.Dispose();


			GenEmptyObj(out imgReduced);
			GenEmptyObj(out PartionRegion);
			GenEmptyObj(out RegionBin);
			GenEmptyObj(out ConnectedRegions);
			GenEmptyObj(out SelectedRegions);
			GenEmptyObj(out validROI);
			

			
			


			int nPartNum;//分割份数	
			ReduceDomain(imgSrc,ROI,out imgReduced);
			SmallestRectangle1(ROI ,out nRow1,out nCol1,out nRow2,out nCol2);
			int nRegionHeight = nRow2 - nRow1 + 1;
			HTuple nWidth, nHeight;
			GetImageSize(imgSrc,out nWidth,out nHeight);
			float fHeiRatio = (float)nRegionHeight / (float)nHeight;

			if (fHeiRatio < 0.3)
			{
				nPartNum = 1;
			}
			else if (fHeiRatio >= 0.3 && fHeiRatio < 0.6)
			{
				nPartNum = 2;
			}
			else
			{
				nPartNum = 3;
			}


			



			int nPartHeight = nRegionHeight / nPartNum + 2;
			PartitionRectangle(ROI,out PartionRegion, nWidth, nPartHeight);
			HTuple Mean, PreMean, Devitation;

			Intensity(PartionRegion, imgSrc, out Mean, out Devitation);
			PreMean = Mean * roiPara.fRoiRatio;

			HObject selected, tempRegion; ;
			GenEmptyObj(out RegionBin);


			
			

			HTuple nNumber;
			CountObj(PartionRegion,  out nNumber);
			for(int i = 0; i < nNumber; i++)
            {
				SelectObj(PartionRegion,out selected,i + 1);

				ReduceDomain(imgSrc, selected,out tempRegion);//这里同一个变量表示是否可行
				Threshold(tempRegion, out tempRegion,PreMean[i],255);
				ConcatObj(RegionBin, tempRegion, out RegionBin);
            }

			Union1(RegionBin,out RegionBin);

			HTuple areaPro, areaMin;
			HTuple rowCenter, colCenter;
		

			AreaCenter(ROI,out areaPro,out rowCenter,out colCenter);
			areaMin = areaPro / 3;
			areaMin = areaMin[0].I > 550 ? 550 : areaMin;
			//areaMin = areaMin[0].I > 550 : areaMin ?550;






			ClosingRectangle1(RegionBin,out RegionBin,1,roiPara.nClosingWH);
			ClosingRectangle1(RegionBin,out RegionBin,roiPara.nClosingWH,1);



			Connection(RegionBin, out ConnectedRegions);
			SelectShape(ConnectedRegions, out SelectedRegions, "area", "and", areaMin, 9999999);
			

			// 判断是几个区域
			
			CountObj(SelectedRegions,out nNumber);
			if(nNumber > 1)
            {
				SelectShapeStd(SelectedRegions,out SelectedRegions, "max_area", 70);
            }
            else
            {
				CopyObj(SelectedRegions,out validROI,1,-1);
            }

			// 找出预处理区域后缩小一定范围进行检测
			// 主要作用是切断那些裂纹,大缺陷
		//	fill_up(SelectedRegions, &SelectedRegions);
			FillUp(SelectedRegions,out SelectedRegions);


			HObject RegGap;
			

			Difference(ROI,SelectedRegions,out RegGap);
			OpeningRectangle1(RegGap,out RegGap,roiPara.nGapWH,roiPara.nGapWH);
			Connection(RegGap, out RegGap);
			SelectShape(RegGap,out RegGap, (new HTuple("width")).TupleConcat(
		  "height"),"and", (new HTuple(roiPara.nGapWH)).TupleConcat(roiPara.nGapWH), (new HTuple(9999)).TupleConcat(
		  9999));


			CountObj(RegGap,out nNumber);


			
		

			// 往内部缩小一定区域
			ErosionCircle(SelectedRegions,out SelectedRegions,new HTuple(2.5));
			//2013.9.16 nanjc 一个区域缩为两个区域，选大区域
			Connection(SelectedRegions,out SelectedRegions);

			CountObj(SelectedRegions,out nNumber);
			if(nNumber > 1)
            {
				SelectShapeStd(SelectedRegions,out SelectedRegions,"max_area",70);

            }

			CopyObj(SelectedRegions,out validROI, 1, -1);
			return true;
			// 判断实际处理区域比设置的处理区域小很多的情况
		}


		public s_CheckResult segmentChar(HObject imageSrc ,HObject regValid,ref s_pOCRPublicPara spOCRPublicPara,out HObject regChar)
		{

			
	

			s_CheckResult sRsu = new s_CheckResult();

			GenEmptyRegion(out regChar);
			HObject ImgMean, ImgReducedSrc, ImgReducedMean;
			HObject regThresh, regDynThresh, regThresh1, regThresh2, regSigChar, regConChar, regAllChar, regTemp, regVar;
			GenEmptyObj(out ImgMean);
			GenEmptyObj(out ImgReducedSrc);
			GenEmptyObj(out ImgReducedMean);
			GenEmptyObj(out regThresh);
			GenEmptyObj(out regDynThresh);
			GenEmptyObj(out regThresh1);
			GenEmptyObj(out regThresh2);
			GenEmptyObj(out regSigChar);
			GenEmptyObj(out regConChar);
			GenEmptyObj(out regAllChar);
			GenEmptyObj(out regTemp);
			GenEmptyObj(out regVar);

			int iCharWidth = spOCRPublicPara.iCharWidth;
			int iCharHeight = spOCRPublicPara.iCharHeight;
			int iCharSpace = spOCRPublicPara.iCharSpace;
			double dVarStdDevScale = spOCRPublicPara.dVarStdDevScale;
			int iVarAbsGray = spOCRPublicPara.iVarAbsGray;
			HTuple lNum, lArea = 30;
			MeanImage(imageSrc, out ImgMean, 17, 9);
			ReduceDomain(imageSrc, regValid, out ImgReducedSrc);

			ReduceDomain(ImgMean, regValid, out ImgReducedMean);
			DynThreshold(ImgReducedSrc, ImgReducedMean, out  regDynThresh, spOCRPublicPara.iSegEdge1, "dark");
			Threshold(ImgReducedSrc, out regThresh, 0, spOCRPublicPara.iSegGray1);


			

			if(0 == spOCRPublicPara.iRelation1)
            {
				Intersection(regDynThresh, regThresh,out regThresh1);
            }
            else
            {
				Union2(regDynThresh, regThresh, out regThresh1);
            }

			//条件2


			DynThreshold(ImgReducedSrc,ImgReducedMean,out regDynThresh,spOCRPublicPara.iSegEdge2,"dark");
			Threshold(ImgReducedSrc,out regThresh,0,spOCRPublicPara.iSegGray2);
			if(0 == spOCRPublicPara.iRelation2)
            {
				Intersection(regDynThresh, regThresh, out regThresh2);
            }
            else
            {
				Union2(regDynThresh,regThresh,out regThresh2);

            }
			Union2(regThresh1, regThresh2,out regDynThresh);
			//合并
		
			//Var阈值

			VarThreshold(ImgReducedSrc,out regVar,iCharWidth,iCharHeight, dVarStdDevScale, iVarAbsGray, "dark");
			//交
			
			Intersection(regVar, regDynThresh, out regDynThresh);

			//write_region(regDynThresh,"m1.reg");
			//处理
			
			//processCharRegion(out regDynThresh);
			//write_region(regDynThresh,"m2.reg");
			//选取
		
			
			SelectShape(regDynThresh, out regChar, (new HTuple("width")).TupleConcat(
"height"), "and", (new HTuple(iCharWidth * 0.1)).TupleConcat(iCharHeight * 0.6), (new HTuple(9999)).TupleConcat(
9999));
			//write_region(*regChar,"m3.reg");
			
			CountObj(regChar,out lNum);
			

			if(lNum == 0)
            {
				sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
				CountObj(regDynThresh,out lNum);
                if (lNum > 0)
                {
					CopyObj(regDynThresh, out sRsu.regError, 1, -1);
                }
                else
                {
					CopyObj(regValid, out sRsu.regError, 1, -1);
                }
			}
			return sRsu;
		}
		public void segmentCharLine(HObject regChar, int iLineCount, out HObject regLine1, out HObject regLine2, out HObject regLine3)
        {
		
			
			
			


			HObject regUnion, regSel;
			HTuple lNum;
			int i;
			HTuple Row1, Column1, Row2, Column2;
			HTuple Area, Row, Column;
			//确实regLine1是输出变量 应该用out修饰
			GenEmptyObj(out regLine1);
			GenEmptyObj(out regLine2);
			GenEmptyObj(out regLine3);
			CountObj(regChar, out lNum);


			if (lNum > 0)
			{
			

				Union1(regChar,out regUnion);
				SmallestRectangle1(regUnion, out Row1, out Column1, out Row2, out Column2);

                switch (iLineCount)
                {
					case 1:
						SortRegion(regChar,out regLine1,"upper_left","true","column");
						break;
					case 2:
						for(i = 0;i< lNum; ++i)
                        {
							SelectObj(regChar, out regSel, i + 1);
							AreaCenter(regSel,out Area, out Row,out Column);
							if(Row < Row1+(Row2 - Row1) / 2)
                            {
								ConcatObj( regLine1,regSel, out regLine1);
                            }
                            else
                            {
								ConcatObj(regLine2, regSel, out regLine2);
                            }
                        }
						SortRegion(regLine1, out regLine1, "upper_left", "true", "column");
						SortRegion(regLine2, out regLine2, "upper_left", "true", "column");


						break;
					case 3:
						for(i = 0; i < lNum; ++i)
                        {
							SelectObj(regChar, out regSel, i + 1);
							AreaCenter(regSel, out Area, out Row, out Column);
							if (Row < Row1 + (Row2 - Row1) / 3)
							{
								//concat_obj(*regLine1, regSel, regLine1);
								ConcatObj(regLine1, regSel,out regLine1);
                            }
                            else
                            {
								if (Row < Row1 + (Row2 - Row1) * 2 / 3)
								{
									

									ConcatObj(regLine2, regSel,out  regLine2);
								}
								else
								{
									
									ConcatObj(regLine3, regSel, out regLine3);
								}
							}
						}


						SortRegion(regLine1, out regLine1, "upper_left", "true", "column");
						SortRegion(regLine2, out regLine2, "upper_left", "true", "column");
						SortRegion(regLine3, out regLine3, "upper_left", "true", "column");
						break;
					default:
					
						SortRegion(regChar,out regLine1, "upper_left", "true", "column");
						break;
				}

			}
		}


		public	s_CheckResult charCheckBase(HObject imageSrc,HObject regValid,ref s_pOCRPara spOCRPara,HObject regCharPos, HObject regChar)
        {

			
			
			

			s_CheckResult sRsu = new s_CheckResult();
			spOCRPara.spOCRErrorInfo.strErrorChar = "";
			GenEmptyRegion(out spOCRPara.spOCRErrorInfo.regErrorChar);
			int iPosMethod = spOCRPara.spOCRPublicPara.iPosMethod;
			int iLineCount = spOCRPara.spOCRPublicPara.iLineCount;
			


			HObject regLine1, regLine2, regLine3;
			HObject regInter, regSel;
			HTuple tpConst, tpArea, tpIndex, tpTemp;
			HTuple tpCharRsu, tpConfidence;
			HTuple lNum, lArea = 30;
			String strCharRsu;
			int i;

			GenEmptyObj(out regLine1);
			GenEmptyObj(out regLine2);
			GenEmptyObj(out regLine3);
			GenEmptyObj(out regInter);
			GenEmptyObj(out regSel);

            switch (iPosMethod)
			
			{

				case 0:
                    {
						sRsu = segmentChar(imageSrc, regValid, ref spOCRPara.spOCRPublicPara,out  regChar);
						if (sRsu.iType > 0)
						{
							return sRsu;
						}
						//分割字符位置
						segmentCharLine(regCharPos, iLineCount, out regLine1, out regLine2, out regLine3);
						if (iLineCount > 0)
						{
							//获取实时检测字符串
							spOCRPara.spOCRPara1.strCharGenerate = CMyGlobalFun::GetCharGenerate(spOCRPara.spOCRPara1.strCharRun);
							//ini文件要实现读写功能

							switch (spOCRPara.spOCRPara1.iCheckMethod)
							{
								case 0:
                                  
				

									Intersection(regLine1, regChar, out regInter);
									SelectShape(regInter, out regSel, "area", "or", 0, lArea);
									CountObj(regSel,out lNum);
									if (lNum > 0)
									{
										sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);

										 CopyObj(regChar,out sRsu.regError,1,-1);
										//第一行字符识别错误
										sRsu.strDescription = "The first line character recognition error:%1!";
										return sRsu;
									}
									//count_obj(regInter,&lNum);
									//area_center(regInter,&tpArea,NULL,NULL);
									//tuple_gen_const(lNum,lArea,&tpConst);
									//tuple_sub(tpArea,tpConst,&tpTemp);
									//tuple_sgn(tpTemp,&tpTemp);
									//tuple_find(tpTemp,-1,lNum);
									break;
								case 1:
									break;
								default:
									break;
							}
						}



						if (iLineCount > 1)
						{
							//获取实时检测字符串
							spOCRPara.spOCRPara2.strCharGenerate = CMyGlobalFun::GetCharGenerate(spOCRPara.spOCRPara2.strCharRun);
							switch (spOCRPara.spOCRPara2.iCheckMethod)
							{
								case 0:
					
									Intersection(regLine2, regChar,out regInter);
									SelectShape(regInter,out regSel,"area","or",0,lArea);
									CountObj(regSel,out lNum);

									if (lNum > 0)
									{
										sRsu.iType =  (int)(e_DefectType.ERROR_CHARMATCH);

										CopyObj(regChar,out sRsu.regError,1,-1);
										//第二行字符识别错误
										sRsu.strDescription = "The second line character recognition error:%1!";
										return sRsu;
									}
									break;
								case 1:
									break;
								default:
									break;
							}
						}

						if (iLineCount > 2)
						{
							//获取实时检测字符串
							spOCRPara.spOCRPara3.strCharGenerate = CMyGlobalFun::GetCharGenerate(spOCRPara.spOCRPara3.strCharRun);
							switch (spOCRPara.spOCRPara3.iCheckMethod)
							{
								case 0:

									Intersection(regLine3, regChar, out regInter);
									SelectShape(regInter, out regSel, "area", "or", 0, lArea);
									CountObj(regSel, out lNum);
									if (lNum > 0)
									{
										sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
										
										CopyObj(regChar,out sRsu.regError,1,-1);
										//第三行字符识别错误
										sRsu.strDescription = "The third line character recognition error:%1!";
										return sRsu;
									}
									break;
								case 1:
									break;
								default:
									break;
							}
						}




					}
					break;
				case 1:
                    {
						//是否使用

						sRsu = segmentChar(imageSrc, regValid,ref  spOCRPara.spOCRPublicPara,out regChar);
						if (sRsu.iType > 0)
						{
							return sRsu;
						}
						//分割字符位置
						//* HObject 表示把这个对象传进去，传进去的就是这个对象
						segmentCharLine(regChar, iLineCount, out regLine1, out regLine2, out regLine3);
						
						if (iLineCount > 0)
						{
							//获取实时检测字符串
							spOCRPara.spOCRPara1.strCharGenerate = CMyGlobalFun::GetCharGenerate(spOCRPara.spOCRPara1.strCharRun);
							switch (spOCRPara.spOCRPara1.iCheckMethod)
							{
								case 0://粗略，不识别
									
									CountObj(regLine1,out lNum);
									if (lNum < spOCRPara.spOCRPara1.strCharGenerate.Length)
									{
										sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
										
										CopyObj(regChar,out sRsu.regError, 1, -1);
										//第一行字符区域错误
										sRsu.strDescription = "The first line character region error:%1!";
										return sRsu;
									}
									break;
								case 1://识别
									{
										
										CountObj(regLine1,out lNum);
										if (lNum < spOCRPara.spOCRPara1.strCharGenerate.Length)
										{
											sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);

											CopyObj(regChar, out sRsu.regError, 1, -1);
											//第一行字符区域错误
											sRsu.strDescription = "The first line character region error:%1!";
											return sRsu;
										}
										strCharRsu = "";
										for (i = 0; i < spOCRPara.spOCRPara1.strCharRule.Length; ++i)
										{
											
											SelectObj(regLine1, out regSel, i + 1);
											char ch = spOCRPara.spOCRPara1.strCharRule[i];
											switch (ch)//这里是要注意，原代码是转成了acc码
											{
												case '0'://数字
													if (spOCRPara.spOCRPublicPara.hNumberModelID < 0)
													{
														sRsu.iType = ( int)(e_DefectType.ERROR_CHARMATCH);
										
														CopyObj(regSel, out sRsu.regError, 1, -1);
														//无效的字符模板
														sRsu.strDescription = "Invalid character template!";
														return sRsu;
													}
													

													DoOcrSingleClassMlp(regSel, imageSrc, spOCRPara.spOCRPublicPara.hNumberModelID,
														1, out tpCharRsu, out tpConfidence);
													if (tpCharRsu[0].S != spOCRPara.spOCRPara1.strCharGenerate[i].ToString() ||  //这里ToString是否正确
														tpConfidence[0].D < spOCRPara.spOCRPublicPara.fScore)
													{
														spOCRPara.spOCRErrorInfo.strErrorChar = tpCharRsu[0].S;
														spOCRPara.spOCRErrorInfo.iType = 0;
														
														CopyObj(regSel, out spOCRPara.spOCRErrorInfo.regErrorChar, 1, -1);
														strCharRsu = strCharRsu + "<" + *tpCharRsu[0].S() + ">";
														sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);;
														
														CopyObj(regSel, out sRsu.regError, 1, -1);
														//第一行字符识别错误
														sRsu.strDescription = "The first line character recognition error:"+ strCharRsu + tpConfidence[0].S;
														return sRsu;
													}
													strCharRsu = strCharRsu + spOCRPara.spOCRPara1.strCharGenerate.at(i);
													break;
												default:
													break;
											}
										}
									}
									break;
								default:
									break;
							}
						}
						if (iLineCount > 1)
						{
							//获取实时检测字符串
							spOCRPara.spOCRPara2.strCharGenerate = CMyGlobalFun::GetCharGenerate(spOCRPara.spOCRPara2.strCharRun);
							switch (spOCRPara.spOCRPara2.iCheckMethod)
							{
								case 0:
								
									CountObj(regLine2, out lNum);
									if (lNum < spOCRPara.spOCRPara2.strCharGenerate.Length)
									{
										sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);

										CopyObj(regChar, out sRsu.regError, 1, -1);
										//第二行字符识别错误
										sRsu.strDescription = "The second line character recognition error:%1!";
										return sRsu;
									}
									break;
								case 1:
									{
										
										CountObj(regLine2, out lNum);
										if (lNum < spOCRPara.spOCRPara2.strCharGenerate.Length)   //这里要注意将length()直接转为Length是否正确
										{
											sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
											
											CopyObj(regChar, out sRsu.regError, 1, -1);
											//第一行字符区域错误
											sRsu.strDescription = "The second line character region error:%1!";
											return sRsu;
										}
										strCharRsu = "";
										for (i = 0; i < spOCRPara.spOCRPara2.strCharRule.Length; ++i)
										{
										
											SelectObj(regLine2,out regSel,i+1);
											char ch = spOCRPara.spOCRPara2.strCharRule[i];
											switch (ch)
											{
												case '0'://数字
													if (spOCRPara.spOCRPublicPara.hNumberModelID < 0)
													{
														sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
														
														CopyObj(regSel, out sRsu.regError, 1, -1);
														//无效的字符模板
														sRsu.strDescription = "Invalid character template!";
														return sRsu;
													}

													DoOcrSingleClassMlp(regSel, imageSrc, spOCRPara.spOCRPublicPara.hNumberModelID,
														1, out tpCharRsu, out tpConfidence);
													if (tpCharRsu[0].S != spOCRPara.spOCRPara2.strCharGenerate[i].ToString() ||   //这里的toString是否正确需要考虑
														tpConfidence[0].D < spOCRPara.spOCRPublicPara.fScore)
													{
														//double和float都是表示小数，但是double
														spOCRPara.spOCRErrorInfo.strErrorChar = tpCharRsu[0].S;
														spOCRPara.spOCRErrorInfo.iType = 0;
														CopyObj(regSel, out spOCRPara.spOCRErrorInfo.regErrorChar, 1, -1);
														
														strCharRsu = strCharRsu + "<" + tpCharRsu[0].S + ">";
														sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
														
														CopyObj(regSel, out sRsu.regError, 1, -1);
														//第一行字符识别错误
														sRsu.strDescription = "The second line character recognition error:"  + strCharRsu + tpConfidence[0].D.ToString();
														return sRsu;
													}
													strCharRsu = strCharRsu + spOCRPara.spOCRPara2.strCharGenerate.at(i);
													break;
												default:
													break;
											}
										}
									}
									break;
								default:
									break;
							}
						}
						if (iLineCount > 2)
						{
							//获取实时检测字符串
							spOCRPara.spOCRPara3.strCharGenerate = CMyGlobalFun::GetCharGenerate(spOCRPara.spOCRPara3.strCharRun);
							switch (spOCRPara.spOCRPara3.iCheckMethod)
							{
								case 0:
									
									CountObj(regLine3,out lNum);
									if (lNum < spOCRPara.spOCRPara3.strCharGenerate.Length)
									{
										sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
										
										CopyObj(regChar, out sRsu.regError, 1, -1);
										//第三行字符识别错误
										sRsu.strDescription = "The third line character recognition error:%1!";
										return sRsu;
									}
									break;
								case 1:
									{
										
										CountObj(regLine3, out lNum);
										if (lNum < spOCRPara.spOCRPara3.strCharGenerate.Length)
										{
											sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);

											CopyObj(regChar, out sRsu.regError, 1, -1);
											//第一行字符区域错误
											sRsu.strDescription = "The third line character region error:%1!";
											return sRsu;
										}
										strCharRsu = "";
										for (i = 0; i < spOCRPara.spOCRPara1.strCharRule.Length; ++i)
										{
											
											SelectObj(regLine1, out regSel, i + 1);
											char ch = spOCRPara.spOCRPara1.strCharRule[i];
											switch (ch)
											{
												case '0'://数字
													if (spOCRPara.spOCRPublicPara.hNumberModelID < 0)
													{
														sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);


														CopyObj(regSel, out sRsu.regError, 1, -1);
														//无效的字符模板
														sRsu.strDescription = "Invalid character template!";
														return sRsu;
													}
											

													DoOcrSingleClassMlp(regSel, imageSrc, spOCRPara.spOCRPublicPara.hNumberModelID,
														1, out tpCharRsu, out tpConfidence);
													
												
													if (tpCharRsu[0].S != spOCRPara.spOCRPara3.strCharGenerate[i].ToString() ||
														tpConfidence[0].D < spOCRPara.spOCRPublicPara.fScore)
													{
														spOCRPara.spOCRErrorInfo.strErrorChar = tpCharRsu[0].S;
														spOCRPara.spOCRErrorInfo.iType = 0;
														
														CopyObj(regSel, out spOCRPara.spOCRErrorInfo.regErrorChar, 1, -1);
														strCharRsu = strCharRsu + "<" + tpCharRsu[0].S + ">";
														sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);

														CopyObj(regSel, out sRsu.regError, 1, -1);
														//第一行字符识别错误
														sRsu.strDescription = "The third line character recognition error:" + strCharRsu + tpConfidence[0].D.ToString();
														return sRsu;
													}
													strCharRsu = strCharRsu + spOCRPara.spOCRPara3.strCharGenerate.at(i);
													break;
												default:
													break;
											}
										}
									}
									break;
								default:
									break;
							}
						}
					}
					break;
				default:
					break;

			}
			return sRsu;

		}


		public s_CheckResult safeRingCheck0( HObject imageSrc, HObject  regValid,s_pCapsPara spCapsPara,bool bLocate,double dOriRow)
        {
			s_CheckResult sRsu = new s_CheckResult();
			HObject regCheck, regCaps;
			HObject imgReduce, imgMean;
			HObject regThresh, regDyn, regTemp, regLine, regThreshCap;
			HTuple nRow1, nRow2, nCol1, nCol2;
			HTuple lNum;
			double dValidRow;
			GenEmptyObj(out regCheck);
			GenEmptyObj(out regCaps);
			GenEmptyObj(out imgReduce);
			GenEmptyObj(out imgMean);
			GenEmptyObj(out regThresh);
			GenEmptyObj(out regDyn);
			GenEmptyObj(out regTemp);
			GenEmptyObj(out regLine);
			GenEmptyObj(out regThreshCap);
			nRow1 = new HTuple();
			nRow2 = new HTuple();
			nCol1 = new HTuple();
			nCol2 = new HTuple();

			nRow1.Dispose();
			nCol1.Dispose();
			nRow2.Dispose();
			nCol2.Dispose();




			//检测安全环: 0-区域中提取regCaps,1-传递进来regCaps
			if (spCapsPara.bSafeRing || spCapsPara.bCapsPos)
			{
				//修改定位方法后改动
				
				SmallestRectangle1(regValid, out nRow1, out nCol1, out nRow2, out nCol2);
				if (bLocate)
				{
					dValidRow = dOriRow;
				}
				else
				{
					dValidRow = nRow2;
				}
				//区域过小
				if (dValidRow - nRow1 < 3)
				{
					//区域预处理错误
					sRsu.iType =  (int)(e_DefectType.ERROR_INVALIDROI);
					
					CopyObj(regValid, out sRsu.regError, 1, -1);
					return sRsu;
				}
				

				GenRectangle1(out regCheck, nRow1, nCol1, dValidRow, nCol2);
				ReduceDomain(imageSrc, regCheck,  out imgReduce);
				switch (spCapsPara.iPolar)
				{
					case 0:
						
						Threshold(imgReduce, out regThresh, 0, spCapsPara.iCapsGray);
						break;
					case 1:
						

						Threshold(imgReduce, out regThresh, spCapsPara.iCapsGray, 255);
						break;
					default:
						
						Threshold(imgReduce, out regThresh, spCapsPara.iCapsGray, 255);
						break;
				}



				Connection(regThresh, out regTemp);

				SelectShape(regTemp, out regThreshCap, "area", "and", 100, 99999999);
				GenRegionLine(out regLine, dValidRow, nCol1, dValidRow, nCol2);
				Union2(regThreshCap, regLine, out regTemp);
				//2015.4.24 缩小纵向闭运算尺度dValidRow-nRow1 ----> (dValidRow-nRow1)/3
				ClosingRectangle1(regTemp,out regTemp,1, (dValidRow - nRow1) / 3);
				OpeningCircle(regTemp, out regTemp,1);
				Connection(regTemp, out regTemp);
				SelectShapeStd(regTemp, out regCaps, "max_area", 70);
				if (spCapsPara.bSafeRing)
				{
					

					ErosionCircle(regCaps, out regTemp, 5.5);
					MeanImage(imageSrc, out imgMean, 51, 31);
					ReduceDomain(imageSrc, regTemp, out imgReduce);
					ReduceDomain(imgMean, regTemp,  out imgMean);
				


					switch (spCapsPara.iPolar)
					{
						case 0:
							

							DynThreshold(imgReduce, imgMean, out regThresh, spCapsPara.iSafeRingEdge, "light");
							Threshold(imgReduce, out regDyn, spCapsPara.iSafeRingGray, 255);
							break;
						case 1:
							


							DynThreshold(imgReduce, imgMean, out regThresh, spCapsPara.iSafeRingEdge, "light");
							Threshold(imgReduce, out regDyn, 0, spCapsPara.iSafeRingGray);
							break;
						default:
						

							DynThreshold(imgReduce, imgMean, out regThresh, spCapsPara.iSafeRingEdge, "light");
							Threshold(imgReduce, out regDyn, spCapsPara.iSafeRingGray, 255);
							break;
					}

                    if (spCapsPara.bSafeRing)
                    {
	

						ErosionCircle(regCaps, out regTemp, 5.5);
						MeanImage(imageSrc,out imgMean,51,31);
						ReduceDomain(imageSrc, regTemp, out imgReduce);
						ReduceDomain(imgMean, regTemp, out imgMean);
					}
				
					Union2(regThresh, regDyn, out regThresh);
					if (spCapsPara.bHorClosing)
					{
					

						ClosingRectangle1(regThreshCap, out regThreshCap, nCol2 - nCol1, 1);
						Difference(regThresh, regThreshCap, out regThresh);


					}
				
				


					OpeningCircle(regThresh,out regTemp,1.5);
					ClosingCircle(regTemp, out regTemp, 2.5);
					Connection(regTemp, out regTemp);
					SelectShape(regTemp, out regTemp, "width", "and", spCapsPara.iSafeRingWidth, 99999);
					SelectShape(regTemp, out regTemp, "height", "and", spCapsPara.iSafeRingHeight, 99999);
					SelectShape(regTemp, out regTemp, "area", "and", spCapsPara.iSafeRingArea, 999999999);

					CountObj(regTemp, out lNum);

					if (lNum > 0)
					{
						//安全环错误
						sRsu.iType =  (int)(e_DefectType.ERROR_SAFERING);



						CopyObj(regTemp,out sRsu.regError, 1, -1);


						return sRsu;
					}
				}
			}
			//正确将瓶盖区域放在错误区域中传递出去,检测高度盖，角度
			
			CopyObj(regCaps, out sRsu.regError, 1, -1);
			return sRsu;

		}

		//*功能：检测瓶盖
		public s_CheckResult capsCheckBase( HObject imageSrc, HObject  oregCaps,
ref	s_pCapsPara spCapsPara,HObject regCapsLine,bool bLocate,int iLocMethod,double dOriRow)
        {


		}


		public s_CheckResult capsCheck(HObject hImg, HObject regCaps,  ref s_pCapsPara spCapsPara, HObject regCapsLine)
        {
			s_CheckResult sRsu = new s_CheckResult();
			if (spCapsPara.bCapsPos)
			{
		

				HObject regValid, regTemp;
				HObject Contours, ContourSelect;
				HTuple tRow1, tCol1, tRow2, tCol2, tArea, tLength;
				int iOpenScale = 30;//开掉水珠影响，无干扰可放小
				HObject image, roi, RegionLines, skeleton;
				HObject hROIRegion, ImageReduced, Regions, ConnectedRegions, SelectedRegions, RegionFillUp;
				HObject ImageReducedSafeCircle, ImageMean, tempRegions, SafeCircleRegion, dynRegion, threRegion;
				GenEmptyObj(out regValid);
				GenEmptyObj(out regTemp);
				GenEmptyObj(out Contours);
				GenEmptyObj(out ContourSelect);
				GenEmptyObj(out image);
				GenEmptyObj(out roi);
				GenEmptyObj(out RegionLines);
				GenEmptyObj(out skeleton);
				GenEmptyObj(out hROIRegion);
				GenEmptyObj(out ImageReduced);
				GenEmptyObj(out Regions);
				GenEmptyObj(out ConnectedRegions);


				GenEmptyObj(out SelectedRegions);
				GenEmptyObj(out RegionFillUp);
				GenEmptyObj(out ImageReducedSafeCircle);
				GenEmptyObj(out ImageMean);
				GenEmptyObj(out tempRegions);
				GenEmptyObj(out SafeCircleRegion);
				GenEmptyObj(out dynRegion);
				GenEmptyObj(out threRegion);
				
				HTuple tRow,tCol;
				HTuple Number;
				HTuple tPhi, tMean;
				int nThreshold;
				int iHighCaps;  //高盖尺寸
				float fCapsPhi; //角度
				HTuple rowCenter, colCenter;
				rowCenter = new HTuple();
				colCenter = new HTuple();




				AreaCenter(regCaps, out tArea,out  rowCenter, out colCenter);
				if (tArea < spCapsPara.iPickCapsArea)
				{
					sRsu.iType = (int)(e_DefectType.ERROR_NOCAPS);

					CopyObj(regCaps,out sRsu.regError,1,-1);
					//未找到正确的瓶盖线，瓶盖提取高度可能设置过小！
					sRsu.strDescription = "No Caps,Area:%1!";
					return sRsu;
				}


				OpeningRectangle1(regCaps, out regValid, iOpenScale, 1);
				SmallestRectangle1(regValid, out tRow1, out tCol1, out tRow2, out tCol2);
				GenRectangle1(out regTemp, tRow1, tCol1, tRow1 + spCapsPara.iPickCapsLineHeight, tCol2);
				Intersection(regValid, regTemp, out regTemp);
				AreaCenter(regTemp,out tArea,out tRow,out tCol1);

			
				if (tArea < 50)
				{
					sRsu.iType =  (int)(e_DefectType.ERROR_TILTCAPS);

					CopyObj(regTemp, out sRsu.regError, 1, -1);
					//未找到正确的瓶盖线，瓶盖提取高度可能设置过小！
					sRsu.strDescription = "Not found the correct caps line,PickCapsLineHeight may be too small!";
					return sRsu;
				}
		

				Boundary(regTemp, out SelectedRegions, "inner");

				GenRegionLine(out Regions, tRow1 + spCapsPara.iPickCapsLineHeight, tCol1, tRow1 + spCapsPara.iPickCapsLineHeight, tCol2);
				Difference(SelectedRegions, Regions, out SelectedRegions);
				SelectShape(SelectedRegions, out SelectedRegions, "area", "and", 2, 999999);


				

				//»ñÈ¡ÂÖÀª
				
				
				Skeleton(SelectedRegions, out skeleton);
				SmallestRectangle1(skeleton, out tRow1, out tCol1, out tRow2, out tCol2);
				int CapWidth = tCol2[0].I - tCol1[0].I;
				int CapCenterColumn = (tCol2[0].I + tCol1[0].I) / 2;


				GenContoursSkeletonXld(skeleton, out Contours, 1, "filter");
				SegmentContoursXld(Contours, out Contours, "lines", 3, 3, 2);
				SelectContoursXld(Contours, out Contours, "contour_length", 30, 20000, -0.5, 0.5);
				SelectShapeXld(Contours, out Contours, "rect2_phi", "and", -0.8, 0.8);
				LengthXld(Contours, out tLength);
				CountObj(Contours, out Number);

				if (Number == 0)
				{
					sRsu.iType =  (int)(e_DefectType.ERROR_TILTCAPS);


					CountObj(SelectedRegions, out Number);

					if (Number > 0)
					{
						
						CopyObj(SelectedRegions, out sRsu.regError, 1, -1);
					}
					else
					{
						
						CopyObj(regCaps,out sRsu.regError,1,-1);
					}
					//未找到正确的瓶盖轮廓线！

					sRsu.strDescription = "Not found the correct contours of caps!";
					return sRsu;
				}
				HTuple drow, dphi, dcol,lenght1,lenght2;
				for (int i = 0; i < Number; i++)
				{
					
				

					SelectObj(Contours, out SelectedRegions, i + 1);
					SmallestRectangle2Xld(SelectedRegions, out drow, out dcol, out dphi, out lenght1, out lenght2);
					if (drow < tRow)
					{
						if (tLength[i].D > CapWidth / 7 || (dphi > -0.2 && dphi < 0.2))
						

						ConcatObj(RegionLines, SelectedRegions, out RegionLines);
					}
				}

				
				CountObj(RegionLines, out Number);
				if (Number == 0)
				{
					sRsu.iType = (int)(e_DefectType.ERROR_TILTCAPS); 
					
					GenRegionContourXld(Contours, out sRsu.regError, "margin");
					//未找到正确的瓶盖轮廓线！
					sRsu.strDescription = "Not found the correct contours of caps!";
					return sRsu;
				}

				//½«¸÷ÖÖÆ¿¸ÇÏßºÏ²¢
				
				UnionAdjacentContoursXld(RegionLines, out RegionLines, 150, 1, "attr_keep");
				//ÕÒµ½×î³¤µÄÏß
				HObject circle;
				HTuple tMax, tIndices;
				HTuple centerRow, centerCol, dis;
				
				LengthXld(RegionLines, out tLength);
				
				TupleMax(tLength, out tMax);
				if (tMax < CapWidth / 3)
				{
					sRsu.iType = (int)(e_DefectType.ERROR_TILTCAPS);					//注意这个类型对不对

					GenRegionContourXld(Contours, out sRsu.regError, "margin");
					//未找到正确的瓶盖轮廓线,轮廓线可能过短！
					sRsu.strDescription = "Not found the correct contours of caps,the width of contour may be too small!";
					return sRsu;
				}
				
				TupleFind(tLength, tMax, out tIndices);
				if (tIndices == -1)
				{
					sRsu.iType = (int)(e_DefectType.ERROR_TILTCAPS);

					GenRegionContourXld(Contours, out sRsu.regError, "margin");
					//未找到正确的瓶盖轮廓线,轮廓线可能过短！
					sRsu.strDescription = "Not found the correct contours of caps,the width of contour may be too small!";
					return sRsu;
				}
			

				SelectObj(RegionLines, out RegionLines, tIndices + 1);
				SmallestRectangle2Xld(RegionLines, out drow, out dcol, out tPhi, out lenght1, out lenght2);
				TupleDeg(tPhi, out tPhi);
				fCapsPhi = Abs(tPhi);				//注意这里求绝对值是否正确
				if (fCapsPhi < spCapsPara.fCapsPhiMin || fCapsPhi > spCapsPara.fCapsPhiMax)
				{
					sRsu.iType = (int)(e_DefectType.ERROR_TILTCAPS);


					GenRegionContourXld(RegionLines, out sRsu.regError, "margin");
					//瓶盖角度错误
					sRsu.strDescription = "Angle of caps is error:%1";
					return sRsu;
				}
				sRsu.strResultInfo += QObject::tr("Angle:%1 ").arg(fCapsPhi);
				//添加并显示瓶盖线
				//if (bDebug)
				//{
				//	gen_region_contour_xld(RegionLines,&regTemp,"margin");
				//	concat_obj(regTemp,regCapsLine,&regCapsLine);
				//}
				
				GenRegionContourXld(RegionLines, out regCapsLine, "margin");
				HTuple lWidth, lHeight;
				
				GetImageSize(hImg, out lWidth, out lHeight);
				//2015.4.24 高盖取最上边的点，且与原点比较
				//area_center_xld(RegionLines,NULL,&centerRow,&centerCol,NULL);
				//iHighCaps = lHeight - centerRow;
				//iHighCaps = m_oFullCaps.sCurrentOri.dOriCol - drow;
				iHighCaps = lHeight - drow;
				if (iHighCaps < spCapsPara.iHighCapsMin || iHighCaps > spCapsPara.iHighCapsMax)
				{
					//高低盖
					sRsu.iType = (int)(e_DefectType.ERROR_HIGHCAPS);
					//gen_circle(&sRsu.regError,centerRow,centerCol,5);


					GenRegionContourXld(RegionLines, out sRsu.regError, "margin");
					//瓶盖高度错误
					sRsu.strDescription = "Height of caps is error:%1";
					return sRsu;
				}
				sRsu.strResultInfo += QObject::tr("Height:%1").arg(iHighCaps);
				////添加并显示瓶盖点
				//if (bDebug)
				//{
				//	gen_circle(&regTemp,centerRow,centerCol,5);
				//	concat_obj(regTemp,m_mFullCaps.regCheck,&m_mFullCaps.regCheck);
				//}
			}
		}

	}
}
