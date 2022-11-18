define("ace/mode/sh_highlight_rules",
    ["require", "exports", "module", "ace/lib/oop", "ace/mode/text_highlight_rules"],
    function(e, t, n) {
        "use strict";
        var r = e("../lib/oop"),
            i = e("./text_highlight_rules").TextHighlightRules,
            s = t.reservedKeywords =
                "!|{|}|case|do|done|elif|else|esac|fi|for|if|in|then|until|while|&|;|export|local|read|typeset|unset|elif|select|set",
            o = t.languageConstructs =
                "[|]|alias|bg|bind|break|builtin|cd|command|compgen|complete|continue|dirs|disown|echo|enable|eval|exec|exit|fc|fg|getopts|hash|help|history|jobs|kill|let|logout|popd|printf|pushd|pwd|return|set|shift|shopt|source|suspend|test|times|trap|type|ulimit|umask|unalias|wait",
            u = function() {
                var e = this.createKeywordMapper(
                        { keyword: s, "support.function.builtin": o, "invalid.deprecated": "debugger" },
                        "identifier"),
                    t = "(?:(?:[1-9]\\d*)|(?:0))",
                    n = "(?:\\.\\d+)",
                    r = "(?:\\d+)",
                    i = "(?:(?:" + r + "?" + n + ")|(?:" + r + "\\.))",
                    u = "(?:(?:" + i + "|" + r + ")" + ")",
                    a = "(?:" + u + "|" + i + ")",
                    f = "(?:&" + r + ")",
                    l = "[a-zA-Z_][a-zA-Z0-9_]*",
                    c = "(?:(?:\\$" + l + ")|(?:" + l + "=))",
                    h = "(?:\\$(?:SHLVL|\\$|\\!|\\?))",
                    p = "(?:" + l + "\\s*\\(\\))";
                this.$rules = {
                    start: [
                        { token: "constant", regex: /\\./ }, { token: ["text", "comment"], regex: /(^|\s)(#.*)$/ },
                        {
                            token: "string",
                            regex: '"',
                            push: [
                                {
                                    token: "constant.language.escape",
                                    regex:
                                        /\\(?:[$abeEfnrtv\\'"]|x[a-fA-F\d]{1,2}|u[a-fA-F\d]{4}([a-fA-F\d]{4})?|c.|\d{1,3})/
                                }, { token: "constant", regex: /\$\w+/ }, { token: "string", regex: '"', next: "pop" },
                                { defaultToken: "string" }
                            ]
                        }, { regex: "<<<", token: "keyword.operator" }, {
                            stateName: "heredoc",
                            regex: "(<<)(\\s*)(['\"`]?)([\\w\\-]+)(['\"`]?)",
                            onMatch: function(e, t, n) {
                                var r = e[2] == "-" ? "indentedHeredoc" : "heredoc", i = e.split(this.splitRegex);
                                return n.push(r, i[4]), [
                                    { type: "constant", value: i[1] }, { type: "text", value: i[2] },
                                    { type: "string", value: i[3] }, { type: "support.class", value: i[4] },
                                    { type: "string", value: i[5] }
                                ];
                            },
                            rules: {
                                heredoc: [
                                    {
                                        onMatch: function(e, t, n) {
                                            return e === n[1]
                                                ? (n.shift(), n.shift(), this.next = n[0] || "start", "support.class")
                                                : (this.next = "", "string");
                                        },
                                        regex: ".*$",
                                        next: "start"
                                    }
                                ],
                                indentedHeredoc: [
                                    { token: "string", regex: "^	+" },
                                    {
                                        onMatch: function(e, t, n) {
                                            return e === n[1]
                                                ? (n.shift(), n.shift(), this.next = n[0] || "start", "support.class")
                                                : (this.next = "", "string");
                                        },
                                        regex: ".*$",
                                        next: "start"
                                    }
                                ]
                            }
                        },
                        {
                            regex: "$",
                            token: "empty",
                            next: function(e, t) { return t[0] === "heredoc" || t[0] === "indentedHeredoc" ? t[0] : e }
                        }, { token: "variable.language", regex: h }, { token: "variable", regex: c },
                        { token: "support.function", regex: p }, { token: "support.function", regex: f },
                        { token: "string", start: "'", end: "'" }, { token: "constant.numeric", regex: a },
                        { token: "constant.numeric", regex: t + "\\b" },
                        { token: e, regex: "[a-zA-Z_][a-zA-Z0-9_]*\\b" },
                        { token: "keyword.operator", regex: "\\+|\\-|\\*|\\*\\*|\\/|\\/\\/|~|<|>|<=|=>|=|!=" },
                        { token: "paren.lparen", regex: "[\\[\\(\\{]" }, { token: "paren.rparen", regex: "[\\]\\)\\}]" }
                    ]
                }, this.normalizeRules();
            };
        r.inherits(u, i), t.ShHighlightRules = u;
    }), define("ace/mode/folding/cstyle",
    ["require", "exports", "module", "ace/lib/oop", "ace/range", "ace/mode/folding/fold_mode"],
    function(e, t, n) {
        "use strict";
        var r = e("../../lib/oop"),
            i = e("../../range").Range,
            s = e("./fold_mode").FoldMode,
            o = t.FoldMode = function(e) {
                e &&
                (this.foldingStartMarker =
                        new RegExp(this.foldingStartMarker.source.replace(/\|[^|]*?$/, "|" + e.start)),
                    this.foldingStopMarker =
                        new RegExp(this.foldingStopMarker.source.replace(/\|[^|]*?$/, "|" + e.end)));
            };
        r.inherits(o, s), function() {
            this.foldingStartMarker = /(\{|\[)[^\}\]]*$|^\s*(\/\*)/, this.foldingStopMarker =
                /^[^\[\{]*(\}|\])|^[\s\*]*(\*\/)/, this.getFoldWidgetRange = function(e, t, n, r) {
                var i = e.getLine(n), s = i.match(this.foldingStartMarker);
                if (s) {
                    var o = s.index;
                    if (s[1]) return this.openingBracketBlock(e, s[1], n, o);
                    var u = e.getCommentFoldRange(n, o + s[0].length, 1);
                    return u && !u.isMultiLine() && (r ? u = this.getSectionRange(e, n) : t != "all" && (u = null)), u;
                }
                if (t === "markbegin") return;
                var s = i.match(this.foldingStopMarker);
                if (s) {
                    var o = s.index + s[0].length;
                    return s[1] ? this.closingBracketBlock(e, s[1], n, o) : e.getCommentFoldRange(n, o, -1);
                }
            }, this.getSectionRange = function(e, t) {
                var n = e.getLine(t), r = n.search(/\S/), s = t, o = n.length;
                t += 1;
                var u = t, a = e.getLength();
                while (++t < a) {
                    n = e.getLine(t);
                    var f = n.search(/\S/);
                    if (f === -1) continue;
                    if (r > f) break;
                    var l = this.getFoldWidgetRange(e, "all", t);
                    if (l) {
                        if (l.start.row <= s) break;
                        if (l.isMultiLine()) t = l.end.row;
                        else if (r == f) break;
                    }
                    u = t;
                }
                return new i(s, o, u, e.getLine(u).length);
            };
        }.call(o.prototype);
    }), define("ace/mode/behaviour/cstyle",
    ["require", "exports", "module", "ace/lib/oop", "ace/mode/behaviour", "ace/token_iterator", "ace/lib/lang"],
    function(e, t, n) {
        "use strict";
        var r = e("../../lib/oop"),
            i = e("../behaviour").Behaviour,
            s = e("../../token_iterator").TokenIterator,
            o = e("../../lib/lang"),
            u = ["text", "paren.rparen", "punctuation.operator"],
            a = ["text", "paren.rparen", "punctuation.operator", "comment"],
            f,
            l = {},
            c = function(e) {
                var t = -1;
                e.multiSelect &&
                (t = e.selection.index, l.rangeCount != e.multiSelect.rangeCount &&
                    (l = { rangeCount: e.multiSelect.rangeCount }));
                if (l[t]) return f = l[t];
                f = l[t] = {
                    autoInsertedBrackets: 0,
                    autoInsertedRow: -1,
                    autoInsertedLineEnd: "",
                    maybeInsertedBrackets: 0,
                    maybeInsertedRow: -1,
                    maybeInsertedLineStart: "",
                    maybeInsertedLineEnd: ""
                };
            },
            h = function() {
                this.add("braces",
                    "insertion",
                    function(e, t, n, r, i) {
                        var s = n.getCursorPosition(), u = r.doc.getLine(s.row);
                        if (i == "{") {
                            c(n);
                            var a = n.getSelectionRange(), l = r.doc.getTextRange(a);
                            if (l !== "" && l !== "{" && n.getWrapBehavioursEnabled())
                                return{ text: "{" + l + "}", selection: !1 };
                            if (h.isSaneInsertion(n, r))
                                return/[\]\}\)]/.test(u[s.column]) || n.inMultiSelectMode
                                    ? (h.recordAutoInsert(n, r, "}"), { text: "{}", selection: [1, 1] })
                                    : (h.recordMaybeInsert(n, r, "{"), { text: "{", selection: [1, 1] });
                        } else if (i == "}") {
                            c(n);
                            var p = u.substring(s.column, s.column + 1);
                            if (p == "}") {
                                var d = r.$findOpeningBracket("}", { column: s.column + 1, row: s.row });
                                if (d !== null && h.isAutoInsertedClosing(s, u, i))
                                    return h.popAutoInsertedClosing(), { text: "", selection: [1, 1] };
                            }
                        } else {
                            if (i == "\n" || i == "\r\n") {
                                c(n);
                                var v = "";
                                h.isMaybeInsertedClosing(s, u) &&
                                    (v = o.stringRepeat("}", f.maybeInsertedBrackets), h.clearMaybeInsertedClosing());
                                var p = u.substring(s.column, s.column + 1);
                                if (p === "}") {
                                    var m = r.findMatchingBracket({ row: s.row, column: s.column + 1 }, "}");
                                    if (!m) return null;
                                    var g = this.$getIndent(r.getLine(m.row));
                                } else {
                                    if (!v) {
                                        h.clearMaybeInsertedClosing();
                                        return;
                                    }
                                    var g = this.$getIndent(u);
                                }
                                var y = g + r.getTabString();
                                return{ text: "\n" + y + "\n" + g + v, selection: [1, y.length, 1, y.length] };
                            }
                            h.clearMaybeInsertedClosing();
                        }
                    }), this.add("braces",
                    "deletion",
                    function(e, t, n, r, i) {
                        var s = r.doc.getTextRange(i);
                        if (!i.isMultiLine() && s == "{") {
                            c(n);
                            var o = r.doc.getLine(i.start.row), u = o.substring(i.end.column, i.end.column + 1);
                            if (u == "}") return i.end.column++, i;
                            f.maybeInsertedBrackets--;
                        }
                    }), this.add("parens",
                    "insertion",
                    function(e, t, n, r, i) {
                        if (i == "(") {
                            c(n);
                            var s = n.getSelectionRange(), o = r.doc.getTextRange(s);
                            if (o !== "" && n.getWrapBehavioursEnabled()) return{ text: "(" + o + ")", selection: !1 };
                            if (h.isSaneInsertion(n, r))
                                return h.recordAutoInsert(n, r, ")"), { text: "()", selection: [1, 1] };
                        } else if (i == ")") {
                            c(n);
                            var u = n.getCursorPosition(),
                                a = r.doc.getLine(u.row),
                                f = a.substring(u.column, u.column + 1);
                            if (f == ")") {
                                var l = r.$findOpeningBracket(")", { column: u.column + 1, row: u.row });
                                if (l !== null && h.isAutoInsertedClosing(u, a, i))
                                    return h.popAutoInsertedClosing(), { text: "", selection: [1, 1] };
                            }
                        }
                    }), this.add("parens",
                    "deletion",
                    function(e, t, n, r, i) {
                        var s = r.doc.getTextRange(i);
                        if (!i.isMultiLine() && s == "(") {
                            c(n);
                            var o = r.doc.getLine(i.start.row), u = o.substring(i.start.column + 1, i.start.column + 2);
                            if (u == ")") return i.end.column++, i;
                        }
                    }), this.add("brackets",
                    "insertion",
                    function(e, t, n, r, i) {
                        if (i == "[") {
                            c(n);
                            var s = n.getSelectionRange(), o = r.doc.getTextRange(s);
                            if (o !== "" && n.getWrapBehavioursEnabled()) return{ text: "[" + o + "]", selection: !1 };
                            if (h.isSaneInsertion(n, r))
                                return h.recordAutoInsert(n, r, "]"), { text: "[]", selection: [1, 1] };
                        } else if (i == "]") {
                            c(n);
                            var u = n.getCursorPosition(),
                                a = r.doc.getLine(u.row),
                                f = a.substring(u.column, u.column + 1);
                            if (f == "]") {
                                var l = r.$findOpeningBracket("]", { column: u.column + 1, row: u.row });
                                if (l !== null && h.isAutoInsertedClosing(u, a, i))
                                    return h.popAutoInsertedClosing(), { text: "", selection: [1, 1] };
                            }
                        }
                    }), this.add("brackets",
                    "deletion",
                    function(e, t, n, r, i) {
                        var s = r.doc.getTextRange(i);
                        if (!i.isMultiLine() && s == "[") {
                            c(n);
                            var o = r.doc.getLine(i.start.row), u = o.substring(i.start.column + 1, i.start.column + 2);
                            if (u == "]") return i.end.column++, i;
                        }
                    }), this.add("string_dquotes",
                    "insertion",
                    function(e, t, n, r, i) {
                        if (i == '"' || i == "'") {
                            c(n);
                            var s = i, o = n.getSelectionRange(), u = r.doc.getTextRange(o);
                            if (u !== "" && u !== "'" && u != '"' && n.getWrapBehavioursEnabled())
                                return{ text: s + u + s, selection: !1 };
                            var a = n.getCursorPosition(),
                                f = r.doc.getLine(a.row),
                                l = f.substring(a.column - 1, a.column);
                            if (l == "\\") return null;
                            var p = r.getTokens(o.start.row), d = 0, v, m = -1;
                            for (var g = 0; g < p.length; g++) {
                                v = p[g], v.type == "string" ? m = -1 : m < 0 && (m = v.value.indexOf(s));
                                if (v.value.length + d > o.start.column) break;
                                d += p[g].value.length;
                            }
                            if (!v ||
                                m < 0 &&
                                v.type !== "comment" &&
                                (v.type !== "string" ||
                                    o.start.column !== v.value.length + d - 1 &&
                                    v.value.lastIndexOf(s) === v.value.length - 1)) {
                                if (!h.isSaneInsertion(n, r)) return;
                                return{ text: s + s, selection: [1, 1] };
                            }
                            if (v && v.type === "string") {
                                var y = f.substring(a.column, a.column + 1);
                                if (y == s) return{ text: "", selection: [1, 1] };
                            }
                        }
                    }), this.add("string_dquotes",
                    "deletion",
                    function(e, t, n, r, i) {
                        var s = r.doc.getTextRange(i);
                        if (!i.isMultiLine() && (s == '"' || s == "'")) {
                            c(n);
                            var o = r.doc.getLine(i.start.row), u = o.substring(i.start.column + 1, i.start.column + 2);
                            if (u == s) return i.end.column++, i;
                        }
                    });
            };
        h.isSaneInsertion = function(e, t) {
                var n = e.getCursorPosition(), r = new s(t, n.row, n.column);
                if (!this.$matchTokenType(r.getCurrentToken() || "text", u)) {
                    var i = new s(t, n.row, n.column + 1);
                    if (!this.$matchTokenType(i.getCurrentToken() || "text", u)) return!1;
                }
                return r.stepForward(), r.getCurrentTokenRow() !== n.row ||
                    this.$matchTokenType(r.getCurrentToken() || "text", a);
            }, h.$matchTokenType = function(e, t) { return t.indexOf(e.type || e) > -1 }, h.recordAutoInsert =
                function(e, t, n) {
                    var r = e.getCursorPosition(), i = t.doc.getLine(r.row);
                    this.isAutoInsertedClosing(r, i, f.autoInsertedLineEnd[0]) || (f.autoInsertedBrackets = 0),
                        f.autoInsertedRow = r.row, f.autoInsertedLineEnd =
                            n + i.substr(r.column), f.autoInsertedBrackets++;
                }, h.recordMaybeInsert = function(e, t, n) {
                var r = e.getCursorPosition(), i = t.doc.getLine(r.row);
                this.isMaybeInsertedClosing(r, i) || (f.maybeInsertedBrackets = 0), f.maybeInsertedRow =
                    r.row, f.maybeInsertedLineStart = i.substr(0, r.column) + n, f.maybeInsertedLineEnd =
                    i.substr(r.column), f.maybeInsertedBrackets++;
            }, h.isAutoInsertedClosing =
                function(e, t, n) {
                    return f.autoInsertedBrackets > 0 &&
                        e.row === f.autoInsertedRow &&
                        n === f.autoInsertedLineEnd[0] &&
                        t.substr(e.column) === f.autoInsertedLineEnd;
                }, h.isMaybeInsertedClosing =
                function(e, t) {
                    return f.maybeInsertedBrackets > 0 &&
                        e.row === f.maybeInsertedRow &&
                        t.substr(e.column) === f.maybeInsertedLineEnd &&
                        t.substr(0, e.column) == f.maybeInsertedLineStart;
                }, h.popAutoInsertedClosing =
                function() { f.autoInsertedLineEnd = f.autoInsertedLineEnd.substr(1), f.autoInsertedBrackets-- },
            h.clearMaybeInsertedClosing =
                function() { f && (f.maybeInsertedBrackets = 0, f.maybeInsertedRow = -1) }, r.inherits(h, i), t
                .CstyleBehaviour = h;
    }), define("ace/mode/sh",
    [
        "require", "exports", "module", "ace/lib/oop", "ace/mode/text", "ace/mode/sh_highlight_rules", "ace/range",
        "ace/mode/folding/cstyle", "ace/mode/behaviour/cstyle"
    ],
    function(e, t, n) {
        "use strict";
        var r = e("../lib/oop"),
            i = e("./text").Mode,
            s = e("./sh_highlight_rules").ShHighlightRules,
            o = e("../range").Range,
            u = e("./folding/cstyle").FoldMode,
            a = e("./behaviour/cstyle").CstyleBehaviour,
            f = function() { this.HighlightRules = s, this.foldingRules = new u, this.$behaviour = new a };
        r.inherits(f, i), function() {
            this.lineCommentStart = "#", this.getNextLineIndent = function(e, t, n) {
                var r = this.$getIndent(t), i = this.getTokenizer().getLineTokens(t, e), s = i.tokens;
                if (s.length && s[s.length - 1].type == "comment") return r;
                if (e == "start") {
                    var o = t.match(/^.*[\{\(\[\:]\s*$/);
                    o && (r += n);
                }
                return r;
            };
            var e = { pass: 1, "return": 1, raise: 1, "break": 1, "continue": 1 };
            this.checkOutdent = function(t, n, r) {
                if (r !== "\r\n" && r !== "\r" && r !== "\n") return!1;
                var i = this.getTokenizer().getLineTokens(n.trim(), t).tokens;
                if (!i) return!1;
                do var s = i.pop();
                while (s && (s.type == "comment" || s.type == "text" && s.value.match(/^\s+$/)));
                return s ? s.type == "keyword" && e[s.value] : !1;
            }, this.autoOutdent = function(e, t, n) {
                n += 1;
                var r = this.$getIndent(t.getLine(n)), i = t.getTabString();
                r.slice(-i.length) == i && t.remove(new o(n, r.length - i.length, n, r.length));
            }, this.$id = "ace/mode/sh";
        }.call(f.prototype), t.Mode = f;
    }), define("ace/mode/dockerfile_highlight_rules",
    ["require", "exports", "module", "ace/lib/oop", "ace/mode/sh_highlight_rules"],
    function(e, t, n) {
        "use strict";
        var r = e("../lib/oop"),
            i = e("./sh_highlight_rules").ShHighlightRules,
            s = function() {
                i.call(this);
                var e = this.$rules.start;
                for (var t = 0; t < e.length; t++)
                    if (e[t].token == "variable.language") {
                        e.splice(t,
                            0,
                            {
                                token: "constant.language",
                                regex:
                                    "(?:^(?:FROM|MAINTAINER|RUN|CMD|EXPOSE|ENV|ADD|ENTRYPOINT|VOLUME|USER|WORKDIR|ONBUILD|COPY)\\b)",
                                caseInsensitive: !0
                            });
                        break;
                    }
            };
        r.inherits(s, i), t.DockerfileHighlightRules = s;
    }), define("ace/mode/dockerfile",
    [
        "require", "exports", "module", "ace/lib/oop", "ace/mode/sh", "ace/mode/dockerfile_highlight_rules",
        "ace/mode/folding/cstyle"
    ],
    function(e, t, n) {
        "use strict";
        var r = e("../lib/oop"),
            i = e("./sh").Mode,
            s = e("./dockerfile_highlight_rules").DockerfileHighlightRules,
            o = e("./folding/cstyle").FoldMode,
            u = function() { i.call(this), this.HighlightRules = s, this.foldingRules = new o };
        r.inherits(u, i), function() { this.$id = "ace/mode/dockerfile" }.call(u.prototype), t.Mode = u;
    })