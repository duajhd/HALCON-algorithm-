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
	public struct s_pCansOutward
	{
		//定位
		public bool bLocate;                   //定位
		public int iMouthGray;                 //罐口灰度值
		public int iBottomGray;                //罐底灰度值
		public int iBottomRingLow;             //罐底亮环半径下限，20150818
		public int iBottomRingHigh;            //罐底亮环半径上限，20150818
		public int iMouthArea;                 //罐口部分亮区域的面积，用于判断口环断裂等缺陷，20150902
											   //口变形
		public bool bMouthBreach;              //罐口变形
		public float fMouthOvalityMax;         //罐口椭圆度最大值，大于该值报错
		public int iMouthBreachArea;           //罐口缺口面积最大值，大于该值报错
		public int iMouthEdge;                 //罐口内侧油污提取对比度

		//壁变形
		public bool bWallDeform;               //罐壁变形
		public int iWallDeformMeanScale;       //罐壁变形平滑尺度
		public int iWallDeformEdge;            //罐壁变形对比度
		public int iWallDeformHeight;          //罐壁变形缺陷高度
		public int iWallDeformArea;            //罐壁变形缺陷面积

		//罐壁杂质
		public bool bWallImpurity;             //罐壁杂质
		public int iWallImpurityGaussScale;    //罐壁变形平滑尺度
		public int iWallImpurityEdge;          //罐壁杂质对比度
		public int iWallImpurityArea;          //罐壁杂质面积
		public int iWallImpurityGray;          //罐壁平均灰度值，用以区别罐内印色，20150911

		//底中心
		public bool bBottomCenter;             //底中心
		public int iBottomCenterEdge;          //罐底中心对比度
		public int iBottomCenterArea;          //罐底中心缺陷面积

		//底外环
		public bool bBottomOutRing;            //底外环
		public int iBottomOutRingEdge;         //罐底外环对比度
		public int iBottomOutRingArea;         //罐底外环缺陷面积

		//底亮环 20150819
		public bool bLightRing;                //底亮环
		public int iLightRingErosionIn;        //以底中心定位环向里腐蚀像素数
		public int iLightRingErosionOut;       //以底中心定位环外里腐蚀像素数
		public int iLightRingDarkGray;         //亮环内低于此灰度值认为是“黑”缺陷，和动态分割区域结果与
		public int iLightRingEdge;             //底亮环对比度
		public int iLightRingArea;             //底亮环缺陷面积
	}
	
