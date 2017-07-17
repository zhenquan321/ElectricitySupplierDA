var iw2s_dashboard_ctr = myApp.controller("iw2s_dashboard_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $filter, $timeout, myApplocalStorage) {

	$scope.zNodes = [];
	$scope.categoryId = "";
	$scope.isActivepro1 = true;
	$scope.getNotRemovedKw = false;
	$scope.selectedTree = '';
	$scope.isActiveShowKw = true;
	$scope.TimeLinkList = [];
	$scope.m = 0;
	$scope.fenyeNum = 3;
	$scope.activeNum = 1;
	$scope.searchTime = '';
	$scope.startTime = '';
	$scope.endTime = '';
	$scope.startNum = 80;
	$scope.endNum = 100;
	$scope.showLink = true;
	$scope.pagesize2 = 5;
	$scope.changeBig = 1;
	$scope.StopWord = {
	    InsertStopWordInput:''
	}
	//用于分享include下的变量
	$scope.data = {
		topNum: 25,
		percent: 0,
		searchTime: '',
		num_1: '',
		page2: 1
	};

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
	}];

	chk_global_vars($cookieStore, $rootScope, null, $location, $http);

	//获取zTree数据

	$scope.GetTreeData = function() {
		var url = "/api/Keyword/GetAllFenZhu?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
		var q = $http.get(url);
		q.success(function(response, status) {
			console.log('iw2s_dashboard_ctr>GetTreeData');
			$scope.zNodes = response;
			//console.log(response);
			//让头部展开
			$scope.zNodes[0].open = true;

			//默认加载有效链接

			//默认加载前8有效链接
			var getId = [];
			for (var i = 0, len = $scope.zNodes.length; i < len; i++) {
				getId.push($scope.zNodes[i].id);
				if (i == 9) {
					break;
				}
			}
			getId = getId.join(";");
			$scope.getId = getId;
			//将默认加载的前几个id赋值给$scope
			if (getId) {
				$scope.Dashboard(getId);
			}
			//默认加载所有关键词分布气泡图
			var getId1 = [];
			for (var i = 0, len = $scope.zNodes.length; i < len; i++) {
				getId1.push($scope.zNodes[i].id);
			}
			getId1 = getId1.join(";");
			if (getId1) {
				$scope.Dashboard1(getId1);
			}
			//默认加载前8词频图
			if (getId) {
				$scope.quanzhongtu(getId);
			}
			//默认加载前8词云图
			if (getId) {
			    $scope.cipintongji(getId);
			   
			}
			//默认加载前8链接表格
			if (getId) {
				$scope.GetTimeLinkList(getId);
			}

			//默认加载所有关键词
			$scope.getkeyword1($scope.zNodes[0].id);

			var setting = {
				check: {
					enable: true,
					chkboxType: {
						"Y": "s",
						"N": "ps"
					}
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
		q.error(function(response) {
			$scope.error = "网络打盹了，请稍后。。。";
		});
	}

	//放大折线图
	$scope.ChangeBigFun = function(num, selectedTime) {
		$scope.changeBig = num;
		$timeout(function() {
			$scope.D_lineChart_Change();
			$scope.D_PieChart($scope.D_PieChartData);
		}, 500);
	}

	$scope.D_lineChart_Change = function() {
		var response = $scope.D_lineChartList;
		//基于准备好的dom，初始化echarts实例
		//指定图表的配置项和数据
		var myChart = echarts.init(document.getElementById('D_lineChart'));
		var timeData1 = response.Times;
		//console.log(response.Times);

		var zhaiyao = response.Sum;
		$scope.D_PieChart(zhaiyao);

		var linkData = response.LineDataList;
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
				data: function() {
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
			series: (function() {
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
		myChart.on('datazoom', function(params) {
			$scope.startTime = timeData[parseInt((params.start * timeData.length) / 100)];
			$scope.endTime = timeData[parseInt((params.end * timeData.length) / 100)];
			if (params.end == 100) {
				$scope.endTime = timeData[parseInt((params.end * timeData.length) / 100) - 1];
			}
			$scope.startNum = Math.round(params.start);
			$scope.endNum = Math.round(params.end);
		});
		//自动摘要
		myChart.on('click', function(params) {
			console.log(params);
			var time = params.data.xAxis;
			$scope.searchLinkByTime(time);
			$scope.data.searchTime = time;
			console.log(time)
		});

	};

	//有效链接图

	$scope.D_lineChart = function(timeValue, config) {
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
			$scope.categoryId = config.categoryId;
			timeInterval = config.timeInterval;
			for(var i=0;i<$scope.timeOptions.length;i++){
				if($scope.timeOptions[i].value==timeInterval){
				$scope.selectedTime=$scope.timeOptions[i];
				}
			}
			if(i==3){
				$scope.selectedTime=$scope.timeOptions[3];
			}
		}
		url = "/api/Keyword/GetTimeLinkCount?categoryIds=" + $scope.categoryId + "&prjId=" + $rootScope.getProjectId + "&startTime=" + $scope.startTime + "&endTime=" + $scope.endTime + "&percent=" + $scope.data.percent + "&topNum=" + $scope.data.topNum + "&sumNum=" + 25 + "&timeInterval=" + timeInterval;
		var q = $http.get(url);
		q.success(function(response, status) {
			console.log('iw2s_dashboard_ctr>lineChart1');
			console.log(response);
			$scope.D_lineChartList = response;
			//基于准备好的dom，初始化echarts实例
			//指定图表的配置项和数据
			var myChart = echarts.init(document.getElementById('D_lineChart'));
			var timeData1 = response.Times;
			//console.log(response.Times);

			var zhaiyao = response.Sum;
			$scope.D_PieChart(zhaiyao);

			var linkData = response.LineDataList;
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
					data: function() {
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
				series: (function() {
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
			myChart.on('datazoom', function(params) {
				$scope.startTime = timeData[parseInt((params.start * timeData.length) / 100)];
				$scope.endTime = timeData[parseInt((params.end * timeData.length) / 100)];
				if (params.end == 100) {
					$scope.endTime = timeData[parseInt((params.end * timeData.length) / 100) - 1];
				}
				$scope.startNum = Math.round(params.start);
				$scope.endNum = Math.round(params.end);
			});
			//自动摘要
			myChart.on('click', function(params) {
				console.log(params);

				var time = params.data.xAxis;
				$scope.searchLinkByTime(time);
				$scope.data.searchTime = time;
				console.log(time)
					//var url = "api/Jieba/GetSummary?projectId=" + $rootScope.getProjectId + "&time=" + params.data.coord[0] + "&categoryName=" + params.name + "&source=";
					//var q = $http.get(url);
					//q.success(function (response, status) {
					//  console.log(response);
					//  $scope.paragraph = response;
					//    //$scope.showLink = false;自动摘要切换

				//});
				//q.error(function (e) {
				//  $scope.addAlert('danger', "网络打盹了，请稍后。。。");
				//});

			});
		});
		q.error(function(response) {
			$scope.error = "网络打盹了，请稍后。。。";
			$scope.isActiveStart = false;
		});

	};

	$scope.D_PieChart = function(date) {
			$scope.D_PieChartData = date;
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
	$scope.changeLink = function() {
		$scope.showLink = !$scope.showLink;
	}

	//有效链接图清除按钮
	$scope.clearModel = function() {
		$scope.data.percent = 0;
		$scope.data.topNum = 0;
	}

	//气泡图

	$scope.D_GetBubbleList = function() {
		var url = "api/Keyword/GetAllDomainCategory?prjId=" + $rootScope.getProjectId;
		var q = $http.get(url);
		q.success(function(response, status) {
			$scope.domainCategory = response;
			$scope.domainCategoryIds = [];
			$scope.domainCategoryNames = [];
			for (var i = 0; i < $scope.domainCategory.length; i++) {
				$scope.domainCategoryIds.push($scope.domainCategory[i]._id);
				$scope.domainCategoryNames.push($scope.domainCategory[i].Name);
			}
			var url = "/api/Keyword/GetDomainStatis?categoryIds=" + $scope.categoryId + "&projectId=" + $rootScope.getProjectId;
			var q = $http.get(url);
			q.success(function (response, status) {
			    console.log(response);
			    var myChart = echarts.init(document.getElementById('D_GetBubbleList'));
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
                                    response[j].DomainColNum,
                                    (response[j].KeywordTotal * bobuSize + 5),
                                    response[j].Domain,
                                    $filter('number')(parseFloat(response[j].PublishRatio), '2') + '%',
                                    response[j].DomainCategoryName
                                ]
                            );
			            } else if (response[j].DomainCategoryId === null) {
			                dataNull.push(
                                [response[j].Count,
                                    response[j].DomainColNum,
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
			        text: '域名收录量'
			    }, {
			        index: 2,
			        text: '链接数'
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
			            top: 20,
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
			            name: '域名收录量',
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
			});
			q.error(function (response) {
			    $scope.error = "网络打盹了，请稍后。。。";
			    $scope.isActiveStart = false;

			});
		});
		q.error(function(e) {
			$scope.addAlert('danger', "网络打盹了，请稍后。。。");
		});

		
	};

	//字的权重图
	$scope.quanzhongtu = function(treeNodeId) {
		//$("#wordcloud1").prev().remove();
		var url = "/api/Jieba/BaiduExtract?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&categoryId=" + treeNodeId;
		var q = $http.get(url);
		q.success(function(response, status) {
			console.log(response);
			$("#wordcloud1").html(response);
			//$('#wordcloud1').before('<h4 style="font-weight:700;margin:17px 0 0 17px;">关键词权重图</h4>');
			$("#wordcloud1").awesomeCloud({
				"size": {
					"grid": 3, // 字间距
					"factor": 3, // 字体大小  0位自动
					"normalize": true // 减少异常值，为更具吸引力的输出
				},
				"options": {
					"color": "random-dark", // 背景颜色，默认为透明
					"rotationRatio": 0, // 0都是水平的，1都是垂直的
					"printMultiplier": 1, // 设置为3，好的打印机输出；更高的数字需要更长的时间
					"sort": "highest" // “最高”，以显示大的话先，“最低”的话先做小的话，“随机”不关心
				},
				"font": "'Microsoft Yahei','Times New Roman', Times, serif", // 设置字体样式
				"shape": "circle" // 设置显示形状 矩形square，钻石diamond，三角形triangle（-forward），五角形pentagon，星形star，形x,默认圆形circle
			});
		});
		q.error(function(response) {
			$scope.error = "网络打盹了，请稍后。。。";
		});
	}

	//词频图
	$scope.cipintongji = function(treeNodeId) {
		var url = "/api/Jieba/BaiduFrequency?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&categoryId=" + treeNodeId;
		var q = $http.get(url);
		q.success(function(response, status) {
			console.log(response);
			$scope.cipintu(response.noun, response.nounCount, response.verb, response.verbCount);
		});
		q.error(function(response) {
			$scope.error = "网络打盹了，请稍后。。。";
		});
	}

	$scope.Dashboard = function(id) {

		$scope.categoryId = id;

		$scope.D_lineChart(null, null);

	}
	$scope.Dashboard1 = function(id) {

		$scope.categoryId = id;

		$scope.D_GetBubbleList();

	}

	$scope.cipintu = function(n, nc, v, vc) {
		//词频柱状图
		//名词
		var myChart = echarts.init(document.getElementById('WordFrequencyM'));

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
			series: [{
				name: '名词',
				type: 'bar',
				data: nc.reverse(),
				itemStyle: {
					normal: {
						label: {
							show: true,
							position: 'insideTop',
							textStyle: {
								color: '#fff'
							}
						}
					}
				}
			}]
		};

		myChart.setOption(option);

		//动词

		var myChart = echarts.init(document.getElementById('WordFrequencyD'));

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
			series: [{
				name: '名词',
				type: 'bar',
				data: vc.reverse(),
				itemStyle: {
					normal: {
						color: 'rgba(81,98,110,1)',
						label: {
							show: true,
							position: 'insideTop',
							textStyle: {
								color: '#fff'
							}
						}
					}
				}
				//itemStyle: {normal: {color: 'rgba(81,98,110,1)'}}
			}]
		};

		myChart.setOption(option);
	}

	//搜索发表时间显示链接表格
	$scope.searchLinkByTime = function(time) {
		$scope.data.searchTime = time;
		var re = /((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9])$)|(^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)$))/;
		if (re.test($scope.data.searchTime) || $scope.data.searchTime == '') {
			if (!$scope.treeNodeId) {
				$scope.treeNodeId1 = $scope.getId;
			} else {
				$scope.treeNodeId1 = $scope.treeNodeId;
			}
			var url = "/api/Keyword/GetTimeLinkList?usr_id=" + $rootScope.userID + "&prjId=" + $rootScope.getProjectId + "&categoryId=" + $scope.treeNodeId1 + "&pubTime=" + time;
			var q = $http.get(url);
			q.success(function(response, status) {
				console.log(response);
				if (response.Count == 0) {
					alert('没有相关数据！')
				}
				$scope.TimeLinkList = response.Result;
				$scope.TimeLinkList1 = $scope.TimeLinkList.slice(0, 10);
				$scope.skipTo(1);
			});
			q.error(function(response) {
				$scope.error = "网络打盹了，请稍后。。。";
			});
		} else {
			alert('输入日期格式不对或日期不存在！')
			return;
		}

	}

	//显示链接表格
	$scope.GetTimeLinkList = function(treeNodeId) {
		var url = "/api/Keyword/GetTimeLinkList?usr_id=" + $rootScope.userID + "&prjId=" + $rootScope.getProjectId + "&categoryId=" + treeNodeId + "&pubTime=";
		var q = $http.get(url);
		q.success(function(response, status) {
			console.log(response);
			$scope.TimeLinkList = response.Result;
			$scope.TimeLinkList1 = $scope.TimeLinkList.slice(0, 10);
			$scope.skipTo(1);
		});
		q.error(function(response) {
			$scope.error = "网络打盹了，请稍后。。。";
		});
	}

	//前端分页函数

	$scope.skipTo = function(Num1) {
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

	//加载所有关键词
	//选择分组后加载关键词
	$scope.getkeyword = function(treeId, treeNode) {
			//单击关键词选中的id赋值给$scope
			$scope.selectedTree = treeNode;
			var url = "/api/Keyword/GetFenleiKeywords?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&treeNodeId=" + treeNode.id + "&status=" + $scope.getNotRemovedKw;
			var q = $http.get(url);
			q.success(function(response, status) {
				$scope.GetAllKeywordCategory_list = response;
				console.log($scope.GetAllKeywordCategory_list);
			});
			q.error(function(response) {
				$scope.error = "网络打盹了，请稍后。。。";
			});
		}
		//未排除的关键词
	$scope.getkeyword1 = function(id) {
		var url = "/api/Keyword/GetFenleiKeywords?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId + "&treeNodeId=" + id + "&status=" + $scope.getNotRemovedKw;
		var q = $http.get(url);
		q.success(function(response, status) {
			$scope.GetAllKeywordCategory_list = response;
			$scope.GetBaiduLevelLinks2(response[0].id);
		});
		q.error(function(response) {
			$scope.error = "网络打盹了，请稍后。。。";
		});
	}

	//显示echarts图
	$scope.showEcharts = function(treeId, treeNode) {
		//有效链接和关键词分布图
		var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
		var nodes = treeObj.getCheckedNodes(true);
		//console.log(nodes);
		var treeNodeId = [];
		for (var i = 0, len = nodes.length; i < len; i++) {
			treeNodeId.push(nodes[i].id);
		}
		treeNodeId = treeNodeId.join(";");
		//checkbox勾选的id赋值给$scope
		$scope.treeNodeId = treeNodeId;
		if (treeNodeId) {
			$scope.Dashboard(treeNodeId);
			$scope.Dashboard1(treeNodeId);
			$scope.quanzhongtu(treeNodeId);
			$scope.cipintongji(treeNodeId);
			$scope.GetTimeLinkList(treeNodeId);
		}
		console.log(treeNodeId);
	}

	//实体库切换
	$scope.changepro1_show = function() {
		$scope.isActivepro1 = true;
	}
	$scope.changepro1_hide = function() {
		$scope.isActivepro1 = false;
		$scope.GetBaiduKeyword();
	}

	//2.3搜索记录的结果
	//2.3.1 百度搜索记录的结果

	$scope.GetBaiduLevelLinks = function() {
			var url = "/api/Keyword/GetLevelLinks?user_id=" + $rootScope.userID + "&categoryId=" + $rootScope.categoryId + "&projectId=" + $rootScope.getProjectId + "&keywordId=" + $rootScope.BaidukeywordId + "&Title=" + $scope.Title +
				"&domain=" + $scope.domain + "&infriLawCode=" + $scope.infriLawCode + "&page=" + ($scope.data.page2 - 1) + "&pagesize=" + $scope.pagesize2 + "&status=" + $scope.status;
			var q = $http.get(url);
			q.success(function(response, status) {
				console.log('showDesc_ctr>GetBaiduLevelLinks');
				$scope.BaiduCount = response.Count;

				if (response != null) {
					$rootScope.resultList = response.Result;
					console.log($rootScope.resultList)
					$scope.Count = response.Count;
				}
			})
			q.error(function(response) {
				$scope.error = "网络打盹了，请稍后。。。";
			});
		}
		//2.3.1.1分页
	$scope.GetBaiduLevelLinks2 = function(id) {
		if (id != "" && id != null) {
			$rootScope.BaidukeywordId = id;
			$cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);
		}
		$scope.GetBaiduLevelLinks()
	}

	//排除关键词
	var checkedId = [];
	$scope.chk = function(id, aa) {
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

	$scope.allchk = function(cal) {
		if (cal) {
			checkedId = [];
			for (var i = 0; i < $scope.GetAllKeywordCategory_list.length; i++) {
				checkedId.push($scope.GetAllKeywordCategory_list[i].id);
			}
		} else {
			checkedId = [];
		}
	}

	$scope.cancelCheckedword = function() {
			if (confirm("确定要删除记录吗？")) {
				checkedId = checkedId.join(";");
				var IsRemoved = true;
				var url = "api/Keyword/SetKeywordStatus?categoryId=" + checkedId + "&status=" + IsRemoved;
				var q = $http.get(url);
				q.success(function(response, status) {
					$scope.addAlert('success', "删除成功！");
					//$rootScope.BaidukeywordId = "";
					//$cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);
					$scope.GetTreeData();
					$scope.GetData();
					$scope.GetD3TreeData();
					checkedId = [];
				});
				q.error(function(e) {
					$scope.addAlert('danger', "网络打盹了，请稍后。。。");
				});
			}
		}
		//恢复
	$scope.recover = function() {
			if (confirm("确定要恢复记录吗？")) {
				checkedId = checkedId.join(";");
				var IsRemoved = false;
				var url = "api/Keyword/SetKeywordStatus?categoryId=" + checkedId + "&status=" + IsRemoved;
				var q = $http.get(url);
				q.success(function(response, status) {
					$scope.addAlert('success', "恢复成功！");
					//$rootScope.BaidukeywordId = "";
					//$cookieStore.put("BaidukeywordId", $rootScope.BaidukeywordId);
					$scope.GetTreeData();
					$scope.GetData();
					$scope.GetD3TreeData();
					checkedId = [];
				});
				q.error(function(e) {
					$scope.addAlert('danger', "网络打盹了，请稍后。。。");
				});
			}
		}
		//显示侵权词
	$scope.showNotRemovedKw = function() {
			$scope.isActiveShowKw = true;
			$scope.getNotRemovedKw = false;
			if (!$scope.selectedTree.id) {
				$scope.getkeyword1($scope.zNodes[0].id);
			} else {
				$scope.getkeyword1($scope.selectedTree.id);
			}
		}
		//显示排除词
	$scope.showRemovedKw = function() {
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
	$scope.GetMoreBaiduKeyword = function() {

		$rootScope.pagesizeBaidu = $rootScope.pagesizeBaidu + 10;
		$cookieStore.put("pagesizeBaidu", $rootScope.pagesizeBaidu);
		$scope.GetBaiduKeyword($rootScope.getProjectId);
	}

	//打开保存设置模态框
	$scope.SaveChart_OT = function(selectedTime) {
		var CP_scope = $scope;
		//获取当前时间间隔
		CP_scope.timeInter = selectedTime.value;
		CP_scope.isReport = false;
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
	$scope.GetChart_OT = function() {
		var CP_scope = $scope;
		CP_scope.isReport = false;
		var frm = $modal.open({
			templateUrl: 'Scripts/app/views/modal/getChart.html',
			controller: getChart_ctr,
			scope: CP_scope,
			// label: label,
			keyboard: false,
			backdrop: 'static',
			size: 'cd'
		});
		frm.result.then(function(response, status) {
			//$rootScope.GetBaiduSearchKeyword2();
		});
	};

    //添加停用词
	$scope.InsertStopWord = function () {
	    $scope.StopWord.InsertStopWordInput = $("#InsertStopWordInput").val();

	    var url = "api/Jieba/InsertStopWord?userId=" + $rootScope.userID + "&words=" + $scope.StopWord.InsertStopWordInput;
	        var q = $http.get(url);
	        q.success(function (response, status) {
	            if (response.IsSuccess) {
	                $scope.alert_fun('success', '添加成功！');
	                $scope.GetStopWord();
	                $scope.quanzhongtu($scope.getId);
	                $scope.cipintongji($scope.getId);
	            } else {
	                $scope.alert_fun('danger', response.Message);
	            }
	        });
	        q.error(function (e) {
	            $scope.addAlert('danger', "网络打盹了，请稍后。。。");
	        });
	  
	}
    //取消添加停用词
	$scope.NoInsertStopWord = function () {
	    $("#InsertStopWordInput").val("");
	}
    //获取停用词
	$scope.GetStopWord = function () {
	    var url = "api/Jieba/GetStopWord?userId=" + $rootScope.userID;
	    var q = $http.get(url);
	    q.success(function (response, status) {
	        $scope.GetStopWordList = response.Words;
	        console.log(response);
	    });
	    q.error(function (e) {
	        $scope.addAlert('danger', "网络打盹了，请稍后。。。");
	    });

	}
    //删除停用词
	$scope.DelStopWord = function (keywords) {
	    var url = "api/Jieba/DelStopWord?userId=" + $rootScope.userID + "&words=" + keywords;
	    var q = $http.get(url);
	    q.success(function (response, status) {
	        if (response.IsSuccess) {
	            $scope.alert_fun('success', '删除成功！');
	            $scope.GetStopWord();
	            $scope.quanzhongtu($scope.getId);
	            $scope.cipintongji($scope.getId);
	        } else {
	            $scope.alert_fun('danger', response.Message);
	        }
	    });
	    q.error(function (e) {
	        $scope.addAlert('danger', "网络打盹了，请稍后。。。");
	    });

	}
    //文章数据分析
	$scope.discourseAnalysis = function (id) {
	    $rootScope.discAId = id;
	    var url = "/DnL/wordtree?id=" + $rootScope.discAId + "&score=1";
	    $location.url(url);
	}



	//自动加载________________________________________________________

	//折叠文件夹
	$scope.GetTreeData();
	$scope.GetStopWord();
});