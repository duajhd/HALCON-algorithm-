using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
namespace bottleWithHALCON.innerData
{


	public enum e_SystemStatus
	{

	}

	public enum e_UserPermission
	{
		//用户权限
		USER_PERMIT_NOLOGIN = 0,                    //未登录用户
		USER_PERMIT_FIR = 1,                    //初级用户
		USER_PERMIT_SEC = 2,                    //中级用户
		USER_PERMIT_THI = 3                      //高级用户
	}

	public enum e_LOGO

	{
		LOGO_DAHENG = 0,                    //大恒
		LOGO_DAYUE = 1,                 //大岳
		LOGO_KESHIMIN = 2                //科时敏
	}



	//检测项ID定义
	public enum e_DetectType
	{

		 DETECT_TYPE_CANSOUTWARD = 0,	//易拉罐外观
		 DETECT_TYPE_CANSBOTFONT = 1,	//易拉罐底部字符
		 DETECT_TYPE_BOTTLECAPS = 2,	//瓶盖
		 DETECT_TYPE_BOTTLEMORPH = 3,	//异形瓶
		 DETECT_TYPE_BOTTLEMOUTHEX = 4,	//瓶口外沿展开
		 DETECT_TYPE_FULLLEVELPOS = 5,	//液位
		 DETECT_TYPE_FULLLCAPS = 6,	//满瓶瓶盖
		 DETECT_TYPE_FACELABEL = 7,	//平面标签(豆瓣酱)
		 DETECT_TYPE_CIRCLELABEL = 8,	//圆形标签(老干妈)
		 DETECT_TYPE_CODING = 9,	//喷码检测
		 DETECT_TYPE_LASERCODING = 10 //激光喷码检测

	}

	public enum e_DrawType
	{
		//绘图类型定义
		  DRAW_LINE = 1,		//线段
		  DRAW_RECTANGLE = 2,	//矩形
		  DRAW_CIRCLE = 3,		//圆形
		  DRAW_POLYGON = 4,		//多边形
		  DRAW_REGION = 5      //区域
	}

	public enum e_LineDirect
	{
		L2R = 1,				
		R2L = 2,
	    T2B = 3,
        B2T = 4
	}
    public enum e_SaveLogType
	{
		OperationLog = 0,           //操作日志
		DebugLog,                   //调试日志(缺图 || 误触发 || 缓存区为空 等日志信息)
		AbnormityLog,               //异常日志(异常捕获 || 理论上不应出现的判断错误)
		WarnLog,                    //报警日志

		//CheckLog,		            //检测日志
		//MendParamLog,			    //修改参数日志
		//StatInfo,					//检测统计信息日志
		//AlgorithmDLL				//算法库日志
	};

	//当前绘制状态
	public enum e_DrawStatus
	{
		DRAW_Status_NULL = 0,           //无操作
		DRAW_Status_MOVE,               //移动
		DRAW_Status_ZOOM,               //缩放
		DRAW_Status_MODIFY,             //修改
	};

	//当前鼠标位置所在点
	public enum e_DrawSelect
	{
		DRAW_Select_NULL = 0,           //无操作点
		DRAW_Select_MOVE,               //移动点
		DRAW_Select_ZOOM,               //缩放点
		DRAW_Select_MODIFY,             //修改点
	};

	public struct s_SelectPoint
    {
		int iX;
		int iY;
		int iOffset;
	}
	//线段结构体
	public struct s_LineShape
    {
		int ifirstX;
		int ifirstY;
		int isecondX;
		int isecondY;
    }

    public struct s_RectangleShape 
	{
		int ileftupX;
		int ileftupY;
		int iWidth;
		int iHeight;
    }

	//必须研究c#结构体如何初始化
    public struct s_CircleShape 
	{
		int iOriX;
		int iOriY;
		int iRadius;
	}
	public enum e_KickMode
	{
		KICK_MODE_NORMAL = 0,           //正常踢废
		KICK_MODE_OK = 1,               //持续好
		KICK_MODE_NG = 2,               //连续踢废
		KICK_MODE_ALTERNATION = 3,      //隔瓶踢
	};
	public struct s_CheckResult
	{
		public int iType;                  //错误类型 -1:未知错误类型
		public HObject regError;           //错误区域
		public String strDescription;     //错误描述
		public String strResultInfo;      //结果描述,如显示尺寸信息
		
	};




