var ReportContents_ctr = myApp.controller("ReportContents_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $interval, $filter, $timeout, $anchorScroll, $modal, myApplocalStorage) {
    $scope.changeBig = 1;
    $scope.maodianNum = 1;
    $scope.leftFloat = {};
    $scope.MouseOn = 0;
    $scope.XiugaiEditer = 0;
    $scope.ChangeTJMSShow = 0;
    $scope.M2description = '';
    $scope.model2 = $scope;
    $scope.chartType = 0;
    $scope.keXiuGai = false;
    $scope.timeInter = 1;
    $scope.ChangeTJMSShow2 = 0;
    $scope.categoryId = '';
    $scope.reportRDecShow = false;
    $scope.leavingMessageShow = true;
    $rootScope.pinglun = false;
    $scope.num = 120;
    $scope.num2 = 1200;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    $scope.keXiuGai = $rootScope.keXiuGai;
    $scope.selsetReport = $rootScope.selsetReport;
    $scope.reportModal_LX = $rootScope.reportModal_LX;
    //展示评论
    $scope.leavingMessageShowFun = function () {
        $scope.leavingMessageShow = !$scope.leavingMessageShow;
    }
    //评论
    $rootScope.huifu = function () {
        $rootScope.pinglun = true;
        $("#textarea_ping").focus();
    }
    $rootScope.huifu_close = function () {
        $rootScope.pinglun = false;
    }
    //获取分享评论
    $rootScope.GetShareOutComment = function () {

        $scope.anaItem = {
            prjId: $scope.selsetReport._id,
            opereateType: $scope.num,
            page: 0,
            pagesize: 100,
            siteSource: $scope.num2,
        };
        $http({
            method: 'get',
            params: $scope.anaItem,
            url: "/api/Share/GetShareOutComment"
        })
            .success(function (response, status) {
                console.log(response);
                $rootScope.ShareOutComment_list = response.Result;
                $rootScope.ShareOutComment_Count = response.Count;
            })
            .error(function (response, status) {
                $scope.alert_fun('danger', "服务器连接出错");
            });
    }
    //新增分享评论
    $rootScope.InsertShareOutComment = function () {
        $scope.paramsList = {
            UserId: $rootScope.userID,
            ShareOperateType: $scope.num,
            ProjectId: $scope.selsetReport._id,
            SiteSource: $scope.num2,
            Comment: $rootScope.ShareOutComment,
        };
        var urls = "/api/Share/InsertShareOutComment";
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
            if (response.IsSuccess == true) {
                $rootScope.GetShareOutComment();
                $rootScope.pinglun = false;
                $rootScope.ShareOutComment = "";
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.alert_fun('danger', "服务器连接出错");
        });
    }
    //删除分享评论
    $rootScope.DelShareOutComment = function (id) {
        if (confirm("您确认删除此评论吗？")) {
            $scope.anaItem = {
                prjId: $scope.selsetReport._id,
                usr_id: $rootScope.userID,
                commentId: id,
            };
            $http({
                method: 'get',
                params: $scope.anaItem,
                url: "/api/Share/DelShareOutComment"
            })
                .success(function (response, status) {
                    $scope.alert_fun('success', "删除评论成功！");
                    $rootScope.GetShareOutComment($scope.num, $scope.num2);
                })
                .error(function (response, status) {
                    $scope.alert_fun('danger', "服务器连接出错");
                });
        }

    }
   
    //滚动轴监听________________________________________________________________________________________________________
    $scope.maodian = function (id, num) {
        var id = "#" + id;
        $("html,body").animate({ scrollTop:( $(id).offset().top-80)}, 500);
        $scope.maodianNum = num;
        //$location.hash(id);
        //$anchorScroll();
    };
    $(window).scroll(function () {
        $scope.ChangeFloat = function () {
            var scrollTop = document.body.scrollTop || document.documentElement.scrollTop;
            //通过判断滚动条的top位置与可视网页之和与整个网页的高度是否相等来决定是否加载内容；  
            var widthRight = document.getElementById('floatRightList').offsetWidth-16;
            if (scrollTop > 100) {
                $scope.leftFloat = {
                    'position': 'fixed',
                    'width': widthRight,
                    'top': '65px',
                }
            } else {
                $scope.leftFloat = {
                    'position': 'static',
                    'width': '100%',
                    'top': '0px',
                };
            }
            var height1 = document.getElementById('modal1').offsetHeight +100;
            var height2 = document.getElementById('modal2').offsetHeight + height1;
            var height3 = document.getElementById('modal3').offsetHeight + height2;
            var height4 = document.getElementById('modal4').offsetHeight + height3;
            //var height5 = document.getElementById('modal5').offsetHeight + height4;
            var height6 = document.getElementById('modal6').offsetHeight + height4;
            var height7 = document.getElementById('modal7').offsetHeight + height6;
            if (scrollTop < height1) {
                $scope.maodianNum = 1;
            }
            if (scrollTop < height2 && scrollTop > height1) {
                $scope.maodianNum = 2;
            }
            if (scrollTop < height3 && scrollTop > height2) {
                $scope.maodianNum = 3;
            }
            if (scrollTop < height4 && scrollTop > height3) {
                $scope.maodianNum = 4;
            }
            if (scrollTop < height6 && scrollTop > height4) {
                $scope.maodianNum = 6;
            }
            if (scrollTop < height7 && scrollTop > height6) {
                $scope.maodianNum = 7;
            }
        }
        $scope.$apply(function () {
            $scope.ChangeFloat();
        });
    })
    //修改可修改状态
    $scope.ChangeXG = function (state) {
        $scope.keXiuGai = state;
    }
    //显示修改描述_____________________________________________________________________________________________
    $scope.XiugaiEditerFun = function (num ,data) {
        $scope.XiugaiEditer = num;
        $scope.chartType = 0;
        if (num >= 100 && num < 200) {
            $timeout(function () {
                $('#model1input').val(data.Title);
                $("#customized-buttonpane").html(data.Description);
                $("textarea[name='customized-buttonpane']").val(data.Description)
            },300)
        }
        if (num >= 600 && num < 700) {
            $timeout(function () {
                $('#model6Title').val(data.Title);
                $('#model6Description').val(data.Description);
            }, 100)
        }
    }
    
    //model1___________________________________________________________________________________________________
    $scope.model1 = $scope;
    //1.添加描述
    $scope.InsertDescription = function (isStart) {
        var JBdescription = document.getElementsByClassName('trumbowyg-textarea')[0].value;
        $scope.model1Title = $('#model1input').val();
        $scope.paramsList = {
            reportId: $scope.selsetReport._id,
            description: JBdescription,
            title:$scope.model1Title,
            id: '',
            isStart: isStart,
        };
        var urls = "/api/Report/InsertDescription";
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
            if (response.IsSuccess==true) {
                $scope.alert_fun('success', '描述添加成功！');
                $scope.GetDescription(isStart);
                $scope.XiugaiEditerFun(0,null)
            } else {
                $scope.alert_fun('worning', response.Message);
            }
        });
        q.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    }

    //2.获取描述
    $scope.GetDescription = function (isStart) {
        var url = "/api/Report/GetDescription?reportId=" + $scope.selsetReport._id + '&isHide=' + true + '&isStart=' + isStart;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (isStart) {
                $scope.GetDescriptionMS = response.Result;
            } else {
                $scope.GetDescriptionMSEnd = response.Result;
            }
            console.log(response.Result);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    $scope.GetDescription(true);
    $timeout(function () {
        $scope.GetDescription(false);
    },2000);
    //删除报告描述
    $scope.DelDescription = function (id,isStart) {
        var url = "/api/Report/DelDescription?descId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess==true) {
                $scope.alert_fun('success', '报告删除成功！');
                $scope.GetDescription(isStart);
            } else {
                $scope.alert_fun('danger',response.Message);
            }

        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //修改报告描述
    $scope.UpdateDescription = function (id, isStart) {
        var JBdescription = document.getElementsByClassName('trumbowyg-textarea')[0].value;
        $scope.model1Title = $('#model1input').val();
        $scope.paramsList = {
            reportId: $scope.selsetReport._id,
            description: JBdescription,
            title: $scope.model1Title,
            id: id,
            isStart: isStart,
        };
        var urls = "/api/Report/UpdateDescription";
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
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '报告修改成功！');
                $scope.GetDescription(isStart);
                $scope.XiugaiEditerFun(0, null);
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    }
    //显示修改_________________________________________________________________________________________________
    $scope.MouseOnShow = function (num) {
        $scope.MouseOn = num;
    }
    //model2___________________________________________________________________________________________________
    //获取关键词
    $scope.model2_getKeyword = function (isReRead) {
        if (isReRead == true) {
            if (confirm("您确定要更新关键词吗？")) {

            } else {
                return;
            }
        }
        var url = "/api/Report/GetKeywordCate?reportId=" + $scope.selsetReport._id + '&isReRead=' + isReRead + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.model2_getKeywordList = response.Result;
            console.log(response)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //隐藏关检测___未完
    $scope.HideKeywordCate = function (isReRead) {
        //if (isReRead == true) {
        //    if (confirm("您确定要更新关键词吗？")) {
        //    } else {
        //        return;
        //    }
        //}
        var url = "/api/Report/HideKeywordCate?keyCateIds=" + $scope.selsetReport._id + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.model2_getKeywordList = response.Result;
            console.log(response)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    $scope.model2_getKeyword(false)
    //model3__________________________________________________________________________________________________
    //获取统计信息
    $scope.GetStatistics = function (isReRead) {
        if (isReRead == true) {
            if (confirm("您要更新统计数据吗？")) {

            } else {
                return;
            }
        }
        var url = "/api/Report/GetStatistics?reportId=" + $scope.selsetReport._id + '&isReRead=' + isReRead + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetStatisticsList = response;
            console.log(response)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //更新统计信息描述
    $scope.UpdateStatistics = function (id) {
        var url = "/api/Report/UpdateStatistics?staId=" +$scope.GetStatisticsList._id + '&description=' + $scope.M2description;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            if (response.IsSuccess==true) {
                $scope.GetStatistics(false);
                $scope.alert_fun('suceess', '统计信息描述更新成功！');
                $scope.ChangeTJMS(0);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //更新统计描述方法
    $scope.ChangeTJMS = function (num) {
        $scope.ChangeTJMSShow = num;
        if(num==1){
             $scope.M2description = $scope.GetStatisticsList.Description;
        } else if (num == 2) {
            $scope.M2Kwtbdescription = $scope.GetKeywordChartList.Description;
        }else if(num==51){
            $scope.GetDCdescription = $scope.GetDomainChartDate.Description;
        }
        //if ($scope.ChangeTJMSShow != 1) {
        //    $scope.GetStatistics(true);
        //}
    }
    //遍历——更新统计描述方法
    $scope.ChangeTJMS2 = function (num,data) {
        $scope.ChangeTJMSShow2 = num;
        if (num >= 4600 && num < 4700) {
            $scope.LinkChartListIpTitle = data.Title;
            $scope.LinkChartListIpDescription = data.Description;
        } else if (num >= 4800 && num < 4900) {
            $scope.DomainChartListIpTitle = data.Title;
            $scope.LinkChartListIpDescription = data.Description;
        }
    }


    //获取矩形图和圆形图
    $scope.GetKeywordChart = function (isReRead) {
        if (isReRead == true) {
            if (confirm("您要更新词组图表数据吗？")) {

            } else {
                return;
            }
        }
        var url = "/api/Report/GetKeywordChart?reportId=" + $scope.selsetReport._id + '&isReRead=' + isReRead + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response)
            $scope.GetKeywordChartList = response;
            $timeout(function () {
                $scope.ecahrtsJZ();
                $scope.D3getJson();
            }, 300)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    $scope.GetKeywordChart(false);
    //矩形图
    $scope.ecahrtsJZ = function () {
        var myChart = echarts.init(document.getElementById('juxingtu'));
        //var jsonUrl = "Scripts/app/data/echarts.txt";
        var rawData = $scope.GetKeywordChartList.Chart_RectangularTree;
        eval("rawData=" + rawData);
            function convert(source, target, basePath) {
                for (var key in source) {
                    var path = basePath ? (basePath + '>' + key) : key;
                    if (key.match(/^\$/)) {

                    }
                    else {
                        target.children = target.children || [];
                        var child = {
                            name: path
                        };
                        target.children.push(child);
                        convert(source[key], child, path);
                    }
                }

                if (!target.children) {
                    target.value = source.$count || 1;
                }
                else {
                    target.children.push({
                        name: basePath,
                        value: source.$count
                    });
                }
            }

            var data = [];
            convert(rawData, data, '');
            myChart.setOption(option = {
                title: {
                    text: '',
                    left: 'leafDepth'
                },
                tooltip: {},
                series: [{
                    name: '总览',
                    type: 'treemap',
                    visibleMin: 100,
                    data: data.children,
                    leafDepth: 2,
                    levels: [
                      {
                          itemStyle: {
                              normal: {
                                  borderColor: '#555',
                                  borderWidth: 2,
                                  gapWidth: 2
                              }
                          }
                      },
                      {
                          colorSaturation: [0.3, 0.6],
                          itemStyle: {
                              normal: {
                                  borderColorSaturation: 0.7,
                                  gapWidth: 1,
                                  borderWidth: 1
                              }
                          }
                      },
                      {
                          colorSaturation: [0.3, 0.5],
                          itemStyle: {
                              normal: {
                                  borderColorSaturation: 0.6,
                                  gapWidth: 1
                              }
                          }
                      },
                      {
                          colorSaturation: [0.3, 0.5]
                      }
                    ]
                }]
            })
    }
    //圆形d3图
    $scope.D3getJson = function () {
        //Radial Reingold–Tilford Tree_______________________
        var root = $scope.GetKeywordChartList.Chart_CategoryTree;
        eval("root=" + root);
        var diameter = 400;
        var tree = d3.layout.tree()
          .size([360, diameter / 2 - 120])
          .separation(function (a, b) {
              return (a.parent == b.parent ? 1 : 2) / a.depth;
          });

        var diagonal = d3.svg.diagonal.radial()
          .projection(function (d) {
              return [d.y, d.x / 180 * Math.PI];
          });

        var svg2 = d3.select("#Tilford-Tree").append("svg")
          .attr("width", diameter)
          .attr("height", diameter)
          .append("g")
          .attr("transform", "translate(" + diameter / 2 + "," + diameter / 2 + ")");
        d3.select(self.frameElement).style("height", diameter - 50 + "px");
        var nodes = tree.nodes(root),
            links = tree.links(nodes);

        var link = svg2.selectAll(".link")
            .data(links)
            .enter().append("path")
            .attr("class", "link")
            .attr("d", diagonal);

        var node = svg2.selectAll(".node")
            .data(nodes)
            .enter().append("g")
            .attr("class", "node")
            .attr("transform", function (d) {
                return "rotate(" + (d.x - 90) + ")translate(" + d.y + ")";
            })

        node.append("circle")
            .attr("r", 4.5);

        node.append("text")
            .attr("dy", ".31em")
            .attr("text-anchor", function (d) {
                return d.x < 180 ? "start" : "end";
            })
            .attr("transform", function (d) {
                return d.x < 180 ? "translate(8)" : "rotate(180)translate(-8)";
            })
            .text(function (d) {
                return d.name;
            });
    }

    //更新关键词分析图表描述：
    $scope.UpdateKeywordChart = function (id) {
        var url = "/api/Report/UpdateKeywordChart?keyChartId=" + $scope.GetKeywordChartList._id + '&description=' + $scope.M2Kwtbdescription;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            if (response.IsSuccess == true) {
                $scope.GetKeywordChart(false);
                $scope.alert_fun('suceess', '关键词分析图表描述更新成功！');
                $scope.ChangeTJMS(0);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
   

    $scope.GetStatistics(false);
    //model3  ++ 结束__________________________________________________________________________________________________
    //model4  ___________________________________________________________________________________________________________
    //获取接检测概况图
    $scope.GetLinkOverview = function (isReRead) {
        if (isReRead == true) {
            if (confirm("您要更新词组图表数据吗？")) {

            } else {
                return;
            }
        }
        var url = "/api/Report/GetLinkOverview?reportId=" + $scope.selsetReport._id + '&isReRead=' + isReRead + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.GetLinkOverviewData = response;
            $scope.D_lineChart();
            $scope.D_PieChart();
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    $scope.GetLinkOverview(false);

    //更新检测概况图描述：
    $scope.UpdateLinkOverview = function (id) {
        var url = "/api/Report/UpdateLinkOverview?overviewId=" + $scope.GetLinkOverviewData._id + '&description=' + $scope.JCGKdescription + '&title=' + $scope.JCGKdescription;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            if (response.IsSuccess == true) {
                $scope.GetLinkOverview(false);
                $scope.alert_fun('suceess', '检测概况图描述更新成功！');
                $scope.ChangeTJMS(0);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //获取连接数
    $scope.GetTimeLinkDetails = function (isReRead) {
        var url = "/api/Report/GetTimeLinkDetails?reportId=" + $scope.selsetReport._id + '&isReRead=' + isReRead + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.GetTimeLinkDetailsDeta = response;
            $scope.skipTo(1);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    $scope.GetTimeLinkDetails(false);
    //点击获取连接
    $scope.GetCateTimeLink = function (time, categoryId) {
        var url = "/api/Report/GetCateTimeLink?reportId=" + $scope.selsetReport._id + '&categoryId=' + categoryId + '&pubTime=' + time + "&timeInterval=7";
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.GetTimeLinkDetailsDeta = response;
            $scope.skipTo(1);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

  
    //前端分页函数
    $scope.skipTo = function (Num1) {
        $scope.TimeLinkList = $scope.GetTimeLinkDetailsDeta;
        var m = Math.ceil($scope.TimeLinkList.length / 10);
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
            $scope.TimeLinkList1 = $scope.TimeLinkList.slice((Num - 1) * 10, ((Num - 1) * 10 + 10));
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
            $scope.TimeLinkList1 = $scope.TimeLinkList.slice((m - 1) * 10, ((m - 1) * 10 + 10));
        }
    }
  
    //放大折线图
    $scope.ChangeBigFun = function (num, selectedTime) {
        $scope.changeBig = num;
        $timeout(function () {
            $scope.D_lineChart();
            $scope.D_PieChart();
        }, 200);
    }
    //有效链接图
    $scope.D_lineChart = function (timeValue, config) {
        var lines;
        var zhaiyao;
        if (timeValue == null && config == null) {
            lines = $scope.GetLinkOverviewData;
            zhaiyao = $scope.GetLinkOverviewData.PieChartData;
            $scope.D_lineChartSet(lines, zhaiyao);
        }
        else {
            var timeInterval = 7;
            if (!config) {
                if (timeValue != null && timeValue != undefined) {
                    timeInterval = timeValue;
                }
              
            } else {
                $scope.startTime = config.startTime;
                $scope.endTime = config.endTime;
                $scope.data.percent = config.percent;
                $scope.data.topNum = config.topNum;
                timeInterval = config.timeInterval;
                $scope.categoryId = config.categoryId;
                for (var i = 0; i < $scope.timeOptions.length; i++) {
                    if ($scope.timeOptions[i].value == timeInterval) {
                        $scope.selectedTime = $scope.timeOptions[i];
                    }
                }
                if (i == 3) {
                    $scope.selectedTime = $scope.timeOptions[3];
                }
            }
            $scope.timeInter = timeInterval;
            var url = "/api/Report/GetTempLinkChart?keyCateIds=";
            if (config != null) {
                url += $scope.categoryId;
            }
            url += "&reportId=" + $scope.selsetReport._id + "&startTime=" + "&endTime=" + "&topNum=" + $scope.data.topNum + "&sumNum=" + $scope.data.topNum + "&timeInterval=" + timeInterval + "&percent=" + $scope.data.percent;
            var q = $http.get(url);
            q.success(function (response, status) {
                if (timeValue == null && config == null) {
                    lines = response;
                  
                    zhaiyao = response.PieChartData;
                    $scope.D_lineChartSet(lines, zhaiyao);
                }
                else {
                    $scope.D_lineChart_shai(response);
                }
            });
            q.error(function(response) {
                $scope.error = "服务器连接出错";
                $scope.isActiveStart = false;
            });
        }
    }
    $scope.D_lineChartSet = function (lines, zhaiyao) {
        console.log(lines);
            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('D_lineChart'));
            var timeData1 = lines.Times;
            //console.log(response.Times);
            var linkData = lines.LineChartData;
            if (linkData.length > 8) {
                linkData.length = 8;
            }
            var timeData = [];
            for (var j = 0; j < timeData1.length; j++) {
                timeData[j] = $filter("date")(timeData1[j], "yyyy-MM-dd");
            }
            option = {
                title: {
                    padding: [
						0, 0, 0, 30
                    ],
                    text: '传播事件统计图',
                    //subtext: '副标题'
                },
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: function () {
                        var biaoti = [];
                        for (var i = 0; i < linkData.length; i++) {
                            biaoti.push(linkData[i].name);
                        };
                        return biaoti;
                    }(),
                    //orient: 'vertical',
                    x: "160px"
                },
                grid: {
                    left: '6%',
                    right: '6%',
                    bottom: '15%',
                    containLabel: true
                },
                dataZoom: [{
                    type: 'slider',
                    height: 10,
                    show: true,
                    xAxisIndex: [0],
                    start: $scope.startNum,
                    end: $scope.endNum
                }],
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: timeData
                },
                yAxis: {
                    type: 'value',
                    name: '影响力链接',
                    axisLabel: {
                        formatter: '{value}'
                    }
                },
                series: (function () {
                    var serie = [];
                    for (var i = 0; i < linkData.length; i++) {
                        var zhaiyaoList = [];
                        var cc = 0;
                        for (aa = 0; aa < zhaiyao.length; aa++) {
                            if (zhaiyao[aa].CategoryName == linkData[i].name) {
                                zhaiyaoList[cc] = zhaiyao[aa];
                                cc++;
                            }
                        }
                        console.log(zhaiyaoList);
                        var xyName = [];
                        var coord = [];
                        for (var j = 0; j < zhaiyaoList.length; j++) {
                            var item = {
                                name: '摘要：' + zhaiyaoList[j].Summary,
                                xAxis: $filter("date")(zhaiyaoList[j].X, "yyyy-MM-dd"),
                                yAxis: zhaiyaoList[j].Y,
                            }
                            xyName.push(item);
                        }
                        console.log(xyName);
                        var item = {
                            name: linkData[i].name,
                            type: 'line',
                            data: linkData[i].LinkCount,
                            markPoint: {
                                data: xyName,
                            }
                        }
                        serie.push(item);
                    };
                    return serie;
                })()
            };
            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
            myChart.on('datazoom', function (params) {
                $scope.startTime = timeData[parseInt((params.start * timeData.length) / 100)];
                $scope.endTime = timeData[parseInt((params.end * timeData.length) / 100)];
                if (params.end == 100) {
                    $scope.endTime = timeData[parseInt((params.end * timeData.length) / 100) - 1];
                }
                $scope.startNum = Math.round(params.start);
                $scope.endNum = Math.round(params.end);
            });
            //自动摘要
            myChart.on('click', function (params) {
                console.log(params);
                var index=params.seriesIndex;
                var CategoryId = lines.LineChartData[index].CategoryId;
                var time = params.data.xAxis;
                $scope.GetCateTimeLink(time, CategoryId);
                $scope.data.searchTime = time;
                console.log(time);
            });
    };
    $scope.D_PieChart = function (date) {
        var date = $scope.GetLinkOverviewData.PieChartData;
        var myChart = echarts.init(document.getElementById('D_PieChart'));
        console.log(date);
        var data_n = [];
        var date_a = [];
        for (var i = 0; i < date.length; i++) {
            if (date[i].Summary) {
                data_n[i] = date[i].CategoryName + ':' + date[i].Summary.substring(0, 13);
            } else {
                data_n[i] = date[i].CategoryName + ':' + date[i].Summary;
            }
            if (date[i].Summary) {
                date_a.push({
                    value: date[i].Y,
                    name: date[i].Summary.substring(0, 18)
                })
            } else {
                date_a.push({
                    value: date[i].Y,
                    name: date[i].Summary
                })
            }
        }

        var title = '传播内容占比';
        var title_f = '仅计算Top' + date.length + '链接数占比，不算总值';
        option = {
            title: {
                text: title,
                subtext: title_f,
                x: 'center'
            },
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            series: [{
                label: {
                    normal: {
                        show: true,
                        textStyle: {
                            fontSize: 8,
                        },
                    },
                    emphasis: {
                        show: true
                    },
                },
                name: '相关话题',
                type: 'pie',
                radius: ['20%', '35%'],
                center: ['50%', '50%'],
                data: date_a,
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                },

            }]
        };

        myChart.setOption(option);

    }
    //自动摘要切换
    $scope.changeLink = function () {
        $scope.showLink = !$scope.showLink;
    }
    //有效链接图清除按钮
    $scope.clearModel = function () {
        $scope.data.percent = 0;
        $scope.data.topNum = 0;
    }
    //插入链接分析图表组
    $scope.Modelindex = 1;
    $scope.InsertLinkChartCate = function () {
        var ISLCdescription = document.getElementsByClassName('trumbowyg-textarea')[0].value;
        $scope.ISLCtitle = $('#model4input').val();
        var url = "/api/Report/InsertLinkChartCate?reportId=" + $scope.selsetReport._id + '&title=' + $scope.ISLCtitle + '&description=' + ISLCdescription + '&index=' + $scope.Modelindex;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess==true) {
                $scope.alert_fun('success', '模块创建成功！');
                $scope.XiugaiEditerFun(0, null);
                $scope.GetLinkChartCate(false);
            } else {
                $scope.alert_fun('danger', response.Message)
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

  




    //获取链接分析图表组
    $scope.GetLinkChartCate = function (isReRead) {
        if (isReRead == true) {
            if (confirm("您要更新词组图表数据吗？")) {
            } else {
                return;
            }
        }
        var url = "/api/Report/GetLinkChartCate?reportId=" + $scope.selsetReport._id + '&isReRead=' + isReRead + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.GetLinkChartCateData = response;
            $timeout(function () {
                $scope.bianliFun();
            },200)
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    $scope.GetLinkChartCate(false);


    //生成遍历的图表。
    $scope.bianliFun = function () {
        for (var a = 0; a < $scope.GetLinkChartCateData.length;a++){
            var LinkChartList = $scope.GetLinkChartCateData[a].LinkChartList;
            var DomainChartList = $scope.GetLinkChartCateData[a].DomainChartList;

            for (var i = 0; i < LinkChartList.length; i++) {
                var LineDomID = 'D_lineChart' + (i + 4200 + (a + 1) * 10000);
                var PieDomID = 'D_pieChart' + (i + 4200 + (a + 1) * 10000);
                $scope.D_lineChart_bianli(LineDomID, LinkChartList[i]);
                $scope.D_PieChart_bianli(PieDomID, LinkChartList[i]);
            };
          
            for(var i = 0; i < DomainChartList.length; i++ ){
                var QPDomId = 'D_QPChart' + (i + 4200 + (a + 1) * 10000);
                $scope.D_QPChart_bianli(QPDomId, DomainChartList[i])
            }
        }
    }
    $scope.D_lineChart_bianli = function (id,data) {
        var response = data;
        var zhaiyao = data.PieChartData;
        //基于准备好的dom，初始化echarts实例
        //指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById(id));
        var timeData1 = response.Times;
        //console.log(response.Times);
        var linkData = response.LineChartData;
        if (linkData.length > 8) {
            linkData.length = 8;
        }
        var timeData = [];
        for (var j = 0; j < timeData1.length; j++) {
            timeData[j] = $filter("date")(timeData1[j], "yyyy-MM-dd");
        }
        option = {
            title: {
                padding: [
                    0, 0, 0, 30
                ],
                text: '传播事件统计图',
                //subtext: '副标题'
            },
            tooltip: {
                trigger: 'axis'
            },
            legend: {
                data: function () {
                    var biaoti = [];
                    for (var i = 0; i < linkData.length; i++) {
                        biaoti.push(linkData[i].name);
                    };
                    return biaoti;
                }(),
                //orient: 'vertical',
                x: "160px"
            },
            grid: {
                left: '6%',
                right: '6%',
                bottom: '15%',
                containLabel: true
            },
            dataZoom: [{
                type: 'slider',
                height: 10,
                show: true,
                xAxisIndex: [0],
                start: $scope.startNum,
                end: $scope.endNum
            }],
            xAxis: {
                type: 'category',
                boundaryGap: false,
                data: timeData
            },
            yAxis: {
                type: 'value',
                name: '影响力链接',
                axisLabel: {
                    formatter: '{value}'
                }
            },
            series: (function () {
                var serie = [];
                for (var i = 0; i < linkData.length; i++) {
                    var zhaiyaoList = [];
                    var cc = 0;
                    for (aa = 0; aa < zhaiyao.length; aa++) {
                        if (zhaiyao[aa].CategoryName == linkData[i].name) {
                            zhaiyaoList[cc] = zhaiyao[aa];
                            cc++;
                        }
                    }
                    console.log(zhaiyaoList);
                    var xyName = [];
                    var coord = [];
                    for (var j = 0; j < zhaiyaoList.length; j++) {
                        var item = {
                            name: '摘要：' + zhaiyaoList[j].Summary,
                            xAxis: $filter("date")(zhaiyaoList[j].X, "yyyy-MM-dd"),
                            yAxis: zhaiyaoList[j].Y,
                        }
                        xyName.push(item);
                    }
                    console.log(xyName);
                    var item = {
                        name: linkData[i].name,
                        type: 'line',
                        data: linkData[i].LinkCount,
                        markPoint: {
                            data: xyName,
                        }
                    }
                    serie.push(item);
                };
                return serie;
            })()
        };
        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
        myChart.on('datazoom', function (params) {
            $scope.startTime = timeData[parseInt((params.start * timeData.length) / 100)];
            $scope.endTime = timeData[parseInt((params.end * timeData.length) / 100)];
            if (params.end == 100) {
                $scope.endTime = timeData[parseInt((params.end * timeData.length) / 100) - 1];
            }
            $scope.startNum = Math.round(params.start);
            $scope.endNum = Math.round(params.end);
        });
        //自动摘要
        myChart.on('click', function (params) {
            console.log(params);
            var time = params.data.xAxis;
            $scope.searchLinkByTime(time);
            $scope.data.searchTime = time;
            console.log(time)
        });
    };
    $scope.D_PieChart_bianli = function (id,data) {
        var date = data.PieChartData;
        var myChart = echarts.init(document.getElementById(id));
        console.log(date);
        var data_n = [];
        var date_a = [];
        for (var i = 0; i < date.length; i++) {
            if (date[i].Summary) {
                data_n[i] = date[i].CategoryName + ':' + date[i].Summary.substring(0, 13);
            } else {
                data_n[i] = date[i].CategoryName + ':' + date[i].Summary;
            }
            if (date[i].Summary) {
                date_a.push({
                    value: date[i].Y,
                    name: date[i].Summary.substring(0, 18)
                })
            } else {
                date_a.push({
                    value: date[i].Y,
                    name: date[i].Summary
                })
            }
        }

        var title = '传播内容占比';
        var title_f = '仅计算Top' + date.length + '链接数占比，不算总值';
        option = {
            title: {
                text: title,
                subtext: title_f,
                x: 'center'
            },
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            series: [{
                label: {
                    normal: {
                        show: true,
                        textStyle: {
                            fontSize: 8,
                        },
                    },
                    emphasis: {
                        show: true
                    },
                },
                name: '相关话题',
                type: 'pie',
                radius: ['20%', '35%'],
                center: ['50%', '50%'],
                data: date_a,
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                },

            }]
        };
        myChart.setOption(option);
    }
    $scope.D_QPChart_bianli = function (id, data) {
        $scope.domainCategory = data.DomainCategory;
        $scope.domainCategoryIds = [];
        $scope.domainCategoryNames = [];
        for (var i = 0; i < $scope.domainCategory.length; i++) {
            $scope.domainCategoryIds.push($scope.domainCategory[i]._id);
            $scope.domainCategoryNames.push($scope.domainCategory[i].Name);
        }
        var response = data.Chart_DomainCategory
        var myChart = echarts.init(document.getElementById(id));
        var datas = [];
        var dataNull = [];
        var bobuSizeB = 1;
        for (var ii = 0; ii < response.length; ii++) {
            if (bobuSizeB < response[ii].KeywordTotal) {
                bobuSizeB = response[ii].KeywordTotal;
            }
        }
        var bobuSize = 200 / bobuSizeB;

        for (var i = 0; i < $scope.domainCategoryIds.length; i++) {
            var data1 = [];
            for (var j = 0; j < response.length; j++) {
                if (response[j].DomainCategoryId == $scope.domainCategoryIds[i]) {
                    data1.push(
						[response[j].Count,
							response[j].RankTotal,
							(response[j].KeywordTotal * bobuSize + 5),
							response[j].Domain,
							$filter('number')(parseFloat(response[j].PublishRatio), '2') + '%',
							response[j].DomainCategoryName
						]
					);
                } else if (response[j].DomainCategoryId === null) {
                    dataNull.push(
						[response[j].Count,
							response[j].RankTotal,
							(response[j].KeywordTotal * bobuSize + 10),
							response[j].Domain,
							$filter('number')(parseFloat(response[j].PublishRatio), '2') + '%',
							response[j].DomainCategoryName
						]
					);
                }
            }
            datas.push(data1);
        }
        datas.push(dataNull);
        for (var k = 0; k < datas.length; k++) {
            if (datas[k].length == 0) {
                datas.splice(k, 1);
                $scope.domainCategoryNames.splice(k, 1);
                k--;
            }
        }
        $scope.domainCategoryNames.push('未分组');
        console.log(datas);
        console.log($scope.domainCategoryNames);
        var schema = [{
            index: 0,
            text: '分组名'
        }, {
            index: 1,
            text: '百度排名'
        }, {
            index: 2,
            text: '有效链接数'
        }, {
            index: 3,
            text: '关键词数'
        }, {
            index: 4,
            text: '含发布时间占比'
        }];
        option = {
            title: {
                text: '命中关键词域名分布图',
                padding: [
					0, 0, 0, 30
                ]
            },
            legend: {
                y: 'top',
                data: $scope.domainCategoryNames,
                selected: {
                    '未分组': false
                },
                textStyle: {
                    color: '#333',
                    fontSize: 12
                }
            },
            tooltip: {
                padding: 10,
                backgroundColor: '#222',
                borderColor: '#777',
                borderWidth: 1,
                formatter: function (obj) {
                    var value = obj.value;
                    return '<div style="border-bottom: 1px solid rgba(255,255,255,.3); font-size: 18px;padding-bottom: 7px;margin-bottom: 7px">' + value[3] + '</div>' + schema[0].text + '：' + value[5] + '<br>' + schema[1].text + '：' + value[1] + '<br>' + schema[2].text + '：' + value[0] + '<br>' + schema[3].text + '：' + Math.round((value[2] - 5) / bobuSize) + '<br>' + schema[4].text + '：' + value[4] + '<br>';
                }
            },
            grid: {
                left: '3%',
                right: '10%',
                bottom: '12%',
                containLabel: true
            },
            xAxis: {
                type: 'value',
                min: 'dataMin',
                max: 'dataMax',
                splitLine: {
                    show: true
                }
            },
            yAxis: {
                name: '百度排名',
                type: 'value',
                min: 'dataMin',
                max: 'dataMax',
                splitLine: {
                    show: true
                }
            },
            dataZoom: [{
                type: 'slider',
                height: 10,
                show: true,
                xAxisIndex: [0],
                start: 0,
                end: 100
            }, {
                type: 'slider',
                width: 10,
                show: true,
                yAxisIndex: [0],
                left: '93%',
                start: 0,
                end: 100
            }, {
                type: 'inside',
                xAxisIndex: [0]
            }, {
                type: 'inside',
                yAxisIndex: [0]
            }],
            visualMap: [{
                left: 'right',
                top: '10%',
                dimension: 2,
                min: 0,
                max: 250,
                itemWidth: 30,
                itemHeight: 120,
                calculable: true,
                precision: 1,
                text: [''],
                textGap: 30000,
                inRange: {
                    symbolSize: [15, 60]
                },
                outOfRange: {
                    symbolSize: [15, 60],
                },

            }, {
                left: 'right',
                bottom: '5%',
                dimension: 6,
                min: 0,
                max: 50,
                itemHeight: 120,
                calculable: true,
                precision: 0.1,
                text: [''],
                textGap: 3000,
                textStyle: {
                    color: '#fff'
                }
            }],
            series: (function () {
                var serie = new Array;
                for (var i = 0; i < datas.length; i++) {
                    var item = {
                        name: $scope.domainCategoryNames[i],
                        type: 'scatter',
                        itemStyle: {
                            normal: {
                                opacity: 0.8
                            }
                        },
                        symbolSize: function (val) {
                            return Math.sqrt(val[2]);
                        },
                        data: datas[i],
                        label: {
                            emphasis: {
                                show: true,
                                formatter: function (param) {
                                    return param.data[3];
                                },
                                position: 'top'
                            }
                        },
                    }
                    serie.push(item);
                };
                return serie;
            })()

        }
        myChart.setOption(option);
    }

    //删除链接分析图表组
    $scope.DelLinkChartCate = function (id) {
        var url = "/api/Report/DelLinkChartCate?lChartCateId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '图表块删除成功！')
                $scope.GetLinkChartCate(false);
            } else {
                $scope.alert_fun('danger', response.Message)
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    //选择图表________________________________________+++++++++++++________________________________________________________
    $scope.data = {
        topNum: 10,
        percent: 0,
        searchTime: '',
        num_1: '',
        page2: 1
    };
    $scope.startTime = '2000/01/01 01:01:01';
    $scope.endTime = new Date;
    //时间选择项
    $scope.timeOptions = [{
        name: "每日",
        value: 1
    }, {
        name: "每周",
        value: 7
    }, {
        name: "每月",
        value: 30
    }, {
        name: "自定义",
        value: 0,
        isShow: true
    }]
    //选择图表
    $scope.selcetChart=function(x,typ){
        $scope.selsetZtu=x;
        $scope.chartType = typ;
        $timeout(function () {
            if (typ == 1) {
                $scope.D_lineChart_shai(null);
            } else if (typ == 2) {
                $scope.D_PieChart_shai(null);
            } else if (typ == 3) {
                $scope.GetDomainOverview(false);
            }
            },100)
    }
    //插入折线图
    $scope.D_lineChart_shai = function (data) {
        var response;
        var zhaiyao;
        if (data == null) {
            response = $scope.GetLinkOverviewData;
            zhaiyao = $scope.GetLinkOverviewData.PieChartData;
        }
        else {
            response = data;
            zhaiyao = data.PieChartData;
        }
        //基于准备好的dom，初始化echarts实例
        //指定图表的配置项和数据
        var myChart = echarts.init(document.getElementById('D_lineChart_shai'));
        var timeData1 = response.Times;
        //console.log(response.Times);
        var linkData = response.LineChartData;
        if (linkData.length > 8) {
            linkData.length = 8;
        }
        var timeData = [];
        for (var j = 0; j < timeData1.length; j++) {
            timeData[j] = $filter("date")(timeData1[j], "yyyy-MM-dd");
        }
        option = {
            title: {
                padding: [
                    0, 0, 0, 30
                ],
                text: '传播事件统计图',
                //subtext: '副标题'
            },
            tooltip: {
                trigger: 'axis'
            },
            legend: {
                data: function () {
                    var biaoti = [];
                    for (var i = 0; i < linkData.length; i++) {
                        biaoti.push(linkData[i].name);
                    };
                    return biaoti;
                }(),
                //orient: 'vertical',
                x: "160px"
            },
            grid: {
                left: '6%',
                right: '6%',
                bottom: '15%',
                containLabel: true
            },
            dataZoom: [{
                type: 'slider',
                height: 10,
                show: true,
                xAxisIndex: [0],
                start: $scope.startNum,
                end: $scope.endNum
            }],
            xAxis: {
                type: 'category',
                boundaryGap: false,
                data: timeData
            },
            yAxis: {
                type: 'value',
                name: '影响力链接',
                axisLabel: {
                    formatter: '{value}'
                }
            },
            series: (function () {
                var serie = [];
                for (var i = 0; i < linkData.length; i++) {
                    var zhaiyaoList = [];
                    var cc = 0;
                    for (aa = 0; aa < zhaiyao.length; aa++) {
                        if (zhaiyao[aa].CategoryName == linkData[i].name) {
                            zhaiyaoList[cc] = zhaiyao[aa];
                            cc++;
                        }
                    }
                    console.log(zhaiyaoList);
                    var xyName = [];
                    var coord = [];
                    for (var j = 0; j < zhaiyaoList.length; j++) {
                        var item = {
                            name: '摘要：' + zhaiyaoList[j].Summary,
                            xAxis: $filter("date")(zhaiyaoList[j].X, "yyyy-MM-dd"),
                            yAxis: zhaiyaoList[j].Y,
                        }
                        xyName.push(item);
                    }
                    console.log(xyName);
                    var item = {
                        name: linkData[i].name,
                        type: 'line',
                        data: linkData[i].LinkCount,
                        markPoint: {
                            data: xyName,
                        }
                    }
                    serie.push(item);
                };
                return serie;
            })()
        };
        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
        myChart.on('datazoom', function (params) {
            $scope.startTime = timeData[parseInt((params.start * timeData.length) / 100)];
            $scope.endTime = timeData[parseInt((params.end * timeData.length) / 100)];
            if (params.end == 100) {
                $scope.endTime = timeData[parseInt((params.end * timeData.length) / 100) - 1];
            }
            $scope.startNum = Math.round(params.start);
            $scope.endNum = Math.round(params.end);
        });
        //自动摘要
        myChart.on('click', function (params) {
            console.log(params);
            var time = params.data.xAxis;
            $scope.searchLinkByTime(time);
            $scope.data.searchTime = time;
            console.log(time)
        });
    };
    //插入饼图
    $scope.D_PieChart_shai = function () {
        var date = $scope.GetLinkOverviewData.PieChartData;
        var myChart = echarts.init(document.getElementById('D_PieChart_shai'));
        console.log(date);
        var data_n = [];
        var date_a = [];
        for (var i = 0; i < date.length; i++) {
            if (date[i].Summary) {
                data_n[i] = date[i].CategoryName + ':' + date[i].Summary.substring(0, 13);
            } else {
                data_n[i] = date[i].CategoryName + ':' + date[i].Summary;
            }
            if (date[i].Summary) {
                date_a.push({
                    value: date[i].Y,
                    name: date[i].Summary.substring(0, 18)
                })
            } else {
                date_a.push({
                    value: date[i].Y,
                    name: date[i].Summary
                })
            }
        }

        var title = '传播内容占比';
        var title_f = '仅计算Top' + date.length + '链接数占比，不算总值';
        option = {
            title: {
                text: title,
                subtext: title_f,
                x: 'center'
            },
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            series: [{
                label: {
                    normal: {
                        show: true,
                        textStyle: {
                            fontSize: 8,
                        },
                    },
                    emphasis: {
                        show: true
                    },
                },
                name: '相关话题',
                type: 'pie',
                radius: ['20%', '35%'],
                center: ['50%', '50%'],
                data: date_a,
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                },

            }]
        };

        myChart.setOption(option);

    }

    //InsertLinkChart插入链接图表
    $scope.InsertLinkChart = function () {
        if (!$scope.selsetZtu._id) {
            $scope.alert_fun("worning","请选择图表类型！")
        }
        $scope.model1Title = $('#model420input').val();
        $scope.description = $('#model421textare').val();
        $scope.paramsList = {
            id: '',
            reportId: $scope.selsetReport._id,
            lChartCateId: $scope.selsetZtu._id,
            keyCateIds: $scope.categoryId,
            description: $scope.description,
            title: $scope.model1Title,
            index: 1,
            startTime: $scope.startTime,
            endTime: $scope.endTime,
            topNum: $scope.data.topNum,
            sumNum: $scope.data.topNum,
            timeInterval:$scope.timeInter,
            chartType:  $scope.chartType,
            isHide: false,
            percent:0,
        };
        var urls = "/api/Report/InsertLinkChart";
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
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '模块添加成功！');
                $scope.startNum = 0;
                $scope.endNum = 100;
                $scope.GetLinkChartCate(false);
                $scope.XiugaiEditerFun(0, null);
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    }
    //删除插入链接图表
    $scope.DelLinkChart = function (id) {
        var url = "/api/Report/DelLinkChart?lChartId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '连接图添加成功！');
                $scope.GetLinkChartCate(false);
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });


    }
    //更新插入链接图表
    $scope.UpdateLinkChart = function (id) {
        $scope.model1Title = $('#model420input').val();
        $scope.description = $('#model421textare').val();
        $scope.paramsList = {
            id: '',
            reportId: $scope.selsetReport._id,
            lChartCateId: id,
            keyCateIds: '',
            description: $scope.LinkChartListIpDescription,
            title: $scope.LinkChartListIpTitle,
            index: 1,
            startTime: $scope.startTime,
            endTime: $scope.endTime,
            topNum: $scope.data.topNum,
            sumNum: $scope.data.topNum,
            timeInterval: $scope.timeInter,
            chartType: $scope.chartType,
            isHide: false,
            percent: 0,
        };
        var urls = "/api/Report/UpdateLinkChart";
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
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '描述更新成功！');
                $scope.GetLinkChartCate(false);
                $scope.ChangeTJMS2(0);
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    }

    //打开保存设置模态框
    $scope.SaveChart_OT = function (selectedTime) {
        var CP_scope = $scope;
        //获取当前时间间隔
        CP_scope.timeInter = selectedTime.value;
        CP_scope.isReport = true;
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/saveChart.html',
            controller: saveChart_ctr,
            scope: CP_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'sm'
        });
    };

    //打开加载设置模态框
    $scope.GetChart_OT = function () {
        var CP_scope = $scope;
        CP_scope.isReport = true;
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/getChart.html',
            controller: getChart_ctr,
            scope: CP_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'cd'
        });
        frm.result.then(function (response, status) {
            //$rootScope.GetBaiduSearchKeyword2();
        });
    };

    //插入气泡图
    $scope.InsertDomainChart = function () {
        $scope.model1Title = $('#model420input').val();
        $scope.description = $('#model421textare').val();
        var url = "/api/Report/InsertDomainChart?reportId=" + $scope.selsetReport._id + '&title=' + $scope.model1Title + '&description=' + $scope.description
        + '&index=' + 1 + '&categoryId=' + $scope.selsetZtu._id + '&keyCateIds=' + '' ;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '气泡图添加成功！');
                $scope.GetLinkChartCate(false);
                $scope.XiugaiEditerFun(0,null);
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });

    }
    //更新插入气泡图

    $scope.UpdateDomainChart = function (id) {
        var url = "/api/Report/UpdateDomainChart?domainChartId=" + id + '&title=' + $scope.DomainChartListIpTitle + '&description=' + $scope.LinkChartListIpDescription;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '气泡图信息更新成功！');
                $scope.GetLinkChartCate(false);
                $scope.ChangeTJMS2(0);
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    //删除插入气泡图
    $scope.DelDomainChart = function (id) {
        var url = "/api/Report/DelDomainChart?domainChartId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '连接图添加成功！');
                $scope.GetLinkChartCate(false);
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
 
    //结束选择图表________________________________________+++++++++++++________________________________________________________




    //model4  ++ 结束__________________________________________________________________________________________________

    //model5  ++ 开始__________________________________________________________________________________________________


    //获取域名分组统计图
    $scope.GetDomainOverview = function (isReRead) {
        if (isReRead == true) {
            if (confirm("您气泡图表数据吗？")) {

            } else {
                return;
            }
        }
        var url = "/api/Report/GetDomainOverview?reportId=" + $scope.selsetReport._id + '&isReRead=' + isReRead + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response)
            $scope.GetDomainChartDate = response;
            $scope.D_GetBubbleList();
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //$scope.GetDomainOverview(false);

    //生成气泡图
    $scope.D_GetBubbleList = function () {
        $scope.domainCategory = $scope.GetDomainChartDate.DomainCategory;
        $scope.domainCategoryIds = [];
        $scope.domainCategoryNames = [];
        for (var i = 0; i < $scope.domainCategory.length; i++) {
            $scope.domainCategoryIds.push($scope.domainCategory[i]._id);
            $scope.domainCategoryNames.push($scope.domainCategory[i].Name);
        }
        var response= $scope.GetDomainChartDate.Chart_DomainCategory
        var myChart = echarts.init(document.getElementById('RP_GetBubbleList'));
        var datas = [];
        var dataNull = [];
        var bobuSizeB = 1;
        for (var ii = 0; ii < response.length; ii++) {
            if (bobuSizeB < response[ii].KeywordTotal) {
                bobuSizeB = response[ii].KeywordTotal;
            }
        }
        var bobuSize = 200 / bobuSizeB;

        for (var i = 0; i < $scope.domainCategoryIds.length; i++) {
            var data1 = [];
            for (var j = 0; j < response.length; j++) {
                if (response[j].DomainCategoryId == $scope.domainCategoryIds[i]) {
                    data1.push(
						[response[j].Count,
							response[j].RankTotal,
							(response[j].KeywordTotal * bobuSize + 5),
							response[j].Domain,
							$filter('number')(parseFloat(response[j].PublishRatio), '2') + '%',
							response[j].DomainCategoryName
						]
					);
                } else if (response[j].DomainCategoryId === null) {
                    dataNull.push(
						[response[j].Count,
							response[j].RankTotal,
							(response[j].KeywordTotal * bobuSize + 10),
							response[j].Domain,
							$filter('number')(parseFloat(response[j].PublishRatio), '2') + '%',
							response[j].DomainCategoryName
						]
					);
                }
            }
            datas.push(data1);
        }
        datas.push(dataNull);
        for (var k = 0; k < datas.length; k++) {
            if (datas[k].length == 0) {
                datas.splice(k, 1);
                $scope.domainCategoryNames.splice(k, 1);
                k--;
            }
        }
        $scope.domainCategoryNames.push('未分组');
        console.log(datas);
        console.log($scope.domainCategoryNames);
        var schema = [{
            index: 0,
            text: '分组名'
        }, {
            index: 1,
            text: '百度排名'
        }, {
            index: 2,
            text: '有效链接数'
        }, {
            index: 3,
            text: '关键词数'
        }, {
            index: 4,
            text: '含发布时间占比'
        }];
        option = {
            title: {
                text: '命中关键词域名分布图',
                padding: [
					0, 0, 0, 30
                ]
            },
            legend: {
                y: 'top',
                data: $scope.domainCategoryNames,
                selected: {
                    '未分组': false
                },
                textStyle: {
                    color: '#333',
                    fontSize: 12
                }
            },
            tooltip: {
                padding: 10,
                backgroundColor: '#222',
                borderColor: '#777',
                borderWidth: 1,
                formatter: function (obj) {
                    var value = obj.value;
                    return '<div style="border-bottom: 1px solid rgba(255,255,255,.3); font-size: 18px;padding-bottom: 7px;margin-bottom: 7px">' + value[3] + '</div>' + schema[0].text + '：' + value[5] + '<br>' + schema[1].text + '：' + value[1] + '<br>' + schema[2].text + '：' + value[0] + '<br>' + schema[3].text + '：' + Math.round((value[2] - 5) / bobuSize) + '<br>' + schema[4].text + '：' + value[4] + '<br>';
                }
            },
            grid: {
                left: '3%',
                right: '10%',
                bottom: '12%',
                containLabel: true
            },
            xAxis: {
                type: 'value',
                min: 'dataMin',
                max: 'dataMax',
                splitLine: {
                    show: true
                }
            },
            yAxis: {
                name: '百度排名',
                type: 'value',
                min: 'dataMin',
                max: 'dataMax',
                splitLine: {
                    show: true
                }
            },
            dataZoom: [{
                type: 'slider',
                height: 10,
                show: true,
                xAxisIndex: [0],
                start: 0,
                end: 100
            }, {
                type: 'slider',
                width: 10,
                show: true,
                yAxisIndex: [0],
                left: '93%',
                start: 0,
                end: 100
            }, {
                type: 'inside',
                xAxisIndex: [0]
            }, {
                type: 'inside',
                yAxisIndex: [0]
            }],
            visualMap: [{
                left: 'right',
                top: '10%',
                dimension: 2,
                min: 0,
                max: 250,
                itemWidth: 30,
                itemHeight: 120,
                calculable: true,
                precision: 1,
                text: [''],
                textGap: 30000,
                inRange: {
                    symbolSize: [15, 60]
                },
                outOfRange: {
                    symbolSize: [15, 60],
                },

            }, {
                left: 'right',
                bottom: '5%',
                dimension: 6,
                min: 0,
                max: 50,
                itemHeight: 120,
                calculable: true,
                precision: 0.1,
                text: [''],
                textGap: 3000,
                textStyle: {
                    color: '#fff'
                }
            }],
            series: (function () {
                var serie = new Array;
                for (var i = 0; i < datas.length; i++) {
                    var item = {
                        name: $scope.domainCategoryNames[i],
                        type: 'scatter',
                        itemStyle: {
                            normal: {
                                opacity: 0.8
                            }
                        },
                        symbolSize: function (val) {
                            return Math.sqrt(val[2]);
                        },
                        data: datas[i],
                        label: {
                            emphasis: {
                                show: true,
                                formatter: function (param) {
                                    return param.data[3];
                                },
                                position: 'top'
                            }
                        },
                    }
                    serie.push(item);
                };
                return serie;
            })()

        }
        myChart.setOption(option);
    };

    //更新关键词分析图表描述：
    $scope.UpdateDomainOverview = function (id) {
        var url = "/api/Report/UpdateDomainOverview?domainChartId=" + $scope.GetDomainChartDate._id + '&description=' + $scope.GetDCdescription;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            if (response.IsSuccess == true) {
                $scope.GetDomainOverview(false);
                $scope.alert_fun('suceess', '气泡图表描述更新成功！');
                $scope.ChangeTJMS(0);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    //model5  ++ 结束__________________________________________________________________________________________________
    //model6  ++ 开始__________________________________________________________________________________________________
    //插入简报话语分析
    $scope.InsertWordTree = function () {
        var text = $(window.frames["iframe_a"].document).find("#textS").html();
        var keyword = $(window.frames["iframe_a"].document).find("#KeywordS").html();
        $scope.model6Title = $('#model6Title').val();
        $scope.model6Description = $('#model6Description').val();


        $scope.paramsList = {
            id:'',
            reportId: $scope.selsetReport._id,
            description: $scope.model6Description,
            title: $scope.model6Title,
            text: text,
            keyword: keyword,
        };
        var urls = "/api/Report/InsertWordTree";
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
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '话语分析添加成功！');
                $scope.GetWordTree();
                $scope.XiugaiEditerFun(0, null)
            } else {
                $scope.alert_fun('warning', response.Message);
            }
        });
        q.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    }
    //获取话语分析
    $scope.GetWordTree = function () {
        var url = "/api/Report/GetWordTree?reportId=" + $scope.selsetReport._id + '&isHide=' + true;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.GetWordTreeList = response.Result;
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    $scope.GetWordTree();
    //删除话语分析
    $scope.DelWordTree = function (id) {
        var url = "/api/Report/DelWordTree?treeId=" + id;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '话语分析删除成功！');
                $scope.GetWordTree();
            } else {
                $scope.alert_fun('danger', response.Message);
            }

        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //修改话语分析
    $scope.UpdateWordTree = function (id) {
        var text = $(window.frames["iframe_a"].document).find("#textS").html();
        var keyword = $(window.frames["iframe_a"].document).find("#KeywordS").html();
        $scope.model6Title = $('#model6Title').val();
        $scope.model6Description = $('#model6Description').val();
        $scope.paramsList = {
            id: id,
            reportId: $scope.selsetReport._id,
            description: $scope.model6Description,
            title: $scope.model6Title,
            text: text,
            keyword: keyword,
        };
        var urls = "/api/Report/UpdateWordTree";
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
            if (response.IsSuccess == true) {
                $scope.alert_fun('success', '话语分析描述修改成功！');
                $scope.GetWordTree();
                $scope.XiugaiEditerFun(0, null);
            } else {
                $scope.alert_fun('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.error = "服务器连接出错";
        });
    }
   
    //显示报告描述与否
    $scope.reportRDecShowFun = function () {
        $scope.reportRDecShow = !$scope.reportRDecShow;
    }
    //_____________________________________________________________________________________
    $rootScope.GetShareOutComment();
});