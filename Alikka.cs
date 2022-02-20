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
    public class Alikka_Strategy : Script
    {	

		public CreateInidicator Bb = new CreateInidicator(EnumIndicators.BollinderBands, 0, "");

		public ParamOptimization Volume_First_Position = new ParamOptimization(1, 1, 500, 1, "Vol_First", "Объем первой позиции");
        public ParamOptimization Volume_Second_Position = new ParamOptimization(1, 1, 500, 1, "Vol_Second", "Объем второй позиции");
		
		public ParamOptimization Profit = new ParamOptimization(5, 1, 500, 1, "Profit", "Профит");
        
		public ParamOptimization Otstup_Open = new ParamOptimization(1, 1, 500, 1, "Otstup_Open", "Отступ от линии боллинжера для входа в позицию");
        public ParamOptimization Otstup_Close = new ParamOptimization(1, 1, 500, 1, "Otstup_Close", "Отступ от линии боллинжера для выхода из позиции");
		
		public override void Execute()
        {
            for (var bar = IndexBar; bar < CandleCount - 1; bar++)
            {
                if (Bb.param.LinesIndicators[0].LineParam[0].Value < bar) 
                {	
                    var priceUp = Bb.param.LinesIndicators[1].PriceSeries;
                    var priceDown = Bb.param.LinesIndicators[2].PriceSeries;
					
                    {
					
						ShortAtLimit(bar + 1, priceUp[bar + 1] + Otstup_Open.ValueInt * FinInfo.Security.MinStep, Volume_First_Position.Value,
                            "Вход в шорт позицию 1");
                        ShortAtLimit(bar + 1, priceUp[bar + 1] + Otstup_Open.ValueInt * FinInfo.Security.MinStep, Volume_Second_Position.Value,
                            "Вход в шорт позицию 2");
						BuyAtLimit(bar + 1, priceDown[bar + 1] - Otstup_Open.ValueInt * FinInfo.Security.MinStep, Volume_First_Position.Value,
                            "Вход в лонг позицию 1");
                        BuyAtLimit(bar + 1, priceDown[bar + 1] - Otstup_Open.ValueInt * FinInfo.Security.MinStep, Volume_Second_Position.Value,
                            "Вход в лонг позицию 2");
					}
					
					if (ShortPos.Count > 0)
                    {
					for (int i = ShortPos.Count - 1; i >= 0; i--)
						{
							var item = ShortPos[i];
							if (item.EntryNameSignal == "Вход в шорт позицию 1")
							{					
									CoverAtProfit(bar + 1, item, item.EntryPrice - Profit.ValueInt * FinInfo.Security.MinStep, "Выход из шорт позиции 1 по профиту");
									CoverAtProfit(bar + 1, item, priceDown[bar + 1] - Otstup_Close.ValueInt * FinInfo.Security.MinStep, "Выход из шорт позиции 1 по пересечению");
							}					
							if (item.EntryNameSignal == "Вход в шорт позицию 2")
							{
								CoverAtProfit(bar + 1, item, priceDown[bar + 1] - Otstup_Close.ValueInt * FinInfo.Security.MinStep, "Выход из шорт позиции 2 по пересечению");
								
							}
						}

					}
					if (LongPos.Count > 0)
                    {
					for (int i = LongPos.Count - 1; i >= 0; i--)
						{
							var item = LongPos[i];
							if (item.EntryNameSignal == "Вход в лонг позицию 1")
							{
								SellAtProfit(bar + 1, item, item.EntryPrice + Profit.ValueInt * FinInfo.Security.MinStep, "Выход из лонг позиции 1 по профиту");
								SellAtProfit(bar + 1, item, priceUp[bar + 1] + Otstup_Close.ValueInt * FinInfo.Security.MinStep, "Выход из лонг позиции 1 по пересечению");
							}					
							if (item.EntryNameSignal == "Вход в лонг позицию 2")
							{
								SellAtProfit(bar + 1, item, priceUp[bar + 1] + Otstup_Close.ValueInt * FinInfo.Security.MinStep, "Выход из лонг позиции 2 по пересечению");	
								
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
            DesParamStratetgy.Version = "1.0.0.0";
            DesParamStratetgy.DateRelease = "18.02.2022";
            DesParamStratetgy.DateChange = "18.02.2022";
            DesParamStratetgy.Author = "Alikka";

            DesParamStratetgy.Description = "" ;
            DesParamStratetgy.Change = "";
            DesParamStratetgy.NameStrategy = "Alikka_Strategy";
        }
    }
}
