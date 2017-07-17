var UserManage_ctr = myApp.controller("UserManage_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $filter, myApplocalStorage) {


    $scope.tab_show1 = false;
    $scope.tab_show2 = false;
    $scope.tab_show3 = false;
    $scope.tab_show4 = false;
    $scope.tab_show5 = false;
    $scope.tab_show6 = false;
    $scope.timer_show = false;
    $scope.extent = 1000;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    //\\________________________________________________________________

    //详情显示
    $scope.tab_show_fun1 = function () {
        $scope.tab_show1 = !$scope.tab_show1;
    }
    $scope.tab_show_fun2 = function () {
        $scope.tab_show2 = !$scope.tab_show2;
    }
    $scope.tab_show_fun3 = function () {
        $scope.tab_show3 = !$scope.tab_show3;
    }
    $scope.tab_show_fun4 = function () {
        $scope.tab_show4 = !$scope.tab_show4;
    }
    $scope.tab_show_fun5 = function () {
        $scope.tab_show5 = !$scope.tab_show5;
    }
    $scope.tab_show_fun6 = function () {
        $scope.tab_show6 = !$scope.tab_show6;
    }
  
    //but组切换

    $scope.select_state = function (num) {
        $scope.state_show_num = num;
        if (num == 11) {

        } else if (num == 12) {

        } else if (num == 13) {

        } else if (num == 21) {

        } else if (num == 22) {

        } else if (num == 23) {

        } else if (num == 24) {

        } else if (num == 25) {

        }
    }
    //获取用户数量统计

    $scope.GetUserCount = function () {
        $scope.anaItem = {
            extent: $scope.extent,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Account/GetUserCount"
        })
        .success(function (response, status) {
            $scope.GetUserCountList = response;
            var a = $scope.GetUserCountList.ActiveUserPercent;
            a = a * 100;
            $scope.GetUserCountList.ActiveUserPercent = a;
            var b = $scope.GetUserCountList.PurchaseeUserPercent;
            b = b * 100;
            $scope.GetUserCountList.PurchaseeUserPercent = b;
            console.log($scope.GetUserCountList);
            $scope.pie_chart_1($scope.GetUserCountList.ActiveUserPercent);
            $scope.pie_chart_2($scope.GetUserCountList.PurchaseeUserPercent);
        })
        .error(function (response, status) {
            // $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }
    $scope.pie_chart_1 = function (date) {

        // 基于准备好的dom，初始化echarts实例
        // 指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('shouzhibi_pie'));
        option = {
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br/>{b}: {c} ({d}%)"
            },
            series: [{
                type: 'pie',
                radius: ['80%', '88%'],
                avoidLabelOverlap: false,
                label: {
                    normal: {
                        show: false,
                        position: 'center'
                    },
                    emphasis: {
                        show: true,
                        textStyle: {
                            fontSize: '24',
                            fontWeight: 'bold'
                        }
                    }
                },
                labelLine: {
                    normal: {
                        show: false
                    }
                },
                data: (function () {
                    var seriesData = [];
                    seriesData.push({
                        value: 100 - date,
                        itemStyle: {
                            normal: {
                                color: '#cccccc',
                            }
                        }
                    });
                    seriesData.push({
                        value: date,
                        itemStyle: {
                            normal: {
                                color: '#009999',
                            }
                        }
                    })
                    return seriesData;
                })()
            }]
        };

        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
    };
    $scope.pie_chart_2 = function (date) {
      
        // 基于准备好的dom，初始化echarts实例
        // 指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('shouzhibi_pie_2'));
        option = {
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br/>{b}: {c} ({d}%)"
            },
            series: [{
                type: 'pie',
                radius: ['80%', '88%'],
                avoidLabelOverlap: false,
                label: {
                    normal: {
                        show: false,
                        position: 'center'
                    },
                    emphasis: {
                        show: true,
                        textStyle: {
                            fontSize: '24',
                            fontWeight: 'bold'
                        }
                    }
                },
                labelLine: {
                    normal: {
                        show: false
                    }
                },
                data: (function () {
                    var seriesData = [];
                    seriesData.push({
                        value: 100 - date,
                        itemStyle: {
                            normal: {
                                color: '#ccc',
                            }
                        }
                    });
                    seriesData.push({
                        value: date,
                        itemStyle: {
                            normal: {
                                color: '#777',
                            }
                        }
                    })
                    return seriesData;
                })()
            }]
        };

        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
    };
    //用户变化趋势图
    $scope.userTypeUL = 0;
    $scope.userTypeULFun = function (num) {
        $scope.userTypeUL = num;
        $scope.GetUserLineChart();
    }
    $scope.GetUserLineChart = function () {
        $scope.anaItem = {
            extent: 365,
            interval:1,
            userType: $scope.userTypeUL,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Account/GetUserLineChart"
        })
        .success(function (response, status) {
            $scope.userTypeULList = response;
            console.log(response);
            $scope.UserQMap(response);
           
        })
        .error(function (response, status) {
            // $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }

    

    $scope.UserQMap = function (date) {

       
        // 基于准备好的dom，初始化echarts实例
        // 指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('UserQMap'));
        option = {
            title: {
                text: '用户变化状况',
            },
            tooltip: {
                trigger: 'axis'
            },
            legend: {
                data: ['普通用户', 'VIP用户']
            },
            toolbox: {
                show: true,
                feature: {
                    dataZoom: {
                        yAxisIndex: 'none'
                    },
                    dataView: { readOnly: false },
                    magicType: { type: ['line', 'bar'] },
                    restore: {},
                    saveAsImage: {}
                }
            },
            xAxis: {
                type: 'category',
                boundaryGap: false,
                data: date.Time
            },
            yAxis: {
                type: 'value',
            },
            series: [
                {
                    name: '普通用户',
                    type: 'line',
                    data: date.Free,
                    markLine: {
                        data: [
                            { type: 'average', name: '平均值' }
                        ]
                    }
                },
                {
                    name: 'VIP用户',
                    type: 'line',
                    data: date.Purchase,
                    markPoint: {
                        data: [
                            { name: '周最高', value: 5, xAxis: 3, yAxis:5 }
                        ]
                    },
                }
            ]
        };


        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
    };

    //User distribution
    $scope.GetUserDType = 0;
    $scope.GetUserDTypeFun = function (num) {
        $scope.GetUserDType = num;
        $scope.GetUserDistribution();
    }
    $scope.GetUserDistribution = function () {
        $scope.anaItem = {
            userType: $scope.GetUserDType,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Account/GetUserDistribution"
        })
        .success(function (response, status) {
            $scope.GetUserDistributionList = response;
            $scope.UserDistribution(response);



        })
        .error(function (response, status) {
            // $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }


    $scope.UserDistribution = function (data) {
      
        // 基于准备好的dom，初始化echarts实例
        // 指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('UserDistribution'));

        var geoCoordMap = {
            "海门": [121.15, 31.89],
            "鄂尔多斯": [109.781327, 39.608266],
            "招远": [120.38, 37.35],
            "舟山": [122.207216, 29.985295],
            "齐齐哈尔": [123.97, 47.33],
            "盐城": [120.13, 33.38],
            "赤峰": [118.87, 42.28],
            "青岛": [120.33, 36.07],
            "乳山": [121.52, 36.89],
            "金昌": [102.188043, 38.520089],
            "泉州": [118.58, 24.93],
            "莱西": [120.53, 36.86],
            "日照": [119.46, 35.42],
            "胶南": [119.97, 35.88],
            "南通": [121.05, 32.08],
            "拉萨": [91.11, 29.97],
            "云浮": [112.02, 22.93],
            "梅州": [116.1, 24.55],
            "文登": [122.05, 37.2],
            "上海": [121.48, 31.22],
            "攀枝花": [101.718637, 26.582347],
            "威海": [122.1, 37.5],
            "承德": [117.93, 40.97],
            "厦门": [118.1, 24.46],
            "汕尾": [115.375279, 22.786211],
            "潮州": [116.63, 23.68],
            "丹东": [124.37, 40.13],
            "太仓": [121.1, 31.45],
            "曲靖": [103.79, 25.51],
            "烟台": [121.39, 37.52],
            "福州": [119.3, 26.08],
            "瓦房店": [121.979603, 39.627114],
            "即墨": [120.45, 36.38],
            "抚顺": [123.97, 41.97],
            "玉溪": [102.52, 24.35],
            "张家口": [114.87, 40.82],
            "阳泉": [113.57, 37.85],
            "莱州": [119.942327, 37.177017],
            "湖州": [120.1, 30.86],
            "汕头": [116.69, 23.39],
            "昆山": [120.95, 31.39],
            "宁波": [121.56, 29.86],
            "湛江": [110.359377, 21.270708],
            "揭阳": [116.35, 23.55],
            "荣成": [122.41, 37.16],
            "连云港": [119.16, 34.59],
            "葫芦岛": [120.836932, 40.711052],
            "常熟": [120.74, 31.64],
            "东莞": [113.75, 23.04],
            "河源": [114.68, 23.73],
            "淮安": [119.15, 33.5],
            "泰州": [119.9, 32.49],
            "南宁": [108.33, 22.84],
            "营口": [122.18, 40.65],
            "惠州": [114.4, 23.09],
            "江阴": [120.26, 31.91],
            "蓬莱": [120.75, 37.8],
            "韶关": [113.62, 24.84],
            "嘉峪关": [98.289152, 39.77313],
            "广州": [113.23, 23.16],
            "延安": [109.47, 36.6],
            "太原": [112.53, 37.87],
            "清远": [113.01, 23.7],
            "中山": [113.38, 22.52],
            "昆明": [102.73, 25.04],
            "寿光": [118.73, 36.86],
            "盘锦": [122.070714, 41.119997],
            "长治": [113.08, 36.18],
            "深圳": [114.07, 22.62],
            "珠海": [113.52, 22.3],
            "宿迁": [118.3, 33.96],
            "咸阳": [108.72, 34.36],
            "铜川": [109.11, 35.09],
            "平度": [119.97, 36.77],
            "佛山": [113.11, 23.05],
            "海口": [110.35, 20.02],
            "江门": [113.06, 22.61],
            "章丘": [117.53, 36.72],
            "肇庆": [112.44, 23.05],
            "大连": [121.62, 38.92],
            "临汾": [111.5, 36.08],
            "吴江": [120.63, 31.16],
            "石嘴山": [106.39, 39.04],
            "沈阳": [123.38, 41.8],
            "苏州": [120.62, 31.32],
            "茂名": [110.88, 21.68],
            "嘉兴": [120.76, 30.77],
            "长春": [125.35, 43.88],
            "胶州": [120.03336, 36.264622],
            "银川": [106.27, 38.47],
            "张家港": [120.555821, 31.875428],
            "三门峡": [111.19, 34.76],
            "锦州": [121.15, 41.13],
            "南昌": [115.89, 28.68],
            "柳州": [109.4, 24.33],
            "三亚": [109.511909, 18.252847],
            "自贡": [104.778442, 29.33903],
            "吉林": [126.57, 43.87],
            "阳江": [111.95, 21.85],
            "泸州": [105.39, 28.91],
            "西宁": [101.74, 36.56],
            "宜宾": [104.56, 29.77],
            "呼和浩特": [111.65, 40.82],
            "成都": [104.06, 30.67],
            "大同": [113.3, 40.12],
            "镇江": [119.44, 32.2],
            "桂林": [110.28, 25.29],
            "张家界": [110.479191, 29.117096],
            "宜兴": [119.82, 31.36],
            "北海": [109.12, 21.49],
            "西安": [108.95, 34.27],
            "金坛": [119.56, 31.74],
            "东营": [118.49, 37.46],
            "牡丹江": [129.58, 44.6],
            "遵义": [106.9, 27.7],
            "绍兴": [120.58, 30.01],
            "扬州": [119.42, 32.39],
            "常州": [119.95, 31.79],
            "潍坊": [119.1, 36.62],
            "重庆": [106.54, 29.59],
            "台州": [121.420757, 28.656386],
            "南京": [118.78, 32.04],
            "滨州": [118.03, 37.36],
            "贵阳": [106.71, 26.57],
            "无锡": [120.29, 31.59],
            "本溪": [123.73, 41.3],
            "克拉玛依": [84.77, 45.59],
            "渭南": [109.5, 34.52],
            "马鞍山": [118.48, 31.56],
            "宝鸡": [107.15, 34.38],
            "焦作": [113.21, 35.24],
            "句容": [119.16, 31.95],
            "北京": [116.46, 39.92],
            "徐州": [117.2, 34.26],
            "衡水": [115.72, 37.72],
            "包头": [110, 40.58],
            "绵阳": [104.73, 31.48],
            "乌鲁木齐": [87.68, 43.77],
            "枣庄": [117.57, 34.86],
            "杭州": [120.19, 30.26],
            "淄博": [118.05, 36.78],
            "鞍山": [122.85, 41.12],
            "溧阳": [119.48, 31.43],
            "库尔勒": [86.06, 41.68],
            "安阳": [114.35, 36.1],
            "开封": [114.35, 34.79],
            "济南": [117, 36.65],
            "德阳": [104.37, 31.13],
            "温州": [120.65, 28.01],
            "九江": [115.97, 29.71],
            "邯郸": [114.47, 36.6],
            "临安": [119.72, 30.23],
            "兰州": [103.73, 36.03],
            "沧州": [116.83, 38.33],
            "临沂": [118.35, 35.05],
            "南充": [106.110698, 30.837793],
            "天津": [117.2, 39.13],
            "富阳": [119.95, 30.07],
            "泰安": [117.13, 36.18],
            "诸暨": [120.23, 29.71],
            "郑州": [113.65, 34.76],
            "哈尔滨": [126.63, 45.75],
            "聊城": [115.97, 36.45],
            "芜湖": [118.38, 31.33],
            "唐山": [118.02, 39.63],
            "平顶山": [113.29, 33.75],
            "邢台": [114.48, 37.05],
            "德州": [116.29, 37.45],
            "济宁": [116.59, 35.38],
            "荆州": [112.239741, 30.335165],
            "宜昌": [111.3, 30.7],
            "义乌": [120.06, 29.32],
            "丽水": [119.92, 28.45],
            "洛阳": [112.44, 34.7],
            "秦皇岛": [119.57, 39.95],
            "株洲": [113.16, 27.83],
            "石家庄": [114.48, 38.03],
            "莱芜": [117.67, 36.19],
            "常德": [111.69, 29.05],
            "保定": [115.48, 38.85],
            "湘潭": [112.91, 27.87],
            "金华": [119.64, 29.12],
            "岳阳": [113.09, 29.37],
            "长沙": [113, 28.21],
            "衢州": [118.88, 28.97],
            "廊坊": [116.7, 39.53],
            "菏泽": [115.480656, 35.23375],
            "合肥": [117.27, 31.86],
            "武汉": [114.31, 30.52],
            "大庆": [125.03, 46.58]
        };

       

        var convertData = function (data) {
            var res = [];
            for (var i = 0; i < data.length; i++) {
                var geoCoord = geoCoordMap[data[i].name];
                if (geoCoord) {
                    res.push({
                        name: data[i].name,
                        value: geoCoord.concat(data[i].value)
                    });
                }
            }
            return res;
        };

        var convertedData = [
            convertData(data),
            convertData(data.sort(function (a, b) {
                return b.value - a.value;
            }).slice(0, 6))
        ];


        option = {
            backgroundColor: '#404a59',
            animation: true,
            animationDuration: 1000,
            animationEasing: 'cubicInOut',
            animationDurationUpdate: 1000,
            animationEasingUpdate: 'cubicInOut',
            title: [
                {
                    left: 'center',
                    textStyle: {
                        color: '#fff'
                    }
                },
                {
                    id: 'statistic',
                    right: 120,
                    top: 40,
                    width: 100,
                    textStyle: {
                        color: '#fff',
                        fontSize: 16
                    }
                }
            ],
            toolbox: {
                iconStyle: {
                    normal: {
                        borderColor: '#fff'
                    },
                    emphasis: {
                        borderColor: '#b1e4ff'
                    }
                },
                right:"40"
            },
            brush: {
                outOfBrush: {
                    color: '#abc'
                },
                brushStyle: {
                    borderWidth: 2,
                    color: 'rgba(0,0,0,0.2)',
                    borderColor: 'rgba(0,0,0,0.5)',
                },
                seriesIndex: [0, 1],
                throttleType: 'debounce',
                throttleDelay: 300,
                geoIndex: 0
            },
            geo: {
                map: 'china',
                left: '10',
                right: '35%',
                center: [113.98561551896913, 33.205000490896193],
                zoom: 1.5,
                label: {
                    emphasis: {
                        show: false
                    }
                },
                roam: true,
                itemStyle: {
                    normal: {
                        areaColor: '#323c48',
                        borderColor: '#111'
                    },
                    emphasis: {
                        areaColor: '#2a333d'
                    }
                }
            },
            tooltip: {
                trigger: 'item'
            },
            grid: {
                right: 40,
                top: 100,
                bottom: 40,
                width: '30%'
            },
            xAxis: {
                type: 'value',
                scale: true,
                position: 'top',
                boundaryGap: false,
                splitLine: { show: false },
                axisLine: { show: false },
                axisTick: { show: false },
                axisLabel: { margin: 2, textStyle: { color: '#aaa' } },
            },
            yAxis: {
                type: 'category',
                name: 'TOP 15',
                nameGap: 16,
                axisLine: { show: false, lineStyle: { color: '#ddd' } },
                axisTick: { show: false, lineStyle: { color: '#ddd' } },
                axisLabel: { interval: 0, textStyle: { color: '#ddd' } },
                data: []
            },
            series: [
                {
                    name: '用户数',
                    type: 'scatter',
                    coordinateSystem: 'geo',
                    data: convertedData[0],
                    symbolSize: function (val) {
                        return Math.max(val[2] / 10, 8);
                    },
                    label: {
                        normal: {
                            formatter: '{b}',
                            position: 'right',
                            show: false
                        },
                        emphasis: {
                            show: true
                        }
                    },
                    itemStyle: {
                        normal: {
                            color: '#ddb926'
                        }
                    }
                },
                {
                    name: 'Top 5',
                    type: 'effectScatter',
                    coordinateSystem: 'geo',
                    data: convertedData[1],
                    symbolSize: function (val) {
                        return Math.max(val[2] / 10, 8);
                    },
                    showEffectOn: 'emphasis',
                    rippleEffect: {
                        brushType: 'stroke'
                    },
                    hoverAnimation: true,
                    label: {
                        normal: {
                            formatter: '{b}',
                            position: 'right',
                            show: true
                        }
                    },
                    itemStyle: {
                        normal: {
                            color: '#f4e925',
                            shadowBlur: 10,
                            shadowColor: '#333'
                        }
                    },
                    zlevel: 1
                },
                {
                    id: 'bar',
                    zlevel: 2,
                    type: 'bar',
                    symbol: 'none',
                    itemStyle: {
                        normal: {
                            color: '#ddb926'
                        }
                    },
                    data: []
                }
            ]
        };

        myChart.on('brushselected', renderBrushed);

        // myChart.setOption(option);
        setTimeout(function () {
            myChart.dispatchAction({
                type: 'brush',
                areas: [
                    {
                        geoIndex: 0,
                        brushType: 'polygon',
                        coordRange: [[113.72, 42.85], [113.68, 42.85], [113.5, 42.84], [113.19, 42.77], [112.76, 42.63], [112.6, 42.6], [112.46, 42.6], [112.41, 42.57], [112.05, 42.56], [117.6, 42.56], [117.41, 42.56],
                            [117.25, 42.56], [117.11, 42.56], [117.02, 42.56], [117, 42.56], [116.94, 42.56], [116.94, 42.55], [116.9, 42.5], [116.88, 42.44], [116.88, 42.37], [116.88, 42.41], [116.88, 42.24], [116.92, 42.15],
                            [116.98, 42.09], [117.05, 42.06], [117.19, 41.96], [117.37, 41.9], [117.43, 41.8], [117.49, 41.75], [117.54, 41.68], [117.6, 41.65], [117.62, 41.61], [117.64, 41.59], [117.68, 41.58], [117.7, 41.52],
                            [117.74, 41.5], [117.74, 41.46], [117.8, 41.44], [117.82, 41.41], [117.86, 41.37], [117.9, 41.3], [117.9, 41.36], [117.9, 41.35], [112.09, 40.97], [112.21, 40.7], [112.37, 40.56], [112.39, 40.5],
                            [112.35, 40.46], [112.35, 40.42], [112.35, 40.36], [112.35, 40.42], [112.37, 40.24], [112.37, 40.14], [112.37, 40.09], [112.44, 40.05], [112.46, 40.01], [112.54, 39.98], [112.6, 39.93], [112.68, 39.86],
                            [112.72, 39.8], [112.74, 39.78], [112.76, 39.74], [112.78, 39.7], [112.82, 39.64], [112.82, 39.62], [112.86, 39.58], [112.86, 39.55], [112.88, 39.54], [112.88, 39.52], [112.9, 39.51], [112.91, 39.48],
                            [112.93, 39.43], [112.95, 39.4], [112.97, 39.39], [112.97, 39.37], [112.97, 39.42], [112.97, 39.35], [112.97, 39.21], [112.97, 39.17], [112.97, 39.12], [112.97, 39.02], [112.97, 38.93], [112.97, 38.87],
                            [112.97, 38.85], [112.95, 38.8], [112.95, 38.77], [112.95, 38.76], [112.93, 38.7], [112.91, 38.63], [112.91, 38.61], [112.91, 38.6], [112.9, 38.6], [112.88, 38.54], [112.88, 38.51], [112.86, 38.51],
                            [112.86, 38.46], [112.72, 38.18], [112.68, 38.1], [112.66, 38.07], [112.62, 37.91], [112.56, 37.73], [112.52, 37.63], [112.48, 37.51], [112.44, 37.42], [112.44, 37.40], [112.43, 37.19], [112.43, 37.14],
                            [112.43, 37.08], [112.44, 37.05], [112.46, 37.05], [112.6, 36.95], [112.64, 36.94], [113.07, 36.51], [113.25, 36.41], [113.36, 36.36], [113.46, 36.19], [113.54, 36.13], [113.66, 36.03], [113.78, 36],
                            [113.87, 35.94], [114.03, 35.86], [114.17, 35.79], [114.23, 35.76], [114.3, 35.72], [114.42, 35.66], [114.52, 35.64], [114.58, 35.63], [114.64, 35.63], [114.77, 35.63], [114.89, 35.61], [114.97, 35.6],
                            [115.07, 35.59], [115.15, 35.59], [115.36, 35.59], [115.38, 35.61], [115.56, 35.73], [115.73, 35.89], [116.03, 36.2], [116.3, 36.5], [116.46, 36.72], [116.5, 36.77], [116.54, 36.82], [116.56, 36.82],
                            [116.58, 36.85], [116.6, 36.86], [116.61, 36.91], [116.71, 37.02], [116.73, 37.08], [116.93, 37.44], [116.99, 37.54], [117.03, 37.66], [117.05, 37.73], [117.16, 37.92], [117.24, 38.02], [117.36, 38.13],
                            [117.40, 38.37], [117.36, 38.36], [117.36, 38.55], [117.36, 38.74], [117.36, 39.05], [117.36, 39.14], [117.36, 39.26], [117.38, 39.42], [117.46, 39.74], [117.48, 39.83], [117.48, 39.95], [117.46, 40.09],
                            [117.42, 40.25], [117.22, 40.39], [117.12, 40.46], [117.07, 40.48], [117.05, 40.49], [116.97, 40.53], [116.91, 40.59], [116.83, 40.81], [116.77, 40.87], [116.71, 40.9], [116.56, 40.97], [116.38, 41.05],
                            [116.3, 41.12], [116.26, 41.15], [116.22, 41.21], [116.22, 41.3], [116.22, 41.39], [116.18, 41.44], [116.07, 41.56], [115.99, 41.69], [115.89, 41.78], [115.69, 42.02], [115.66, 42.05], [115.64, 42.08]]
                    }
                ]
            });
        }, 0);

        function renderBrushed(params) {
            var mainSeries = params.batch[0].selected[0];

            var selectedItems = [];
            var categoryData = [];
            var barData = [];
            var maxBar = 15;
            var sum = 0;
            var count = 0;

            for (var i = 0; i < mainSeries.dataIndex.length; i++) {
                var rawIndex = mainSeries.dataIndex[i];
                var dataItem = convertedData[0][rawIndex];
                var pmValue = dataItem.value[2];

                sum += pmValue;
                count++;

                selectedItems.push(dataItem);
            }

            selectedItems.sort(function (a, b) {
                return a.value[2] - b.value[2];
            });

            for (var i = 0; i < Math.min(selectedItems.length, maxBar) ; i++) {
                categoryData.push(selectedItems[i].name);
                barData.push(selectedItems[i].value[2]);
            }

            this.setOption({
                yAxis: {
                    data: categoryData
                },
                xAxis: {
                    axisLabel: { show: !!count }
                },
                title: {
                    id: 'statistic',
                    text: count ? '平均: ' + (sum / count).toFixed(4) : ''
                },
                series: {
                    id: 'bar',
                    data: barData
                }
            });
        }

        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);

    }

    //用户详细信息
    $scope.GetUInfoPage = 1;
    $scope.GetUInfoPagesize = 10;
    $scope.GetUserInfo = function () {
        $scope.anaItem = {
            page: $scope.GetUInfoPage - 1,
            pagesize: $scope.GetUInfoPagesize
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Account/GetUserInfo"
        })
        .success(function (response, status) {
            $scope.GetUserInfoList = response.Result;
            $scope.GetUserInfoListCount = response.Count;
            console.log(response);
        })
        .error(function (response, status) {
            // $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }
    //-----------------------------------------------------------------------
    $scope.GetUserCount();
    $scope.GetUserLineChart()
    $scope.GetUserDistribution();
    $scope.GetUserInfo();
})