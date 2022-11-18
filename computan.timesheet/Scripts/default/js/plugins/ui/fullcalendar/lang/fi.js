(function(e) { "function" == typeof define && define.amd ? define(["jquery", "moment"], e) : e(jQuery, moment) })(
    function(e, t) {
        function n(e, t, n, r) {
            var s = "";
            switch (n) {
            case"s":
                return r ? "muutaman sekunnin" : "muutama sekunti";
            case"m":
                return r ? "minuutin" : "minuutti";
            case"mm":
                s = r ? "minuutin" : "minuuttia";
                break;
            case"h":
                return r ? "tunnin" : "tunti";
            case"hh":
                s = r ? "tunnin" : "tuntia";
                break;
            case"d":
                return r ? "päivän" : "päivä";
            case"dd":
                s = r ? "päivän" : "päivää";
                break;
            case"M":
                return r ? "kuukauden" : "kuukausi";
            case"MM":
                s = r ? "kuukauden" : "kuukautta";
                break;
            case"y":
                return r ? "vuoden" : "vuosi";
            case"yy":
                s = r ? "vuoden" : "vuotta";
            }
            return s = i(e, r) + " " + s;
        }

        function i(e, t) { return 10 > e ? t ? s[e] : r[e] : e }

        var r = "nolla yksi kaksi kolme neljä viisi kuusi seitsemän kahdeksan yhdeksän".split(" "),
            s = ["nolla", "yhden", "kahden", "kolmen", "neljän", "viiden", "kuuden", r[7], r[8], r[9]];
        (t.defineLocale || t.lang).call(t,
            "fi",
            {
                months:
                    "tammikuu_helmikuu_maaliskuu_huhtikuu_toukokuu_kesäkuu_heinäkuu_elokuu_syyskuu_lokakuu_marraskuu_joulukuu"
                        .split("_"),
                monthsShort: "tammi_helmi_maalis_huhti_touko_kesä_heinä_elo_syys_loka_marras_joulu".split("_"),
                weekdays: "sunnuntai_maanantai_tiistai_keskiviikko_torstai_perjantai_lauantai".split("_"),
                weekdaysShort: "su_ma_ti_ke_to_pe_la".split("_"),
                weekdaysMin: "su_ma_ti_ke_to_pe_la".split("_"),
                longDateFormat:
                {
                    LT: "HH.mm",
                    LTS: "HH.mm.ss",
                    L: "DD.MM.YYYY",
                    LL: "Do MMMM[ta] YYYY",
                    LLL: "Do MMMM[ta] YYYY, [klo] LT",
                    LLLL: "dddd, Do MMMM[ta] YYYY, [klo] LT",
                    l: "D.M.YYYY",
                    ll: "Do MMM YYYY",
                    lll: "Do MMM YYYY, [klo] LT",
                    llll: "ddd, Do MMM YYYY, [klo] LT"
                },
                calendar: {
                    sameDay: "[tänään] [klo] LT",
                    nextDay: "[huomenna] [klo] LT",
                    nextWeek: "dddd [klo] LT",
                    lastDay: "[eilen] [klo] LT",
                    lastWeek: "[viime] dddd[na] [klo] LT",
                    sameElse: "L"
                },
                relativeTime: {
                    future: "%s päästä",
                    past: "%s sitten",
                    s: n,
                    m: n,
                    mm: n,
                    h: n,
                    hh: n,
                    d: n,
                    dd: n,
                    M: n,
                    MM: n,
                    y: n,
                    yy: n
                },
                ordinalParse: /\d{1,2}\./,
                ordinal: "%d.",
                week: { dow: 1, doy: 4 }
            }), e.fullCalendar.datepickerLang("fi",
            "fi",
            {
                closeText: "Sulje",
                prevText: "&#xAB;Edellinen",
                nextText: "Seuraava&#xBB;",
                currentText: "Tänään",
                monthNames:
                [
                    "Tammikuu", "Helmikuu", "Maaliskuu", "Huhtikuu", "Toukokuu", "Kesäkuu", "Heinäkuu", "Elokuu",
                    "Syyskuu", "Lokakuu", "Marraskuu", "Joulukuu"
                ],
                monthNamesShort:
                [
                    "Tammi", "Helmi", "Maalis", "Huhti", "Touko", "Kesä", "Heinä", "Elo", "Syys", "Loka", "Marras",
                    "Joulu"
                ],
                dayNamesShort: ["Su", "Ma", "Ti", "Ke", "To", "Pe", "La"],
                dayNames: ["Sunnuntai", "Maanantai", "Tiistai", "Keskiviikko", "Torstai", "Perjantai", "Lauantai"],
                dayNamesMin: ["Su", "Ma", "Ti", "Ke", "To", "Pe", "La"],
                weekHeader: "Vk",
                dateFormat: "d.m.yy",
                firstDay: 1,
                isRTL: !1,
                showMonthAfterYear: !1,
                yearSuffix: ""
            }), e.fullCalendar.lang("fi",
            {
                defaultButtonText: { month: "Kuukausi", week: "Viikko", day: "Päivä", list: "Tapahtumat" },
                allDayText: "Koko päivä",
                eventLimitText: "lisää"
            });
    });