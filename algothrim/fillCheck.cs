using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
//using HalconDotNet.HOperatorSet;
using static HalconDotNet.HOperatorSet;
using bottleWithHALCON.innerData;
namespace bottleWithHALCON.algothrim.fillCheck
{
 public struct s_My_Ori
    {
        float Row;
        float Col;
        float Angle;
    }

    public struct s_pCreatModel {

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
		public float fPhiStart;    //**起始角度（弧度，范围-3.14到3.14）
		public float fPhiExtent;   //**角度幅度（弧度，范围0到6.28）
		public float fScore;       //***匹配度，该值越小匹配越耗时，范围[0，1]，推荐值（0.3-0.7）
		public int nNumMatchs; //匹配个数，0为所有匹配，指定个数，则选择匹配度最高的
		public float fMaxOverlap;  //允许的最大重叠面积，范围[0，1]，无格挡可设为0
								   //**亚像素精度影响检测速度和匹配结果（*角度*），若精度要求不高，可使用'interpolation'，综合精度和速度可使用'least_squares'
		public String strSubPixel;//**亚像素精度。
		public float fGreediness;  //****用于定位加速，该值越大速度越快，但可能导致无法找到匹配目标，推荐（0.7-0.9）
	}


	public struct s_pPMToolPara
    {
		s_pCreatModel spCreatModel;
		s_pFindModel spFindModel;
	}

	public struct s_pOCRPublicPara
    {
		HTuple lNumberModelID;       //数字字模库
		HTuple lLowerCaseModelID;    //小写字母字模库
		HTuple lUpperCaseModelID;    //大写字母字模库

		int iLineCount;     //行数
		int iRelation1;     //与|或
		int iSegEdge1;      //对比度1
		int iSegGray1;      //灰度1
		int iRelation2;     //与|或
		int iSegGray2;      //对比度2
		int iSegEdge2;      //灰度2
		float fVarStdDevScale;//Var阈值参数
		int iVarAbsGray;

		int iCharSpace;     //字符间隔
		int iCharWidth;     //字符宽度
		int iCharHeight;    //字符高度
		float fScore;       //匹配度

		int iPosMethod;     //查找字符位置方法0、记忆1、自动
	}


	public struct s_pOCRLinePara
    {
		String strErrorChar;   //错误字符，1个，检测到错误即退出
		HObject regErrorChar;   //错误区域，1个，检测到错误即退出
		int iType;              //错误字符类型
	}
	public struct s_pOCRErrorInfo
	{
		String strErrorChar;   //错误字符，1个，检测到错误即退出
		HObject regErrorChar;   //错误区域，1个，检测到错误即退出
		int iType;              //错误字符类型
	}
	

	
	

	





	public struct s_pOCRPara
	{
		s_pOCRPublicPara spOCRPublicPara;
		s_pOCRErrorInfo spOCRErrorInfo;
		s_pOCRLinePara spOCRPara1;
		s_pOCRLinePara spOCRPara2;
		s_pOCRLinePara spOCRPara3;
	}
	

	public struct s_pCapsPara
	{
		//找瓶盖和安全环
		int iPolar;             //极性 0：黑色盖子，白色安全环缺陷，1：白色盖子,黑色安全环缺陷
		int iCapsGray;          //灰度值
		float fClosingScale;    //闭合比例系数，合并断开的安全环和瓶盖，纵向闭合，尺度为fClosingScale×高度，标定时，闭合尺度默认为20（小）

		bool bSafeRing;         //安全环
		bool bHorClosing;       //水平闭合，不检测安全环中间缺陷
		int iSafeRingEdge;
		int iSafeRingGray;
		int iSafeRingWidth;     //安全环缺口宽度
		int iSafeRingHeight;    //安全环缺口高度
		int iSafeRingArea;      //安全环面积

		bool bCapsPos;          //瓶盖高低、角度、大小
		int iHighCapsMin;       //高盖
		int iHighCapsMax;
		int iPickCapsLineHeight;//提取瓶盖角度线，所切割的高度
		int iPickCapsArea;      //面积
		float fCapsPhiMin;      //角度
		float fCapsPhiMax;
	}

	//预处理区域参数
	public struct s_pROIPara
	{
		float fRoiRatio;
		int nClosingWH;
		int nGapWH;

		
	}

	//线定位参数
	public struct s_LineLocPara
	{
		int iRow1;
		int iCol1;
		int iRow2;
		int iCol2;
		int iDistance;
		
	}

	public struct s_LineLocParaEx
	{
		s_LineLocPara sLineOuter;
		s_LineLocPara sLineInner;
	}
	
	
	//原点
	public struct s_MyOri
    {
		double dOriRow;
		double dOriCol;
		double dOriPhi;
	}

