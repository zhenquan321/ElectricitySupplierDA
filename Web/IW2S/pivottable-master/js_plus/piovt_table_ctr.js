var piovt_table_ctr = myApp.controller("piovt_table_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout) {

    $scope.famhfa_s = true;
    $scope.showAmendAndOff = false;
    $scope.show_list = 1;
    $scope.bumen_show_num = 1;
    $scope.bumen_show_ban = false;
    $scope.CustServDeptIds = "";
    $scope.CustServDepts = [];//
    $scope.State = ""; //案件状态
    $scope.ProofMode = "";//取证方式
    $scope.CaseNo = "";//案件号
    $scope.ExecDeptIds = "";//执行部门_id
    $scope.ExecDepts = [];//执行部门
    $scope.CaseName = "";//案件名称
    $scope.CustServMana = "";//客户经理
    $scope.Objective = "";//
    $scope.ExecMana = "";//执行部门
    $scope.Page = 0;
    $scope.PageSize = 1000000;

    $scope.state_show_num = 0;
    $scope.ProofMode_show_num = 0;
    $scope.select_sc = '案件号';
    $scope.search_kw = '';
    $scope.select_sc_n = 1;
    $scope.select_sc_n_if = 0;
    $scope.is_xiangqing = false;
    $scope.search_kw_if = "";
    $scope.xiangqing_anj = "";

    $scope.select_sc_1 = "全部部门";
    $scope.select_sc_n_1 = 0;
    $scope.select_sc_2 = "全部地区";
    $scope.select_sc_n_2 = 0;
    $scope.select_sc_3 = "全部经理";

    $scope.select_sc_n_3 = 0;
    $scope.diqu_bumen = [];
    $scope.diqu_bumen_show = false;
    $scope.BudgetRatio = 0;
    $scope.Deptstype = 0;//1：执行部门，2：客服部门
    $scope.shuju_state_show_num = 0;

    $scope.search_tj_show = false;
    $scope.search_tj_1_show = false;

    $scope.search_tj_2_show = false;
    $scope.search_tj_3_show = false;
    $scope.GetCase_list_count = 0;
    $scope.xiala_s = false;
    $scope.duibi_s = true;
    $scope.selected_tu_num_2 = 0;
    $scope.selected_tu_n_2 = '补充图';

    $rootScope.selected_page = 'IPRWorx';
    $rootScope.selected_page_2 = "执行绩效管理";
    $scope.selected_tu_n = '补充图';
    $scope.selected_tu_num = 0;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http);

    //\\________________________________________________________________

    //获取用户与项目ID
    $scope.allID= function (){
        $rootScope.userID = window.parent.document.getElementById('userID').innerHTML;
        $rootScope.getProjectId = window.parent.document.getElementById('projectID').innerHTML;
        $scope.GetCase();
    }
  



  
    //获取连接
    $scope.GetCase = function () {
        var url = "/api/Keyword/GetLinkReport?userId=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response)
            $scope.GetCase_list = response;
            $scope.GetCase_list_count = $scope.GetCase_list.length;
           
            $scope.is_xiangqing_fun_x_2($scope.GetCase_list[0]);
            $scope.pottle_table();
            //创建对比处
            if (!$scope.duibi_s) {
                $scope.pottle_table_2();
            }
        });
        q.error(function (e) {
            $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
        });
    }
    //设置搜索条件
    $scope.search_tj_show_fun = function () {
        $scope.search_tj_show = !$scope.search_tj_show;
    }

    $scope.search_tj = function (num, st) {
        $scope.search_tj_show = !$scope.search_tj_show;
        $scope.select_sc = st;
        $scope.select_sc_n = num;
    }
    //案件状态
    $scope.select_state = function (select, num) {
        $scope.State = select;
        $scope.state_show_num = num;
        $scope.GetCase();
    }
    //数据数量
    $scope.shuju_select_state = function (select, num) {
        if (select!='所有') {
            $scope.PageSize = select;
        } else {
            $scope.PageSize = 100000;
        }
        $scope.shuju_state_show_num = num;
        $scope.GetCase();
    }
    //取证方式
    $scope.select_ProofMode = function (select, num) {
        $scope.ProofMode = select;
        $scope.ProofMode_show_num = num;
        $scope.GetCase();
    }


    //加载更多
    $scope.GetCase_more = function () {
        $scope.PageSize = $scope.PageSize + 20;
        $scope.GetCase();
    }
    //切换详情
    $scope.is_xiangqing_fun = function () {
        $scope.is_xiangqing = !$scope.is_xiangqing;
    }

    $scope.is_xiangqing_fun_x_2 = function (x) {
        $scope.xiangqing_anj = x;
        $scope.BudgetRatio = $scope.xiangqing_anj.BudgetRatio * 100;
        $scope.BudgetRatio = $scope.BudgetRatio.toFixed(2);
        $timeout(function () {
           
        }, 1000);

    }
    $scope.is_xiangqing_fun_x = function (x) {
        $scope.xiangqing_anj = x;
        $scope.BudgetRatio = $scope.xiangqing_anj.BudgetRatio * 100;
        $scope.BudgetRatio = $scope.BudgetRatio.toFixed(2);
        $timeout(function () {
           
        }, 1000);

        var scroll_offset = $("#dashboard").offset();  //得到pos这个div层的offset，包含两个值，top和left
        $("body,html").animate({
            scrollTop: scroll_offset.top - 90 //让body的scrollTop等于pos的top，就实现了滚动
        }, 500);


    }
    //显示下拉
    $scope.xiala_s_fun =function(num){
        $scope.xiala_s = !$scope.xiala_s;
        if (num == 1) {
            $scope.GetCase();
        }
    }
 
    //fangda
    $scope.nei_rs_c6 = {
        'margin-top': '-8px'
    }
    $scope.fangda_fun = function () {
        $scope.famhfa_s = !$scope.famhfa_s;
        var winWidth = 0;
        var winWidth = 0;
        // 获取窗口宽度 
        if (window.innerWidth){
            winWidth = window.innerWidth;
        }
        else if ((document.body) && (document.body.clientWidth)) {
            winWidth = document.body.clientWidth;
        }
        // 获取窗口高度 
        if (window.innerHeight){
            winHeight = window.innerHeight;
        }
        else if ((document.body) && (document.body.clientHeight)) {
            winHeight = document.body.clientHeight;
        }
           
        if ($scope.famhfa_s != false) {


            $scope.famhfa = {
                "position": "fixed",
                "width": winWidth,
                "height": winHeight,
                "top": "0",
                "left": "0",
                "z-index": "100000",
                'overflow': 'auto',
            }
          
            $scope.nei_rs = {
                "width": winWidth - 90,
                "height": winHeight - 110,
                "margin": '20px 10px 10px',
                "position":"relative"
            }
            $scope.nei_rs_c6 = {
                "height": winHeight - 110,
            }

            var a = (winHeight - 110) * 0.9
            var obj = document.getElementsByClassName("pvtVertList");
            obj[0].height = a + 'px';
        } else {
            $scope.nei_rs_c6 = {
               
            }
            $scope.famhfa = {
               

            }
            $scope.nei_rs = {
                "margin": '20px',
                "position": "relative"
            }
        }
       

    }


    $scope.fangda_fun_2 = function () {
        if ($scope.famhfa_s == true) {
            var winWidth = 0;
            var winWidth = 0;
            // 获取窗口宽度 
            if (window.innerWidth) {
                winWidth = window.innerWidth;
            }
            else if ((document.body) && (document.body.clientWidth)) {
                winWidth = document.body.clientWidth;
            }
            // 获取窗口高度 
            if (window.innerHeight) {
                winHeight = window.innerHeight;
            }
            else if ((document.body) && (document.body.clientHeight)) {
                winHeight = document.body.clientHeight;
            }
            $scope.famhfa = {
                "position": "fixed",
                "width": winWidth,
                "height": winHeight,
                "top": "0",
                "left": "0",
                "z-index": "100000",
                'overflow': 'auto',
            }
            $scope.nei_rs = {
                "width": winWidth - 90,
                "height": winHeight - 110,
                "margin": '20px 10px 10px',
                "position": "relative"
            }
            $scope.nei_rs_c6 = {
                "height": winHeight - 110,
            }
            var a=(winHeight - 110) * 0.9
            var obj = document.getElementsByClassName("pvtVertList");
            obj[0].height = a+'px';
        }
    }
    $(window).resize(function () {
        $scope.$apply(function () {
            $scope.fangda_fun_2();
        });
    });

    //切换图表
    $scope.change_chart = function (num) {
        $scope.selected_tu_num = num;
        if(num==1){
            $scope.selected_tu_n = '地图';
        } else if (num ==0) {
            $scope.selected_tu_n = '补充图';
        } else if (num == 2) {
            $scope.selected_tu_n = '矩形树图';
        } else if (num == 3) {
            $scope.selected_tu_n = '气泡图';
        }
        $scope.GetPageExtract(1);
    }

    //切换图表_2
    $scope.change_chart_2 = function (num) {
        $scope.selected_tu_num_2 = num;
        if (num == 1) {
            $scope.selected_tu_n_2 = '地图';
        } else if (num == 0) {
            $scope.selected_tu_n_2 = '补充图';
        } else if (num == 2) {
            $scope.selected_tu_n_2 = '矩形树图';
        } else if (num == 3) {
            $scope.selected_tu_n_2 = '气泡图';
        }
        $scope.GetPageExtract(2);
    }
    //++  ______ 补充图 _________________________________________
    //导出ex
    $scope.GetExcel = function () {
        var test = document.getElementsByClassName('pvtTable')[0].innerHTML;
        $scope.paramsList = {
            code: test,
        };
        var urls = "/api/User/GetExcel";
        var q = $http.post(
                urls,
               JSON.stringify($scope.paramsList),
               {
                   headers: {
                       'Content-Type': 'application/json'
                   }
               }
            )
        q.success(function (response, status) {
            console.log(response);
            $scope.url = response.code;
            var domain = window.location.host;
            domain = 'http://' + domain+'/';
            window.location.href = domain + $scope.url;
           
        });
        q.error(function (e) {
            $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
        });
    }
    //上传页面内容
    $scope.GetPageExtract = function (num) {
        var num = num;
        // var test = document.getElementsByClassName('pvtTable')[0].innerHTML;
        if(num==1){
             var test = $("#output").find(".pvtTable")[0].innerHTML;
        }else{
            var test = $("#output2").find(".pvtTable")[0].innerHTML;
        }
        $scope.paramsList = {
            code: test,
        };
        var urls = "/api/User/GetPageExtract";
        var q = $http.post(
                urls,
               JSON.stringify($scope.paramsList),
               {
                   headers: {
                       'Content-Type': 'application/json'
                   }
               }
            )
        q.success(function (response, status) {
            console.log(response);
            for (var i = 0; i < response.length; i++) {
                for (var a = 0 ; a < response[i].ItemArray.length; a++) {
                    if (response[i].ItemArray[a] == null ) {
                        response[i].ItemArray.splice(a, 1);
                        a = a - 1;
                    }
                }
            }
            $scope.PageExtract_list = response;
            console.log($scope.PageExtract_list);
         
            if (num==1) {
                if ($scope.selected_tu_num == 1) {
                    // $scope.selected_tu_n = '地图';
                    $scope.ditu_s(1);
                } else if ($scope.selected_tu_num == 0) {
                    // $scope.selected_tu_n = '补充图';
                } else if ($scope.selected_tu_num == 2) {
                    //  $scope.selected_tu_n = '矩形树图';
                } else if ($scope.selected_tu_num == 3) {
                    // $scope.selected_tu_n = '气泡图';
                }
            } else if (num == 2) {
                if ($scope.selected_tu_num_2 == 1) {
                    // $scope.selected_tu_n = '地图';
                    $scope.ditu_s(2);
                } else if ($scope.selected_tu_num_2 == 0) {
                    // $scope.selected_tu_n = '补充图';
                } else if ($scope.selected_tu_num_2 == 2) {
                    //  $scope.selected_tu_n = '矩形树图';
                } else if ($scope.selected_tu_num_2 == 3) {
                    // $scope.selected_tu_n = '气泡图';
                }
            }
            
           
        });
        q.error(function (e) {
            $scope.alert_fun('danger', '哎呀，网络打盹了，请重试一下！');
        });
    }
    //获取数据值

    //获取城市
    $scope.ditu_getCite = function () {
        $.getJSON("cityData.txt", function (data) {
            $scope.Mydata = {};
            for (var i = 0; i < data.length; i++) {
                $scope.datavalue = [];
                data1 = '';
                datacity = '';
                $scope.datavalue.push(data[i].longitude);
                $scope.datavalue.push(data[i].latitude);
                data1 = data[i].province + data[i].city;
                datacity = data[i].city;
                $scope.Mydata[data1] = $scope.datavalue;
                $scope.Mydata[datacity] = $scope.datavalue;
            }
            var geoCoordMap = {
                '西藏': [91.11, 29.97],
                '上海': [121.48, 31.22],
                '福建': [119.3, 26.08],
                '山西': [112.53, 37.87],
                '云南': [102.73, 25.04],
                '辽宁': [123.38, 41.8],
                '吉林': [125.35, 43.88],
                '宁夏': [106.27, 38.47],
                '青海': [101.74, 36.56],
                '内蒙古': [111.65, 40.82],
                '四川': [104.06, 30.67],
                '陕西': [108.95, 34.27],
                '重庆': [106.54, 29.59],
                '江苏': [118.78, 32.04],
                '贵州': [106.71, 26.57],
                '北京': [116.46, 39.92],
                '新疆': [87.68, 43.77],
                '浙江': [120.19, 30.26],
                '山东': [117, 36.65],
                '甘肃': [103.73, 36.03],
                '天津': [117.2, 39.13],
                '河南': [113.65, 34.76],
                '黑龙江': [126.63, 45.75],
                '河北': [114.48, 38.03],
                '安徽': [117.27, 31.86],
                '湖北': [114.31, 30.52],
                '海南': [109.511909, 18.252847],
                '江西': [115.89, 28.68],
                '湖南': [113, 28.21],
                '广东': [113.23, 23.16],
                '广西': [108.33, 22.84],
                '台湾': [121.27, 23.86],
                '香港': [114.2, 22.30],
                '澳门': [113.60, 22.15],
            };
            $scope.Mydata = Object.assign($scope.Mydata, geoCoordMap);
            console.log($scope.Mydata);
        })
    }

    //地图
    $scope.ditu_s = function (num) {
        if (num==1) {
            var myChart = echarts.init(document.getElementById('ditu_chart'));
        }else if(num==2){
            var myChart = echarts.init(document.getElementById('ditu_chart2'));
        }
        var geoCoordMap = $scope.Mydata;
        var data = [];
        var cite_list = [];
        var data_nei = [];
        var aa = 0;
        var name_S=[];
        if ($scope.PageExtract_list[0]) {
            aa = $scope.PageExtract_list[0].ItemArray.length;
            aa = (aa - 1) / 2;
        }
        for (var bb = 0; bb < aa; bb++) {
            data_nei.push({ name: "", date_list: [] });
            var ee = bb * 2 + 2;
            var ec = ee-1;
            data_nei[bb].name = $scope.PageExtract_list[0].ItemArray[ec];
            name_S[bb]= $scope.PageExtract_list[0].ItemArray[ec];
            for (var cc = 0; cc < $scope.PageExtract_list.length; cc++) {
                data_nei[bb].date_list.push({ name: '', value:0 })
                data_nei[bb].date_list[cc].value = parseFloat($scope.PageExtract_list[cc].ItemArray[ee].replace(",",""));
                data_nei[bb].date_list[cc].name = $scope.PageExtract_list[cc].ItemArray[0];
            }
            var data_c = data_nei[bb].date_list;
           
            var res = [];
            var max_num = 0;
            for (var i = 0; i < data_c.length; i++) {
                var geoCoord = geoCoordMap[data_c[i].name];
                if (geoCoord) {
                    res.push({
                        name: data_c[i].name,
                        value: geoCoord.concat(data_c[i].value)
                    });
                }
                if (bb == (aa - 1)) {
                    if (i == (data_c.length-1)) {
                      
                        } else {
                        if (max_num < data_c[i].value) {
                            max_num = data_c[i].value;
                        }
                    }
                } else {
                    if (max_num < data_c[i].value) {
                        max_num = data_c[i].value;
                    }
                }
                
            }
            var daxiao_k = max_num / 50;
            console.log(res);
            data.push({
                name: data_nei[bb].name,
                type: 'scatter',
                coordinateSystem: 'geo',
                data: res,
                symbolSize: function (val) {
                    return val[2] / daxiao_k+10;
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
                zlevel: bb+1,
            }
             )
        }
        //选中数据
        var selected_shuju = new Object();
        for (var gg = 0; gg < name_S.length;gg++){
            if (gg != name_S.length - 1) {
                var as = name_S[gg];
                selected_shuju[as] = false;
            } else {
                var as = name_S[gg];
                selected_shuju[as] = true;

            }
        }
        console.log(data_nei);
        console.log(data);
        console.log(name_S);
        console.log(selected_shuju);
        option = {
            color: ['#c23531', '#427d84', '#d48265', '#91c7ae', '#345a73', '#ca8622', '#bda29a', '#6e7074', '#20323e', '#ddb926'],
            backgroundColor: '#404a59',
            title: {
                text: '地域分布',
                subtext: '可点击图表下方模块类型，切换查看不同的类型数据',
                left: 'center',
                textStyle: {
                    color: '#fff'
                },
                top:'20'

            },
            legend: {
                data: name_S,
                selected: selected_shuju,
                textStyle: {
                    color: '#fff',
                    fontSize: 16
                },
                bottom: '20',
                backgroundColor: 'rgba(255, 255, 255, 0.15)'
            },
            tooltip: {
                formatter: function (obj) {
                    var value = obj.value;
                    return '<div style="border-bottom: 1px solid rgba(255,255,255,.3); font-size: 18px;padding-bottom: 7px;margin-bottom: 7px">'
                        + obj.name + '</div>' + value[2] + '<br>'
                }
            },
            geo: {
                map: 'china',
                label: {
                    emphasis: {
                        show: false
                    }
                },
                roam: true,
                itemStyle: {
                    normal: {
                        areaColor: '#fff',
                        borderColor: '#111'
                    },
                    emphasis: {
                        areaColor: '#e8e8e8'
                    }
                }
            },
            series: data
        };

        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
    }

    $scope.ditu_s_2 = function () {
        var myChart = echarts.init(document.getElementById('ditu_chart'));
        var geoCoordMap = $scope.Mydata;
        var data = [];
        var cite_list = [];
        var data_nei = [];
        var aa = 0;
        var name_S = [];
        if ($scope.PageExtract_list[0]) {
            aa = $scope.PageExtract_list[0].ItemArray.length;
            aa = (aa - 1) / 2;
        }
        for (var bb = 0; bb < aa; bb++) {
            data_nei.push({ name: "", date_list: [] });
            var ee = bb * 2 + 2;
            var ec = ee - 1;
            data_nei[bb].name = $scope.PageExtract_list[0].ItemArray[ec];
            name_S[bb] = $scope.PageExtract_list[0].ItemArray[ec];
            for (var cc = 0; cc < $scope.PageExtract_list.length; cc++) {
                data_nei[bb].date_list.push({ name: '', value: 0 })
                data_nei[bb].date_list[cc].value = parseFloat($scope.PageExtract_list[cc].ItemArray[ee].replace(",", ""));
                data_nei[bb].date_list[cc].name = $scope.PageExtract_list[cc].ItemArray[0];
            }
            var data_c = data_nei[bb].date_list;

            var res = [];
            var max_num = 0;
            for (var i = 0; i < data_c.length; i++) {
                var geoCoord = geoCoordMap[data_c[i].name];
                if (geoCoord) {
                    res.push({
                        name: data_c[i].name,
                        value: geoCoord.concat(data_c[i].value)
                    });
                }
                if (max_num < data_c[i].value) {
                    max_num = data_c[i].value;
                }
            }

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

            var res_2 = [];

            var rse_3 = data_c.length.sort(function (a, b) {
                return b.value - a.value;
            }).slice(0, 6)
            for (var i = 0; i < rse_3.length; i++) {
                var geoCoord = geoCoordMap[rse_3[i].name];
                if (geoCoord) {
                    res_2.push({
                        name: rse_3[i].name,
                        value: geoCoord.concat(rse_3[i].value)
                    });
                }
            }
            var convertedData = [
              res,res_2
            ];
            var res_4=[bb]

            var daxiao_k = max_num / 50;
            console.log(res);
            data.push({
                name: data_nei[bb].name,
                type: 'scatter',
                coordinateSystem: 'geo',
                data: res_4,
                symbolSize: function (val) {
                    return val[2] / daxiao_k + 10;
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
                zlevel: bb + 1,
            }
             )
        }
        data.push({
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
        });
        //选中数据
        var selected_shuju = new Object();
        for (var gg = 0; gg < name_S.length; gg++) {
            if (gg != name_S.length - 1) {
                var as = name_S[gg];
                selected_shuju[as] = false;
            } else {
                var as = name_S[gg];
                selected_shuju[as] = true;

            }
        }
        console.log(data_nei);
        console.log(data);
        console.log(name_S);
        console.log(selected_shuju);
        option = {
            backgroundColor: '#404a59',
            animation: true,
            animationDuration: 1000,
            animationEasing: 'cubicInOut',
            animationDurationUpdate: 1000,
            animationEasingUpdate: 'cubicInOut',
            title: [
                {
                    text: '全国主要城市 PM 2.5',
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
                }
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
            legend: {
                data: name_S,
                selected: selected_shuju,
                textStyle: {
                    color: '#fff',
                    fontSize: 16
                },
                bottom: '20'
            },
            tooltip: {
                formatter: function (obj) {
                    var value = obj.value;
                    return '<div style="border-bottom: 1px solid rgba(255,255,255,.3); font-size: 18px;padding-bottom: 7px;margin-bottom: 7px">'
                        + obj.name + '</div>' + value[2] + '<br>'
                }
            },
            geo: {
                map: 'china',
                left: '10',
                right: '35%',
                center: [117.98561551896913, 31.205000490896193],
                zoom: 2.5,
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
                name: 'TOP 20',
                nameGap: 16,
                axisLine: { show: false, lineStyle: { color: '#ddd' } },
                axisTick: { show: false, lineStyle: { color: '#ddd' } },
                axisLabel: { interval: 0, textStyle: { color: '#ddd' } },
                data: []
            },
            series: data
        };
        myChart.on('brushselected', renderBrushed);
        setTimeout(function () {
            myChart.dispatchAction({
                type: 'brush',
                areas: [
                    {
                        geoIndex: 0,
                        brushType: 'polygon',
                        coordRange: [[119.72, 34.85], [119.68, 34.85], [119.5, 34.84], [119.19, 34.77], [118.76, 34.63], [118.6, 34.6], [118.46, 34.6], [118.33, 34.57], [118.05, 34.56], [117.6, 34.56], [117.41, 34.56], [117.25, 34.56], [117.11, 34.56], [117.02, 34.56], [117, 34.56], [116.94, 34.56], [116.94, 34.55], [116.9, 34.5], [116.88, 34.44], [116.88, 34.37], [116.88, 34.33], [116.88, 34.24], [116.92, 34.15], [116.98, 34.09], [117.05, 34.06], [117.19, 33.96], [117.29, 33.9], [117.43, 33.8], [117.49, 33.75], [117.54, 33.68], [117.6, 33.65], [117.62, 33.61], [117.64, 33.59], [117.68, 33.58], [117.7, 33.52], [117.74, 33.5], [117.74, 33.46], [117.8, 33.44], [117.82, 33.41], [117.86, 33.37], [117.9, 33.3], [117.9, 33.28], [117.9, 33.27], [118.09, 32.97], [118.21, 32.7], [118.29, 32.56], [118.31, 32.5], [118.35, 32.46], [118.35, 32.42], [118.35, 32.36], [118.35, 32.34], [118.37, 32.24], [118.37, 32.14], [118.37, 32.09], [118.44, 32.05], [118.46, 32.01], [118.54, 31.98], [118.6, 31.93], [118.68, 31.86], [118.72, 31.8], [118.74, 31.78], [118.76, 31.74], [118.78, 31.7], [118.82, 31.64], [118.82, 31.62], [118.86, 31.58], [118.86, 31.55], [118.88, 31.54], [118.88, 31.52], [118.9, 31.51], [118.91, 31.48], [118.93, 31.43], [118.95, 31.4], [118.97, 31.39], [118.97, 31.37], [118.97, 31.34], [118.97, 31.27], [118.97, 31.21], [118.97, 31.17], [118.97, 31.12], [118.97, 31.02], [118.97, 30.93], [118.97, 30.87], [118.97, 30.85], [118.95, 30.8], [118.95, 30.77], [118.95, 30.76], [118.93, 30.7], [118.91, 30.63], [118.91, 30.61], [118.91, 30.6], [118.9, 30.6], [118.88, 30.54], [118.88, 30.51], [118.86, 30.51], [118.86, 30.46], [118.72, 30.18], [118.68, 30.1], [118.66, 30.07], [118.62, 29.91], [118.56, 29.73], [118.52, 29.63], [118.48, 29.51], [118.44, 29.42], [118.44, 29.32], [118.43, 29.19], [118.43, 29.14], [118.43, 29.08], [118.44, 29.05], [118.46, 29.05], [118.6, 28.95], [118.64, 28.94], [119.07, 28.51], [119.25, 28.41], [119.36, 28.28], [119.46, 28.19], [119.54, 28.13], [119.66, 28.03], [119.78, 28], [119.87, 27.94], [120.03, 27.86], [120.17, 27.79], [120.23, 27.76], [120.3, 27.72], [120.42, 27.66], [120.52, 27.64], [120.58, 27.63], [120.64, 27.63], [120.77, 27.63], [120.89, 27.61], [120.97, 27.6], [121.07, 27.59], [121.15, 27.59], [121.28, 27.59], [121.38, 27.61], [121.56, 27.73], [121.73, 27.89], [122.03, 28.2], [122.3, 28.5], [122.46, 28.72], [122.5, 28.77], [122.54, 28.82], [122.56, 28.82], [122.58, 28.85], [122.6, 28.86], [122.61, 28.91], [122.71, 29.02], [122.73, 29.08], [122.93, 29.44], [122.99, 29.54], [123.03, 29.66], [123.05, 29.73], [123.16, 29.92], [123.24, 30.02], [123.28, 30.13], [123.32, 30.29], [123.36, 30.36], [123.36, 30.55], [123.36, 30.74], [123.36, 31.05], [123.36, 31.14], [123.36, 31.26], [123.38, 31.42], [123.46, 31.74], [123.48, 31.83], [123.48, 31.95], [123.46, 32.09], [123.34, 32.25], [123.22, 32.39], [123.12, 32.46], [123.07, 32.48], [123.05, 32.49], [122.97, 32.53], [122.91, 32.59], [122.83, 32.81], [122.77, 32.87], [122.71, 32.9], [122.56, 32.97], [122.38, 33.05], [122.3, 33.12], [122.26, 33.15], [122.22, 33.21], [122.22, 33.3], [122.22, 33.39], [122.18, 33.44], [122.07, 33.56], [121.99, 33.69], [121.89, 33.78], [121.69, 34.02], [121.66, 34.05], [121.64, 34.08]]
                    }
                ]
            });
        }, 0);


        function renderBrushed(params) {
            var mainSeries = params.batch[0].selected[0];

            var selectedItems = [];
            var categoryData = [];
            var barData = [];
            var maxBar = 30;
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
    //显示对比
    $scope.duibi_s_fun = function () {
        $scope.duibi_s = !$scope.duibi_s;
        $timeout(function () {
            $scope.caozuo();
        }, 300);
    }
    $scope.caozuo = function () {
        if (!$scope.GetCase_list) {
            return
        }
        if ($scope.duibi_s == true) {
            $scope.pottle_table();
            if ($scope.selected_tu_num == 1) {
                $scope.ditu_s(1);
            }
        } else {
            $scope.pottle_table();
            if ($scope.selected_tu_num == 1) {
                $scope.ditu_s(1);
            }
            $scope.pottle_table_2();
            if ($scope.selected_tu_num == 2) {
                $scope.ditu_s(2);
            }
        }
    }
    //生成交叉表
    $scope.pottle_table = function () {
        var derivers = $.pivotUtilities.derivers;
        var renderers = $.extend($.pivotUtilities.renderers, $.pivotUtilities.c3_renderers);
        $("#output").pivotUI($scope.GetCase_list, {
            renderers: renderers,
            //derivedAttributes: {
            //    "Age Bin": derivers.bin("Age", 10),
            //    "Gender Imbalance": function (mp) {
            //        return mp["Gender"] == "Male" ? 1 : -1;
            //    }
            //},
            //cols: ["年度", "月份"],
            //rows: ["行业大类"],
            cols: [],
            rows: ["域名分组", '域名','连接标题','命中关键词数','网页关连数','评论数','连接影响力'],
            vals: [],
            rendererName: "Heatmap",
            aggregatorName: "Count",
        });
    }

    //再次生成一个交叉表
    $scope.pottle_table_2 = function () {
        var derivers = $.pivotUtilities.derivers;
        var renderers = $.extend($.pivotUtilities.renderers, $.pivotUtilities.c3_renderers);
        $("#output2").pivotUI($scope.GetCase_list, {
            renderers: renderers,
            //derivedAttributes: {
            //    "Age Bin": derivers.bin("Age", 10),
            //    "Gender Imbalance": function (mp) {
            //        return mp["Gender"] == "Male" ? 1 : -1;
            //    }
            //},
            //cols: ["年度", "月份"],
            //rows: ["行业大类"],
            cols: ["行业大类"],
            rows: ['市'],
            vals: ['完成工作量'],
            rendererName: "Heatmap",
            aggregatorName: "Sum",
        });
    }

    //-----------------------------------------------------------------------
  
    $scope.allID();


 

});