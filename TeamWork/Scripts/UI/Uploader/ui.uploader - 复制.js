$(function () {
    $.fn.bannerShow = function (g) {
        g = jQuery.extend({
            wraper: "fshow",
            autotime: 6000,
            isAuto: true,
            ind: 0,
            changeColor: null
        }, g || {});
        return this.each(function () {
            var o;
            var n = 0;
            var i = this;
            var j = $("#bnav"),
                k = $("li", j);
            var h = [
                ["#002c4a", "#005584"],
                ["#35ac03", "#3f4303"],
                ["#ac0908", "#cd5726"],
                ["18bbff", "#00486b"]
            ];
            $(this).hover(
                function () {
                    g.isAuto = false
                },
                function () {
                    g.isAuto = true
                }
            );
            m(0);
            k.each(function (p, q) {
                $(q).mouseover(function () {
                    n = p; k.removeClass().eq(n).addClass("current");
                    m(n);
                })
            });
            o = setInterval(function () {
                if (!g.isAuto) {
                    return false
                }
                k.each(function (p, q) {
                    if ($(q).hasClass("current")) {
                        n = p
                    }
                    m(n);
                });
                if (n == k.size() - 1) {
                    n = 0;
                    m(n);
                    k.removeClass().eq(n).addClass("current");
                } else {
                    n += 1; m(n);
                    k.removeClass().eq(n).addClass("current");
                }
            }, g.autotime);
            function m(p) {
                $(".banners", i).hide();
                $(".banners", i).eq(p).show();
                if (g.changeColor) {
                    g.changeColor(h[p]).set();
                }
            }
        })
    };
    if (!!document.createElement("canvas").getContext) {
        $.getScript("http://im-img.qq.com/pcqq/js/200/cav.js", function () {
            var t = {
                width: 1.5,
                height: 1.5,
                depth: 10,
                segments: 12,
                slices: 6,
                xRange: 0.8,
                yRange: 0.1,
                zRange: 1,
                ambient: "#525252",
                diffuse: "#FFFFFF",
                speed: 0.0002
            };
            var G = {
                count: 2,
                xyScalar: 1,
                zOffset: 100,
                ambient: "#002c4a",
                diffuse: "#005584",
                speed: 0.001,
                gravity: 1200,
                dampening: 0.95,
                minLimit: 10,
                maxLimit: null,
                minDistance: 20,
                maxDistance: 400,
                autopilot: false,
                draw: false,
                bounds: CAV.Vector3.create(),
                step: CAV.Vector3.create(
                    Math.randomInRange(0.2, 1),
                    Math.randomInRange(0.2, 1),
                    Math.randomInRange(0.2, 1))
            };
            var m = "canvas";
            var E = "svg";
            var x = { renderer: m };
            var i, n = Date.now();
            var L = CAV.Vector3.create();
            var k = CAV.Vector3.create();
            var z = document.getElementById("container");
            var w = document.getElementById("anitOut");
            var D, I, h, q, y;
            var g;
            var r;
            function C() {
                F();
                p();
                s();
                B();
                v();
                K(z.offsetWidth, z.offsetHeight);
                o();
            }
            function F() {
                g = new CAV.CanvasRenderer();
                H(x.renderer);
            }
            function H(N) {
                if (D) {
                    w.removeChild(D.element)
                }
                switch (N) {
                    case m: D = g;
                        break
                }
                D.setSize(z.offsetWidth, z.offsetHeight);
                w.appendChild(D.element);
            }
            function p() {
                I = new CAV.Scene();
            }
            function s() {
                I.remove(h);
                D.clear();
                q = new CAV.Plane(t.width * D.width, t.height * D.height, t.segments, t.slices);
                y = new CAV.Material(t.ambient, t.diffuse);
                h = new CAV.Mesh(q, y);
                I.add(h);
                var N, O;
                for (N = q.vertices.length - 1; N >= 0; N--) {
                    O = q.vertices[N];
                    O.anchor = CAV.Vector3.clone(O.position);
                    O.step = CAV.Vector3.create(Math.randomInRange(0.2, 1), Math.randomInRange(0.2, 1), Math.randomInRange(0.2, 1));
                    O.time = Math.randomInRange(0, Math.PIM2);
                }
            }
            function B() {
                var O, N;
                for (O = I.lights.length - 1; O >= 0; O--) {
                    N = I.lights[O];
                    I.remove(N);
                }
                D.clear();
                for (O = 0; O < G.count; O++) {
                    N = new CAV.Light(G.ambient, G.diffuse);
                    N.ambientHex = N.ambient.format();
                    N.diffuseHex = N.diffuse.format();
                    I.add(N);
                    N.mass = Math.randomInRange(0.5, 1);
                    N.velocity = CAV.Vector3.create();
                    N.acceleration = CAV.Vector3.create();
                    N.force = CAV.Vector3.create();
                }
            }
            function K(O, N) {
                D.setSize(O, N);
                CAV.Vector3.set(L, D.halfWidth, D.halfHeight);
                s();
            }
            function o() {
                i = Date.now() - n;
                u();
                M();
                requestAnimationFrame(o);
            }
            function u() {
                var Q, P, O, R, T, V, U, S = t.depth / 2; CAV.Vector3.copy(G.bounds, L);
                CAV.Vector3.multiplyScalar(G.bounds, G.xyScalar);
                CAV.Vector3.setZ(k, G.zOffset);
                for (R = I.lights.length - 1; R >= 0; R--) {
                    T = I.lights[R];
                    CAV.Vector3.setZ(T.position, G.zOffset);
                    var N = Math.clamp(CAV.Vector3.distanceSquared(T.position, k), G.minDistance, G.maxDistance);
                    var W = G.gravity * T.mass / N;
                    CAV.Vector3.subtractVectors(T.force, k, T.position);
                    CAV.Vector3.normalise(T.force);
                    CAV.Vector3.multiplyScalar(T.force, W);
                    CAV.Vector3.set(T.acceleration);
                    CAV.Vector3.add(T.acceleration, T.force);
                    CAV.Vector3.add(T.velocity, T.acceleration);
                    CAV.Vector3.multiplyScalar(T.velocity, G.dampening);
                    CAV.Vector3.limit(T.velocity, G.minLimit, G.maxLimit);
                    CAV.Vector3.add(T.position, T.velocity);
                }
                for (V = q.vertices.length - 1; V >= 0; V--) {
                    U = q.vertices[V];
                    Q = Math.sin(U.time + U.step[0] * i * t.speed);
                    P = Math.cos(U.time + U.step[1] * i * t.speed);
                    O = Math.sin(U.time + U.step[2] * i * t.speed);
                    CAV.Vector3.set(U.position, t.xRange * q.segmentWidth * Q, t.yRange * q.sliceHeight * P, t.zRange * S * O - S);
                    CAV.Vector3.add(U.position, U.anchor);
                }
                q.dirty = true;
            }
            function M() {
                D.render(I)
            }
            function J(O) {
                var Q, N, S = O;
                var P = function (T) {
                    for (Q = 0, l = I.lights.length; Q < l; Q++) {
                        N = I.lights[Q]; N.ambient.set(T);
                        N.ambientHex = N.ambient.format();
                    }
                };
                var R = function (T) {
                    for (Q = 0, l = I.lights.length; Q < l; Q++) {
                        N = I.lights[Q];
                        N.diffuse.set(T);
                        N.diffuseHex = N.diffuse.format();
                    }
                };
                return {
                    set: function () {
                        P(S[0]);
                        R(S[1]);
                    }
                }
            }
            function v() {
                window.addEventListener("resize", j);
            }
            function A(N) {
                CAV.Vector3.set(k, N.x, D.height - N.y);
                CAV.Vector3.subtract(k, L);
            }
            function j(N) {
                K(z.offsetWidth, z.offsetHeight);
                M();
            }
            C();
            $("#fshow").bannerShow({
                changeColor: J
            });
        })
    } else {
        $("#fshow").bannerShow();
    }

    $("#copytime").html((new Date()).getFullYear());
    $("#ovpage").click(function (g) {
        g.preventDefault();
        $("html, body").animate({ scrollTop: $("#container").height() }, 1000)
    });
    var c = function () { };
    if (typeof (pgvMain) != "function") {
        var f = "http://pingjs.qq.com/tcss.ping.js?v=1";
        $.getScript(f, function () {
            if (typeof (pgvMain) == "function") {
                pgvMain()
            }
            c = pgvSendClick;
            $(".forTcss").click(function () {
                var g = $(this).attr("name");
                var h = window.setTimeout(function () {
                    c({ hottag: g })
                }, 500)
            })
        })
    }
    window.online_resp = function (g) {
        if (g && g.c) { $("#cur_online").text(g.c.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,")); $("#max_online").text(g.h.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,")) }
    };
    $.getJSON("http://mma.qq.com/cgi-bin/im/online&callback=?", function (g) { });
    swfobject.embedSWF("http://im.qq.com/online/flash/flash20140304.swf", "flashBox", "910", "700", "9.0.0", "expressInstall.swf");
    $("#viewOnline").click(function (g) {
        g.preventDefault();
        $("#flashDiv").show();
        $("#overlay").show();
        $("#overlay").css({
            height: $(document).height()
        });
        c({ hottag: "pcqq.openflash" })
    });
    $("#flashClose").click(function (g) {
        g.preventDefault();
        $("#flashDiv").hide();
        $("#overlay").hide()
    });
    $("#flashZone").click(function () {
        var h = ($(window).width() - 615) / 2, g = ($(window).height() - 713) / 2;
        window.open("http://sns.qzone.qq.com/cgi-bin/qzshare/cgi_qzshare_onekey?url=http%3A%2F%2Fim.qq.com%2F&showcount=1&desc=%E4%BA%BF%E4%B8%87%E4%B8%AA%E9%97%AA%E8%80%80%E6%98%9F%E5%85%89%E8%83%8C%E5%90%8E%EF%BC%8C%E6%98%AF%E6%9C%8B%E5%8F%8B%EF%BC%8C%E6%98%AF%E5%AE%B6%E4%BA%BA%EF%BC%8C%E6%98%AF%E7%88%B1%E4%BA%BA%E2%80%A6%E2%80%A6%E7%99%BB%E5%BD%95QQ%EF%BC%8C%E4%B8%BA%E4%BB%96%EF%BC%88%E5%A5%B9%EF%BC%89%E7%82%B9%E4%BA%AE%E4%BD%A0%E7%9A%84%E9%82%A3%E9%A2%97%E6%98%9F%E3%80%82&summary=&title=&site=IM%20QQ%20-%20QQ%E5%AE%98%E6%96%B9%E7%BD%91%E7%AB%99&pics=http%3A%2F%2Fim-img.qq.com%2Fhome%2Fimg%2Fq2013%2Fyun.png&style=203&width=98&height=22&otype=share", "_blank", "toolbar=yes, location=yes, directories=no, status=no, menubar=yes, scrollbars=yes, resizable=no, copyhistory=yes, width=615, height=363, left=" + h + ", top=" + g);
        c({
            hottag: "pcqq.sharetozone"
        })
    });
    $("#flashWeibo").click(function () {
        window.open("http://share.v.t.qq.com/index.php?c=share&a=index&url=http%3A%2F%2Fim.qq.com%2F&title=%E4%BA%BF%E4%B8%87%E4%B8%AA%E9%97%AA%E8%80%80%E6%98%9F%E5%85%89%E8%83%8C%E5%90%8E%EF%BC%8C%E6%98%AF%E6%9C%8B%E5%8F%8B%EF%BC%8C%E6%98%AF%E5%AE%B6%E4%BA%BA%EF%BC%8C%E6%98%AF%E7%88%B1%E4%BA%BA%E2%80%A6%E2%80%A6%E7%99%BB%E5%BD%95QQ%EF%BC%8C%E4%B8%BA%E4%BB%96%EF%BC%88%E5%A5%B9%EF%BC%89%E7%82%B9%E4%BA%AE%E4%BD%A0%E7%9A%84%E9%82%A3%E9%A2%97%E6%98%9F%E3%80%82&pic=http%3A%2F%2Fim-img.qq.com%2Fhome%2Fimg%2Fq2013%2Fyun.png&appkey=", "_blank", "toolbar=yes, location=yes, directories=no, status=no, menubar=yes, scrollbars=yes, resizable=no, copyhistory=yes, width=615, height=363"); c({ hottag: "pcqq.sharetoweibo" })
    });
    var d = $(".ipage");
    var e = [],
        b = [];
    $(d).each(function (g, h) {
        e[g] = $(h).offset().top + Math.floor($(h).height() / 2);
        b[g] = $(h).find(".imgwrap")
    });
    var a = $(window).height();
    $(window).resize(function () {
        a = $(window).height()
    });
    $(window).scroll(function () {
        var g = $(window).scrollTop();
        $(e).each(function (h, j) {
            if (Math.floor(a / 2) + g >= j) {
                if (!$(b[h]).hasClass("showbg")) {
                    $(b[h]).addClass("showbg")
                }
            }
        })
    })
});