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
	//罐底字符检测参数
	public struct s_pCansBotFont
	{
		public bool bModelLoc;             //是否模板定位
		public int iGray;                  //灰度值
		public s_pCreatModel spCreatModel; //建模参数
		public s_pFindModel spFindModel;   //模板匹配参数

		public bool bCharMatch;            //是否模板定位
		public int iCharEdge;              //字符对比度
		public int iCharWidth;             //字符宽度
		public int iCharHeiht;             //字符高度
	};


public struct MyOri
    {
		public float Row;
		public float Col;
		public float Angle;
	}
	//图形参数
	public struct s_oCansBotFont
	{
		public HTuple lLocModelID;      //定位模板ID
		public HTuple lCharModelID;     //字符模板ID
		public MyOri modelOri;         //坐标原点
		public HObject regModelRegion; //模板区域
		
	};

	//内部参数，仅用于储存内部检测临时数据
	public struct s_tCansBotFont
	{
		public HTuple homMat2D;        //反射矩阵
		public HTuple homMat2DInv;     //逆反射矩阵
	};

	//调试中间结果图像（仅调试时使用，不同类型绘制不同颜色）
	public struct s_mCansBotFont
	{
		public HObject regCheck;       //检测出的结果
		public HObject regCorrect;     //修正结果
	};

	public   class CCheckCansBotFont:CommonCheck
    {
		public s_pCansBotFont m_pCansBotFont;  //数字参数
		public s_oCansBotFont m_oCansBotFont;  //图像参数
		public s_tCansBotFont m_tCansBotFont;  //临时参数
		public s_mCansBotFont m_mCansBotFont;  //中间结果（调试时使用）
		public CCheckCansBotFont()
        {

        }

		public void readParam()
        {

        }

		public void writeParam()
        {

        }

		public s_CheckResult check(HObject srcImg, bool bDebug/* = false*/)
        {
			s_CheckResult sRsu = new s_CheckResult();
			HObject reduceImg, meanImg, affImg;
			HObject regPrompt, regThresh, regTemp, regChar;
			
			HTuple lNum;
			HTuple tpCharRsu;

			GenEmptyObj(out reduceImg);
			GenEmptyObj(out meanImg);
			GenEmptyObj(out regThresh);
			GenEmptyObj(out regTemp);
			try
			{
				if (bDebug)                 //如果调试模式
				{
					

					GenEmptyObj(out m_mCansBotFont.regCheck);
					GenEmptyObj(out m_mCansBotFont.regCorrect);
				}
				
				//模板定位
				if (m_pCansBotFont.bModelLoc)           //如果是模板定位
				{
					if (m_oCansBotFont.lLocModelID < 0)     //如果模板定位失败
					{
						sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
						//无效的定位模板
						sRsu.strDescription = "Invalid loction model!";
						
						GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
						return sRsu;
					}
					//提取罐底区域
				

					Threshold(srcImg, out regThresh, m_pCansBotFont.iGray, 255);
					Connection(regThresh, out regPrompt);
					SelectShapeStd(regPrompt, out regTemp, "max_area", 70);
					FillUp(regTemp, out regThresh);  //区域填充


					//fast_threshold (srcImg, &regThresh, m_pCansBotFont.iGray, 255, 20);
					//closing_circle (regThresh, &regTemp, 3.5);
					//connection (regTemp, &regTemp);
					//select_shape(regTemp, &regPrompt, "area", "and", 300, 99999999);
					//select_shape(regPrompt, &regTemp, "circularity", "and", 0.5, 1);
					//select_shape_std(regTemp,&regThresh,"max_area",70);
					                       //区域技术
					CountObj(regThresh, out lNum);
					if (lNum == 0)
					{
						sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
						//未找到罐底，请检测灰度值设置是否合适!
						sRsu.strDescription = "Bottom of the tank isn't found,Please examine whether gray is appropriate or not!";
						
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
					HTuple tpRows, tpCols, tpPhis, tpScores;
				
					ReduceDomain(srcImg, regThresh, out reduceImg);
					findModel(reduceImg, m_oCansBotFont.lLocModelID,ref m_pCansBotFont.spFindModel, out tpRows, out tpCols, out tpPhis, out tpScores);     //使用模板寻找瓶底位置
				
					TupleLength(tpScores, out lNum);
					if (lNum != 1)                                                                                                      //如果匹配的模板的数量不唯一
					{
						sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
						//未找到正确的定位目标!
						sRsu.strDescription = "The correct object of location isn't found!";
						if (lNum > 0)
						{
							HTuple tpRadius;
							

							TupleGenConst(lNum, 10, out tpRadius);
							GenCircle(out sRsu.regError, tpRows, tpCols, tpRadius);
						}
						else//小于等于0
						{
						
							GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
						}
						return sRsu;
					}
					//添加中间区域
					if (bDebug)
					{
				

						GenCircle(out regTemp, tpRows[0].D, tpCols[0].D, 10);
						ConcatObj(m_mCansBotFont.regCheck, regTemp, out m_mCansBotFont.regCheck);
					}
					
					
					//反射变换矩阵
					VectorAngleToRigid(tpRows[0].D, tpCols[0].D, tpPhis[0].D,//vector_angle_to_rigid算子计算变换矩阵，参数是平移起点，平移终点，旋转角度(执行到这里说明找到的模板数量为1)
						m_oCansBotFont.modelOri.Row, m_oCansBotFont.modelOri.Col, m_oCansBotFont.modelOri.Angle, out m_tCansBotFont.homMat2D);
					HomMat2dInvert(m_tCansBotFont.homMat2D, out m_tCansBotFont.homMat2DInv);  //执行变换
				}
				if (m_pCansBotFont.bCharMatch)
				{
					//获取有效区域和图像
					if (m_pCansBotFont.bModelLoc)           //如果确实是模板定位
					{
						//affine_trans_image (reduceImg, &reduceImg, m_tCansBotFont.homMat2D, "constant", "false");
						//affine_trans_region(regThresh,&regThresh,m_tCansBotFont.homMat2D,"false");
						//2014.8.21 修改转原图
						         //变换，变换的结果就是区域的位置变了(因为有平移和旋转)
						

						AffineTransImage(srcImg, out affImg, m_tCansBotFont.homMat2D, "constant", "false");
						AffineTransRegion(regThresh, out regThresh, m_tCansBotFont.homMat2D, "false");
						ReduceDomain(affImg, regThresh, out reduceImg);
					}
					else
					{
						//提取罐底区域
						                                  //全局阈值选出较亮区域
			

						Threshold(srcImg, out regThresh, m_pCansBotFont.iGray, 255);
						Connection(regThresh, out regPrompt);
						SelectShapeStd(regPrompt, out regTemp, "max_area", 70);
						FillUp(regTemp, out regThresh);

						//fast_threshold (srcImg, &regThresh, m_pCansBotFont.iGray, 255, 20);
						//closing_circle (regThresh, &regTemp, 3.5);
						//connection (regTemp, &regTemp);
						//select_shape(regTemp, &regPrompt, "area", "and", 300, 99999999);
						//select_shape(regPrompt, &regTemp, "circularity", "and", 0.5, 1);
						//select_shape_std(regTemp,&regThresh,"max_area",70);
						

						CountObj(regThresh, out lNum);
						if (lNum == 0)
						{
							sRsu.iType =  (int)(e_DefectType.ERROR_LOCATEFAIL);
							//未找到罐底，请检测灰度值设置是否合适!
							sRsu.strDescription = "Bottom of the tank isn't found,Please examine whether gray is appropriate or not!";
							
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
						
						ReduceDomain(srcImg, regThresh, out reduceImg);
					}
					
			

					//提取字符区域
					MeanImage(reduceImg, out meanImg, 31, 31); //中值滤波，目的是动态阈值
					DynThreshold(reduceImg, meanImg, out regThresh, m_pCansBotFont.iCharEdge, "dark");
					ClosingRectangle1(regThresh, out regTemp, 1, 8); //闭运算
					ClosingCircle(regTemp, out regTemp, 5.5);
					Connection(regTemp, out regPrompt);  //区域联通
					SelectShape(regPrompt, out regTemp, "width", "and", 1.0 * m_pCansBotFont.iCharWidth / 3, 99999);
					SelectShape(regTemp, out regTemp, "height", "and", 3.0 * m_pCansBotFont.iCharHeiht / 4, 1.5 * m_pCansBotFont.iCharHeiht);
					PartitionRectangle(regTemp, out regTemp, m_pCansBotFont.iCharWidth, m_pCansBotFont.iCharHeiht);
					Intersection(regTemp, regThresh, out regTemp);
					ClosingCircle(regTemp, out regChar, 4.5);
					CountObj(regChar, out lNum);
					

					if(lNum == 0)
                    {
						sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
						sRsu.strDescription = "Char region isn't found,Please examine whether the value of charWidth and charHeight is appropriate or not!";
						CountObj(regPrompt, out lNum);
						if(lNum > 0)
                        {
                            if (m_pCansBotFont.bModelLoc)
                            {
								AffineTransRegion(regPrompt, out regPrompt, m_tCansBotFont.homMat2DInv, "false");
                            }
							CopyObj(regPrompt, out sRsu.regError, 1, -1);
                        }
                        else
                        {
							GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
                        }
					}
					if (bDebug)
					{
						
						ConcatObj(m_mCansBotFont.regCheck, regChar, out m_mCansBotFont.regCheck);
					}
					//字符识别
					HTuple Confidence;
		

					SortRegion(regChar, out regChar, "character", "true", "row"); //区域排序
					DoOcrMultiClassMlp(regChar, reduceImg, m_oCansBotFont.lCharModelID, out tpCharRsu,out Confidence); //mlp识别向量
					TupleLength(tpCharRsu, out lNum);
					if (lNum <= 0)
					{
						sRsu.iType = (int)(e_DefectType.ERROR_CHARMATCH);
						//识别字符错误!
						sRsu.strDescription = "Failed to recognition character!";
						//变换回正常图像
						if (m_pCansBotFont.bModelLoc)
                        {
							AffineTransRegion(regChar, out regChar, m_tCansBotFont.homMat2DInv, "false");
						}							

							
						
						CopyObj(regChar, out sRsu.regError, 1, -1);
						return sRsu;
					}
					for (int i = 0; i < lNum; ++i)
					{
						sRsu.strDescription += tpCharRsu[i].ToString();
					}
					sRsu.strDescription += QString("%1").arg("\0");
				}
				return sRsu;
			}
			catch (Exception e)
			{
				//Halcon异常		
				String tempStr;
				tempStr = e.message;
				tempStr.remove(0, 20);
				sRsu.iType =  (int)(e_DefectType.ERROR_EXCEPTION);
				sRsu.strDescription = "CCheckCansBotFont::check," + tempStr;
				//2013.12.13 保存异常图像和模板	
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}
			catch (...)
			{
				sRsu.iType =  (int)(e_DefectType.ERROR_EXCEPTION);
				sRsu.strDescription = QString("CCheckCansBotFont::check");
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}
			}
	}
}
