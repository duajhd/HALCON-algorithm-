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
	//满瓶瓶盖检测参数
	public struct s_pFaceLabel
	{
		//定位
		public int iLocMethod;             //定位方法 0、（垂直两侧定位）1、（垂直单侧定位）
		public bool bLocate;               //定位
		public int iEdge;                  //对比度
		public int iFloatRange;
		//位置信息
		public bool bLabelPosition;
		public float fLeftMin1;            //左1
		public float fLeftMax1;
		public float fLeftMin2;            //左2
		public float fLeftMax2;
		public float fRightMin1;       //右1
		public float fRightMax1;
		public float fRightMin2;       //右2
		public float fRightMax2;
		public float fTopMin1;         //上1
		public float fTopMax1;
		public float fTopMin2;         //上1
		public float fTopMax2;
		public float fBottomMin1;      //下2
		public float fBottomMax1;
		public float fBottomMin2;      //下2
		public float fBottomMax2;
		public float fWidthMin1;       //宽1
		public float fWidthMax1;
		public float fWidthMin2;       //宽2
		public float fWidthMax2;
		public float fHeightMin1;      //高1
		public float fHeightMax1;
		public float fHeightMin2;      //高2
		public float fHeightMax2;
		public float fPhiMin1;         //角度1
		public float fPhiMax1;
		public float fPhiMin2;         //角度2
		public float fPhiMax2;

		//字符检测
		//模板定位
		public bool bPMTool;
		public s_pPMToolPara spPMToolPara; //PM定位
		public float fScore;           //分值
									   //背景
		public bool bBackGround;       //去除背景
		public s_pPMToolPara spBackGroundPMToolPara;   //背景定位
		public int iBackGroundGraySub; //背景灰度差
		public int iBackGroundOpenWidth;
		public int iBackGroundOpenHeight;
		public float fBackGroundScore;
		//OCR识别
		public bool bOCR;
		public s_pOCRPara spOCRPara;   //字符识别
									   //扣盖
		public bool bCover;
		public int iCoverEdge;
		public float fCoverSizeMin;
		public float fCoverSizeMax;
		//检测头发
		public bool bHair;
		public int iHairLength;
	};
	
	//图形参数
	public struct s_oFaceLabel
	{
		public HObject regHorLine1, regHorLine2;//水平线1、2
		public HObject regVerLine1, regVerLine2;//垂直线1、2
		public float fVerLinePos1, fVerLinePos2;//垂直线位置
		public s_MyOri sModelOri;          //标签模板原点
		public s_MyOri sCurrentOri;        //标签当前原点

		public s_MyOri sCharModelOri;      //字符模板原点
		public s_MyOri sCharCurrentOri;    //字符当前原点

		public s_MyOri sBackGroundModelOri;//背景模板原点
		public s_MyOri sBackGroundCurrentOri;//背景当前原点

		public float fWidth1;  //判断连瓶
		public float fWidth2;

		public int iStdWidth1, iStdWidth2, iStdWidth3, iStdWidth4;
		public int iStdHeight1, iStdHeight2, iStdHeight3, iStdHeight4;
		public float fStdWidthMin, fStdWidthMax;
		public float fStdHeightMin, fStdHeightMax;

		public HObject regPMSearchRegion, AregPMSearchRegion;  //搜索区域
		public HObject regPMModelRegion;                       //模板区域
		public HTuple lLocModelID;                          //定位模板ID

		public HTuple lBackGroundModelID;                       //背景模板ID
		public HObject imgBackGround;                          //背景图像
		public HObject regBackGroundModelRegion;               //背景模板区域

		public HObject regCodePosRegion, AregCodePosRegion, SregCodePosRegion; //字符位置//注：位置参数中s读取时读出来，保存时存进去
		public HObject regCharRegion, AregCharRegion;  //字符区域
		public HObject regCoverRegion, AregCoverRegion;//扣盖区域
	};



	//内部参数，仅用于储存内部检测临时数据
	public struct s_tFaceLabel
	{
		public HObject ImgCode;
		public HObject regCode;
		public s_LineLocParaEx sHoriLine1;
		public s_LineLocParaEx sHoriLine2;
		public s_LineLocPara sVeriLine1;
		public s_LineLocPara sVeriLine2;
		public s_LineLocPara sCoverRegion;
		public double fWidth1, fWidth2;        //宽度
		public double fHeight1, fHeight2;  //高度
		public double fPhi1, fPhi2;            //角度
		public double fLeft1, fRight1, fTop1, fBottom1;//边距
		public double fLeft2, fRight2, fTop2, fBottom2;//边距
		public double fCoverSize;          //扣盖宽
		_tFaceLabel()
		{
			fWidth1 = fWidth2 = 0;
			fHeight1 = fHeight2 = 0;
			fPhi1 = fPhi2 = 0;
			fLeft1 = fRight1 = fTop1 = fBottom1 = 0;
			fLeft2 = fRight2 = fTop2 = fBottom2 = 0;
		}
	};

	//调试中间结果图像（仅调试时使用，不同类型绘制不同颜色）
	public struct s_mFaceLabel
	{
		public HObject regCheck;       //检测出的结果
		public HObject regCorrect;     //修正结果
	};

	public class CCheckFaceLabel:CommonCheck
    {

		public s_pFaceLabel m_pFaceLabel;  //数字参数
		public s_oFaceLabel m_oFaceLabel;  //图像参数
		public s_tFaceLabel m_tFaceLabel;  //临时参数
		public s_mFaceLabel m_mFaceLabel;  //中间结果（调试时使用）
		public CCheckFaceLabel()
        {

        }
        ~CCheckFaceLabel()
        {

        }

		public void readParam()
        {

        }

		public void writeParam()
        {

        }

		public void displayShape(HTuple lWindID)//显示区域
		{
			//显示模板区域
			if (m_pFaceLabel.bLocate)
			{


				SetColor(lWindID,"green");
				DispObj(m_oFaceLabel.regHorLine1, lWindID);
				DispObj(m_oFaceLabel.regHorLine2, lWindID);
				DispObj(m_oFaceLabel.regVerLine1, lWindID);
				DispObj(m_oFaceLabel.regVerLine2, lWindID);
				//位置信息
				if (m_pFaceLabel.bLabelPosition)
				{
					HTuple rowCent1, colCent1, rowCent2, colCent2;
					String strCurDist;
					//宽
					rowCent1 = (m_tFaceLabel.sHoriLine1.sLineInner.iRow1 + m_tFaceLabel.sHoriLine1.sLineInner.iRow2) / 2;
					colCent1 = (m_tFaceLabel.sHoriLine1.sLineInner.iCol1 + m_tFaceLabel.sHoriLine1.sLineInner.iCol2) / 2;
					rowCent2 = (m_tFaceLabel.sHoriLine2.sLineInner.iRow1 + m_tFaceLabel.sHoriLine2.sLineInner.iRow2) / 2;
					colCent2 = (m_tFaceLabel.sHoriLine2.sLineInner.iCol1 + m_tFaceLabel.sHoriLine2.sLineInner.iCol2) / 2;


					SetColor(lWindID, "yellow");
					DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sHoriLine1.sLineInner.iRow1, m_tFaceLabel.sHoriLine1.sLineInner.iCol1, 3);
					DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sHoriLine1.sLineInner.iRow2, m_tFaceLabel.sHoriLine1.sLineInner.iCol2, 3);
					DispArrow(lWindID, rowCent2, colCent2, m_tFaceLabel.sHoriLine2.sLineInner.iRow1, m_tFaceLabel.sHoriLine2.sLineInner.iCol1, 3);
					DispArrow(lWindID, rowCent2, colCent2, m_tFaceLabel.sHoriLine2.sLineInner.iRow2, m_tFaceLabel.sHoriLine2.sLineInner.iCol2, 3);

					//显示尺寸数据
					set_color(lWindID, "navy");
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fWidth1, 'f', 0));
					set_tposition(lWindID, rowCent1, colCent1);
					write_string(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fWidth2, 'f', 0));
					set_tposition(lWindID, rowCent2, colCent2);
					write_string(lWindID, strCurDist.toLocal8Bit().constData());

					//显示尺寸数据
					SetColor(lWindID, "navy");
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fWidth1, 'f', 0));
					SetTposition(lWindID, rowCent1, colCent1);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fWidth2, 'f', 0));
					SetTposition(lWindID, rowCent2, colCent2);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());

					//高
					rowCent1 = (m_tFaceLabel.sVeriLine1.iRow1 + m_tFaceLabel.sVeriLine1.iRow2) / 2;
					colCent1 = (m_tFaceLabel.sVeriLine1.iCol1 + m_tFaceLabel.sVeriLine1.iCol2) / 2;
					rowCent2 = (m_tFaceLabel.sVeriLine2.iRow1 + m_tFaceLabel.sVeriLine2.iRow2) / 2;
					colCent2 = (m_tFaceLabel.sVeriLine2.iCol1 + m_tFaceLabel.sVeriLine2.iCol2) / 2;
			

					SetColor(lWindID, "yellow");
					DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sVeriLine1.iRow1, m_tFaceLabel.sVeriLine1.iCol1, 3);
					DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sVeriLine1.iRow2, m_tFaceLabel.sVeriLine1.iCol2, 3);
					DispArrow(lWindID, rowCent2, colCent2, m_tFaceLabel.sVeriLine2.iRow1, m_tFaceLabel.sVeriLine2.iCol1, 3);
					DispArrow(lWindID, rowCent2, colCent2, m_tFaceLabel.sVeriLine2.iRow2, m_tFaceLabel.sVeriLine2.iCol2, 3);
					//显示尺寸数据
					set_color(lWindID, "navy");
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fHeight1, 'f', 0));
					set_tposition(lWindID, rowCent1, colCent1);
					write_string(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fHeight2, 'f', 0));
					set_tposition(lWindID, rowCent2, colCent2);
					write_string(lWindID, strCurDist.toLocal8Bit().constData());
					//显示尺寸数据
					SetColor(lWindID, "navy");
					//这里的arg就是前面的1%占位符
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fHeight1, 'f', 0));
					SetTposition(lWindID, rowCent1, colCent1);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = 
					SetTposition(lWindID, rowCent2, colCent2);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());


					//角度
					rowCent1 = (m_tFaceLabel.sVeriLine1.iRow1 + m_tFaceLabel.sVeriLine2.iRow1) / 2;
					colCent1 = (m_tFaceLabel.sVeriLine1.iCol1 + m_tFaceLabel.sVeriLine2.iCol1) / 2;
					rowCent2 = (m_tFaceLabel.sVeriLine1.iRow2 + m_tFaceLabel.sVeriLine2.iRow2) / 2;
					colCent2 = (m_tFaceLabel.sVeriLine1.iCol2 + m_tFaceLabel.sVeriLine2.iCol2) / 2;


					SetColor(lWindID, "navy");
					strCurDist = QString("%1 °").arg(QString::number(m_tFaceLabel.fPhi1, 'f', 2));
					SetTposition(lWindID, rowCent1, colCent1);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 °").arg(QString::number(m_tFaceLabel.fPhi2, 'f', 2));
					SetTposition(lWindID, rowCent2, colCent2);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());





					//左
					rowCent1 = (m_tFaceLabel.sHoriLine1.sLineInner.iRow1 + m_tFaceLabel.sHoriLine1.sLineOuter.iRow1) / 2;
					colCent1 = (m_tFaceLabel.sHoriLine1.sLineInner.iCol1 + m_tFaceLabel.sHoriLine1.sLineOuter.iCol1) / 2;
					rowCent2 = (m_tFaceLabel.sHoriLine2.sLineInner.iRow1 + m_tFaceLabel.sHoriLine2.sLineOuter.iRow1) / 2;
					colCent2 = (m_tFaceLabel.sHoriLine2.sLineInner.iCol1 + m_tFaceLabel.sHoriLine2.sLineOuter.iCol1) / 2;
					
					SetColor(lWindID, "yellow");
					DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sHoriLine1.sLineInner.iRow1, m_tFaceLabel.sHoriLine1.sLineInner.iCol1, 3);
					DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sHoriLine1.sLineOuter.iRow1, m_tFaceLabel.sHoriLine1.sLineOuter.iCol1, 3);
					DispArrow(lWindID, rowCent2, colCent2, m_tFaceLabel.sHoriLine2.sLineInner.iRow1, m_tFaceLabel.sHoriLine2.sLineInner.iCol1, 3);
					DispArrow(lWindID, rowCent2, colCent2, m_tFaceLabel.sHoriLine2.sLineOuter.iRow1, m_tFaceLabel.sHoriLine2.sLineOuter.iCol1, 3);

					//显示尺寸数据
				
					SetColor(lWindID, "navy");
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fLeft1, 'f', 0));
					SetTposition(lWindID, rowCent1, colCent1);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fLeft2, 'f', 0));
					SetTposition(lWindID, rowCent2, colCent2);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());


					//右
					rowCent1 = (m_tFaceLabel.sHoriLine1.sLineInner.iRow2 + m_tFaceLabel.sHoriLine1.sLineOuter.iRow2) / 2;
					colCent1 = (m_tFaceLabel.sHoriLine1.sLineInner.iCol2 + m_tFaceLabel.sHoriLine1.sLineOuter.iCol2) / 2;
					rowCent2 = (m_tFaceLabel.sHoriLine2.sLineInner.iRow2 + m_tFaceLabel.sHoriLine2.sLineOuter.iRow2) / 2;
					colCent2 = (m_tFaceLabel.sHoriLine2.sLineInner.iCol2 + m_tFaceLabel.sHoriLine2.sLineOuter.iCol2) / 2;
					

					SetColor(lWindID, "yellow");
					DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sHoriLine1.sLineInner.iRow2, m_tFaceLabel.sHoriLine1.sLineInner.iCol2, 3);
					DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sHoriLine1.sLineOuter.iRow2, m_tFaceLabel.sHoriLine1.sLineOuter.iCol2, 3);
					DispArrow(lWindID, rowCent2, colCent2, m_tFaceLabel.sHoriLine2.sLineInner.iRow2, m_tFaceLabel.sHoriLine2.sLineInner.iCol2, 3);
					DispArrow(lWindID, rowCent2, colCent2, m_tFaceLabel.sHoriLine2.sLineOuter.iRow2, m_tFaceLabel.sHoriLine2.sLineOuter.iCol2, 3);
					//显示尺寸数据
				

					SetColor(lWindID, "navy");
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fRight1, 'f', 0));
					SetTposition(lWindID, rowCent1, colCent1);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fRight2, 'f', 0)); ;
					SetTposition(lWindID, rowCent2, colCent2);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());


					//上下
					set_color(lWindID, "navy");
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fTop1, 'f', 0));
					set_tposition(lWindID, m_tFaceLabel.sVeriLine1.iRow1, m_tFaceLabel.sVeriLine1.iCol1);
					write_string(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fBottom1, 'f', 0));
					set_tposition(lWindID, m_tFaceLabel.sVeriLine1.iRow2, m_tFaceLabel.sVeriLine1.iCol2);
					write_string(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fTop2, 'f', 0));
					set_tposition(lWindID, m_tFaceLabel.sVeriLine2.iRow1, m_tFaceLabel.sVeriLine2.iCol1);
					write_string(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fBottom2, 'f', 0));
					set_tposition(lWindID, m_tFaceLabel.sVeriLine2.iRow2, m_tFaceLabel.sVeriLine2.iCol2);
					write_string(lWindID, strCurDist.toLocal8Bit().constData());

					SetColor(lWindID, "navy");
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fTop1, 'f', 0));
					SetTposition(lWindID, m_tFaceLabel.sVeriLine1.iRow1, m_tFaceLabel.sVeriLine1.iCol1);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fBottom1, 'f', 0));
					SetTposition(lWindID, m_tFaceLabel.sVeriLine1.iRow2, m_tFaceLabel.sVeriLine1.iCol2);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fTop2, 'f', 0));
					SetTposition(lWindID, m_tFaceLabel.sVeriLine2.iRow1, m_tFaceLabel.sVeriLine2.iCol1);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
					strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fBottom2, 'f', 0));
					SetTposition(lWindID, m_tFaceLabel.sVeriLine2.iRow2, m_tFaceLabel.sVeriLine2.iCol2);
					WriteString(lWindID, strCurDist.toLocal8Bit().constData());
				}
			}
			//模板匹配
			if (m_pFaceLabel.bPMTool)
			{
				

				SetColor(lWindID, "green");
				DispObj(m_oFaceLabel.AregPMSearchRegion, lWindID);
				DispObj(m_oFaceLabel.regPMModelRegion, lWindID);


			}
			if (m_pFaceLabel.bBackGround)
			{
			

				SetColor(lWindID, "blue");
				DispObj(m_oFaceLabel.regBackGroundModelRegion, lWindID);
			}
			//字符
			if (m_pFaceLabel.bOCR)
			{
				

				SetColor(lWindID, "green");
				DispObj(m_oFaceLabel.AregCharRegion, lWindID);
				//记忆
				if (0 == m_pFaceLabel.spOCRPara.spOCRPublicPara.iPosMethod)
				{
					

					SetColor(lWindID, "blue");
					DispObj(m_oFaceLabel.AregCodePosRegion, lWindID);
				}
			}
			//扣盖
			if (m_pFaceLabel.bCover)
			{
				

				SetColor(lWindID, "green");
				DispObj(m_oFaceLabel.AregCoverRegion, lWindID);

				HTuple rowCent1, colCent1, rowCent2, colCent2;
				String strCurDist;
				//宽
				rowCent1 = (m_tFaceLabel.sCoverRegion.iRow1 + m_tFaceLabel.sCoverRegion.iRow2) / 2;
				colCent1 = (m_tFaceLabel.sCoverRegion.iCol1 + m_tFaceLabel.sCoverRegion.iCol2) / 2;
			

				SetColor(lWindID, "yellow");
				DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sCoverRegion.iRow1, m_tFaceLabel.sCoverRegion.iCol1, 3);
				DispArrow(lWindID, rowCent1, colCent1, m_tFaceLabel.sCoverRegion.iRow2, m_tFaceLabel.sCoverRegion.iCol2, 3);
				//显示尺寸数据
				
				SetColor(lWindID, "navy");
				strCurDist = QString("%1 pix").arg(QString::number(m_tFaceLabel.fCoverSize, 'f', 0));
				SetTposition(lWindID, rowCent1 - 50, colCent1);
				WriteString(lWindID, strCurDist.toLocal8Bit().constData());
			}
			//显示调试图像
			if (g_iUserPermit > 5)
			{
				

				SetColor(lWindID, "yellow");
				DispObj(m_mFaceLabel.regCheck, lWindID);
				SetColor(lWindID, "blue");
				DispObj(m_mFaceLabel.regCorrect, lWindID);

			}
		}

		public s_CheckResult location(HObject hImg, bool bDebug = false, bool bUpdateOri = false, bool bCalib = false)     //定位
        {

        }


		public s_CheckResult check(HObject hImg, bool bDebug = false)
		{

		}

		public void affineShape()
        {
			
            if (m_pFaceLabel.bLocate)
            {
				HTuple homMat2DIdentity;
				HomMat2dIdentity(out homMat2DIdentity);
				VectorAngleToRigid(m_oFaceLabel.sModelOri.dOriRow, m_oFaceLabel.sModelOri.dOriCol, 0,
					m_oFaceLabel.sCurrentOri.dOriRow, m_oFaceLabel.sCurrentOri.dOriCol, 0, out homMat2DIdentity);
				//此处仅移动不旋转
				AffineTransRegion(m_oFaceLabel.regCoverRegion, out m_oFaceLabel.AregCoverRegion, homMat2DIdentity, "false");
				AffineTransRegion(m_oFaceLabel.regPMSearchRegion, out m_oFaceLabel.AregPMSearchRegion, homMat2DIdentity, "false");

			}


			else
			{
				
				CopyObj(m_oFaceLabel.regCoverRegion, out m_oFaceLabel.AregCoverRegion, 1, -1);
				CopyObj(m_oFaceLabel.regPMSearchRegion, out m_oFaceLabel.AregPMSearchRegion, 1, -1);
			}
		}

		public void affineShape1()
        {

        }
		//*功能重载：处理字符区域
		public void processCharRegion(HObject regChar)
        {

        }

		//标签位置
		public	s_CheckResult MeasureLabel()
        {

        }
		//标签定位
		public s_CheckResult LabelLocation(HObject hImg, bool bDebug = false, bool bUpdateOri = false, bool bCalib = false)
        {
			s_CheckResult sRsu = new s_CheckResult();
            try
            {
				HObject regLine, regCircle;
				HTuple tpRows, tpCols;
				HTuple Row1, Col1, Row2, Col2;
				double MidRow1, MidCol1, MidRow2, MidCol2, OriAngle;
				float fLeftCol, fRightCol;
				float fTopRow, fBottomRow;
				float fPos1, fPos2;
				HTuple fPhi1, fPhi2;
				//如果确实使用线定位
				if (m_pFaceLabel.bLocate)
                {
					//定位第一条线
					//第一条线定位失败
					//
					if(!findEdgePointDoubleEx(hImg, m_oFaceLabel.regHorLine1, out tpRows, out tpCols, m_pFaceLabel.iEdge, m_pFaceLabel.iFloatRange / 100.0, m_oFaceLabel.fWidth1, 0, false))
                    {
						CopyObj(m_oFaceLabel.regHorLine1, out sRsu.regError, 1, -1);
						return sRsu;
                    }
                    else
                    {
						m_tFaceLabel.sHoriLine1.sLineOuter.iCol1 = tpCols[0];
						m_tFaceLabel.sHoriLine1.sLineOuter.iCol2 = tpCols[1];
						m_tFaceLabel.sHoriLine1.sLineOuter.iRow1 = tpRows[0];
						m_tFaceLabel.sHoriLine1.sLineOuter.iRow2 = tpRows[1];
						m_tFaceLabel.sHoriLine1.sLineOuter.iDistance = m_tFaceLabel.sHoriLine1.sLineOuter.iCol2 - m_tFaceLabel.sHoriLine1.sLineOuter.iCol1;

						if (bDebug)
						{


							GenCircle(out regCircle, m_tFaceLabel.sHoriLine1.sLineOuter.iRow1, m_tFaceLabel.sHoriLine1.sLineOuter.iCol1, 5);
							ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);
							GenCircle(out regCircle, m_tFaceLabel.sHoriLine1.sLineOuter.iRow2, m_tFaceLabel.sHoriLine1.sLineOuter.iCol2, 5);
							ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);
						}
						if (bCalib)
						{
							m_oFaceLabel.fWidth2 = m_tFaceLabel.sHoriLine2.sLineOuter.iDistance;
						}

						GenRegionLine(out regLine, m_tFaceLabel.sHoriLine2.sLineOuter.iRow1, m_tFaceLabel.sHoriLine2.sLineOuter.iCol1,
						m_tFaceLabel.sHoriLine2.sLineOuter.iRow2, m_tFaceLabel.sHoriLine2.sLineOuter.iCol2);
						if (findEdgePointDouble(hImg, regLine, out tpRows, out tpCols, m_pFaceLabel.iEdge, L2R, 1, false) < 2)
						{

							CopyObj(regLine, out sRsu.regError, 1, -1);
							return sRsu;
						}
						else
						{
							m_tFaceLabel.sHoriLine2.sLineInner.iCol1 = tpCols[0];
							m_tFaceLabel.sHoriLine2.sLineInner.iCol2 = tpCols[1];
							m_tFaceLabel.sHoriLine2.sLineInner.iRow1 = tpRows[0];
							m_tFaceLabel.sHoriLine2.sLineInner.iRow2 = tpRows[1];
							m_tFaceLabel.sHoriLine2.sLineInner.iDistance = m_tFaceLabel.sHoriLine2.sLineInner.iCol2 - m_tFaceLabel.sHoriLine2.sLineInner.iCol1;
							if (bDebug)
							{


								GenCircle(out regCircle, m_tFaceLabel.sHoriLine2.sLineInner.iRow1, m_tFaceLabel.sHoriLine2.sLineInner.iCol1, 5);
								ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);
								GenCircle(out regCircle, m_tFaceLabel.sHoriLine2.sLineInner.iRow2, m_tFaceLabel.sHoriLine2.sLineInner.iCol2, 5);
								ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);

							}
						}
					}
					//第二条线
					//第二条水平线定位失败
					sRsu.iType = ERROR_LOCATEFAIL;
					sRsu.strDescription = QObject::tr("Failure to locate the second horizon line!");
					if (!findEdgePointDoubleEx(hImg, m_oFaceLabel.regHorLine2, &tpRows, &tpCols, m_pFaceLabel.iEdge, m_pFaceLabel.iFloatRange / 100.0, m_oFaceLabel.fWidth2, 0, false))
					{
						CopyObj(m_oFaceLabel.regHorLine2, out sRsu.regError, 1, -1);
						return sRsu;
					}
					else
					{
						m_tFaceLabel.sHoriLine2.sLineOuter.iCol1 = tpCols[0];
						m_tFaceLabel.sHoriLine2.sLineOuter.iCol2 = tpCols[1];
						m_tFaceLabel.sHoriLine2.sLineOuter.iRow1 = tpRows[0];
						m_tFaceLabel.sHoriLine2.sLineOuter.iRow2 = tpRows[1];
						m_tFaceLabel.sHoriLine2.sLineOuter.iDistance = m_tFaceLabel.sHoriLine2.sLineOuter.iCol2 - m_tFaceLabel.sHoriLine2.sLineOuter.iCol1;
						if (bDebug)
						{
							gen_circle(&regCircle, m_tFaceLabel.sHoriLine2.sLineOuter.iRow1, m_tFaceLabel.sHoriLine2.sLineOuter.iCol1, 5);
							concat_obj(m_mFaceLabel.regCheck, regCircle, &m_mFaceLabel.regCheck);
							gen_circle(&regCircle, m_tFaceLabel.sHoriLine2.sLineOuter.iRow2, m_tFaceLabel.sHoriLine2.sLineOuter.iCol2, 5);
							concat_obj(m_mFaceLabel.regCheck, regCircle, &m_mFaceLabel.regCheck);
						}
						if (bCalib)
						{
							m_oFaceLabel.fWidth2 = m_tFaceLabel.sHoriLine2.sLineOuter.iDistance;
						}
						gen_region_line(&regLine, m_tFaceLabel.sHoriLine2.sLineOuter.iRow1, m_tFaceLabel.sHoriLine2.sLineOuter.iCol1,
							m_tFaceLabel.sHoriLine2.sLineOuter.iRow2, m_tFaceLabel.sHoriLine2.sLineOuter.iCol2);
						if (findEdgePointDouble(hImg, regLine, &tpRows, &tpCols, m_pFaceLabel.iEdge, L2R, 1, false) < 2)
						{
							copy_obj(regLine, &sRsu.regError, 1, -1);
							return sRsu;
						}
						else
						{
							m_tFaceLabel.sHoriLine2.sLineInner.iCol1 = tpCols[0];
							m_tFaceLabel.sHoriLine2.sLineInner.iCol2 = tpCols[1];
							m_tFaceLabel.sHoriLine2.sLineInner.iRow1 = tpRows[0];
							m_tFaceLabel.sHoriLine2.sLineInner.iRow2 = tpRows[1];
							m_tFaceLabel.sHoriLine2.sLineInner.iDistance = m_tFaceLabel.sHoriLine2.sLineInner.iCol2 - m_tFaceLabel.sHoriLine2.sLineInner.iCol1;
							if (bDebug)
							{


								GenCircle(out regCircle, m_tFaceLabel.sHoriLine2.sLineInner.iRow1, m_tFaceLabel.sHoriLine2.sLineInner.iCol1, 5);
								ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);
								GenCircle(out regCircle, m_tFaceLabel.sHoriLine2.sLineInner.iRow2, m_tFaceLabel.sHoriLine2.sLineInner.iCol2, 5);
								ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);

							}
						}
					}

					//可确定纵向线
					fLeftCol = (m_tFaceLabel.sHoriLine1.sLineInner.iCol1 + m_tFaceLabel.sHoriLine2.sLineInner.iCol1) / 2;
					fRightCol = (m_tFaceLabel.sHoriLine1.sLineInner.iCol2 + m_tFaceLabel.sHoriLine2.sLineInner.iCol2) / 2;
					if (!bUpdateOri)
					{


						SmallestRectangle1(m_oFaceLabel.regVerLine1, out Row1, out Col1, out Row2, out Col2);
						GenRegionLine(out m_oFaceLabel.regVerLine1, Row1, fLeftCol + m_oFaceLabel.fVerLinePos1 * (fRightCol - fLeftCol),
							Row2, fLeftCol + m_oFaceLabel.fVerLinePos1 * (fRightCol - fLeftCol));
						SmallestRectangle1(m_oFaceLabel.regVerLine2, out Row1, out Col1, out Row2, out Col2);
						GenRegionLine(out m_oFaceLabel.regVerLine2, Row1, fLeftCol + m_oFaceLabel.fVerLinePos2 * (fRightCol - fLeftCol),
							Row2, fLeftCol + m_oFaceLabel.fVerLinePos2 * (fRightCol - fLeftCol));

					}
					else
					{
						if (0 == fRightCol - fLeftCol)
						{
							fPos1 = (float)0.3;
							fPos2 = (float)0.7;
						}
						else
						{

							SmallestRectangle1(m_oFaceLabel.regVerLine1, out Row1, out Col1, out Row2, out Col2);
							fPos1 = ((Col1 + Col2) / 2 - fLeftCol) / (fRightCol - fLeftCol);
							if (fPos1 <= 0 || fPos1 >= 1)
							{
								fPos1 = 0.3;
							}

							GenRegionLine(out m_oFaceLabel.regVerLine1, Row1, fLeftCol + fPos1 * (fRightCol - fLeftCol),
								Row2, fLeftCol + fPos1 * (fRightCol - fLeftCol));

							SmallestRectangle1(m_oFaceLabel.regVerLine2, out Row1, out Col1, out Row2, out Col2);
							fPos2 = ((Col1 + Col2) / 2 - fLeftCol) / (fRightCol - fLeftCol);
							if (fPos2 <= 0 || fPos2 >= 1)
							{
								fPos2 = 0.7;
							}

							GenRegionLine(out m_oFaceLabel.regVerLine2, Row1, fLeftCol + fPos2 * (fRightCol - fLeftCol),
								Row2, fLeftCol + fPos2 * (fRightCol - fLeftCol));
						}



					}

					//第三条线
					sRsu.iType = ERROR_LOCATEFAIL;
					sRsu.strDescription = QObject::tr("Failure to locate the first vertical line!");
					if (findEdgePointDouble(hImg, m_oFaceLabel.regVerLine1, out tpRows, out tpCols, m_pFaceLabel.iEdge, T2B, 1, false) < 2)
					{

						CopyObj(m_oFaceLabel.regVerLine1, out sRsu.regError, 1, -1);
						return sRsu;
					}
					else
					{
						m_tFaceLabel.sVeriLine1.iCol1 = tpCols[0];
						m_tFaceLabel.sVeriLine1.iCol2 = tpCols[1];
						m_tFaceLabel.sVeriLine1.iRow1 = tpRows[0];
						m_tFaceLabel.sVeriLine1.iRow2 = tpRows[1];
						m_tFaceLabel.sVeriLine1.iDistance = m_tFaceLabel.sVeriLine1.iRow2 - m_tFaceLabel.sVeriLine1.iRow1;
						if (bDebug)
						{

							GenCircle(out regCircle, m_tFaceLabel.sVeriLine1.iRow1, m_tFaceLabel.sVeriLine1.iCol1, 5);
							ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);
							GenCircle(out regCircle, m_tFaceLabel.sVeriLine1.iRow2, m_tFaceLabel.sVeriLine1.iCol2, 5);
							ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);
						}
					}


					//第四条线
					sRsu.iType = ERROR_LOCATEFAIL;
					sRsu.strDescription = QObject::tr("Failure to locate the second vertical line!");
					if (findEdgePointDouble(hImg, m_oFaceLabel.regVerLine2, out tpRows, out tpCols, m_pFaceLabel.iEdge, T2B, 1, false) < 2)
					{

						CopyObj(m_oFaceLabel.regVerLine2, out sRsu.regError, 1, -1);
						return sRsu;
					}
					else
					{
						m_tFaceLabel.sVeriLine2.iCol1 = tpCols[0];
						m_tFaceLabel.sVeriLine2.iCol2 = tpCols[1];
						m_tFaceLabel.sVeriLine2.iRow1 = tpRows[0];
						m_tFaceLabel.sVeriLine2.iRow2 = tpRows[1];
						m_tFaceLabel.sVeriLine2.iDistance = m_tFaceLabel.sVeriLine2.iRow2 - m_tFaceLabel.sVeriLine2.iRow1;
						if (bDebug)
						{


							GenCircle(out regCircle, m_tFaceLabel.sVeriLine2.iRow1, m_tFaceLabel.sVeriLine2.iCol1, 5);
							ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);
							GenCircle(out regCircle, m_tFaceLabel.sVeriLine2.iRow2, m_tFaceLabel.sVeriLine2.iCol2, 5);
							ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);

						}
					}
					fTopRow = (m_tFaceLabel.sVeriLine1.iRow1 + m_tFaceLabel.sVeriLine2.iRow1) / 2;
					fBottomRow = (m_tFaceLabel.sVeriLine1.iRow2 + m_tFaceLabel.sVeriLine2.iRow2) / 2;
					//当前原点
					m_oFaceLabel.sCurrentOri.dOriRow = (fTopRow + fBottomRow) / 2;
					m_oFaceLabel.sCurrentOri.dOriCol = (fLeftCol + fRightCol) / 2;
					m_oFaceLabel.sCurrentOri.dOriPhi = (m_tFaceLabel.fPhi1 + m_tFaceLabel.fPhi2) / 2;
					//计算位置

					LineOrientation(m_tFaceLabel.sVeriLine1.iRow1, m_tFaceLabel.sVeriLine1.iCol1, m_tFaceLabel.sVeriLine2.iRow1, m_tFaceLabel.sVeriLine2.iCol1, out fPhi1);
					//if (fPhi1 < 0)
					//{
					//	fPhi1 = PI+fPhi1;
					//}

					LineOrientation(m_tFaceLabel.sVeriLine1.iRow2, m_tFaceLabel.sVeriLine1.iCol2, m_tFaceLabel.sVeriLine2.iRow2, m_tFaceLabel.sVeriLine2.iCol2, out fPhi2);

					//if (fPhi2 < 0)
					//{
					//	fPhi2 = PI+fPhi2;
					//}
					m_tFaceLabel.fWidth1 = m_tFaceLabel.sHoriLine1.sLineInner.iDistance;
					m_tFaceLabel.fWidth2 = m_tFaceLabel.sHoriLine2.sLineInner.iDistance;
					m_tFaceLabel.fHeight1 = m_tFaceLabel.sVeriLine1.iDistance;
					m_tFaceLabel.fHeight2 = m_tFaceLabel.sVeriLine2.iDistance;


					TupleDeg(fPhi1, out m_tFaceLabel.fPhi1);
					TupleDeg(fPhi2, out m_tFaceLabel.fPhi2);
					m_tFaceLabel.fTop1 = m_tFaceLabel.sVeriLine1.iRow1;
					m_tFaceLabel.fTop2 = m_tFaceLabel.sVeriLine2.iRow1;
					m_tFaceLabel.fBottom1 = m_tFaceLabel.sVeriLine1.iRow2;
					m_tFaceLabel.fBottom2 = m_tFaceLabel.sVeriLine2.iRow2;
					m_tFaceLabel.fLeft1 = m_tFaceLabel.sHoriLine1.sLineInner.iCol1 - m_tFaceLabel.sHoriLine1.sLineOuter.iCol1;
					m_tFaceLabel.fLeft2 = m_tFaceLabel.sHoriLine2.sLineInner.iCol1 - m_tFaceLabel.sHoriLine2.sLineOuter.iCol1;
					m_tFaceLabel.fRight1 = m_tFaceLabel.sHoriLine1.sLineOuter.iCol2 - m_tFaceLabel.sHoriLine1.sLineInner.iCol2;
					m_tFaceLabel.fRight2 = m_tFaceLabel.sHoriLine2.sLineOuter.iCol2 - m_tFaceLabel.sHoriLine2.sLineInner.iCol2;
					if (bDebug)
					{
						GenCircle(out regCircle, m_oFaceLabel.sCurrentOri.dOriRow, m_oFaceLabel.sCurrentOri.dOriCol, 5);
						ConcatObj(m_mFaceLabel.regCheck, regCircle, out m_mFaceLabel.regCheck);
					}
					//更新原点：检测测试和保存模板时，需更新模板原点。实际检测时不更新
					if (bUpdateOri)
					{
						m_oFaceLabel.sModelOri = m_oFaceLabel.sCurrentOri;
						m_oFaceLabel.fVerLinePos1 = fPos1;
						m_oFaceLabel.fVerLinePos2 = fPos2;
					}


				}
				sRsu.iType = GOOD_BOTTLE;
				sRsu.strDescription = "";


			}
			catch (Exception e)
	{
				//Halcon异常		
				QString tempStr;
				tempStr = e.message;
				tempStr.remove(0, 20);
				sRsu.iType = ERROR_EXCEPTION;
				sRsu.strDescription = QString("CCheckFaceLabel::LabelLocation,") + tempStr;
				//2013.12.13 保存异常图像和模板	
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}


			}

		//字符
		//定位
		public	s_CheckResult ModelLocation(HObject hImg, bool bDebug = false, bool bUpdateOri = false)
        {
			s_CheckResult sRsu = new s_CheckResult();
            try
            {
                if (m_pFaceLabel.bPMTool)
                {
					HObject reduceImg;
					HObject regThresh, regTemp;
					HTuple lNum;
					
					//模板匹配
					if (m_oFaceLabel.lLocModelID < 0)
					{
						sRsu.iType = ERROR_MODELMATCH;
						//无效的匹配模板
						sRsu.strDescription = QObject::tr("Invalid match model!");
						GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
						return sRsu;
					}
					HTuple tpRows, tpCols, tpPhis, tpScores;
					ReduceDomain(hImg, m_oFaceLabel.AregPMSearchRegion, out  reduceImg);
					findModel(reduceImg, m_oFaceLabel.lLocModelID,ref m_pFaceLabel.spPMToolPara.spFindModel, out tpRows, out tpCols, out tpPhis, out tpScores);
					TupleLength(tpScores, out lNum);
					if (0 == lNum)
					{
						sRsu.iType = ERROR_MODELMATCH; ;
						//未找到正确的定位目标!
						sRsu.strDescription = QObject::tr("Not Find Object!");
						CopyObj(m_oFaceLabel.AregPMSearchRegion, out sRsu.regError, 1, -1);
						return sRsu;
					}
					else
					{
						if (tpScores[0].D() < m_pFaceLabel.fScore)
						{
							sRsu.iType = ERROR_MODELMATCH;
							//匹配度设置不当!
							sRsu.strDescription = QObject::tr("Model Match Falid!");
							HTuple tpRadius;

							TupleGenConst(lNum, 10, out tpRadius);
							GenCircle(out sRsu.regError, tpRows, tpCols, tpRadius);
							return sRsu;
						}
					}
					//添加中间区域
					if (bDebug)
					{
						
						GenCircle(out regTemp, tpRows[0].D, tpCols[0].D, 10);
						ConcatObj(m_mFaceLabel.regCheck, regTemp, out m_mFaceLabel.regCheck);
					}
					m_oFaceLabel.sCharCurrentOri.dOriRow = tpRows[0].D();
					m_oFaceLabel.sCharCurrentOri.dOriCol = tpCols[0].D();
					m_oFaceLabel.sCharCurrentOri.dOriPhi = tpPhis[0].D();
					//更新原点：检测测试和保存模板时，需更新模板原点。实际检测时不更新
					if (bUpdateOri)
					{
						m_oFaceLabel.sCharModelOri = m_oFaceLabel.sCharCurrentOri;
					}
					HTuple dRow1, dCol1, dRow2, dCol2;
					

					SmallestRectangle1(m_oFaceLabel.regPMModelRegion, out dRow1, out dCol1, out dRow2, out dCol2);
					GenRectangle1(out m_oFaceLabel.regPMModelRegion, m_oFaceLabel.sCharCurrentOri.dOriRow - (dRow2 - dRow1) / 2,
						m_oFaceLabel.sCharCurrentOri.dOriCol - (dCol2 - dCol1) / 2, m_oFaceLabel.sCharCurrentOri.dOriRow + (dRow2 - dRow1) / 2,
						m_oFaceLabel.sCharCurrentOri.dOriCol + (dCol2 - dCol1) / 2);
				}
			}
			catch (Exception e)
			{
				//Halcon异常		
				QString tempStr;
				tempStr = e.message;
				tempStr.remove(0, 20);
				sRsu.iType = ERROR_EXCEPTION;
				sRsu.strDescription = QString("CCheckFaceLabel::LabelLocation,") + tempStr;
				//2013.12.13 保存异常图像和模板	
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}
		}
		//检测
		public s_CheckResult CodeCheck(HObject hImg, bool bDebug = false, bool bCalib = false)
        {
			s_CheckResult sRsu = new s_CheckResult();
			if (m_pFaceLabel.bOCR)
			{
				HObject regChar;
				sRsu = charCheckBase(hImg, m_oFaceLabel.AregCharRegion, m_pFaceLabel.spOCRPara, m_oFaceLabel.AregCodePosRegion, &regChar);
				if ((g_iUserPermit > 5 || bDebug) && 0 == sRsu.iType)//好结果也是显示
				{
					
					ConcatObj(m_mFaceLabel.regCheck, regChar, out m_mFaceLabel.regCheck);
				}
			}
			return sRsu;
		}
		
		//扣盖检测
		public s_CheckResult BuckleCover(HObject hImg, bool bDebug = false)
        {

        }

		//背景
		public s_CheckResult BackGround(HObject hImg, bool bDebug = false, bool bUpdateOri = false)
        {
			s_CheckResult sRsu = new s_CheckResult();
            //去除背景
            if (m_pFaceLabel.bBackGround)
            {
				HObject reduceImg, AffinImg;
				HObject regThresh, regTemp, regSub;
				HTuple lNum;
				//模板匹配
				if (m_oFaceLabel.lBackGroundModelID < 0)
				{
					sRsu.iType = ERROR_LABELBACKGROUND;
					//无效的匹配模板
					sRsu.strDescription = QObject::tr("Invalid match model!");
					
					GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
					return sRsu;
				}

				HTuple tpRows, tpCols, tpPhis, tpScores;
				findModel(hImg, m_oFaceLabel.lBackGroundModelID,ref m_pFaceLabel.spBackGroundPMToolPara.spFindModel, out tpRows, out tpCols, out tpPhis, out tpScores);
				//find_shape_model (hImg,m_oFaceLabel.lBackGroundModelID, -0.1, 0.2, 0.5, 0, 0.5, "least_squares", 0, 0.7, 
				//	&tpRows,&tpCols,&tpPhis, &tpScores);
				
				TupleLength(tpScores, out lNum);
				if (0 == lNum)
				{
					sRsu.iType = ERROR_LABELBACKGROUND; ;
					//未找到正确的定位目标!
					sRsu.strDescription = QObject::tr("Not Find Object!");
					
					CopyObj(m_oFaceLabel.regBackGroundModelRegion, out sRsu.regError, 1, -1);
					return sRsu;
				}
				else
				{
					if (tpScores[0].D() < m_pFaceLabel.fBackGroundScore)
					{
						sRsu.iType = ERROR_LABELBACKGROUND;
						//匹配度设置不当!
						sRsu.strDescription = QObject::tr("Model Match Falid!");
						HTuple tpRadius;
					

						TupleGenConst(lNum, 10, out tpRadius);
						GenCircle(out sRsu.regError, tpRows, tpCols, tpRadius);
						return sRsu;
					}
				}
				//添加中间区域
				if (bDebug)
				{
					
					GenCircle(out regTemp, tpRows[0].D, tpCols[0].D, 5);
					ConcatObj(m_mFaceLabel.regCheck, regTemp, out m_mFaceLabel.regCheck);
				}
				m_oFaceLabel.sBackGroundCurrentOri.dOriRow = tpRows[0].D();
				m_oFaceLabel.sBackGroundCurrentOri.dOriCol = tpCols[0].D();
				m_oFaceLabel.sBackGroundCurrentOri.dOriPhi = tpPhis[0].D();
				//更新原点：检测测试和保存模板时，需更新模板原点。实际检测时不更新
				if (bUpdateOri)
				{
					m_oFaceLabel.sBackGroundModelOri = m_oFaceLabel.sBackGroundCurrentOri;
				}
				HTuple dRow1, dCol1, dRow2, dCol2;
				SmallestRectangle1(m_oFaceLabel.regBackGroundModelRegion, out dRow1, out dCol1, out dRow2, out dCol2);
				GenRectangle1(out m_oFaceLabel.regBackGroundModelRegion, m_oFaceLabel.sBackGroundCurrentOri.dOriRow - (dRow2 - dRow1) / 2,
					m_oFaceLabel.sBackGroundCurrentOri.dOriCol - (dCol2 - dCol1) / 2, m_oFaceLabel.sBackGroundCurrentOri.dOriRow + (dRow2 - dRow1) / 2,
					m_oFaceLabel.sBackGroundCurrentOri.dOriCol + (dCol2 - dCol1) / 2);
				//训练时不提取
				if (!bUpdateOri)
				{
					//原图-模板图像提取字符
					HTuple homMat2DIdentity;
					HomMat2dIdentity(out homMat2DIdentity);
					VectorAngleToRigid(m_oFaceLabel.sBackGroundModelOri.dOriRow, m_oFaceLabel.sBackGroundModelOri.dOriCol, 0,
						m_oFaceLabel.sBackGroundCurrentOri.dOriRow, m_oFaceLabel.sBackGroundCurrentOri.dOriCol, m_oFaceLabel.sBackGroundCurrentOri.dOriPhi, out homMat2DIdentity);
					//vector_angle_to_rigid (567.5, 611.5, 0,
					//	m_oFaceLabel.sBackGroundCurrentOri.dOriRow, m_oFaceLabel.sBackGroundCurrentOri.dOriCol, m_oFaceLabel.sBackGroundCurrentOri.dOriPhi,&homMat2DIdentity);
				
					AffineTransImage(m_oFaceLabel.imgBackGround, out AffinImg, homMat2DIdentity, "constant", "false");
					
					//float k1 = homMat2DIdentity[0].D();
					//float k2 = homMat2DIdentity[1].D();
					//float k3 = homMat2DIdentity[2].D();
					//float k4 = homMat2DIdentity[3].D();
					//float k5= homMat2DIdentity[4].D();
					//float k6 = homMat2DIdentity[5].D();
					//write_image(AffinImg, "bmp", 0, "0.bmp");	
					SubImage(AffinImg, hImg, out m_tFaceLabel.ImgCode, 1, 0);
					//write_image(hImg, "bmp", 0, "1.bmp");
				}
			}
			return sRsu;
		}

		//检测头发
		public s_CheckResult CheckHair(HObject hImg, bool bDebug/* = false*/)
        {
			s_CheckResult sRsu = new s_CheckResult();
            if (m_pFaceLabel.bHair)
            {
				HObject imgMean;
				HObject regDyn, regTemp, regCon, regHair;
				long lNum;
				
				MeanImage(hImg, out imgMean, 21, 3);
				DynThreshold(hImg, imgMean, out regDyn, 5, "dark");
				OpeningCircle(regDyn, out regTemp, 2.5);
				Difference(regDyn, regTemp, out regDyn);
				Connection(regDyn, out regCon);
				SelectShape(regCon, out regHair, "ra", "and", m_pFaceLabel.iHairLength, 9999999);
				SelectShape(regHair, out regTemp, "phi", "and", -0.314, 0.314);//去掉正负10度干扰
				Difference(regHair, regTemp, out regHair);
				SelectShape(regHair, out regTemp, "phi", "and", -1.6, -1.3);//去掉盒体边缘【1.45弧度】干扰
				Difference(regHair, regTemp, out regHair);
				SelectShape(regHair, out regTemp, "phi", "and", 1.3, 1.6); //去掉盒体边缘【1.45弧度】干扰
				Difference(regHair, regTemp, &regHair);
				SelectShape(regHair, out regHair, "area", "and", 10, 999999999);
				CountObj(regHair, out lNum);
				if (lNum > 0)
				{
					sRsu.iType = ERROR_HAIR;
					CopyObj(regHair, out sRsu.regError, 1, -1);
					return sRsu;
				}
			}
			return sRsu;
		}

		//标签位置
		public	s_CheckResult MeasureLabel()
        {
			s_CheckResult sRsu = new s_CheckResult();
			if(m_pFaceLabel.bLocate && m_pFaceLabel.bLabelPosition)
            {
				sRsu.iType = ERROR_LABELPOSITION;
				//宽度
				if (m_tFaceLabel.fWidth1 < m_pFaceLabel.fWidthMin1 || m_tFaceLabel.fWidth1 > m_pFaceLabel.fWidthMax1)
				{
					sRsu.strDescription = QObject::tr("width1 error");
					CopyObj(m_oFaceLabel.regHorLine1, out sRsu.regError, 1, -1);
					return sRsu;
				}
				if (m_tFaceLabel.fWidth2 < m_pFaceLabel.fWidthMin2 || m_tFaceLabel.fWidth2 > m_pFaceLabel.fWidthMax2)
				{
					sRsu.strDescription = QObject::tr("width2 error");
					CopyObj(m_oFaceLabel.regHorLine2, out sRsu.regError, 1, -1);
					return sRsu;
				}
				//高度
				if (m_tFaceLabel.fHeight1 < m_pFaceLabel.fHeightMin1 || m_tFaceLabel.fHeight1 > m_pFaceLabel.fHeightMax1)
				{
					sRsu.strDescription = QObject::tr("height1 error");
					CopyObj(m_oFaceLabel.regVerLine1, out sRsu.regError, 1, -1);
					return sRsu;
				}
				if (m_tFaceLabel.fHeight2 < m_pFaceLabel.fHeightMin2 || m_tFaceLabel.fHeight2 > m_pFaceLabel.fHeightMax2)
				{
					sRsu.strDescription = QObject::tr("height2 error");
					CopyObj(m_oFaceLabel.regVerLine2, out sRsu.regError, 1, -1);

					//经常看到这个CopyObj算子，这个算子有什么用?

					return sRsu;
				}
				//角度
				if (m_tFaceLabel.fPhi1 < m_pFaceLabel.fPhiMin1 || m_tFaceLabel.fPhi1 > m_pFaceLabel.fPhiMax1)
				{
					sRsu.strDescription = QObject::tr("phi1 error");
					ConcatObj(m_oFaceLabel.regVerLine1, m_oFaceLabel.regVerLine2, out sRsu.regError);
					//ConcatObj算子也经常出现，这个算子有什么用?

					return sRsu;
				}
				if (m_tFaceLabel.fPhi2 < m_pFaceLabel.fPhiMin2 || m_tFaceLabel.fPhi2 > m_pFaceLabel.fPhiMax2)
				{
					sRsu.strDescription = QObject::tr("phi2 error");
					ConcatObj(m_oFaceLabel.regVerLine1, m_oFaceLabel.regVerLine2, out sRsu.regError);
					return sRsu;
				}
				//上
				if (m_tFaceLabel.fTop1 < m_pFaceLabel.fTopMin1 || m_tFaceLabel.fTop1 > m_pFaceLabel.fTopMax1)
				{
					sRsu.strDescription = QObject::tr("top1 error");
					CopyObj(m_oFaceLabel.regVerLine1, out sRsu.regError, 1, -1);
					return sRsu;
				}
				if (m_tFaceLabel.fTop2 < m_pFaceLabel.fTopMin2 || m_tFaceLabel.fTop2 > m_pFaceLabel.fTopMax2)
				{
					sRsu.strDescription = QObject::tr("top2 error");
					CopyObj(m_oFaceLabel.regVerLine2, out sRsu.regError, 1, -1);
					return sRsu;
				}
				//下
				if (m_tFaceLabel.fBottom1 < m_pFaceLabel.fBottomMin1 || m_tFaceLabel.fBottom1 > m_pFaceLabel.fBottomMax1)
				{
					sRsu.strDescription = QObject::tr("bottom1 error");
					CopyObj(m_oFaceLabel.regVerLine1, out sRsu.regError, 1, -1);
					return sRsu;
				}
				if (m_tFaceLabel.fBottom2 < m_pFaceLabel.fBottomMin2 || m_tFaceLabel.fBottom2 > m_pFaceLabel.fBottomMax2)
				{
					sRsu.strDescription = QObject::tr("bottom2 error");
					CopyObj(m_oFaceLabel.regVerLine2, out sRsu.regError, 1, -1);
					return sRsu;
				}
				//左
				if (m_tFaceLabel.fLeft1 < m_pFaceLabel.fLeftMin1 || m_tFaceLabel.fLeft1 > m_pFaceLabel.fLeftMax1)
				{
					sRsu.strDescription = QObject::tr("left1 error");
					CopyObj(m_oFaceLabel.regHorLine1, out sRsu.regError, 1, -1);
					return sRsu;
				}
				if (m_tFaceLabel.fLeft2 < m_pFaceLabel.fLeftMin2 || m_tFaceLabel.fLeft2 > m_pFaceLabel.fLeftMax2)
				{
					sRsu.strDescription = QObject::tr("left2 error");
					CopyObj(m_oFaceLabel.regHorLine2, out sRsu.regError, 1, -1);
					return sRsu;
				}
				//右
				if (m_tFaceLabel.fRight1 < m_pFaceLabel.fRightMin1 || m_tFaceLabel.fRight1 > m_pFaceLabel.fRightMax1)
				{
					sRsu.strDescription = QObject::tr("right1 error");
					CopyObj(m_oFaceLabel.regHorLine1, out sRsu.regError, 1, -1);
					return sRsu;
				}
				if (m_tFaceLabel.fRight2 < m_pFaceLabel.fRightMin2 || m_tFaceLabel.fRight2 > m_pFaceLabel.fRightMax2)
				{
					sRsu.strDescription = QObject::tr("right2 error");
					CopyObj(m_oFaceLabel.regHorLine2, out sRsu.regError, 1, -1);
					return sRsu;
				}
			}
		}

 //*功能重载：处理字符区域
		public void processCharRegion(HObject regChar)
        {
			//2015.3.28 i 误报，宽度修改为0.1
			HObject regCon, regSel, regTemp, regUnion, regLine1, regLine2, regLine;
			double Row, Column;
			long dRow1, dCol1, dRow2, dCol2;
			HTuple lNum;
			int iCharWidth = m_pFaceLabel.spOCRPara.spOCRPublicPara.iCharWidth;
			int iCharHeight = m_pFaceLabel.spOCRPara.spOCRPublicPara.iCharHeight;
			int iCharSpace = m_pFaceLabel.spOCRPara.spOCRPublicPara.iCharSpace;

			closing_rectangle1(*regChar, &regTemp, 1, 3);
			connection(regTemp, &regCon);
			gen_empty_region(&regLine1);
			gen_empty_region(&regLine2);
			count_obj(regCon, &lNum);
			smallest_rectangle1(m_oFaceLabel.AregCharRegion, &dRow1, &dCol1, &dRow2, &dCol2);

			ClosingRectangle1(regChar, out regTemp, 1, 3);
			Connection(regTemp, out regCon);
			GenEmptyRegion(out regLine1);
			GenEmptyRegion(out regLine2);
			CountObj(regCon, out lNum);
			SmallestRectangle1(m_oFaceLabel.AregCharRegion, out dRow1, &dCol1, &dRow2, &dCol2);
			//正常，这时候不正是体现程序员的价值了吗，出了问题就得程序员修，我们才有饭吃

		}

	}
}
