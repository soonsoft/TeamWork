;(function(global, factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        // For CommonJS and CommonJS-like environments where a proper `window`
        // is present, execute the factory and get avalon.
        // For environments that do not have a `window` with a `document`
        // (such as Node.js), expose a factory as module.exports.
        // This accentuates the need for the creation of a real `window`.
        // e.g. var ui = require("ui")(window);
        module.exports = global.document ? factory(global, true) : function(w) {
            if (!w.document) {
                throw new Error("ui.core.js requires a window with a document")
            }
            return factory(w);
        };
    } else {
        factory(global);
    }
}(typeof window !== "undefined" ? window : this, function(window, noGlobal) {
    //支持HTML5标签
    if(ui.core.ieVersion > 0 && ui.core.ieVersion < 9) {
        document.createElement("section");
        document.createElement("header");
        document.createElement("footer");
        document.createElement("address");
        document.createElement("article");
        document.createElement("aside");
        document.createElement("nav");
        document.createElement("menu");
    }
    
    //边栏管理器
    function SidebarManager() {
        this.sidebars = new ui.keyArray();
    }
    SidebarManager.prototype._checkName = function(name) {
        return ui.core.type(name) !== "string" || name.length == 0;
    };
    SidebarManager.prototype.setElement = function(name, option, element) {
        if(this._checkName(name)) {
            return;
        }
        var sidebar = null,
            that = this;
        if(this.sidebars.contains(name)) {
            sidebar = this.sidebars.get(name);
            if(element) {
                sidebar.set(element);
            }
        } else {
            if(!option || !option.parent) {
                throw ui.error("option is null");
            }
            sidebar = ui.ctrls.Sidebar(option, element);
            sidebar.hiding(function(e) {
                that.currentBar = null;
            });
            this.sidebars.set(name, sidebar);
        }
        return sidebar;
    };
    SidebarManager.prototype.get = function(name) {
        if(this._checkName(name)) {
            return null;
        }
        if(this.sidebars.contains(name)) {
            return this.sidebars.get(name);
        }
        return null;
    };
    SidebarManager.prototype.remove = function(name) {
        if(this._checkName(name)) {
            return;
        }
        if(this.sidebars.contains(name)) {
            this.sidebars.remove(name);
        }
    };
    SidebarManager.prototype.isShow = function() {
        return this.currentBar && this.currentBar.isShow();
    };
    SidebarManager.prototype.show = function(name) {
        if(this._checkName(name)) {
            return;
        }
        var sidebar = null,
            that = this;
        if(this.sidebars.contains(name)) {
            sidebar = this.sidebars.get(name);
            if(sidebar.isShow()) {
                return null;
            }
            if(this.currentBar) {
                return this.currentBar.hide().done(function() {
                    that.currentBar = sidebar;
                    sidebar.show();
                });
            } else {
                this.currentBar = sidebar;
                return sidebar.show();
            }
        }
        return null;
    };
    SidebarManager.prototype.hide = function() {
        var sidebar = this.currentBar,
            that = this;
        if(this._checkName(name)) {
            sidebar = this.currentBar;
        } else if(this.sidebars.contains(name)) {
            sidebar = this.sidebars.get(name);
        }
       if(!sidebar.isShow()) {
            return null;
        }
        if(sidebar) {
            this.currentBar = null;
            return sidebar.hide();
        }
        return null;
    };
    
    var masterpage = window.masterpage = {
        Name: "姓名",
        Department: "部门",
        Position: "职位",
        
        //当前是否为起始页
        isHomePage: false,
        //当前页面是否加载了导航菜单
        noMenu: true,
        //内容区域宽度
        contentBodyWidth: 0,
        //内容区域高度
        contentBodyHeight: 0,

        init: function() {
            var that = this;
            ui.docReady(function (e) {
                that._initElements();
                that._initContentSize();
                that._initUserSettings();
                ui.resize(function (e, clientWidth, clientHeight) {
                    that._initContentSize();
                }, ui.eventPriority.bodyResize);
                
                if(window.pageLogic) {
                    that.pageInit(pageLogic.init, pageLogic);
                }
            }, ui.eventPriority.masterReady);
        },
        _initElements: function () {
            this.sidebarManager = this.createSidebarManager();
        },
        _initContentSize: function () {
            var clientWidth = ui.core.root.clientWidth;
            var clientHeight = ui.core.root.clientHeight;
            this.head = $("#head");
            this.body = $("#body");
            if(this.head.length > 0) {
                clientHeight -= this.head.height();
            }
            var bodyMinHeight = clientHeight;
            this.body.css("height", bodyMinHeight + "px");
            this.contentBodyHeight = bodyMinHeight;
            this.contentBodyWidth = clientWidth;
        },
        _initUserSettings: function() {
            var userCover = $("#user"),
                that = this;
            
            var element = $("<section class='user-settings' />"),
                userInfo = $("<div class='user-info' />"),
                lightColor = $("<div class='light-color' />"),
                cmdList = $("<div class='cmd-list' />"),
                userSidebar,
                _oldShow;
            
            var htmlBuilder = [];
            htmlBuilder.push(
                "<div class='cover border-highlight'>",
                "<img src='", userCover.children("img").prop("src"), "' alt='用户头像' /></div>",
                "<div class='info'>",
                "<span class='font-highlight' style='font-size:18px;line-height:36px;'>", this.Name, "</span><br />",
                "<span>", this.Department, "</span><br />",
                "<span>", this.Position, "</span>",
                "</div>",
                "<div style='clear:left'></div>"
            );
            userInfo.append(htmlBuilder.join(""));
            
            if($.isArray(ui.theme.Colors)) {
                htmlBuilder = [];
                htmlBuilder.push("<h3 class='group-title font-highlight'>个性色</h3>");
                var i = 0,
                    len = ui.theme.Colors.length,
                    color;
                htmlBuilder.push("<div style='width:100%;height:auto'>");
                for(; i < len; i++) {
                    color = ui.theme.Colors[i];
                    htmlBuilder.push("<a href='javascript:void(0)' style='background-color:", color.Color, ";");
                    htmlBuilder.push("' title='", color.Name, "' data-index='", i, "'></a>");
                }
                htmlBuilder.push("</div>");
                lightColor.append(htmlBuilder.join(""));
                lightColor.click(function(e) {
                    var elem = $(e.target),
                        color;
                    if (elem.nodeName() === "A") {
                        color = ui.theme.Colors[parseInt(elem.attr("data-index"), 10)];
                        ui.ajax.ajaxPost(
                            "/Home/ChangeTheme", 
                            { themeId: color.Id },
                            function(success) {
                                if(success.Result) {
                                    $("#theme").prop("href", "/Home/Theme?themeId=" + color.Id);
                                    ui.theme.setThemeId(color.Id);
                                    ui.fire("themeChanged", color);
                                }
                            },
                            function(error) {
                                ui.msgshow("修改主题失败，" + error.Message, true);
                            }
                        );
                    }
                });
            }
            
            //添加一些主题API
            (function(){
                ui.theme.setThemeId = function (themeId) {
                    this.themeInfo = this.getTheme(themeId);
                    this.themeId = this.themeInfo.Id;
                };
                ui.theme.getCurrentThemeInfo = function () {
                    if (this.themeInfo) {
                        return ui.obj.clone(this.themeInfo);
                    } else {
                        return ui.obj.clone(this.getTheme());
                    }
                };
                ui.theme.getCurrentColor = function () {
                    if (this.themeInfo) {
                        return this.themeInfo.Color;
                    } else {
                        return this.getTheme().Color;
                    }
                };
                ui.theme.changeCssColor = function (id, cssObj) {
                    var themeLink = $("#" + id);
                    if (themeLink.length == 0)
                        return;
                    var sheet = themeLink.sheet();
                    var rules = sheet.cssRules();

                    var selector;
                    var item;
                    for (var i = 0; i < rules.length; i++) {
                        item = $(rules[i]);
                        selector = item.prop("selectorText");
                        if (cssObj.hasOwnProperty(selector)) {
                            item.css(cssObj[selector]);
                            cssObj[selector] = null;
                        }
                    }
                };
            })();
            
            htmlBuilder = [];
            htmlBuilder.push(
                "<ul>",
                "<li>",
                "<span>用户信息</span>",
                "<a href='/Account/Information'></a>",
                "</li>",
                "<li>",
                "<span>修改密码</span>",
                "<a href='/Account/ChangePassword'></a>",
                "</li>",
                "<li>",
                "<span>退出</span>",
                "<a href='/Account/Logout'></a>",
                "</li>",
                "</ul>"
            );
            cmdList.append(htmlBuilder.join(""));
            
            element
                .append(userInfo)
                .append("<hr />")
                .append(lightColor)
                .append("<hr />")
                .append(cmdList);
            
            userSidebar = this.sidebarManager.setElement("userSidebar", {
                parent: "body",
                width: 240
            }, element);
            userSidebar.animator[0].ease = ui.AnimationStyle.easeFromTo;
            userSidebar.contentAnimator = ui.animator({
                target: element,
                begin: 100,
                end: 0,
                ease: ui.AnimationStyle.easeTo,
                onChange: function (val, elem) {
                    elem.css("left", val + "%");
                }
            });
            userSidebar.contentAnimator.duration = 200;
            userSidebar.showing(function () {
                element.css("display", "none");
            });
            userSidebar.showed(function () {
                element.css({
                    "display": "block",
                    "left": "100%"
                });
                this.contentAnimator.start();
            });
            if(userCover.length > 0) {
                userCover.click(function(e) {
                    that.sidebarManager.show("userSidebar");
                });
            }
            
            //初始化当前用户的主题ID
            var tid = ui.theme.getCurrentThemeId();
            if (!tid) {
                throw ui.error("没有导入css主题文件");
            }
            ui.theme.setThemeId(tid);
            ui.fire("themeChanged", ui.theme.getCurrentThemeInfo());
        },
        pageInit: function (initObj, caller) {
            var func = null,
                caller = caller || this;
            if (ui.core.isPlainObject(initObj)) {
                for (var key in initObj) {
                    func = initObj[key];
                    if ($.isFunction(func)) {
                        func.call(caller);
                    }
                }
            }
        },
        
        //创建边栏管理器
        createSidebarManager: function() {
            return new SidebarManager();
        },
        //准备动态磁贴
        getTileOptions: function (option, menus, parentField, idField) {
            if (!$.isArray(menus) || menus.length == 0)
                return option;
            var menuTree = ui.data.listToTree(menus, parentField, idField);
            var options = [];
            if (option) {
                options.push(option);
            }
            var item = null;
            var i, j, len, t;
            for (i = 0; i < menuTree.length; i++) {
                item = menuTree[i];
                if (item.children == null || item.children.length == 0) {
                    if (option == null) {
                        option = {
                            tiles: []
                        };
                        options.push(option);
                    }
                    t = this._createTile(item);
                    if(t)
                        option.tiles.push(t);
                } else {
                    option = {
                        title: item.FunctionName,
                        tiles: []
                    };
                    for (j = 0, len = item.children.length; j < len; j++) {
                        t = this._createTile(item.children[j]);
                        if(t)
                            option.tiles.push(t);
                    }
                    if(option.tiles.length > 0)
                        options.push(option);
                }
            }
            return options;
        },
        _createTile: function (item) {
            if (!item.IsTile) {
                return null;
            }
            var tile = {
                type: item.Size || "medium",
                bgColor: item.BGColor || ui.theme.getCurrentColor(),
                title: item.FunctionName,
                icon: item.Icon,
                link: item.Url
            };
            if (item.MenuKey) {
                tile.name = item.MenuKey;
            }
            return tile;
        },
        initToolbar: function(toolbar) {
            var toolbarPanel = ui.core.getJQueryElement(toolbar),
                position;
            if(!toolbarPanel) {
                return;
            }
            position = toolbarPanel.css("position");
            toolbar = {
                toolbarPanel: toolbarPanel
            };
            toolbar.tools = [];
            toolbarPanel.children(".tools").each(function(index) {
                toolbar.tools.push($(this));
            });
            toolbar.extendPanel = toolbarPanel.children(".toolbar-extend");
            if(toolbar.extendPanel.length > 0) {
                if (position !== "absolute" && position !== "relative" && position !== "fixed") {
                    toolbar.toolbarPanel.css("position", "relative");
                }
                toolbar.extendWrapPanel = $("<div style='position:absolute;height:0px;width:100%;display:none;overflow:hidden;'/>");
                toolbar.extendWrapPanel.css("top", toolbar.toolbarPanel.height() + "px");
                toolbar.extendPanel.css("top", -parseFloat(toolbar.extendPanel.css("height"), 10) + "px");
                toolbar.extendPanel.addClass("clear");
                toolbar.extendWrapPanel.append(toolbar.extendPanel);
                toolbar.toolbarPanel.append(toolbar.extendWrapPanel);
                
                toolbar.showExtend = function() {
                    this.extendAnimator.stop();
                    var option = this.extendAnimator[0];
                    option.begin = parseFloat(option.target.css("top"), 10);
                    option.end = 0;
                    
                    option = this.extendAnimator[1];
                    option.begin = option.target.height();
                    option.end = this.extendPanel.height();
                    
                    this._cssOverflow = this.toolbarPanel.css("overflow");
                    this.toolbarPanel.css("overflow", "visible");
                    option.target.css("display", "block");
                    this.extendAnimator.start();
                };
                toolbar.hideExtend = function() {
                    var that = this;
                    this.extendAnimator.stop();
                    var option = this.extendAnimator[0];
                    option.begin = parseFloat(option.target.css("top"), 10);
                    option.end = -option.target.height();
                    
                    option = this.extendAnimator[1];
                    option.begin = option.target.height();
                    option.end = 0;
                    
                    this.extendAnimator.start().done(function() {
                        that.toolbarPanel.css("overflow", that._cssOverflow);
                        option.target.css("display", "none");
                    });
                };
                toolbar.extendAnimator = ui.animator({
                    target: toolbar.extendPanel,
                    ease: ui.AnimationStyle.easeFromTo,
                    onChange: function(val) {
                        this.target.css("top", val + "px");
                    }
                }).addTarget({
                    target: toolbar.extendWrapPanel,
                    ease: ui.AnimationStyle.easeFromTo,
                    onChange: function(val) {
                        this.target.css("height", val + "px");
                    }
                });
                toolbar.extendAnimator.duration = 300;
                if(toolbar.tools.length == 0) {
                    toolbar.tools = [$("<ul class='tools'>")];
                    toolbar.extendPanel.before(toolbar.tools[0]);
                }
                toolbar.extendButton = $("<li class='toolbar-icon-button' title='更多'><i class='fa fa-ellipsis-h'></i></li>");
                toolbar.extendButton.click(function(e) {
                    var elem = toolbar.extendButton;
                    if(elem.hasClass("extend-show")) {
                        elem.removeClass("extend-show")
                            .addClass("font-highlight-hover")
                            .removeClass("background-highlight");
                        toolbar.hideExtend();
                    } else {
                        elem.addClass("extend-show")
                            .removeClass("font-highlight-hover")
                            .addClass("background-highlight");
                        toolbar.showExtend();
                    }
                });
                toolbar.tools[toolbar.tools.length - 1].append(toolbar.extendButton);
            }
            toolbarPanel.find(".toolbar-icon-button").addClass("font-highlight-hover");
            if(pageLogic) {
                pageLogic.toolbar = toolbar;
            } else {
                this.toolbar = toolbar;
            }
        },
		//注册后退方法
        pushBack: function (func) {
            var defaultUrl,
                that;
			if(!this.backButton) {
                this.backButton = $("#backBtn");
                if(this.backButton.length == 0) {
                    this.backButton = null;
                    return;
                }
            }
            if ($.isFunction(func)) {
                if (!this.backList) {
                    this.backList = [];
                    defaultUrl = this.backButton.prop("href");
                    this.backButton.prop("href", "javascript:void(0)");
                    this.backList.push(function () {
                        location.href = defaultUrl;
                    });
                    that = this;
                    this.backButton.click(function(e) {
                        var func = null;
                        if (that.backList.length == 1) {
                            func = that.backList[0];
                        } else {
                            func = that.backList.pop();
                        }
                        func.call(that);
                    });
                }
                this.backList.push(func);
            }
        },
        //托管dom ready事件
        ready: function (func) {
            if ($.isFunction(func)) {
                ui.docReady(func, ui.eventPriority.pageReady);
            }
        },
        //托管window resize事件
        resize: function (func, autoCall) {
            if ($.isFunction(func)) {
                ui.resize(func, ui.eventPriority.elementResize);
                if(autoCall !== false) {
                    func.call(ui);
                }
            }
        }
    };

    var formatter = {
        getMonthCostValue: function (value, column) {
            value = parseFloat(value);
            if (isNaN(value)) {
                return null;
            }
            arguments[0] = value * 21.75;
            return ui.ColumnStyle.cfn.money.apply(this, arguments);
        },
        getMonthOfferValue: function(value, column) {
            value = parseFloat(value);
            if (isNaN(value)) {
                return null;
            }
            arguments[0] = value * 21.75;
            return ui.ColumnStyle.cfn.money.apply(this, arguments);
        }
    };

    masterpage.formatter = formatter;

    return masterpage;
}));