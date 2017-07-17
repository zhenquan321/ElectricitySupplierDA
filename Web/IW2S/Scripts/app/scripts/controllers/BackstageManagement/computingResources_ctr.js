var computingResources_ctr = myApp.controller("computingResources_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $filter, myApplocalStorage) {

  $scope.isByDay = true;
  $scope.GetBotList = [];
  $scope.start = "";
  $scope.end = "";
  $scope.aa = "按分钟显示";
  $scope.status = "baidu";
  $scope.showComputingResources = true;
  $scope.showUserInfo = false;
  $scope.m = 0;
  $scope.fenyeNum = 3;
  $scope.activeNum = 1;
  $scope.num_1 = '';


  chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


  //切换
  $scope.showUser = function () {
    $scope.GetUserList();
    $scope.showComputingResources = false;
    $scope.showUserInfo = true;
  }
  $scope.showAdmin = function () {
    $scope.GetLineChartDatasMin();
    $scope.GetLineChartDatasDay();
    $scope.GetBotList();
    $scope.showComputingResources = true;
    $scope.showUserInfo = false;
  }
  //退出
  $scope.off = function () {
    $location.path("/home/main_1").replace();
    $rootScope.userID = "";
    $rootScope.LoginName = "";
    $rootScope.UsrRole = "";
    $rootScope.UsrKey = "";
    $rootScope.UsrNum = "";
    $rootScope.UsrEmail = "";
    //页面
    $rootScope.keyword = "";
    $rootScope.keywordName = "";
    //changexinyuan
    $rootScope.selectStatus = "";
    $rootScope.ZhibokeywordId = "";
    $rootScope.BaidukeywordId = "";
    $rootScope.keywordsListRecord = "";
    $rootScope.getBaiduRecordId = "";
    $cookieStore.remove("userID");
    $cookieStore.remove("LoginName");
    $cookieStore.remove("UsrRole");
    $cookieStore.remove("UsrKey");
    $cookieStore.remove("UsrNum");
    $cookieStore.remove("UsrEmail");
    $cookieStore.remove("keyword");
    $cookieStore.remove("keywordName");
    $cookieStore.remove("ZhibokeywordId");
    $cookieStore.remove("BaidukeywordId");
    $cookieStore.remove("keywordsListRecord");
    $cookieStore.remove("getBaiduRecordId");
    $cookieStore.remove("selectStatus");
  }


  //资源管理部分-------------------------------------------------------------------------------------------------------

  //1.获取当前数据统计
  $scope.GetLineChartDatasDay = function () {
    $scope.isByDay = true;
    $scope.myDate = new Date();
    $scope.minus30d = new Date($scope.myDate.getTime() - 30 * 24 * 60 * 60 * 1000);
    $scope.end = $filter("date")($scope.myDate, "yyyy-MM-dd HH:mm:ss");
    $scope.start = $filter("date")($scope.minus30d, "yyyy-MM-dd HH:mm:ss");
    $scope.aa = "按天数显示";
    $scope.lineChart1();
  };

  $scope.GetLineChartDatasMin = function () {
    $scope.isByDay = false;
    $scope.myDate = new Date();
    $scope.minus6h = new Date($scope.myDate.getTime() - 6 * 60 * 60 * 1000);
    $scope.end = $filter("date")($scope.myDate, "yyyy-MM-dd HH:mm:ss");
    $scope.start = $filter("date")($scope.minus6h, "yyyy-MM-dd HH:mm:ss");
    $scope.aa = "按分钟显示";
    $scope.lineChart2();
  };
  //2.获取bot
  $scope.GetBotList = function () {
    var url = "/api/DataInfo/GetBotList";
    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('computingRusource_ctr>GetBotList');
      $scope.GetBotListShow = response;
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  };

  //图表1
  $scope.lineChart1 = function () {

    var url = "/api/DataInfo/GetLineChartDatas?isByDay=" + $scope.isByDay + "&start=" + $scope.start + "&end=" + $scope.end;
    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('computingRusource_ctr>lineChart1');
      console.log(response);
      //console.log(response);
      $scope.date = response.Legend;
      $scope.date1 = response.XAxis;

      $scope.LineAvProjs1 = response.LineAvProjs;
      $scope.LineAvUsers1 = response.LineAvUsers;
      $scope.LineNewUsers1 = response.LineNewUsers;
      $scope.LineNewProjs1 = response.LineNewProjs;
      $scope.LineNewLinks1 = response.LineNewLinks;
      $scope.LineKeywords1 = response.LineKeywords;
      $scope.LineKwComplete1 = response.LineKwComplete;
      $scope.LineKwWait1 = response.LineKwWait;
      $scope.LineLinks1 = response.LineLinks;
      $scope.LineUsers1 = response.LineUsers;
      $scope.LineProjects1 = response.LineProjects;
      $scope.LineKwSearch1 = response.LineKwSearch;


      $scope.LineAvProjs = response.LineAvProjs[response.LineAvProjs.length - 1];
      $scope.LineAvUsers = response.LineAvUsers[response.LineAvUsers.length - 1];
      $scope.LineLinks = response.LineLinks[response.LineLinks.length - 1];
      $scope.LineUsers = response.LineUsers[response.LineUsers.length - 1];
      $scope.LineProjects = response.LineProjects[response.LineProjects.length - 1];

      $scope.LineAvProjsB = (response.LineAvProjs[response.LineAvProjs.length - 1] / response.LineAvProjs[response.LineAvProjs.length - 2]) - 1;
      $scope.LineAvUsersB = response.LineAvUsers[response.LineAvUsers.length - 1] / response.LineAvUsers[response.LineAvUsers.length - 2] - 1;
      $scope.LineLinksB = response.LineLinks[response.LineLinks.length - 1] / response.LineLinks[response.LineLinks.length - 2] - 1;
      $scope.LineUsersB = response.LineUsers[response.LineUsers.length - 1] / response.LineUsers[response.LineUsers.length - 2] - 1;
      $scope.LineProjectsB = response.LineProjects[response.LineProjects.length - 1] / response.LineProjects[response.LineProjects.length - 2] - 1;

      // pie
      //百度
      $scope.LineKeywords = response.LineKeywords[response.LineKeywords.length - 1];
      $scope.LineKwComplete = response.LineKwComplete[response.LineKwComplete.length - 1];
      $scope.LineKwWait = response.LineKwWait[response.LineKwWait.length - 1];
      $scope.LineKwSearch = response.LineKwSearch[response.LineKwSearch.length - 1];
      //微博
      $scope.LineWBKeywords = response.LineWBKeywords[response.LineWBKeywords.length - 1];
      $scope.LineWBKwComplete = response.LineWBKwComplete[response.LineWBKwComplete.length - 1];
      $scope.LineWBKwWait = response.LineWBKwWait[response.LineWBKwWait.length - 1];
      $scope.LineWBKwSearch = response.LineWBKwSearch[response.LineWBKwSearch.length - 1];
      //搜狗
      $scope.LineSGKeywords = response.LineSGKeywords[response.LineSGKeywords.length - 1];
      $scope.LineSGKwComplete = response.LineSGKwComplete[response.LineSGKwComplete.length - 1];
      $scope.LineSGKwWait = response.LineSGKwWait[response.LineSGKwWait.length - 1];
      $scope.LineSGKwSearch = response.LineSGKwSearch[response.LineSGKwSearch.length - 1];
      //微信
      $scope.LineWXKeywords = response.LineWXKeywords[response.LineWXKeywords.length - 1];
      $scope.LineWXKwComplete = response.LineWXKwComplete[response.LineWXKwComplete.length - 1];
      $scope.LineWXKwWait = response.LineWXKwWait[response.LineWXKwWait.length - 1];
      $scope.LineWXKwSearch = response.LineWXKwSearch[response.LineWXKwSearch.length - 1];
      //图片
      $scope.LineImgKeywords = response.LineImgKeywords[response.LineImgKeywords.length - 1];
      $scope.LineImgKwComplete = response.LineImgKwComplete[response.LineImgKwComplete.length - 1];
      $scope.LineImgKwWait = response.LineImgKwWait[response.LineImgKwWait.length - 1];
      $scope.LineImgKwSearch = response.LineImgKwSearch[response.LineImgKwSearch.length - 1];

      $scope.lineChartUser = function () {
        // 基于准备好的dom，初始化echarts实例
        // 指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('lineChartUser'));
        option = {

          tooltip: {
            trigger: 'axis'
          },

          legend: {
            data: ['用户数', '活跃用户数']
          },
          xAxis: {
            type: 'category',
            boundaryGap: false,
            data: $scope.date1
          },
          yAxis: {
            name: '用户数',
            type: 'value',
            axisLabel: {
              formatter: '{value}'
            }
          },
          series: [
            {
              name: '用户数',
              type: 'line',
              data: $scope.LineUsers1,
            },

            {
              name: '活跃用户数',
              type: 'line',
              data: $scope.LineAvUsers1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            }, {
              name: '新增用户数',
              type: 'line',
              data: $scope.LineNewUsers1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            }

          ]
        };


        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
      };
      $scope.lineChartProject = function () {
        // 基于准备好的dom，初始化echarts实例
        // 指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('lineChartProject'));
        option = {

          tooltip: {
            trigger: 'axis'
          },
          legend: {
            data: ['活跃项目', '项目']
          },
          calculable: true,
          xAxis: [
            {
              type: 'category',
              data: $scope.date1
            }
          ],
          yAxis: [
            {
              name: '项目数',
              type: 'value'
            }
          ],
          series: [
            {
              name: '项目',
              type: 'line',
              data: $scope.LineProjects1,
            },
            {
              name: '活跃项目',
              type: 'line',
              data: $scope.LineAvProjs1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            },

            {
              name: '新增项目',
              type: 'line',
              data: $scope.LineNewProjs1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            },

          ]
        };


        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
      };
      $scope.lineChartUser();
      $scope.lineChartProject();
      $scope.changPieChart('baidu')
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
      $scope.isActiveStart = false;
    });
  };
  //改变饼图
  $scope.changPieChart = function (modal) {
    if (modal == "baidu") {
      $scope.status = "baidu";
      $scope.sum_keywords = $scope.LineKeywords;
      $scope.data1_name = '百度再搜关键词';
      $scope.data2_name = '百度等待关键词';
      $scope.data3_name = '百度完成关键词';
      $scope.data1 = $scope.LineKwSearch;
      $scope.data2 = $scope.LineKwWait;
      $scope.data3 = $scope.LineKwComplete;


      $scope.keyWordsChart();
    } else if (modal == "weibo") {
      $scope.status = "weibo";
      $scope.sum_keywords = $scope.LineWBKeywords;
      $scope.data1_name = '微博再搜关键词';
      $scope.data2_name = '微博等待关键词';
      $scope.data3_name = '微博完成关键词';
      $scope.data1 = $scope.LineWBKwSearch;
      $scope.data2 = $scope.LineWBKwWait;
      $scope.data3 = $scope.LineWBKwComplete;


      $scope.keyWordsChart();
    } else if (modal == "weixin") {
      $scope.status = "weixin";
      $scope.sum_keywords = $scope.LineWXKeywords;
      $scope.data1_name = '微信再搜关键词';
      $scope.data2_name = '微信等待关键词';
      $scope.data3_name = '微信完成关键词';
      $scope.data1 = $scope.LineWXKwSearch;
      $scope.data2 = $scope.LineWXKwWait;
      $scope.data3 = $scope.LineWXKwComplete;


      $scope.keyWordsChart();
    } else if (modal == "sougou") {
      $scope.status = "sougou";
      $scope.sum_keywords = $scope.LineSGKeywords;
      $scope.data1_name = '搜狗再搜关键词';
      $scope.data2_name = '搜狗等待关键词';
      $scope.data3_name = '搜狗完成关键词';
      $scope.data1 = $scope.LineSGKwSearch;
      $scope.data2 = $scope.LineSGKwWait;
      $scope.data3 = $scope.LineSGKwComplete;

      $scope.keyWordsChart();
    } else if (modal == "tupian") {
      $scope.status = "tupian";
      $scope.sum_keywords = $scope.LineImgKeywords;
      $scope.data1_name = '图片再搜关键词';
      $scope.data2_name = '图片等待关键词';
      $scope.data3_name = '图片完成关键词';
      $scope.data1 = $scope.LineImgKwSearch;
      $scope.data2 = $scope.LineImgKwWait;
      $scope.data3 = $scope.LineImgKwComplete;


      $scope.keyWordsChart();
    }
  }
  //饼图
  $scope.keyWordsChart = function () {
    // 基于准备好的dom，初始化echarts实例
    // 指定图表的配置项和数据
    var myChart = echarts.init(document.getElementById('keyWordsChart'));
    option = {
      title: {
        text: $scope.sum_keywords,
        subtext: '关键词总数',
        top: 'bottom',
        left: 'right'
      },
      tooltip: {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
      },
      legend: {
        orient: 'vertical',
        x: 'left',
        data: [$scope.data1_name, $scope.data2_name, $scope.data3_name,]
      },

      calculable: true,
      series: [
        {
          name: '关键词数（占比）',
          type: 'pie',
          radius: [20, 100],
          center: ['50%', 120],
          roseType: 'radius',
          label: {
            normal: {
              show: false
            },
            emphasis: {
              show: true
            }
          },
          lableLine: {
            normal: {
              show: false
            },
            emphasis: {
              show: true
            }
          },
          data: [
            {value: $scope.data1, name: $scope.data1_name},
            {value: $scope.data2, name: $scope.data2_name},
            {value: $scope.data3, name: $scope.data3_name},

          ]
        },
      ]
    };
    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
  };


  //切换信源_关键词数据状态
  $scope.changKeywordData = function (modal) {
    if (modal == "baidu") {
      $scope.LineKwSearchXinyuan = $scope.LineKwSearch1;
      $scope.LineKwWaitXinyuan = $scope.LineKwWait1;
      $scope.LineKwCompleteXinyuan = $scope.LineKwComplete1;

      $scope.LineKeywordsXinyuan = $scope.LineKeywords1;

      $scope.lineChartKeyword();
    } else if (modal == "weibo") {
      $scope.LineKwSearchXinyuan = $scope.LineWBKwSearch1;
      $scope.LineKwWaitXinyuan = $scope.LineWBKwWait1;
      $scope.LineKwCompleteXinyuan = $scope.LineWBKwComplete1;

      $scope.LineKeywordsXinyuan = $scope.LineWBKeywords1;

      $scope.lineChartKeyword();
    } else if (modal == "weixin") {
      $scope.LineKwSearchXinyuan = $scope.LineWXKwSearch1;
      $scope.LineKwWaitXinyuan = $scope.LineWXKwWait1;
      $scope.LineKwCompleteXinyuan = $scope.LineWXKwComplete1;

      $scope.LineKeywordsXinyuan = $scope.LineWXKeywords1;

      $scope.lineChartKeyword();
    } else if (modal == "sougou") {
      $scope.LineKwSearchXinyuan = $scope.LineSGKwSearch1;
      $scope.LineKwWaitXinyuan = $scope.LineSGKwWait1;
      $scope.LineKwCompleteXinyuan = $scope.LineSGKwComplete1;

      $scope.LineKeywordsXinyuan = $scope.LineSGKeywords1;

      $scope.lineChartKeyword();
    } else if (modal == "tupian") {
      $scope.LineKwSearchXinyuan = $scope.LineImgKwSearch1;
      $scope.LineKwWaitXinyuan = $scope.LineImgKwWait1;
      $scope.LineKwCompleteXinyuan = $scope.LineImgKwComplete1;

      $scope.LineKeywordsXinyuan = $scope.LineImgKeywords1;

      $scope.lineChartKeyword();
    }
  }

  $scope.lineChart2 = function () {

    var url = "/api/DataInfo/GetLineChartDatas?isByDay=" + $scope.isByDay + "&start=" + $scope.start + "&end=" + $scope.end;
    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('computingRusource_ctr>lineChart2');
      console.log(response);
      $scope.date = response.Legend;
      $scope.date1 = response.XAxis;
      $scope.LineAvProjs1 = response.LineAvProjs;
      $scope.LineAvUsers1 = response.LineAvUsers;
      $scope.LineLinks1 = response.LineLinks;
      $scope.LineUsers1 = response.LineUsers;
      $scope.LineProjects1 = response.LineProjects;


      //百度
      $scope.LineKeywords1 = response.LineKeywords;
      $scope.LineKwComplete1 = response.LineKwComplete;
      $scope.LineKwWait1 = response.LineKwWait;
      $scope.LineKwSearch1 = response.LineKwSearch;
      $scope.LineNewLinks1 = response.LineNewLinks;

      //微信
      $scope.LineWXKeywords1 = response.LineWXKeywords;
      $scope.LineWXKwComplete1 = response.LineWXKwComplete;
      $scope.LineWXKwWait1 = response.LineWXKwWait;
      $scope.LineWXKwSearch1 = response.LineWXKwSearch;
      $scope.LineWXNewLinks1 = response.LineWXNewLinks;
      //搜狗
      $scope.LineSGKeywords1 = response.LineSGKeywords;
      $scope.LineSGKwComplete1 = response.LineSGKwComplete;
      $scope.LineSGKwWait1 = response.LineSGKwWait;
      $scope.LineSGKwSearch1 = response.LineSGKwSearch;
      $scope.LineSGNewLinks1 = response.LineSGNewLinks;
      //微博
      $scope.LineWBKeywords1 = response.LineWBKeywords;
      $scope.LineWBKwComplete1 = response.LineWBKwComplete;
      $scope.LineWBKwWait1 = response.LineWBKwWait;
      $scope.LineWBKwSearch1 = response.LineWBKwSearch;
      $scope.LineWBNewLinks1 = response.LineWBNewLinks;
      //搜图
      $scope.LineImgKeywords1 = response.LineImgKeywords;
      $scope.LineImgKwComplete1 = response.LineImgKwComplete;
      $scope.LineImgKwWait1 = response.LineImgKwWait;
      $scope.LineImgKwSearch1 = response.LineImgKwSearch;
      $scope.LineImgNewLinks1 = response.LineImgNewLinks;

      //默认加载百度关键词数据状态
      $scope.LineKwSearchXinyuan = $scope.LineKwSearch1;
      $scope.LineKwWaitXinyuan = $scope.LineKwWait1;
      $scope.LineKwCompleteXinyuan = $scope.LineKwComplete1;
      $scope.LineKeywordsXinyuan = $scope.LineKeywords1;


      $scope.lineChartKeyword = function () {
        // 基于准备好的dom，初始化echarts实例
        // 指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('lineChartKeyword'));

        option = {
          tooltip: {
            trigger: 'axis'
          },
          dataZoom: [
            {
              show: true,
              realtime: true,
              start: 50,
              end: 100
            },
            {
              type: 'inside',
              realtime: true,
              start: 50,
              end: 100
            }
          ],
          legend: {
            data: ['在搜关键词', '等待关键词', '完成关键词', '总关键词']
          },
          grid: {
            left: '3%',
            right: '4%',
            bottom: '13%',
            containLabel: true
          },
          xAxis: [
            {
              type: 'category',
              data: $scope.date1
            }
          ],
          yAxis: [
            {
              type: 'value',
              name: '关键词数',
              axisLabel: {
                formatter: '{value}'
              }
            },
            {
              type: 'value',
              name: '在搜关键词',
              axisLabel: {
                formatter: '{value}'
              }
            }
          ],
          series: [
            {
              name: '在搜关键词',
              type: 'line',
              yAxisIndex: 1,
              data: $scope.LineKwSearchXinyuan

            },
            {
              name: '等待关键词',
              type: 'bar',
              data: $scope.LineKwWaitXinyuan
            },
            {
              name: '完成关键词',
              type: 'bar',
              data: $scope.LineKwCompleteXinyuan
            },
            {
              name: '总关键词',
              type: 'bar',
              data: $scope.LineKeywordsXinyuan
            }
          ]
        };


        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
      };

      $scope.lineChartLinks = function () {
        // 基于准备好的dom，初始化echarts实例
        // 指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('lineChartLinks'));
        option = {

          tooltip: {
            trigger: 'axis'
          },
          legend: {
            data: ['百度新增链接数', '微信新增链接数', '搜狗新增链接数', '微博新增链接数', '搜图新增链接数']
          },
          //toolbox: {
          //    show: true,
          //    feature: {
          //        dataView: { show: true, readOnly: false },
          //        magicType: { show: true, type: ['line', 'bar'] },
          //    }
          //},
          dataZoom: [
            {
              show: true,
              realtime: true,
              start: 50,
              end: 100
            },
            {
              type: 'inside',
              realtime: true,
              start: 50,
              end: 100
            }
          ],
          calculable: true,
          grid: {
            left: '3%',
            right: '4%',
            bottom: '13%',
            containLabel: true
          },
          xAxis: [
            {
              type: 'category',
              data: $scope.date1
            }
          ],
          yAxis: [
            {
              name: '连接数',
              type: 'value'
            }
          ],
          series: [
            {
              name: '百度新增链接数',
              type: 'line',
              data: $scope.LineNewLinks1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            },
            {
              name: '微信新增链接数',
              type: 'line',
              data: $scope.LineWXNewLinks1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            },
            {
              name: '搜狗新增链接数',
              type: 'line',
              data: $scope.LineSGNewLinks1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            },
            {
              name: '微博新增链接数',
              type: 'line',
              data: $scope.LineWBNewLinks1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            },
            {
              name: '搜图新增链接数',
              type: 'line',
              data: $scope.LineImgNewLinks1,
              markPoint: {
                data: [
                  {type: 'max', name: '最大值'},
                  {type: 'min', name: '最小值'}
                ]
              },
              markLine: {
                data: [
                  {type: 'average', name: '平均值'}
                ]
              }
            },
          ]
        };


        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
      };
      $scope.lineChartKeyword();
      $scope.lineChartLinks();
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
      $scope.isActiveStart = false;
    });
  };

  $scope.GetLineChart1 = function () {

    $scope.GetLineChartDatasMin();
    $scope.GetLineChartDatasDay();
  };

  //用户管理部分-------------------------------------------------------------------------------------------------------
  $scope.GetUserList = function () {
    var url = "/api/Account/GetUserList";
    var q = $http.get(url);
    q.success(function (response, status) {
      $rootScope.userList = response;
      $scope.skipTo(1);
      console.log(response);
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  }
  $scope.jumpToUserResource = function (id,name) {
    $rootScope.currentId = id;
    $rootScope.currentName = name;
    $cookieStore.put("currentId", $rootScope.currentId);
    $cookieStore.put("currentName", $rootScope.currentName);
    $location.path("/modelSelect").replace();
  }
  //前端分页函数

  $scope.skipTo = function (Num1) {
    var m = Math.ceil($rootScope.userList.length / 10);
    $scope.m = m;
    var Num = parseInt(Num1);
    if (parseInt(Num1) && Num1 > 0 && Num1 <= m) {
      if (Num == 1) {
        $scope.activeNum = 1;
      } else if (Num == 2) {
        $scope.activeNum = 2;
      } else {
        $scope.activeNum = 3;
      }
      $scope.fenyeNum2 = Num;
      if (Num < 3) {
        $scope.fenyeNum = 3;
      } else {
        $scope.fenyeNum = Num;
      }
      $scope.userList1 = $rootScope.userList.slice((Num - 1) * 10, ((Num - 1) * 10 + 10));
    }
    if (Num1 == 'lastPage') {
      if (Num == 1) {
        $scope.activeNum = 1;
      } else if (Num == 2) {
        $scope.activeNum = 2;
      } else {
        $scope.activeNum = 3;
      }
      $scope.fenyeNum2 = m;
      $scope.fenyeNum = m;
      $scope.userList1 = $rootScope.userList.slice((m - 1) * 10, ((m - 1) * 10 + 10));
    }
  }

  //自动加载________________________________________________________________________________________________________
  //默认加载资源管理部分
  $scope.GetLineChartDatasMin();
  $scope.GetLineChartDatasDay();
  $scope.GetBotList();

});