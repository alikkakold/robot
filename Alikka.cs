using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptSolution;
using ScriptSolution.Indicators;
using ScriptSolution.Indicators.Model;
using ScriptSolution.Model;
using ScriptSolution.Model.Interfaces;
using SourceEts.Models;

namespace ETSBots.ETSBots.CandleFormations
{
    public class HangingMan : Script
    {	

		public CreateInidicator Bb = new CreateInidicator(EnumIndicators.BollinderBands, 0, "");

		public ParamOptimization Volume_First_Position = new ParamOptimization(1, 1, 500, 1, "Vol_First", "Объем первой позиции");
        public ParamOptimization Volume_Second_Position = new ParamOptimization(1, 1, 500, 1, "Vol_Second", "Объем второй позиции");
		
		public ParamOptimization Profit = new ParamOptimization(5, 1, 500, 1, "Profit", "Профит");
        
		public ParamOptimization Otstup_Open = new ParamOptimization(1, 1, 500, 1, "Otstup_Open", "Отступ от линии боллинжера для входа в позицию");
        public ParamOptimization Otstup_Close = new ParamOptimization(1, 1, 500, 1, "Otstup_Close", "Отступ от линии боллинжера для выхода из позиции");
		
		public bool Reopening = true; 
		
		public override void Execute()
        {
            for (var bar = IndexBar; bar < CandleCount - 1; bar++)
            {
                if (Bb.param.LinesIndicators[0].LineParam[0].Value < bar) 
                {	
                    var priceUp = Bb.param.LinesIndicators[1].PriceSeries;
                    var priceDown = Bb.param.LinesIndicators[2].PriceSeries;



					if (LongPos.Count == 0 && ShortPos.Count == 0)
                    {
						if (Reopening) {
						ShortLess(bar + 1, priceUp[bar + 1] + Otstup_Open.ValueInt * FinInfo.Security.MinStep, Volume_First_Position.Value,
                            "Open 1");
							AddLogRobot("Open 1");
						}
                        ShortLess(bar + 1, priceUp[bar + 1] + Otstup_Open.ValueInt * FinInfo.Security.MinStep, Volume_Second_Position.Value,
                            "Open 2");
						AddLogRobot("Open 2");
                    } 
					if (LongPos.Count == 0 && ShortPos.Count == 0)
                    {
						if (Reopening)
						BuyAtLimit(bar + 1, priceDown[bar + 1] - Otstup_Open.ValueInt * FinInfo.Security.MinStep, Volume_First_Position.Value,
                            "Open 1");
                        BuyAtLimit(bar + 1, priceDown[bar + 1] - Otstup_Open.ValueInt * FinInfo.Security.MinStep, Volume_Second_Position.Value,
                            "Open 2");
                    }

            
					if (ShortPos.Count > 0)
                    {
					for (int i = ShortPos.Count - 1; i >= 0; i--)
						{
							var item = ShortPos[i];
							if (item.EntryNameSignal == "Open 1")
							{
								if ((item.EntryPrice - Profit.ValueInt) > (priceDown[bar + 1] - Otstup_Close.ValueInt))									
									CoverAtStop(bar + 1, item, item.EntryPrice - Profit.ValueInt * FinInfo.Security.MinStep, "Профит");
								else
									SellAtProfit(bar + 1, item, priceDown[bar + 1] - Otstup_Close.ValueInt * FinInfo.Security.MinStep, "Пересечение");
								
							}					
							if (item.EntryNameSignal == "Open 2")
							{
								SellAtProfit(bar + 1, item, priceDown[bar + 1] - Otstup_Close.ValueInt * FinInfo.Security.MinStep, "Пересечение");
								Reopening = !Reopening;
							}
						}

					}
					if (LongPos.Count > 0)
                    {
					for (int i = LongPos.Count - 1; i >= 0; i--)
						{
							var item = LongPos[i];
							if (item.EntryNameSignal == "Open 1")
							{
								if ((item.EntryPrice + Profit.ValueInt) < (priceUp[bar + 1] + Otstup_Close.ValueInt))
				
								SellAtProfit(bar + 1, item, item.EntryPrice + Profit.ValueInt * FinInfo.Security.MinStep, "Профит");
								else
			
								SellAtProfit(bar + 1, item, priceUp[bar + 1] + Otstup_Close.ValueInt * FinInfo.Security.MinStep, "Пересечение");
							}					
							if (item.EntryNameSignal == "Open 2")
							{
								SellAtProfit(bar + 1, item, priceUp[bar + 1] + Otstup_Close.ValueInt * FinInfo.Security.MinStep, "Пересечение");	
								Reopening = !Reopening;
							}
						}
					}

                    if (bar > 2)
                    {
                        ParamDebug("Верх. канал тек.",
                            Bb.param.LinesIndicators[0].PriceSeries[bar + 1]);
                        ParamDebug("Ниж. канал тек.",
                            Bb.param.LinesIndicators[1].PriceSeries[bar + 1]);

                    }

                    

                    if (LongPos.Count != 0 || ShortPos.Count != 0)
                    {
                        SendStandartStopFromForm(bar + 1, "");
                        SendTimePosCloseFromForm(bar + 1, "Время");
                        SendClosePosOnRiskFromForm(bar + 1, "");
                    }
                }

            }
        }

        public override void GetAttributesStratetgy()
        {
            DesParamStratetgy.Version = "1.0";
            DesParamStratetgy.DateRelease = "16.01.2022";
            DesParamStratetgy.DateChange = "16.01.2022";
            DesParamStratetgy.Author = "РобоКоммерц";

            DesParamStratetgy.Description = "dasa";
            DesParamStratetgy.Change = "";
            DesParamStratetgy.NameStrategy = "Alikka";
        }
    }
}