	public partial class CCheck
	{
		public CCheck()
		{

		}
		~CCheck()
		{

		}
		//引用参数就是；读取时和传值参数一样，写入时会改变原变量
		public void makeModel(HObject srcImg, HObject regModel, s_pCreatModel sModel, out HTuple lLocModelID, int iMethod = 0)   //c++中的const参数和引用搭配使用时表示引用参数内容不可被修改
		{

			HObject reducedImg;
			HObject xldModel;


			ReduceDomain(srcImg, regModel,out reducedImg);
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
		public int findEdgePointSingleEx(HObject Image, HObject LineSeg,
		out HTuple RowPt, out HTuple ColPt, int nThresh, int nLength, int nEdge = 10, int nDirect = L2R, int nType = 0,
		bool bMean = false)
		{
			int iCount = findEdgePointSingle();
            if (iCount < 2)
            {
				return iCount;
            }
			HObject imgDomain, imgSobel, regThresh;
			HObject regRect;
			int nWidthEx = 50, nHeightEx = 10;
			int nTempRow, nTempCol;
			long numTemp;
			int i;



		}

		//*功能：获取一个边缘点，返回边缘点的个数，找到点后判断污点干扰；nThresh边缘提取阈值，nLength边缘长度
		//返回边缘以外点的个数，最后一个点是边缘点(亮到暗或暗到亮的分界点)
		public int findEdgePointSingle( HObject Image, HObject LineSeg, 
		out HTuple RowPt,out HTuple ColPt, int nEdge = 10, int nDirect = (int)e_LineDirect.L2R, int nType = 0,
		bool bMean = false,bool bAllPoint = false)
        {
			RowPt = new HTuple();
			ColPt = new HTuple();


			HObject ImageReduce, ImageGauss, NewLine;
			HTuple Row, Col, GrayValue;
			HTuple PI;		//PI到底应该定义为多少?
			int nGrayDiff1, nGrayDiff2, nGrayDiff3, nCount, nLength, diff;
			HTuple nImgWidth, nImgHeight;
			HTuple dPhi;
			Row = new HTuple();
			Col = new HTuple();
			GrayValue = new HTuple();
			dPhi = new HTuple();
			PI = new HTuple();
			PI.Dispose();
			Row.Dispose();
			Col.Dispose();
			GrayValue.Dispose();
			dPhi.Dispose();
			// 边界点的数目
			nCount = 0;
			PI = 180;
			GetRegionPoints(LineSeg,out Row,out Col);
			nLength = Col.Length;
			if (nLength < 4)
			{
				return nCount;
			}
			diff = Col[0] - Col[nLength - 1];
			

			OrientationRegion(LineSeg,out dPhi);
			dPhi = dPhi.TupleFabs();				//求绝对值
			switch (nDirect)
			{
				case (int)e_LineDirect.L2R:
					if (diff > 0)
					{
						//Row = Row.Inverse();

						//	Col = Col.Sort();
						Row = Row.TupleInverse();
						Col = Col.TupleSort();
					}
					break;
				case (int)e_LineDirect.R2L:
					if (diff < 0)
					{
						//Row = Row.Inverse();
					//	Col = Col.Inverse();

						Row = Row.TupleInverse();
						Col = Col.TupleInverse();
					}
					else if (diff > 0 && dPhi < PI / 4)
					{
						//Col = Col.Sort();
						
						//Col = Col.Inverse();
						Col = Col.TupleSort();
						Col = Col.TupleInverse();
					}
					break;
				case (int)e_LineDirect.T2B:
					if (diff > 0 && dPhi < PI / 4)
					{
						//Col = Col.Sort();
						//Col = Col.Inverse();
						Col = Col.TupleSort();
						Col = Col.TupleInverse();
					}
					break;
				case (int)e_LineDirect.B2T:
					if (diff < 0)
					{
						//Row = Row.Inverse();
						//Col = Col.Inverse();

						Row = Row.TupleInverse();
						Col = Col.TupleInverse();
					}
					else
					{
						//Row = Row.Inverse();
						//Col = Col.Sort();

						Row = Row.TupleInverse();
						Col = Col.TupleSort();
					}
					break;
				default:
					break;
			}

			//Hobject ImgMedian, ImgDomain, RegInter;
			//long nNum;
			//get_domain(Image, &ImgDomain);
			//intersection(LineSeg, ImgDomain, &RegInter);
			///connection(RegInter, &RegInter);
			//select_shape(RegInter, &RegInter, "area", "and", 1, 99999999);
			//count_obj(RegInter, &nNum);

			HObject ImgMedian = null, ImgDomain = null, RegInter = null;
			HTuple nNum;
			nNum = new HTuple();
			nNum.Dispose();
			GetDomain(Image,out ImgMedian);
			Intersection(LineSeg, ImgDomain,out RegInter);
			Connection(RegInter,out RegInter);
			SelectShape(RegInter,out RegInter,"area","and",1, 99999999);
			CountObj(RegInter,out nNum);

			if (nNum == 0)
			{
				return nCount;
			}
		//	reduce_domain(Image, LineSeg, &ImgMedian);
			if (bMean)						
			{
				//Hobject regDilation, imgMean;
				//dilation_circle(LineSeg, &regDilation, 3.5);
				//reduce_domain(Image, regDilation, &imgMean);
				//mean_image(imgMean, &imgMean, 3, 3);//应力检测需要平滑
				//reduce_domain(imgMean, LineSeg, &ImgMedian);

				HObject regDilation = null, imgMean = null;
				DilationCircle(LineSeg, out regDilation, 3.5);
				ReduceDomain(Image, regDilation, out imgMean);
				MeanImage(imgMean, out imgMean, 3, 3);
				

			}
			//median_image(ImgMedian, &ImgMedian, "circle", 5, "continued");

			MedianImage(ImgMedian, out ImgMedian, "circle", 5, "continued");

			ReduceDomain(Image, LineSeg,out ImgMedian);

			HTuple pointer,type;
			pointer = new HTuple();
			type = new HTuple();
			pointer.Dispose();
			type.Dispose();
			int i, x, y;
			//第二个输出参数 poiter包含什么信息?pointer是一个指向图像数据块的指针，每个字节是8位因为需要明确c#中如何访问指针型数据
			
			GetImagePointer1(ImgMedian,out pointer,out type, out nImgWidth, out nImgHeight);
			//pointer
			//byte* ptr = (byte *)pointer[0].L;		
			//需要将明确byte[]首地址是什么格式
			//计算机是一个内存对应一个字节地址，64位计算机一个地址的长度是64位
			//指针总是指向变量的第一个字节
			//因此这里需要指向图像数据字节数组第一个元素的指针，因此需要byte*还是long *?不论什么类型指针，在内存中都是64位，只是不同类型指针加1时所加的字节数不同
			//每一个内存单元都有一个地址
			//内存单元 = 8b一个int型数据由4个内存单元组成，一共4B 32b，理论上这四个内存单元每一个都有一个地址，但是用指针指向时只有储存第一个存储单元的地址即可。任何类型的指针都是指向该类型变量的首地址，由于指针类型已确定，因此编译器能够确定数据所占字节数，从而编译时会安排相应的指令访问该地址中的数据。所以指针 + 1，代表其储存的地址直接略过连续的几个存储单元（int类型指针则直接把连续的4个字节看成整体，指针加一则一次性直接跳过4个），变为下一个相同类型数据的首储存单元地址。


			for (i = 0; i < nLength; ++i)
			{
				x = Col[i].I;
				y = Row[i].I;
				if (x < nImgWidth && y < nImgHeight)
				{
					GrayValue[i] = ptr[y * nImgWidth + x];//获取这条区域上的所有灰度值
				}
				else
				{
					GrayValue[i] = 0;
				}
			}
			//尽量访问指针提高执行速度
			if (nType == 0)//灰度由高到低
			{//从亮到暗
				//获取单个边缘点
				for (i = 0; i < nLength - 5; ++i)
				{
					nGrayDiff1 = GrayValue[i] - GrayValue[i + 3];
					nGrayDiff2 = GrayValue[i + 1] - GrayValue[i + 4];
					nGrayDiff3 = GrayValue[i + 2] - GrayValue[i + 5];
					if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)    //如果一直在暗区内部(有一个小于边缘说明出来区域)     //找到亮点
					{
						//不进入这个if说明进入了暗区
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
				for (i = 0; i < nLength - 5; ++i)//从暗到亮
				{
					nGrayDiff1 = GrayValue[i + 3] - GrayValue[i];
					nGrayDiff2 = GrayValue[i + 4] - GrayValue[i + 1];
					nGrayDiff3 = GrayValue[i + 5] - GrayValue[i + 2];
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
		//*功能：获取两个边缘点，返回边缘点的个数
		public unsafe  int findEdgePointDouble( HObject Image,  HObject LineSeg, 
		out HTuple RowPt,out HTuple ColPt, int nEdge = 10, int nDirect = (int)e_LineDirect.L2R, int nType = 0, bool bMean = false)
        {
			RowPt = new HTuple();
			ColPt = new HTuple();
			RowPt.Dispose();
			ColPt.Dispose();



			HObject ImageReduce, ImageGauss, NewLine;
			HTuple Row, Col, GrayValue;
			HTuple nGrayDiff1, nGrayDiff2, nGrayDiff3, nCount, nLength, diff;
			HTuple nImgWidth, nImgHeight;
			HTuple dPhi;
			Row = new HTuple();
			Col = new HTuple();
			nImgHeight = new HTuple();
			nImgWidth = new HTuple();
			GrayValue = new HTuple();
			nImgHeight.Dispose();
			nImgWidth.Dispose();
			Row.Dispose();
			Col.Dispose();
			GrayValue.Dispose();
			// 边界点的数目
			nCount = 0;

			
			GetRegionPoints(LineSeg,out Row,out Col);


			nLength = Col.Length;
			if (nLength < 4)
			{
				return nCount;
			}
			diff = Col[0] - Col[nLength - 1];
			OrientationRegion(LineSeg, out dPhi);
			dPhi.TupleFabs();

			switch (nDirect)
			{
				case (int)e_LineDirect.L2R:
					if (diff > 0)
					{
						

						Row = Row.TupleInverse();
						Col = Col.TupleSort();
					}
					break;
				case (int)e_LineDirect.R2L:
					if (diff < 0)
					{
						

						Row = Row.TupleInverse();
						Col = Col.TupleInverse();
					}
					else if (diff > 0 && dPhi < PI / 4)
					{
						

						Col = Col.TupleSort();
						Col = Col.TupleInverse();
					}
					break;
				case (int)e_LineDirect.T2B:
					if (diff > 0 && dPhi < PI / 4)
					{
				

						Col = Col.TupleSort();
						Col = Col.TupleInverse();
							
					}
					break;
				case (int)e_LineDirect.B2T:
					if (diff < 0)
					{
						//Row = Row.Inverse();
						//Col = Col.Inverse();

						Row = Row.TupleInverse();
						Col = Col.TupleInverse();
					}
					else
					{
						//Row = Row.Inverse();
						//Col = Col.Sort();

						Row = Row.TupleInverse();
						Col = Col.TupleInverse();
					}
					break;
				default:
					break;
			}

			HObject ImgMedian, ImgDomain, RegInter;

			HTuple nNum;

			HTuple imageType;
			//get_domain(Image, &ImgDomain);
			//intersection(LineSeg, ImgDomain, &RegInter);
			//	connection(RegInter, &RegInter);
			///	select_shape(RegInter, &RegInter, "area", "and", 1, 99999999);
			//count_obj(RegInter, &nNum);
			nNum = new HTuple();
			imageType = new HTuple();

			nNum.Dispose();

			GetDomain(Image,out ImgDomain);
			Intersection(LineSeg, ImgDomain,out RegInter);
			Connection(RegInter,out RegInter);
			SelectShape(RegInter,out RegInter,"area","and",1,99999999);
			CountObj(RegInter,out nNum);

			if (nNum == 0)
			{
				return nCount;
			}
			//reduce_domain(Image, LineSeg, &ImgMedian);
			ReduceDomain(Image, LineSeg, out ImgMedian);
			if (bMean)			//如果需要中值滤波
			{
				//Hobject regDilation, imgMean;
				//dilation_circle(LineSeg, &regDilation, 3.5);
				//reduce_domain(Image, regDilation, &imgMean);
				//mean_image(imgMean, &imgMean, 3, 3);//应力检测需要平滑
				//reduce_domain(imgMean, LineSeg, &ImgMedian);

				HObject regDilation, imgMean;
				DilationCircle(LineSeg,out regDilation,3.5);
				ReduceDomain(Image, regDilation, out imgMean);
				MeanImage(imgMean,out imgMean ,3,3);
				ReduceDomain(imgMean,LineSeg,out imgMean);

				


			}
			//median_image(ImgMedian, &ImgMedian, "circle", 5, "continued");
			MedianImage(ImgMedian,out ImgMedian,"circle",5,"continued");
			
			if(nNum.I == 0)
            {
				return nCount;
            }
			ReduceDomain(Image,LineSeg,out ImgMedian);
            if (bMean)
            {
				HObject regDilation, imgMean;
            }

			//unsigned char* ptr;
			HTuple ptr;
			
			ptr = new HTuple();
			ptr.Dispose();
			int i, j, x, y;
			byte*  pBufer;				//指向字节数据的指针
			//get_image_pointer1(ImgMedian, (long*)&ptr, NULL, &nImgWidth, &nImgHeight);
			GetImagePointer1(ImgMedian,out ptr,out imageType,out nImgWidth,out nImgHeight);
			//先用GetImagePointer算子得到指向图像数据块的指针,存储在HTuple中
			//将HTuple转为long值
			//将整型转为byte*指针型变量

			//使用指针访问图像字节数组
			pBufer = (byte*)ptr.I;
			for (i = 0; i < nLength; ++i)
			{
				x = Col[i].I;
				y = Row[i].I;
				if (x < nImgWidth && y < nImgHeight)
				{
					GrayValue[i] = pBufer[y * nImgWidth + x];
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
					nGrayDiff1 = GrayValue[i] - GrayValue[i + 3];
					nGrayDiff2 = GrayValue[i + 1] - GrayValue[i + 4];
					nGrayDiff3 = GrayValue[i + 2] - GrayValue[i + 5];
					if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)
					{
						RowPt[nCount] = (HTuple)(Row[i + 3]);
						ColPt[nCount] = (HTuple)(Col[i + 3]);
						++nCount;
						break;
					}
				}

				if (nCount > 0)
				{
					for (j = nLength - 1; j > i + 8; --j)
					{
						nGrayDiff1 = GrayValue[j] - GrayValue[j - 3];
						nGrayDiff2 = GrayValue[j - 1] - GrayValue[j - 4];
						nGrayDiff3 = GrayValue[j - 2] - GrayValue[j - 5];
						if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)
						{
							RowPt[nCount] = (HTuple)(Row[j - 3]);
							ColPt[nCount] = (HTuple)(Col[j - 3]);

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
					nGrayDiff1 = GrayValue[i + 3] - GrayValue[i];
					nGrayDiff2 = GrayValue[i + 4] - GrayValue[i + 1];
					nGrayDiff3 = GrayValue[i + 5] - GrayValue[i + 2];
					if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)
					{
						RowPt[nCount] = (HTuple)(Row[i + 3]);
						ColPt[nCount] = (HTuple)(Col[i + 3]);
						++nCount;
						break;
					}
				}

				if (nCount > 0)
				{
					for (j = nLength - 1; j > i + 8; --j)
					{
						nGrayDiff1 = GrayValue[j - 3] - GrayValue[j];
						nGrayDiff2 = GrayValue[j - 4] - GrayValue[j - 1];
						nGrayDiff3 = GrayValue[j - 5] - GrayValue[j - 2];
						if (nGrayDiff1 > nEdge && nGrayDiff2 > nEdge && nGrayDiff3 > nEdge)
						{
							RowPt[nCount] = (HTuple)(Row[j - 3]);
							ColPt[nCount] = (HTuple)(Col[j - 3]);

							++nCount;
							break;
						}
					}
				}
			}
			return nCount;

		}
		//*功能：基于原点模板的数据判断新原点是否准确，主要防止连瓶、光源脏等
		public int testEdgePointDouble( HObject Image,  HObject LineSeg, 
			HTuple RowPt, HTuple ColPt, int nEdge, float fRange,
		float oldWidth,int nType = 0, bool bMean = false)
        {


			HTuple newWidth;
			HTuple range = (HTuple)fRange;//宽度浮动范围比例	
			HTuple newPtCol, newPtRow;
			HTuple tempColL, tempColR;
			HTuple row1, row2, numTemp;
			HTuple col1, col2;
			HObject tempLineL, tempLineR, concatLines, newLine;
			HTuple rPtTemp, cPtTemp;

			newWidth = new HTuple();
			range = new HTuple();
			newPtCol = new HTuple();
			newPtRow = new HTuple();
			tempColL = new HTuple();
			tempColR = new HTuple();
			row1 = new HTuple();
			row2 = new HTuple();
			col1 = new HTuple();
			col2 = new HTuple();
			GenEmptyObj(out tempLineL);
			GenEmptyObj(out tempLineR);
			

			rPtTemp = new HTuple();
			cPtTemp = new HTuple();

			newWidth.Dispose();
			range.Dispose();
			newPtRow.Dispose();
			tempColL.Dispose();
			tempColR.Dispose();
			row1.Dispose();
			row2.Dispose();
			col1.Dispose();
			col2.Dispose();
			tempLineL.Dispose();
			tempLineR.Dispose();



			newWidth = ColPt[1].I - ColPt[0].I;//C
			
			if (fabs(newWidth - oldWidth) / (oldWidth + 0.0001) < range)
				return 0;
			SmallestRectangle1(LineSeg,out row1,out col1, out row2,out col2);
			tempColL = ColPt[0].I + oldWidth * (1 - range / 2);
			tempColR = ColPt[0].I + oldWidth * (1 + range / 2);

			GenRegionLine(out tempLineL, row1 - 5, tempColL, row2 + 5, tempColL);			//生成一条直线区域，区域包含的点为这条线上的所有点
			GenRegionLine(out tempLineR, row1 - 5, tempColR, row2 + 5, tempColR);
			ConcatObj(tempLineL, tempLineR,out concatLines);
			Union1(concatLines, out concatLines);
			Difference(LineSeg, concatLines, out newLine);									//两个区域的差集结果是什么？

			Connection(newLine, out newLine);
			SelectShape(newLine,out newLine, "area", "and", 1, 9999);
			CountObj(newLine,out numTemp);

            if (numTemp != 3)
            {
				return 0;
			}

			//SortContoursXld()

			SortRegion(newLine,out newLine, "upper_left", "true", "column");

			SelectObj(newLine, out newLine, 2);
			int nRet = findEdgePointSingle(Image, newLine,out rPtTemp,out cPtTemp, nEdge, (int)e_LineDirect.R2L, nType, bMean);

			if (nRet == 1)
			{
				newPtCol = cPtTemp[0];
				newPtRow = rPtTemp[0];
				if (fabs(newPtCol - ColPt[0].I - oldWidth) / oldWidth < range)
				{
					ColPt[1] = newPtCol;
					RowPt[1] = newPtRow;
					return 0;
				}
			}


	
			//以右点为基准，向左找线
			tempColL = ColPt[1].I - oldWidth * (1 + range / 2);
			tempColR = ColPt[1].I - oldWidth * (1 - range / 2);
			


			GenRegionLine(out tempLineL,row1 - 5, tempColL, row2 + 5, tempColL);
			GenRegionLine(out tempLineR, row1 - 5, tempColR, row2 + 5, tempColR);
			ConcatObj(tempLineL, tempLineR,out concatLines);                    //将多个对象合并到一个对象数组中
			Union1(concatLines, out concatLines);                               //将对象数组中多个对象合并成一个对象
			Difference(LineSeg,concatLines,out newLine);
			Connection(newLine,out newLine);
			SelectShape(newLine,out newLine,"area","and",1,9999);
			CountObj(newLine,out numTemp);

			if (numTemp != 3)
				return 0;
			
			//nRet = findEdgePointSingle(Image, newLine, &rPtTemp, &cPtTemp, nEdge, L2R, nType, bMean);

			SortRegion(newLine,out newLine, "upper_left", "true", "column");
			SelectObj(newLine,out newLine, 2);
			nRet = findEdgePointSingle(Image, newLine, out rPtTemp,out cPtTemp, nEdge, (int)e_LineDirect.L2R, nType, bMean);

		
			if (nRet == 1)
			{
				newPtCol = cPtTemp[0];
				newPtRow = rPtTemp[0];
				if (fabs(ColPt[1].I() - newPtCol - oldWidth) / oldWidth < range)
				{
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
		public bool findEdgePointDoubleEx( HObject Image,  HObject LineSeg,
	out	HTuple RowPt, out HTuple ColPt,int nEdge,float fRange,
		float oldWidth,int nType = 0, bool bMean = false,bool bRedress = true)
        {

			int iPointNum = 0;

			RowPt = new HTuple();
			ColPt = new HTuple();
			RowPt.Dispose();
			ColPt.Dispose();
			if (findEdgePointDouble(Image, LineSeg,out RowPt,out ColPt, nEdge, (int)e_LineDirect.L2R, nType, bMean) < 2)
			{
				return false;
			}

			if (fRange > 0 && oldWidth > 0 && bRedress)//自动纠错
			{
				iPointNum = testEdgePointDouble(Image, LineSeg,  RowPt, ColPt, nEdge, fRange, oldWidth, nType, bMean);
				if (iPointNum != 0)/*左右点都不正确时,缩放line重定位*/
				{
					HObject zoomLine;
					
					if (iPointNum == 1)//左右大于实际
					{
						///gen_region_line(&zoomLine, RowPt, (*ColPt)[0].L(), (*RowPt)[1].L(), (*ColPt)[1].L());
						GenRegionLine(out zoomLine, RowPt[0], ColPt[0], RowPt[1], ColPt[1]);
					//	*RowPt = HTuple();
					//	*ColPt = HTuple();
						if (findEdgePointDouble(Image, zoomLine,out RowPt,out ColPt, nEdge, (int)e_LineDirect.L2R, nType, bMean) < 2)
						{
							return false;
						}
						if (testEdgePointDouble(Image, zoomLine, RowPt,  ColPt, nEdge, fRange, oldWidth, nType, bMean) != 0)
						{
							return false;
						}
					}
					else if (iPointNum == 2)//左右小于实际
					{
						HTuple MoveDist = 30;//左右扩展的长度
						HTuple Phi;
						HTuple rowLeft, colLeft, rowRight, colRight;
						Phi = new HTuple();
						rowLeft = new HTuple();
						colLeft = new HTuple();
						rowRight = new HTuple();
						colRight = new HTuple();

						Phi.Dispose();
						rowLeft.Dispose();
						colLeft.Dispose();
						rowRight.Dispose();
						colRight.Dispose();

					
						//大量使用HTuple是否会导致性能问题?
						//当时我为什么将基本类型全部换成HTuple
						//line_orientation((*RowPt)[0].D(), (*ColPt)[0].D(), (*RowPt)[1].D(), (*ColPt)[1].D(), &Phi);
						
						LineOrientation(RowPt[0], ColPt[0], RowPt[1], ColPt[1], out Phi);
						rowLeft = RowPt[0] + MoveDist * Phi.TupleSin();
						colLeft = ColPt[0] + MoveDist * Phi.TupleCos();
						rowRight = RowPt[1] + MoveDist * Phi.TupleSin();
						colRight = ColPt[1] + MoveDist * Phi.TupleCos();
						GenRegionLine(out zoomLine,rowLeft, colLeft, rowRight,colRight);
						if (findEdgePointDouble(Image, zoomLine,out RowPt,out ColPt, nEdge, (int)e_LineDirect.L2R, nType, bMean) < 2)
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


		}
		//*功能：找定位线(厨邦葫芦处，赵岩添加)
		public s_CheckResult findBulbLine( HObject hImg, HObject regBulbLing,out HTuple RowPt,out HTuple ColPt)
        {
			s_CheckResult sRsu = new s_CheckResult();
			HObject imgReduce, imgMedian;
			HObject regMedian, regRect;
			HObject xldSub, xldLeft, xldRight;
			HTuple lNum;
			HTuple Row1 = new HTuple(), Col1 = new HTuple(), Row2 = new HTuple(), Column2 = new HTuple();
			HTuple LeftRow = new HTuple(), LeftCol = new HTuple(), RightRow = new HTuple(), RightCol = new HTuple();
			HTuple SelLeftRow = new HTuple(), SelLeftCol = new HTuple(), SelRightRow = new HTuple(), SelRightCol = new HTuple();
			HTuple tpMin = new HTuple(), tpIndices = new HTuple();//,tpSum =HTuple();
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
			tpIndices.Dispose();

			RowPt = new HTuple();
			RowPt.Dispose();
			ColPt = new HTuple();
			ColPt.Dispose();

			//reduce_domain(hImg, regBulbLing, &imgReduce);
			//median_rect(imgReduce, &regMedian, 1, 30);
			ReduceDomain(hImg, regBulbLing, out imgReduce);
			MedianRect(imgReduce,out regMedian,1,30);
			//去掉相应的小水珠
			

			SmallestRectangle1(regMedian,out Row1,out Col1,out Row2,out Column2);
			GenRectangle1(out regRect, Row1 + 2,Col1+2,Row2 - 2,Column2 - 2);
			ReduceDomain(hImg,regRect,out imgReduce);
			EdgesSubPix(imgReduce,out xldSub,"canny",1,10,20);
			SelectShapeXld(xldSub,out xldSub,"height","and",40, 99999999);
			SortContoursXld(xldSub,out xldSub, "upper_left", "true", "column");
			CountObj(xldSub, out lNum);

			if (lNum < 2)
			{
				sRsu.iType = ERROR_LOCATEFAIL;
			//	copy_obj(regRect, &sRsu.regError, 1, -1);
				CopyObj(regRect, out sRsu.regError, 1, -1);
				return sRsu;
			}
			

			SelectObj(xldSub,out xldLeft, 1);
			SelectObj(xldSub, out xldRight, lNum);

		

			GetContourXld(xldLeft, out LeftRow, out LeftCol);

			TupleMin(LeftCol,out tpMin);
			TupleFind(LeftCol, tpMin, out tpIndices);
			TupleSelect(LeftRow, tpIndices,out SelLeftRow);

		

			
		

			TupleSelect(LeftCol, tpIndices,out SelLeftCol);
			tpIndices = new HTuple();
			tpIndices.Dispose();
			tpIndices = 0;
			tpMin = new HTuple();
			GetContourXld(xldRight,out RightRow,out RightCol);
			TupleMax(RightCol, out tpMin);
			TupleFind(RightCol, tpMin,out tpIndices);
			TupleSelect(RightRow, tpIndices,out SelRightRow);

		
			TupleSelect(RightCol, tpIndices,out SelRightCol);
			//tuple_sum(SelRightCol,&tpSum);
			//tuple_length(SelRightCol,&lNum);
			//if (lNum==0)
			//{
			//	sRsu.iType =ERROR_LOCATEFAIL;
			//	copy_obj(regRect,&sRsu.regError,1,-1);
			//	return sRsu;
			//}
			//(*RowPt)[1] = tpSum/lNum;
			//if(bDebug)
			//{
			//	gen_cross_contour_xld(&Cross2, SR4, SC4, 6, 0.785398);//display
			//}

			RowPt[0] = SelLeftRow[0];
			ColPt[0] = SelLeftCol[0];
			RowPt[1] = SelRightRow[0];
			ColPt[1] = SelRightCol[0];
			return sRsu;


		}
		//*功能：找PET环(背景-iBackground，0黑色背景找白色区域，1白色背景找黑色区域)
		public s_CheckResult findPetRing( HObject hImg, HObject regPetRing,out HTuple RowPt,out HTuple ColPt,
		int iGray,int iBackground = 1)
        {

			s_CheckResult sRsu = new s_CheckResult();
			HObject ImageReduced, Regions;
			HObject ROILeft, ROIRight;
			HTuple dRow1, dCol1, dRow2, dCol2;
			HTuple num = 0, Area, AreaAll;
			HTuple RowCenter, ColCenter;				//区域的行中心点和列中心点
			HTuple LinePhi;
			HTuple i;
		

			RowPt = new HTuple();
			ColPt = new HTuple();
			RowCenter = new HTuple();
			ColCenter = new HTuple();
			GenEmptyObj(out ImageReduced);
			GenEmptyObj(out Regions);
			GenEmptyObj(out ROILeft);
			GenEmptyObj(out ROIRight);

			RowPt.Dispose();
			ColPt.Dispose();

			

			ReduceDomain(hImg,regPetRing,out ImageReduced);
			if(0 == iBackground)
            {
				Threshold(ImageReduced,out Regions,iGray,255);
            }
            else
            {
				Threshold(ImageReduced,out Regions,0, iGray);
            }
			

			Connection(Regions, out Regions);
			SelectShapeStd(Regions,out Regions, "max_area", 70);
			SmallestRectangle1(Regions,out dRow1,out dCol1,out dRow2,out dCol2);
			//找左边角点
			

			GenRectangle1(out ROILeft, dRow1, dCol1, dRow2, (dCol1 + dCol2) / 2);
			ReduceDomain(hImg, ROILeft,out ImageReduced);


			

			if(0 == iBackground)
            {
				Threshold(ImageReduced,out Regions, iGray, 255);
            }
            else
            {
				Threshold(ImageReduced, out Regions, 0 ,iGray);
            }
			Connection(Regions,out Regions);
			SelectShapeStd(Regions,out  Regions, "max_area", 70);
			AreaCenter(Regions,out  Area, out RowCenter, out ColCenter);

			//判断是否黑图|白图
			

			AreaCenter(ROILeft,out AreaAll, out RowCenter, out ColCenter);                  //这里代码的RowCenter和 ColCenter前面用过，这这里再次使用覆盖了原代码会不会有问题?
			if ((Area < 50) || (AreaAll - Area) < 50)
            {
				sRsu.iType = ERROR_LOCATEFAIL;
				if(Area > 0)
                {
					CopyObj(Regions, out sRsu.regError, 1, -1);
                }
                else
                {
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
                }

				sRsu.strDescription = "Not found left locate point!";
				return sRsu;
			}

			HTuple LeftRows, LeftColumns;
			HTuple LeftColumnMin, LeftRowsMax;
			HTuple Indices, Length, Rows1;
			//获得轮廓点
			LeftRows = new HTuple();
			LeftColumns = new HTuple();
			LeftColumnMin = new HTuple();
			LeftRowsMax = new HTuple();
			Indices = new HTuple();
			Length = new HTuple();
			Rows1 = new HTuple();
			LeftRows.Dispose();
			LeftColumns.Dispose();
			LeftColumnMin.Dispose();
			LeftRowsMax.Dispose();
			Indices.Dispose();
			Length.Dispose();
			Rows1.Dispose();

			GetRegionConvex(Regions,out LeftRows,out LeftColumns);
			TupleLength(LeftColumns,out Length);
			if (Length == 0)
			{
				sRsu.iType = ERROR_LOCATEFAIL;
				if (Area > 0)
				{
						//	copy_obj(Regions, &sRsu.regError, 1, -1);
					CopyObj(Regions,out sRsu.regError, 1, -1);
				}
				else
				{
					//gen_rectangle1(&sRsu.regError, 20, 20, 120, 120);
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
				}
				//未找到左侧定位点！
				//sRsu.strDescription = QObject::tr("Not found left locate point!");
				sRsu.strDescription = "Not found left locate point!";
				return sRsu;
			}
			else
			{
			


				TupleSortIndex(LeftColumns, out Indices);
				LeftColumnMin = LeftColumns[Indices[0].I];
				LeftRowsMax = LeftRows[Indices[0].I];

				if (Length > 1)
				{
					//差3个像素认为是安全环
					for (i = 1; i < Length; ++i)
					{
						if (LeftColumns[Indices[i].I].I - LeftColumns[Indices[0].I].I < 4)
						{
							//2014.2.24 水珠干扰定位线由上移到下
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


			//找右边角点
			gen_rectangle1(&ROIRight, dRow1, (dCol1 + dCol2) / 2, dRow2, dCol2);
			reduce_domain(hImg, ROIRight, &ImageReduced);
			if (0 == iBackground)
			{
				threshold(ImageReduced, &Regions, iGray, 255);
			}
			else
			{
				threshold(ImageReduced, &Regions, 0, iGray);
			}
			double row, col;
			HOperatorSet.AreaCenter(ROIRight,out row,out col,out col);
			GenRectangle1(out ROIRight, dRow1, (dCol1 + dCol2) / 2, dRow2, dCol2);

			connection(Regions, &Regions);
			select_shape_std(Regions, &Regions, "max_area", 70);

			area_center(Regions, &Area, NULL, NULL);
			area_center(ROIRight, &AreaAll, NULL, NULL);
			if ((Area < 50) || (AreaAll - Area) < 50)
			{
				sRsu.iType = ERROR_LOCATEFAIL;
				if (Area > 0)
				{
					copy_obj(Regions, &sRsu.regError, 1, -1);
				}
				else
				{
					gen_rectangle1(&sRsu.regError, 20, 20, 120, 120);
				}
				//未找到右侧定位点！
				sRsu.strDescription = QObject::tr("Not found right locate point!");
				return sRsu;
			}

			HTuple RightRows, RightColumns;
			int RightColumnMax, RightRowsMax;
			HTuple Rows3;
			get_region_convex(Regions, &RightRows, &RightColumns);
			tuple_length(RightColumns, &Length);
			if (Length == 0)
			{
				sRsu.iType = ERROR_LOCATEFAIL;
				if (Area > 0)
				{
					copy_obj(Regions, &sRsu.regError, 1, -1);
				}
				else
				{
					gen_rectangle1(&sRsu.regError, 20, 20, 120, 120);
				}
				//未找到右侧定位点！
				sRsu.strDescription = QObject::tr("Not found right locate point!");
				return sRsu;
			}
			else
			{
				tuple_sort_index(RightColumns, &Indices);
				RightColumnMax = RightColumns[Indices[Length - 1].I()];
				RightRowsMax = RightRows[Indices[Length - 1].I()];
				if (Length > 1)
				{
					for (i = 1; i < Length; ++i)
					{
						if (RightColumns[Indices[Length - 1].I()].I() - RightColumns[Indices[Length - 1 - i].I()].I() < 4)//???
						{
							if (RightRowsMax < RightRows[Indices[Length - 1 - i].I()].I())
							{
								RightColumnMax = RightColumns[Indices[Length - 1 - i].I()];
								RightRowsMax = RightRows[Indices[Length - 1 - i].I()];
							}
						}
						else
							break;
					}
				}
			}

			//如果基准线角度不对直接返回
			if (abs(LeftRowsMax - RightRowsMax) > 60)
			{
				sRsu.iType = ERROR_LOCATEFAIL;
				if (Area > 0)
				{
					copy_obj(Regions, &sRsu.regError, 1, -1);
				}
				else
				{
					gen_rectangle1(&sRsu.regError, 20, 20, 120, 120);
				}
				return sRsu;
			}

	(*RowPt)[0] = LeftRowsMax;
			(*RowPt)[1] = RightRowsMax;
			(*ColPt)[0] = LeftColumnMin;
			(*ColPt)[1] = RightColumnMax;
			return sRsu;




		}
		//*功能：添加单个字符到模板中
		public void AddSigCharModel(HObject hImg, s_pOCRErrorInfo spOCRErrorInfo, HTuple lCharModelID)
		{
			try
			{
				AddSigCharModel(hImg, spOCRErrorInfo.regErrorChar, spOCRErrorInfo.strErrorChar, spOCRErrorInfo.iType, lCharModelID);
			}
			catch (HException &e)
	{
				//Halcon异常		
				QString strTemp;
				strTemp = e.message;
				strTemp.remove(0, 20);
				strTemp = QString("CCheck::AddSigCharModel,") + strTemp;
				//2013.12.13 保存异常图像和模板	
				WriteLog(strTemp, AbnormityLog);
			}
	catch (...)
	{
				QString strTemp = QString("CCheck::AddSigCharModel,Exception!");
				WriteLog(strTemp, AbnormityLog);
			}
			}
			public void AddSigCharModel(HObject hImg, HObject regChar, String ch, int iType, HTuple lCharModelID)
			{
				Hobject imgCharModel;
				Hobject regObj;
				HTuple tpCharModel;
				long dRow1, dCol1, dRow2, dCol2;
				long lNum;
				if (ch.length() != 1)
				{
					return;
				}
				//2015.4.16 防止空区域
				select_shape(regChar, &regObj, "area", "and", 1, 9999999);
				count_obj(regObj, &lNum);
				if (1 != lNum)
				{
					return;
				}
				QString strTrfFile, strBackTrfFile;
				switch (iType)
				{
					case 0:
						strTrfFile = g_strAppPath + "OCRModel\\NumberModel.trf";
						strBackTrfFile = g_strAppPath + "OCRModel\\BackNumberModel.trf";
						break;
					case 1:
						strTrfFile = g_strAppPath + "OCRModel\\LowerCaseModel.trf";
						strBackTrfFile = g_strAppPath + "OCRModel\\BackLowerCaseModel.trf";
						break;
					case 2:
						strTrfFile = g_strAppPath + "OCRModel\\UpperCaseModel.trf";
						strBackTrfFile = g_strAppPath + "OCRModel\\BackUpperCaseModel.trf";
						break;
					default:
						strTrfFile = g_strAppPath + "OCRModel\\NumberModel.trf";
						strBackTrfFile = g_strAppPath + "OCRModel\\BackNumberModel.trf";
						break;
				}
				//删除备份文件
				if (QFile::exists(strBackTrfFile))
				{
					QFile::remove(strBackTrfFile);
				}
				//拷贝备份文件操作
				if (QFile::exists(strTrfFile))
				{
					QFile::copy(strTrfFile, strBackTrfFile);
					read_ocr_trainf(&imgCharModel, strBackTrfFile, &tpCharModel);
					tuple_concat(tpCharModel, HTuple(ch.toLocal8Bit().constData()), &tpCharModel);
					tuple_sort(tpCharModel, &tpCharModel);
					tuple_uniq(tpCharModel, &tpCharModel);
				}
				else
				{
					tuple_uniq(HTuple(ch.toLocal8Bit().constData()), &tpCharModel);
				}
				append_ocr_trainf(regObj, hImg, ch, strBackTrfFile);
				smallest_rectangle1(regObj, &dRow1, &dCol1, &dRow2, &dCol2);
				create_ocr_class_mlp(dCol2 - dCol1, dRow2 - dRow1,
					"constant", "default", tpCharModel, 80, "none", 10, 42, &lCharModelID);
				trainf_ocr_class_mlp(lCharModelID, strBackTrfFile, 200, 1, 0.01, NULL, NULL);
				//2015.4.16正确修正后-备份字符
				BackUpCharModel();
			}
			//*检测区域预处理
			//结构体也要传指针
			public bool genValidROI(HObject imgSrc, ref s_pROIPara roiPara, HObject ROI, out HObject validROI)
            {
				Hlong nRow1, nCol1, nRow2, nCol2;
				Hobject imgReduced;
				Hobject PartionRegion, RegionBin, ConnectedRegions, SelectedRegions;
				gen_empty_obj(validROI);

				int nPartNum;//分割份数	
				reduce_domain(imgSrc, ROI, &imgReduced);
				smallest_rectangle1(ROI, &nRow1, &nCol1, &nRow2, &nCol2);
				int nRegionHeight = nRow2 - nRow1 + 1;
				long nWidth, nHeight;
				get_image_size(imgSrc, &nWidth, &nHeight);
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
				partition_rectangle(ROI, &PartionRegion, nWidth, nPartHeight);
				HTuple Mean, PreMean;
				intensity(PartionRegion, imgSrc, &Mean, NULL);
				PreMean = Mean * roiPara.fRoiRatio;

				Hobject selected, tempRegion;
				gen_empty_obj(&RegionBin);

				// 在区域较小时分割后的区域个数会小于分割参数
				long nNumber;
				count_obj(PartionRegion, &nNumber);

				for (int i = 0; i < nNumber; ++i)
				{
					select_obj(PartionRegion, &selected, i + 1);
					reduce_domain(imgSrc, selected, &tempRegion);
					threshold(tempRegion, &tempRegion, HTuple(PreMean[i]), HTuple(255));
					concat_obj(RegionBin, tempRegion, &RegionBin);
				}
				union1(RegionBin, &RegionBin);

				long areaPro, areaMin;
				area_center(ROI, &areaPro, NULL, NULL);
				areaMin = areaPro / 3;
				areaMin = areaMin > 550 ? 550 : areaMin;

				// 填充裂纹区域
				closing_rectangle1(RegionBin, &RegionBin, 1, roiPara.nClosingWH);
				closing_rectangle1(RegionBin, &RegionBin, roiPara.nClosingWH, 1);

				connection(RegionBin, &ConnectedRegions);
				select_shape(ConnectedRegions, &SelectedRegions, HTuple("area"),
					HTuple("and"), HTuple(areaMin), HTuple(9999999));

				// 判断是几个区域
				count_obj(SelectedRegions, &nNumber);
				if (nNumber > 1)
				{
					select_shape_std(SelectedRegions, &SelectedRegions, "max_area", 70);
				}
				else
				{
					copy_obj(SelectedRegions, validROI, 1, -1);
					return false;
				}

				// 找出预处理区域后缩小一定范围进行检测
				// 主要作用是切断那些裂纹,大缺陷
				fill_up(SelectedRegions, &SelectedRegions);

				// 判断实际处理区域比设置的处理区域小很多的情况
				Hobject RegGap;
				difference(ROI, SelectedRegions, &RegGap);
				opening_rectangle1(RegGap, &RegGap, roiPara.nGapWH, roiPara.nGapWH);
				connection(RegGap, &RegGap);
				select_shape(RegGap, &RegGap, HTuple("width").Concat("height"), "and",
					HTuple(roiPara.nGapWH).Concat(roiPara.nGapWH),
					HTuple(9999).Concat(9999));
				count_obj(RegGap, &nNumber);
				if (nNumber > 0)
				{
					copy_obj(RegGap, validROI, 1, -1);
					return false;
				}

				// 往内部缩小一定区域
				erosion_circle(SelectedRegions, &SelectedRegions, HTuple(2.5));
				//2013.9.16 nanjc 一个区域缩为两个区域，选大区域
				connection(SelectedRegions, &SelectedRegions);
				count_obj(SelectedRegions, &nNumber);
				if (nNumber > 1)
				{
					select_shape_std(SelectedRegions, &SelectedRegions, "max_area", 70);
				}
				copy_obj(SelectedRegions, validROI, 1, -1);
				return true;
			}
		//*功能：按行分割
		public void segmentCharLine(HObject regChar, int iLineCount, out HObject regLine1, out HObject regLine2, out HObject regLine3)
            {
				Hobject regUnion, regSel;
				Hlong lNum;
				int i;
				Hlong Row1, Column1, Row2, Column2;
				double Row, Column;
				//确实regLine1是输出变量 应该用out修饰
				gen_empty_region(regLine1);
				gen_empty_region(regLine2);
				gen_empty_region(regLine3);
				count_obj(regChar, &lNum);
				if (lNum > 0)
				{
					union1(regChar, &regUnion);
					smallest_rectangle1(regUnion, &Row1, &Column1, &Row2, &Column2);
					switch (iLineCount)
					{
						case 1:
							sort_region(regChar, regLine1, "upper_left", "true", "column");
							break;
						case 2:
							for (i = 0; i < lNum; ++i)
							{
								select_obj(regChar, &regSel, i + 1);
								area_center(regSel, NULL, &Row, &Column);
								if (Row < Row1 + (Row2 - Row1) / 2)
								{
									concat_obj(*regLine1, regSel, regLine1);
								}
								else
								{
									concat_obj(*regLine2, regSel, regLine2);
								}
							}
							sort_region(*regLine1, regLine1, "upper_left", "true", "column");
							sort_region(*regLine2, regLine2, "upper_left", "true", "column");
							break;
						case 3:
							for (i = 0; i < lNum; ++i)
							{
								select_obj(regChar, &regSel, i + 1);
								area_center(regSel, NULL, &Row, &Column);
								if (Row < Row1 + (Row2 - Row1) / 3)
								{
									concat_obj(*regLine1, regSel, regLine1);
								}
								else
								{
									if (Row < Row1 + (Row2 - Row1) * 2 / 3)
									{
										concat_obj(*regLine2, regSel, regLine2);
									}
									else
									{
										concat_obj(*regLine3, regSel, regLine3);
									}
								}
							}
							sort_region(*regLine1, regLine1, "upper_left", "true", "column");
							sort_region(*regLine2, regLine2, "upper_left", "true", "column");
							sort_region(*regLine3, regLine3, "upper_left", "true", "column");
							break;
						default:
							sort_region(regChar, regLine1, "upper_left", "true", "column");
							break;
					}
				}
			}
		//*提取字符区域
		public s_CheckResult segmentChar( HObject imageSrc, HObject regValid,s_pOCRPublicPara spOCRPublicPara,out HObject regChar);
		//*功能：检测安全环: 0-区域中提取regCaps,1-传递进来regCaps
		//c++传指针就相当于c#传引用
		//先写下定义，每个函数的实现还须检查参数是否合理
		public s_CheckResult safeRingCheck0( HObject imageSrc, HObject regValid,ref s_pCapsPara spCapsPara,bool bLocate,double dOriRow);
		public s_CheckResult safeRingCheck1( HObject imageSrc, HObject regValid,ref s_pCapsPara spCapsPara);
		//*功能：检测瓶盖，高低盖和角度
		public s_CheckResult capsCheck( HObject hImg, HObject regCaps,ref s_pCapsPara spCapsPara,out HObject regCapsLine);
		//原则，c#中对象直接传递；需要将数值输出的需要用out修饰;需要引用结构体成员值或者在函数中改变结构体成员的值需要传递结构体的引用

		public s_CheckResult charCheckBase(const Hobject &imageSrc,const Hobject &regValid,s_pOCRPara &spOCRPara,Hobject regCharPos, Hobject *regChar);
	//*功能：检测瓶盖
		public s_CheckResult capsCheckBase(const Hobject &imageSrc,const Hobject &regValid,const Hobject &regCaps,
		s_pCapsPara &spCapsPara,Hobject &regCapsLine,bool bLocate,int iLocMethod,double dOriRow);

		public void savePMToolPara(QSettings &paraSet,ref s_pPMToolPara spPMToolPara);
		public void readPMToolPara(QSettings &paraSet, ref s_pPMToolPara spPMToolPara);
		//*功能：读写OCR参数
		public void saveOCRPara(QSettings &paraSet, ref s_pOCRPara spOCRPara);
		public void readOCRPara(QSettings &paraSet, ref s_pOCRPara spOCRPara);
		//*功能：读写Caps参数
		public void saveCapsPara(QSettings &paraSet, ref s_pCapsPara spCapsPara);
		public void readCapsPara(QSettings &paraSet, ref s_pCapsPara spCapsPara);

	}
}
