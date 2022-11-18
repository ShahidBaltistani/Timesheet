(function(t) { "function" == typeof define && define.amd ? define(["jquery", "moment"], t) : t(jQuery, moment) })(
    function(t, e) {
        function n(t) { return t > 1 && 5 > t && 1 !== ~~(t / 10) }

        function i(t, e, i, r) {
            var s = t + " ";
            switch (i) {
            case"s":
                return e || r ? "pár sekund" : "pár sekundami";
            case"m":
                return e ? "minuta" : r ? "minutu" : "minutou";
            case"mm":
                return e || r ? s + (n(t) ? "minuty" : "minut") : s + "minutami";
            case"h":
                return e ? "hodina" : r ? "hodinu" : "hodinou";
            case"hh":
                return e || r ? s + (n(t) ? "hodiny" : "hodin") : s + "hodinami";
            case"d":
                return e || r ? "den" : "dnem";
            case"dd":
                return e || r ? s + (n(t) ? "dny" : "dní") : s + "dny";
            case"M":
                return e || r ? "měsíc" : "měsícem";
            case"MM":
                return e || r ? s + (n(t) ? "měsíce" : "měsíců") : s + "měsíci";
            case"y":
                return e || r ? "rok" : "rokem";
            case"yy":
                return e || r ? s + (n(t) ? "roky" : "let") : s + "lety";
            }
        }

        var r = "leden_únor_březen_duben_květen_červen_červenec_srpen_září_říjen_listopad_prosinec".split("_"),
            s = "led_úno_bře_dub_kvě_čvn_čvc_srp_zář_říj_lis_pro".split("_");
        (e.defineLocale || e.lang).call(e,
            "cs",
            {
                months: r,
                monthsShort: s,
                monthsParse: function(t, e) {
                    var n, i = [];
                    for (n = 0; 12 > n; n++) i[n] = RegExp("^" + t[n] + "$|^" + e[n] + "$", "i");
                    return i;
                }(r, s),
                weekdays: "neděle_pondělí_úterý_středa_čtvrtek_pátek_sobota".split("_"),
                weekdaysShort: "ne_po_út_st_čt_pá_so".split("_"),
                weekdaysMin: "ne_po_út_st_čt_pá_so".split("_"),
                longDateFormat: {
                    LT: "H:mm",
                    LTS: "LT:ss",
                    L: "DD.MM.YYYY",
                    LL: "D. MMMM YYYY",
                    LLL: "D. MMMM YYYY LT",
                    LLLL: "dddd D. MMMM YYYY LT"
                },
                calendar: {
                    sameDay: "[dnes v] LT",
                    nextDay: "[zítra v] LT",
                    nextWeek: function() {
                        switch (this.day()) {
                        case 0:
                            return"[v neděli v] LT";
                        case 1:
                        case 2:
                            return"[v] dddd [v] LT";
                        case 3:
                            return"[ve středu v] LT";
                        case 4:
                            return"[ve čtvrtek v] LT";
                        case 5:
                            return"[v pátek v] LT";
                        case 6:
                            return"[v sobotu v] LT";
                        }
                    },
                    lastDay: "[včera v] LT",
                    lastWeek: function() {
                        switch (this.day()) {
                        case 0:
                            return"[minulou neděli v] LT";
                        case 1:
                        case 2:
                            return"[minulé] dddd [v] LT";
                        case 3:
                            return"[minulou středu v] LT";
                        case 4:
                        case 5:
                            return"[minulý] dddd [v] LT";
                        case 6:
                            return"[minulou sobotu v] LT";
                        }
                    },
                    sameElse: "L"
                },
                relativeTime: {
                    future: "za %s",
                    past: "před %s",
                    s: i,
                    m: i,
                    mm: i,
                    h: i,
                    hh: i,
                    d: i,
                    dd: i,
                    M: i,
                    MM: i,
                    y: i,
                    yy: i
                },
                ordinalParse: /\d{1,2}\./,
                ordinal: "%d.",
                week: { dow: 1, doy: 4 }
            }), t.fullCalendar.datepickerLang("cs",
            "cs",
            {
                closeText: "Zavřít",
                prevText: "&#x3C;Dříve",
                nextText: "Později&#x3E;",
                currentText: "Nyní",
                monthNames:
                [
                    "leden", "únor", "březen", "duben", "květen", "červen", "červenec", "srpen", "září", "říjen",
                    "listopad", "prosinec"
                ],
                monthNamesShort: ["led", "úno", "bře", "dub", "kvě", "čer", "čvc", "srp", "zář", "říj", "lis", "pro"],
                dayNames: ["neděle", "pondělí", "úterý", "středa", "čtvrtek", "pátek", "sobota"],
                dayNamesShort: ["ne", "po", "út", "st", "čt", "pá", "so"],
                dayNamesMin: ["ne", "po", "út", "st", "čt", "pá", "so"],
                weekHeader: "Týd",
                dateFormat: "dd.mm.yy",
                firstDay: 1,
                isRTL: !1,
                showMonthAfterYear: !1,
                yearSuffix: ""
            }), t.fullCalendar.lang("cs",
            {
                defaultButtonText: { month: "Měsíc", week: "Týden", day: "Den", list: "Agenda" },
                allDayText: "Celý den",
                eventLimitText: function(t) { return"+další: " + t }
            });
    });