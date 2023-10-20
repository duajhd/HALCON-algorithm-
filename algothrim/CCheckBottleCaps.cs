using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalconDotNet.HOperatorSet;
using bottleWithHALCON.innerData;
using HalconDotNet;


namespace bottleWithHALCON.algothrim
{

	public struct s_pBottleCaps
	{
		public	bool bModelMatch;           //模板匹配检测
		public int iGray;                  //灰度值
		public float fScore;               //2015.1.21,增加匹配度,与s_pFindModel中的fScore有所不同，以s_pFindModel找到匹配模板后再于该值比较
		public s_pCreatModel spCreatModel; //建模参数
		public s_pFindModel spFindModel;   //模板匹配参数
	};




	public struct s_oBottleCaps
	{
		public HTuple lLocModelID;      //定位模板ID
		public HObject regModelCircle; //模板圆形区域
	};
	//图形参数

	//调试中间结果图像（仅调试时使用，不同类型绘制不同颜色）
	public struct s_mBottleCaps
	{
		public HObject regCheck;       //检测出的结果
		public HObject regCorrect;     //修正结果
	}
	



  public  class CCheckBottleCaps : CommonCheck
    {

		public s_pBottleCaps m_pBottleCaps;    //数字参数
		public s_oBottleCaps m_oBottleCaps;    //图像参数
		public s_mBottleCaps m_mBottleCaps;    //中间结果（调试时使用）
		public CCheckBottleCaps()
        {

        }

		 ~CCheckBottleCaps()
        {

        }


		public void readParam(String strFilePath)
        {

        }
		public void writeParam(String strFilePath)
        {

        }
		public void displayShape(HTuple lWindID)
        {

        }
		public s_CheckResult location(HObject hImg, bool bDebug = false, bool bUpdateOri = false, bool bCalib = false) {

			s_CheckResult sRes = new s_CheckResult();







		}//定位

		public s_CheckResult check(HObject srcImg, bool bDebug = false)
        {
			//匹配成功之后又如何?
			s_CheckResult sRsu = new s_CheckResult();
			try
			{
				

                if (bDebug)
                {
					GenEmptyObj(out m_mBottleCaps.regCheck);
					GenEmptyObj(out m_mBottleCaps.regCorrect);
                }
				if (m_pBottleCaps.bModelMatch)
				{
					//图像变量用得到方法+图像缩写命名
					HObject reduceImg, meanImg, affImg;
					HObject regPrompt, regThresh, regTemp, regChar;
					HTuple lNum;

				
					HTuple tpCharRsu;

					//模板匹配
					if (m_oBottleCaps.lLocModelID < 0)
					{
						sRsu.iType = ERROR_CAPSTYPE;
						//无效的匹配模板
						sRsu.strDescription = "Invalid match model!";
						
						GenRectangle1( out sRsu.regError, 20, 20, 120, 120);
						return sRsu;
					}
					//提取瓶盖区域
				

					Threshold(srcImg, out regThresh, m_pBottleCaps.iGray, 255);
					Connection(regThresh, out regPrompt);
					FillUp(regPrompt,out regTemp);
					OpeningCircle(regTemp, out regTemp, 33.5);
					Connection(regTemp,out regTemp);
					SelectShape(regTemp,out regTemp, "circularity", "and", 0.5, 1);
					SelectShapeStd(regTemp, out regThresh, "max_area", 70);
					CountObj(regThresh,out lNum);

					if (lNum == 0)
					{
						sRsu.iType = ERROR_NOCAPS;
						//未找到瓶盖，请检测灰度值设置是否合适!
						sRsu.strDescription = "The bottle caps isn't found,Please examine whether gray is appropriate or not!";
						
						CountObj(regTemp,out lNum);
						if (lNum > 0)
						{
							//copy_obj(regPrompt, &sRsu.regError, 1, -1);
							CopyObj(regPrompt,out sRsu.regError, 1, -1);
						}
						else
						{
							//gen_rectangle1(&sRsu.regError, 20, 20, 120, 120);
							GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
						}
						return sRsu;
					}
					//sRsu.iType = ERROR_NOCAPS;
					////未找到瓶盖，请检测灰度值设置是否合适!
					//sRsu.strDescription = QObject::tr("The bottle caps isn't found,Please examine whether gray is appropriate or not!");
					//copy_obj(regThresh,&sRsu.regError,1,-1);
					//return sRsu;

					//模板定位
					HTuple tpRows, tpCols, tpPhis, tpScores;
					
					//c#结构体

					ReduceDomain(srcImg, regThresh, out reduceImg);
					findModel(reduceImg, m_oBottleCaps.lLocModelID,ref m_pBottleCaps.spFindModel, out tpRows, out tpCols, out tpPhis, out tpScores);
					TupleLength(tpScores,out lNum);

					if (0 == lNum)
					{
						sRsu.iType = ERROR_CAPSTYPE;
						//未找到正确的定位目标!
						sRsu.strDescription = "Type of the caps is error,match falid!Score is 0";
						

						CopyObj(m_oBottleCaps.regModelCircle, out sRsu.regError, 1, -1);
						return sRsu;
					}
					else
					{
						if (tpScores[0].D < m_pBottleCaps.fScore)
						{
							sRsu.iType = ERROR_CAPSTYPE;
							//匹配度设置不当!
							sRsu.strDescription = "Type of the caps is error,Please examine whether score is appropriate or not!Score is %1" QObject::tr().arg(tpScores[0].D());
							HTuple tpRadius;
					


							TupleGenConst(lNum,10,out tpRadius);
							GenCircle(out sRsu.regError,tpRows,tpCols,tpRadius);
							return sRsu;
						}
					}
					//添加中间区域
					if (bDebug)
					{
						
						GenCircle(out regTemp, tpRows[0].D, tpCols[0].D, 10);
						ConcatObj(m_mBottleCaps.regCheck, regTemp, out m_mBottleCaps.regCheck);
					}
					sRsu.strDescription = "match success!Score is %1" + tpScores[0].S;
					HTuple dRadius;
					

					SmallestCircle(m_oBottleCaps.regModelCircle, out tpRows, out tpCols, out dRadius);
					GenCircle( out m_oBottleCaps.regModelCircle, tpRows, tpCols, dRadius);
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
				sRsu.strDescription = "CCheckBottleCaps::check," + tempStr;
				//2013.12.13 保存异常图像和模板	
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}
	catch (...)
	{
				sRsu.iType = ERROR_EXCEPTION;
				sRsu.strDescription = QString("CCheckBottleCaps::check");
				WriteLog(sRsu.strDescription, AbnormityLog);
				return sRsu;
			}
			}

	}
}
