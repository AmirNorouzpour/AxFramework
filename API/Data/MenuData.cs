using System.Collections.Generic;
using Entities.Framework;

namespace API.Data
{
    public class MenuData
    {
        public static List<IndicatorGroup> GetMenuData()
        {
            var data = new List<IndicatorGroup>
            {
                new ()
                {
                    Title = "MarketData",
                    Icon = "sliders",
                    Indicators = new List<Indicator>
                    {
                        new()
                        {
                            Title = "Open",Parameters = new List<IndicatorParameter>{
                            new()
                            {
                                Title = "Result",IsInput = false,Type = "list"
                            }
                        }},
                        new()
                        {
                            Title = "High",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "list"
                                }
                            }},
                        new()
                        {
                            Title = "Low",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "list"
                                }
                            }},
                        new()
                        {
                            Title = "Close",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "list"
                                }
                            },Description = "Close of candle price "},
                        new()
                        {
                            Title = "OHLC4",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "list"
                                }
                            }},
                        new()
                        {
                            Title = "HLC3",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "list"
                                }
                            }},
                    },
                    IsOpen = true
                },
                new ()
                {
                    Title = "Indicators",
                    Icon = "function",
                    Indicators = new List<Indicator>
                    {
                        new(){Title = "RSI",Parameters = new List<IndicatorParameter>{
                            new()
                            {
                                Title = "Source",IsInput = true,Type = "list"
                            },
                            new()
                            {
                                Title = "Length",IsInput = true,Type = "int"
                            },
                            new()
                            {
                                Title = "Result",IsInput = false,Type = "list"
                            },
                        }},
                        new(){Title = "SMA",Parameters = new List<IndicatorParameter>{
                            new()
                            {
                                Title = "Source",IsInput = true,Type = "list"
                            },
                            new()
                            {
                                Title = "Length",IsInput = true,Type = "int"
                            },
                            new()
                            {
                                Title = "Result",IsInput = false,Type = "list"
                            },
                        }},
                        new(){Title = "EMA",Parameters = new List<IndicatorParameter>{
                            new()
                            {
                                Title = "Source",IsInput = true,Type = "list"
                            },
                            new()
                            {
                                Title = "Length",IsInput = true,Type = "int"
                            },
                            new()
                            {
                                Title = "Result",IsInput = false,Type = "list"
                            },
                        }}
                        ,
                        new(){Title = "MACD",Parameters = new List<IndicatorParameter>{
                            new()
                            {
                                Title = "Source",IsInput = true,Type = "list"
                            },
                            new()
                            {
                                Title = "Fast Length",IsInput = true,Type = "int"
                            },
                            new()
                            {
                                Title = "Slow Length",IsInput = true,Type = "int"
                            },
                            new()
                            {
                                Title = "Signal",IsInput = true,Type = "int"
                            },
                            new()
                            {
                                Title = "Result",IsInput = false,Type = "list"
                            }
                        }}
                    }
                },
                new ()
                {
                    Title = "Equations",
                    Icon = "bulb",
                    Indicators = new List<Indicator>
                    {
                        new()
                        {
                            Title = "And",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Input",IsInput = true,Type = "list"
                                },
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "list"
                                }
                            }},
                        new()
                        {
                            Title = "Or",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Input",IsInput = true,Type = "list"
                                },
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "list"
                                }
                            }},
                        new()
                        {
                            Title = "Equals",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Input1",IsInput = true,Type = "object"
                                },
                                new()
                                {
                                    Title = "Input2",IsInput = true,Type = "object"
                                },
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "bool"
                                }
                            }},
                        new()
                        {
                            Title = "IsBiggerThan",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Input1",IsInput = true,Type = "object"
                                },
                                new()
                                {
                                    Title = "Input2",IsInput = true,Type = "object"
                                },
                                new()
                                {
                                    Title = "Input1 > Input2",IsInput = false,Type = "bool"
                                }
                            }},
                        new()
                        {
                            Title = "IsSmallerThan",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Input1",IsInput = true,Type = "object"
                                },
                                new()
                                {
                                    Title = "Input2",IsInput = true,Type = "object"
                                },
                                new()
                                {
                                    Title = "Input1 < Input2",IsInput = false,Type = "bool"
                                }
                            }},
                        new()
                        {
                            Title = "Compare",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Input1",IsInput = true,Type = "bool"
                                },
                                new()
                                {
                                    Title = "Input2",IsInput = true,Type = "bool"
                                },
                                new()
                                {
                                    Title = "IsAbove",IsInput = false,Type = "bool"
                                },
                                new()
                                {
                                    Title = "IsEqual",IsInput = false,Type = "bool"
                                },
                                new()
                                {
                                    Title = "ISBelow",IsInput = false,Type = "bool"
                                }
                            }}, new()
                        {
                            Title = "Not",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Input",IsInput = true,Type = "list"
                                },

                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "list"
                                }
                            }}
                    }
                },  new ()
                {
                    Title = "Flow",
                    Icon = "rise",
                    Indicators = new List<Indicator>
                    {
                        new()
                        {
                            Title = "Branch",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Input",IsInput = true,Type = "bool"
                                },
                                new()
                                {
                                    Title = "IsTrue",IsInput = false,Type = "object"
                                },
                                new()
                                {
                                    Title = "IsFalse",IsInput = false,Type = "object"
                                }
                            }},
                        new()
                        {
                            Title = "If",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Statement",IsInput = true,Type = "bool"
                                },
                                new()
                                {
                                    Title = "IsTrue",IsInput = false,Type = "object"
                                },
                                new()
                                {
                                    Title = "IsFalse",IsInput = false,Type = "object"
                                }
                            }}
                    }
                }, new ()
                {
                    Title = "Inputs",
                    Icon = "form",
                    Indicators = new List<Indicator>
                    {
                        new()
                        {
                            Title = "Input",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Label",IsInput = null,Type = "string", DataEntry=true
                                },
                                new()
                                {
                                    Title = "Value",IsInput = false,Type = "object", DataEntry=true
                                },
                                new()
                                {
                                    Title = "Category",IsInput = null,Type = "string", DataEntry=true
                                },
                                new()
                                {
                                    Title = "ToolTip",IsInput = null,Type = "string", DataEntry=true
                                }
                            }}
                    }
                },
                new ()
                {
                    Title = "Trade",
                    Icon = "play-square",
                    Indicators = new List<Indicator>
                    {
                        new()
                        {
                            Title = "Long",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Execute",IsInput = true,Type = "bool"
                                },
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "bool"
                                }
                            }},
                        new()
                        {
                            Title = "Short",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Execute",IsInput = true,Type = "bool"
                                },
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "bool"
                                }
                            }},
                        new()
                        {
                            Title = "Close Position",Parameters = new List<IndicatorParameter>{
                                new()
                                {
                                    Title = "Condition",IsInput = true,Type = "bool"
                                },
                                new()
                                {
                                    Title = "Result",IsInput = false,Type = "bool"
                                }
                            }}
                    }
                }
            };
            return data;
        }

    }
}
