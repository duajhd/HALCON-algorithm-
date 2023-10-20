using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
namespace bottleWithHALCON.algothrim
{
	public struct s_pBottleMouthEx
	{
		bool bLocate;       //定位
		int iLocateEdge;
		float fLocateOpenScale;

		bool bInnerRing;    //口内环
		int iInnerGray;
		int iInnerArea;
		float fInnerOpenScale;
		bool bDualMouth;    //双口
		int iDualMouthEdge;

		bool bMiddleRing;   //口平面
		int iMiddleEdge;
		int iMiddleArea;
		float fMiddleOpenScale;
		bool bLof;          //剪刀印
		int iLofEdge;
		int iLofLength;
		float fLofRab;
		int iLofSideLength;
		float fLofSideRab;

		bool bOutterRing;   //口外环
		int iOutterGray;
		int iOutterArea;
		float fOutterOpenScale;
		bool bBreach;   //缺口
		int iBreachEdge;
		int iBreachWidth;
		int iBreachHeight;
		bool bBroken;   //断环
		int iBrokenEdge;
		int iBrokenGray;
		int iBrokenLength;
		float fBrokenOpenScale;

		bool bOutterRingEx; //口外沿
		int iOutterRingExEdge;
		int iOutterRingExArea;
		float fOutterRingExOpenScale;
	}
	

	public struct s_oBottleMouthEx
    {
		//定位
		HObject regInnerRing;
		HObject regOutterRing;
		//内环
		HObject regInnerInCircle;
		HObject regInnerOutCircle;
		//中环
		HObject regMiddleInCircle;
		HObject regMiddleOutCircle;
		//外环
		HObject regOutterInCircle;
		HObject regOutterOutCircle;
		//口外沿
		HObject regOutterExInCircle;
		HObject regOutterExOutCircle;
	}


	public struct s_mBottleMouthEx
    {
		HObject regCheck;       //检测出的结果
		HObject regCorrect;     //修正结果
	}


	//可以用结构体做成员变量
	//检测飞边可以用当前圆环与标准圆环相减
	//检测黑点可以用增强对比度再阈值分割实现
	//水纹可以检测平均灰度值来实现
	public class CCheckBottleMouthEx
    {
		

	}

	


}
