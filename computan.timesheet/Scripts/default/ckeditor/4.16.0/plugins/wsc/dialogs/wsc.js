﻿/*
 Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 For licensing, see LICENSE.html or http://ckeditor.com/license
*/
(function() {
    function A(a) { return a && a.domId && a.getInputElement().$ ? a.getInputElement() : a && a.$ ? a : !1 }

    function J(a) {
        if (!a) throw"Languages-by-groups list are required for construct selectbox";
        var d = [], c = "", e;
        for (e in a)
            for (var f in a[e]) {
                var h = a[e][f];
                "en_US" == h ? c = h : d.push(h)
            }
        d.sort();
        c && d.unshift(c);
        return{
            getCurrentLangGroup: function(d) {
                a:{
                    for (var c in a)
                        for (var e in a[c])
                            if (e.toUpperCase() === d.toUpperCase()) {
                                d = c;
                                break a
                            }
                    d = ""
                    }
                return d
            },
            setLangList: function() {
                var d = {}, c;
                for (c in a)
                    for (var e in a[c])
                        d[a[c][e]] =
                            e;
                return d
            }()
        }
    }

    var g = function() {
            var a = function(a, b, e) {
                e = e || {};
                var f = e.expires;
                if ("number" == typeof f && f) {
                    var h = new Date;
                    h.setTime(h.getTime() + 1E3 * f);
                    f = e.expires = h
                }
                f && f.toUTCString && (e.expires = f.toUTCString());
                b = encodeURIComponent(b);
                a = a + "\x3d" + b;
                for (var k in e) b = e[k], a += "; " + k, !0 !== b && (a += "\x3d" + b);
                document.cookie = a
            };
            return{
                postMessage: {
                    init: function(a) {
                        window.addEventListener
                            ? window.addEventListener("message", a, !1)
                            : window.attachEvent("onmessage", a)
                    },
                    send: function(a) {
                        var b = Object.prototype.toString,
                            e = a.fn || null,
                            f = a.id || "",
                            h = a.target || window,
                            k = a.message || { id: f };
                        a.message &&
                            "[object Object]" == b.call(a.message) &&
                            (a.message.id ? a.message.id : a.message.id = f, k = a.message);
                        a = window.JSON.stringify(k, e);
                        h.postMessage(a, "*")
                    },
                    unbindHandler: function(a) {
                        window.removeEventListener
                            ? window.removeEventListener("message", a, !1)
                            : window.detachEvent("onmessage", a)
                    }
                },
                hash: { create: function() {}, parse: function() {} },
                cookie: {
                    set: a,
                    get: function(a) {
                        return(a = document.cookie.match(new RegExp("(?:^|; )" +
                                a.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g,
                                    "\\$1") +
                                "\x3d([^;]*)")))
                            ? decodeURIComponent(a[1])
                            : void 0
                    },
                    remove: function(d) { a(d, "", { expires: -1 }) }
                },
                misc: {
                    findFocusable: function(a) {
                        var b = null;
                        a &&
                        (b = a.find(
                            "a[href], area[href], input, select, textarea, button, *[tabindex], *[contenteditable]"));
                        return b
                    },
                    isVisible: function(a) {
                        var b;
                        (b = 0 === a.offsetWidth || 0 == a.offsetHeight) ||
                        (b = "none" ===
                        (document.defaultView && document.defaultView.getComputedStyle
                            ? document.defaultView.getComputedStyle(a, null).display
                            : a.currentStyle
                            ? a.currentStyle.display
                            : a.style.display));
                        return!b
                    },
                    hasClass: function(a, b) {
                        return!(!a.className || !a.className.match(new RegExp("(\\s|^)" + b + "(\\s|$)")))
                    }
                }
            }
        }(),
        a = a || {};
    a.TextAreaNumber = null;
    a.load = !0;
    a.cmd = { SpellTab: "spell", Thesaurus: "thes", GrammTab: "grammar" };
    a.dialog = null;
    a.optionNode = null;
    a.selectNode = null;
    a.grammerSuggest = null;
    a.textNode = {};
    a.iframeMain = null;
    a.dataTemp = "";
    a.div_overlay = null;
    a.textNodeInfo = {};
    a.selectNode = {};
    a.selectNodeResponce = {};
    a.langList = null;
    a.langSelectbox = null;
    a.banner = "";
    a.show_grammar = null;
    a.div_overlay_no_check =
        null;
    a.targetFromFrame = {};
    a.onLoadOverlay = null;
    a.LocalizationComing = {};
    a.OverlayPlace = null;
    a.sessionid = "";
    a.LocalizationButton = {
        ChangeTo_button: { instance: null, text: "Change to", localizationID: "ChangeTo" },
        ChangeAll: { instance: null, text: "Change All" },
        IgnoreWord: { instance: null, text: "Ignore word" },
        IgnoreAllWords: { instance: null, text: "Ignore all words" },
        Options: { instance: null, text: "Options", optionsDialog: { instance: null } },
        AddWord: { instance: null, text: "Add word" },
        FinishChecking_button: {
            instance: null,
            text: "Finish Checking",
            localizationID: "FinishChecking"
        },
        Option_button: { instance: null, text: "Options", localizationID: "Options" },
        FinishChecking_button_block: { instance: null, text: "Finish Checking", localizationID: "FinishChecking" }
    };
    a.LocalizationLabel = {
        ChangeTo_label: { instance: null, text: "Change to", localizationID: "ChangeTo" },
        Suggestions: { instance: null, text: "Suggestions" },
        Categories: { instance: null, text: "Categories" },
        Synonyms: { instance: null, text: "Synonyms" }
    };
    var K = function(b) {
            var d, c, e;
            for (e in b) {
                if (d = a.dialog.getContentElement(a.dialog._.currentTabId,
                    e)) d = d.getElement();
                else if (b[e].instance) d = b[e].instance.getElement().getFirst() || b[e].instance.getElement();
                else continue;
                c = b[e].localizationID || e;
                d.setText(a.LocalizationComing[c])
            }
        },
        L = function(b) {
            var d, c, e;
            for (e in b)
                d = a.dialog.getContentElement(a.dialog._.currentTabId, e), d || (d = b[e].instance), d.setLabel &&
                    (c = b[e].localizationID || e, d.setLabel(a.LocalizationComing[c] + ":"))
        },
        t,
        B;
    a.framesetHtml = function(b) {
        return"\x3ciframe id\x3d" +
            a.iframeNumber +
            "_" +
            b +
            ' frameborder\x3d"0" allowtransparency\x3d"1" style\x3d"width:100%;border: 1px solid #AEB3B9;overflow: auto;background:#fff; border-radius: 3px;"\x3e\x3c/iframe\x3e'
    };
    a.setIframe = function(b, d) {
        var c;
        c = a.framesetHtml(d);
        var e = a.iframeNumber + "_" + d;
        b.getElement().setHtml(c);
        c = document.getElementById(e);
        c = c.contentWindow
            ? c.contentWindow
            : c.contentDocument.document
            ? c.contentDocument.document
            : c.contentDocument;
        c.document.open();
        c.document.write(
            '\x3c!DOCTYPE html\x3e\x3chtml\x3e\x3chead\x3e\x3cmeta charset\x3d"UTF-8"\x3e\x3ctitle\x3eiframe\x3c/title\x3e\x3cstyle\x3ehtml,body{margin: 0;height: 100%;font: 13px/1.555 "Trebuchet MS", sans-serif;}a{color: #888;font-weight: bold;text-decoration: none;border-bottom: 1px solid #888;}.main-box {color:#252525;padding: 3px 5px;text-align: justify;}.main-box p{margin: 0 0 14px;}.main-box .cerr{color: #f00000;border-bottom-color: #f00000;}\x3c/style\x3e\x3c/head\x3e\x3cbody\x3e\x3cdiv id\x3d"content" class\x3d"main-box"\x3e\x3c/div\x3e\x3ciframe src\x3d"" frameborder\x3d"0" id\x3d"spelltext" name\x3d"spelltext" style\x3d"display:none; width: 100%" \x3e\x3c/iframe\x3e\x3ciframe src\x3d"" frameborder\x3d"0" id\x3d"loadsuggestfirst" name\x3d"loadsuggestfirst" style\x3d"display:none; width: 100%" \x3e\x3c/iframe\x3e\x3ciframe src\x3d"" frameborder\x3d"0" id\x3d"loadspellsuggestall" name\x3d"loadspellsuggestall" style\x3d"display:none; width: 100%" \x3e\x3c/iframe\x3e\x3ciframe src\x3d"" frameborder\x3d"0" id\x3d"loadOptionsForm" name\x3d"loadOptionsForm" style\x3d"display:none; width: 100%" \x3e\x3c/iframe\x3e\x3cscript\x3e(function(window) {var ManagerPostMessage \x3d function() {var _init \x3d function(handler) {if (document.addEventListener) {window.addEventListener("message", handler, false);} else {window.attachEvent("onmessage", handler);};};var _sendCmd \x3d function(o) {var str,type \x3d Object.prototype.toString,fn \x3d o.fn || null,id \x3d o.id || "",target \x3d o.target || window,message \x3d o.message || { "id": id };if (o.message \x26\x26 type.call(o.message) \x3d\x3d "[object Object]") {(o.message["id"]) ? o.message["id"] : o.message["id"] \x3d id;message \x3d o.message;};str \x3d JSON.stringify(message, fn);target.postMessage(str, "*");};return {init: _init,send: _sendCmd};};var manageMessageTmp \x3d new ManagerPostMessage;var appString \x3d (function(){var spell \x3d parent.CKEDITOR.config.wsc.DefaultParams.scriptPath;var serverUrl \x3d parent.CKEDITOR.config.wsc.DefaultParams.serviceHost;return serverUrl + spell;})();function loadScript(src, callback) {var scriptTag \x3d document.createElement("script");scriptTag.type \x3d "text/javascript";callback ? callback : callback \x3d function() {};if(scriptTag.readyState) {scriptTag.onreadystatechange \x3d function() {if (scriptTag.readyState \x3d\x3d "loaded" ||scriptTag.readyState \x3d\x3d "complete") {scriptTag.onreadystatechange \x3d null;setTimeout(function(){scriptTag.parentNode.removeChild(scriptTag)},1);callback();}};}else{scriptTag.onload \x3d function() {setTimeout(function(){scriptTag.parentNode.removeChild(scriptTag)},1);callback();};};scriptTag.src \x3d src;document.getElementsByTagName("head")[0].appendChild(scriptTag);};window.onload \x3d function(){loadScript(appString, function(){manageMessageTmp.send({"id": "iframeOnload","target": window.parent});});}})(this);\x3c/script\x3e\x3c/body\x3e\x3c/html\x3e');
        c.document.close();
        a.div_overlay.setEnable()
    };
    a.setCurrentIframe = function(b) { a.setIframe(a.dialog._.contents[b].Content, b) };
    a.setHeightBannerFrame = function() {
        var b = a.dialog.getContentElement("SpellTab", "banner").getElement(),
            d = a.dialog.getContentElement("GrammTab", "banner").getElement(),
            c = a.dialog.getContentElement("Thesaurus", "banner").getElement();
        b.setStyle("height", "90px");
        d.setStyle("height", "90px");
        c.setStyle("height", "90px")
    };
    a.setHeightFrame = function() {
        document.getElementById(a.iframeNumber +
            "_" +
            a.dialog._.currentTabId).style.height = "240px"
    };
    a.sendData = function(b) {
        var d = b._.currentTabId, c = b._.contents[d].Content, e, f;
        a.previousTab = d;
        a.setIframe(c, d);
        var h = function(h) {
            d = b._.currentTabId;
            h = h || window.event;
            h.data.getTarget().is("a") &&
                d !== a.previousTab &&
                (a.previousTab = d, c = b._.contents[d].Content, e =
                    a.iframeNumber + "_" + d, a.div_overlay.setEnable(), c.getElement().getChildCount()
                    ? F(a.targetFromFrame[e], a.cmd[d])
                    : (a.setIframe(c, d), f = document.getElementById(e), a.targetFromFrame[e] = f.contentWindow))
        };
        b.parts.tabs.removeListener("click", h);
        b.parts.tabs.on("click", h)
    };
    a.buildSelectLang = function(a) {
        var d = new CKEDITOR.dom.element("div"), c = new CKEDITOR.dom.element("select");
        a = "wscLang" + a;
        d.addClass("cke_dialog_ui_input_select");
        d.setAttribute("role", "presentation");
        d.setStyles({
            height: "auto",
            position: "absolute",
            right: "0",
            top: "-1px",
            width: "160px",
            "white-space": "normal"
        });
        c.setAttribute("id", a);
        c.addClass("cke_dialog_ui_input_select");
        c.setStyles({ width: "160px" });
        d.append(c);
        return d
    };
    a.buildOptionLang =
        function(b, d) {
            var c = document.getElementById("wscLang" + d), e = document.createDocumentFragment(), f, h, k = [];
            if (0 === c.options.length) {
                for (f in b) k.push([f, b[f]]);
                k.sort();
                for (var l = 0; l < k.length; l++)
                    f = document.createElement("option"), f.setAttribute("value", k[l][1]), h =
                        document.createTextNode(k[l][0]), f.appendChild(h), e.appendChild(f);
                c.appendChild(e)
            }
            for (e = 0; e < c.options.length; e++)
                c.options[e].value == a.selectingLang && (c.options[e].selected = "selected")
        };
    a.buildOptionSynonyms = function(b) {
        b = a.selectNodeResponce[b];
        var d = A(a.selectNode.Synonyms);
        a.selectNode.Synonyms.clear();
        for (var c = 0; c < b.length; c++) {
            var e = document.createElement("option");
            e.text = b[c];
            e.value = b[c];
            d.$.add(e, c)
        }
        a.selectNode.Synonyms.getInputElement().$.firstChild.selected = !0;
        a.textNode.Thesaurus.setValue(a.selectNode.Synonyms.getInputElement().getValue())
    };
    var C = function(a) {
            var d = document, c = a.target || d.body, e = a.id || "overlayBlock", f = a.opacity || "0.9";
            a = a.background || "#f1f1f1";
            var h = d.getElementById(e), k = h || d.createElement("div");
            k.style.cssText =
                "position: absolute;top:30px;bottom:41px;left:1px;right:1px;z-index: 10020;padding:0;margin:0;background:" +
                a +
                ";opacity: " +
                f +
                ";filter: alpha(opacity\x3d" +
                100 * f +
                ");display: none;";
            k.id = e;
            h || c.appendChild(k);
            return{
                setDisable: function() { k.style.display = "none" },
                setEnable: function() { k.style.display = "block" }
            }
        },
        M = function(b, d, c) {
            var e = new CKEDITOR.dom.element("div"),
                f = new CKEDITOR.dom.element("input"),
                h = new CKEDITOR.dom.element("label"),
                k = "wscGrammerSuggest" + b + "_" + d;
            e.addClass("cke_dialog_ui_input_radio");
            e.setAttribute("role", "presentation");
            e.setStyles({ width: "97%", padding: "5px", "white-space": "normal" });
            f.setAttributes({ type: "radio", value: d, name: "wscGrammerSuggest", id: k });
            f.setStyles({ "float": "left" });
            f.on("click", function(b) { a.textNode.GrammTab.setValue(b.sender.getValue()) });
            c ? f.setAttribute("checked", !0) : !1;
            f.addClass("cke_dialog_ui_radio_input");
            h.appendText(b);
            h.setAttribute("for", k);
            h.setStyles({ display: "block", "line-height": "16px", "margin-left": "18px", "white-space": "normal" });
            e.append(f);
            e.append(h);
            return e
        },
        G = function(a) {
            a = a || "true";
            null !== a && "false" == a && u()
        },
        w = function(b) {
            var d = new J(b);
            b = "wscLang" + a.dialog.getParentEditor().name;
            b = document.getElementById(b);
            var c, e = a.iframeNumber + "_" + a.dialog._.currentTabId;
            a.buildOptionLang(d.setLangList, a.dialog.getParentEditor().name);
            if (c = d.getCurrentLangGroup(a.selectingLang)) v[c].onShow();
            G(a.show_grammar);
            b.onchange = function(b) {
                b = d.getCurrentLangGroup(this.value);
                var c = a.dialog._.currentTabId;
                v[b].onShow();
                G(a.show_grammar);
                a.div_overlay.setEnable();
                a.selectingLang = this.value;
                c = a.cmd[c];
                b && v[b] && v[b].allowedTabCommands[c] || (c = v[b].defaultTabCommand);
                for (var k in a.cmd)
                    if (a.cmd[k] == c) {
                        a.previousTab = k;
                        break
                    }
                g.postMessage.send({
                    message: { changeLang: a.selectingLang, interfaceLang: a.interfaceLang, text: a.dataTemp, cmd: c },
                    target: a.targetFromFrame[e],
                    id: "selectionLang_outer__page"
                })
            }
        },
        N = function(b) {
            var d,
                c = function(b) {
                    b = a.dialog.getContentElement(a.dialog._.currentTabId, b) || a.LocalizationButton[b].instance;
                    b.getElement().hasClass("cke_disabled")
                        ? b.getElement().setStyle("color",
                            "#a0a0a0")
                        : b.disable()
                };
            d = function(b) {
                b = a.dialog.getContentElement(a.dialog._.currentTabId, b) || a.LocalizationButton[b].instance;
                b.enable();
                b.getElement().setStyle("color", "#333")
            };
            "no_any_suggestions" == b
                ? (b = "No suggestions", d =
                    a.dialog.getContentElement(a.dialog._.currentTabId, "ChangeTo_button") ||
                    a.LocalizationButton.ChangeTo_button.instance, d.disable(), d =
                    a.dialog.getContentElement(a.dialog._.currentTabId, "ChangeAll") ||
                    a.LocalizationButton.ChangeAll.instance, d.disable(), c("ChangeTo_button"), c("ChangeAll"))
                : (d("ChangeTo_button"), d("ChangeAll"));
            return b
        },
        P = {
            iframeOnload: function(b) {
                b = a.dialog._.currentTabId;
                F(a.targetFromFrame[a.iframeNumber + "_" + b], a.cmd[b])
            },
            suggestlist: function(b) {
                delete b.id;
                a.div_overlay_no_check.setDisable();
                D();
                w(a.langList);
                var d = N(b.word), c = "";
                d instanceof Array && (d = b.word[0]);
                c = d = d.split(",");
                a.textNode.SpellTab.setValue(c[0]);
                b = A(B);
                B.clear();
                for (d = 0; d < c.length; d++) {
                    var e = document.createElement("option");
                    e.text = c[d];
                    e.value = c[d];
                    b.$.add(e, d)
                }
                p();
                a.div_overlay.setDisable()
            },
            grammerSuggest: function(b) {
                delete b.id;
                delete b.mocklangs;
                D();
                w(a.langList);
                var d = b.grammSuggest[0];
                a.grammerSuggest.getElement().setHtml("");
                a.textNode.GrammTab.reset();
                a.textNode.GrammTab.setValue(d);
                a.textNodeInfo.GrammTab.getElement().setHtml("");
                a.textNodeInfo.GrammTab.getElement().setText(b.info);
                b = b.grammSuggest;
                for (var d = b.length, c = !0, e = 0; e < d; e++)
                    a.grammerSuggest.getElement().append(M(b[e], b[e], c)), c = !1;
                p();
                a.div_overlay.setDisable()
            },
            thesaurusSuggest: function(b) {
                delete b.id;
                delete b.mocklangs;
                D();
                w(a.langList);
                a.selectNodeResponce = b;
                a.textNode.Thesaurus.reset();
                var d = A(a.selectNode.Categories), c = 0;
                a.selectNode.Categories.clear();
                for (var e in b) b = document.createElement("option"), b.text = e, b.value = e, d.$.add(b, c), c++;
                d = a.selectNode.Categories.getInputElement().getChildren().$[0].value;
                a.selectNode.Categories.getInputElement().getChildren().$[0].selected = !0;
                a.buildOptionSynonyms(d);
                p();
                a.div_overlay.setDisable()
            },
            finish: function(b) {
                delete b.id;
                O();
                b = a.dialog.getContentElement(a.dialog._.currentTabId,
                    "BlockFinishChecking").getElement();
                b.removeStyle("display");
                b.removeStyle("position");
                b.removeStyle("left");
                b.show();
                a.div_overlay.setDisable()
            },
            settext: function(b) {
                function d() {
                    try {
                        c.focus()
                    } catch (d) {
                    }
                    c.setData(b.text,
                        function() {
                            a.dataTemp = "";
                            c.unlockSelection();
                            c.fire("saveSnapshot");
                            a.dialog.hide()
                        })
                }

                delete b.id;
                a.dialog.getParentEditor().getCommand("checkspell");
                var c = a.dialog.getParentEditor(), e = CKEDITOR.plugins.scayt, f = c.scayt;
                if (e && c.wsc) {
                    var h = c.wsc.udn, k = c.wsc.ud, l, g;
                    if (f) {
                        var x = function() {
                            if (k)
                                for (l =
                                        k.split(","), g = 0;
                                    g < l.length;
                                    g += 1) f.addWordToUserDictionary(l[g]);
                            else c.wsc.DataStorage.setData("scayt_user_dictionary", []);
                            d()
                        };
                        e.state.scayt[c.name] && f.setMarkupPaused(!1);
                        h
                            ? (c.wsc.DataStorage.setData("scayt_user_dictionary_name", h),
                                f.restoreUserDictionary(h, x, x))
                            : (c.wsc.DataStorage.setData("scayt_user_dictionary_name", ""), f.removeUserDictionary(
                                void 0,
                                x,
                                x))
                    } else
                        h
                            ? c.wsc.DataStorage.setData("scayt_user_dictionary_name", h)
                            : c.wsc.DataStorage.setData("scayt_user_dictionary_name", ""), k &&
                        (l = k.split(","),
                            c.wsc.DataStorage.setData("scayt_user_dictionary", l)), d()
                } else d()
            },
            ReplaceText: function(b) {
                delete b.id;
                a.div_overlay.setEnable();
                a.dataTemp = b.text;
                a.selectingLang = b.currentLang;
                (b.cmd = "0" !== b.len && b.len)
                    ? a.div_overlay.setDisable()
                    : window.setTimeout(function() {
                            try {
                                a.div_overlay.setDisable()
                            } catch (b) {
                            }
                        },
                        500);
                K(a.LocalizationButton);
                L(a.LocalizationLabel)
            },
            options_checkbox_send: function(b) {
                delete b.id;
                b = { osp: g.cookie.get("osp"), udn: g.cookie.get("udn"), cust_dic_ids: a.cust_dic_ids };
                g.postMessage.send({
                    message: b,
                    target: a.targetFromFrame[a.iframeNumber + "_" + a.dialog._.currentTabId],
                    id: "options_outer__page"
                })
            },
            getOptions: function(b) {
                var d = b.DefOptions.udn;
                a.LocalizationComing = b.DefOptions.localizationButtonsAndText;
                a.show_grammar = b.show_grammar;
                a.langList = b.lang;
                a.bnr = b.bannerId;
                a.sessionid = b.sessionid;
                if (b.bannerId) {
                    a.setHeightBannerFrame();
                    var c = b.banner;
                    a.dialog.getContentElement(a.dialog._.currentTabId, "banner").getElement().setHtml(c)
                } else a.setHeightFrame();
                "undefined" == d &&
                (a.userDictionaryName
                    ? (d = a.userDictionaryName,
                        c = {
                            osp: g.cookie.get("osp"),
                            udn: a.userDictionaryName,
                            cust_dic_ids: a.cust_dic_ids,
                            id: "options_dic_send",
                            udnCmd: "create"
                        }, g.postMessage.send({ message: c, target: a.targetFromFrame[void 0] }))
                    : d = "");
                g.cookie.set("osp", b.DefOptions.osp);
                g.cookie.set("udn", d);
                g.cookie.set("cust_dic_ids", b.DefOptions.cust_dic_ids);
                g.postMessage.send({ id: "giveOptions" })
            },
            options_dic_send: function(b) {
                b = {
                    osp: g.cookie.get("osp"),
                    udn: g.cookie.get("udn"),
                    cust_dic_ids: a.cust_dic_ids,
                    id: "options_dic_send",
                    udnCmd: g.cookie.get("udnCmd")
                };
                g.postMessage.send({
                    message: b,
                    target: a.targetFromFrame[a.iframeNumber + "_" + a.dialog._.currentTabId]
                })
            },
            data: function(a) { delete a.id },
            giveOptions: function() {},
            setOptionsConfirmF: function() {},
            setOptionsConfirmT: function() { t.setValue("") },
            clickBusy: function() { a.div_overlay.setEnable() },
            suggestAllCame: function() {
                a.div_overlay.setDisable();
                a.div_overlay_no_check.setDisable()
            },
            TextCorrect: function() { w(a.langList) }
        },
        H = function(a) {
            a = a || window.event;
            var d;
            try {
                d = window.JSON.parse(a.data)
            } catch (c) {
            }
            if (d && d.id) P[d.id](d)
        },
        F = function(b, d, c, e) {
            d = d || CKEDITOR.config.wsc_cmd;
            c = c || a.dataTemp;
            g.postMessage.send({
                message: {
                    customerId: a.wsc_customerId,
                    text: c,
                    txt_ctrl: a.TextAreaNumber,
                    cmd: d,
                    cust_dic_ids: a.cust_dic_ids,
                    udn: a.userDictionaryName,
                    slang: a.selectingLang,
                    interfaceLang: a.interfaceLang,
                    reset_suggest: e || !1,
                    sessionid: a.sessionid
                },
                target: b,
                id: "data_outer__page"
            });
            a.div_overlay.setEnable()
        },
        v = {
            superset: {
                onShow: function() {
                    a.dialog.showPage("Thesaurus");
                    a.dialog.showPage("GrammTab");
                    q()
                },
                allowedTabCommands: {
                    spell: !0,
                    grammar: !0,
                    thes: !0
                },
                defaultTabCommand: "spell"
            },
            usual: {
                onShow: function() {
                    y();
                    u();
                    q()
                },
                allowedTabCommands: { spell: !0 },
                defaultTabCommand: "spell"
            },
            rtl: {
                onShow: function() {
                    y();
                    u();
                    q()
                },
                allowedTabCommands: { spell: !0 },
                defaultTabCommand: "spell"
            },
            spellgrammar: {
                onShow: function() {
                    y();
                    a.dialog.showPage("GrammTab");
                    q()
                },
                allowedTabCommands: { spell: !0, grammar: !0 },
                defaultTabCommand: "spell"
            },
            spellthes: {
                onShow: function() {
                    a.dialog.showPage("Thesaurus");
                    u();
                    q()
                },
                allowedTabCommands: { spell: !0, thes: !0 },
                defaultTabCommand: "spell"
            }
        },
        I = function(b) {
            var d =
            (new function(a) {
                var b = {};
                return{
                    getCmdByTab: function(d) {
                        for (var h in a) b[a[h]] = h;
                        return b[d]
                    }
                }
            }(a.cmd)).getCmdByTab(CKEDITOR.config.wsc_cmd);
            p();
            b.selectPage(d);
            a.sendData(b)
        },
        y = function() { a.dialog.hidePage("Thesaurus") },
        u = function() { a.dialog.hidePage("GrammTab") },
        q = function() { a.dialog.showPage("SpellTab") },
        p = function() {
            var b = a.dialog.getContentElement(a.dialog._.currentTabId, "bottomGroup").getElement();
            b.removeStyle("display");
            b.removeStyle("position");
            b.removeStyle("left");
            b.show()
        },
        O = function() {
            var b =
                    a.dialog.getContentElement(a.dialog._.currentTabId, "bottomGroup").getElement(),
                d = document.activeElement,
                c;
            b.setStyles({ display: "block", position: "absolute", left: "-9999px" });
            setTimeout(function() {
                    b.removeStyle("display");
                    b.removeStyle("position");
                    b.removeStyle("left");
                    b.hide();
                    a.dialog._.editor.focusManager.currentActive.focusNext();
                    c = g.misc.findFocusable(a.dialog.parts.contents);
                    if (g.misc.hasClass(d, "cke_dialog_tab") ||
                        g.misc.hasClass(d, "cke_dialog_contents_body") ||
                        !g.misc.isVisible(d))
                        for (var e = 0,
                            f;
                            e < c.count();
                            e++) {
                            if (f = c.getItem(e), g.misc.isVisible(f.$)) {
                                try {
                                    f.$.focus()
                                } catch (h) {
                                }
                                break
                            }
                        }
                    else
                        try {
                            d.focus()
                        } catch (k) {
                        }
                },
                0)
        },
        D = function() {
            var b = a.dialog.getContentElement(a.dialog._.currentTabId, "BlockFinishChecking").getElement(),
                d = document.activeElement,
                c;
            b.setStyles({ display: "block", position: "absolute", left: "-9999px" });
            setTimeout(function() {
                    b.removeStyle("display");
                    b.removeStyle("position");
                    b.removeStyle("left");
                    b.hide();
                    a.dialog._.editor.focusManager.currentActive.focusNext();
                    c = g.misc.findFocusable(a.dialog.parts.contents);
                    if (g.misc.hasClass(d, "cke_dialog_tab") ||
                        g.misc.hasClass(d, "cke_dialog_contents_body") ||
                        !g.misc.isVisible(d))
                        for (var e = 0, f; e < c.count(); e++) {
                            if (f = c.getItem(e), g.misc.isVisible(f.$)) {
                                try {
                                    f.$.focus()
                                } catch (h) {
                                }
                                break
                            }
                        }
                    else
                        try {
                            d.focus()
                        } catch (k) {
                        }
                },
                0)
        };
    CKEDITOR.dialog.add("checkspell",
        function(b) {
            function d(a) {
                var c = parseInt(b.config.wsc_left, 10),
                    d = parseInt(b.config.wsc_top, 10),
                    e = parseInt(b.config.wsc_width, 10),
                    g = parseInt(b.config.wsc_height, 10),
                    m = CKEDITOR.document.getWindow().getViewPaneSize();
                a.getPosition();
                var n = a.getSize(), r = 0;
                if (!a._.resized) {
                    var r = n.height -
                            a.parts.contents.getSize("height",
                                !(CKEDITOR.env.gecko || CKEDITOR.env.opera || CKEDITOR.env.ie && CKEDITOR.env.quirks)),
                        E = n.width - a.parts.contents.getSize("width", 1);
                    if (e < f.minWidth || isNaN(e)) e = f.minWidth;
                    e > m.width - E && (e = m.width - E);
                    if (g < f.minHeight || isNaN(g)) g = f.minHeight;
                    g > m.height - r && (g = m.height - r);
                    n.width = e + E;
                    n.height = g + r;
                    a._.fromResizeEvent = !1;
                    a.resize(e, g);
                    setTimeout(function() {
                            a._.fromResizeEvent = !1;
                            CKEDITOR.dialog.fire("resize",
                                {
                                    dialog: a,
                                    width: e,
                                    height: g
                                },
                                b)
                        },
                        300)
                }
                a._.moved ||
                (r = isNaN(c) && isNaN(d) ? 0 : 1, isNaN(c) && (c = (m.width - n.width) / 2), 0 > c && (c = 0),
                    c > m.width - n.width && (c = m.width - n.width), isNaN(d) && (d = (m.height - n.height) / 2),
                    0 > d && (d = 0), d > m.height - n.height && (d = m.height - n.height), a.move(c, d, r))
            }

            function c() {
                b.wsc = {};
                (function(a) {
                    var b = {
                            separator: "\x3c$\x3e",
                            getDataType: function(a) {
                                return"undefined" === typeof a
                                    ? "undefined"
                                    : null === a
                                    ? "null"
                                    : Object.prototype.toString.call(a).slice(8, -1)
                            },
                            convertDataToString: function(a) {
                                return this.getDataType(a).toLowerCase() +
                                    this.separator +
                                    a
                            },
                            restoreDataFromString: function(a) {
                                var b = a, c;
                                a = this.backCompatibility(a);
                                if ("string" === typeof a)
                                    switch (b = a.indexOf(this.separator), c = a.substring(0, b), b =
                                        a.substring(b + this.separator.length), c) {
                                    case "boolean":
                                        b = "true" === b;
                                        break;
                                    case "number":
                                        b = parseFloat(b);
                                        break;
                                    case "array":
                                        b = "" === b ? [] : b.split(",");
                                        break;
                                    case "null":
                                        b = null;
                                        break;
                                    case "undefined":
                                        b = void 0
                                    }
                                return b
                            },
                            backCompatibility: function(a) {
                                var b = a, c;
                                "string" === typeof a &&
                                (c = a.indexOf(this.separator), 0 > c &&
                                (b = parseFloat(a), isNaN(b) &&
                                ("[" === a[0] && "]" === a[a.length - 1]
                                    ? (a = a.replace("[", ""), a = a.replace("]", ""), b = "" === a ? [] : a.split(","))
                                    : b = "true" === a || "false" === a ? "true" === a : a), b =
                                    this.convertDataToString(b)));
                                return b
                            }
                        },
                        c = {
                            get: function(a) { return b.restoreDataFromString(window.localStorage.getItem(a)) },
                            set: function(a, c) {
                                var d = b.convertDataToString(c);
                                window.localStorage.setItem(a, d)
                            },
                            del: function(a) { window.localStorage.removeItem(a) },
                            clear: function() { window.localStorage.clear() }
                        },
                        d = {
                            expiration: 31622400,
                            get: function(a) { return b.restoreDataFromString(this.getCookie(a)) },
                            set: function(a, c) {
                                var d = b.convertDataToString(c);
                                this.setCookie(a, d, { expires: this.expiration })
                            },
                            del: function(a) { this.deleteCookie(a) },
                            getCookie: function(a) {
                                return(a = document.cookie.match(new RegExp("(?:^|; )" +
                                        a.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, "\\$1") +
                                        "\x3d([^;]*)")))
                                    ? decodeURIComponent(a[1])
                                    : void 0
                            },
                            setCookie: function(a, b, c) {
                                c = c || {};
                                var d = c.expires;
                                if ("number" === typeof d && d) {
                                    var e = new Date;
                                    e.setTime(e.getTime() + 1E3 * d);
                                    d = c.expires = e
                                }
                                d && d.toUTCString && (c.expires = d.toUTCString());
                                b = encodeURIComponent(b);
                                a = a + "\x3d" + b;
                                for (var h in c) b = c[h], a += "; " + h, !0 !== b && (a += "\x3d" + b);
                                document.cookie = a
                            },
                            deleteCookie: function(a) { this.setCookie(a, null, { expires: -1 }) },
                            clear: function() {
                                for (var a = document.cookie.split(";"), b = 0; b < a.length; b++) {
                                    var c = a[b], d = c.indexOf("\x3d"), c = -1 < d ? c.substr(0, d) : c;
                                    this.deleteCookie(c)
                                }
                            }
                        },
                        e = window.localStorage ? c : d;
                    a.DataStorage = {
                        getData: function(a) { return e.get(a) },
                        setData: function(a, b) { e.set(a, b) },
                        deleteData: function(a) { e.del(a) },
                        clear: function() { e.clear() }
                    }
                })(b.wsc);
                b.wsc.operationWithUDN =
                    function(b, c) {
                        g.postMessage.send({
                            message: { udn: c, id: "operationWithUDN", udnCmd: b },
                            target: a.targetFromFrame[a.iframeNumber + "_" + a.dialog._.currentTabId]
                        })
                    };
                b.wsc.getLocalStorageUDN = function() {
                    var a = b.wsc.DataStorage.getData("scayt_user_dictionary_name");
                    if (a) return a
                };
                b.wsc.getLocalStorageUD = function() {
                    var a = b.wsc.DataStorage.getData("scayt_user_dictionary");
                    if (a) return a
                };
                b.wsc.addWords = function(a, c) {
                    var d = b.config.wsc.DefaultParams.serviceHost +
                            b.config.wsc.DefaultParams.ssrvHost +
                            "?cmd\x3ddictionary\x26format\x3djson\x26customerid\x3d1%3AncttD3-fIoSf2-huzwE4-Y5muI2-mD0Tt-kG9Wz-UEDFC-tYu243-1Uq474-d9Z2l3\x26action\x3daddword\x26word\x3d" +
                            a +
                            "\x26callback\x3dtoString\x26synchronization\x3dtrue",
                        e = document.createElement("script");
                    e.type = "text/javascript";
                    e.src = d;
                    document.getElementsByTagName("head")[0].appendChild(e);
                    e.onload = c;
                    e.onreadystatechange = function() { "loaded" === this.readyState && c() }
                };
                b.wsc.cgiOrigin = function() {
                    var a = b.config.wsc.DefaultParams.serviceHost.split("/");
                    return a[0] + "//" + a[2]
                };
                b.wsc.isSsrvSame = !1
            }

            var e = function(c) {
                    this.getElement().focus();
                    a.div_overlay.setEnable();
                    c = a.dialog._.currentTabId;
                    var d = a.iframeNumber +
                            "_" +
                            c,
                        e = a.textNode[c].getValue(),
                        f = this.getElement().getAttribute("title-cmd");
                    g.postMessage.send({
                        message: { cmd: f, tabId: c, new_word: e },
                        target: a.targetFromFrame[d],
                        id: "cmd_outer__page"
                    });
                    "ChangeTo" != f && "ChangeAll" != f || b.fire("saveSnapshot");
                    "FinishChecking" == f && b.config.wsc_onFinish.call(CKEDITOR.document.getWindow().getFrame())
                },
                f = { minWidth: 560, minHeight: 444 };
            return{
                title: b.config.wsc_dialogTitle || b.lang.wsc.title,
                minWidth: f.minWidth,
                minHeight: f.minHeight,
                buttons: [CKEDITOR.dialog.cancelButton],
                onLoad: function() {
                    a.dialog =
                        this;
                    y();
                    u();
                    q();
                    b.plugins.scayt && c()
                },
                onShow: function() {
                    a.dialog = this;
                    b.lockSelection(b.getSelection());
                    a.TextAreaNumber = "cke_textarea_" + b.name;
                    g.postMessage.init(H);
                    a.dataTemp = b.getData();
                    a.OverlayPlace = a.dialog.parts.tabs.getParent().$;
                    if (CKEDITOR && CKEDITOR.config) {
                        a.wsc_customerId = b.config.wsc_customerId;
                        a.cust_dic_ids = b.config.wsc_customDictionaryIds;
                        a.userDictionaryName = b.config.wsc_userDictionaryName;
                        a.defaultLanguage = CKEDITOR.config.defaultLanguage;
                        var c = "file:" == document.location.protocol ? "http:" : document.location.protocol,
                            c = b.config.wsc_customLoaderScript ||
                                c + "//www.webspellchecker.net/spellcheck31/lf/22/js/wsc_fck2plugin.js";
                        d(this);
                        CKEDITOR.scriptLoader.load(c,
                            function(c) {
                                CKEDITOR.config && CKEDITOR.config.wsc && CKEDITOR.config.wsc.DefaultParams
                                    ? (a.serverLocationHash =
                                            CKEDITOR.config.wsc.DefaultParams.serviceHost, a.logotype =
                                            CKEDITOR.config.wsc.DefaultParams.logoPath, a.loadIcon =
                                            CKEDITOR.config.wsc.DefaultParams.iconPath, a.loadIconEmptyEditor =
                                            CKEDITOR.config.wsc.DefaultParams.iconPathEmptyEditor,
                                        a.LangComparer = new CKEDITOR.config.wsc.DefaultParams._SP_FCK_LangCompare)
                                    : (a.serverLocationHash = DefaultParams.serviceHost, a.logotype =
                                        DefaultParams.logoPath, a.loadIcon =
                                        DefaultParams.iconPath, a.loadIconEmptyEditor =
                                        DefaultParams.iconPathEmptyEditor, a.LangComparer = new _SP_FCK_LangCompare);
                                a.pluginPath = CKEDITOR.getUrl(b.plugins.wsc.path);
                                a.iframeNumber = a.TextAreaNumber;
                                a.templatePath = a.pluginPath + "dialogs/tmp.html";
                                a.LangComparer.setDefaulLangCode(a.defaultLanguage);
                                a.currentLang = b.config.wsc_lang ||
                                    a.LangComparer.getSPLangCode(b.langCode) ||
                                    "en_US";
                                a.interfaceLang = b.config.wsc_interfaceLang;
                                a.selectingLang = a.currentLang;
                                a.div_overlay = new C({
                                    opacity: "1",
                                    background: "#fff url(" + a.loadIcon + ") no-repeat 50% 50%",
                                    target: a.OverlayPlace
                                });
                                var d = a.dialog.parts.tabs.getId(), d = CKEDITOR.document.getById(d);
                                d.setStyle("width", "97%");
                                d.getElementsByTag("DIV").count() ||
                                    d.append(a.buildSelectLang(a.dialog.getParentEditor().name));
                                a.div_overlay_no_check = new C({
                                    opacity: "1",
                                    id: "no_check_over",
                                    background: "#fff url(" + a.loadIconEmptyEditor + ") no-repeat 50% 50%",
                                    target: a.OverlayPlace
                                });
                                c && (I(a.dialog), a.dialog.setupContent(a.dialog));
                                b.plugins.scayt &&
                                (b.wsc.isSsrvSame = function() {
                                    var a = CKEDITOR.config.wsc.DefaultParams.serviceHost
                                            .replace("lf/22/js/../../../", "").split("//")[1],
                                        c = CKEDITOR.config.wsc.DefaultParams.ssrvHost,
                                        d = b.config.scayt_srcUrl,
                                        e,
                                        f,
                                        h,
                                        g,
                                        l;
                                    window.SCAYT &&
                                        window.SCAYT.CKSCAYT &&
                                        (h = SCAYT.CKSCAYT.prototype.basePath, h.split("//"), g =
                                            h.split("//")[1].split("/")[0], l =
                                            h.split(g + "/")[1].replace("/lf/scayt3/ckscayt/", "") +
                                            "/script/ssrv.cgi");
                                    !d ||
                                        h ||
                                        b.config.scayt_servicePath ||
                                        (d.split("//"), e = d.split("//")[1].split("/")[0], f =
                                            d.split(e + "/")[1].replace("/lf/scayt3/ckscayt/ckscayt.js", "") +
                                            "/script/ssrv.cgi");
                                    return"//" + a + c ===
                                        "//" +
                                        (b.config.scayt_serviceHost || g || e) +
                                        "/" +
                                        (b.config.scayt_servicePath || l || f)
                                }());
                                if (window.SCAYT && b.wsc) {
                                    var e = b.wsc.cgiOrigin();
                                    b.wsc.syncIsDone = !1;
                                    c = function(a) {
                                        a.origin === e &&
                                        (a = JSON.parse(a.data), a.ud && "undefined" !== a.ud
                                            ? b.wsc.ud = a.ud
                                            : "undefined" === a.ud && (b.wsc.ud = void 0), a.udn &&
                                            "undefined" !== a.udn
                                            ? b.wsc.udn = a.udn
                                            : "undefined" === a.udn &&
                                            (b.wsc.udn =
                                                void 0), b.wsc.syncIsDone || (f(b.wsc.ud), b.wsc.syncIsDone = !0))
                                    };
                                    var f = function(c) {
                                        c = b.wsc.getLocalStorageUD();
                                        var d;
                                        c instanceof Array && (d = c.toString());
                                        void 0 !== d &&
                                            "" !== d &&
                                            setTimeout(function() {
                                                    b.wsc.addWords(d,
                                                        function() {
                                                            I(a.dialog);
                                                            a.dialog.setupContent(a.dialog)
                                                        })
                                                },
                                                400)
                                    };
                                    window.addEventListener
                                        ? addEventListener("message", c, !1)
                                        : window.attachEvent("onmessage", c);
                                    setTimeout(function() {
                                            var a = b.wsc.getLocalStorageUDN();
                                            void 0 !== a && b.wsc.operationWithUDN("restore", a)
                                        },
                                        500)
                                }
                            })
                    } else a.dialog.hide()
                },
                onHide: function() {
                    b.unlockSelection();
                    a.dataTemp = "";
                    a.sessionid = "";
                    g.postMessage.unbindHandler(H)
                },
                contents: [
                    {
                        id: "SpellTab",
                        label: "SpellChecker",
                        accessKey: "S",
                        elements: [
                            { type: "html", id: "banner", label: "banner", style: "", html: "\x3cdiv\x3e\x3c/div\x3e" },
                            {
                                type: "html",
                                id: "Content",
                                label: "spellContent",
                                html: "",
                                setup: function(b) {
                                    b = a.iframeNumber + "_" + b._.currentTabId;
                                    var c = document.getElementById(b);
                                    a.targetFromFrame[b] = c.contentWindow
                                }
                            }, {
                                type: "hbox",
                                id: "bottomGroup",
                                style: "width:560px; margin: 0 auto;",
                                widths: ["50%", "50%"],
                                className: "wsc-spelltab-bottom",
                                children: [
                                    {
                                        type: "hbox",
                                        id: "leftCol",
                                        align: "left",
                                        width: "50%",
                                        children: [
                                            {
                                                type: "vbox",
                                                id: "rightCol1",
                                                widths: ["50%", "50%"],
                                                children: [
                                                    {
                                                        type: "text",
                                                        id: "ChangeTo_label",
                                                        label: a.LocalizationLabel.ChangeTo_label.text + ":",
                                                        labelLayout: "horizontal",
                                                        labelStyle: "font: 12px/25px arial, sans-serif;",
                                                        width: "140px",
                                                        "default": "",
                                                        onShow: function() {
                                                            a.textNode.SpellTab = this;
                                                            a.LocalizationLabel.ChangeTo_label.instance = this
                                                        },
                                                        onHide: function() { this.reset() }
                                                    }, {
                                                        type: "hbox",
                                                        id: "rightCol",
                                                        align: "right",
                                                        width: "30%",
                                                        children: [
                                                            {
                                                                type: "vbox",
                                                                id: "rightCol_col__left",
                                                                children: [
                                                                    {
                                                                        type: "text",
                                                                        id: "labelSuggestions",
                                                                        label: a.LocalizationLabel.Suggestions.text +
                                                                            ":",
                                                                        onShow: function() {
                                                                            a.LocalizationLabel.Suggestions.instance =
                                                                                this;
                                                                            this.getInputElement()
                                                                                .setStyles({ display: "none" })
                                                                        }
                                                                    }, {
                                                                        type: "html",
                                                                        id: "logo",
                                                                        html: "",
                                                                        setup: function(b) {
                                                                            this.getElement().$.src = a.logotype;
                                                                            this.getElement().getParent()
                                                                                .setStyles({ "text-align": "left" })
                                                                        }
                                                                    }
                                                                ]
                                                            }, {
                                                                type: "select",
                                                                id: "list_of_suggestions",
                                                                labelStyle: "font: 12px/25px arial, sans-serif;",
                                                                size: "6",
                                                                inputStyle: "width: 140px; height: auto;",
                                                                items: [["loading..."]],
                                                                onShow: function() { B = this },
                                                                onChange: function() {
                                                                    a.textNode.SpellTab.setValue(this.getValue())
                                                                }
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }
                                        ]
                                    }, {
                                        type: "hbox",
                                        id: "rightCol",
                                        align: "right",
                                        width: "50%",
                                        children: [
                                            {
                                                type: "vbox",
                                                id: "rightCol_col__left",
                                                widths: ["50%", "50%", "50%", "50%"],
                                                children: [
                                                    {
                                                        type: "button",
                                                        id: "ChangeTo_button",
                                                        label: a.LocalizationButton.ChangeTo_button.text,
                                                        title: "Change to",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd", "ChangeTo");
                                                            a.LocalizationButton.ChangeTo_button.instance =
                                                                this
                                                        },
                                                        onClick: e
                                                    }, {
                                                        type: "button",
                                                        id: "ChangeAll",
                                                        label: a.LocalizationButton.ChangeAll.text,
                                                        title: "Change All",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd", this.id);
                                                            a.LocalizationButton.ChangeAll.instance = this
                                                        },
                                                        onClick: e
                                                    }, {
                                                        type: "button",
                                                        id: "AddWord",
                                                        label: a.LocalizationButton.AddWord.text,
                                                        title: "Add word",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd", this.id);
                                                            a.LocalizationButton.AddWord.instance = this
                                                        },
                                                        onClick: e
                                                    }, {
                                                        type: "button",
                                                        id: "FinishChecking_button",
                                                        label: a.LocalizationButton.FinishChecking_button.text,
                                                        title: "Finish Checking",
                                                        style: "width: 100%;margin-top: 9px;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd",
                                                                "FinishChecking");
                                                            a.LocalizationButton.FinishChecking_button.instance = this
                                                        },
                                                        onClick: e
                                                    }
                                                ]
                                            }, {
                                                type: "vbox",
                                                id: "rightCol_col__right",
                                                widths: ["50%", "50%", "50%"],
                                                children: [
                                                    {
                                                        type: "button",
                                                        id: "IgnoreWord",
                                                        label: a.LocalizationButton.IgnoreWord.text,
                                                        title: "Ignore word",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd",
                                                                this.id);
                                                            a.LocalizationButton.IgnoreWord.instance = this
                                                        },
                                                        onClick: e
                                                    }, {
                                                        type: "button",
                                                        id: "IgnoreAllWords",
                                                        label: a.LocalizationButton.IgnoreAllWords.text,
                                                        title: "Ignore all words",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd", this.id);
                                                            a.LocalizationButton.IgnoreAllWords.instance = this
                                                        },
                                                        onClick: e
                                                    }, {
                                                        type: "button",
                                                        id: "Options",
                                                        label: a.LocalizationButton.Options.text,
                                                        title: "Option",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            a.LocalizationButton.Options.instance = this;
                                                            "file:" ==
                                                                document.location.protocol &&
                                                                this.disable()
                                                        },
                                                        onClick: function() {
                                                            this.getElement().focus();
                                                            "file:" == document.location.protocol
                                                                ? alert(
                                                                    "WSC: Options functionality is disabled when runing from file system")
                                                                : (z = document.activeElement, b.openDialog("options"))
                                                        }
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }, {
                                type: "hbox",
                                id: "BlockFinishChecking",
                                style: "width:560px; margin: 0 auto;",
                                widths: ["70%", "30%"],
                                onShow: function() {
                                    this.getElement().setStyles({
                                        display: "block",
                                        position: "absolute",
                                        left: "-9999px"
                                    })
                                },
                                onHide: p,
                                children: [
                                    {
                                        type: "hbox",
                                        id: "leftCol",
                                        align: "left",
                                        width: "70%",
                                        children: [
                                            {
                                                type: "vbox",
                                                id: "rightCol1",
                                                setup: function() {
                                                    this.getChild()[0].getElement().$.src = a.logotype;
                                                    this.getChild()[0].getElement().getParent()
                                                        .setStyles({ "text-align": "center" })
                                                },
                                                children: [{ type: "html", id: "logo", html: "" }]
                                            }
                                        ]
                                    }, {
                                        type: "hbox",
                                        id: "rightCol",
                                        align: "right",
                                        width: "30%",
                                        children: [
                                            {
                                                type: "vbox",
                                                id: "rightCol_col__left",
                                                children: [
                                                    {
                                                        type: "button",
                                                        id: "Option_button",
                                                        label: a.LocalizationButton.Options.text,
                                                        title: "Option",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd",
                                                                this.id);
                                                            "file:" == document.location.protocol && this.disable()
                                                        },
                                                        onClick: function() {
                                                            this.getElement().focus();
                                                            "file:" == document.location.protocol
                                                                ? alert(
                                                                    "WSC: Options functionality is disabled when runing from file system")
                                                                : (z = document.activeElement, b.openDialog("options"))
                                                        }
                                                    }, {
                                                        type: "button",
                                                        id: "FinishChecking_button_block",
                                                        label: a.LocalizationButton.FinishChecking_button_block.text,
                                                        title: "Finish Checking",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd",
                                                                "FinishChecking")
                                                        },
                                                        onClick: e
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }, {
                        id: "GrammTab",
                        label: "Grammar",
                        accessKey: "G",
                        elements: [
                            { type: "html", id: "banner", label: "banner", style: "", html: "\x3cdiv\x3e\x3c/div\x3e" },
                            {
                                type: "html",
                                id: "Content",
                                label: "GrammarContent",
                                html: "",
                                setup: function() {
                                    var b = a.iframeNumber + "_" + a.dialog._.currentTabId,
                                        c = document.getElementById(b);
                                    a.targetFromFrame[b] = c.contentWindow
                                }
                            }, {
                                type: "vbox",
                                id: "bottomGroup",
                                style: "width:560px; margin: 0 auto;",
                                children: [
                                    {
                                        type: "hbox",
                                        id: "leftCol",
                                        widths: ["66%", "34%"],
                                        children: [
                                            {
                                                type: "vbox",
                                                children: [
                                                    {
                                                        type: "text",
                                                        id: "text",
                                                        label: "Change to:",
                                                        labelLayout: "horizontal",
                                                        labelStyle: "font: 12px/25px arial, sans-serif;",
                                                        inputStyle: "float: right; width: 200px;",
                                                        "default": "",
                                                        onShow: function() { a.textNode.GrammTab = this },
                                                        onHide: function() { this.reset() }
                                                    },
                                                    {
                                                        type: "html",
                                                        id: "html_text",
                                                        html:
                                                            "\x3cdiv style\x3d'min-height: 17px; line-height: 17px; padding: 5px; text-align: left;background: #F1F1F1;color: #595959; white-space: normal!important;'\x3e\x3c/div\x3e",
                                                        onShow: function(b) { a.textNodeInfo.GrammTab = this }
                                                    }, {
                                                        type: "html",
                                                        id: "radio",
                                                        html: "",
                                                        onShow: function() { a.grammerSuggest = this }
                                                    }
                                                ]
                                            }, {
                                                type: "vbox",
                                                children: [
                                                    {
                                                        type: "button",
                                                        id: "ChangeTo_button",
                                                        label: "Change to",
                                                        title: "Change to",
                                                        style: "width: 133px; float: right;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd", "ChangeTo")
                                                        },
                                                        onClick: e
                                                    },
                                                    {
                                                        type: "button",
                                                        id: "IgnoreWord",
                                                        label: "Ignore word",
                                                        title: "Ignore word",
                                                        style: "width: 133px; float: right;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd", this.id)
                                                        },
                                                        onClick: e
                                                    }, {
                                                        type: "button",
                                                        id: "IgnoreAllWords",
                                                        label: "Ignore Problem",
                                                        title: "Ignore Problem",
                                                        style: "width: 133px; float: right;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd", this.id)
                                                        },
                                                        onClick: e
                                                    },
                                                    {
                                                        type: "button",
                                                        id: "FinishChecking_button",
                                                        label: a.LocalizationButton.FinishChecking_button.text,
                                                        title: "Finish Checking",
                                                        style: "width: 133px; float: right; margin-top: 9px;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd",
                                                                "FinishChecking")
                                                        },
                                                        onClick: e
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }, {
                                type: "hbox",
                                id: "BlockFinishChecking",
                                style: "width:560px; margin: 0 auto;",
                                widths: ["70%", "30%"],
                                onShow: function() {
                                    this.getElement().setStyles({
                                        display: "block",
                                        position: "absolute",
                                        left: "-9999px"
                                    })
                                },
                                onHide: p,
                                children: [
                                    {
                                        type: "hbox",
                                        id: "leftCol",
                                        align: "left",
                                        width: "70%",
                                        children: [
                                            {
                                                type: "vbox",
                                                id: "rightCol1",
                                                children: [
                                                    {
                                                        type: "html",
                                                        id: "logo",
                                                        html: "",
                                                        setup: function() {
                                                            this.getElement().$.src = a.logotype;
                                                            this.getElement().getParent()
                                                                .setStyles({ "text-align": "center" })
                                                        }
                                                    }
                                                ]
                                            }
                                        ]
                                    }, {
                                        type: "hbox",
                                        id: "rightCol",
                                        align: "right",
                                        width: "30%",
                                        children: [
                                            {
                                                type: "vbox",
                                                id: "rightCol_col__left",
                                                children: [
                                                    {
                                                        type: "button",
                                                        id: "FinishChecking_button_block",
                                                        label: a.LocalizationButton.FinishChecking_button_block.text,
                                                        title: "Finish Checking",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd",
                                                                "FinishChecking")
                                                        },
                                                        onClick: e
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }, {
                        id: "Thesaurus",
                        label: "Thesaurus",
                        accessKey: "T",
                        elements: [
                            { type: "html", id: "banner", label: "banner", style: "", html: "\x3cdiv\x3e\x3c/div\x3e" },
                            {
                                type: "html",
                                id: "Content",
                                label: "spellContent",
                                html: "",
                                setup: function() {
                                    var b = a.iframeNumber + "_" + a.dialog._.currentTabId,
                                        c = document.getElementById(b);
                                    a.targetFromFrame[b] = c.contentWindow
                                }
                            }, {
                                type: "vbox",
                                id: "bottomGroup",
                                style: "width:560px; margin: -10px auto; overflow: hidden;",
                                children: [
                                    {
                                        type: "hbox",
                                        widths: ["75%", "25%"],
                                        children: [
                                            {
                                                type: "vbox",
                                                children: [
                                                    {
                                                        type: "hbox",
                                                        widths: ["65%", "35%"],
                                                        children: [
                                                            {
                                                                type: "text",
                                                                id: "ChangeTo_label",
                                                                label: a.LocalizationLabel.ChangeTo_label.text + ":",
                                                                labelLayout: "horizontal",
                                                                inputStyle: "width: 160px;",
                                                                labelStyle: "font: 12px/25px arial, sans-serif;",
                                                                "default": "",
                                                                onShow: function(b) {
                                                                    a.textNode.Thesaurus = this;
                                                                    a.LocalizationLabel.ChangeTo_label.instance =
                                                                        this
                                                                },
                                                                onHide: function() { this.reset() }
                                                            }, {
                                                                type: "button",
                                                                id: "ChangeTo_button",
                                                                label: a.LocalizationButton.ChangeTo_button.text,
                                                                title: "Change to",
                                                                style: "width: 121px; margin-top: 1px;",
                                                                onLoad: function() {
                                                                    this.getElement().setAttribute("title-cmd",
                                                                        "ChangeTo");
                                                                    a.LocalizationButton.ChangeTo_button.instance = this
                                                                },
                                                                onClick: e
                                                            }
                                                        ]
                                                    }, {
                                                        type: "hbox",
                                                        children: [
                                                            {
                                                                type: "select",
                                                                id: "Categories",
                                                                label: a.LocalizationLabel.Categories.text + ":",
                                                                labelStyle: "font: 12px/25px arial, sans-serif;",
                                                                size: "5",
                                                                inputStyle: "width: 180px; height: auto;",
                                                                items: [],
                                                                onShow: function() {
                                                                    a.selectNode.Categories = this;
                                                                    a.LocalizationLabel.Categories.instance = this
                                                                },
                                                                onChange: function() {
                                                                    a.buildOptionSynonyms(this.getValue())
                                                                }
                                                            }, {
                                                                type: "select",
                                                                id: "Synonyms",
                                                                label: a.LocalizationLabel.Synonyms.text + ":",
                                                                labelStyle: "font: 12px/25px arial, sans-serif;",
                                                                size: "5",
                                                                inputStyle: "width: 180px; height: auto;",
                                                                items: [],
                                                                onShow: function() {
                                                                    a.selectNode.Synonyms = this;
                                                                    a.textNode.Thesaurus.setValue(this.getValue());
                                                                    a.LocalizationLabel.Synonyms.instance = this
                                                                },
                                                                onChange: function(b) {
                                                                    a.textNode.Thesaurus.setValue(this.getValue())
                                                                }
                                                            }
                                                        ]
                                                    }
                                                ]
                                            },
                                            {
                                                type: "vbox",
                                                width: "120px",
                                                style: "margin-top:46px;",
                                                children: [
                                                    {
                                                        type: "html",
                                                        id: "logotype",
                                                        label: "WebSpellChecker.net",
                                                        html: "",
                                                        setup: function() {
                                                            this.getElement().$.src = a.logotype;
                                                            this.getElement().getParent()
                                                                .setStyles({ "text-align": "center" })
                                                        }
                                                    },
                                                    {
                                                        type: "button",
                                                        id: "FinishChecking_button",
                                                        label: a.LocalizationButton.FinishChecking_button.text,
                                                        title: "Finish Checking",
                                                        style: "width: 100%; float: right; margin-top: 9px;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd",
                                                                "FinishChecking")
                                                        },
                                                        onClick: e
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            },
                            {
                                type: "hbox",
                                id: "BlockFinishChecking",
                                style: "width:560px; margin: 0 auto;",
                                widths: ["70%", "30%"],
                                onShow: function() {
                                    this.getElement().setStyles({
                                        display: "block",
                                        position: "absolute",
                                        left: "-9999px"
                                    })
                                },
                                children: [
                                    {
                                        type: "hbox",
                                        id: "leftCol",
                                        align: "left",
                                        width: "70%",
                                        children: [
                                            {
                                                type: "vbox",
                                                id: "rightCol1",
                                                children: [
                                                    {
                                                        type: "html",
                                                        id: "logo",
                                                        html: "",
                                                        setup: function() {
                                                            this.getElement().$.src = a.logotype;
                                                            this.getElement().getParent()
                                                                .setStyles({ "text-align": "center" })
                                                        }
                                                    }
                                                ]
                                            }
                                        ]
                                    }, {
                                        type: "hbox",
                                        id: "rightCol",
                                        align: "right",
                                        width: "30%",
                                        children: [
                                            {
                                                type: "vbox",
                                                id: "rightCol_col__left",
                                                children: [
                                                    {
                                                        type: "button",
                                                        id: "FinishChecking_button_block",
                                                        label: a.LocalizationButton.FinishChecking_button_block.text,
                                                        title: "Finish Checking",
                                                        style: "width: 100%;",
                                                        onLoad: function() {
                                                            this.getElement().setAttribute("title-cmd",
                                                                "FinishChecking")
                                                        },
                                                        onClick: e
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
        });
    var z = null;
    CKEDITOR.dialog.add("options",
        function(b) {
            var d = null, c = {}, e = {}, f = null, h = null;
            g.cookie.get("udn");
            g.cookie.get("osp");
            b = function(a) {
                h = this.getElement().getAttribute("title-cmd");
                a = [];
                a[0] = e.IgnoreAllCapsWords;
                a[1] = e.IgnoreWordsNumbers;
                a[2] = e.IgnoreMixedCaseWords;
                a[3] = e.IgnoreDomainNames;
                a = a.toString().replace(/,/g, "");
                g.cookie.set("osp", a);
                g.cookie.set("udnCmd", h ? h : "ignore");
                "delete" != h && (a = "", "" !== t.getValue() && (a = t.getValue()), g.cookie.set("udn", a));
                g.postMessage.send({ id: "options_dic_send" })
            };
            var k = function() {
                f.getElement().setHtml(a.LocalizationComing.error);
                f.getElement().show()
            };
            return{
                title: a.LocalizationComing.Options,
                minWidth: 430,
                minHeight: 130,
                resizable: CKEDITOR.DIALOG_RESIZE_NONE,
                contents: [
                    {
                        id: "OptionsTab",
                        label: "Options",
                        accessKey: "O",
                        elements: [
                            {
                                type: "hbox",
                                id: "options_error",
                                children: [
                                    {
                                        type: "html",
                                        style:
                                            "display: block;text-align: center;white-space: normal!important; font-size: 12px;color:red",
                                        html: "\x3cdiv\x3e\x3c/div\x3e",
                                        onShow: function() { f = this }
                                    }
                                ]
                            }, {
                                type: "vbox",
                                id: "Options_content",
                                children: [
                                    {
                                        type: "hbox",
                                        id: "Options_manager",
                                        widths: ["52%", "48%"],
                                        children: [
                                            {
                                                type: "fieldset",
                                                label: "Spell Checking Options",
                                                style: "border: none;margin-top: 13px;padding: 10px 0 10px 10px",
                                                onShow: function() {
                                                    this.getInputElement().$.children[0].innerHTML =
                                                        a.LocalizationComing.SpellCheckingOptions
                                                },
                                                children: [
                                                    {
                                                        type: "vbox",
                                                        id: "Options_checkbox",
                                                        children: [
                                                            {
                                                                type: "checkbox",
                                                                id: "IgnoreAllCapsWords",
                                                                label: "Ignore All-Caps Words",
                                                                labelStyle:
                                                                    "margin-left: 5px; font: 12px/16px arial, sans-serif;display: inline-block;white-space: normal;",
                                                                style: "float:left; min-height: 16px;",
                                                                "default": "",
                                                                onClick: function() {
                                                                    e[this.id] = this.getValue() ? 1 : 0
                                                                }
                                                            }, {
                                                                type: "checkbox",
                                                                id: "IgnoreWordsNumbers",
                                                                label: "Ignore Words with Numbers",
                                                                labelStyle:
                                                                    "margin-left: 5px; font: 12px/16px arial, sans-serif;display: inline-block;white-space: normal;",
                                                                style: "float:left; min-height: 16px;",
                                                                "default": "",
                                                                onClick: function() {
                                                                    e[this.id] = this.getValue() ? 1 : 0
                                                                }
                                                            },
                                                            {
                                                                type: "checkbox",
                                                                id: "IgnoreMixedCaseWords",
                                                                label: "Ignore Mixed-Case Words",
                                                                labelStyle:
                                                                    "margin-left: 5px; font: 12px/16px arial, sans-serif;display: inline-block;white-space: normal;",
                                                                style: "float:left; min-height: 16px;",
                                                                "default": "",
                                                                onClick: function() {
                                                                    e[this.id] = this.getValue() ? 1 : 0
                                                                }
                                                            }, {
                                                                type: "checkbox",
                                                                id: "IgnoreDomainNames",
                                                                label: "Ignore Domain Names",
                                                                labelStyle:
                                                                    "margin-left: 5px; font: 12px/16px arial, sans-serif;display: inline-block;white-space: normal;",
                                                                style: "float:left; min-height: 16px;",
                                                                "default": "",
                                                                onClick: function() {
                                                                    e[this.id] = this.getValue() ? 1 : 0
                                                                }
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }, {
                                                type: "vbox",
                                                id: "Options_DictionaryName",
                                                children: [
                                                    {
                                                        type: "text",
                                                        id: "DictionaryName",
                                                        style: "margin-bottom: 10px",
                                                        label: "Dictionary Name:",
                                                        labelLayout: "vertical",
                                                        labelStyle: "font: 12px/25px arial, sans-serif;",
                                                        "default": "",
                                                        onLoad: function() {
                                                            t =
                                                                this;
                                                            var b = a.userDictionaryName
                                                                ? a.userDictionaryName
                                                                : (g.cookie.get("udn"), this.getValue());
                                                            this.setValue(b)
                                                        },
                                                        onShow: function() {
                                                            t = this;
                                                            var b = g.cookie.get("udn")
                                                                ? g.cookie.get("udn")
                                                                : this.getValue();
                                                            this.setValue(b);
                                                            this.setLabel(a.LocalizationComing.DictionaryName)
                                                        },
                                                        onHide: function() { this.reset() }
                                                    }, {
                                                        type: "hbox",
                                                        id: "Options_buttons",
                                                        children: [
                                                            {
                                                                type: "vbox",
                                                                id: "Options_leftCol_col",
                                                                widths: ["50%", "50%"],
                                                                children: [
                                                                    {
                                                                        type: "button",
                                                                        id: "create",
                                                                        label: "Create",
                                                                        title: "Create",
                                                                        style: "width: 100%;",
                                                                        onLoad: function() {
                                                                            this.getElement().setAttribute("title-cmd",
                                                                                this.id)
                                                                        },
                                                                        onShow: function() {
                                                                            (this.getElement().getFirst() ||
                                                                                    this.getElement())
                                                                                .setText(a.LocalizationComing.Create)
                                                                        },
                                                                        onClick: b
                                                                    },
                                                                    {
                                                                        type: "button",
                                                                        id: "restore",
                                                                        label: "Restore",
                                                                        title: "Restore",
                                                                        style: "width: 100%;",
                                                                        onLoad: function() {
                                                                            this.getElement().setAttribute("title-cmd",
                                                                                this.id)
                                                                        },
                                                                        onShow: function() {
                                                                            (this.getElement().getFirst() ||
                                                                                    this.getElement())
                                                                                .setText(a.LocalizationComing.Restore)
                                                                        },
                                                                        onClick: b
                                                                    }
                                                                ]
                                                            }, {
                                                                type: "vbox",
                                                                id: "Options_rightCol_col",
                                                                widths: ["50%", "50%"],
                                                                children: [
                                                                    {
                                                                        type: "button",
                                                                        id: "rename",
                                                                        label: "Rename",
                                                                        title: "Rename",
                                                                        style: "width: 100%;",
                                                                        onLoad: function() {
                                                                            this.getElement().setAttribute("title-cmd",
                                                                                this.id)
                                                                        },
                                                                        onShow: function() {
                                                                            (this.getElement().getFirst() ||
                                                                                    this.getElement())
                                                                                .setText(a.LocalizationComing.Rename)
                                                                        },
                                                                        onClick: b
                                                                    },
                                                                    {
                                                                        type: "button",
                                                                        id: "delete",
                                                                        label: "Remove",
                                                                        title: "Remove",
                                                                        style: "width: 100%;",
                                                                        onLoad: function() {
                                                                            this.getElement().setAttribute("title-cmd",
                                                                                this.id)
                                                                        },
                                                                        onShow: function() {
                                                                            (this.getElement().getFirst() ||
                                                                                    this.getElement())
                                                                                .setText(a.LocalizationComing.Remove)
                                                                        },
                                                                        onClick: b
                                                                    }
                                                                ]
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }
                                        ]
                                    }, {
                                        type: "hbox",
                                        id: "Options_text",
                                        children: [
                                            {
                                                type: "html",
                                                style:
                                                    "text-align: justify;margin-top: 15px;white-space: normal!important; font-size: 12px;color:#777;",
                                                html: "\x3cdiv\x3e" +
                                                    a.LocalizationComing.OptionsTextIntro +
                                                    "\x3c/div\x3e",
                                                onShow: function() {
                                                    this.getElement().setText(a.LocalizationComing.OptionsTextIntro)
                                                }
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ],
                buttons: [CKEDITOR.dialog.okButton, CKEDITOR.dialog.cancelButton],
                onOk: function() {
                    var a = [];
                    a[0] = e.IgnoreAllCapsWords;
                    a[1] = e.IgnoreWordsNumbers;
                    a[2] = e.IgnoreMixedCaseWords;
                    a[3] = e.IgnoreDomainNames;
                    a = a.toString().replace(/,/g, "");
                    g.cookie.set("osp", a);
                    g.postMessage.send({ id: "options_checkbox_send" });
                    f.getElement().hide();
                    f.getElement().setHtml(" ")
                },
                onLoad: function() {
                    d = this;
                    c.IgnoreAllCapsWords = d.getContentElement("OptionsTab", "IgnoreAllCapsWords");
                    c.IgnoreWordsNumbers = d.getContentElement("OptionsTab", "IgnoreWordsNumbers");
                    c.IgnoreMixedCaseWords = d.getContentElement("OptionsTab", "IgnoreMixedCaseWords");
                    c.IgnoreDomainNames = d.getContentElement("OptionsTab", "IgnoreDomainNames")
                },
                onShow: function() {
                    g.postMessage.init(k);
                    var b = g.cookie.get("osp").split("");
                    e.IgnoreAllCapsWords = b[0];
                    e.IgnoreWordsNumbers = b[1];
                    e.IgnoreMixedCaseWords = b[2];
                    e.IgnoreDomainNames = b[3];
                    parseInt(e.IgnoreAllCapsWords, 10)
                        ? c.IgnoreAllCapsWords.setValue("checked", !1)
                        : c.IgnoreAllCapsWords.setValue("", !1);
                    parseInt(e.IgnoreWordsNumbers, 10)
                        ? c.IgnoreWordsNumbers.setValue("checked", !1)
                        : c.IgnoreWordsNumbers.setValue("", !1);
                    parseInt(e.IgnoreMixedCaseWords, 10)
                        ? c.IgnoreMixedCaseWords.setValue("checked", !1)
                        : c.IgnoreMixedCaseWords.setValue("", !1);
                    parseInt(e.IgnoreDomainNames,
                            10)
                        ? c.IgnoreDomainNames.setValue("checked", !1)
                        : c.IgnoreDomainNames.setValue("", !1);
                    e.IgnoreAllCapsWords = c.IgnoreAllCapsWords.getValue() ? 1 : 0;
                    e.IgnoreWordsNumbers = c.IgnoreWordsNumbers.getValue() ? 1 : 0;
                    e.IgnoreMixedCaseWords = c.IgnoreMixedCaseWords.getValue() ? 1 : 0;
                    e.IgnoreDomainNames = c.IgnoreDomainNames.getValue() ? 1 : 0;
                    c.IgnoreAllCapsWords.getElement().$.lastChild.innerHTML = a.LocalizationComing.IgnoreAllCapsWords;
                    c.IgnoreWordsNumbers.getElement().$.lastChild.innerHTML =
                        a.LocalizationComing.IgnoreWordsWithNumbers;
                    c.IgnoreMixedCaseWords.getElement().$.lastChild.innerHTML =
                        a.LocalizationComing.IgnoreMixedCaseWords;
                    c.IgnoreDomainNames.getElement().$.lastChild.innerHTML = a.LocalizationComing.IgnoreDomainNames
                },
                onHide: function() {
                    g.postMessage.unbindHandler(k);
                    if (z)
                        try {
                            z.focus()
                        } catch (a) {
                        }
                }
            }
        });
    CKEDITOR.dialog.on("resize",
        function(b) {
            b = b.data;
            var d = b.dialog, c = CKEDITOR.document.getById(a.iframeNumber + "_" + d._.currentTabId);
            "checkspell" == d._.name &&
            (a.bnr
                ? c && c.setSize("height", b.height - 310)
                : c &&
                c.setSize("height",
                    b.height -
                    220), d._.fromResizeEvent && !d._.resized && (d._.resized = !0), d._.fromResizeEvent = !0)
        });
    CKEDITOR.on("dialogDefinition",
        function(b) {
            if ("checkspell" === b.data.name) {
                var d = b.data.definition;
                a.onLoadOverlay =
                    new C({ opacity: "1", background: "#fff", target: d.dialog.parts.tabs.getParent().$ });
                a.onLoadOverlay.setEnable();
                d.dialog.on("cancel",
                    function(b) {
                        d.dialog.getParentEditor().config.wsc_onClose.call(this.document.getWindow().getFrame());
                        a.div_overlay.setDisable();
                        a.onLoadOverlay.setDisable();
                        return!1
                    },
                    this,
                    null,
                    -1)
            }
        })
})();