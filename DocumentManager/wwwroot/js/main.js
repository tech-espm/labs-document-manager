﻿"use strict";

window.isEmpty = function (x) {
	return (x === undefined || x === null);
};
window.seal$ = function (obj) {
	if (Object.seal)
		Object.seal(obj);
	return obj;
};
window.freeze$ = function (obj) {
	if (Object.freeze)
		Object.freeze(obj);
	return obj;
};
window._ = function (id) {
	return ((typeof id === "string") ? document.getElementById(id) : id);
};
window.cancelEvent = function (evt) {
	if (evt) {
		if ("isCancelled" in evt)
			evt.isCancelled = true;
		if ("preventDefault" in evt)
			evt.preventDefault();
		if ("stopPropagation" in evt)
			evt.stopPropagation();
	}
	return false;
};
window.parseQueryString = function () {
	var i, pair, assoc = {}, keyValues = location.search.substring(1).split("&");
	for (i in keyValues) {
		pair = keyValues[i].split("=");
		if (pair.length > 1) {
			assoc[decodeURIComponent(pair[0].replace(/\+/g, " "))] = decodeURIComponent(pair[1].replace(/\+/g, " "));
		}
	}
	window.queryString = assoc;
	return assoc;
};
window.encode = (function () {
	var lt = /</g, gt = />/g;
	return function (x) {
		return x.replace(lt, "&lt;").replace(gt, "&gt;");
	};
})();
window.normalize = function (x) {
	var i, c, prev, sb = trim(x).toLocaleUpperCase(), ret = "";
	if (!sb.length)
		return "";
	for (i = 0; i < sb.length; i++) {
		switch (c = sb.charAt(i)) {
			case '~':
			case '`':
			case '´':
			case '·':
			case '¸':
			case '¡':
			case '!':
			case '@':
			case '#':
			case '$':
			case '%':
			case '^':
			case '&':
			case '*':
			case '(':
			case ')':
			case '=':
			case '{':
			case '}':
			case '|':
			case '[':
			case ']':
			case '\\':
			case ':':
			case '\"':
			case ';':
			case '\'':
			case '<':
			case '>':
			case '?':
			case ',':
			case '.':
			case '/':
			case '¿':
			case '¶':
			case '»':
			case '«':
			case '÷':
			case '¢':
			case '¤':
			case '£':
			case '¥':
			case '§':
			case '¦':
			case '¬':
			case '±':
			case '×':
			case '®':
			case '©':
			//sb.Remove(i, 1);
			//i--;
			//break;
			case '\t':
			case '\n':
			case '\r':
			case '_':
			case '¯':
			case '+':
			case '-':
			case '\xAD': //hifen
			case '\xA0': //nbsp
				ret += ' ';
				break;
			case '¹':
				ret += '1';
				break;
			case '²':
				ret += '2';
				break;
			case '³':
				ret += '3';
				break;
			case '¼':
				ret += '14';
				break;
			case '½':
				ret += '12';
				break;
			case '¾':
				ret += '34';
				break;
			case 'Þ':
				ret += 'T';
				break;
			case 'Ñ':
				ret += 'N';
				break;
			case 'ß':
				ret += 'S';
				break;
			case 'Ç':
				ret += 'C';
				break;
			case 'Æ':
				ret += 'AE';
				break;
			case 'Á':
			case 'Ä':
			case 'À':
			case 'Ã':
			case 'Â':
			case 'Å':
			case 'ª':
				ret += 'A';
				break;
			case 'É':
			case 'Ë':
			case 'È':
			case 'Ê':
				ret += 'E';
				break;
			case 'Í':
			case 'Ï':
			case 'Ì':
			case 'Î':
			case 'Y':
			case 'Ý':
			case 'Ÿ':
				ret += 'I';
				break;
			case 'Ó':
			case 'Ö':
			case 'Ò':
			case 'Ô':
			case 'Õ':
			case 'Ø':
			case '°':
			case 'º':
				ret += 'O';
				break;
			case 'Ú':
			case 'Ü':
			case 'Ù':
			case 'Û':
			case 'Μ':
			case 'W':
			case 'V':
				ret += 'U';
				break;
			case 'Ð':
				ret += 'D';
				break;
			/*case 'g':
				ret += 'g';
				if (i < sb.length - 1 && sb.charAt(i + 1) == 'u')
					i++;
				break;
			case 'q':
				ret += 'k';
				if (i < sb.length - 1 && sb.charAt(i + 1) == 'u')
					i++;
				break;
			case 's':
			case 'c':
				if (i < sb.length - 1) {
					if (sb.charAt(i + 1) == 'h') {
						ret += 'x';
						i++;
					} else if (sb.charAt(i + 1) == 'c' || sb.charAt(i + 1) == 's') {
						ret += 's';
						i++;
					} else {
						ret += c;
					}
				} else {
					ret += c;
				}
				break;
			case 'p':
				if (i < sb.length - 1 && sb.charAt(i + 1) == 'h') {
					ret += 'f';
					i++;
				} else {
					ret += c;
				}
				break;
			case 't':
				if (i < sb.length - 1 && sb.charAt(i + 1) == 'h') {
					ret += 'd';
					i++;
				} else if (i < sb.length - 2 && sb.charAt(i + 1) == 'c' && sb.charAt(i + 2) == 'h') {
					ret += 't';
					i += 2;
				} else {
					ret += c;
				}
				break;
			case 'x':
				if (i < sb.length - 1 && sb.charAt(i + 1) == 'c') {
					ret += 's';
					i++;
				} else {
					ret += c;
				}
				break;*/
			default:
				ret += c;
				break;
		}
	}
	//remove repeated chars
	prev = ret.charAt(0);
	sb = prev;
	for (i = 1; i < ret.length; i++) {
		if ((c = ret.charAt(i)) !== prev)
			sb += c;
		prev = c;
	}
	return trim(sb);
	//phonetic issues
	/*ret = "";
	for (i = 0; i < sb.length; i++) {
		switch (c = sb.charAt(i)) {
			case 'h':
				break;
			case 'l':
				ret += 'u';
				break;
			default:
				ret += c;
				break;
		}
	}
	return trim(ret);*/
};
window.customFilterHandler = function (table, input) {
	var lastSearch = "", handler = function () {
		var s = normalize(input.value);
		if (lastSearch !== s) {
			lastSearch = s;
			table.search(s).draw();
		}
		return true;
	};
	input.onchange = handler;
	input.onkeyup = handler;
};
window.customFilterHandlerPlain = function (table, input) {
	var lastSearch = "", handler = function () {
		var s = input.value;
		if (lastSearch !== s) {
			lastSearch = s;
			table.search(s).draw();
		}
		return true;
	};
	input.onchange = handler;
	input.onkeyup = handler;
};
window.prepareCustomFilter = function (table, tableId, customFilterLabel, placeholder) {
	var label, input, parent = _(tableId + "_filter");
	if (parent) {
		while (parent.firstChild)
			parent.removeChild(parent.firstChild);
		label = document.createElement("label");
		label.appendChild(document.createTextNode((customFilterLabel === null || customFilterLabel === undefined) ? "Filtro:" : customFilterLabel));
		input = document.createElement("input");
		if (window.prepareCustomFilterPlain)
			customFilterHandlerPlain(table, input);
		else
			customFilterHandler(table, input);
		input.className = "form-control input-sm upper";
		input.setAttribute("type", "search");
		input.setAttribute("placeholder", placeholder || "");
		input.setAttribute("aria-controls", tableId);
		label.appendChild(input);
		parent.appendChild(label);
	}
};
window.format2 = function (x) {
	return ((x < 10) ? ("0" + x) : x);
};
window.formatPeriod = function (period) {
	return (period < 60 ? (period + " minutos") : (period == 60 ? "1 hora" : (((period / 60) | 0) + " horas")));
};
window.formatDuration = function (duration) {
	var s = ((duration / 1000) | 0), m = ((s / 60) | 0);
	s = s % 60;
	return format2(m) + ":" + format2(s);
};
window.formatSize = (function () {
	var expr = /\B(?=(\d{3})+(?!\d))/g, thousands = (window.currentLanguageId === 1 ? "." : ",");
	window.formatSizeLong = function (size) {
		//if (size < 16384)
		//	return size + " bytes";
		//return ((size * 0.0009765625) | 0).toString().replace(expr, ".") + " KB";
		if (size) {
			size = (size * 0.0009765625) | 0;
			if (size <= 0)
				size = 1;
		}
		return size.toString().replace(expr, thousands) + " KB";
	};
	return function (size) {
		//if (size < 16384)
		//	return size + " bytes";
		//return (size >>> 10).toString().replace(expr, ".") + " KB";
		if (size) {
			size >>>= 10;
			if (size <= 0)
				size = 1;
		}
		return size.toString().replace(expr, thousands) + " KB";
	};
})();
window.formatNumber = (function () {
	var expr = /\B(?=(\d{3})+(?!\d))/g;
	return function (x) {
		return x.toString().replace(expr, ".");
	};
})();
window.formatHour = function (x) {
	return format2(x >>> 6) + ":" + format2(x & 63);
};
//https://github.com/igorescobar/jQuery-Mask-Plugin
//https://igorescobar.github.io/jQuery-Mask-Plugin/
window.maskCNPJ = function (field) {
	$(field).mask("00.000.000/0000-00");
};
window.maskCPF = function (field) {
	$(field).mask("000.000.000-00");
};
window.maskPhone = function (field) {
	$(field).mask("(00) 0000-0000JJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ", { translation: { "J": { pattern: /[\d\D]/g } } });
};
window.maskHour = function (field) {
	$(field).mask("00:00");
};
window.addFilterButton = function (parent, icon, text, handler, title, btnClass) {
	var p = _(parent), label, btn, i;
	if (!p)
		return;
	label = document.createElement("label");
	btn = document.createElement("button");
	btn.setAttribute("type", "button");
	btn.className = "btn btn-outline btn-sm " + (btnClass || "btn-default");
	i = document.createElement("i");
	i.className = "fa fa14 " + icon;
	btn.appendChild(i);
	if (text)
		btn.appendChild(document.createTextNode(text));
	if (title)
		btn.setAttribute("title", title);
	btn.onclick = handler;
	label.appendChild(btn);
	p.insertBefore(document.createTextNode(" "), p.firstChild);
	p.insertBefore(label, p.firstChild);
	return btn;
};
window.selectRow = function (row, selected) {
	if (selected) {
		row.style.background = "#b8f7b8";
		$(row).addClass("included-row");
	} else {
		row.style.background = "";
		$(row).removeClass("included-row");
	}
};
window.enableTableUIForMultipleSelection = function (id, enable) {
	var i, lis, li, dlg;

	if ((dlg = _(id)) && (lis = dlg.getElementsByTagName("thead")).length) {
		if (!enable) {
			lis[0].style.opacity = "0.5";
			lis[0].style.pointerEvents = "none";
		} else {
			lis[0].style.opacity = "";
			lis[0].style.pointerEvents = "";
		}
	}

	if ((dlg = _(id + "_paginate")) && (lis = dlg.getElementsByTagName("li")).length) {
		if (!enable) {
			for (i = lis.length - 1; i >= 0; i--) {
				li = $(lis[i]);
				if (li.hasClass("disabled")) {
					li[0].removeAttribute("data-disabled");
				} else {
					li[0].setAttribute("data-disabled", "true");
					li.addClass("disabled");
				}
			}
		} else {
			for (i = lis.length - 1; i >= 0; i--) {
				li = $(lis[i]);
				if (li[0].getAttribute("data-disabled")) {
					li[0].removeAttribute("data-disabled");
					li.removeClass("disabled");
				}
			}
		}
	}

	if ((dlg = _(id + "_length")) && (lis = dlg.getElementsByTagName("select")).length) {
		if (!enable)
			lis[0].setAttribute("disabled", "disabled");
		else
			lis[0].removeAttribute("disabled");
	}

	if ((dlg = _(id + "_filter")) && (lis = dlg.getElementsByTagName("input")).length) {
		if (!enable)
			lis[0].setAttribute("disabled", "disabled");
		else
			lis[0].removeAttribute("disabled");
	}
};
window.toggleMultipleSelection = (function () {
	var firstShift = null, firstShiftWasChecking = false;
	window.resetMultipleSelection = function () {
		firstShift = null;
		firstShiftWasChecking = false;
	};

	return function (e) {
		if (e.button || JsonWebApi.active)
			return;
		var i, row, dtrows, rows, first, last;
		if (firstShift) {
			enableTableUIForMultipleSelection("dataTableMain", true);
			if (document.getSelection && (i = document.getSelection()).removeAllRanges)
				i.removeAllRanges();
			dtrows = dataTableMain.rows({ order: "current" });
			rows = dtrows.nodes();
			first = rows.indexOf(firstShift);
			last = rows.indexOf(this.parentNode.parentNode);
			if (first > last) {
				i = first;
				first = last;
				last = i;
			}
			firstShift = null;
			for (i = first; i <= last; i++) {
				row = rows[i];
				row.getElementsByTagName("input")[0].checked = firstShiftWasChecking;
				selectRow(row, firstShiftWasChecking);
			}
			return;
		}

		selectRow(this.parentNode.parentNode, this.checked);

		if (e.shiftKey) {
			enableTableUIForMultipleSelection("dataTableMain", false);
			if (document.getSelection && (i = document.getSelection()).removeAllRanges)
				i.removeAllRanges();
			firstShift = this.parentNode.parentNode;
			firstShiftWasChecking = this.checked;
		}
	};
})();
window.prepareDataTableMain = (function () {
	var lastUl = null, lastUlParent = null, wrapper = null, justShown = false, docOk = false,
		lastBootstrap = null,
		closeLastBootstrap = function () {
			if (lastBootstrap) {
				if (lastBootstrap.getAttribute("aria-expanded") == "true") {
					$(lastBootstrap.parentNode).removeClass("open");
					lastBootstrap.setAttribute("aria-expanded", "false");
					lastBootstrap = null;
					return true;
				}
				lastBootstrap = null;
			}
			return false;
		},
		docHandler = function () {
			if (!lastUl)
				return;
			if (justShown) {
				justShown = false;
				return;
			}
			lastUl.style.display = "";
			lastUl.style.width = "";
			lastUl.style.left = "";
			lastUl.style.top = "";
			lastUl.style.right = "";
			lastUl.style.bottom = "";
			lastUl.parentNode.removeChild(lastUl);
			lastUlParent.appendChild(lastUl);
			lastUl = null;
			lastUlParent = null;
			if (wrapper)
				wrapper.style.position = "";
		};

	return function (menu, menuItemCount) {
		$("#dataTableMain > tbody").on("click", "tr", function (e) {
			var a, ul, rect, rect2, x, y;

			switch (e.target.tagName) {
				case "A":
				case "INPUT":
					return;
				case "I":
					if (e.target.parentNode.tagName != "BUTTON")
						return;
				case "BUTTON":
					if (menu) {
						if (menuItemCount > 3) {
							ul = this.getElementsByTagName("ul");
							if (!ul || !(ul = ul[0]))
								return;
							ul.style.bottom = (this.parentNode.getElementsByTagName("tr")[0] === this ? "-28px" : "");
						}
						closeLastBootstrap();
						lastBootstrap = ((e.target.tagName == "I") ? e.target.parentNode : e.target);
					}
					return;
				case "TD":
					ul = e.target.getElementsByTagName("input");
					if (e.button || !ul || ul.length !== 1 || ul[0].getAttribute("type") !== "checkbox")
						break;
					if (e.shiftKey) {
						if (("MouseEvent" in window)) {
							ul[0].dispatchEvent(new MouseEvent("click", {
								bubbles: true,
								cancelable: true,
								shiftKey: true
							}));
						} else if (("createEvent" in document) &&
							(a = document.createEvent("MouseEvent")) &&
							("initMouseEvent" in a)) {
							a.initMouseEvent("click", true, true, window, 0, 0, 0, 0, 0, false, false, true, false, 0, null);
							ul[0].dispatchEvent(a);
						} else {
							ul[0].click();
						}
					} else {
						ul[0].click();
					}
					return;
			}

			if (menu) {
				if (!docOk) {
					document.body.addEventListener("click", docHandler, false);
					docOk = true;
				}
				if (!wrapper)
					wrapper = document.body;//_("wrapper");
				ul = this.getElementsByTagName("ul");
				if (closeLastBootstrap())
					return;
				if (lastUl) {
					docHandler();
					return;
				}
				if (!ul || !(ul = ul[0]))
					return;
				if (menuItemCount > 3)
					ul.style.bottom = "";
				lastUl = ul;
				lastUlParent = ul.parentNode;
				wrapper.style.position = "relative";
				rect = wrapper.getBoundingClientRect();
				x = ((e.originalEvent.clientX - rect.left) | 0);
				//if ((x + 160) >= rect.right)
				//	x = rect.right - 161;
				ul.style.width = "auto";
				ul.style.left = x + "px";
				ul.style.top = (y = ((e.originalEvent.clientY - rect.top - 2) | 0)) + "px";
				ul.style.right = "auto";
				ul.style.bottom = "auto";
				ul.parentNode.removeChild(ul);
				wrapper.appendChild(ul);
				ul.style.display = "block";
				rect2 = ul.getBoundingClientRect();
				if (rect2.right >= rect.right) {
					x -= (rect2.right - rect.right) + 1;
					ul.style.left = x + "px";
				}
				if (rect2.bottom >= rect.bottom) {
					y -= (rect2.bottom - rect.bottom) + 1;
					ul.style.top = y + "px";
				}
				justShown = true;
				return;// cancelEvent(e);
			} else {
				a = this.getElementsByTagName("a");
				if (!a || !a.length)
					a = this.getElementsByTagName("button");
				if (a && a.length) {
					x = a.length - 1;
					if (x && a[x].getAttribute("data-delete") == "1")
						x--;
					a[x].click();
				}
			}
		});
	};
})();
if (window.currentLanguageId === 1) {
	window.months = ["JAN/", "FEB/", "MAR/", "APR/", "MAY/", "JUN/", "JUL/", "AUG/", "SEP/", "OCT/", "NOV/", "DEC/"];
	window.monthsToInt = { JAN: 1, FEB: 2, MAR: 3, APR: 4, MAY: 5, JUN: 6, JUL: 7, AUG: 8, SEP: 9, OCT: 10, NOV: 11, DEC: 12 };
	window.formatDateTime = function (utcTicks) {
		//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date
		//Date has 2 versions for the methods: getXXX() and getUTCXXX()
		//The constructor always takes the ticks since epoch at UTC
		//
		//The getTime() method returns the numeric value corresponding to the time for the specified date according to universal time.
		//getTime() always uses UTC for time representation.For example, a client browser in one timezone, getTime() will be the same as a client browser in any other timezone.
		var d = new Date(utcTicks);
		return months[d.getUTCMonth()] + format2(d.getUTCDate()) + "/" + d.getUTCFullYear() + " at " + format2(d.getUTCHours()) + ":" + format2(d.getUTCMinutes());
		//var tmp = (utcTicks / (64 * 64 * 32)) | 0;
		//return format2(tmp & 31) + months[(tmp >>> 5) & 15] + (tmp >>> 9) + " às " + format2((tmp >>> 12) & 31) + ":" + format2((tmp >>> 6) & 63);
	};
	window.formatDate = function (utcTicks) {
		//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date
		//Date has 2 versions for the methods: getXXX() and getUTCXXX()
		//The constructor always takes the ticks since epoch at UTC
		//
		//The getTime() method returns the numeric value corresponding to the time for the specified date according to universal time.
		//getTime() always uses UTC for time representation.For example, a client browser in one timezone, getTime() will be the same as a client browser in any other timezone.
		var d = new Date(utcTicks);
		return months[d.getUTCMonth()] + format2(d.getUTCDate()) + "/" + d.getUTCFullYear();
		//var tmp = (utcTicks / (64 * 64 * 32)) | 0;
		//return format2(tmp & 31) + months[(tmp >>> 5) & 15] + (tmp >>> 9);
	};
} else {
	window.months = ["/JAN/", "/FEV/", "/MAR/", "/ABR/", "/MAI/", "/JUN/", "/JUL/", "/AGO/", "/SET/", "/OUT/", "/NOV/", "/DEZ/"];
	window.monthsToInt = { JAN: 1, FEV: 2, MAR: 3, ABR: 4, MAI: 5, JUN: 6, JUL: 7, AGO: 8, SET: 9, OUT: 10, NOV: 11, DEZ: 12 };
	window.formatDateTime = function (utcTicks) {
		//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date
		//Date has 2 versions for the methods: getXXX() and getUTCXXX()
		//The constructor always takes the ticks since epoch at UTC
		//
		//The getTime() method returns the numeric value corresponding to the time for the specified date according to universal time.
		//getTime() always uses UTC for time representation.For example, a client browser in one timezone, getTime() will be the same as a client browser in any other timezone.
		var d = new Date(utcTicks);
		return format2(d.getUTCDate()) + months[d.getUTCMonth()] + d.getUTCFullYear() + " às " + format2(d.getUTCHours()) + ":" + format2(d.getUTCMinutes());
		//var tmp = (utcTicks / (64 * 64 * 32)) | 0;
		//return format2(tmp & 31) + months[(tmp >>> 5) & 15] + (tmp >>> 9) + " às " + format2((tmp >>> 12) & 31) + ":" + format2((tmp >>> 6) & 63);
	};
	window.formatDate = function (utcTicks) {
		//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date
		//Date has 2 versions for the methods: getXXX() and getUTCXXX()
		//The constructor always takes the ticks since epoch at UTC
		//
		//The getTime() method returns the numeric value corresponding to the time for the specified date according to universal time.
		//getTime() always uses UTC for time representation.For example, a client browser in one timezone, getTime() will be the same as a client browser in any other timezone.
		var d = new Date(utcTicks);
		return format2(d.getUTCDate()) + months[d.getUTCMonth()] + d.getUTCFullYear();
		//var tmp = (utcTicks / (64 * 64 * 32)) | 0;
		//return format2(tmp & 31) + months[(tmp >>> 5) & 15] + (tmp >>> 9);
	};
}
window.parseDateInput = function (input, setAtEndOfDay) {
	var date = trimValue(input).toUpperCase(), day, month, year, literalMonth = false, i;
	if (!(new RegExp("[0-9][0-9]?/[0-9][0-9]?/[0-9][0-9][0-9][0-9]")).test(date)) {
		literalMonth = true;
		if (window.currentLanguageId === 1) {
			if (!(new RegExp("[A-Z][A-Z][A-Z]/[0-9][0-9]?/[0-9][0-9][0-9][0-9]")).test(date))
				return 0;
		} else {
			if (!(new RegExp("[0-9][0-9]?/[A-Z][A-Z][A-Z]/[0-9][0-9][0-9][0-9]")).test(date))
				return 0;
		}
	}
	date = date.split("/", 3);
	if (window.currentLanguageId === 1) {
		if (literalMonth) {
			date[0] = monthsToInt[date[0]];
			if (!date[0])
				return 0;
		}
		month = parseInt(date[0]);
		day = parseInt(date[1]);
	} else {
		if (literalMonth) {
			date[1] = monthsToInt[date[1]];
			if (!date[1])
				return 0;
		}
		day = parseInt(date[0]);
		month = parseInt(date[1]);
	}
	if (date.length !== 3 ||
		isNaN(day) ||
		isNaN(month) ||
		isNaN(year = parseInt(date[2])) ||
		day <= 0 ||
		day > 31 ||
		month <= 0 ||
		month > 12 ||
		year < 1900 ||
		year > 2200)
		return 0;
	if ((month == 2 && day > 29) ||
		((month == 4 ||
			month == 6 ||
			month == 9 ||
			month == 11) && day > 30))
		return 0;
	//SQL server rounds to the following day when using 999 millis
	//return (year * 67108864) + (month * 4194304) + (day * 131072) + (setAtEndOfDay ? 98043 : 0);
	//return (setAtEndOfDay ? new Date(year, month - 1, day, 23, 59, 59, 0) : new Date(year, month - 1, day, 0, 0, 0, 0)).getTime();
	return (setAtEndOfDay ? Date.UTC(year, month - 1, day, 23, 59, 59, 0) : Date.UTC(year, month - 1, day, 0, 0, 0, 0));
	//Date.UTC(year, month[, day[, hour[, minute[, second[, millisecond]]]]])
	//returns the number of milliseconds in a Date object since January 1, 1970, 00:00:00, universal time
	//Date.UTC() uses universal time instead of the local time.
	//Date.UTC() returns a time value as a number instead of creating a Date object.
	//return date;
};
window.parseNoNaN = function (str) {
	var x = parseInt(trim(str));
	return (isNaN(x) ? 0 : x);
};
window.parseHour = function (str) {
	if (!str || str.length < 5 || str.charAt(2) != ':')
		return -1;
	var h = parseInt(str.substr(0, 2)), m = parseInt(str.substr(3, 2));
	return ((isNaN(h) || isNaN(m) || h < 0 || h > 23 || m < 0 || m > 59) ? -1 : ((h << 6) | m));
};
window.trim = (function () {
	if (window.String && window.String.prototype && window.String.prototype.trim)
		return function (str) { return str.trim(); };
	var expr = /^\s+|\s+$/g;
	return function (str) { return str.replace(expr, ""); };
})();
window.trimValue = function (input) {
	return trim(_(input).value);
};
window.endsWith = function (str, end) {
	// Try to simulate the actual behavior of endsWith()
	if (str === "")
		return (end === "");
	if (!str)
		return false;
	if (end === "")
		return true;
	if (!end || end.length > str.length)
		return false;
	var i = str.lastIndexOf(end);
	return (i >= 0 && i === (str.length - end.length));
};
window.resetForm = function (f) {
	var $form = $(f), i, validator;
	if (!$form || !$form.length)
		return;
	for (i = $form.length - 1; i >= 0; i--)
		$form[i].reset();
	$form.find("label.error").remove();
	$form.find(".error").removeClass("error");
	$form.find(".valid").removeClass("valid");
	validator = $form.validate();
	if (validator) {
		validator.resetForm();
		validator.formSubmitted = false;
	}
};
window.createItem = function (parent, icon, className, text, badge, clickHandler, name0, value0) {
	var i, btn = document.createElement("button"), c = (className || "btn-outline btn-default");
	btn.setAttribute("type", "button");
	for (i = 6; i < arguments.length; i += 2)
		btn.setAttribute(arguments[i], arguments[i + 1]);
	if (icon) {
		i = document.createElement("i");
		i.className = "fa fa-nomargin " + icon;
		btn.appendChild(i);
	}
	if (badge) {
		btn.appendChild(document.createTextNode(text + " "));
		i = document.createElement("span");
		i.className = "badge";
		i.textContent = badge;
		btn.appendChild(i);
	} else {
		btn.appendChild(document.createTextNode(text));
	}
	if (clickHandler)
		btn.onclick = clickHandler;
	btn.className = c + (icon ? " btn btn-block btn-social" : " btn btn-block");
	if (parent)
		parent.appendChild(btn);
	return btn;
};
window.createRoundLink = function (parent, icon, style, href, name0, value0) {
	var i, btn = document.createElement("a");
	btn.setAttribute("href", href);
	for (i = 4; i < arguments.length; i += 2)
		btn.setAttribute(arguments[i], arguments[i + 1]);
	btn.className = "btn " + style;// + " btn-circle";
	i = document.createElement("i");
	i.className = "fa fa-nomargin " + icon;
	btn.appendChild(i);
	if (parent)
		parent.appendChild(btn);
	return btn;
};
window.createRoundButton = function (parent, icon, style, clickHandler, name0, value0) {
	var i, btn = document.createElement("button");
	btn.setAttribute("type", "button");
	for (i = 4; i < arguments.length; i += 2)
		btn.setAttribute(arguments[i], arguments[i + 1]);
	btn.className = "btn " + style;// + " btn-circle";
	i = document.createElement("i");
	i.className = "fa fa-nomargin " + icon;
	btn.appendChild(i);
	if (clickHandler)
		btn.onclick = clickHandler;
	if (parent)
		parent.appendChild(btn);
	return btn;
};
window.cleanupFile = function (name) {
	var n = (name || "").toUpperCase(), i;
	if ((i = n.lastIndexOf("/")) >= 0)
		n = n.substr(i + 1);
	if ((i = n.lastIndexOf("\\")) >= 0)
		n = n.substr(i + 1);
	if ((i = n.lastIndexOf(".")) >= 0)
		n = n.substr(0, i);
	return n;
};
(function () {
	var fullScreenFrame = null;

	window.openFullScreenFrame = function (url) {
		if (fullScreenFrame)
			return;
		fullScreenFrame = document.createElement("iframe");
		fullScreenFrame.className = "fullscreen-iframe";
		fullScreenFrame.setAttribute("src", url);
		document.body.appendChild(fullScreenFrame);
		document.body.style.overflow = "hidden";
		var tmp = fullScreenFrame;
		setTimeout(function () { tmp.className = "fullscreen-iframe fullscreen-iframe-visible"; }, 10);
		setTimeout(function () { if (tmp === fullScreenFrame) _("wrapper").style.display = "none"; }, 410);
	}

	window.closeFullScreenFrame = function () {
		if (!fullScreenFrame)
			return;
		var tmp = fullScreenFrame, b = null;
		fullScreenFrame = null;
		if (tmp.contentWindow)
			b = tmp.contentWindow.document.getElementsByTagName("body");
		else if (tmp.contentDocument)
			b = tmp.contentDocument.getElementsByTagName("body");
		if (b && b.length)
			b[0].style.overflow = "hidden";
		_("wrapper").style.display = "";
		document.body.style.overflow = "";
		tmp.className = "fullscreen-iframe";
		setTimeout(function () { document.body.removeChild(tmp); }, 410);
	}

	window.removeFullScreenBackground = function () {
		if (!fullScreenFrame)
			return;
		fullScreenFrame.style.background = "none";
	}
})();
/*!
 JsonWebApi v1.0.0 is distributed under the FreeBSD License

 Copyright (c) 2016, Carlos Rafael Gimenes das Neves
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions are met:

 * Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

 * Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 https://github.com/carlosrafaelgn/JsonWebApi
*/
(function () {
	var buildException = function (xhr, ex) {
		return (ex.message ?
			{ xhr: xhr, success: false, status: -1, value: JsonWebApi.messages.exceptionDescription + ex.message, exceptionType: (ex.name || "Error") } :
			{ xhr: xhr, success: false, status: -1, value: JsonWebApi.messages.exceptionDescription + ex, exceptionType: (ex.name || "Error") });
	},
		buildResponse = function (xhr) {
			try {
				if (xhr.status === 200) {
					return { xhr: xhr, success: true, status: 200, value: JSON.parse(xhr.responseText) };
				} else if (xhr.status > 200 && xhr.status < 299) {
					return { xhr: xhr, success: true, status: xhr.status, value: "" };
				} else {
					// Errors are handled here (299 is a special value treated as error)
					var err = JSON.parse(xhr.responseText);
					if (err && err.ExceptionMessage)
						return { xhr: xhr, success: false, status: (xhr.status === 299 ? 500 : xhr.status), value: err.ExceptionMessage, exceptionType: (err.ExceptionType || "System.Exception") };
					else if (err && err.Message)
						return { xhr: xhr, success: false, status: (xhr.status === 299 ? 500 : xhr.status), value: err.Message, exceptionType: (err.ExceptionType || "System.Exception") };
					else if (err && err.length)
						return { xhr: xhr, success: false, status: (xhr.status === 299 ? 500 : xhr.status), value: err.toString(), exceptionType: (err.ExceptionType || "System.Exception") };
					else
						return { xhr: xhr, success: false, status: (xhr.status === 299 ? 500 : xhr.status), value: JsonWebApi.messages.networkError + xhr.status, exceptionType: "System.Exception" };
				}
			} catch (ex) {
				if (ex.name === "SyntaxError")
					return { xhr: xhr, success: false, status: -1, value: xhr.responseText, exceptionType: "SyntaxError" };
				return buildException(xhr, ex);
			}
		},
		buildFullUrl = function (url, args, start) {
			var name, value, i, j, fullUrl = url + "?";
			for (i = start; i < args.length; i += 2) {
				name = args[i];
				value = args[i + 1];

				if (!name && name !== 0)
					throw JsonWebApi.messages.invalidParameterName;
				name = encodeURIComponent(name) + "=";

				// Completely skip the parameter
				if (value === undefined || value === null)
					continue;

				if (value.constructor === Array) {
					if (!value.length) {
						// Completely skip the parameter, because if "name=" is sent, ASP.NET
						// will deserialize it as an array with 1 element containing default(type)
						continue;
					} else {
						if (i !== start)
							fullUrl += "&";

						fullUrl += name + encodeURIComponent((value[0] === undefined || value[0] === null) ? "" : value[0]);
						for (j = 1; j < value.length; j++)
							fullUrl += "&" + name + encodeURIComponent((value[j] === undefined || value[j] === null) ? "" : value[j]);
						continue;
					}
				} else {
					switch ((typeof value)) {
						case "function":
							throw JsonWebApi.messages.parameterValueCannotBeFunction;
						case "object":
							throw JsonWebApi.messages.parameterValueCannotBeObject;
					}
				}

				if (i !== start)
					fullUrl += "&";

				fullUrl += name + encodeURIComponent(value);
			}
			return fullUrl;
		},
		sendRequest = function (async, method, url, callback, bodyObject) {
			var done = false, xhr;

			JsonWebApi.active++;

			try {
				xhr = new XMLHttpRequest();

				xhr.open(method, url, async);

				if (JsonWebApi.avoidCache) {
					xhr.setRequestHeader("Cache-Control", "no-cache, no-store");
					xhr.setRequestHeader("Pragma", "no-cache");
				}

				xhr.setRequestHeader("Accept", "application/json");

				if (async) {
					xhr.onreadystatechange = function () {
						if (xhr.readyState === 4 && !done) {
							done = true;
							JsonWebApi.active--;
							callback(buildResponse(xhr));
						}
					}
				}

				xhr.setRequestHeader("Content-type", "application/json; charset=utf-8");
				if (bodyObject !== undefined)
					xhr.send(JSON.stringify(bodyObject));
				else
					xhr.send();

				if (async)
					return true;

				return buildResponse(xhr);
			} catch (ex) {
				if (!async)
					return buildException(xhr, ex);

				done = true;
				JsonWebApi.active--;
				callback(buildException(xhr, ex));
				return false;
			} finally {
				if (!async)
					JsonWebApi.active--;
			}
		};
	window.JsonWebApi = {
		messages: {
			invalidURL: "URL inválido",
			invalidCallback: "Callback inválido",
			invalidBodyObject: "Objeto do corpo da requisição inválido",
			invalidArguments: "Argumentos inválidos",
			invalidArgumentCount: "Quantidade de argumentos inválidos",
			invalidParameterName: "Nome do parâmetro inválido",
			parameterValueCannotBeObject: "O valor de um parâmetro não podem ser um objeto",
			parameterValueCannotBeFunction: "O valor de um parâmetro não podem ser uma função",
			exceptionDescription: "Ocorreu o seguinte erro: ",
			networkError: "Ocorreu um erro de rede: "
		},
		active: 0,
		avoidCache: true,
		redirect: function (url, name0, value0) {
			if (!url)
				throw JsonWebApi.messages.invalidURL;

			if (!(arguments.length & 1))
				throw JsonWebApi.messages.invalidArgumentCount;

			window.location.href = ((arguments.length > 1) ? buildFullUrl(url, arguments, 1) : url);

			return true;
		},
		getSync: function (url, name0, value0) {
			if (!url)
				throw JsonWebApi.messages.invalidURL;

			if (!(arguments.length & 1))
				throw JsonWebApi.messages.invalidArgumentCount;

			return sendRequest(false, "get", (arguments.length > 1) ? buildFullUrl(url, arguments, 1) : url, null);
		},
		get: function (url, callback, name0, value0) {
			if (!url)
				throw JsonWebApi.messages.invalidURL;

			if (!callback)
				throw JsonWebApi.messages.invalidCallback;

			if ((arguments.length & 1))
				throw JsonWebApi.messages.invalidArgumentCount;

			return sendRequest(true, "get", (arguments.length > 2) ? buildFullUrl(url, arguments, 2) : url, callback);
		},
		postSync: function (url, bodyObject, name0, value0) {
			if (!url)
				throw JsonWebApi.messages.invalidURL;

			if (bodyObject === undefined)
				throw JsonWebApi.messages.invalidBodyObject

			if ((arguments.length & 1))
				throw JsonWebApi.messages.invalidArgumentCount;

			return sendRequest(false, "post", (arguments.length > 2) ? buildFullUrl(url, arguments, 2) : url, null, bodyObject);
		},
		post: function (url, bodyObject, callback, name0, value0) {
			if (!url)
				throw JsonWebApi.messages.invalidURL;

			if (bodyObject === undefined)
				throw JsonWebApi.messages.invalidBodyObject;

			if (!callback)
				throw JsonWebApi.messages.invalidCallback;

			if (!(arguments.length & 1))
				throw JsonWebApi.messages.invalidArgumentCount;

			return sendRequest(true, "post", (arguments.length > 3) ? buildFullUrl(url, arguments, 3) : url, callback, bodyObject);
		},
		postForm: function (url, callback, name0, value0) {
			//
			//Para usar isso:
			//
			//http://stackoverflow.com/questions/11593595/is-there-a-way-to-handle-form-post-data-in-a-web-api-controller
			//
			//
			if (!url || !url.length) {
				throw "URL inválido";
			}

			if (!callback) {
				throw "Callback inválido";
			}

			if ((arguments.length & 1) || arguments.length < 4) {
				throw "Quantidade de argumentos inválidos";
			}

			var i, name, value, req, done, formData = new FormData();

			JsonWebApi.active++;

			try {
				//Preenche todos os campos do formulário
				for (i = 2; i < arguments.length; i += 2) {
					if (!arguments[i] || !arguments[i + 1]) {
						throw "Argumentos inválidos";
					}
					name = arguments[i].toString();
					value = arguments[i + 1].toString();
					if (!name || !name.length) {
						throw "Argumentos inválidos";
					}
					formData.append(name, value);
				}

				// Cria uma requisição AJAX
				req = new XMLHttpRequest();

				// Abre a requisição com o método HTTP POST

				// *** A requisição está sendo aberta em modo assíncrono!
				req.open("post", url, true);

				// Configura a requisição para enviar dados JSON através do corpo
				// da mensagem (por onde será enviado o objeto pessoa)
				req.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

				// Pede para o servidor devolver dados em JSON
				req.setRequestHeader("Accept", "application/json");

				// Configura o callback assíncrono
				req.onreadystatechange = function () {
					if (req.readyState === 4 && !done) {
						done = true;
						JsonWebApi.active--;
						callback(buildResponse(req));
					}
				}

				// Envia a requisição assincronamente
				req.send(formData);
			} catch(ex) {
				done = true;
				JsonWebApi.active--;
				callback(buildException(xhr, ex));
			}
		},
		postFormData: function (url, formData, callback) {
			//
			//Para usar isso:
			//
			//http://stackoverflow.com/questions/11593595/is-there-a-way-to-handle-form-post-data-in-a-web-api-controller
			//
			//
			if (!url || !url.length) {
				throw "URL inválido";
			}

			if (!formData) {
				throw "Formulário inválido";
			}

			if (!callback) {
				throw "Callback inválido";
			}

			var req, done;

			JsonWebApi.active++;

			try {
				// Cria uma requisição AJAX
				req = new XMLHttpRequest();

				// Abre a requisição com o método HTTP POST

				// *** A requisição está sendo aberta em modo assíncrono!
				req.open("post", url, true);

				// Configura a requisição para enviar dados JSON através do corpo
				// da mensagem (por onde será enviado o objeto pessoa)
				if (window.$ && formData.constructor != FormData)
					req.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

				// Pede para o servidor devolver dados em JSON
				req.setRequestHeader("Accept", "application/json");

				// Configura o callback assíncrono
				req.onreadystatechange = function () {
					if (req.readyState === 4 && !done) {
						done = true;
						JsonWebApi.active--;
						callback(buildResponse(req));
					}
				}

				// Envia a requisição assincronamente
				req.send((window.$ && formData.constructor != FormData) ? $(formData).serialize() : formData);
			} catch (ex) {
				done = true;
				JsonWebApi.active--;
				callback(buildException(xhr, ex));
			}
		}
	};
})();
/* https://developer.mozilla.org/en-US/docs/Web/API/Document/cookie
:: cookies.js ::

A complete cookies reader/writer framework with full unicode support.

Revision #1 - September 4, 2014

https://developer.mozilla.org/en-US/docs/Web/API/document.cookie
https://developer.mozilla.org/User:fusionchess

This framework is released under the GNU Public License, version 3 or later.
http://www.gnu.org/licenses/gpl-3.0-standalone.html

Modified by Carlos Rafael Gimenes das Neves
*/
window.Cookies = {
	create: function (name, value, expires, path, domain, secure) {
		if (!name || /^(?:expires|max\-age|path|domain|secure)$/i.test(name)) return false;
		var exp = "";
		if (expires) {
			switch (expires.constructor) {
				case Number:
					if (expires === Infinity) {
						exp = "; expires=Fri, 31 Dec 9999 23:59:59 GMT";
					} else {
						exp = new Date();
						exp.setTime(exp.getTime() + (expires * 60 * 60 * 1000));
						exp = "; expires=" + exp.toUTCString();
					}
					break;
				case Date:
					exp = "; expires=" + expires.toUTCString();
					break;
				case String:
					exp = "; expires=" + expires;
					break;
			}
		}
		document.cookie = encodeURIComponent(name) + "=" + encodeURIComponent(value) + exp + (path ? "; path=" + path : "") + (domain ? "; domain=" + domain : "") + (secure ? "; secure" : "");
		return true;
	},
	get: function (name) {
		return (!name ? null : (decodeURIComponent(document.cookie.replace(new RegExp("(?:(?:^|.*;)\\s*" + encodeURIComponent(name).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*([^;]*).*$)|^.*$"), "$1")) || null));
	},
	remove: function (name, path, domain) {
		if (!Cookies.exists(name)) return false;
		document.cookie = encodeURIComponent(name) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (path ? "; path=" + path : "") + (domain ? "; domain=" + domain : "");
		return true;
	},
	exists: function (name) {
		if (!name) return false;
		return (new RegExp("(?:^|;\\s*)" + encodeURIComponent(name).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie);
	},
	names: function () {
		var ns = document.cookie.replace(/((?:^|\s*;)[^\=]+)(?=;|$)|^\s*|\s*(?:\=[^;]*)?(?:\1|$)/g, "").split(/\s*(?:\=[^;]*)?;\s*/);
		for (var len = ns.length, idx = 0; idx < len; idx++) ns[idx] = decodeURIComponent(ns[idx]);
		return ns;
	}
};
window.Notification = {
	div: null,
	span: null,
	btn: null,
	version: 0,
	timeout: 0,
	timeoutVisible: 0,
	timeoutGone: 0,
	isVisible: false,
	pathBase: "",
	wait: function (msg) {
		var div = document.createElement("div");
		div.innerHTML = "<img alt=\"Aguarde\" src=\"" + (Notification.pathBase || "/") + "images/loading-grey-t.gif\"> " + (msg || "Por favor, aguarde...");
		return Notification.show(div, "default", -1);
	},
	success: function (message, important) {
		return Notification.show(message, "success", important ? 5000 : 2500, true);
	},
	error: function (message, important) {
		return Notification.show(message, "danger", important ? 5000 : 2500, true);
	},
	default: function (message, important) {
		return Notification.show(message, "default", important ? 5000 : 2500, true);
	},
	warning: function (message, important) {
		return Notification.show(message, "warning", important ? 5000 : 2500, true);
	},
	show: function (message, type, timeout, closeable) {
		if (!Notification.div) {
			Notification.div = document.createElement("div");
			Notification.div.setAttribute("role", "alert");
			Notification.div.className = "alert alert-notification";
			Notification.span = document.createElement("span");
			Notification.btn = document.createElement("button");
			Notification.btn.setAttribute("aria-label", "Fechar");
			Notification.btn.innerHTML = "&times;";
			Notification.btn.onclick = Notification.hide;
			Notification.div.appendChild(Notification.span);
			Notification.div.appendChild(Notification.btn);
			document.body.appendChild(Notification.div);
		}

		Notification.isVisible = true;
		Notification.version++;

		var version = Notification.version;

		if (Notification.timeout) {
			clearTimeout(Notification.timeout);
			Notification.timeout = 0;
		}

		if (Notification.timeoutVisible) {
			clearTimeout(Notification.timeoutVisible);
			Notification.timeoutVisible = 0;
		}

		if (Notification.timeoutGone) {
			clearTimeout(Notification.timeoutGone);
			Notification.timeoutGone = 0;
		}

		if (timeout !== -1) {
			if (isNaN(timeout) || timeout <= 0)
				closeable = true;
			else
				Notification.timeout = setTimeout(function () {
					if (Notification.version !== version)
						return;
					Notification.hide();
				}, timeout + 30);
		}

		if (type !== "success" && type !== "info" && type !== "danger" && type !== "warning")
			type = "default";

		Notification.btn.className = (closeable ? "close" : "close hidden");
		Notification.div.className = "alert alert-notification alert-" + type + (closeable ? " alert-dismissible" : "");
		Notification.timeoutVisible = setTimeout(function () {
			if (Notification.version !== version)
				return;

			if ((typeof message) === "string") {
				Notification.span.textContent = message;
			} else {
				while (Notification.span.firstChild)
					Notification.span.removeChild(Notification.span.firstChild);
				Notification.span.appendChild(message);
			}

			$(Notification.div).addClass("alert-notification-shown");
		}, 30);
	},
	hide: function () {
		if (!Notification.div || !Notification.isVisible)
			return;

		Notification.isVisible = false;
		Notification.version++;

		var version = Notification.version;

		if (Notification.timeout) {
			clearTimeout(Notification.timeout);
			Notification.timeout = 0;
		}

		if (Notification.timeoutVisible) {
			clearTimeout(Notification.timeoutVisible);
			Notification.timeoutVisible = 0;
		}

		if (Notification.timeoutGone) {
			clearTimeout(Notification.timeoutGone);
			Notification.timeoutGone = 0;
		}

		$(Notification.div).removeClass("alert-notification-shown");
		Notification.timeoutGone = setTimeout(function () {
			if (Notification.version !== version)
				return;
			$(Notification.div).addClass("alert-notification-gone");
		}, 600);
	}
};
window.BlobDownloader = {
	blobURL: null,

	saveAs: (window.saveAs || window.webkitSaveAs || window.mozSaveAs || window.msSaveAs || window.navigator.saveBlob || window.navigator.webkitSaveBlob || window.navigator.mozSaveBlob || window.navigator.msSaveBlob),

	supported: (("Blob" in window) && ("URL" in window) && ("createObjectURL" in window.URL) && ("revokeObjectURL" in window.URL)),

	alertNotSupported: function () {
		Notification.error("Infelizmente seu navegador não suporta essa funcionalidade \uD83D\uDE22", true);
		return false;
	},

	download: function (filename, blob) {
		if (!BlobDownloader.supported)
			return false;
		if (BlobDownloader.blobURL) {
			URL.revokeObjectURL(BlobDownloader.blobURL);
			BlobDownloader.blobURL = null;
		}

		if (BlobDownloader.saveAs) {
			try {
				BlobDownloader.saveAs.call(window.navigator, blob, filename);
				return;
			} catch (ex) {
				Notification.error("Ocorreu um erro durante o download dos dados \uD83D\uDE22", true);
			}
		}

		var a = document.createElement("a"), evt;
		BlobDownloader.blobURL = URL.createObjectURL(blob);
		a.href = BlobDownloader.blobURL;
		a.download = filename;
		if (document.createEvent && (window.MouseEvent || window.MouseEvents)) {
			try {
				evt = document.createEvent("MouseEvents");
				evt.initMouseEvent("click", true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
				a.dispatchEvent(evt);
				return;
			} catch (ex) {
			}
		}
		a.click(); // Works on Chrome but not on Firefox...
		return true;
	}
};
// Search for selects
(function () {
	var regSlash = /[\/\\]/g, regTrim = /^\s+|\s+$/g, regA = /[ÁÀÃÂÄ]/g, regE = /[ÉÈÊË]/g, regI = /[ÍÌÎ]/g, regO = /[ÓÒÕÔ]/g, regU = /[ÚÙÛ]/g, regC = /[Ç]/g;

	function cbSearch_SetValue(select, value) {
		select.value = value;
		if ("createEvent" in document) {
			var evt = document.createEvent("HTMLEvents");
			evt.initEvent("change", false, true);
			select.dispatchEvent(evt);
		} else {
			select.fireEvent("onchange");
		}
	}

	function cbSearch_Normalize(x) {
		return x.toUpperCase().replace(regSlash, " ").replace(regTrim, "").replace(regA, "A").replace(regE, "E").replace(regI, "I").replace(regO, "O").replace(regU, "U").replace(regC, "C");
	}

	function cbSearch_Change() {
		var i, opt = this.selectedOptions, v;
		if (opt) {
			opt = opt[0];
			this.cbSearchInput.value = ((opt && opt.value && parseInt(opt.value)) ? opt.textContent : "");
		} else {
			opt = this.options;
			v = this.value;
			if (v && parseInt(v)) {
				for (i = opt.length - 1; i >= 0; i--) {
					if (opt[i].value == v) {
						this.cbSearchInput.value = opt[i].textContent;
						return;
					}
				}
			}
			this.cbSearchInput.value = "";
		}
	}

	function cbSearch_MouseDown(e) {
		if (e.button)
			return;
		if (e.offsetX < 38) { //(this.offsetWidth - 25)) {
			this.cbSearchFocusByMouse = false;
			this.cbSearchInput.focus();
			if (this.cbSearchInput.setSelectionRange)
				this.cbSearchInput.setSelectionRange(0, this.cbSearchInput.value.length);
			else
				this.cbSearchInput.select();
			return cancelEvent(e);
		} else {
			this.cbSearchFocusByMouse = true;
		}
	}

	function cbSearch_Blur() {
		if (this.cbSearchSelect) {
			var data = this.cbSearchData;
			if (!data)
				return;

			if (data.timerID)
				clearTimeout(data.timerID);

			if (data.dropDown.className != "dropdown") {
				data.version++;
				data.timerID = setTimeout(cbSearch_BlurTimeout, 300, { data: data, version: data.version });
			}

			if (!this.value)
				cbSearch_SetValue(this.cbSearchSelect, this.cbSearchSelect.options[0].value);
			else
				cbSearch_Change.apply(this.cbSearchSelect);
		} else if (this.cbSearchInput) {
			$(this.cbSearchInput).removeClass("forced-focus");
		}
	}

	function cbSearch_BlurTimeout(x) {
		if (!x || !x.data || x.data.version !== x.version)
			return;
		x.data.close();
		//if (x.data.onblur)
		//	x.data.onblur.call(x.data.elemento);
		cbSearch_Change.apply(x.data.cbSearchSelect);
	}

	function cbSearch_Focus() {
		if (this.cbSearchInput) {
			$(this.cbSearchInput).addClass("forced-focus");
			if (this.cbSearchFocusByMouse)
				this.cbSearchFocusByMouse = false;
			else
				this.cbSearchInput.focus();
		}
	}

	function cbSearch_AClick(e) {
		var data = this.cbSearchData;
		if (data) {
			data.close();
			cbSearch_SetValue(data.cbSearchSelect, this.parentNode.cbSearchValue);
		}
		return cancelEvent(e);
	}

	function cbSearch_KeyDown(e) {
		var data = this.cbSearchData, keyCode;
		if (!data)
			return;

		if ("key" in e) {
			switch (e.key) {
				case "ArrowUp":
					keyCode = 38;
					break;
				case "ArrowDown":
					keyCode = 40;
					break;
				case "ArrowLeft":
					keyCode = 37;
					break;
				case "ArrowRight":
					keyCode = 39;
					break;
				case "Enter":
					keyCode = 13;
					break;
				case "Escape":
					keyCode = 27;
					break;
				case "Shift":
				case "ShiftLeft":
				case "ShiftRight":
					keyCode = 16;
					break;
				case "Control":
				case "ControlLeft":
				case "ControlRight":
					keyCode = 17;
					break;
				case "Tab":
					keyCode = 9;
					break;
				default:
					keyCode = 0;
					break;
			}
		} else if ("keyCode" in e) {
			keyCode = e.keyCode;
		} else {
			keyCode = e.which;
		}

		switch (keyCode) {
			case 9: // tab
			case 16: // shift
			case 17: // ctrl
			case 37: // left
			case 39: // right
				return true;
			case 38: // up
				if (e.preventDefault)
					e.preventDefault();
				if (data.dropDown.className != "dropdown") {
					data.selection--;
					data.updateSelection();
				} else {
					data.open(data.cbSearchInput.value);
				}
				return false;
			case 40: // down
				if (e.preventDefault)
					e.preventDefault();
				if (data.dropDown.className != "dropdown") {
					data.selection++;
					data.updateSelection();
				} else {
					data.open(data.cbSearchInput.value);
				}
				return false;
			case 13: // enter
				if (e.preventDefault)
					e.preventDefault();
				if (data.dropDown.className == "dropdown")
					data.open(data.cbSearchInput.value);
				return false;
			case 27: // escape
				if (data.dropDown.className != "dropdown") {
					data.close();
					return cancelEvent(e);
				}
				break;
		}
		return true;
	}

	function cbSearch_KeyUp(e) {
		var data = this.cbSearchData, keyCode;
		if (!data)
			return;

		if ("key" in e) {
			switch (e.key) {
				case "ArrowUp":
					keyCode = 38;
					break;
				case "ArrowDown":
					keyCode = 40;
					break;
				case "ArrowLeft":
					keyCode = 37;
					break;
				case "ArrowRight":
					keyCode = 39;
					break;
				case "Enter":
					keyCode = 13;
					break;
				case "Escape":
					keyCode = 27;
					break;
				case "Shift":
				case "ShiftLeft":
				case "ShiftRight":
					keyCode = 16;
					break;
				case "Control":
				case "ControlLeft":
				case "ControlRight":
					keyCode = 17;
					break;
				case "Tab":
					keyCode = 9;
					break;
				default:
					keyCode = 0;
					break;
			}
		} else if ("keyCode" in e) {
			keyCode = e.keyCode;
		} else {
			keyCode = e.which;
		}

		switch (keyCode) {
			case 9: // tab
			case 16: // shift
			case 17: // ctrl
			case 37: // left
			case 39: // right
				return true;
			case 38: // up
			case 40: // down
				if (e.preventDefault)
					e.preventDefault();
				if (data.dropDown.className != "dropdown")
					return false;
				data.lastSearch = null;
				break;
			case 13: // enter
				if (e.preventDefault)
					e.preventDefault();
				if (data.dropDown.className != "dropdown") {
					data.select();
					return false;
				}
				data.lastSearch = null;
				break;
			//case 27: // escape
			//	return cancelEvent(e);
		}

		var normalized = cbSearch_Normalize(this.value);
		if (data.lastSearch == normalized)
			return;

		data.lastSearch = normalized;

		if (normalized)
			data.open(normalized);
		else
			data.close();

		return true;
	}

	function cbSearch_DataSelect() {
		if (!this.menu || !this.menu.childNodes.length)
			return;
		if (this.selection < 0)
			this.selection = 0;
		else if (this.selection >= this.menu.childNodes.length)
			this.selection = this.menu.childNodes.length - 1;
		var li = this.menu.childNodes[this.selection];
		this.close();
		cbSearch_SetValue(this.cbSearchSelect, li.cbSearchValue);
	}

	function cbSearch_DataUpdateSelection() {
		if (!this.menu || !this.menu.childNodes.length)
			return;
		if (this.selection < 0)
			this.selection = 0;
		else if (this.selection >= this.menu.childNodes.length)
			this.selection = this.menu.childNodes.length - 1;
		var i, c;
		for (i = this.menu.childNodes.length - 1; i >= 0; i--)
			this.menu.childNodes[i].style.background = "";
		c = this.menu.childNodes[this.selection];
		this.menu.scrollTop = c.offsetTop - 5;
		c.style.background = "rgba(102,175,233,.75)";
	}

	function cbSearch_DataOpen(normalized) {
		var i, li, a, ok = false, cbSearchSelect = this.cbSearchSelect, list = cbSearchSelect.getElementsByTagName("OPTION"), menu = this.menu, txt, norm, value = null;

		while (this.menu.firstChild)
			this.menu.removeChild(this.menu.firstChild);

		for (i = 0; i < list.length; i++) {
			li = list[i];
			if (!(value = li.value) || !parseInt(value))
				continue;
			txt = li.textContent;
			norm = li.cbSearchNormalized;
			if (!norm) {
				norm = cbSearch_Normalize(txt);
				li.cbSearchNormalized = norm;
			}
			if (!normalized || norm.indexOf(normalized) >= 0) {
				li = document.createElement("li");
				if (value)
					li.cbSearchValue = value;
				if (!ok)
					li.style.background = "rgba(102,175,233,.75)";
				a = document.createElement("a");
				a.setAttribute("href", "#");
				a.cbSearchData = this;
				a.onclick = cbSearch_AClick;
				a.appendChild(document.createTextNode(txt));
				li.appendChild(a);
				menu.appendChild(li);
				ok = true;
			}
		}
		menu.scrollTop = 0;

		this.version++;

		if (this.timerID) {
			clearTimeout(this.timerID);
			this.timerID = null;
		}

		this.selection = 0;

		if (ok)
			this.dropDown.className = "dropdown open";
		else
			this.dropDown.className = "dropdown";
	}

	function cbSearch_DataClose() {
		while (this.menu.firstChild)
			this.menu.removeChild(this.menu.firstChild);
		this.version++;
		if (this.timerID) {
			clearTimeout(this.timerID);
			this.timerID = null;
		}
		this.lastSearch = null;
		this.selection = -1;
		this.dropDown.className = "dropdown";
	}

	window.setCbSearch = function (select, value) {
		if (!select)
			return;
		select.value = value;
		if (("createEvent" in document)) {
			var e = document.createEvent("HTMLEvents");
			e.initEvent("change", false, true);
			select.dispatchEvent(e);
		} else if (("fireEvent" in select)) {
			select.fireEvent("onchange");
		} else if (select.onchange) {
			select.onchange();
		}
		if (select.cbSearchChange)
			select.cbSearchChange();
	};

	window.prepareCbSearch = function (select) {
		if (!select)
			return;

		var parent = select.parentNode,
			outerdiv = document.createElement("div"),
			groupdiv = document.createElement("div"),
			span = document.createElement("span"),
			button = document.createElement("button"),
			i = document.createElement("i"),
			input = document.createElement("input"),
			data = {
				cbSearchSelect: select,
				cbSearchInput: input,
				selection: -1,
				lastSearch: null,
				version: 0,
				dropDown: outerdiv,
				menu: document.createElement("ul"),
				select: cbSearch_DataSelect,
				updateSelection: cbSearch_DataUpdateSelection,
				open: cbSearch_DataOpen,
				close: cbSearch_DataClose
			};

		select.cbSearchData = data;
		select.cbSearchInput = input;
		select.onfocus = cbSearch_Focus;
		select.onblur = cbSearch_Blur;
		select.onmousedown = cbSearch_MouseDown;
		select.addEventListener("change", cbSearch_Change);
		select.cbSearchChange = cbSearch_Change;
		outerdiv.className = "dropdown";
		groupdiv.className = "form-group input-group";
		groupdiv.style.position = "absolute";
		groupdiv.style.left = "0";
		groupdiv.style.top = "0";
		groupdiv.style.pointerEvents = "none";
		span.className = "input-group-btn";
		button.className = "btn btn-default btn-force-border";
		button.setAttribute("type", "button");
		button.setAttribute("aria-label", "Pesquisar");
		button.setAttribute("tabindex", "-1");
		button.cbSearchSelect = select;
		i.className = "fa fa-nomargin fa-filter";
		input.className = "form-control upper select-arrow";
		input.setAttribute("type", "text");
		input.setAttribute("spellcheck", "false");
		input.setAttribute("tabindex", "-1");
		// In order to disable address autofill/autocomplete
		// https://stackoverflow.com/a/30976223
		input.setAttribute("autocomplete", "new-password");
		if (select.options[0])
			input.setAttribute("placeholder", select.options[0].textContent);
		input.cbSearchData = data;
		input.cbSearchSelect = select;
		input.onfocus = cbSearch_Focus;
		input.onblur = cbSearch_Blur;
		input.onkeydown = cbSearch_KeyDown;
		input.onkeyup = cbSearch_KeyUp;
		data.menu.className = "dropdown-menu";
		data.menu.style.maxHeight = "140px";// 10 (padding) + (26 x item count)
		data.menu.style.overflowY = "auto";

		button.appendChild(i);
		span.appendChild(button);
		groupdiv.appendChild(span);
		groupdiv.appendChild(input);

		parent.removeChild(select);
		select.style.borderColor = "transparent";
		select.style.webkitBoxShadow = "none";
		select.style.boxShadow = "none";

		outerdiv.appendChild(select);
		outerdiv.appendChild(groupdiv);
		outerdiv.appendChild(data.menu);

		parent.appendChild(outerdiv);

		if (select.value && parseInt(select.value))
			cbSearch_Change.apply(select);
	};

	window.prepareCascadeCbSearch = function (select, nextSelect, emptyValue, autoSetSingleValue, valueGetter, textGetter, optsCallback) {
		if (!select)
			return;

		if (!select.cbSearchChange)
			prepareCbSearch(select);

		if (!nextSelect.cbSearchChange)
			prepareCbSearch(nextSelect);

		if (!nextSelect || !valueGetter || !textGetter || !optsCallback)
			return;

		select.addEventListener("change", function () {
			var i, nextValue = emptyValue, opt, opts = optsCallback(select.options.selectedIndex, select.value);
			while (nextSelect.childNodes.length > 1)
				nextSelect.removeChild(nextSelect.childNodes[1]);
			if (opts && opts.length) {
				for (i = 0; i < opts.length; i++) {
					opt = document.createElement("option");
					opt.setAttribute("value", valueGetter(opts[i]));
					opt.textContent = textGetter(opts[i]);
					nextSelect.appendChild(opt);
				}
				if (autoSetSingleValue && opts.length === 1)
					nextValue = valueGetter(opts[0]);
			}
			setCbSearch(nextSelect, nextValue);
		});
	};
})();