//图形参数
	public struct s_oCansOutward
	{
		public HObject regWallOutCircle;           //罐壁外环区域
		public HObject regWallInCircle;            //罐壁内环区域

		public HObject regBottomCenter;            //罐底中心

		public HObject regBottomOutRingInCircle;   //罐底外环内圈
		public HObject regBottomOutRingOutCircle;  //罐底外环外圈
	};

	//内部参数，仅用于储存内部检测临时数据
	public struct s_tCansOutward
	{
		public HTuple dMouthRow;       //口中心
		public HTuple dMouthCol;
		public HTuple dBottomRow;      //底中心
		public HTuple dBottomCol;
		public HTuple fMouthOvality;   //罐口椭圆度
	};

	//调试中间结果图像（仅调试时使用，不同类型绘制不同颜色）
	public struct s_mCansOutward
	{
		public HObject regCheck;       //检测出的结果
		public HObject regCorrect;     //修正结果
	};



	public class CCheckCansOutward : CommonCheck
	{
		public s_pCansOutward m_pCansOutward;  //数字参数
		public s_oCansOutward m_oCansOutward;  //图像参数
		public s_tCansOutward m_tCansOutward;  //临时参数
		public s_mCansOutward m_mCansOutward;  //中间结果（调试时使用）
		public CCheckCansOutward()
		{

		}

		~CCheckCansOutward()
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

			HTuple dRow, dCol, dRadius;
			if (m_pCansOutward.bLocate)
			{
				HTuple dRadius1, dRadius2;
				//罐壁内环以罐底中心为圆心

				//	gen_circle(&m_oCansOutward.regWallInCircle, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, dRadius1);

				SmallestCircle(m_oCansOutward.regWallInCircle, out dRow, out dCol, out dRadius);
				GenCircle(out m_oCansOutward.regWallInCircle, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, dRadius1);
				//罐壁外环以罐口中心为圆心
				smallest_circle(m_oCansOutward.regWallOutCircle, NULL, NULL, &dRadius2);
				gen_circle(&m_oCansOutward.regWallOutCircle, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, dRadius2);


				SmallestCircle(m_oCansOutward.regWallOutCircle, out dRow, out dCol, out dRadius2);
				GenCircle(out m_oCansOutward.regWallOutCircle, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, dRadius2);
				//罐底和外环区域均已罐底中心为圆心
				smallest_circle(m_oCansOutward.regBottomCenter, NULL, NULL, &dRadius1);
				gen_circle(&m_oCansOutward.regBottomCenter, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, dRadius1);
				smallest_circle(m_oCansOutward.regBottomOutRingInCircle, NULL, NULL, &dRadius1);
				gen_circle(&m_oCansOutward.regBottomOutRingInCircle, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, dRadius1);
				smallest_circle(m_oCansOutward.regBottomOutRingOutCircle, NULL, NULL, &dRadius2);
				gen_circle(&m_oCansOutward.regBottomOutRingOutCircle, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, dRadius2);


				SmallestCircle(m_oCansOutward.regBottomCenter, out dRow, out dCol, out dRadius1);
				GenCircle(out m_oCansOutward.regBottomCenter, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, dRadius1);
				SmallestCircle(m_oCansOutward.regBottomOutRingInCircle, out dRow, out dCol, out dRadius1);
				GenCircle(out m_oCansOutward.regBottomOutRingInCircle, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, dRadius1);
				SmallestCircle(m_oCansOutward.regBottomOutRingOutCircle out dRow, out dCol, out dRadius2);
				GenCircle(out m_oCansOutward.regBottomOutRingOutCircle, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, dRadius2);

			}
		}
		public s_CheckResult check(HObject hImg, bool bDebug/* = false*/)
		{

			s_CheckResult sRsu = new s_CheckResult();
			sRsu.iType = (int)(e_DefectType.GOOD_BOTTLE);
			HObject regThresh, regTemp, regPrompt, regDiff, regCircle, regPolar, regDyn, regMouth, regMouthIn, regCircleMouthOut, regCircleMouthIn;
			HObject regConnection, regWallDeform;
			HObject imgReduce, imgPolar, imgGauss, imgGauss2, imgMean, imgMean2;
			HObject xldMouth, xldMouthIn, xldBottom;

			String strOvality = "";//椭圆度信息

			HObject regLightRing, regXY, regDifference, regRingIn, regRingOut, regDark, regIntersection, regLight;
			HObject imgReduce2, imgPolarMean;
			HObject regBottom, regCircleLight;
			HTuple dRadiusRingIn, dRadiusRingOut;
			HTuple tpArea;
			HTuple Row2, Column;
			try
			{
				if (bDebug)
				{

					GenEmptyObj(out m_mCansOutward.regCheck);
					GenEmptyObj(out m_mCansOutward.regCorrect);

				}

				//GenEmptyObj(out regThresh);
				//GenEmptyObj(out regMouth);
				//GenEmptyObj(out regTemp);
				//GenEmptyObj(out regPrompt);
				//GenEmptyObj(out regDiff);
				//GenEmptyObj(out regCircle);
				//GenEmptyObj(out regPolar);
				//GenEmptyObj(out regDyn);
				//GenEmptyObj(out regCircleMouthOut);
				//GenEmptyObj(out regCircleMouthIn);
				//GenEmptyObj(out regConnection);
				//GenEmptyObj(out regWallDeform);
				//GenEmptyObj(out imgReduce);
				HObject edges, contourssplit, selectedcontours, sortedcontours;
				HTuple tpRow, tpCol, tpRa, tpRowMean, tpColMean, tpRaMean, tpBottomRadius;
				HTuple lImageWidth, lImageHeight;
				HTuple lNum, lNum1, lNum2, lNum3, lNum4, lNum5, lArea;
				HTuple dPhi, dRadius1, dRadius2, dPhiMouth, dRadius12, dRadius22;
				HTuple startPhi, endPhi, pointerOrder;
				HTuple dRaMouthIn, dRaMouthOut;
				HTuple dMean, deviation;
				//tpRow = new HTuple();
				//tpCol = new HTuple();
				//tpRa = new HTuple();
				//tpRaMean = new HTuple();
				//tpRaMean = new HTuple();
				//tpBottomRadius = new HTuple();
				//lImageWidth = new HTuple();
				//lImageHeight = new HTuple();
				//lNum = new HTuple();
				//lNum1 = new HTuple();
				//lNum2 = new HTuple();
				//lNum3 = new HTuple();
				//lNum4 = new HTuple();
				//lNum5 = new HTuple();
				//lArea = new HTuple();
				//dPhi = new HTuple();
				//dRadius1 = new HTuple();
				//dRadius2 = new HTuple();
				//dPhiMouth = new HTuple();
				//dRadius12 = new HTuple();
				//dRadius22 = new HTuple();

				//vs注释多行代码先按ctr+k再按ctr+c



				//代码中使用这么多HTuple到底合不合理
				int k = 0;

				//int iMouthBordeWidth = 30;//口外沿宽

				GetImageSize(hImg, out lImageWidth, out lImageHeight);


				//定位
				if (m_pCansOutward.bLocate || m_pCansOutward.bMouthBreach)          //寻找罐口
				{


					Threshold(hImg, out regThresh, m_pCansOutward.iMouthGray, 255);
					FillUp(regThresh, out regThresh);
					Connection(regThresh, out regThresh);
					SelectShape(regThresh, out regPrompt, "area", "and", m_pCansOutward.iMouthArea, 9999999);
					SelectShape(regPrompt, out regTemp, "circularity", "and", 0.6, 1);
					SelectShapeStd(regTemp, out regMouth, "max_area", 70);
					GenContourRegionXld(regMouth, out xldMouth, "border");//生成区域的轮廓线
					CountObj(xldMouth, out lNum1);



					if (0 == lNum1)                                                 //如果xld对象的数量等于0
					{
						if (m_pCansOutward.bLocate)
						{
							sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);
							//未找到罐口
							sRsu.strDescription = "Not found the mouth of the cans!";



							Union1(regThresh, out regThresh);
							CopyObj(regThresh, out sRsu.regError, 1, -1);
							return sRsu;

						}
					}
					else//如果找到罐口
					{
						if (bDebug)
						{

							ConcatObj(m_mCansOutward.regCheck, regMouth, out m_mCansOutward.regCheck); //连接对象
						}




						//计算平均灰度值，平均值偏大时判定为罐底
						ReduceDomain(hImg, regMouth, out imgReduce);
						Intensity(regMouth, imgReduce, out dMean, out deviation);//计算平均灰度值
																				 //明确
						if (dMean > (m_pCansOutward.iMouthGray * 0.95f))
						{

							if (m_pCansOutward.bLocate)
							{
								sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);
								//未找到罐口
								strOvality = dMean[0].S;
								sRsu.strDescription = strOvality;

								CountObj(regPrompt, out lNum2);
								if (lNum2 > 0)
								{



									Union1(regPrompt, out regPrompt);
									CopyObj(regPrompt, out sRsu.regError, 1, -1);
								}
								else
								{


									GenRectangle1(out sRsu.regError, 20, 20, 120, 120);
								}
								return sRsu;
							}
						}



						FitEllipseContourXld(xldMouth, "fitzgibbon", -1, 0, 0, 200, 3, 2, out m_tCansOutward.dMouthRow, out m_tCansOutward.dMouthCol, //拟合成椭圆
							out dPhi, out dRadius1, out dRadius2, out startPhi, out endPhi, out pointerOrder);
						//计算口环内外拟合圆半径

						FitCircleContourXld(xldMouth, "algebraic", -1, 0, 0, 3, 2, out Row2, out Column, out dRaMouthOut, out startPhi, out endPhi, out pointerOrder);  //拟合成圆
																																										//6:45到
																																										//底定位
						if (m_pCansOutward.bLocate)
						{
							//底定位：利用口环，找到口环内区域


							ErosionCircle(regMouth, out regTemp, 3.5);
							ReduceDomain(hImg, regTemp, out imgReduce);
							Threshold(imgReduce, out regThresh, 0, m_pCansOutward.iMouthGray);
							FillUp(regThresh, out regThresh);
							Connection(regThresh, out regTemp);
							SelectShapeStd(regTemp, out regMouthIn, "max_area", 70);
							SelectShape(regMouthIn, out regTemp, "area", "and", 1000, 999999);
							CountObj(regTemp, out lNum2);

							if (0 == lNum2)
							{
								//罐底区域定位不成功，则将罐口中心作为罐底中心
								m_tCansOutward.dBottomRow = m_tCansOutward.dMouthRow;
								m_tCansOutward.dBottomCol = m_tCansOutward.dMouthCol;
							}
							else
							{
								//计算口环内外拟合圆半径

								GenContourRegionXld(regTemp, out xldMouthIn, "border");
								FitCircleContourXld(xldMouthIn, "algebraic", -1, 0, 0, 3, 2, out Row2, out Column, out dRaMouthIn, out startPhi, out endPhi, out pointerOrder);
								//拟合罐口内侧

								FitEllipseContourXld(xldMouthIn, "fitzgibbon", -1, 0, 0, 200, 3, 2, out Row2, out Column, out dPhi, out dRadius12, out dRadius22, out startPhi, out endPhi, out pointerOrder);
								//底定位：从口环内部提取符合条件的轮廓

								ErosionCircle(regTemp, out regTemp, 15);//加多宝15--》151
								ReduceDomain(hImg, regTemp, out imgReduce);
								EdgesSubPix(imgReduce, out edges, "canny", 1, 20, 40);//耗时多。。。
																					  //threshold_sub_pix(imgReduce,&edges,150);20150907容易定位失败


								//C#代码时一定要注意双括号错误，如((

								SegmentContoursXld(edges, out contourssplit, "lines_circles", 5, 4, 2);//先分割边缘；再执行segment_contour_xld
								SelectContoursXld(contourssplit, out selectedcontours, "contour_length", 100, 99999, 0, 100);//加多宝 100--》200
								SortContoursXld(selectedcontours, out sortedcontours, "upper_left", "true", "row");
								CountObj(sortedcontours, out lNum3);







								if (0 == lNum3)
								{
									//罐底区域定位不成功，则将罐口中心作为罐底中心
									m_tCansOutward.dBottomRow = m_tCansOutward.dMouthRow;
									m_tCansOutward.dBottomCol = m_tCansOutward.dMouthCol;
									WriteLog("轮廓筛选不符合条件", DebugLog);

									sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);
									sRsu.strDescription = "未找到罐底-1";

									CopyObj(regTemp, out sRsu.regError, 1, -1);
									return sRsu;
								}
								//底定位：通过拟合圆，筛选符合预设半径条件的值，取其均值作为底中心
								//fit_circle_contour_xld(sortedcontours,"algebraic",-1,0,0,3,2,&tpRow,&tpCol,&tpRa,NULL,NULL,NULL);
								FitCircleContourXld(sortedcontours, "algebraic", -1, 0, 0, 3, 2, out tpRow, out tpCol, out tpRa, out startPhi, out endPhi, out pointerOrder);
								tpRowMean = new HTuple(); //Htuple类型的初始化问题
								tpColMean = new HTuple();
								tpRaMean = new HTuple();
								double row, col, ra;
								for (int i = 0; i < lNum3; i++)
								{
									row = tpRow[i].D;
									col = tpCol[i].D;
									ra = tpRa[i].D;
									if ((tpRa[i].D > m_pCansOutward.iBottomRingLow) && (tpRa[i].D < m_pCansOutward.iBottomRingHigh))
									{


										TupleConcat(tpRowMean, tpRow[i], out tpRowMean);
										TupleConcat(tpColMean, tpCol[i], out tpColMean);
										TupleConcat(tpRaMean, tpRa[i], out tpRaMean);

									}
								}

								TupleLength(tpRaMean, out lNum4);
								if (0 == lNum4)
								{
									//罐底区域定位不成功，则将罐口中心作为罐底中心
									m_tCansOutward.dBottomRow = m_tCansOutward.dMouthRow;
									m_tCansOutward.dBottomCol = m_tCansOutward.dMouthCol;
									WriteLog("半径筛选不符合条件", DebugLog);


									sRsu.iType = (int)(e_DefectType.ERROR_LOCATEFAIL);

									strOvality = "半径：";
									//count_obj(sortedcontours,&lNum);
									CountObj(sortedcontours, out lNum);
									for (k = 0; k < lNum; k++)
									{
										strOvality += QObject::tr("%1,").arg(tpRa[k].D());
									}
									sRsu.strDescription = strOvality;

									CopyObj(regTemp, out sRsu.regError, 1, -1);
									return sRsu;
								}
								//把列相等的点聚集在一起，然后两个tuple相减
								//计算列值相等的点，聚集在一起，计算所有点的下标
								TupleMean(tpRowMean, out m_tCansOutward.dBottomRow);
								TupleMean(tpColMean, out m_tCansOutward.dBottomCol);
								TupleMean(tpRaMean, out tpBottomRadius);
								GenCircle(out regCircleLight, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, tpBottomRadius[0].D);

								if (bDebug)
								{

									ConcatObj(m_mCansOutward.regCheck, regCircleLight, out m_mCansOutward.regCheck);
								}

								//计算亮环区域
								HTuple Row, Col;




								ErosionCircle(regCircleLight, out regRingIn, m_pCansOutward.iLightRingErosionIn);
								DilationCircle(regCircleLight, out regRingOut, m_pCansOutward.iLightRingErosionOut);
								SmallestCircle(regRingIn, out Row, out Col, out dRadiusRingIn);
								Difference(regRingOut, regRingIn, out regDifference);

							}
						}
					}
				}

				//区域变换
				affineShape();


				//口变形
				if (m_pCansOutward.bMouthBreach)            //检测口变形
				{
					//罐口外侧
					m_tCansOutward.fMouthOvality = fabs(dRadius1 - dRadius2) / dRadius1;
					if (m_tCansOutward.fMouthOvality > m_pCansOutward.fMouthOvalityMax)
					{
						sRsu.iType = (int)(e_DefectType.ERROR_MOUTHDEFORM);
						//椭圆度错误
						sRsu.strDescription = QObject::tr("Ovality error : %1!").arg(m_tCansOutward.fMouthOvality, 0, 'f', 5);

						CopyObj(regMouth, out sRsu.regError, 1, -1);

						return sRsu;
					}
					//罐口内侧
					m_tCansOutward.fMouthOvality = fabs(dRadius12 - dRadius22) / dRadius12;
					if (m_tCansOutward.fMouthOvality > m_pCansOutward.fMouthOvalityMax)
					{
						sRsu.iType = ERROR_MOUTHDEFORM;
						//椭圆度错误
						sRsu.strDescription = QObject::tr("Ovality error : %1!").arg(m_tCansOutward.fMouthOvality, 0, 'f', 5);

						GenRegionContourXld(xldMouthIn, out sRsu.regError, "margin");
						return sRsu;
					}

					//closing_circle(regMouth,&regTemp,(int)dRadius2);
					//口环缺口检测
					dRaMouthOut = dRaMouthOut + 20;
					dRaMouthIn = dRaMouthIn - 20;//保证不为负值，暂省略

					GenCircle(out regCircleMouthOut, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, dRaMouthOut);
					GenCircle(out regCircleMouthIn, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, dRaMouthIn);

					Difference(regCircleMouthOut, regCircleMouthIn, out regDiff); //两个圆做差

					ReduceDomain(hImg, regDiff, out imgReduce);

					PolarTransRegion(regDiff, out regPolar, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, 0, 2 * PI,         //展开到直角坐标系
						dRaMouthIn, dRaMouthOut, lImageWidth, dRaMouthOut - dRaMouthIn, "nearest_neighbor");

					PolarTransImageExt(imgReduce, out imgPolar, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, 0, 2 * PI,
						dRaMouthIn, dRaMouthOut, lImageWidth, dRaMouthOut - dRaMouthIn, "nearest_neighbor");


					Threshold(imgPolar, out regThresh, m_pCansOutward.iMouthGray, 255);
					OpeningCircle(regThresh, out regTemp, 1.5);
					//closing_rectangle1(regTemp,&regTemp,151,1);//加多宝

					ClosingRectangle1(regTemp, out regTemp, 51, 1);
					Difference(regTemp, regThresh, out regTemp);
					//difference(regTemp,regThresh,&regTemp);

					OpeningRectangle1(regTemp, out regTemp, 1, 4);
					ClosingRectangle1(regTemp, out regTemp, 5, 1);



					Connection(regTemp, out regTemp);
					PolarTransRegionInv(regTemp, out regTemp, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, 0, 2 * PI,
						dRaMouthIn, dRaMouthOut, lImageWidth, dRaMouthOut - dRaMouthIn, lImageWidth, lImageHeight, "nearest_neighbor");
					SelectShape(regTemp, out regTemp, "area", "and", m_pCansOutward.iMouthBreachArea, 999999);
					CountObj(regTemp, out lNum);
					strOvality = QObject::tr("Ovality : %1").arg(m_tCansOutward.fMouthOvality, 0, 'f', 4);
					if (lNum > 0)
					{
						

						AreaCenter(regTemp, out tpArea);
						strOvality = "缺陷面积：";

						for (k = 0; k < lNum; k++)
						{
							strOvality += QObject::tr("%1,").arg(tpArea[k].I());
						}
						sRsu.iType = ERROR_MOUTHBREACH;
						//结果信息显示椭圆度
						sRsu.strResultInfo += strOvality;



						Union1(regTemp, out regTemp);
						CopyObj(regTemp, out sRsu.regError, 1, -1);
						return sRsu;
					}


			
					//旋转角度-90~270，再次检测罐口缺口
					PolarTransRegion(regDiff, out regPolar, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, -PI / 2, 3 * PI / 2,
				dRaMouthIn, dRaMouthOut, lImageWidth, dRaMouthOut - dRaMouthIn, "nearest_neighbor");
					PolarTransImageExt(imgReduce, out imgPolar, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, -PI / 2, 3 * PI / 2,
				dRaMouthIn, dRaMouthOut, lImageWidth, dRaMouthOut - dRaMouthIn, "nearest_neighbor");
					Threshold(imgPolar, out regThresh, m_pCansOutward.iMouthGray, 255);
					OpeningCircle(regThresh, out regTemp, 1.5);
					ClosingRectangle1(regTemp, out regTemp, 51, 1);
					Difference(regTemp, regThresh, out regTemp);
					OpeningRectangle1(regTemp, out regTemp, 1, 4);
					Difference(regTemp, regThresh, out regTemp);
					OpeningRectangle1(regTemp, out regTemp, 1, 4);
					ClosingRectangle1(regTemp, out regTemp, 51, 1);
					Connection(regTemp, out regTemp);
					PolarTransRegionInv(regTemp, out regTemp, m_tCansOutward.dMouthRow, m_tCansOutward.dMouthCol, -PI / 2, 3 * PI / 2,
				dRaMouthIn, dRaMouthOut, lImageWidth, dRaMouthOut - dRaMouthIn, lImageWidth, lImageHeight, "nearest_neighbor");
					SelectShape(regTemp, out regTemp, "area", "and", m_pCansOutward.iMouthBreachArea, 999999);
					CountObj(regTemp, out lNum);

					strOvality = QObject::tr("Ovality : %1").arg(m_tCansOutward.fMouthOvality, 0, 'f', 4);
					if (lNum > 0)
					{
						
						AreaCenter(regTemp, out tpArea);
						strOvality = "缺陷面积：";

						for (k = 0; k < lNum; k++)
						{
							strOvality += QObject::tr("%1,").arg(tpArea[k].I());
						}
						sRsu.iType = ERROR_MOUTHBREACH;

						//结果信息显示椭圆度
						sRsu.strResultInfo += strOvality;


						Union1(regTemp, out regTemp);

						CopyObj(regTemp, out sRsu.regError, 1, -1);

						return sRsu;
					}



				
					//口环内部油污检测
					int iMouthErosionIn = 15;
					double dMouthRow, dMouthCol;
					ErosionCircle(regMouthIn, out regCircleMouthOut, 5);
					ErosionCircle(regCircleMouthOut, out regCircleMouthIn, iMouthErosionIn);
					Difference(regCircleMouthOut, regCircleMouthIn, out regDiff);
					GenContourRegionXld(regCircleMouthIn, out xldMouthIn, "border");
					FitCircleContourXld(xldMouthIn, "algebraic", -1, 0, 0, 3, 2, out dMouthRow, out dMouthCol, out dRaMouthIn);
					ReduceDomain(hImg, regDiff, out imgReduce);
					PolarTransRegion(regDiff, out regPolar, dMouthRow, dMouthCol, 0, 2 * PI, dRaMouthIn, dRaMouthIn + iMouthErosionIn, lImageWidth, iMouthErosionIn, "nearest_neighbor");
					PolarTransImageExt(imgReduce, out imgPolar, dMouthRow, dMouthCol, 0, 2 * PI, dRaMouthIn, dRaMouthIn + iMouthErosionIn, lImageWidth, iMouthErosionIn, "nearest_neighbor");



					ScaleImageMax(imgPolar, out imgPolar);
					GaussImage(imgPolar, out imgGauss, 5);
					MeanImage(imgGauss, out imgMean, 51, 11);
					ReduceDomain(imgGauss, regPolar, out imgGauss);
					ReduceDomain(imgMean, regPolar, out imgMean);
					DynThreshold(imgGauss, imgMean, out regThresh, m_pCansOutward.iMouthEdge, "dark");
					PolarTransRegionInv(regThresh, out regXY, dMouthRow, dMouthCol, 0, 2 * PI, dRaMouthIn, dRaMouthIn + iMouthErosionIn, lImageWidth, iMouthErosionIn, lImageWidth, lImageHeight, "nearest_neighbor");
					Connection(regXY, out regTemp);
					SelectShape(regTemp, out regTemp, "area", "and", m_pCansOutward.iMouthBreachArea, 999999);
					CountObj(regTemp, out lNum);
					strOvality = QObject::tr("Ovality : %1").arg(m_tCansOutward.fMouthOvality, 0, 'f', 4);
					if (lNum > 0)
					{
						area_center(regTemp, &tpArea, NULL, NULL);
						strOvality = "缺陷面积：";

						for (k = 0; k < lNum; k++)
						{
							strOvality += QObject::tr("%1,").arg(tpArea[k].I());
						}
						sRsu.iType = ERROR_MOUTHBREACH;
						//结果信息显示椭圆度
						sRsu.strResultInfo += strOvality;

						union1(regTemp, &regTemp);
						copy_obj(regTemp, &sRsu.regError, 1, -1);
						return sRsu;
					}

					//口环内侧凸起检测
					//reduce_domain(hImg,regDiff,&imgReduce);
					//threshold(imgReduce,&regThresh,m_pCansOutward.iMouthGray,255);
					//difference(regCircleMouthOut,regCircleMouthIn,&regDiff);
					//difference(regThresh,regDiff,&regTemp);
					//connection(regDiff,&regTemp);
					//select_shape(regTemp,&regTemp,"area","and",m_pCansOutward.iMouthBreachArea,999999);
					//count_obj(regTemp,&lNum);
					//strOvality = QObject::tr("Ovality : %1").arg(m_tCansOutward.fMouthOvality,0,'f',4);
					//if(lNum>0)
					//{
					//	area_center(regTemp,&tpArea,NULL,NULL);
					//	strOvality = "缺陷面积：";

					//	for (k=0;k<lNum;k++)
					//	{
					//		strOvality += QObject::tr("%1,").arg(tpArea[k].I());
					//	}
					//	sRsu.iType = ERROR_MOUTHBREACH;
					//	//结果信息显示椭圆度
					//	sRsu.strResultInfo += strOvality;

					//	union1(regTemp,&regTemp);
					//	copy_obj(regTemp,&sRsu.regError,1,-1);
					//	return sRsu;
					//}
				}

				//罐壁变形&&罐壁杂质
				if (m_pCansOutward.bWallDeform || m_pCansOutward.bWallImpurity)
				{

					
					//获取半径，极坐标
					SmallestCircle((m_oCansOutward.regWallInCircle, NULL, NULL, out dRadius1);
					SmallestCircle(m_oCansOutward.regWallOutCircle, NULL, NULL, out dRadius2);


					difference(m_oCansOutward.regWallOutCircle, m_oCansOutward.regWallInCircle, &regDiff);
					area_center(regDiff, &lArea, NULL, NULL);
					if (lArea > 1000 && dRadius2 - dRadius1 > 10)
					{
						//以罐底中心变换，也可考虑区域中心
						polar_trans_region(regDiff, &regPolar, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,
							dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, "nearest_neighbor");
						polar_trans_image_ext(hImg, &imgPolar, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,
							dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, "nearest_neighbor");
						/*polar_trans_region (regDiff, &regPolar, m_tCansOutward.dMouthRow,m_tCansOutward.dMouthCol, 0, 2*PI, 
							dRadius1, dRadius2, lImageWidth, dRadius2-dRadius1, "nearest_neighbor");	
						polar_trans_image_ext (hImg, &imgPolar, m_tCansOutward.dMouthRow,m_tCansOutward.dMouthCol, 0, 2*PI, 
							dRadius1, dRadius2, lImageWidth, dRadius2-dRadius1, "nearest_neighbor");*/
						//取高斯平滑后的对比度区域，若检测变形效果不好，可考虑去掉；杂质时最好使用平滑图像
						scale_image_max(imgPolar, &imgPolar);
						//gauss_image (imgPolar, &imgGauss,7);//加多宝
						gauss_image(imgPolar, &imgGauss, 5);
						gauss_image(imgPolar, &imgGauss2, 3);
						//mean_image (imgGauss, &imgMean, m_pCansOutward.iWallDeformMeanScale*2+1, 1);//加多宝
						//mean_image (imgGauss2, &imgMean2, m_pCansOutward.iWallDeformMeanScale/2+1, 3);//加多宝
						mean_image(imgGauss, &imgMean, m_pCansOutward.iWallDeformMeanScale / 2 + 1, 1);
						mean_image(imgGauss2, &imgMean2, m_pCansOutward.iWallDeformMeanScale / 4 + 1, 3);
						reduce_domain(imgPolar, regPolar, &imgPolar);
						reduce_domain(imgGauss, regPolar, &imgGauss);
						reduce_domain(imgGauss2, regPolar, &imgGauss2);
						reduce_domain(imgMean, regPolar, &imgMean);
						reduce_domain(imgMean2, regPolar, &imgMean2);



						//检测变形应该咨询郝工
						//检测变形
						if (m_pCansOutward.bWallDeform)
						{
							dyn_threshold(imgGauss, imgMean, &regDyn, m_pCansOutward.iWallDeformEdge, "not_equal");
							connection(regDyn, &regTemp);
							select_shape(regTemp, &regTemp, "height", "and", m_pCansOutward.iWallDeformHeight, 999999);         //选择瓶壁区域				选择出瓶壁区域

							union1(regTemp, &regTemp);                                                                              //将多个区域合并成一个区域
							closing_circle(regTemp, &regTemp, 7);
							opening_circle(regTemp, &regTemp, 5);
							connection(regTemp, &regTemp);
							select_shape(regTemp, &regWallDeform, "area", "and", m_pCansOutward.iWallDeformArea, 99999999);

							polar_trans_region_inv(regWallDeform, &regTemp, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,
								dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, lImageWidth, lImageHeight, "nearest_neighbor");

							count_obj(regTemp, &lNum);
							if (lNum > 0)//
							{
								area_center(regWallDeform, &tpArea, NULL, NULL);
								strOvality = "缺陷面积：";

								for (k = 0; k < lNum; k++)
								{
									strOvality += QObject::tr("%1,").arg(tpArea[k].I());
								}
								sRsu.iType = ERROR_WALLDEFORM;
								//结果信息显示椭圆度
								sRsu.strResultInfo += strOvality;

								union1(regTemp, &regTemp);
								copy_obj(regTemp, &sRsu.regError, 1, -1);
								return sRsu;
							}
						}

						//检测杂质
						if (m_pCansOutward.bWallImpurity)
						{



							DynThreshold(imgGauss2, imgMean2, out regDyn, m_pCansOutward.iWallImpurityEdge, "dark");
							Connection(regDyn, out regConnection);
							SelectShape(regConnection, out regConnection, "area", "and", m_pCansOutward.iWallImpurityArea, 99999999);
							PolarTransRegionInv(regConnection, out regTemp, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,               //反极坐标变换，将方形转为圆形
						dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, lImageWidth, lImageHeight, "nearest_neighbor");
							CountObj(regTemp, out lNum);
							if (lNum > 0)
							{
								area_center(regConnection, &tpArea, NULL, NULL);
								strOvality = "缺陷面积：";

								for (k = 0; k < lNum; k++)
								{
									strOvality += QObject::tr("%1,").arg(tpArea[k].I());
								}
								sRsu.iType = ERROR_WALLDIMPURITY;
								//结果信息显示椭圆度
								sRsu.strResultInfo += strOvality;


								Union1(regTemp, out regTemp);
								CopyObj(regTemp, out sRsu.regError, 1, -1);

								return sRsu;
							}

							//	//多点小面积杂质处理
							//	dyn_threshold (imgPolar, imgMean2, &regDyn, 33, "dark");
							//	connection(regDyn,&regConnection);
							//	select_shape (regConnection, &regConnection, "area","and",10 ,99999999);
							//	polar_trans_region_inv (regConnection, &regTemp, m_tCansOutward.dBottomRow,m_tCansOutward.dBottomCol, 0, 2*PI,
							//		dRadius1, dRadius2, lImageWidth, dRadius2-dRadius1, lImageWidth,lImageHeight,"nearest_neighbor");

							//	count_obj(regTemp,&lNum);
							//	if (lNum>1)
							//	{
							//		area_center(regConnection,&tpArea,NULL,NULL);
							//		strOvality = "缺陷面积：";

							//		for (k=0;k<lNum;k++)
							//		{
							//			strOvality += QObject::tr("%1,").arg(tpArea[k].I());
							//		}
							//		sRsu.iType = ERROR_WALLDIMPURITY;
							//		//结果信息显示椭圆度
							//		sRsu.strResultInfo += strOvality;
							//		union1(regTemp,&regTemp);
							//		copy_obj(regTemp,&sRsu.regError,1,-1);
							//		return sRsu;
							//	}

						}


						//测试平均灰度值-罐内印色
						if (m_pCansOutward.bWallImpurity)
						{
							reduce_domain(hImg, regDiff, &imgReduce);
							intensity(regDiff, imgReduce, &dMean, NULL);

							ReduceDomain(Img, regDiff, out imgReduce);
							Intensity(regDiff, imgReduce, out dMean, );
							strOvality = "罐壁平均灰度值：" + QString::number(dMean, 'f', 1);
							if (dMean < m_pCansOutward.iWallImpurityGray)
							{
								sRsu.iType = ERROR_WALLDIMPURITY;

								Threshold(imgReduce, out sRsu.regError, 0, dMean);
								sRsu.strDescription = strOvality;
								return sRsu;
							}
						}
						//测试代码结束

					}
				}


				if (m_pCansOutward.bLightRing)              //bLightRing:底亮环
				{
					area_center(regDifference, &lArea, NULL, NULL);
					if (lArea > 1000)
					{


						ReduceDomain(hImg, regDifference, out imgReduce2);
						PolarTransRegion(regDifference, out regPolar, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI, dRadiusRingIn, dRadiusRingOut, lImageWidth, dRadiusRingOut - dRadiusRingIn, "nearest_neighbor");
						PolarTransImageExt(imgReduce2, out imgPolar, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI, dRadiusRingIn, dRadiusRingOut, lImageWidth, dRadiusRingOut - dRadiusRingIn, "nearest_neighbor");
						Threshold(imgPolar, out regDark, 0, m_pCansOutward.iLightRingDarkGray);//提取“黑”缺陷
						MeanImage(imgPolar, out imgPolarMean, 21, 21);
						DynThreshold(imgPolar, imgPolarMean, out regTemp, m_pCansOutward.iLightRingEdge, "dark");
						Union2(regTemp, regDark, out regDark);
						OpeningCircle(regDark, out regDark, 1);




						DynThreshold(imgPolar, imgPolarMean, out regLight, m_pCansOutward.iLightRingEdge, "light");
						ClosingRectangle1(regLight, out regLight, 2 * m_pCansOutward.iBottomRingLow, 1);
						ErosionCircle(regLight, out regLight, 1);
						Connection(regLight, out regLight);

						//select_shape(regLight,&regLight,"area","and",1000,999999);//加多宝



						Intersection(regDark, regLight, out regIntersection);
						Connection(regIntersection, out regTemp);
						SelectShape(regTemp, out regTemp, "height", "and", 3, 999);
						Union1(regTemp, out regTemp);
						PolarTransRegionInv(regTemp, out regXY, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI, dRadiusRingIn, dRadiusRingOut, lImageWidth, dRadiusRingOut - dRadiusRingIn, lImageWidth, lImageHeight, "nearest_neighbor");
						Connection(regXY, out regTemp);
						SelectShape(regTemp, out regTemp, "area", "and", m_pCansOutward.iLightRingArea, 999999);
						CountObj(regTemp, out lNum5);


						if (lNum5 > 0)
						{

							AreaCenter(regTemp, out tpArea, out Row2, out Column);
							strOvality = "缺陷面积：";

							for (k = 0; k < lNum5; k++)
							{
								strOvality += QObject::tr("%1,").arg(tpArea[k].I());
							}

							sRsu.iType = ERROR_BOTTOMDIMPURITY;
							//结果信息显示椭圆度
							sRsu.strResultInfo += strOvality;


							Union1(regTemp, out regTemp);
							CopyObj(regTemp, out sRsu.regError, 1, -1);
							return sRsu;
						}
					}
				}



				if (m_pCansOutward.bBottomCenter)                           //瓶底中心
				{



					MeanImage(hImg, out imgMean, 21, 21);
					ReduceDomain(hImg, m_oCansOutward.regBottomCenter, out imgReduce);
					ReduceDomain(imgMean, m_oCansOutward.regBottomCenter, out imgMean);
					DynThreshold(imgReduce, imgMean, out regDyn, m_pCansOutward.iBottomCenterEdge, "dark");
					//罐底中心在过曝的情况下 如有大黑斑是什么效果？？？
					Connection(regDyn, out regTemp);
					SelectShape(regTemp, out regTemp, "area", "and", m_pCansOutward.iBottomCenterArea, 99999999);
					CountObj(regTemp, out lNum);
					if (lNum > 0)
					{

						AreaCenter(regTemp, out tpArea, out Row2, out Column);
						strOvality = "缺陷面积：";

						for (k = 0; k < lNum; k++)
						{
							strOvality += QObject::tr("%1,").arg(tpArea[k].I());
						}
						sRsu.iType = ERROR_BOTTOMDIMPURITY;
						//结果信息显示椭圆度
						sRsu.strResultInfo += strOvality;

						Union1(regTemp, out regTemp);
						CopyObj(regTemp, out sRsu.regError, 1, -1);
						return sRsu;
					}

					////底部中心纸片检测
					//erosion_circle(regCircleLight,&regBottom,10);
					//reduce_domain(hImg,regBottom,&imgReduce);
					//bin_threshold(imgReduce,&regTemp);
					//connection(regTemp,&regTemp);
					//select_shape(regTemp,&regPrompt,"area","and",150,999999);
					//dilation_circle(regPrompt,&regTemp,1.5);
					//difference(regBottom,regTemp,&regTemp);
					//reduce_domain(hImg,regTemp,&imgReduce);
					//edges_sub_pix(imgReduce,&edges,"canny",1,10,40);
					//select_contours_xld(edges,&edges,"contour_length",20,9999,-0.5,0.5);
					//count_obj(edges,&lNum);
					//if (lNum>0)
					//{
					//	sRsu.iType = ERROR_BOTTOMDIMPURITY;
					//	//结果信息显示椭圆度
					//	sRsu.strResultInfo += "罐底杂质-轮廓";
					//	//copy_obj(regBottom,&sRsu.regError,1,-1);
					//	gen_region_contour_xld(edges,&sRsu.regError,"filled");
					//	return sRsu;
					//}

				}
				if (m_pCansOutward.bBottomOutRing)                      //底部圆环
				{


					//获取半径，极坐标
					SmallestCircle(m_oCansOutward.regBottomOutRingInCircle, out Row2, out Column, out dRadius1);
					SmallestCircle(m_oCansOutward.regBottomOutRingOutCircle, out Row2, out Column, out dRadius2);


					Difference(m_oCansOutward.regBottomOutRingOutCircle, m_oCansOutward.regBottomOutRingInCircle, out regDiff);
					AreaCenter(regDiff, out lArea, out Row2, out Column);
					if (lArea > 1000 && (dRadius2 - dRadius1) > 10)
					{
						//以罐底中心变换,检测杂质
						polar_trans_region(regDiff, &regPolar, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,
							dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, "nearest_neighbor");
						polar_trans_image_ext(hImg, &imgPolar, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,
							dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, "nearest_neighbor");

						PolarTransRegion(regDiff, &regPolar, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,
					dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, "nearest_neighbor");
						PolarTransImageExt(hImg, out imgPolar, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,
					dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, "nearest_neighbor");
						//罐底暂不作高斯平滑
						//gauss_image (imgPolar, &imgGauss,3) ;
						//mean_image (imgPolar, &imgMean, 101, 1);//加多宝
						mean_image(imgPolar, &imgMean, 51, 1);
						reduce_domain(imgPolar, regPolar, &imgPolar);
						reduce_domain(imgMean, regPolar, &imgMean);
						dyn_threshold(imgPolar, imgMean, &regDyn, m_pCansOutward.iBottomOutRingEdge, "dark");


						MeanImage(imgPolar, out imgMean, 51, 1);
						ReduceDomain(imgPolar, regPolar, out imgPolar);
						ReduceDomain(imgMean, regPolar, out imgMean);
						//DynThreshold算子最后一个参数是什么意思
						DynThreshold(imgPolar, imgMean, out regDyn, m_pCansOutward.iBottomOutRingEdge, "dark");//动态阈值能分割主要是提取那些能用特征筛选出来的区域
																											   //需增加以绝对阈值提取“黑色”缺陷（大黑斑）的方法，动态分割“亮”区域（纸屑）的方法


						PolarTransRegionInv(regDyn, out regTemp, m_tCansOutward.dBottomRow, m_tCansOutward.dBottomCol, 0, 2 * PI,                 //反极坐标变换，将
					dRadius1, dRadius2, lImageWidth, dRadius2 - dRadius1, lImageWidth, lImageHeight, "nearest_neighbor");
						Connection(regTemp, out regTemp);
						SelectShape(regTemp, out regTemp, "area", "and", m_pCansOutward.iBottomOutRingArea, 99999999);
						CountObj(regTemp, out lNum);

						if (lNum > 0)
						{
							area_center(regTemp, &tpArea, NULL, NULL);
							AreaCenter(regTemp, out tpArea, out Row2, out Column);
							strOvality = "缺陷面积：";

							for (k = 0; k < lNum; k++)
							{
								strOvality += QObject::tr("%1,").arg(tpArea[k].I());
							}
							sRsu.iType = ERROR_BOTTOMDIMPURITY;
							//结果信息显示椭圆度
							sRsu.strResultInfo += strOvality;


							Union1(regTemp, out regTemp);
							CopyObj(regTemp, out sRsu.regError, 1, -1);

							return sRsu;
						}
					}
				}
				//1.c#中queue是值类型还是引用类型?2.c#dequeue弹出的是应用还是值(必须要有方法区分一个变量所占的字节大小)
				//答queue是引用类型，引用类型在哪里分配空间?分配的空间会不会自动释放
				//结果信息显示椭圆度
				//sRsu.iType = GOOD_BOTTLE;
				sRsu.strResultInfo += strOvality;
				return sRsu;
            }
            catch (Exception e)
            {

            }
			

			}

		}
		
}
