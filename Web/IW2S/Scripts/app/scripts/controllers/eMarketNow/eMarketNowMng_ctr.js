/// <reference path="../../views/kmean.aspx" />
var eMarketNowMng_ctr = myApp.controller("eMarketNowMng_ctr", function ($scope, $rootScope, $http, $location, $window, $cookieStore, $templateCache, $modal, myApplocalStorage) {

    $scope.keywordCisActive1 = true;
    $scope.keywordCisActive2 = false;
    $scope.toDoGetTreeData = false;
    $scope.isActivepro1 = true;
    $scope.isActiveShowKw = true;
    $scope.toDoGetTreeData = false;
    $scope.CurSelectedKeyword = [];
    chk_global_vars($cookieStore, $rootScope, null, $location, $http, myApplocalStorage);
    //_______________________________________________________________


    ////14.下拉框

    //$scope.changePrjAsIt_list1 = function () {
    //    console.log($rootScope.PrjAnalysisItemName_list);
    //    $scope.InfriTypes1 = new Array();
    //    for (i = 0; i < $rootScope.PrjAnalysisItemName_list.length ; i++) {
    //        $scope.InfriTypes1[$rootScope.PrjAnalysisItemName_list[i]._id] = $rootScope.PrjAnalysisItemName_list[i].Name;
    //    }
    //};
    //$scope.changePrjAsIt_list1();


    $scope.kindGroup = function (groupId, InfriLawCode, InfriLawCodeStr, Weight) {
        $scope.CurSelectedKeyword = [];
        $scope.InfriLawCode = InfriLawCode;
        $scope.Weight1 = Weight;
        $scope.InfriLawCodeStr1 = InfriLawCodeStr;
        var url = "/api/EMN/GetKeywordGroup?usr_id=" + $rootScope.userID + "&groupType=" + $scope.groupType + "&projectId=" + $rootScope.getProjectId;
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

            $scope.modelCallBack(groupId, 1, null, $scope.InfriLawCode, 0);
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    };

    $scope.EditKindGroup = function (groupId, parentid, Name, InfriLawCode, Weight) {
        $scope.CurSelectedKeyword = [];

        var url = "/api/EMN/GetEditKeywordGroup?usr_id=" + $rootScope.userID + "&groupType=" + $scope.groupType + "&projectId=" + $rootScope.getProjectId;
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
                $scope.modelCallBack(groupId, 2, Name, InfriLawCode, Weight);
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });

    };

    //
    $scope.modal_demo = function () {
        var kw_scope = $rootScope.$new();
        var frm = $modal.open({
            templateUrl: 'Scripts/app/views/modal/addElementGroupEMN.html',
            controller: addElementGroupEMN_ctr,
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
        var url = "/api/EMN/GetKeywordCategory?user_id=" + $rootScope.userID + "&groupType=" + $scope.groupType + "&projectId=" + $rootScope.getProjectId;
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
            $scope.error = "服务器连接出错";
        });
    };

    //关键词词组点击事件
    $scope.keywordMngClick = function (id) {
        $rootScope.keywordMngClicked = id;
        $cookieStore.put("keywordMngClicked", $rootScope.keywordMngClicked);


    }
    $scope.updateCategoryName = function (groupId) {
        var name = $("#" + groupId).text();
        var url = "/api/EMN/UpdateKeywordGroupName?groupid=" + groupId + "&groupName=" + name + "&user_id=" + $rootScope.userID;

        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('keywordMng_ctr>updateCategoryName');

        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    $scope.DelCategory = function (categoryId) {
        if (confirm("确定要删除这个组吗？")) {
            var url = "/api/EMN/DelKeywordCategory?categoryId=" + categoryId;

            var q = $http.get(url);
            q.success(function (response, status) {
                console.log('keywordMng_ctr>DelCategory');
                $scope.RefreshList();
            });
            q.error(function (response) {
                $scope.error = "服务器连接出错";
            });
        }
    };
    //operate:1,新增；2，修改
    $scope.modelCallBack = function (groupId, operate, Name, InfriLawCode, Weight) {
        var kw_scope = $rootScope.$new();
        kw_scope.GetBaiduSearchKeyword = $scope.SelectedKeyword;
        kw_scope.SelectedKeyword = $scope.CurSelectedKeyword;
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
            templateUrl: 'Scripts/app/views/modal/addElementGroupEMN.html',
            controller: addElementGroupEMN_ctr,
            scope: kw_scope,
            // label: label,
            keyboard: false,
            backdrop: 'static',
            size: 'lg'
        });
        frm.result.then(function (data) {
            $scope.RefreshList();
        });
    };

    $scope.RefreshList = function () {
        var url = "/api/EMN/GetKeywordCategory?user_id=" + $rootScope.userID + "&groupType=" + $scope.groupType + "&projectId=" + $rootScope.getProjectId;
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
            $scope.error = "服务器连接出错";
        });
    };


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
            y = y * scale + viewerHeight / 6;
            d3.select('g').transition()
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
        $scope.toDoGetTreeData = true;
        var url = "/api/EMN/GetAllGroupTree?usr_id=" + $rootScope.userID + "&projectId=" + $rootScope.getProjectId;
        var q = $http.get(url);
        q.success(function (response, status) {
            console.log('keywordMng_ctr>GetTreeData');
            $scope.d3tree(response);
            console.log(response);

        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
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
            url: "/api/EMN/ExportKeywordGroup"
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


    //关键词管理切换
    $scope.keywordControlIsActive1 = function () {
        $scope.keywordCisActive1 = true;
        $scope.keywordCisActive2 = false;
    };
    $scope.keywordControlIsActive2 = function () {
        $scope.keywordCisActive1 = false;
        $scope.keywordCisActive2 = true;
        if (!$scope.toDoGetTreeData) {
            $scope.GetTreeData();
        }
    };


    //实体库切换
    $scope.changepro1_show = function () {
        $scope.isActivepro1 = true;
    }
    $scope.changepro1_hide = function () {
        $scope.isActivepro1 = false;
        //$scope.GetBaiduKeyword();
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

    //自动加载__________________________________________________
    $rootScope.GetBaiduSearchKeyword2();
    //$scope.GetBaiduKeyword();
    //$scope.GetPrjAnalysisItem();



    //重搜
    $scope.searcheMarketNowAgain = function (id) {
        $scope.isgroup = true;
        var url = "/api/EMN/SetCommendEMNStatus?categoryId=" + id + "&prjId=" + $rootScope.getProjectId + "&isgroup=" + $scope.isgroup + "&status=" + 0 + "&user_id=" + $rootScope.userID;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess) {
                 $rootScope.addAlert('success', "所选分组正在重新搜索...");
                //alert("所选分组正在重新搜索...");
                $rootScope.GetBaiduSearchKeyword2();
                $scope.GetKeywordCategory();
                $scope.GetBaiduKeyword();
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }

    //搜所有关键词
    $scope.searchAlleMarketNowAgain = function (id) {
        $scope.isgroup = false;
        var url = "/api/EMN/SetCommendEMNStatus?categoryId=" + id + "&prjId=" + $rootScope.getProjectId + "&isgroup=" + $scope.isgroup + "&status=" + 0 + "&user_id=" + $rootScope.userID;
        var q = $http.get(url);
        q.success(function (response, status) {
            if (response.IsSuccess) {
                 $scope.addAlert('success', "所有关键词正在重新搜索...");
                //alert ("所有关键词正在重新搜索...");
                $rootScope.GetBaiduSearchKeyword2();
                $scope.GetAllKeywordCategory();
                $scope.GetBaiduKeyword();
            }
        });
        q.error(function (response) {
            $scope.error = "服务器连接出错";
        });
    }
});






