/*
 * Globalize Culture af
 *
 * http://github.com/jquery/globalize
 *
 * Copyright Software Freedom Conservancy, Inc.
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * This file was generated by the Globalize Culture Generator
 * Translation: bugs found in this file need to be fixed in the generator
 */

(function(window, undefined) {

    var Globalize;

    if (typeof require !== "undefined" &&
        typeof exports !== "undefined" &&
        typeof module !== "undefined") {
        // Assume CommonJS
        Globalize = require("globalize");
    } else {
        // Global variable
        Globalize = window.Globalize;
    }

    Globalize.addCultureInfo("af",
        "default",
        {
            name: "af",
            englishName: "Afrikaans",
            nativeName: "Afrikaans",
            language: "af",
            numberFormat: {
                percent: {
                    pattern: ["-n%", "n%"]
                },
                currency: {
                    pattern: ["$-n", "$ n"],
                    symbol: "R"
                }
            },
            calendars: {
                standard: {
                    days: {
                        names: ["Sondag", "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrydag", "Saterdag"],
                        namesAbbr: ["Son", "Maan", "Dins", "Woen", "Dond", "Vry", "Sat"],
                        namesShort: ["So", "Ma", "Di", "Wo", "Do", "Vr", "Sa"]
                    },
                    months: {
                        names: [
                            "Januarie", "Februarie", "Maart", "April", "Mei", "Junie", "Julie", "Augustus", "September",
                            "Oktober", "November", "Desember", ""
                        ],
                        namesAbbr: [
                            "Jan", "Feb", "Mar", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Des", ""
                        ]
                    },
                    patterns: {
                        d: "yyyy/MM/dd",
                        D: "dd MMMM yyyy",
                        t: "hh:mm tt",
                        T: "hh:mm:ss tt",
                        f: "dd MMMM yyyy hh:mm tt",
                        F: "dd MMMM yyyy hh:mm:ss tt",
                        M: "dd MMMM",
                        Y: "MMMM yyyy"
                    }
                }
            }
        });

}(this));