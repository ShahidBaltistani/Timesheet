/*
 * Globalize Culture te
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

    Globalize.addCultureInfo("te",
        "default",
        {
            name: "te",
            englishName: "Telugu",
            nativeName: "తెలుగు",
            language: "te",
            numberFormat: {
                groupSizes: [3, 2],
                percent: {
                    groupSizes: [3, 2]
                },
                currency: {
                    pattern: ["$ -n", "$ n"],
                    groupSizes: [3, 2],
                    symbol: "రూ"
                }
            },
            calendars: {
                standard: {
                    "/": "-",
                    firstDay: 1,
                    days: {
                        names: ["ఆదివారం", "సోమవారం", "మంగళవారం", "బుధవారం", "గురువారం", "శుక్రవారం", "శనివారం"],
                        namesAbbr: ["ఆది.", "సోమ.", "మంగళ.", "బుధ.", "గురు.", "శుక్ర.", "శని."],
                        namesShort: ["ఆ", "సో", "మం", "బు", "గు", "శు", "శ"]
                    },
                    months: {
                        names: [
                            "జనవరి", "ఫిబ్రవరి", "మార్చి", "ఏప్రిల్", "మే", "జూన్", "జూలై", "ఆగస్టు", "సెప్టెంబర్",
                            "అక్టోబర్", "నవంబర్", "డిసెంబర్", ""
                        ],
                        namesAbbr: [
                            "జనవరి", "ఫిబ్రవరి", "మార్చి", "ఏప్రిల్", "మే", "జూన్", "జూలై", "ఆగస్టు", "సెప్టెంబర్",
                            "అక్టోబర్", "నవంబర్", "డిసెంబర్", ""
                        ]
                    },
                    AM: ["పూర్వాహ్న", "పూర్వాహ్న", "పూర్వాహ్న"],
                    PM: ["అపరాహ్న", "అపరాహ్న", "అపరాహ్న"],
                    patterns: {
                        d: "dd-MM-yy",
                        D: "dd MMMM yyyy",
                        t: "HH:mm",
                        T: "HH:mm:ss",
                        f: "dd MMMM yyyy HH:mm",
                        F: "dd MMMM yyyy HH:mm:ss",
                        M: "dd MMMM"
                    }
                }
            }
        });

}(this));