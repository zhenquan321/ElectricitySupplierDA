var weiboDashboard_ctr = myApp.controller("weiboDashboard_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $filter, myApplocalStorage) {

    $scope.zNodes = [];
    $scope.categoryId = "";
    $scope.isActivepro1 = true;
    $scope.getNotRemovedKw = false;
    $scope.selectedTree = '';
    $scope.isActiveShowKw = true;
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);


    //获取zTree数据

    $scope.GetTreeData = function () {
        var url = "/api/weibo/GetAllFenZhu?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_dashboard_ctr>GetTreeData');
            $scope.zNodes = response;
            //console.log(response);
            //让头部展开
            $scope.zNodes[0].open = true;

            //默认加载有效链接

            //默认加载前五有效链接
            var getId = [];
            for (var i = 1, len = $scope.zNodes.length; i < len; i++) {
                getId.push($scope.zNodes[i].id);
                if (i == 4) {
                    break;
                }
            }
            getId = getId.join(";");
            if (getId) {
                $scope.Dashboard(getId);
            }
            //默认加载所有关键词分布气泡图
            var getId1 = [];
            for (var i = 1, len = $scope.zNodes.length; i < len; i++) {
                getId1.push($scope.zNodes[i].id);
            }
            getId1 = getId1.join(";");
            if (getId1) {
                $scope.Dashboard1(getId1);
            }
            //默认加载前4词频图
            if (getId) {
                $scope.quanzhongtu(getId);
            }
            //默认加载前20词云图
            if (getId) {
                $scope.cipintongji(getId);
            }

            //默认加载所有关键词
            $scope.getkeyword1($scope.zNodes[0].id);

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
                    beforeClick: $scope.getkeyword,
                    onCheck: $scope.showEcharts
                }

            };


            $.fn.zTree.init($("#treeDemo"), setting, $scope.zNodes);


        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    //圆形d3图
    $scope.GetD3TreeData = function () {
        var url = "/api/weibo/GetAllGroupTreeUrl?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_dashboard_ctr>GetTreeData');
            //console.log(response);
            //$scope.getJson(JSON.stringify(response));
            $scope.getJson(response.Url);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    //Radial Reingold–Tilford Tree_______________________


    var diameter = 600;

    var tree = d3.layout.tree()
        .size([360, diameter / 2 - 120])
        .separation(function (a, b) {
            return (a.parent == b.parent ? 1 : 2) / a.depth;
        });

    var diagonal = d3.svg.diagonal.radial()
        .projection(function (d) {
            return [d.y, d.x / 180 * Math.PI];
        });

    var svg = d3.select("#Tilford-Tree").append("svg")
        .attr("width", diameter)
        .attr("height", diameter - 50)
        .append("g")
        .attr("transform", "translate(" + diameter / 2 + "," + diameter / 2 + ")");

    $scope.getJson = function (jsonUrl) {
        d3.json(jsonUrl, function (error, root) {
            if (error) throw error;

            var nodes = tree.nodes(root),
                links = tree.links(nodes);

            var link = svg.selectAll(".link")
                .data(links)
                .enter().append("path")
                .attr("class", "link")
                .attr("d", diagonal);

            var node = svg.selectAll(".node")
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
        });
    }
    d3.select(self.frameElement).style("height", diameter - 50 + "px");

    //关键词对比图

    var margin = {top: 150, right: 0, bottom: 10, left: 150},
        width = 600,
        height = 600;

    var x = d3.scale.ordinal().rangeBands([0, width]),
        z = d3.scale.linear().domain([0, 4]).clamp(true),
        c = d3.scale.category10().domain(d3.range(10));

    var svg1 = d3.select("#bd").append("svg")
        .attr("width", width)
        .attr("height", height)
        //.style("margin-left", margin.left + "px")
        .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");


    $scope.GetData = function () {
        var url = "/api/weibo/GetKeywordBygroup?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_dashboard_ctr>GetData');
            if (response.Error) {
                alert(response.Error);
            } else {
                $scope.getjson(response.Url);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    $scope.getjson = function (url) {
        d3.json(url, function (miserables) {
            var matrix = [],
                nodes = miserables.nodes,
                n = nodes.length;

            //Compute index per node.
            nodes.forEach(function (node, i) {
                node.index = i;
                node.count = 0;
                matrix[i] = d3.range(n).map(function (j) {
                    return {x: j, y: i, z: 0};
                });
            });

            //Convert links to matrix; count character occurrences.
            miserables.links.forEach(function (link) {
                matrix[link.source][link.target].z += link.value;
                matrix[link.target][link.source].z += link.value;
                matrix[link.source][link.source].z += link.value;
                matrix[link.target][link.target].z += link.value;
                nodes[link.source].count += link.value;
                nodes[link.target].count += link.value;
            });

            //Precompute the orders.
            var orders = {
                name: d3.range(n).sort(function (a, b) {
                    return d3.ascending(nodes[a].name, nodes[b].name);
                }),
                count: d3.range(n).sort(function (a, b) {
                    return nodes[b].count - nodes[a].count;
                }),
                group: d3.range(n).sort(function (a, b) {
                    return nodes[b].group - nodes[a].group;
                })
            };

            //The default sort order.
            x.domain(orders.name);

            svg1.append("rect")
                .attr("class", "background")
                .attr("width", width * .7)
                .attr("height", height * .7);

            var row = svg1.selectAll(".row")
                .data(matrix)
                .enter().append("g")
                .attr("class", "row")
                .attr("transform", function (d, i) {
                    return "translate(0," + x(i) * .7 + ")";
                })
                .each(row);

            row.append("line")
                .attr("x2", width * .7);

            row.append("text")
                .attr("x", -6)
                .attr("y", x.rangeBand() / 2 * .7)
                .attr("dy", ".32em")
                .attr("text-anchor", "end")
                //设置字体大小为9像素
                .attr("style", "font-size:9px")
                .text(function (d, i) {
                    return nodes[i].name;
                });

            var column = svg1.selectAll(".column")
                .data(matrix)
                .enter().append("g")
                .attr("class", "column")
                .attr("transform", function (d, i) {
                    return "translate(" + x(i) * .7 + ")rotate(-90)";
                });

            column.append("line")
                .attr("x1", -width * .7);

            column.append("text")
                .attr("x", 6)
                .attr("y", x.rangeBand() / 2 * .7)
                .attr("dy", ".32em")
                .attr("text-anchor", "start")
                //设置字体大小为9像素
                .attr("style", "font-size:9px")
                .text(function (d, i) {
                    return nodes[i].name;
                });

            function row(row) {
                var cell = d3.select(this).selectAll(".cell")
                    .data(row.filter(function (d) {
                        return d.z;
                    }))
                    .enter().append("rect")
                    .attr("class", "cell")
                    .attr("x", function (d) {
                        return x(d.x) * .7;
                    })
                    .attr("width", x.rangeBand() * .7)
                    .attr("height", x.rangeBand() * .7)
                    .style("fill-opacity", function (d) {
                        return z(d.z);
                    })
                    .style("fill", function (d) {
                        return nodes[d.x].group == nodes[d.y].group ? c(nodes[d.x].group) : null;
                    })
                    .on("mouseover", mouseover)
                    .on("mouseout", mouseout);
            }

            function mouseover(p) {
                d3.selectAll(".row text").classed("active", function (d, i) {
                    return i == p.y;
                });
                d3.selectAll(".column text").classed("active", function (d, i) {
                    return i == p.x;
                });
            }

            function mouseout() {
                d3.selectAll("text").classed("active", false);
            }

            d3.select("#order").on("change", function () {
                clearTimeout(timeout);
                order(this.value);
            });

            function order(value) {
                x.domain(orders[value]);

                var t = svg1.transition().duration(2500);

                t.selectAll(".row")
                    .delay(function (d, i) {
                        return x(i) * 4;
                    })
                    .attr("transform", function (d, i) {
                        return "translate(0," + x(i) * .7 + ")";
                    })
                    .selectAll(".cell")
                    .delay(function (d) {
                        return x(d.x) * 4;
                    })
                    .attr("x", function (d) {
                        return x(d.x) * .7;
                    });

                t.selectAll(".column")
                    .delay(function (d, i) {
                        return x(i) * 4;
                    })
                    .attr("transform", function (d, i) {
                        return "translate(" + x(i) * .7 + ")rotate(-90)";
                    });
            }

            var timeout = setTimeout(function () {
                order("group");
                d3.select("#order").property("selectedIndex", 2).node().focus();
            }, 5000);
        });
    }


    //有效链接图

    $scope.D_lineChart = function () {
        var url = "/api/weibo/GetTimeLinkCount?categoryId=" + $scope.categoryId + "&prjId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_dashboard_ctr>lineChart1');
            console.log(response);
            //基于准备好的dom，初始化echarts实例
            //指定图表的配置项和数据
            var myChart = echarts.init(document.getElementById('D_lineChartWB'));
            var timeData1 = response.Times;
            //console.log(response.Times);
            var linkData = response.LineDataList;
            if (linkData.length > 4) {
                linkData.length = 4;
            }
            var timeData = [];
            for (var j = 0; j < timeData1.length; j++) {
                timeData[j] = $filter("date")(timeData1[j], "yyyy-MM-dd");
            }
            option = {
                title: {
                    text: '有效链接统计图',
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
                        }
                        ;
                        return biaoti;
                    }(),
                    //orient: 'vertical',
                    x: "160px"
                },
                toolbox: {
                    show: true,
                    feature: {
                        dataView: {readOnly: false}
                    },
                    top: '4px',
                    left: '130px'
                },
                dataZoom: [
                    {
                        type: 'slider',
                        height: 10,
                        show: true,
                        xAxisIndex: [0],
                        start: 0,
                        end: 100
                    }
                ],
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: timeData
                },
                yAxis: {
                    type: 'value',
                    name: '链接数(最多4条)',
                    axisLabel: {
                        formatter: '{value}'
                    }
                },
                series: (function () {
                    var serie = [];
                    for (var i = 0; i < linkData.length; i++) {
                        var item = {
                            name: linkData[i].name,
                            type: 'line',
                            data: linkData[i].LinkCount
                        }
                        serie.push(item);
                    }
                    ;
                    return serie;
                })()
            };


            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;
        });
    };

    //气泡图

    $scope.D_GetBubbleList = function () {
        var url = "/api/weibo/GetNickNameStatis?categoryId=" + $scope.categoryId + "&prjId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_dashboard_ctr>GetBubbleList2');
            console.log(response);
            var myChart = echarts.init(document.getElementById('D_GetBubbleListWB'));


            var data1 = [];

            for (var i = 0; i < response.length; i++) {
                data1.push([response[i].Count, response[i].RankTotal, (response[i].KeywordTotal / 20 + 0.2), response[i].Domain, $filter('number')(parseFloat(response[i].PublishRatio), '2') + '%']);
            }
            var schema = [
                {index: 0, text: '域名'},
                {index: 1, text: '关键词数'},
                {index: 2, text: '百度排名'},
                {index: 3, text: '有效链接数'},
                {index: 4, text: '含发布时间占比'}
            ];
            option = {
                title: {
                    text: '命中关键词域名分布图',
                },
                animation: false,
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
                            + schema[1].text + '：' + Math.round((value[2] - 0.2) * 20) + '<br>'
                            + schema[2].text + '：' + value[1] + '<br>'
                            + schema[3].text + '：' + value[0] + '<br>'
                            + schema[4].text + '：' + value[4] + '<br>';
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
                dataZoom: [
                    {
                        type: 'slider',
                        height: 10,
                        show: true,
                        xAxisIndex: [0],
                        start: 0,
                        end: 30
                    },
                    {
                        type: 'slider',
                        width: 10,
                        show: true,
                        yAxisIndex: [0],
                        left: '93%',
                        start: 0,
                        end: 30
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
                series: [
                    {
                        type: 'scatter',
                        itemStyle: {
                            normal: {
                                opacity: 0.8
                            }
                        },
                        symbolSize: function (val) {
                            return val[2] * 40;
                        },
                        data: data1
                    },
                ]
            }
            myChart.setOption(option);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
            $scope.isActiveStart = false;

        });
    };

    //字的权重图
    $scope.quanzhongtu = function (treeNodeId) {
        //$("#wordcloud1").prev().remove();
        var url = "/api/Jieba/WBExtract?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&categoryId=" + treeNodeId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $("#wordcloud1WB").html(response);
            //$('#wordcloud1').before('<h4 style="font-weight:700;margin:17px 0 0 17px;">关键词权重图</h4>');
            $("#wordcloud1WB").awesomeCloud({
                "size" : {
                    "grid" : 3, // 字间距
                    "factor" : 1, // 字体大小  0位自动
                    "normalize": true // 减少异常值，为更具吸引力的输出
                },
                "options" : {
                    "color" : "random-dark", // 背景颜色，默认为透明
                    "rotationRatio" : 0, // 0都是水平的，1都是垂直的
                    "printMultiplier" : 1, // 设置为3，好的打印机输出；更高的数字需要更长的时间
                    "sort" : "highest" // “最高”，以显示大的话先，“最低”的话先做小的话，“随机”不关心
                },
                "font" : "'Microsoft Yahei','Times New Roman', Times, serif", // 设置字体样式
                "shape" : "circle" // 设置显示形状 矩形square，钻石diamond，三角形triangle（-forward），五角形pentagon，星形star，形x,默认圆形circle
            });
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    //词频图
    $scope.cipintongji = function (treeNodeId) {
        var url = "/api/Jieba/WBFrequency?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&categoryId=" + treeNodeId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log(response);
            $scope.cipintu(response.noun, response.nounCount, response.verb, response.verbCount);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    $scope.Dashboard = function (id) {

        $scope.categoryId = id;

        $scope.D_lineChart();

    }
    $scope.Dashboard1 = function (id) {

        $scope.categoryId = id;

        $scope.D_GetBubbleList();

    }


    $scope.cipintu = function (n, nc, v, vc) {
        //词频柱状图
        //名词
        var myChart = echarts.init(document.getElementById('WordFrequencyMWB'));

        option = {
            title: {
                text: '频词统计图',
                subtext: '高频词n'
            },
            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'shadow'
                }
            },
            grid: {
                left: '3%',
                right: '4%',
                bottom: '3%',
                containLabel: true
            },
            xAxis: {
                type: 'value',
                boundaryGap: [0, .1]
            },
            yAxis: {
                type: 'category',
                data: n.reverse()
            },
            series: [
                {
                    name: '名词',
                    type: 'bar',
                    data: nc.reverse()
                    //itemStyle: { normal: { label: { show: true, position: 'insideTop', textStyle: { color: '#fff' } } } },
                }
            ]
        };

        myChart.setOption(option);

        //动词

        var myChart = echarts.init(document.getElementById('WordFrequencyDWB'));

        option = {
            title: {
                //text: '高频词v',
                subtext: '高频词v'

            },
            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'shadow'
                }
            },
            grid: {
                left: '3%',
                right: '4%',
                bottom: '3%',
                containLabel: true
            },
            xAxis: {
                type: 'value',
                boundaryGap: [0, .1]
            },
            yAxis: {
                type: 'category',
                data: v.reverse()
            },
            series: [
                {
                    name: '名词',
                    type: 'bar',
                    data: vc.reverse(),
                    //itemStyle: { normal: { color: 'rgba(81,98,110,1)', label: { show: true, position: 'insideTop', textStyle: { color: '#fff' } } } },
                    itemStyle: {normal: {color: 'rgba(81,98,110,1)'}},
                }
            ]
        };

        myChart.setOption(option);
    }

    //加载所有关键词
    //选择分组后加载关键词
    $scope.getkeyword = function (treeId, treeNode) {
        $scope.selectedTree = treeNode;
        var url = "/api/weibo/GetFenleiKeywords?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&treeNodeId=" + treeNode.id + "&status=" + $scope.getNotRemovedKw;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetAllKeywordCategory_list = response;
            console.log($scope.GetAllKeywordCategory_list);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
    //未排除的关键词
    $scope.getkeyword1 = function (id) {
        var url = "/api/weibo/GetFenleiKeywords?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&treeNodeId=" + id + "&status=" + $scope.getNotRemovedKw;
        var q = $http.get(url);
        q.success(function (response, status) {
            $scope.GetAllKeywordCategory_list = response;
            $scope.GetBaiduLevelLinks2(response[0].id);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    //显示echarts图     
    $scope.showEcharts = function (treeId, treeNode) {
        //有效链接和关键词分布图
        var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
        var nodes = treeObj.getCheckedNodes(true);
        //console.log(nodes);
        var treeNodeId = [];
        for (var i = 0, len = nodes.length; i < len; i++) {
            treeNodeId.push(nodes[i].id);
        }
        treeNodeId = treeNodeId.join(";");
        if (treeNodeId) {
            $scope.Dashboard(treeNodeId);
            $scope.Dashboard1(treeNodeId);
            $scope.quanzhongtu(treeNodeId);
            $scope.cipintongji(treeNodeId);
        }
        console.log(treeNodeId);
    }


    //实体库切换
    $scope.changepro1_show = function () {
        $scope.isActivepro1 = true;
    }
    $scope.changepro1_hide = function () {
        $scope.isActivepro1 = false;
        $scope.GetBaiduKeyword();
    }

    //2.3搜索记录的结果
    //2.3.1 百度搜索记录的结果

    $scope.GetBaiduLevelLinks = function (id) {
        var url = "/api/Weibo/GetWBLinks?user_id=" + $rootScope.userID + "&searchTaskId=" + id + "&projectId=" + $rootScope.getProjectId + "&page=" + ($scope.page2 - 1) + "&pagesize=" + $scope.pagesize2 + "&status=" + $scope.status;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('iw2s_ctr>GetBaiduLevelLinks');
            $scope.BaiduCount = response.Count;
            console.log(response);
            if (response != null) {
                $rootScope.WBresultList = response.Result;
                //console.log($rootScope.WBresultList)
                $scope.Count = response.Count;
            }
        })
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }


    //2.3.1.1分页
    $scope.GetBaiduLevelLinks2 = function (id) {
        if (id != "" && id != null) {
            $rootScope.BaidukeywordId = id;
            $cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);
        }
        $scope.GetBaiduLevelLinks(id)
    }

    //排除关键词
    var checkedId = [];
    $scope.chk = function (id, aa) {
        if (aa) {
            checkedId.push(id);
        } else {
            for (var i = 0; i < checkedId.length; i++) {
                if (checkedId[i] == id) {
                    checkedId.splice(i, 1);
                    break;
                }
            }
        }
        console.log(checkedId);
    }

    $scope.allchk = function (cal) {
        if (cal) {
            checkedId = [];
            for (var i = 0; i < $scope.GetAllKeywordCategory_list.length; i++) {
                checkedId.push($scope.GetAllKeywordCategory_list[i].id);
            }
        } else {
            checkedId = [];
        }
    }

    $scope.cancelCheckedword = function () {
        if (confirm("确定要删除记录吗？")) {
            checkedId = checkedId.join(";");
            var IsRemoved = true;
            var url = "api/weibo/SetKeywordStatus?categoryId=" + checkedId + "&status=" + IsRemoved;
            var q = $http.get(url);
            q.success(function (response, status) {
                $scope.addAlert('success', "删除成功！");
                //$rootScope.BaidukeywordId = "";
                //$cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);
                $scope.GetTreeData();
                $scope.GetData();
                $scope.GetD3TreeData();
                checkedId = [];
            });
            q.error(function (e) {
                $scope.addAlert('danger', "服务器连接出错");
            });
        }
    }
    //恢复
    $scope.recover = function () {
        if (confirm("确定要恢复记录吗？")) {
            checkedId = checkedId.join(";");
            var IsRemoved = false;
            var url = "api/weibo/SetKeywordStatus?categoryId=" + checkedId + "&status=" + IsRemoved;
            var q = $http.get(url);
            q.success(function (response, status) {
                $scope.addAlert('success', "恢复成功！");
                //$rootScope.BaidukeywordId = "";
                //$cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);
                $scope.GetTreeData();
                $scope.GetData();
                $scope.GetD3TreeData();
                checkedId = [];
            });
            q.error(function (e) {
                $scope.addAlert('danger', "服务器连接出错");
            });
        }
    }
    //显示侵权词
    $scope.showNotRemovedKw = function () {
        $scope.isActiveShowKw = true;
        $scope.getNotRemovedKw = false;
        if (!$scope.selectedTree.id) {
            $scope.getkeyword1($scope.zNodes[0].id);
        } else {
            $scope.getkeyword1($scope.selectedTree.id);
        }
    }
    //显示排除词
    $scope.showRemovedKw = function () {
        $scope.isActiveShowKw = false;
        $scope.getNotRemovedKw = true;
        console.log($scope.selectId)
        if (!$scope.selectedTree.id) {
            $scope.getkeyword1($scope.zNodes[0].id);
        } else {
            $scope.getkeyword1($scope.selectedTree.id);
        }

    }


    //2.2.1加载更多
    $scope.GetMoreBaiduKeyword = function () {

        $rootScope.pagesizeBaidu = $rootScope.pagesizeBaidu + 10;
        $cookieStore.put("pagesizeBaidu", $rootScope.pagesizeBaidu);
        $scope.GetBaiduKeyword($rootScope.getProjectId);
    }


    //自动加载________________________________________________________

    //折叠文件夹
    $scope.GetTreeData();

    //关键词对比
    $scope.GetData();

    //圆形d3表
    $scope.GetD3TreeData();


});



