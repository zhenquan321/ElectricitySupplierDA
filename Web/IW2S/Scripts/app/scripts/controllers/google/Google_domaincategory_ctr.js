var Google_domaincategory_ctr = myApp.controller("Google_domaincategory_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $filter, myApplocalStorage) {

    $scope.hasGroup = false;
    $scope.tab1 = true;
    $scope.tab2 = false;
    $scope.tab3 = false;
    $scope.currentTab1 = true;
    $scope.currentTab2 = false;
    $scope.currentTab3 = false;
    $scope.hasAddDomain = false;
    $scope.tab1Editor = false;
    $scope.tab2Editor = false;
    $scope.step1 = true;
    $scope.step2 = false;
    $scope.step3 = false;
    $scope.pagesizeDomainCategory = 10;
    $scope.currentCategoryId = '';
    $scope.editorCategoryId = '';
    $scope.activeId = '';
    //解决 model值拿不到的方法
    $scope.ctrlScope = $scope;
    //checkbox
    $scope.ctrlScope.xxx = false;
    //分页
    $scope.ctrlScope.pageDomainCategory = 1;

    $scope.scatter = false;

    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);

    // 此处为页面切换函数-------------------------------------------------------------------------------------------------
    //分组管理 和气泡图
    $scope.toScatter = function () {
        $scope.scatter = !$scope.scatter;
        if($scope.scatter){
            $scope.GetTreeData();
        }
    }

    // 详情 编辑 添加

    $scope.changeCurrentTab1 = function () {
        $scope.currentTab1 = true;
        $scope.currentTab2 = false;
        $scope.currentTab3 = false;
        $scope.tab1 = true;
        $scope.tab2 = false;
        $scope.tab3 = false;
    }

    $scope.changeCurrentTab2 = function () {
        $scope.currentTab1 = false;
        $scope.currentTab2 = true;
        $scope.currentTab3 = false;
        $scope.tab1 = false;
        $scope.tab2 = true;
        $scope.tab3 = false;
    }

    $scope.changeCurrentTab3 = function () {
        $scope.currentTab1 = false;
        $scope.currentTab2 = false;
        $scope.currentTab3 = true;
        $scope.tab1 = false;
        $scope.tab2 = false;
        $scope.tab3 = true;
    }
    //添加-  第一步 第二步 第三步
    $scope.toStep1 = function () {
        $scope.step1 = true;
        $scope.step2 = false;
        $scope.step3 = false;
    }

    $scope.toStep2 = function () {
        $scope.step2 = true;
        $scope.step1 = false;
        $scope.step3 = false;

    }

    $scope.toStep3 = function () {
        $scope.step3 = true;
        $scope.step1 = false;
        $scope.step2 = false;
    }
    //编辑分组切换
    $scope.editorCategory = function (id, name) {
        $scope.editorCategoryId = id;
        $scope.tab2Editor = true;
        $scope.editorGroup = [];
        $scope.editorGroup.push({x: name});
    }
    //取消修改分组
    $scope.cancleAmendDomainName = function () {
        $scope.tab2Editor = false;
        $scope.editorGroup = [];
        $scope.editorGroup.push({x: name});
    }
    //添加更多域名
    $scope.addDomainAgain = function () {
        $scope.changeCurrentTab3();
        $scope.toStep3();
    }
    //编辑域名
    $scope.editorChooseDomain = function () {
        if ($scope.checkedId.length == 0) {
            $scope.addAlert('danger', "没有勾选要编辑的域名");
        } else {
            $scope.tab1Editor = true;
        }
    }
    //取消编辑域名
    $scope.cancleEditorDomain = function () {
        $scope.editorDomains = [];
        for (var i = 0; i < $scope.copyeditorDomains.length; i++) {
            $scope.editorDomains.push({x: $scope.copyeditorDomains[i].x})
        }
        $scope.tab1Editor = false;
    }
    //------------------------------------------------------------------------------------------------------------------
    // 第一次添加分组
    $scope.goToCurrentTab3 = function () {
        $scope.hasGroup = false;
        $scope.changeCurrentTab3();
        $scope.toStep1();
    }

    //新建分组（下一步）
    $scope.newCategory = function (groupName) {
        if (!groupName) {
            $scope.addAlert('danger', "域名分组不能为空！");
        } else {
            //新建域名分组
            $scope.newCategoryList = {
                Name: groupName,
                _id: '',
                ParentId: '',
                Num: 0,
                UsrId: $rootScope.userID,
                ProjectId: $rootScope.getProjectId
            }


            var urls = "api/Google/SaveDomainCategory";
            var q = $http.post(
                urls,
                JSON.stringify($scope.newCategoryList),
                {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }
            )
            q.success(function (response, status) {
                if (response.IsSuccess) {
                    //分组model值清空
                    $scope.groupName = '';
                    $scope.currentCategoryId = '';
                    $scope.toStep2();
                    $scope.GetAllDomainCategory();
                    $scope.activeId = response.NewId;
                } else {
                    $scope.addAlert('danger', response.Message);
                }
            });
            q.error(function (e) {
                $scope.addAlert('danger', "网络打盹了，请稍后。。。");
            });
        }
    }


    //修改分组
    $scope.amendDomainName = function () {
        $scope.amendCategoryList = {
            Name: $scope.editorGroup[0].x,
            _id: $scope.editorCategoryId,
            ParentId: '',
            Num: 0,
            UsrId: $rootScope.userID,
            ProjectId: $rootScope.getProjectId
        }

        var urls = "api/Google/SaveDomainCategory";
        var q = $http.post(
            urls,
            JSON.stringify($scope.amendCategoryList),
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        )
        q.success(function (response, status) {
            if (response.IsSuccess) {
                $scope.addAlert('success', "修改分组名成功");
                $scope.tab2Editor = false;
                $scope.GetAllDomainCategory();
                $scope.toStep1();
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }
    //删除分组
    $scope.removeCategory = function (id) {
        if (confirm("您确定要删除该分组吗？")) {
            var url = "api/Google/DelDomainCategory?id=" + id;
            var q = $http.get(url);
            q.success(function (response, status) {
                if (response.IsSuccess) {
                    $scope.addAlert('success', "删除成功");
                    $scope.GetAllDomainCategory();
                    $scope.toStep1();
                }
            });
            q.error(function (e) {
                $scope.addAlert('danger', "网络打盹了，请稍后。。。");
            });
        }
    }
    //------------------------------------------------------------------------------------------------------------------

    //添加域名初始化值
    $scope.domains = [];
    $scope.domains.push({x: ""});
    //添加域名input框
    $scope.addDomain = function () {
        $scope.domains.push({x: ""});
    }
    //删除域名input框
    $scope.removeDomain = function (x) {
        for (var i = 0; i < $scope.domains.length; i++) {
            if ($scope.domains.length > 1) {
                if (x == $scope.domains[i].x) {
                    $scope.domains.splice(i, 1);
                    break;
                }
            }
        }
    }
    //取消添加域名操作
    $scope.cancleAddDomain = function () {
        $scope.domains = [];
        $scope.domains.push({x: ""});
    }

    //为新建分组添加域名
    $scope.newDomain = function () {
        if ($scope.currentCategoryId) {
            var id = $scope.currentCategoryId;
        } else {
            var id = $scope.groupList[$scope.groupList.length - 1]._id;
        }

        //获取添加域名input框model值用;拼接
        $scope.domainsModel = [];
        for (var i = 0; i < $scope.domains.length; i++) {
            $scope.domainsModel.push($scope.domains[i].x);
        }
        $scope.newDomainList = [];
        for (var j = 0; j < $scope.domainsModel.length; j++) {
            $scope.newDomainList.push({
                _id: '',
                DomainName: $scope.domainsModel[j],
                DomainCategoryId: id,
                UsrId: $rootScope.userID,
            })
        }
        var urls = "api/Google/SaveDomainCategoryData";
        var q = $http.post(
            urls,
            JSON.stringify($scope.newDomainList),
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        )
        q.success(function (response, status) {
            if (response.IsSuccess) {
                $scope.addAlert('success', '域名添加成功');
                $scope.cancleAddDomain();
                $scope.toStep1();
                $scope.GetAllDomainCategory();
                $scope.GetDomainCategoryData(id);
            } else {
                $scope.addAlert('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }

    //获取复选框id
    $scope.checkedId = [];
    $scope.editorDomains = [];
    $scope.copyeditorDomains = [];
    //单选
    $scope.chkOne = function (id, name, aa) {
        if (aa) {
            $scope.checkedId.push(id);
            $scope.editorDomains.push({x: name});
            $scope.copyeditorDomains.push({x: name});
        } else {
            for (var i = 0; i < $scope.checkedId.length; i++) {
                if ($scope.checkedId[i] == id) {
                    $scope.checkedId.splice(i, 1);
                    $scope.editorDomains.splice(i, 1);
                    $scope.copyeditorDomains.splice(i, 1);
                    break;
                }
            }
        }
    }
    //全选
    $scope.chkAll = function (bb) {
        if (bb) {
            $scope.checkedId = [];
            $scope.editorDomains = [];
            $scope.copyeditorDomains = [];
            for (var i = 0; i < $scope.domainList.length; i++) {
                $scope.checkedId.push($scope.domainList[i]._id);
                $scope.editorDomains.push({x: $scope.domainList[i].DomainName});
                $scope.copyeditorDomains.push({x: $scope.domainList[i].DomainName});
            }
        } else {
            $scope.checkedId = [];
            $scope.editorDomains = [];
            $scope.copyeditorDomains = [];
        }
    }

    //修改域名
    $scope.editorGroup = [];
    $scope.editorGroup.push({x: ""});

    $scope.amendDomain = function () {
        if ($scope.currentCategoryId) {
            var id = $scope.currentCategoryId;
        } else {
            var id = $scope.groupList[$scope.groupList.length - 1]._id;
        }
        $scope.amendDomains = [];
        for (var j = 0; j < $scope.checkedId.length; j++) {
            $scope.amendDomains.push({
                _id: $scope.checkedId[j],
                DomainName: $scope.editorDomains[j].x,
                DomainCategoryId: id,
                UsrId: $rootScope.userID,
            })
        }
        var urls = "api/Google/SaveDomainCategoryData";
        var q = $http.post(
            urls,
            JSON.stringify($scope.amendDomains),
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        )
        q.success(function (response, status) {
            if (response.IsSuccess) {
                $scope.addAlert('success', '域名修改成功');
                $scope.checkedId = [];
                $scope.editorDomains = [];
                $scope.copyeditorDomains=[];
                $scope.tab1Editor = false;
                $scope.GetDomainCategoryData(id);
                $scope.ctrlScope.xxx = false;
            } else {
                $scope.addAlert('danger', response.Message);
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }

    //删除域名
    $scope.removeChooseDomain = function () {
        $scope.checkedId1 = $scope.checkedId.join(';');
        if ($scope.checkedId1 != '') {
            var url = "api/Google/DelDomainCategoryData?DomainId=" + $scope.checkedId1;
            var q = $http.get(url);
            q.success(function (response, status) {
                if (response.IsSuccess) {
                    $scope.addAlert('success', "域名删除成功");
                    if ($scope.currentCategoryId) {
                        var id = $scope.currentCategoryId;
                    } else {
                        var id = $scope.groupList[$scope.groupList.length - 1]._id;
                    }
                    $scope.checkedId = [];
                    $scope.editorDomains = [];
                    $scope.copyeditorDomains=[];
                    $scope.GetAllDomainCategory();
                    $scope.GetDomainCategoryData(id);
                    $scope.ctrlScope.xxx = false;
                }
            });
            q.error(function (e) {
                $scope.addAlert('danger', "网络打盹了，请稍后。。。");
            });
        } else {
            $scope.addAlert('danger', "没有勾选要删除的域名");
        }

    }


    // 默认加载首个分组下的域名
    $scope.GetAllDomainCategoryFrist = function () {
        var url = "api/Google/GetAllDomainCategory?prjId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.length == 0) {
                $scope.hasGroup = true;
            } else {
                $scope.groupList = response;
                $scope.GetDomainCategoryData(response[0]._id);
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }

    //获取所有分组列表
    $scope.GetAllDomainCategory = function () {
        var url = "api/Google/GetAllDomainCategory?prjId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.length == 0) {
                $scope.hasGroup = true;
            }
            $scope.groupList = response;
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }

    //获取分组下的所有域名
    $scope.GetDomainCategoryData = function (id) {
        $scope.currentCategoryId = id;
        var url = "api/Google/GetDomainCategoryData?usrId=" + $rootScope.userID + "&categoryId=" + id + "&page=" + ($scope.pageDomainCategory - 1) + "&pagesize=" + $scope.pagesizeDomainCategory;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.changeCurrentTab1();
            $scope.domains = [];
            $scope.domains.push({x: ""});
            $scope.domainList = response.Result;
            $scope.domainCount = response.Count;
            $scope.activeId = response.DomainCategoryId;
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }

    //分页
    $scope.GetDomainCategoryData2 = function () {
        if ($scope.currentCategoryId) {
            $scope.GetDomainCategoryData($scope.currentCategoryId);
        } else {
            $scope.GetDomainCategoryData($scope.groupList[$scope.groupList.length - 1]._id);
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    //获取zTree数据
    $scope.GetTreeData = function () {
        var url = "/api/Google/GetAllFenZhu?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.zNodes = response;
            //让头部展开
            $scope.zNodes[0].open = true;
            //默认加载所有关键词分布气泡图
            var getId = [];
            for (var i = 1, len = $scope.zNodes.length; i < len; i++) {
                getId.push($scope.zNodes[i].id);
            }
            getId = getId.join(";");
            $scope.Dashboard1(getId);
            var setting = {
                check: {
                    enable: true,
                    chkboxType: {"Y": "s", "N": "ps"}
                },
                data: {
                    simpleData: {
                        enable: true
                    }
                },
                callback: {
                    onCheck: $scope.showEcharts
                }
            };
            $.fn.zTree.init($("#treeDemo"), setting, $scope.zNodes);
        });
        q.error(function (response) {
            $scope.error = "网络打盹了，请稍后。。。";
        });
    }

    //显示echarts图
    $scope.showEcharts = function (treeId, treeNode) {
        //有效链接和关键词分布图
        var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
        var nodes = treeObj.getCheckedNodes(true);
        $scope.treeNodeId = [];
        for (var i = 0, len = nodes.length; i < len; i++) {
            $scope.treeNodeId.push(nodes[i].id);
        }
        $scope.treeNodeId = $scope.treeNodeId.join(";");
        if ($scope.treeNodeId) {
            $scope.Dashboard1($scope.treeNodeId);
        }
    }
    $scope.Dashboard1 = function (id) {
        $scope.categoryId = id;
        $scope.D_GetBubbleList();
    }
    //气泡图
    $scope.D_GetBubbleList = function () {
        var url = "api/Google/GetAllDomainCategory?prjId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.domainCategory = response;
            $scope.domainCategoryIds = [];
            $scope.domainCategoryNames = [];
            for (var i = 0; i < $scope.domainCategory.length; i++) {
                $scope.domainCategoryIds.push($scope.domainCategory[i]._id);
                $scope.domainCategoryNames.push($scope.domainCategory[i].Name);
            }
        });
        q.error(function (e) {
            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });

        var url = "/api/Google/GetDomainStatis?categoryId=" + $scope.categoryId + "&prjId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
                console.log(response);
                var myChart = echarts.init(document.getElementById('D_GetBubbleList'));
                var datas = [];
                var dataNull = [];
                for (var i = 0; i < $scope.domainCategoryIds.length; i++) {
                    var data1 = [];
                    for (var j = 0; j < response.length; j++) {
                        if (response[j].DomainCategoryId == $scope.domainCategoryIds[i]) {
                            data1.push(
                                [response[j].Count,
                                    response[j].RankTotal,
                                    (response[j].KeywordTotal / 20 + 0.2),
                                    response[j].Domain,
                                    $filter('number')(parseFloat(response[j].PublishRatio), '2') + '%',
                                    response[j].DomainCategoryName]
                            );
                        } else if (response[j].DomainCategoryId === null) {
                            dataNull.push(
                                [response[j].Count,
                                    response[j].RankTotal,
                                    (response[j].KeywordTotal / 20 + 0.2),
                                    response[j].Domain,
                                    $filter('number')(parseFloat(response[j].PublishRatio), '2') + '%',
                                    response[j].DomainCategoryName]
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
                var schema = [
                    {index: 0, text: '分组名'},
                    {index: 1, text: '百度排名'},
                    {index: 2, text: '有效链接数'},
                    {index: 3, text: '关键词数'},
                    {index: 4, text: '含发布时间占比'}
                ];
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
                            return '<div style="border-bottom: 1px solid rgba(255,255,255,.3); font-size: 18px;padding-bottom: 7px;margin-bottom: 7px">'
                                + value[3]
                                + '</div>'
                                + schema[0].text + '：' + value[5] + '<br>'
                                + schema[1].text + '：' + value[1] + '<br>'
                                + schema[2].text + '：' + value[0] + '<br>'
                                + schema[3].text + '：' + Math.round((value[2] - 0.2) * 20) + '<br>'
                                + schema[4].text + '：' + value[4] + '<br>';
                        }
                    },
                    grid: {
                        left: '3%',
                        right: '10%',
                        bottom: '12%',
                        top:'15%',
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
                    dataZoom: [
                        {
                            type: 'slider',
                            height: 10,
                            show: true,
                            xAxisIndex: [0],
                            start: 0,
                            end: 100
                        },
                        {
                            type: 'slider',
                            width: 10,
                            show: true,
                            yAxisIndex: [0],
                            left: '93%',
                            start: 0,
                            end: 100
                        },
                        {
                            type: 'inside',
                            xAxisIndex: [0]
                        },
                        {
                            type: 'inside',
                            yAxisIndex: [0]
                        }
                    ],
                    series: function () {
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
                                    return val[2] * 40;
                                },
                                data: datas[i]
                            }
                            serie.push(item);
                        }
                        ;
                        return serie;
                    }()

                }
                myChart.setOption(option);
            }
        );
        q.error(function (response) {
            $scope.error = "网络打盹了，请稍后。。。";
            $scope.isActiveStart = false;

        });
    };

    //自动加载---------------------------------------------------
    $scope.GetAllDomainCategoryFrist();
});