	public enum e_DefectType
	{
		 GOOD_BOTTLE = 0,		//正常
		 ERROR_LOCATEFAIL = 1,		//定位失败
		 ERROR_MODELMATCH = 2,		//模板匹配错误
		 ERROR_CHARMATCH = 3,		//字符识别错误
		 ERROR_MOUTHDEFORM = 4,		//口变形
         ERROR_MOUTHBREACH = 5,		//缺口
         ERROR_WALLDEFORM = 6,			//罐壁变形
         ERROR_WALLDIMPURITY = 7,			//罐壁杂质
         ERROR_BOTTOMDIMPURITY = 8,		//罐底杂质
         ERROR_HORIZONTALSIZE = 9,			//横向尺寸错误
         ERROR_VERTICALSIZE = 10,			//纵向尺寸错误
         ERROR_INVALIDROI = 11,		//区域预处理错误
         ERROR_BOTTLEBODY = 12,			//瓶身缺陷
         ERROR_LEVELPOS = 13,				//液位错误
         ERROR_SAFERING = 14,				//安全环错误
         ERROR_HIGHCAPS = 15,				//高低盖
         ERROR_TILTCAPS = 16,				//歪盖
         ERROR_BOTTLECOLOR = 17,			//颜色错误
		 ERROR_FULLNOCHAR = 18,			//无喷码
		 ERROR_MOUTHINNERRING = 19,		//口内环错误
		 ERROR_MOUTHDUAL = 20,				//双口
		 ERROR_MOUTHMIDDLERING = 21,		//口平面错误
         ERROR_MOUTHLOF = 22,				//剪刀印
         ERROR_MOUTHOUTTERRING = 23,		//口外环错误
         ERROR_MOUTHBROKEN = 24,			//断环
		 ERROR_MOUTHOUTTERRINGEX = 25,		//口外沿错误
         ERROR_NOCAPS = 26,			//无盖
         ERROR_CAPSTYPE = 27,				//瓶盖品种错误
         ERROR_LABELPOSITION = 28,			//标签位置错误
         ERROR_BUCKLECOVER = 29,			//扣盖错误
         ERROR_LABELBACKGROUND = 30,		//标签背景错误
         ERROR_HAIR = 31,					//头发
         ERROR_NOLABEL = 32,				//无标
         ERROR_LABELDISLOC = 33,			//贴标错位
//const int ERROR_BUCKLECOVER = 29;			//扣盖错误
		 ERROR_EXCEPTION = 99,				//异常



	}
	//报警类型定义（暂用于报警对话框中）
	public enum WARNING_NORMAL
    {
		WARNING_NORMAL = 0,             //正常无报警
		WARNING_CHECK = 1,          //未开启检测报警
		WARNING_KICK = 2,                   //踢废报警，2014.11.26,亚太酿酒瓶盖检测废品大约1天1个，每次踢废发送报警信号
		WARNING_CONTINUOUSKICK = 3,     //连续踢废报警
		WARNING_KICKPROTECT = 4,            //踢废保护报警
		WARNING_SPECIAL = 5,              //特殊结果报警
	}

	//
	//汇总完服务器统一给踢瓶信号
	//对话框信息-提示对话框只能在主线程弹出，将弹出信息传递给主线程
	//每个电脑都有一个触发卡

	public enum e_Status       //当前状态
	{
		NORMAL = 0,             //正常
		DRAW,                   //形状绘制
		CONVERT,                //剪切
								//SHOW,					//显示
	};


  public struct s_ImgWidgetShowInfo
	{
		double dTime;           //采集||检测耗时
		int iSingalCount;       //信号
		bool bCheck;            //是否检测
		s_CheckResult sResult;  //检测结果
	}
	//

	public struct s_pOCRErrorInfo
	{
		String strErrorChar;   //错误字符，1个，检测到错误即退出
		HObject regErrorChar;   //错误区域，1个，检测到错误即退出
		int iType;              //错误字符类型
	}



}
