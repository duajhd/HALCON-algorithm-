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

	//圆形标签检测参数
	public struct s_pCircleLabel
	{
		public bool bLocate;               //定位
		public int iLocGray;               //灰度值

		public bool bCaps;                 //瓶盖
		public s_pCapsPara spCapsPara;

		public bool bLabel;                //标签
		public int iLabelGray;             //灰度
		public int iLabelDislocation;      //错位
		public int iLabelLength;           //长度
		public int iLabelMeanGray;         //平均灰度
	};


	//图形参数
	public struct s_oCircleLabel
	{
		public HObject regLocateRegion;            //定位区域
		public s_MyOri sModelOri;          //模板原点
		public s_MyOri sCurrentOri;        //当前坐标原点

		public HObject regCapsRegion, AregCapsRegion;
		public HObject regLabelRegion, AregLabelRegion;

	};

	//内部参数，仅用于储存内部检测临时数据
	public struct s_tCircleLabel
	{

	};

	//调试中间结果图像（仅调试时使用，不同类型绘制不同颜色）
	public struct s_mCircleLabel
	{
		public HObject regCheck;       //检测出的结果
		public HObject regCorrect;     //修正结果
	};

	public class CCheckCircleLabel : CommonCheck
	{
		s_pCircleLabel m_pCircleLabel;  //数字参数
		s_oCircleLabel m_oCircleLabel;  //图像参数
		s_tCircleLabel m_tCircleLabel;  //临时参数
		s_mCircleLabel m_mCircleLabel;  //中间结果（调试时使用）
		public CCheckCircleLabel()
		{

		}
		~CCheckCircleLabel()
		{

		}

		public void readParam()
		{

		}
		public void writeParam()
		{

		}

		public void affineShape()
		{
			

			if (m_pCircleLabel.bLocate)
			{
				HTuple homMat2DIdentity;

				//此处仅平移
				HomMat2dIdentity(out homMat2DIdentity);
				VectorAngleToRigid(0, m_oCircleLabel.sModelOri.dOriCol, 0,
					0, m_oCircleLabel.sCurrentOri.dOriCol, 0, out homMat2DIdentity);
				AffineTransRegion(m_oCircleLabel.regCapsRegion, out m_oCircleLabel.AregCapsRegion, homMat2DIdentity, "false");
				AffineTransRegion(m_oCircleLabel.regLabelRegion, out m_oCircleLabel.AregLabelRegion, homMat2DIdentity, "false");

			}
			else
			{
				CopyObj(m_oCircleLabel.regCapsRegion, out m_oCircleLabel.AregCapsRegion, 1, -1);
				CopyObj(m_oCircleLabel.regLabelRegion, out m_oCircleLabel.AregLabelRegion, 1, -1);
			}
		}

		public void displayShape(HTuple lWindID)
		{
			if (m_pCircleLabel.bLocate)
			{
				

				SetColor(lWindID, "green");
				DispObj(m_oCircleLabel.regLocateRegion, lWindID);

			}
			//需根据模板原点变换的区域，显示变换后的区域
			if (m_pCircleLabel.bCaps)
			{
				

				SetColor(lWindID, "green");
				DispObj(m_oCircleLabel.AregCapsRegion, lWindID);
			}
			//标签
			if (m_pCircleLabel.bLabel)
			{
				

				SetColor(lWindID, "green");
				DispObj(m_oCircleLabel.AregLabelRegion, lWindID);
			}
			//显示调试图像
			if (g_iUserPermit > 5)
			{
			

				SetColor(lWindID, "yellow");
				DispObj(m_mCircleLabel.regCheck, lWindID);
				SetColor(lWindID, "blue");
				DispObj(m_mCircleLabel.regCorrect, lWindID);
			}


		}
		public s_CheckResult location(HObject hImg, bool bDebug/* = false*/, bool bUpdateOri/* = false*/, bool bCalib/* = false*/)
		{

			s_CheckResult sRsu = new s_CheckResult();
			try
			{
				if (m_pCircleLabel.bLocate)
				{

					

					HObject imgReduce;
					HObject regThresh, regTemp, regCircle;
					HTuple dRow1, dCol1, dRow2, dCol2;
					HTuple lNum;
					SmallestRectangle1(m_oCircleLabel.regLocateRegion, out dRow1, out dCol1, out dRow2, out dCol2);
					ReduceDomain(hImg, m_oCircleLabel.regLocateRegion, out imgReduce);
					Threshold(imgReduce, out regThresh, m_pCircleLabel.iLocGray, 255);
					Connection(regThresh, out regTemp);
					SelectShape(regTemp, out regTemp, "area", "and", 100, 99999);
					SelectShape(regTemp, out regTemp, "width", "and", (dCol2 - dCol1) / 2, 99999);
					CountObj(regTemp, out lNum);

					if (lNum == 0)
					{
						sRsu.iType = ERROR_LOCATEFAIL;
						//竖直线定位失败
						sRsu.strDescription = QObject::tr("Not find region of the Caps!");

						CopyObj(m_oCircleLabel.regLocateRegion, out sRsu.regError, 1, -1);
						return sRsu;
					}


					SelectShapeStd(regTemp, out regTemp, "max_area", 70);
					SmallestRectangle1(regTemp, out dRow1, out dCol1, out dRow2, out dCol2);
					m_oCircleLabel.sCurrentOri.dOriRow = dRow1;
					m_oCircleLabel.sCurrentOri.dOriCol = (dCol2 + dCol1) / 2;
					m_oCircleLabel.sCurrentOri.dOriPhi = 0;
					if (bDebug)
					{

						GenCircle(out regCircle, m_oCircleLabel.sCurrentOri.dOriRow, m_oCircleLabel.sCurrentOri.dOriCol, 5);
						ConcatObj(m_mCircleLabel.regCheck, regCircle, out m_mCircleLabel.regCheck);
					}
					//更新原点：检测测试和保存模板时，需更新模板原点。实际检测时不更新
					if (bUpdateOri)
					{
						m_oCircleLabel.sModelOri = m_oCircleLabel.sCurrentOri;
					}
				}
				return sRsu;
			}
			catch (Exception e)
			{
				//Halcon异常		
				String tempStr;
				tempStr = e.message;
				tempStr.remove(0, 20);
				sRsu.iType = ERROR_EXCEPTION;
				sRsu.strDescription = "CCheckCircleLabel::location," + tempStr;
				//2013.12.13 保存异常图像和模板	
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}
			catch (...)   //c#中如何表示两个catch
			{
				sRsu.iType = ERROR_EXCEPTION;
				sRsu.strDescription = "CCheckCircleLabel::location";
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}

			}

		}

		public s_CheckResult labelCheck(Hobject srcImg, bool bDebug/* = false*/)
        {
			s_CheckResult sRsu = new s_CheckResult();
			try
			{
				if (m_pCircleLabel.bLabel)
				{
					HObject imgReduce, Rectangle, Rectangle1, imgReduce1, regConnectRegions, RegionDifference, skeleton;
					HObject regThresh, regFillUp, regOpening, ConnectedRegions1, SelectedRegions2, regLine, regDiff, regOpening1;
					HObject xldLabel, xldTemp, xldSel, regTemp;
					HObject regSelected1, regSelecteds, regIntersection, RegionUnion, RegionLines, regClose, RegionBorder;
					HObject Contours, ContoursSplit, SelectedXLD;
					HTuple dRow1, dCol1, dRow2, dCol2;
					HTuple lRow11, lColumn11, lRow21, lColumn21;
					HTuple lRow1, lColumn1, lRow2, lColumn2;
					HTuple xldRow1, xldRow2, xldCol1, xldCol2, dMeanGray;
					HTuple LMeanRow, RMeanRow, LMean, RMean;
					int iDislocation = 0;
					HTuple tpRow, tpCol, tpMin, tpMax, tpIndex, tpSel;
					HTuple lNum;
					int i;


					ReduceDomain(srcImg, m_oCircleLabel.AregLabelRegion, out imgReduce);
					Threshold(imgReduce, out regThresh, m_pCircleLabel.iLabelGray, 255);
					FillUpShape(regThresh, out regFillUp, "area", 1, 100);
					OpeningRectangle1(regFillUp, out regOpening, 30, 120);
					Connection(regOpening, out ConnectedRegions1);
					SelectShapeStd(ConnectedRegions1, out SelectedRegions2, "max_area", 70);
					CountObj(SelectedRegions2, out lNum);

					if (lNum == 0)
					{
						sRsu.iType = ERROR_NOLABEL;
						//竖直线定位失败
						sRsu.strDescription = QObject::tr("Not find region of the Label!");

						CopyObj(m_oCircleLabel.AregLabelRegion, out sRsu.regError, 1, -1);
						return sRsu;
					}


					SmallestRectangle1(SelectedRegions2, out lRow1, out lColumn1, out lRow2, out lColumn2);

					GenRectangle1(out Rectangle, lRow1 - 10, lColumn1, lRow2, lColumn2);
					ReduceDomain(imgReduce, Rectangle, out imgReduce);
					Threshold(imgReduce, out imgReduce1, 30, 255);
					OpeningRectangle1(imgReduce, out regOpening1, 5, 20);
					Connection(regOpening, out regConnectRegions);
					SelectShape(regConnectRegions, out regSelected1, "area", "and", 300, 999999);
					SelectShapeStd(regSelected1, out regSelecteds, "max_area", 70);
					CountObj(regSelecteds, out lNum);


					if (lNum == 0)
					{
						sRsu.iType = ERROR_NOLABEL;
						//竖直线定位失败
						sRsu.strDescription = QObject::tr("Not find region of the Label!");

						CopyObj(regSelected1, out sRsu.regError, 1, -1);
						return sRsu;
					}


					SmallestRectangle1(regSelecteds, out lRow11, out lColumn11, out lRow21, out lColumn21);
					GenRectangle1(out Rectangle1, lRow11 - 10, lColumn11, lRow11 + 50, lColumn21);
					Intersection(SelectedRegions2, Rectangle1, out regIntersection);



					GenRegionLine(out RegionLines, lRow11 + 50, 0, lRow11 + 50, lColumn21);
					Union2(regIntersection, RegionLines, out RegionUnion);
					ClosingRectangle1(RegionUnion, out regClose, 1, 1000);
					Boundary(regClose, out RegionBorder, "inner");
					Difference(RegionBorder, RegionLines, out RegionDifference);
					Skeleton(RegionDifference, out skeleton);
					GenContoursSkeletonXld(skeleton, out Contours, 1, "filter");
					SegmentContoursXld(Contours, out ContoursSplit, "lines", 5, 4, 2);
					SelectShapeXld(ContoursSplit, out SelectedXLD, "width", "and", 40, 99999);
					CountObj(SelectedXLD, out lNum);


					if (lNum == 0)
					{
						sRsu.iType = ERROR_NOLABEL;
						//竖直线定位失败
						sRsu.strDescription = QObject::tr("Not find region of the Label!");

						CopyObj(ContoursSplit, out sRsu.regError, 1, -1);
						return sRsu;
					}


					if (lNum > 1)
					{

						SortContoursXld(SelectedXLD, out xldLabel, "upper_left", "true", "column");
						for (i = 0; i < lNum; ++i)
						{


							SelectObj(xldLabel, out xldSel, i + 1);
							GetContourXld(xldSel, out tpRow, out tpCol);
							TupleMin(tpCol, out tpMin);
							TupleFind(tpCol, tpMin, out tpIndex);
							TupleSelect(tpRow, tpIndex, out tpSel);
							TupleMean(tpSel, out LMean);
							TupleMax(tpCol, out tpMax);
							TupleFind(tpCol, tpMax, out tpIndex);
							TupleSelect(tpRow, tpIndex, out tpSel);
							TupleMean(tpSel, out RMean);

							if (0 == i)
							{
								LMeanRow = LMean;
								RMeanRow = RMean;
							}
							else
							{
								if (fabs(RMeanRow - LMean) > iDislocation)
									iDislocation = floor(fabs(RMeanRow - LMean));
								LMeanRow = LMean;
								RMeanRow = RMean;
							}
						}
						if (iDislocation > m_pCircleLabel.iLabelDislocation)
						{
							sRsu.iType = ERROR_LABELDISLOC;
							//竖直线定位失败
							sRsu.strDescription = QObject::tr("Label Dislocation %1!").arg(iDislocation);


							GenRegionContourXld(xldLabel, out sRsu.regError, "margin");
							return sRsu;
						}
						else
						{
							sRsu.strResultInfo += QObject::tr("Dislocation Value %1!	").arg(iDislocation);
						}


						UnionAdjacentContoursXld(xldLabel, out xldTemp, 20, 1, "attr_keep");
						SmallestRectangle1Xld(xldTemp, out xldRow1, out xldCol1, out xldRow2, out xldCol2);

						if (xldCol2 - xldCol1 < m_pCircleLabel.iLabelLength)
						{
							sRsu.iType = ERROR_NOLABEL;
							//竖直线定位失败
							sRsu.strDescription = QObject::tr("Error length of the Label:%1!").arg(xldCol2 - xldCol1);

							CopyObj(regThresh, out sRsu.regError, 1, -1);
							return sRsu;
						}
						else
						{
							sRsu.strResultInfo += QObject::tr("Length:%1!	").arg(xldCol2 - xldCol1);
						}
					}
					if (bDebug)
					{


						GenRegionContourXld(SelectedXLD, out regTemp, "margin");
						ConcatObj(m_mCircleLabel.regCheck, regTemp, out m_mCircleLabel.regCheck);


					}

				}
			}
			catch (HException &e)
			{
				//Halcon异常		
				String tempStr;
				tempStr = e.message;
				tempStr.remove(0, 20);
				sRsu.iType = ERROR_EXCEPTION;
				sRsu.strDescription = QString("CCheckCircleLabel::labelCheck,") + tempStr;
				//2013.12.13 保存异常图像和模板	
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}
			catch (...)
			{
				sRsu.iType = ERROR_EXCEPTION;
				sRsu.strDescription = QString("CCheckCircleLabel::location");
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}
			}
			}
	}
}