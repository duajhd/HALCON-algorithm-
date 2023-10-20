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


	//罐壁变形检测参数
	public struct s_pBottleMorph
	{
		//异形瓶
		public	bool bLocate;               //定位
		public int iFloatRange;            //矫正
		public int iEdge;                  //对比度，用于定位和尺寸
		public float fActiveWidth;         //输入宽度，用于标定
		public float fScale;               //比例尺
										   //尺寸
										   //宽度
		public bool bHoriSize;
		public float fHoriMin;
		public float fHoriMax;
		//高度
		public bool bVertSize;
		public float fVertMin;
		public float fVertMax;
		//尺寸1
		public bool bSize1;
		public float fSizeMin1;
		public float fSizeMax1;
		//其它颜色瓶
		public bool bColor;
		public int iColorGrayMin;
		public int iColorGrayMax;

		//缺陷
		public bool bGenRoi;   //预处理
		public s_pROIPara sROIPara;
		public bool bDefect;
		public int iDefectEdge;
		public int iDefectArea;
		public int iDefectLength;
	};


	//图形参数
	public struct s_oBottleMorph
	{
		public s_MyOri sModelOri;      //模板原点
		public s_MyOri sCurrentOri;    //当前坐标原点

		public float fWidth1;  //判断连瓶
		public float fWidth2;

		public HObject regHorizonRect;         //横向区域
		public HObject regVerticalLine;        //纵向线
		public HObject regSizeRegion1, AregSizeRegion1;            //尺寸1区域
		public HObject regColorRegion, AregColorRegion;            //颜色区域
		public HObject regDefectRegion, AregDefectRegion;      //缺陷检测区域
	};

	//内部参数，仅用于储存内部检测临时数据
	public struct s_tBottleMorph
	{
		public s_LineLocPara sHoriLine1;//横向线
		public s_LineLocPara sHoriLine2;
		public double dVertSize;       //纵向尺寸
		public s_LineLocPara sSize1;   //size1
		public int iColorGray;         //颜色灰度值
	};

	//调试中间结果图像（仅调试时使用，不同类型绘制不同颜色）
	public struct s_mBottleMorph
	{
		public HObject regCheck;       //检测出的结果
		public HObject regCorrect;     //修正结果
	};

	public  class CCheckBottleMorph :CommonCheck
    {
		public s_pBottleMorph m_pBottleMorph;  //数字参数
		public s_oBottleMorph m_oBottleMorph;  //图像参数
		public s_tBottleMorph m_tBottleMorph;  //临时参数
		public s_mBottleMorph m_mBottleMorph;  //中间结果（调试时使用）


		public void readParam()
        {
			string INIPath = Convert.ToString(System.AppDomain.CurrentDomain.BaseDirectory) + "test.ini";
			Setting parSet = new Setting(INIPath);
			parSet.beginGroup("default");
			m_pBottleMorph.bColor = Convert.ToBoolean(parSet.value("bColor", "Fasle"));
			parSet.endGroup();

		}

		public void affineShape()
        {
			if (m_pBottleMorph.bLocate)
			{
				HTuple homMat2DIdentity;
				



				
				HomMat2dIdentity(out homMat2DIdentity);
				//此处仅移动不旋转
				VectorAngleToRigid(m_oBottleMorph.sModelOri.dOriRow, m_oBottleMorph.sModelOri.dOriCol, 0,
					m_oBottleMorph.sCurrentOri.dOriRow, m_oBottleMorph.sCurrentOri.dOriCol, 0, out homMat2DIdentity);

				AffineTransRegion(m_oBottleMorph.regSizeRegion1, out m_oBottleMorph.AregSizeRegion1, homMat2DIdentity, "false");
				AffineTransRegion(m_oBottleMorph.regColorRegion, out m_oBottleMorph.AregColorRegion, homMat2DIdentity, "false");
				AffineTransRegion(m_oBottleMorph.regDefectRegion, out m_oBottleMorph.AregDefectRegion, homMat2DIdentity, "false");
			}
			else
			{
			

				CopyObj(m_oBottleMorph.regSizeRegion1, out m_oBottleMorph.AregSizeRegion1, 1, -1);
				CopyObj(m_oBottleMorph.regColorRegion, out m_oBottleMorph.AregColorRegion, 1, -1);
				CopyObj(m_oBottleMorph.regDefectRegion, out m_oBottleMorph.AregDefectRegion, 1, -1);
			}
		}

		void displayShape(HTuple lWindID)
		{
			//显示定位区域
			if (m_pBottleMorph.bLocate && (m_pBottleMorph.bHoriSize || m_pBottleMorph.bVertSize || m_pBottleMorph.bColor || m_pBottleMorph.bDefect))
			{
				

				SetColor(lWindID, "green");
				DispObj(m_oBottleMorph.regHorizonRect, lWindID);
				DispObj(m_oBottleMorph.regVerticalLine, lWindID);

			}
			//颜色
			if (m_pBottleMorph.bColor)
			{
				
				//显示灰度
				//显示尺寸数据
				
		

				SetColor(lWindID, "green");
				DispObj(m_oBottleMorph.AregColorRegion, lWindID);
				HTuple dRow1, dCol1, dRow2, dCol2;
				SmallestRectangle1(m_oBottleMorph.AregColorRegion, out dRow1, out dCol1, out dRow2, out dCol2);
				String strCurGray;

				SetColor(lWindID, "navy");
				strCurGray =m_tBottleMorph.iColorGray.ToString(); 
				SetTposition(lWindID, (dRow1 + dRow2) / 2, (dCol1 + dCol2) / 2);
				WriteString(lWindID, strCurGray.toLocal8Bit().constData());

			}
			//需根据模板原点变换的区域，显示变换后的区域
			if (m_pBottleMorph.bDefect)
			{
				

				SetColor(lWindID, "green");
				DispObj(m_oBottleMorph.AregDefectRegion, lWindID);
			}
			//显示尺寸
			if (m_pBottleMorph.bHoriSize)
			{
				
				

				HTuple rowCent1, colCent1, rowCent2, colCent2;
				rowCent1 = (m_tBottleMorph.sHoriLine1.iRow1 + m_tBottleMorph.sHoriLine1.iRow2) / 2;
				colCent1 = (m_tBottleMorph.sHoriLine1.iCol1 + m_tBottleMorph.sHoriLine1.iCol2) / 2;
				rowCent2 = (m_tBottleMorph.sHoriLine2.iRow1 + m_tBottleMorph.sHoriLine2.iRow2) / 2;
				colCent2 = (m_tBottleMorph.sHoriLine2.iCol1 + m_tBottleMorph.sHoriLine2.iCol2) / 2;


				

				SetColor(lWindID, "yellow");
				DispArrow(lWindID, rowCent1, colCent1, m_tBottleMorph.sHoriLine1.iRow1, m_tBottleMorph.sHoriLine1.iCol1, 3);
				DispArrow(lWindID, rowCent1, colCent1, m_tBottleMorph.sHoriLine1.iRow2, m_tBottleMorph.sHoriLine1.iCol2, 3);
				DispArrow(lWindID, rowCent2, colCent2, m_tBottleMorph.sHoriLine2.iRow1, m_tBottleMorph.sHoriLine2.iCol1, 3);
				DispArrow(lWindID, rowCent2, colCent2, m_tBottleMorph.sHoriLine2.iRow2, m_tBottleMorph.sHoriLine2.iCol2, 3);
				//显示尺寸数据
			//	QString strCurDist;
				
				//question1 arg函数有什么用2.
				//QString::number作用是将字符串转换成double同时保留有效位数
				//现在转换成c#就要1.先将字符串转换成浮点数2.再保留相应位数3.最后填充模板
				//number作用是将
				//这里的%1什么意思？%1没有特殊含义，就是普通占位符
			//	strCurDist = QString("%1 mm").arg(QString::number(m_tBottleMorph.sHoriLine1.iDistance / m_pBottleMorph.fScale, 'f', 2));


				String strCurDist;
				 strCurDist = String.Format("{ 0:F}",m_tBottleMorph.sHoriLine1.iDistance / m_pBottleMorph.fScale) +"mm" ;

				SetColor(lWindID, "navy");
				SetTposition(lWindID, m_tBottleMorph.sHoriLine1.iRow1, m_tBottleMorph.sHoriLine1.iCol1);
				WriteString(lWindID, strCurDist.toLocal8Bit().constData());

				strCurDist = String.Format("{ 0:F}", m_tBottleMorph.sHoriLine2.iDistance / m_pBottleMorph.fScale) + "mm";
				SetTposition(lWindID, m_tBottleMorph.sHoriLine2.iRow1, m_tBottleMorph.sHoriLine2.iCol1);
				WriteString(lWindID, strCurDist.toLocal8Bit().constData());
				//修复toLocal8Bit必须要明确tolocal8Bit的功能是什么
			}
			//显示尺寸
			if (m_pBottleMorph.bVertSize)
			{
				//显示尺寸数据
		
			//	arg函数有什么用?
				String strCurDist;
				SetColor(lWindID, "navy");
				strCurDist = String.Format("{ 0:F}", m_tBottleMorph.dVertSize / m_pBottleMorph.fScale) + "mm";
				SetTposition(lWindID, m_oBottleMorph.sCurrentOri.dOriRow, m_oBottleMorph.sCurrentOri.dOriCol);
				WriteString(lWindID, strCurDist.toLocal8Bit().constData());
			}
			//显示size1
			if (m_pBottleMorph.bSize1)
			{
				
	
				//显示尺寸数据
				SetColor(lWindID, "green");
				DispObj(m_oBottleMorph.AregSizeRegion1, lWindID);
				HTuple rowCent1, colCent1, rowCent2, colCent2;
				rowCent1 = (m_tBottleMorph.sSize1.iRow1 + m_tBottleMorph.sSize1.iRow2) / 2;
				colCent1 = (m_tBottleMorph.sSize1.iCol1 + m_tBottleMorph.sSize1.iCol2) / 2;
				SetColor(lWindID, "yellow");
				DispArrow(lWindID, rowCent1, colCent1, m_tBottleMorph.sSize1.iRow1, m_tBottleMorph.sSize1.iCol1, 3);
				DispArrow(lWindID, rowCent1, colCent1, m_tBottleMorph.sSize1.iRow2, m_tBottleMorph.sSize1.iCol2, 3);
				//显示尺寸数据
				String strCurDist;
				SetColor(lWindID, "navy");
				strCurDist = String.Format("{ 0:F}", m_tBottleMorph.sSize1.iDistance / m_pBottleMorph.fScale) + "mm";
				SetTposition(lWindID, m_tBottleMorph.sSize1.iRow1, m_tBottleMorph.sSize1.iCol1);
				WriteString(lWindID, strCurDist.toLocal8Bit().constData());



			}
			//显示调试图像
			if (g_iUserPermit > 5)      //g_iUserPermit这个变量没有定义
			{


				SetColor(lWindID, "yellow");
				DispObj(m_mBottleMorph.regCheck, lWindID);
				SetColor(lWindID, "blue");
				DispObj(m_mBottleMorph.regCorrect, lWindID);
			}
		}



		//定位
		s_CheckResult CCheckBottleMorph::location(HObject hImg, bool bDebug/* = false*/, bool bUpdateOri /*= false*/, bool bCalib/* = false*/)
		{
			s_CheckResult sRsu = new s_CheckResult();
			HObject regLine1, regLine2, regLine3, regCircle;
			HTuple tpRows, tpCols;
			HTuple Row1, Col1, Row2, Col2;
			HTuple MidRow1, MidCol1, MidRow2, MidCol2, OriAngle;
			if (m_pBottleMorph.bLocate && (m_pBottleMorph.bHoriSize || m_pBottleMorph.bVertSize || m_pBottleMorph.bDefect || bCalib))
			{
				//	smallest_rectangle1(m_oBottleMorph.regHorizonRect, &Row1, &Col1, &Row2, &Col2);
				SmallestRectangle1(m_oBottleMorph.regHorizonRect, out Row1, out Col1, out Row2, out Col2);
				//第一条线
				//gen_region_line(&regLine1, Row1 + (Row2 - Row1) / 3, Col1, Row1 + (Row2 - Row1) / 3, Col2);
				GenRegionLine(out regLine1, Row1 + (Row2 - Row1) / 3, Col1, Row1 + (Row2 - Row1) / 3, Col2);
				if (bDebug)
				{
					
					ConcatObj(m_mBottleMorph.regCheck, regLine1, out m_mBottleMorph.regCheck);
				}
				if (!findEdgePointDoubleEx(hImg, regLine1, out tpRows, out tpCols, m_pBottleMorph.iEdge, (float)(m_pBottleMorph.iFloatRange / 100.0), m_oBottleMorph.fWidth1, 0, false, !bCalib))
				{
					sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
					//第一条水平线定位失败
					sRsu.strDescription = "Failure to locate the first horizon line!";
					
					CopyObj(regLine1,out sRsu.regError, 1, -1);
					return sRsu;
				}
				m_tBottleMorph.sHoriLine1.iCol1 = tpCols[0];
				m_tBottleMorph.sHoriLine1.iCol2 = tpCols[1];
				m_tBottleMorph.sHoriLine1.iRow1 = tpRows[0];
				m_tBottleMorph.sHoriLine1.iRow2 = tpRows[1];
				m_tBottleMorph.sHoriLine1.iDistance = m_tBottleMorph.sHoriLine1.iCol2 - m_tBottleMorph.sHoriLine1.iCol1;
				if (bDebug)
				{


					GenCircle(out regCircle, m_tBottleMorph.sHoriLine1.iRow1, m_tBottleMorph.sHoriLine1.iCol1, 5);
					ConcatObj(m_mBottleMorph.regCheck, regCircle, out m_mBottleMorph.regCheck);
					GenCircle(out regCircle, m_tBottleMorph.sHoriLine1.iRow2, m_tBottleMorph.sHoriLine1.iCol2, 5);
					ConcatObj(m_mBottleMorph.regCheck, regCircle, out m_mBottleMorph.regCheck)
				}
				if (bCalib)
				{
					m_oBottleMorph.fWidth1 = m_tBottleMorph.sHoriLine1.iDistance;
				}
				//第二条线
			//	gen_region_line(&regLine2, Row1 + (Row2 - Row1) * 2 / 3, Col1, Row1 + (Row2 - Row1) * 2 / 3, Col2);
				GenRegionLine( out regLine2, Row1 + (Row2 - Row1) * 2 / 3, Col1, Row1 + (Row2 - Row1) * 2 / 3, Col2);
				if (bDebug)
				{
					//concat_obj(m_mBottleMorph.regCheck, regLine2, &m_mBottleMorph.regCheck);
					ConcatObj(m_mBottleMorph.regCheck, regLine2, out m_mBottleMorph.regCheck);
				}
				if (!findEdgePointDoubleEx(hImg, regLine2, out tpRows, out tpCols, m_pBottleMorph.iEdge,(float)(m_pBottleMorph.iFloatRange / 100.0), m_oBottleMorph.fWidth2, 0, false, !bCalib))
				{
					sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
					//第二条水平线定位失败
					sRsu.strDescription = "Failure to locate the second horizon line!";
				
					CopyObj(regLine2, out sRsu.regError, 1, -1);
					return sRsu;
				}
				m_tBottleMorph.sHoriLine2.iCol1 = tpCols[0];
				m_tBottleMorph.sHoriLine2.iCol2 = tpCols[1];
				m_tBottleMorph.sHoriLine2.iRow1 = tpRows[0];
				m_tBottleMorph.sHoriLine2.iRow2 = tpRows[1];
				m_tBottleMorph.sHoriLine2.iDistance = m_tBottleMorph.sHoriLine2.iCol2 - m_tBottleMorph.sHoriLine2.iCol1;
				if (bDebug)
				{
				


					GenCircle(out regCircle, m_tBottleMorph.sHoriLine2.iRow1, m_tBottleMorph.sHoriLine2.iCol1, 5);
					ConcatObj(m_mBottleMorph.regCheck, regCircle, out m_mBottleMorph.regCheck);
					GenCircle(out regCircle, m_tBottleMorph.sHoriLine2.iRow2, m_tBottleMorph.sHoriLine2.iCol2, 5);
					ConcatObj(m_mBottleMorph.regCheck, regCircle, out m_mBottleMorph.regCheck);
				}
				//标定，两条线结束
				if (bCalib)
				{
					m_oBottleMorph.fWidth2 = m_tBottleMorph.sHoriLine2.iDistance;
					return sRsu;
				}
				//第三条线
				MidRow1 = (m_tBottleMorph.sHoriLine1.iRow1 + m_tBottleMorph.sHoriLine1.iRow2) / 2.0;
				MidCol1 = (m_tBottleMorph.sHoriLine1.iCol1 + m_tBottleMorph.sHoriLine1.iCol2) / 2.0;
				MidRow2 = (m_tBottleMorph.sHoriLine2.iRow1 + m_tBottleMorph.sHoriLine2.iRow2) / 2.0;
				MidCol2 = (m_tBottleMorph.sHoriLine2.iCol1 + m_tBottleMorph.sHoriLine2.iCol2) / 2.0;
				
				LineOrientation(MidRow1, MidCol1, MidRow2, MidCol2, out OriAngle);
				if (OriAngle < 0)
				{
					OriAngle = PI + OriAngle;
				}
			//	smallest_rectangle1(m_oBottleMorph.regVerticalLine, &Row1, &Col1, &Row2, &Col2);
				SmallestRectangle1(m_oBottleMorph.regVerticalLine, out Row1, out Col1, out Row2, out Col2);
				double fCol1, fCol2;
				if (0 == MidRow2 - MidRow1)
				{
					fCol1 = (MidCol1 + MidCol2) / 2.0;
					fCol2 = (MidCol1 + MidCol2) / 2.0;
				}
				else
				{
					fCol1 = MidCol1 + (MidCol2 - MidCol1) * (Row1 - MidRow1) / (MidRow2 - MidRow1);
					fCol2 = MidCol1 + (MidCol2 - MidCol1) * (Row2 - MidRow1) / (MidRow2 - MidRow1);
				}

				HTuple nWidth, nHeight;
				HTuple pointer,type;
				//get_image_pointer1(hImg, NULL, NULL, &nWidth, &nHeight);
				GetImagePointer1(hImg,out pointer,out type,out nWidth,out nHeight);
				if (fCol1 < 0 || fCol1 > nWidth - 1 || fCol2 < 0 || fCol2 > nWidth - 1)
				{
					sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
					//竖直线定位失败
					sRsu.strDescription = "Failure to locate the vertical line!";
					//gen_rectangle1(&sRsu.regError, 20, 20, 120, 120);
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
					return sRsu;
				}
				
				GenRegionLine(out regLine3, Row1, fCol1, Row2, fCol2);
				// 由上到下，寻找从亮到暗的边界点
				//2014.12.19 第三条线暂用固定的对比度8
				if (findEdgePointSingleEx(hImg, regLine3, out tpRows, out tpCols, 10, 40, 8, T2B, 0, false) != 1)
				{
					sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
					//竖直线定位失败
					sRsu.strDescription = "Failure to locate the vertical line!";
					
					CopyObj(regLine3, out sRsu.regError, 1, -1);
					return sRsu;
				}
				m_oBottleMorph.sCurrentOri.dOriRow = tpRows[0];
				m_oBottleMorph.sCurrentOri.dOriCol = tpCols[0];
				m_oBottleMorph.sCurrentOri.dOriPhi = OriAngle;
				m_tBottleMorph.dVertSize = nHeight - m_oBottleMorph.sCurrentOri.dOriRow;
				if (bDebug)
				{
					
					GenCircle(out regCircle, m_oBottleMorph.sCurrentOri.dOriRow, m_oBottleMorph.sCurrentOri.dOriCol, 5);
					ConcatObj(m_mBottleMorph.regCheck, regCircle, out m_mBottleMorph.regCheck);
				}
				//更新原点：检测测试和保存模板时，需更新模板原点。实际检测时不更新
				if (bUpdateOri)
				{
					m_oBottleMorph.sModelOri = m_oBottleMorph.sCurrentOri;
				}
			}
			return sRsu;
		}


	}
}
