using ScriptSolution;
using ScriptSolution.Indicators;
using ScriptSolution.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETSBots.ETSBots.CandleFormations
{
    internal class Alikka : Script
    {
        public CreateInidicator Bb = new CreateInidicator(EnumIndicators.BollinderBands, 0, "");

        public ParamOptimization Volume_First_Position = new ParamOptimization(1, 1, 500, 1, "Vol_First", "Объем первой позиции");
        public ParamOptimization Volume_Second_Position = new ParamOptimization(1, 1, 500, 1, "Vol_Second", "Объем второй позиции");

        public ParamOptimization Profit = new ParamOptimization(5, 1, 500, 1, "Profit", "Профит");

        public ParamOptimization Otstup_Open = new ParamOptimization(1, 1, 500, 1, "Otstup_Open", "Отступ от линии боллинжера для входа в позицию");
        public ParamOptimization Otstup_Close = new ParamOptimization(1, 1, 500, 1, "Otstup_Close", "Отступ от линии боллинжера для выхода из позиции");

        public override void Execute()
        {
            for (int bar = IndexBar; bar < CandleCount - 1; bar++)
            {
                if (Bb.param.LinesIndicators[0].LineParam[0].Value < bar)
                {
                    var priceUp = Bb.param.LinesIndicators[1].PriceSeries;
                    var priceDown = Bb.param.LinesIndicators[2].PriceSeries;

                    if (LongPos.Count == 0 && ShortPos.Count == 0)
                    {
                        BuyAtLimit(bar + 1, priceDown[bar] - Otstup_Open.Value, Volume_First_Position.Value, "Вход в лонг позицию 1");
                        BuyAtLimit(bar + 1, priceDown[bar] - Otstup_Open.Value, Volume_Second_Position.Value, "Вход в лонг позицию 2");
                    }

                    if (LongPos.Count == 0 && ShortPos.Count == 0)
                    {
                        ShortAtLimit(bar + 1, priceUp[bar] + Otstup_Open.Value, Volume_First_Position.Value, "Вход в шорт позицию 1");
                        ShortAtLimit(bar + 1, priceUp[bar] + Otstup_Open.Value, Volume_Second_Position.Value, "Вход в шорт позицию 2");
                    }

                    if (LongPos.Count > 0)
                    {
                        for (int i = 0; i < LongPos.Count; i++)
                        {
                            if (LongPos[i].EntryNameSignal.Equals("Вход в лонг позицию 2"))
                            {
                                SellAtProfit(bar + 1, LongPos[i], priceUp[bar] + Otstup_Close.Value, "Выход из лонг позиции 2");

                                continue;
                            }
                            if (LongPos[i].EntryNameSignal.Equals("Вход в лонг позицию 1"))
                            {
                                SellAtProfit(bar + 1, LongPos[i], LongPos[i].EntryPrice + Profit.Value, "Выход из лонг позиции 1");
                                SellAtProfit(bar + 1, LongPos[i], priceUp[bar] + Otstup_Close.Value, "Выход из лонг позиции 1");

                                BuyAtLimit(bar + 1, priceDown[bar] - Otstup_Open.Value, Volume_First_Position.Value, "Повторный вход в лонг позицию 1");

                                continue;
                            }
                            if (LongPos[i].EntryNameSignal.Equals("Повторный вход в лонг позицию 1"))
                            {
                                SellAtProfit(bar + 1, LongPos[i], priceUp[bar] + Otstup_Close.Value, "Повторный выход из лонг позиции 1");
                                continue;
                            }
                        }
                    }
                    if (ShortPos.Count > 0)
                    {
                        for (int i = 0; i < ShortPos.Count; i++)
                        {
                            if (ShortPos[i].EntryNameSignal.Equals("Вход в шорт позицию 2"))
                            {
                                CoverAtProfit(bar + 1, ShortPos[i], priceDown[bar] - Otstup_Close.Value, "Выход из шорт позиции 2");
                                continue;
                            }

                            if (ShortPos[i].EntryNameSignal.Equals("Вход в шорт позицию 1"))
                            {
                                CoverAtProfit(bar + 1, ShortPos[i], ShortPos[i].EntryPrice - Profit.Value, "Выход из шорт позиции 1");
                                CoverAtProfit(bar + 1, ShortPos[i], priceDown[bar] - Otstup_Close.Value, "Выход из шорт позиции 1");

                                ShortAtLimit(bar + 1, priceUp[bar] + Otstup_Open.Value, Volume_First_Position.Value, "Повторный вход в шорт позицию 1");

                                continue;
                            }
                            if (ShortPos[i].EntryNameSignal.Equals("Повторный вход в шорт позицию 1"))
							{
								CoverAtProfit(bar + 1, ShortPos[i], priceDown[bar] - Otstup_Close.Value, "Повторный выход из шорт позиции 1");
								continue;
							}
                        }
                    }
                }
            }
        }

        public override void GetAttributesStratetgy()
        {
            DesParamStratetgy.Version = "1.0.0.1";
            DesParamStratetgy.DateRelease = "18.02.2021";
            DesParamStratetgy.DateChange = "20.02.2021";
            DesParamStratetgy.Author = "РобоКоммерц";

            DesParamStratetgy.Description = "";
            DesParamStratetgy.Change = "";
            DesParamStratetgy.NameStrategy = "Alikka";
        }
    }
}
