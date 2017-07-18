var keywordMng_ctr = myApp.controller("keywordMng_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $modal, $filter, $timeout, myApplocalStorage) {


  $scope.CurSelectedKeyword = [];

  $scope.keywordCisActive = 1;
  $scope.Analysis_list = [];
  $scope.isActive_AnalysisZ1 = true;
  $scope.isActive_AnalysisZ2 = false;
  $scope.isActive_AnalysisZ3 = false;
  $scope.Analysis_name = '';
  $scope.Analysis_list = [];
  $scope.Analysis_id = "";
  $scope.isactriveChgSelIt = true;
  $scope.isActiveAnalysis_selected = true;
  $scope.isgroup = true;
  $scope.GXtimeInterval = 0;
  $scope.TopListShow = 1;
  $scope.NamedEntity = false;
  $scope.addEntityLeishow = true;
  $scope.WebRelations = true;
  chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
  //_______________________________________________________________


  //14.下拉框

  $scope.changePrjAsIt_list1 = function () {
      console.log($rootScope.PrjAnalysisItemName_list);
      if (!$rootScope.PrjAnalysisItemName_list) {
          return
      }
    $scope.InfriTypes1 = new Array();
    for (var i = 0; i < $rootScope.PrjAnalysisItemName_list.length; i++) {
      $scope.InfriTypes1[$rootScope.PrjAnalysisItemName_list[i]._id] = $rootScope.PrjAnalysisItemName_list[i].Name;
    }
  };
  $scope.changePrjAsIt_list1();


  $scope.kindGroup = function (groupId, InfriLawCode, InfriLawCodeStr, Weight, ParentName, ParentId) {

    $scope.CurSelectedKeyword = [];
    $scope.InfriLawCode = InfriLawCode;
    $scope.Weight1 = Weight;
    $scope.InfriLawCodeStr1 = InfriLawCodeStr;
    var url = "/api/Keyword/GetKeywordGroup?usr_id=" + $rootScope.userID + "&groupType=" + $scope.groupType + "&projectId=" + $rootScope.getProjectId;
    if ($scope.keywordId != null) {
      url += "&keywordId=" + $scope.keywordId;
    }
    else {
      url += "&keywordId=";
    }
    if (groupId != null) {
      url += "&groupid=" + groupId;
    }
    else {
      url += "&groupid=";
    }

    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('keywordMng_ctr>kindGroup');
      $scope.SelectedKeyword = response;
      if ($scope.InfriLawCode == null || $scope.InfriLawCode == "") {
        $scope.InfriTypeschange = true;
      } else {
        $scope.InfriTypeschange = false;
      }

      $scope.modelCallBack(groupId, 1, null, $scope.InfriLawCode, 0, ParentName, ParentId);
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  };

  $scope.EditKindGroup = function (groupId, parentid, Name, InfriLawCode, Weight, ParentName, ParentId) {
    $scope.CurSelectedKeyword = [];

    var url = "/api/Keyword/GetEditKeywordGroup?usr_id=" + $rootScope.userID + "&groupType=" + $scope.groupType + "&projectId=" + $rootScope.getProjectId;
    if ($scope.keywordId != null) {
      url += "&keywordId=" + $scope.keywordId;
    }
    else {
      url += "&keywordId=";
    }
    if (groupId != null) {
      url += "&groupid=" + groupId;
    }
    else {
      url += "&groupid=";
    }
    if (parentid != null) {
      url += "&parentid=" + parentid;
    }
    else {
      url += "&parentid=";
    }

    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('keywordMng_ctr>EditKindGroup');
      if (response != null) {
        $scope.SelectedKeyword = response.UnSelected
        $scope.CurSelectedKeyword = response.Selected;
        $scope.modelCallBack(groupId, 2, Name, InfriLawCode, Weight, ParentName, ParentId);
      }
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });

  };

  //重搜
  $scope.searchCategoryAgain = function (id) {
    $scope.isgroup = true;
    var url = "/api/Keyword/SetCommendKeywordsStatus?categoryId=" + id + "&prjId=" + $rootScope.getProjectId + "&isgroup=" + $scope.isgroup + "&status=" + 0 + "&user_id=" + $rootScope.userID;
    var q = $http.get(url);
    q.success(function (response, status) {
      if (response.IsSuccess) {
        $scope.addAlert('success', "所选分组正在重新搜索...");
        $rootScope.GetBaiduSearchKeyword2();
        $scope.GetAllKeywordCategory();
        $scope.GetBaiduKeyword();
      }
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  }
  //搜所有关键词
  $scope.searchAllKeywordAgain = function (id) {
    $scope.isgroup = false;
    var url = "/api/Keyword/SetCommendKeywordsStatus?categoryId=" + id + "&prjId=" + $rootScope.getProjectId + "&isgroup=" + $scope.isgroup + "&status=" + 0 + "&user_id=" + $rootScope.userID;
    var q = $http.get(url);
    q.success(function (response, status) {
      if (response.IsSuccess) {
        $scope.addAlert('success', "所有关键词正在重新搜索...");
        $rootScope.GetBaiduSearchKeyword2();
        $scope.GetAllKeywordCategory();
        $scope.GetBaiduKeyword();
      }
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  }

  //
  $scope.modal_demo = function () {
    var kw_scope = $rootScope.$new();
    var frm = $modal.open({
      templateUrl: 'Scripts/app/views/modal/addElementGroup.html',
      controller: addElementGroup_ctr,
      scope: kw_scope,
      // label: label,
      keyboard: false,
      backdrop: 'static',
      size: 'lg'
    });
    frm.result.then(function (response, status) {
    });
  };
  //
  $scope.inputKeyword_OT = function (id) {
    var CP_scope = $rootScope.$new();
    CP_scope.projectId = id;
    var frm = $modal.open({
      templateUrl: 'Scripts/app/views/modal/inputKeywords.html',
      controller: inputKeywords_ctr,
      scope: CP_scope,
      // label: label,
      keyboard: false,
      backdrop: 'static',
      size: 'cd'
    });
    frm.result.then(function (response, status) {
      $rootScope.GetBaiduSearchKeyword2();
    });
  };

  //获取直搜第一行
  //$scope.GetBaiduSearchKeywordLenght = 0;
  $rootScope.GetBaiduSearchKeyword = [];
  $scope.groupType = 1;
  $scope.keywordId = null;
  $rootScope.GetBaiduSearchKeyword2 = function () {
    var url = "/api/Keyword/GetKeywordCategory?user_id=" + $rootScope.userID + "&groupType=" + $scope.groupType + "&projectId=" + $rootScope.getProjectId;
    if ($scope.keywordId != null) {
      url += "&keywordId=" + $scope.keywordId;
    }
    else {
      url += "&keywordId=";
    }
    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('keywordMng_ctr>GetBaiduSearchkeyword2');
      $rootScope.GetBaiduKeywordCategory2 = response;
      console.log(response);
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  };

  //关键词词组点击事件
  $scope.keywordMngClick = function (id) {
    $rootScope.keywordMngClicked = id;
    $cookieStore.put("keywordMngClicked", $rootScope.keywordMngClicked);


  }
  $scope.updateCategoryName = function (groupId) {
    var name = $("#" + groupId).text();
    var url = "/api/Keyword/UpdateKeywordGroupName?groupid=" + groupId + "&groupName=" + encodeURIComponent(name) + "&user_id=" + $rootScope.userID;

    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('keywordMng_ctr>updateCategoryName');

    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  }

  $scope.DelCategory = function (categoryId) {
    if (confirm("确定要删除这个组吗？")) {
      var url = "/api/Keyword/DelKeywordCategory?categoryId=" + categoryId + "&user_id=" + $rootScope.userID;

      var q = $http.get(url);
      q.success(function (response, status) {
        console.log('keywordMng_ctr>DelCategory');
        $scope.RefreshList();
      });
      q.error(function (response) {
        $scope.error = "网络打盹了，请稍后。。。";
      });
    }
  };
  //operate:1,新增；2，修改
  $scope.modelCallBack = function (groupId, operate, Name, InfriLawCode, Weight, ParentName, ParentId) {
    var kw_scope = $rootScope.$new();
    kw_scope.GetBaiduSearchKeyword = $scope.SelectedKeyword;
    kw_scope.SelectedKeyword = $scope.CurSelectedKeyword;
    kw_scope.ParentName = ParentName;
    kw_scope.ParentId = ParentId;
    kw_scope.parentgroupId = groupId;
    kw_scope.operate = operate;
    kw_scope.InfriTypes = $scope.InfriTypes;
    kw_scope.groupName = Name;
    kw_scope.InfriType = InfriLawCode;
    kw_scope.weight = Weight;
    kw_scope.InfriTypeschange = $scope.InfriTypeschange;
    kw_scope.Weight1 = $scope.Weight1
    kw_scope.InfriLawCodeStr1 = $scope.InfriLawCodeStr1
    var frm = $modal.open({
      templateUrl: 'Scripts/app/views/modal/addElementGroup.html',
      controller: addElementGroup_ctr,
      scope: kw_scope,
      // label: label,
      keyboard: false,
      backdrop: 'static',
      size: 'lg'
    });
    frm.result.then(function (data) {
      $rootScope.GetBaiduSearchKeyword2();
    });
  };

  $scope.RefreshList = function () {
    var url = "/api/Keyword/GetKeywordCategory?user_id=" + $rootScope.userID + "&groupType=" + $scope.groupType + "&projectId=" + $rootScope.getProjectId;
    if ($scope.keywordId != null) {
      url += "&keywordId=" + $scope.keywordId;
    }
    else {
      url += "&keywordId=";
    }
    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('keywordMng_ctr>RefreshList');
      $rootScope.GetBaiduKeywordCategory2 = response;

    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  };

  //矩阵图
  $scope.juzhentu = function () {
    var url = "/api/Keyword/GetRectangularTreeUrl?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
    var q = $http.get(url);
    q.success(function (response, status) {
        if (response.IsSuccess) {
            $scope.ecahrtsJZ(response.Json);
        } else {
            $scope.alert_fun(response.Message);
        }
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  }
  $scope.ecahrtsJZ = function (jsonStr) {
    var myChart = echarts.init(document.getElementById('charts111'));
    //var jsonUrl = "Scripts/app/data/echarts.txt";
    myChart.showLoading();

    rawData = eval('(' + jsonStr + ')');
    //$.getJSON(jsonUrl, function (rawData) {

      myChart.hideLoading();

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
          text: '矩阵图demo',
          subtext: '2016/08',
          left: 'leafDepth'
        },
        tooltip: {},
        series: [{
          name: '根目录',
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
    //});
  }

  //圆形d3图
  $scope.GetD3TreeData = function () {
    var url = "/api/Keyword/GetAllGroupTreeUrl?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
    var q = $http.get(url);
    q.success(function (response, status) {
      //console.log(response);
      //$scope.getJson(JSON.stringify(response));
        if (response.IsSuccess) {
            $scope.getJson(response.Json);
        } else {
            $scope.alert_fun(response.Message);
        }
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  }

  $scope.getJson = function (jsonStr) {
    //Radial Reingold–Tilford Tree_______________________
    var diameter = 700;
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

    root = eval('(' + jsonStr + ')');
    //d3.json(jsonUrl, function (error, root) {
    //  if (error) throw error;

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
    //});
  }

  //关键词对比图

  $scope.GetData = function () {
    var url = "/api/Keyword/GetKeywordBygroup?user_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('iw2s_dashboard_ctr>GetData');
      if (response.IsSuccess) {
          $scope.getjson(response.Json);
      } else {
          $scope.alert_fun(response.Message);
      }
    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  }

  $scope.getjson = function (jsonStr) {

    var margin = {top: 150, right: 0, bottom: 10, left: 150},
      width = 750,
      height = 750;

    var x = d3.scale.ordinal().rangeBands([0, width]),
      z = d3.scale.linear().domain([0, 4]).clamp(true),
      c = d3.scale.category10().domain(d3.range(10));

    var svg1 = d3.select("#bd").append("svg")
      .attr("width", width)
      .attr("height", height)
      //.style("margin-left", margin.left + "px")
      .append("g")
      .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    miserables = eval('(' + jsonStr + ')');
    //d3.json(url, function (miserables) {
      var matrix = [],
        nodes = miserables[0].nodes,
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
      miserables[0].links.forEach(function (link) {
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
      }, 2000);
    //});
  }

  //树状图
  $scope.d3tree = function (treeData) {

    // Calculate total nodes, max label length
    var totalNodes = 0;
    var maxLabelLength = 0;
    // variables for drag/drop
    var selectedNode = null;
    var draggingNode = null;
    // panning variables
    var panSpeed = 200;
    var panBoundary = 20; // Within 20px from edges will pan when dragging.
    // Misc. variables
    var i = 0;
    var duration = 750;
    var root;

    // size of the diagram
    var viewerWidth = ($('#treeBlock').width()) - 5;
    var viewerHeight = ($('#treeBlock').height()) - 5;
    //console.log(viewerWidth);
    //console.log(viewerWidth);

    var tree = d3.layout.tree()
      .size([viewerHeight, viewerWidth]);

    // define a d3 diagonal projection for use by the node paths later on.
    var diagonal = d3.svg.diagonal()
      .projection(function (d) {
        return [d.y, d.x];
      });

    // A recursive helper function for performing some setup by walking through all nodes

    function visit(parent, visitFn, childrenFn) {
      if (!parent) return;

      visitFn(parent);

      var children = childrenFn(parent);
      if (children) {
        var count = children.length;
        for (var i = 0; i < count; i++) {
          visit(children[i], visitFn, childrenFn);
        }
      }
    }

    // Call visit function to establish maxLabelLength
    visit(treeData, function (d) {
      totalNodes++;
      maxLabelLength = Math.max(d.name.length, maxLabelLength);

    }, function (d) {
      return d.children && d.children.length > 0 ? d.children : null;
    });


    // sort the tree according to the node names

    function sortTree() {
      tree.sort(function (a, b) {
        return b.name.toLowerCase() < a.name.toLowerCase() ? 1 : -1;
      });
    }

    // Sort the tree initially incase the JSON isn't in a sorted order.
    sortTree();

    // TODO: Pan function, can be better implemented.

    function pan(domNode, direction) {
      var speed = panSpeed;
      if (panTimer) {
        clearTimeout(panTimer);
        translateCoords = d3.transform(svgGroup.attr("transform"));
        if (direction == 'left' || direction == 'right') {
          translateX = direction == 'left' ? translateCoords.translate[0] + speed : translateCoords.translate[0] - speed;
          translateY = translateCoords.translate[1];
        } else if (direction == 'up' || direction == 'down') {
          translateX = translateCoords.translate[0];
          translateY = direction == 'up' ? translateCoords.translate[1] + speed : translateCoords.translate[1] - speed;
        }
        scaleX = translateCoords.scale[0];
        scaleY = translateCoords.scale[1];
        scale = zoomListener.scale();
        svgGroup.transition().attr("transform", "translate(" + translateX + "," + translateY + ")scale(" + scale + ")");
        d3.select(domNode).select('g.node').attr("transform", "translate(" + translateX + "," + translateY + ")");
        zoomListener.scale(zoomListener.scale());
        zoomListener.translate([translateX, translateY]);
        panTimer = setTimeout(function () {
          pan(domNode, speed, direction);
        }, 50);
      }
    }

    // Define the zoom function for the zoomable tree

    function zoom() {
      svgGroup.attr("transform", "translate(" + d3.event.translate + ")scale(" + d3.event.scale + ")");
    }


    // define the zoomListener which calls the zoom function on the "zoom" event constrained within the scaleExtents
    var zoomListener = d3.behavior.zoom().scaleExtent([0.1, 3]).on("zoom", zoom);

    function initiateDrag(d, domNode) {
      draggingNode = d;
      d3.select(domNode).select('.ghostCircle').attr('pointer-events', 'none');
      d3.selectAll('.ghostCircle').attr('class', 'ghostCircle show');
      d3.select(domNode).attr('class', 'node activeDrag');

      svgGroup.selectAll("g.node").sort(function (a, b) { // select the parent and sort the path's
        if (a.id != draggingNode.id) return 1; // a is not the hovered element, send "a" to the back
        else return -1; // a is the hovered element, bring "a" to the front
      });
      // if nodes has children, remove the links and nodes
      if (nodes.length > 1) {
        // remove link paths
        links = tree.links(nodes);
        nodePaths = svgGroup.selectAll("path.link")
          .data(links, function (d) {
            return d.target.id;
          }).remove();
        // remove child nodes
        nodesExit = svgGroup.selectAll("g.node")
          .data(nodes, function (d) {
            return d.id;
          }).filter(function (d, i) {
            if (d.id == draggingNode.id) {
              return false;
            }
            return true;
          }).remove();
      }

      // remove parent link
      parentLink = tree.links(tree.nodes(draggingNode.parent));
      svgGroup.selectAll('path.link').filter(function (d, i) {
        if (d.target.id == draggingNode.id) {
          return true;
        }
        return false;
      }).remove();

      dragStarted = null;
    }

    // define the baseSvg, attaching a class for styling and the zoomListener
    var baseSvg = d3.select("#tree-container").append("svg")
      .attr("width", viewerWidth)
      .attr("height", viewerHeight)
      .attr("class", "overlay")
      .call(zoomListener);


    // Define the drag listeners for drag/drop behaviour of nodes.
    dragListener = d3.behavior.drag()
      .on("dragstart", function (d) {
        if (d == root) {
          return;
        }
        dragStarted = true;
        nodes = tree.nodes(d);
        d3.event.sourceEvent.stopPropagation();
        // it's important that we suppress the mouseover event on the node being dragged. Otherwise it will absorb the mouseover event and the underlying node will not detect it d3.select(this).attr('pointer-events', 'none');
      })
      .on("drag", function (d) {
        if (d == root) {
          return;
        }
        if (dragStarted) {
          domNode = this;
          initiateDrag(d, domNode);
        }

        // get coords of mouseEvent relative to svg container to allow for panning
        relCoords = d3.mouse($('svg').get(0));
        if (relCoords[0] < panBoundary) {
          panTimer = true;
          pan(this, 'left');
        } else if (relCoords[0] > ($('svg').width() - panBoundary)) {

          panTimer = true;
          pan(this, 'right');
        } else if (relCoords[1] < panBoundary) {
          panTimer = true;
          pan(this, 'up');
        } else if (relCoords[1] > ($('svg').height() - panBoundary)) {
          panTimer = true;
          pan(this, 'down');
        } else {
          try {
            clearTimeout(panTimer);
          } catch (e) {

          }
        }

        d.x0 += d3.event.dy;
        d.y0 += d3.event.dx;
        var node = d3.select(this);
        node.attr("transform", "translate(" + d.y0 + "," + d.x0 + ")");
        updateTempConnector();
      }).on("dragend", function (d) {
        if (d == root) {
          return;
        }
        domNode = this;
        if (selectedNode) {
          // now remove the element from the parent, and insert it into the new elements children
          var index = draggingNode.parent.children.indexOf(draggingNode);
          if (index > -1) {
            draggingNode.parent.children.splice(index, 1);
          }
          if (typeof selectedNode.children !== 'undefined' || typeof selectedNode._children !== 'undefined') {
            if (typeof selectedNode.children !== 'undefined') {
              selectedNode.children.push(draggingNode);
            } else {
              selectedNode._children.push(draggingNode);
            }
          } else {
            selectedNode.children = [];
            selectedNode.children.push(draggingNode);
          }
          // Make sure that the node being added to is expanded so user can see added node is correctly moved
          expand(selectedNode);
          sortTree();
          endDrag();
        } else {
          endDrag();
        }
      });

    function endDrag() {
      selectedNode = null;
      d3.selectAll('.ghostCircle').attr('class', 'ghostCircle');
      d3.select(domNode).attr('class', 'node');
      // now restore the mouseover event or we won't be able to drag a 2nd time
      d3.select(domNode).select('.ghostCircle').attr('pointer-events', '');
      updateTempConnector();
      if (draggingNode !== null) {
        update(root);
        centerNode(draggingNode);
        draggingNode = null;
      }
    }

    // Helper functions for collapsing and expanding nodes.

    function collapse(d) {
      if (d.children) {
        d._children = d.children;
        d._children.forEach(collapse);
        d.children = null;
      }
    }

    function expand(d) {
      if (d._children) {
        d.children = d._children;
        d.children.forEach(expand);
        d._children = null;
      }
    }

    var overCircle = function (d) {
      selectedNode = d;
      updateTempConnector();
    };
    var outCircle = function (d) {
      selectedNode = null;
      updateTempConnector();
    };

    // Function to update the temporary connector indicating dragging affiliation
    var updateTempConnector = function () {
      var data = [];
      if (draggingNode !== null && selectedNode !== null) {
        // have to flip the source coordinates since we did this for the existing connectors on the original tree
        data = [{
          source: {
            x: selectedNode.y0,
            y: selectedNode.x0
          },
          target: {
            x: draggingNode.y0,
            y: draggingNode.x0
          }
        }];
      }
      var link = svgGroup.selectAll(".templink").data(data);

      link.enter().append("path")
        .attr("class", "templink")
        .attr("d", d3.svg.diagonal())
        .attr('pointer-events', 'none');

      link.attr("d", d3.svg.diagonal());

      link.exit().remove();
    };

    // Function to center node when clicked/dropped so node doesn't get lost when collapsing/moving with large amount of children.

    function centerNode(source) {
      scale = zoomListener.scale();
      x = -source.y0;
      y = -source.x0;
      x = x * scale + viewerWidth / 4;
      y = y * scale + viewerHeight / 4;
      d3.select('#tree-container g').transition()
        .duration(duration)
        .attr("transform", "translate(" + x + "," + y + ")scale(" + scale + ")");
      zoomListener.scale(scale);
      zoomListener.translate([x, y]);
    }

    // Toggle children function

    function toggleChildren(d) {
      if (d.children) {
        d._children = d.children;
        d.children = null;
      } else if (d._children) {
        d.children = d._children;
        d._children = null;
      }
      return d;
    }

    // Toggle children on click.

    function click(d) {
      if (d3.event.defaultPrevented) return; // click suppressed
      d = toggleChildren(d);
      update(d);
      centerNode(d);
    }

    function update(source) {
      // Compute the new height, function counts total children of root node and sets tree height accordingly.
      // This prevents the layout looking squashed when new nodes are made visible or looking sparse when nodes are removed
      // This makes the layout more consistent.
      var levelWidth = [1];
      var childCount = function (level, n) {

        if (n.children && n.children.length > 0) {
          if (levelWidth.length <= level + 1) levelWidth.push(0);

          levelWidth[level + 1] += n.children.length;
          n.children.forEach(function (d) {
            childCount(level + 1, d);
          });
        }
      };
      childCount(0, root);
      var newHeight = d3.max(levelWidth) * 25; // 25 pixels per line
      tree = tree.size([newHeight, viewerWidth]);

      // Compute the new tree layout.
      var nodes = tree.nodes(root).reverse(),
        links = tree.links(nodes);

      // Set widths between levels based on maxLabelLength.
      nodes.forEach(function (d) {
        d.y = (d.depth * (maxLabelLength * 10)); //maxLabelLength * 10px
        // alternatively to keep a fixed scale one can set a fixed depth per level
        // Normalize for fixed-depth by commenting out below line
        // d.y = (d.depth * 500); //500px per level.
      });

      // Update the nodes…
      node = svgGroup.selectAll("g.node")
        .data(nodes, function (d) {
          return d.id || (d.id = ++i);
        });

      // Enter any new nodes at the parent's previous position.
      var nodeEnter = node.enter().append("g")
        .call(dragListener)
        .attr("class", "node")
        .attr("transform", function (d) {
          return "translate(" + source.y0 + "," + source.x0 + ")";
        })
        .on('click', click);

      nodeEnter.append("circle")
        .attr('class', 'nodeCircle')
        .attr("r", 0)
        .style("fill", function (d) {
          return d._children ? "lightsteelblue" : "#fff";
        });

      nodeEnter.append("text")
        .attr("x", function (d) {
          return d.children || d._children ? -10 : 10;
        })
        .attr("dy", ".35em")
        .attr('class', 'nodeText')
        .attr("text-anchor", function (d) {
          return d.children || d._children ? "end" : "start";
        })
        .text(function (d) {
          return d.name;
        })
        .style("fill-opacity", 0);

      // phantom node to give us mouseover in a radius around it
      nodeEnter.append("circle")
        .attr('class', 'ghostCircle')
        .attr("r", 30)
        .attr("opacity", 0.2) // change this to zero to hide the target area
        .style("fill", "#00b5ad")
        .attr('pointer-events', 'mouseover')
        .on("mouseover", function (node) {
          overCircle(node);
        })
        .on("mouseout", function (node) {
          outCircle(node);
        });

      // Update the text to reflect whether node has children or not.
      node.select('text')
        .attr("x", function (d) {
          return d.children || d._children ? -10 : 10;
        })
        .attr("text-anchor", function (d) {
          return d.children || d._children ? "end" : "start";
        })
        .text(function (d) {
          return d.name;
        });

      // Change the circle fill depending on whether it has children and is collapsed
      node.select("circle.nodeCircle")
        .attr("r", 4.5)
        .style("fill", function (d) {
          return d._children ? "lightsteelblue" : "#fff";
        });

      // Transition nodes to their new position.
      var nodeUpdate = node.transition()
        .duration(duration)
        .attr("transform", function (d) {
          return "translate(" + d.y + "," + d.x + ")";
        });

      // Fade the text in
      nodeUpdate.select("text")
        .style("fill-opacity", 1);

      // Transition exiting nodes to the parent's new position.
      var nodeExit = node.exit().transition()
        .duration(duration)
        .attr("transform", function (d) {
          return "translate(" + source.y + "," + source.x + ")";
        })
        .remove();

      nodeExit.select("circle")
        .attr("r", 0);

      nodeExit.select("text")
        .style("fill-opacity", 0);

      // Update the links…
      var link = svgGroup.selectAll("path.link")
        .data(links, function (d) {
          return d.target.id;
        });

      // Enter any new links at the parent's previous position.
      link.enter().insert("path", "g")
        .attr("class", "link")
        .attr("d", function (d) {
          var o = {
            x: source.x0,
            y: source.y0
          };
          return diagonal({
            source: o,
            target: o
          });
        });

      // Transition links to their new position.
      link.transition()
        .duration(duration)
        .attr("d", diagonal);

      // Transition exiting nodes to the parent's new position.
      link.exit().transition()
        .duration(duration)
        .attr("d", function (d) {
          var o = {
            x: source.x,
            y: source.y
          };
          return diagonal({
            source: o,
            target: o
          });
        })
        .remove();

      // Stash the old positions for transition.
      nodes.forEach(function (d) {
        d.x0 = d.x;
        d.y0 = d.y;
      });
    }

    // Append a group which holds all nodes and which the zoom Listener can act upon.
    var svgGroup = baseSvg.append("g");

    // Define the root
    root = treeData;
    root.x0 = viewerHeight / 2;
    root.y0 = 0;

    // Collapse all children of roots children before rendering.
    root.children.forEach(function (child) {
      collapse(child);
    });

    // Layout the tree initially and center on the root node.
    update(root);
    centerNode(root);

  };

  $scope.GetTreeData = function () {
    var url = "/api/Keyword/GetAllGroupTree?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
    var q = $http.get(url);
    q.success(function (response, status) {
      console.log('keywordMng_ctr>GetTreeData');
      $scope.d3tree(response);
      console.log(response);

    });
    q.error(function (response) {
      $scope.error = "网络打盹了，请稍后。。。";
    });
  }

   
    //侧栏Top显示
  $scope.CListShow = true;
  $scope.CListShowFun = function () {
      $scope.CListShow = !$scope.CListShow;
      $scope.GetECGuanxi();
  }
   //echart关系图切换显示
  $scope.GXtimeIntervalFun = function (num) {
      $scope.GXtimeInterval = num;
      $scope.GetECGuanxi();
  }
    //切换关系图类型
  
  $scope.WebRelationsFun = function () {
      $scope.WebRelations = !$scope.WebRelations;
      $scope.GetECGuanxi();
  }

  //echart关系图

  $scope.GetECGuanxi = function () {
      if ($scope.WebRelations) {
          var titlelei = '报道关系现状'
          var url = "/api/Keyword/GetLinkReference?prjId=" + $rootScope.getProjectId + "&timeInterval=" + $scope.GXtimeInterval;
      } else {
          var titlelei = '网站关系现状';
          var url = "/api/Keyword/GetDomainReference?prjId=" + $rootScope.getProjectId + "&timeInterval=" + $scope.GXtimeInterval;
      }
      var url = "/api/Keyword/GetLinkReference?prjId=" + $rootScope.getProjectId + "&timeInterval=" + $scope.GXtimeInterval;
      var q = $http.get(url);
      q.success(function (response, status) {
          console.log('keywordMng_ctr>GetTreeData');
          console.log(response);
         
          var timeData = response.DateTimeList;
          for (var i = 0; i < timeData.length; i++) {
              if ($scope.GXtimeInterval==0) {
                  timeData[i] = '所有'
              } else if ($scope.GXtimeInterval == 3) {
                  timeData[i] = $filter('date')(timeData[i], 'yyyy')
              } else {
                  timeData[i] = $filter('date')(timeData[i], 'yyyy-MM')
              }
             
          }
       
          $scope.ReferList = response.ReferList;
          for (var i = 0; i < $scope.ReferList.length; i++) {
              $scope.ReferList[i].Json = eval('(' + $scope.ReferList[i].Json + ')');
          }
          var Funcategories = $scope.ReferList[0].Json;
          var datalegend=[];
          for (var i = 0; i < Funcategories.categories.length; i++) {
              datalegend[i] = Funcategories.categories[i].base;
          }
          var data = {};
          data.counties = datalegend;
          data.timeline = timeData;
          data.series = $scope.ReferList;
          if ($scope.WebRelations) {
              var myChart = echarts.init(document.getElementById('GetECGuanxiTime'));
          } else {
              var myChart = echarts.init(document.getElementById('GetECGuanxiTime2'));
          }
          option = {
    
                baseOption: {
                    timeline: {
                        axisType: 'category',
                        orient: 'horizontal',
                        autoPlay: false,
                        left: 20,
                        right: 20,
                        bottom:0,
                        playInterval: 4000,
                        currentIndex:0,
                        label: {
                            normal: {
                                textStyle: {
                                    color: '#999'
                                }
                            },
                            emphasis: {
                                textStyle: {
                                    color: '#555'
                                }
                            }
                        },
                        symbol: 'none',
                        checkpointStyle: {
                            color: '#bbb',
                            borderColor: '#777',
                            borderWidth: 2
                        },
                        controlStyle: {
                            normal: {
                                color: '#666',
                                borderColor: '#666'
                            },
                            emphasis: {
                                color: '#aaa',
                                borderColor: '#aaa'
                            }
                        },
                        data: []
                      },
                      grid: {
                          top: '12%',
                          bottom: '110'
                      },
                      legend: {
                          bottom: 55,
                          data: data.counties,
                          backgroundColor: 'rgba(171, 171, 171, 0.35)',
                      },
                      toolbox: {
                          show: true,
                          feature: {
                              restore: { show: true },
                              saveAsImage: { show: true }
                          }
                      },
                      //brush: {
                      //    outOfBrush: {
                      //        color: '#abc'
                      //    },
                      //    brushStyle: {
                      //        borderWidth: 2,
                      //        color: 'rgba(0,0,0,0.2)',
                      //        borderColor: 'rgba(0,0,0,0.5)',
                      //    },
                      //    seriesIndex: [0, 1],
                      //    throttleType: 'debounce',
                      //    throttleDelay: 300,
                      //    geoIndex: 0
                      //},
                 
                      tooltip: {
                            padding: 10,
                            trigger: 'item',
                            formatter: function (obj) {
                                var value = obj.data;
                                var ci = '';
                                if (value.keyWordList) {
                                    for (var i = 0; i < value.keyWordList.length; i++) {
                                        ci = ci + value.keyWordList[i]+'、'
                                    }
                                    return '<div style="border-bottom: 1px solid rgba(255,255,255,.3); font-size: 16px;padding-bottom: 7px;margin-bottom: 7px;max-width:320px">'
                                    + value.name
                                    + '</div>'
                                    + '<span style="font-wight:600">命中词数：</span>' + value.keyWordCount + '<br>'
                                    + '<p style="white-space:normal;max-width:320px">'
                                    + '<span style="font-wight:600">命中词：</span>' + ci
                                    + '</p>'   
                                    + '<p style="white-space:normal;max-width:320px; text-align: justify;margin-top: -13px;">'
                                    + '<span style="font-wight:600">文章摘要：</span>' + value.base
                                    + '</p>'
                                }
                            }
                        },
                        toolbox: {
                            feature: {
                                dataView: { readOnly: false },
                                restore: {},
                                saveAsImage: {}
                            }
                        },
                        ceData: data.series[0],
                        ceDataIndex: 0,
                      series: [
                          {
                              type: 'graph',
                              layout: 'force',
                              animation: false,
                              label: {
                                  normal: {
                                      position: 'right',
                                      formatter: '{b}'
                                  }
                              },
                              roam: true,//是否滚动鼠标缩放
                              draggable: false,//节点是否可拖动
                              //progressive: true,//渐进渲染
                              bounding: 'all',
                              animation :false,
                              data: data.series[0].Json.nodes.map(function (node, idx) {
                                  node.id = idx;
                                  node.symbolSize = node.keyWordCount * 2 + 5;

                                  return node;
                              }),
                              categories: data.series[0].Json.categories,
                              force: {

                                  edgeLength: 5,
                                  repulsion: 20,
                                  gravity: 0.2
                              },
                              edges: data.series[0].Json.links
                          }
                      ],
                      animationDurationUpdate:500,
                      animationEasingUpdate: 'quinticInOut'
                  },
                  options: []
              };
              for (var n = 0; n < data.timeline.length; n++) {
                  option.baseOption.timeline.data.push(data.timeline[n]);
                  option.options.push({
                      ceData: data.series[n],
                      ceDataIndex: n,
                      title: {
                          show: true,
                          'text': data.timeline[n] + titlelei,
                          left: 'center',
                      },
                      series: {
                        type: 'graph',
                        layout: 'force',
                        animation: false,
                        label: {
                            normal: {
                                position: 'right',
                                formatter: '{b}'
                            }
                        },
                        roam: true,//是否滚动鼠标缩放
                        draggable: false,//节点是否可拖动
                        data: data.series[n].Json.nodes.map(function (node, idx) {
                            node.id = idx;
                            node.symbolSize = node.keyWordCount * 2+5;

                            return node;
                        }),
                        categories: data.series[n].Json.categories,
                        force: {
                            // initLayout: 'circular'
                            // repulsion: 20,
                            edgeLength: 5,
                            repulsion: 20,
                            gravity: 0.2
                        },
                        edges: data.series[n].Json.links
                      }
                  });
              }
              myChart.setOption(option);


              var modal = myChart.getOption();
              var timeLineIdexNum = modal.ceDataIndex;
              var timeLineIdexData = modal.ceData;
              $scope.ariticalsNum = timeLineIdexData.Json.nodes.length;
              $scope.ariticalslinksNum = timeLineIdexData.Json.links.length;
              $scope.TopArtices = timeLineIdexData.TopData;
              $scope.CateWeights = timeLineIdexData.CateWeights;
              $scope.TopCateArtices = timeLineIdexData.TopCateData;//关联数最对
              $scope.TopKeyArtices = timeLineIdexData.TopKeyData;//命中关键词数排行

              $scope.GaiBianData = function () {
                  var modal = myChart.getOption();
                  if (timeLineIdexNum != modal.ceDataIndex) {
                      timeLineIdexNum = modal.ceDataIndex
                      timeLineIdexData = modal.ceData;
                      $scope.ariticalsNum = timeLineIdexData.Json.nodes.length;
                      $scope.ariticalslinksNum = timeLineIdexData.Json.links.length;
                      $scope.TopArtices = timeLineIdexData.TopData;
                      $scope.CateWeights = timeLineIdexData.CateWeights;
                      
                      $scope.TopCateArtices = timeLineIdexData.TopCateData;
                      $scope.TopKeyArtices = timeLineIdexData.TopKeyData;
                  }
                  $timeout(function () {
                      $scope.$apply(
                           $scope.GaiBianData()
                          )}, 100)
              }
              if($scope.GXtimeInterval!=0){
                  $scope.GaiBianData();
              }
   
      });
      q.error(function (response) {
          $scope.error = "网络打盹了，请稍后。。。";
      });
  }

  //切换最具影响力TOP10
  $scope.TopListShowFun = function (num) {
      $scope.TopListShow = num;

  }


    //命名实体抽取+++++++++++++++++++++++++++++++++=
    //添加命名实体
  $scope.AddMMSTShow = 0;
  $scope.AddMMSTFun = function (num) {
      $scope.AddMMSTShow = num;
      if (num == 0) {
          if ($scope.TextExtractByEntityList.length > 0) {
              $scope.AddMMSTShow = 3;
          }
      }else if (num==1) {
          $scope.GetEntity()
      }
    
  }
    //添加命名实体
  var AddedList=[];
  $scope.AddedList = [];
  $scope.AddMMSTListFun = function (data) {
      if(AddedList.length==0){
          AddedList.push(data);
          $scope.AddedList = AddedList
          return
      }
      var shouldAdd = true;
      for (var i = 0; i < $scope.GetEntityMappingList.length; i++) {
          if (data.Entity.Key == $scope.GetEntityMappingList[i].EntityName) {
              var shouldAdd = false;
          }
      }
      for (var i = 0; i < AddedList.length; i++) {
          if (data.Id == AddedList[i].Id) {
              var shouldAdd = false;
              AddedList.splice(i, 1)
          }
      }
      if (shouldAdd) {
          AddedList.push(data);
      } 
      $scope.AddedList = AddedList;
  }
  $scope.InsertEntityMapping = function () {
      $scope.AddedListIds = '';
     
      if ($scope.AddedList.length < 1) {
          if ($scope.GetEntityMappingList.length < 1) {
              $scope.alert_fun('info', '请选择命名实体');
          } else {
              $scope.TextExtractByEntity();
          }
          return
      }
      for (var c = 0; c < $scope.AddedList.length; c++) {
          $scope.AddedListIds = $scope.AddedListIds + $scope.AddedList[c].Id + ';';
      }
      $scope.paramsList = {
          userId: $rootScope.userID,
          entityId: $scope.AddedListIds,
          projectId: $rootScope.getProjectId,
      };
      $http({
          method: 'get',
          params: $scope.paramsList,
          url: "/api/Keyword/InsertEntityMapping"
      })
        .success(function (response, status) {
            if (response.IsSuccess) {
                $rootScope.alert_fun('success', '命名实体映射创建成功！');
                $scope.AddMMSTShow = 4;
                $scope.ExtractionFun();
            } else {
                $rootScope.alert_fun('warning', response.Message)
            }
        })
        .error(function (response, status, headers, config) {
            $rootScope.alert_fun('warning', '数据库连接失败，请稍后再试')
        });
  }

    //话语提取进度
  $scope.baifenbi_x = 0.0;
  $scope.ExtractionFun = function () {
      if ($scope.baifenbi_x >= 100) {
          $scope.TextExtractByEntity(),
          $timeout(function () {
              $scope.$apply(
              $scope.AddMMSTShow = 3,
              $scope.baifenbi_x = 0.0
              )
          }, 1000)
        
      } else {
          $scope.baifenbi_x = $scope.baifenbi_x + 0.4;
          $scope.baifenbi_x_2=$scope.baifenbi_x+'%';
          $scope.myObj = {
              'width': $scope.baifenbi_x_2
              };
          $timeout(function () {
              $scope.$apply($scope.ExtractionFun())
          }, 40)
      }
  }
    //获取话语提取数据

  $scope.TextExtractByEntity = function () {
      $scope.paramsList = {
          userId: $rootScope.userID,
          projectId: $rootScope.getProjectId,
      };
      $http({
          method: 'get',
          params: $scope.paramsList,
          url: "/api/Keyword/TextExtractByEntity"
      })
        .success(function (response, status) {
            console.log(response);
            $scope.TextExtractByEntityList = response;
            if ($scope.TextExtractByEntityList.length>0) {
                $scope.AddMMSTShow = 3;
            } else {
                $scope.AddMMSTShow = 5;
            }
        })
        .error(function (response, status, headers, config) {
            $rootScope.alert_fun('worning', '数据库连接失败，请稍后再试')
        });
  }
    //修改命名实体
  $scope.changeEntity = function () {
      $scope.AddMMSTFun(1);
      $scope.GetEntityMapping();
  }
    //获取已添加命名实体
  

  $scope.GetEntityMapping = function () {
      $scope.paramsList = {
          projectId: $rootScope.getProjectId,
      };
      $http({
          method: 'get',
          params: $scope.paramsList,
          url: "/api/Keyword/GetEntityMapping"
      })
        .success(function (response, status) {
            console.log(response);
            $scope.GetEntityMappingList = response;
            
        })
        .error(function (response, status, headers, config) {
            $rootScope.alert_fun('worning', '数据库连接失败，请稍后再试')
        });
  }
    //删除已添加命名实体

  $scope.DelEntityMapping = function (id) {
      $scope.paramsList = {
          mappingId: id
      };
      $http({
          method: 'get',
          params: $scope.paramsList,
          url: "/api/Keyword/DelEntityMapping"
      })
        .success(function (response, status) {
            if (response.IsSuccess) {
                $scope.GetEntityMapping();
            } else {
                $rootScope.alert_fun('worning', response.Message)
            }
        })
        .error(function (response, status, headers, config) {
            $rootScope.alert_fun('worning', '数据库连接失败，请稍后再试')
        });
  }

  //导出关键词词组
  $scope.ExportKeywordGroup = function () {

    $scope.paramsList = {
      user_id: $rootScope.userID,
      projectId: $rootScope.getProjectId,
    };
    $http({
      method: 'get',
      params: $scope.paramsList,
      url: "/api/Keyword/ExportKeywordGroup"
    })
      .success(function (response, status) {
        if (response != null) {
          if (response != "没有要导出的数据") {
            window.location.href = "Export/DownLoadExcel?path=" + response;
            $rootScope.addAlert('success', "关键词词组导出成功！");
          } else {
            $rootScope.addAlert('danger', "没有要导出关键词词组！");
          }
        }
      })
      .error(function (response, status, headers, config) {
        publicFunc.showAlert("温馨提示", "连接接服务器出错");
      });
  }
  //分析指项
  $scope.chang_AnalysisZ1 = function () {
    $scope.isActive_AnalysisZ1 = true;
    $scope.isActive_AnalysisZ2 = false;
    $scope.isActive_AnalysisZ3 = false;
    $scope.isactriveChgSelIt = true;
  }
  $scope.chang_AnalysisZ2 = function () {
    $scope.isActive_AnalysisZ1 = false;
    $scope.isActive_AnalysisZ2 = true;
    $scope.isActive_AnalysisZ3 = false;
    $scope.isactriveChgSelIt = true;
  }
  $scope.chang_AnalysisZ3 = function () {
    $scope.isActive_AnalysisZ1 = false;
    $scope.isActive_AnalysisZ2 = false;
    $scope.isActive_AnalysisZ3 = true;
  }
  //获取分析指项
  $scope.GetAnalysisItem = function () {
    $scope.paramsList = {
      prj_id: $rootScope.getProjectId,
    };
    $http({
      method: 'get',
      params: $scope.paramsList,
      url: "/api/Keyword/GetAnalysisItem"
    })
      .success(function (response, status) {
        $scope.Analysis_list = response;
        console.log("  1 ")
        console.log(response);
      })
      .error(function (response, status) {
        alert("温馨提示:连接接服务器出错");
      });


  }

  //添加分析指项

  $scope.ItemValues = [];
  $scope.ItemValues.push({Name: "", SeqNo: ""});
  $scope.ItemValues.push({Name: "", SeqNo: ""});
  $scope.addAnalysisItem = function () {
    $scope.ItemValues.push({Name: "", SeqNo: ""});
  }
  $scope.InsertAnalysisItem = function () {
    if ($scope.Analysis_name == "" || $scope.Analysis_name == null) {
      $scope.addAlert('danger', "请输入分析指项名称");
      return
    }
    if ($scope.ItemValues[$scope.ItemValues.length - 1].Name == '' || $scope.ItemValues[$scope.ItemValues.length - 1].Name == null || $scope.ItemValues[$scope.ItemValues.length - 1].SeqNo == '' || $scope.ItemValues[$scope.ItemValues.length - 1].SeqNo == null) {
      $scope.addAlert('danger', "请输入完整的“分类指项”类目序号内容");
      return
    }
    $scope.anaItem = {
      UsrId: $rootScope.userID,
      ProjectId: $rootScope.getProjectId,
      Name: $scope.Analysis_name,
      ItemValues: $scope.ItemValues,
      _id: $scope.Analysis_id

    };
    var urls = "api/Keyword/InsertAnalysisItem";
    var q = $http.post(
      urls,
      JSON.stringify($scope.anaItem),
      {
        headers: {
          'Content-Type': 'application/json'
        }
      }
    )
    q.success(function (response, status) {
      if (response.IsSuccess == true) {
        $scope.addAlert('success', response.Message);

        $scope.GetAnalysisItem();
        $scope.chang_AnalysisZ2();


      } else {
        $scope.addAlert('danger', response.Message);
      }
    });
    q.error(function (e) {
      $scope.addAlert('danger', "网络打盹了，请稍后。。。");
    });
  }
  $scope.InsertAnalysisItem2 = function () {
    if ($scope.Analysis_name == "" || $scope.Analysis_name == null) {
      $scope.addAlert('danger', "请输入分析指项名称");
      return
    }
    if ($scope.ItemValues[$scope.ItemValues.length - 1].Name == '' || $scope.ItemValues[$scope.ItemValues.length - 1].Name == null || $scope.ItemValues[$scope.ItemValues.length - 1].SeqNo == '' || $scope.ItemValues[$scope.ItemValues.length - 1].SeqNo == null) {
      $scope.addAlert('danger', "请输入完整的“分类指项”类目序号内容");
      return
    }
    $scope.anaItem = {
      UsrId: $rootScope.userID,
      ProjectId: $rootScope.getProjectId,
      Name: $scope.Analysis_name,
      ItemValues: $scope.ItemValues,
      _id: $scope.Analysis_id
    };
    var urls = "api/Keyword/InsertAnalysisItem";
    var q = $http.post(
      urls,
      JSON.stringify($scope.anaItem),
      {
        headers: {
          'Content-Type': 'application/json'
        }
      }
    )
    q.success(function (response, status) {
      if (response.IsSuccess == true) {
        $scope.addAlert('success', response.Message);
        $scope.GetPrjAnalysisItem2();
        $scope.chang_AnalysisZ1();


      } else {
        $scope.addAlert('danger', response.Message);
      }
    });
    q.error(function (e) {
      $scope.addAlert('danger', "网络打盹了，请稍后。。。");
    });
  }
  $scope.clearAnalysisItem = function () {
    $scope.Analysis_name = "";
    $scope.ItemValues = [];
    $scope.ItemValues.push({Name: "", SeqNo: ""});
    $scope.ItemValues.push({Name: "", SeqNo: ""});
  }
  //设置为项目分析指项
  $scope.SetPrjAnalysisItem = function (id) {
    //if (confirm("设置分析指项后便不能在设置与修改分析指项，你确定要将本分析指项设置为项目分析指项吗？")) {
    $scope.anaItem = {
      usr_id: $rootScope.userID,
      prjId: $rootScope.getProjectId,
      anaItemId: id,
    };
    $http({
      method: 'get',
      params: $scope.anaItem,
      url: "/api/Keyword/SetPrjAnalysisItem"
    })
      .success(function (response, status) {
        $scope.addAlert('success', response.Message);
        $scope.GetPrjAnalysisItem();
        $scope.chang_AnalysisZ1();
        $scope.isActiveAnalysis_selected = false;
        $scope.GetPrjAnalysisItem2();

      })
      .error(function (response, status) {
        $scope.addAlert('danger', "网络打盹了，请稍后。。。");
      });
    //}
  }
  //获取设置项目分析指项
  $scope.GetPrjAnalysisItem = function () {
    $scope.anaItem = {
      usr_id: $rootScope.userID,
      prjId: $rootScope.getProjectId,
    };
    $http({
      method: 'get',
      params: $scope.anaItem,
      url: "/api/Keyword/GetPrjAnalysisItem"
    })
      .success(function (response, status) {
        if (response.Name == "" || response.Name == null) {
          $scope.isActiveAnalysis_selected = false;
        } else {
          $scope.isActiveAnalysis_selected = true;
          $scope.GetPrjAnalysisItem_list = response;
          $scope.GetPrjAnalysisItemName_list = response.ItemValues;
          $rootScope.PrjAnalysisItemName_list = response.ItemValues;
          $scope.changePrjAsIt_list1();
        }

      })
      .error(function (response, status) {
        $scope.addAlert('danger', "网络打盹了，请稍后。。。");
      });
  }
  //修改分析指项
  $scope.changeAnalysisItem = function (id, name, ItemValues) {
    $scope.Analysis_name = name;
    $scope.ItemValues = ItemValues;
    $scope.chang_AnalysisZ3();
    $scope.Analysis_id = id;
  };
  $scope.changeAnalysisItem2 = function (id, name, ItemValues) {
    $scope.Analysis_name = name;
    $scope.isactriveChgSelIt = false;
    $scope.ItemValues = ItemValues;
    $scope.chang_AnalysisZ3();
    $scope.Analysis_id = id;
  };
  //新建分析指项
  $scope.newAnalysisItem = function () {
    $scope.clearAnalysisItem();
    $scope.keywordControlIsActive3();
  }
  //删除分析指项
  $scope.RemoveAnalysisItem = function (id) {
    if (confirm("您确定要删除本分析指项吗？")) {
      $scope.anaItem = {
        anaItemId: id,
        user_id: $rootScope.userID
      };
      $http({
        method: 'get',
        params: $scope.anaItem,
        url: "/api/Keyword/RemoveAnalysisItem"
      })
        .success(function (response, status) {
          $scope.GetAnalysisItem();
          $scope.addAlert('success', response.Message);
        })
        .error(function (response, status) {
          $scope.addAlert('danger', "网络打盹了，请稍后。。。");
        });
    }

  }
  //命名实体管理____________________________
  $scope.NamedEntitySFun = function () {
      $scope.NamedEntityShow = !$scope.NamedEntityShow;
  }
    //获取实体树
  $scope.GetEntity = function () {
        $scope.paramsList = {
            userId: $rootScope.userID,
        };
        $http({
            method: 'get',
            params: $scope.paramsList,
            url: "/api/Keyword/GetEntity"
        })
        .success(function (response, status) {
            $scope.EntityList = response;
            if (response.length>0) {
                $scope.NamedEntityShow = true;
            } else {
                $scope.NamedEntityShow = false;
            }
            console.log(response);
        })
        .error(function (response, status) {
            alert("温馨提示:连接接服务器出错");
        });
  }
  //添加命名实体
  $scope.EntityItemValues = [];
  $scope.Entity = {};
  $scope.EntityPicUrl = "";
  $scope.add = function () {
      $scope.EntityItemValues.push({ Name: "", Key: "", Varients: '' });
  }
  $scope.InsertEntity = function () {
      if ($scope.EntityPicUrl == "" || $scope.EntityPicUrl == null) {
          $scope.alert_fun('danger', '请确保图片已经上传！上传图片时是否点击“确认”按钮')
          return
      }
      if ($scope.Entity.Key == "" || $scope.Entity.Key == null) {
          $scope.alert_fun('danger','请输入实体名称')
          return
      }
      if ($scope.Entity.Varients == "" || $scope.Entity.Varients == null) {
          $scope.alert_fun('danger', '请输入实体别称')
          return
      }
      if ((!$scope.EntityItemValues[$scope.EntityItemValues.length - 1].Name) || (!$scope.EntityItemValues[$scope.EntityItemValues.length - 1].Key)) {
          $scope.alert_fun('danger', '请输入至少一组实体属性')
          return
      }
      if (typeof ($scope.Entity.Varients) == "string") {
           $scope.Entity.Varients = $scope.Entity.Varients.split(';' || '；');
      }

      var ParentId = "";
      if ($scope.addEntityCard != 0 && $scope.addEntityCard != 1 && $scope.addEntityCard != null && $scope.addEntityCard!=undefined) {
          ParentId = $scope.addEntityCard;
      }

      $scope.anaItem = {
          UsrId: $rootScope.userID,
          Id: '',
          ParentId: ParentId,
          CreatedAt: '',
          Entity: $scope.Entity,
          Attributes: $scope.EntityItemValues,
          PicUrl: $scope.EntityPicUrl
      };
      var urls = "api/Keyword/InsertEntity";
      var q = $http.post(
        urls,
        JSON.stringify($scope.anaItem),
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
      )
      q.success(function (response, status) {
          if (response.IsSuccess == true) {
              $scope.addAlert('success','命名实体添加成功');
              $scope.GetEntity();
              $scope.addEntityCardfun('');
              $scope.Entity = {};
              $scope.EntityItemValues = [];
              $scope.EntityPicUrl = '';
              $scope.addEntityLeiFun(0);
          } else {
              $scope.addAlert('danger', response.Message);
          }
      });
      q.error(function (e) {
          $scope.addAlert('danger', "网络打盹了，请稍后。。。");
      });
  }
    //删除命名实体
  $scope.DelEntity=function(id){
      var url = "api/Keyword/DelEntity?id=" + id;
      var q = $http.get(url);
      q.success(function (response, status) {
          if (response.IsSuccess == true) {
              $scope.alert_fun('success', '命名实体删除成功！');
              $scope.GetEntity();
          } else {
              $scope.alert_fun('worning',response.Message)
          }
         
      })
      q.error(function (response, status) {
          alert("温馨提示:连接接服务器出错");
      });

  }

    //显示命名实体数card
  $scope.addEntityCardfun = function (id) {
      $scope.addEntityCard = id;
      if (!id){
          $scope.addEntityLeiFun(0);
      } else {
          $timeout(function () {
              $scope.editImage();
          },200)
      }
     
  }
    //选择类型
  $scope.addEntityLeiFun = function (num) {
      $scope.addEntityLeishow = !$scope.addEntityLeishow;
      if (num == 1) {
          $scope.EntityItemValues = [{ Name: "与父实体关系", Key: "", Varients: '' }, { Name: "单位性质", Key: "", Varients: '' },{ Name: "单位属地", Key: "", Varients: '' }];
      } else if (num == 2) {
          $scope.EntityItemValues = [{ Name: "与父实体关系", Key: "", Varients: '' }, { Name: "职位", Key: "", Varients: '' }, { Name: "年龄", Key: "", Varients: '' }];
      } else if (num == 3) {
          $scope.EntityItemValues = [{ Name: "与父实体关系", Key: "", Varients: '' }, { Name: "产品品类", Key: "", Varients: '' }, { Name: "单价", Key: "", Varients: '' }];
      } else if (num ==0) {
          $scope.EntityItemValues = [];
          $scope.addEntityLeishow = true;
      }
  }
  
  //显示实体介绍卡片
  $scope.ShowCardId = '';
  $scope.ShowCardFun = function (id) {
      $scope.ShowCardId = id;
  }
 
    //上传命名实体头像
  $scope.editImage = function () {
      xiuxiu.remove("userpic");
      $('#flashEditorOut').html("<div id='altContent'></div>");
      xiuxiu.embedSWF("altContent", 1, "100%", "100%", "userpic");
      xiuxiu.setUploadURL('http://43.240.138.233:9999/api/File/ImgUpload');
      xiuxiu.setUploadType(2);
      xiuxiu.setUploadDataFieldName("upload_file");
      //var cc = $rootScope.uer_PictureSrc;
      //xiuxiu.onInit = function () {
      //    xiuxiu.loadPhoto(cc, false);
      //}
      xiuxiu.onBeforeUpload = function (data, id) {
          var size = data.size;
          if (size > 2 * 1024 * 1024) {
              alert("图片不能超过2M");
              return false;
          }
          return true;
      }
      xiuxiu.onUploadResponse = function (data) {
          $scope.PictureSrc = data.substring(1, data.length - 1);
          $scope.EntityPicUrl = $scope.PictureSrc;
         
      }
      xiuxiu.onDebug = function (data) {
          alert("错误响应" + data);
      }
  }

  //自动加载__________________________________________________
  $rootScope.GetBaiduSearchKeyword2();
  $scope.GetBaiduKeyword();
  $scope.GetPrjAnalysisItem();










































    //域名管理+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
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

    // 此处为页面切换函数-------------------------------------------------------------------------------------------------
    //分组管理 和气泡图
  $scope.toScatter = function () {
      $scope.scatter = !$scope.scatter;
      if ($scope.scatter) {
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
      $scope.editorGroup.push({ x: name });
  }
    //取消修改分组
  $scope.cancleAmendDomainName = function () {
      $scope.tab2Editor = false;
      $scope.editorGroup = [];
      $scope.editorGroup.push({ x: name });
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
          $scope.editorDomains.push({ x: $scope.copyeditorDomains[i].x })
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


          var urls = "api/Keyword/SaveDomainCategory";
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

      var urls = "api/Keyword/SaveDomainCategory";
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
          var url = "api/Keyword/DelDomainCategory?id=" + id;
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
  $scope.domains.push({ x: "" });
    //添加域名input框
  $scope.addDomain = function () {
      $scope.domains.push({ x: "" });
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
      $scope.domains.push({ x: "" });
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
      var urls = "api/Keyword/SaveDomainCategoryData";
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
          $scope.editorDomains.push({ x: name });
          $scope.copyeditorDomains.push({ x: name });
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
              $scope.editorDomains.push({ x: $scope.domainList[i].DomainName });
              $scope.copyeditorDomains.push({ x: $scope.domainList[i].DomainName });
          }
      } else {
          $scope.checkedId = [];
          $scope.editorDomains = [];
          $scope.copyeditorDomains = [];
      }
  }

    //修改域名
  $scope.editorGroup = [];
  $scope.editorGroup.push({ x: "" });

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
      var urls = "api/Keyword/SaveDomainCategoryData";
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
              $scope.copyeditorDomains = [];
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
          var url = "api/Keyword/DelDomainCategoryData?DomainId=" + $scope.checkedId1;
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
                  $scope.copyeditorDomains = [];
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
      var url = "api/Keyword/GetAllDomainCategory?prjId=" + $rootScope.getProjectId;
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
      var url = "api/Keyword/GetAllDomainCategory?prjId=" + $rootScope.getProjectId;
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
      var url = "api/Keyword/GetDomainCategoryData?usrId=" + $rootScope.userID + "&categoryId=" + id + "&page=" + ($scope.pageDomainCategory - 1) + "&pagesize=" + $scope.pagesizeDomainCategory;
      var q = $http.get(url);
      q.success(function (response, status) {
          $scope.changeCurrentTab1();
          $scope.domains = [];
          $scope.domains.push({ x: "" });
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
      var url = "/api/Keyword/GetAllFenZhu?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
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
                  chkboxType: { "Y": "s", "N": "ps" }
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
      var url = "api/Keyword/GetAllDomainCategory?prjId=" + $rootScope.getProjectId;
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

      var url = "/api/Keyword/GetDomainStatis?categoryId=" + $scope.categoryId + "&prjId=" + $rootScope.getProjectId;
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
              { index: 0, text: '分组名' },
              { index: 1, text: '百度排名' },
              { index: 2, text: '有效链接数' },
              { index: 3, text: '关键词数' },
              { index: 4, text: '含发布时间占比' }
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
                  top: '15%',
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

    //网页关系简述
  $scope.GetLinkReferCount = function () {
      var url = "/api/Keyword/GetLinkReferCount?projectId=" + $rootScope.getProjectId;
      var q = $http.get(url);
      q.success(function (response, status) {
          $scope.GetLinkReferCountList = response;
      });
      q.error(function (response) {
          $scope.error = "网络打盹了，请稍后。。。";
      });
  }
    //网页关系简述设置
  $scope.GXT = {describeNum:0};
  $scope.GXTdescribe = function (num) {
      if ($scope.GXT.describeNum == num) {
          $scope.GXT.describeNum = 0;

      } else {
          $scope.GXT.describeNum = num;

      }
    
  }
    //插入关系图描述
  $scope.descList = {a:"",b:"",c:"",d:""};
  $scope.InsertReferChartDesc = function () {
      var addAll = true;
      $scope.descList2 = [$scope.descList.a, $scope.descList.b, $scope.descList.c, $scope.descList.d];
      for (var i = 0; i < $scope.descList2.length; i++) {
          if (!$scope.descList2[i]) {
             addAll=false
          }
      }
      if (!addAll) {
          $scope.alert_fun('warning', '请输入完整的关系图描述');
      } else {
          $scope.paramsList = {
              descList: $scope.descList2,
              projectId: $rootScope.getProjectId,
          };
          var urls = "/api/Keyword/InsertReferChartDesc";
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
                  $scope.alert_fun('success', "添加成功！");
                  $scope.GXTdescribe(0);
              } else {
                  $scope.alert_fun('danger', response.Message);
              }
          });
          q.error(function (e) {
              alert("网络打盹了，请稍后。。。");
          });
      }

  }
  //更新关系图描述
  $scope.UpdateReferChartDesc = function () {
      var addAll = true;
      $scope.descList2 = [$scope.descList.a, $scope.descList.b, $scope.descList.c, $scope.descList.d];
      for (var i = 0; i < $scope.descList2.length; i++) {
          if (!$scope.descList2[i]) {
              addAll = false
          }
      }
      if (!addAll) {
          $scope.alert_fun('warning', '请输入完整的关系图描述');
      } else {
          $scope.paramsList = {
              descList: $scope.descList2,
              descId: $scope.GetReferChartDescList.Id,
              projectId: $rootScope.getProjectId,
          };
          var urls = "/api/Keyword/UpdateReferChartDesc";
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
                  $scope.alert_fun('success', "更新成功！");
                  $scope.GXTdescribe(0);
              } else {
                  $scope.alert_fun('danger', response.Message);
              }
          });
          q.error(function (e) {
              alert("网络打盹了，请稍后。。。");
          });
      }

  }
    //获取获取关系图描述
  $scope.GetReferChartDesc = function () {
      var url = "/api/Keyword/GetReferChartDesc?projectId=" + $rootScope.getProjectId;
      var q = $http.get(url);
      q.success(function (response, status) {
          $scope.GetReferChartDescList = response;
          $scope.descList = { a: $scope.GetReferChartDescList.DescList[0], b: $scope.GetReferChartDescList.DescList[1], c: $scope.GetReferChartDescList.DescList[2], d: $scope.GetReferChartDescList.DescList[3] };
          console.log(response);
      });
      q.error(function (response) {
          $scope.error = "网络打盹了，请稍后。。。";
      });
  }




    //切换与加载_______________________________________________________
  $scope.keywordControlIsActive = function (num) {
      $scope.keywordCisActive = num;
      if (num == 1) {

      } else if (num == 2) {
          $('#Tilford-Tree svg').remove();
          $scope.GetD3TreeData();
      } else if (num == 3) {
          $('#tree-container svg').remove();
          $scope.GetTreeData();
      } else if (num == 4) {
          $('#bd svg').remove();
          $scope.GetData();
      } else if (num == 5) {
          $scope.juzhentu();
      } else if (num == 6) {
          $scope.GetAnalysisItem();
      } else if (num == 7) {
          $scope.GetECGuanxi();
          $scope.TextExtractByEntity();
          $scope.GetLinkReferCount();
          $scope.GetReferChartDesc();

      } else if (num == 8) {
          $scope.GetEntity();
      } else if (num == 9) {
          $scope.GetAllDomainCategoryFrist();
      }
  };



});