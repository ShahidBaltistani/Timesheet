/*
 * Globalize Culture en-GB
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

    Globalize.addCultureInfo("en-GB",
        "default",
        {
            name: "en-GB",
            englishName: "English (United Kingdom)",
            nativeName: "English (United Kingdom)",
            numberFormat: {
                currency: {
                    pattern: ["-$n", "$n"],
                    symbol: "£"
                }
            },
            calendars: {
                standard: {
                    firstDay: 1,
                    patterns: {
                        d: "dd/MM/yyyy",
                        D: "dd MMMM yyyy",
                        t: "HH:mm",
                        T: "HH:mm:ss",
                        f: "dd MMMM yyyy HH:mm",
                        F: "dd MMMM yyyy HH:mm:ss",
                        M: "dd MMMM",
                        Y: "MMMM yyyy"
                    }
                }
            }
        });

}(this));