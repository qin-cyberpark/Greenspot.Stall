/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId])
/******/ 			return installedModules[moduleId].exports;
/******/
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			exports: {},
/******/ 			id: moduleId,
/******/ 			loaded: false
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.loaded = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/*!******************!*\
  !*** multi main ***!
  \******************/
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(/*! ./Content/middle-js/app.js */2);
	module.exports = __webpack_require__(/*! ./Content/less/customer.less */3);


/***/ },
/* 1 */,
/* 2 */
/*!**********************************!*\
  !*** ./Content/middle-js/app.js ***!
  \**********************************/
/***/ function(module, exports) {

	'use strict';
	
	(function () {
	    'use strict';
	
	    angular.module('greenspotStall', ['ngMaterial', 'ngMessages']).config(['$locationProvider', '$mdThemingProvider', '$mdIconProvider', function config($locationProvider, $mdThemingProvider, $mdIconProvider) {
	        //theme
	        $mdThemingProvider.theme('default').primaryPalette('cyan', { 'default': '700' }).accentPalette('orange', { 'default': '800' }).backgroundPalette('grey', { 'default': '100' });
	
	        //
	        $mdIconProvider.defaultIconSet('/static/img/icon/core-icons.svg', 24);
	
	        //location
	        $locationProvider.html5Mode(true);
	    }]).service('CommonService', ['$rootScope', function ($rootScope) {
	        var self = this;
	
	        /* redirect */
	        $rootScope.gotoUrl = function (url) {
	            self.showLoading();
	            window.location.href = url;
	        };
	
	        $rootScope.loadingCircle = false;
	        self.showLoading = function () {
	            $rootScope.loadingCircle = true;
	        };
	
	        self.hideLoading = function () {
	            $rootScope.loadingCircle = false;
	        };
	
	        self.isLoading = function () {
	            return $rootScope.loadingCircle;
	        };
	    }]).filter("trust", ['$sce', function ($sce) {
	        return function (htmlCode) {
	            return $sce.trustAsHtml(htmlCode);
	        };
	    }]);
	})();

/***/ },
/* 3 */
/*!************************************!*\
  !*** ./Content/less/customer.less ***!
  \************************************/
/***/ function(module, exports, __webpack_require__) {

	// style-loader: Adds some css to the DOM by adding a <style> tag
	
	// load the styles
	var content = __webpack_require__(/*! !./../../~/css-loader!./../../~/less-loader!./customer.less */ 4);
	if(typeof content === 'string') content = [[module.id, content, '']];
	// add the styles to the DOM
	var update = __webpack_require__(/*! ./../../~/style-loader/addStyles.js */ 6)(content, {});
	if(content.locals) module.exports = content.locals;
	// Hot Module Replacement
	if(false) {
		// When the styles change, update the <style> tags
		if(!content.locals) {
			module.hot.accept("!!./../../node_modules/css-loader/index.js!./../../node_modules/less-loader/index.js!./customer.less", function() {
				var newContent = require("!!./../../node_modules/css-loader/index.js!./../../node_modules/less-loader/index.js!./customer.less");
				if(typeof newContent === 'string') newContent = [[module.id, newContent, '']];
				update(newContent);
			});
		}
		// When the module is disposed, remove the <style> tags
		module.hot.dispose(function() { update(); });
	}

/***/ },
/* 4 */
/*!*******************************************************************!*\
  !*** ./~/css-loader!./~/less-loader!./Content/less/customer.less ***!
  \*******************************************************************/
/***/ function(module, exports, __webpack_require__) {

	exports = module.exports = __webpack_require__(/*! ./../../~/css-loader/lib/css-base.js */ 5)();
	// imports
	
	
	// module
	exports.push([module.id, "/* Stall Home Page */\n#stall-header {\n  padding: 10px;\n  background-color: #0097A7;\n}\n#stall-header > img {\n  display: inline-block;\n  width: 40%;\n}\n#stall-header .info {\n  float: right;\n  width: 60%;\n  text-align: center;\n}\n#stall-header .info > div {\n  color: #F5F5F5;\n  font-size: 2rem;\n  font-weight: bold;\n}\n#stall-header .info > button {\n  border: none;\n  color: #E0E0E0;\n  background-color: #0097A7;\n  font-size: 1rem;\n}\n/* Product Page*/\n#product-content {\n  padding-bottom: 20px;\n}\n#product-content > img {\n  width: 100%;\n}\n#product-content .line {\n  padding: 5px 5px 10px 10px;\n  display: -webkit-flex;\n  display: flex;\n  -webkit-flex-flow: row;\n  flex-flow: row;\n  align-items: center;\n  -webkit-align-items: center;\n}\n#product-content .line .sel-quantity {\n  font-size: 1.5rem;\n}\n#product-content .line .text-name {\n  font-weight: bold;\n  font-size: 1.5rem;\n}\n#product-content .line .text-price {\n  font-size: 2rem;\n  color: #0097A7;\n}\n#product-content .line .align-left {\n  text-align: left;\n}\n#product-content .line .align-right {\n  text-align: right;\n  flex: auto;\n  -webkit-flex: auto;\n}\n#product-content .line .btn-2line {\n  display: inline-block;\n  text-align: center;\n  width: 30px;\n}\n#product-content .line .btn-2line .caption {\n  line-height: 0.8rem;\n  font-size: 0.8rem;\n}\n/* Cart Page */\n#cart-content {\n  padding: 5px;\n}\n#cart-content md-checkbox {\n  padding: 0px;\n  margin: 0px;\n}\n#cart-content .cart-stall .cart-item {\n  vertical-align: middle;\n  padding-left: 30px;\n  height: 24px;\n}\n#cart-content .cart-stall .cart-item .item-price {\n  width: 40px;\n}\n#cart-content .cart-stall .cart-item .qty-lbl {\n  padding: 0px;\n}\n#cart-content .cart-stall .cart-item .md-button {\n  margin: 5px;\n  padding: 0px;\n  width: 24px;\n  min-width: 24px;\n}\n#cart-content .cart-stall .cart-item .md-icon {\n  width: 20px;\n  height: 20px;\n}\n#checkout-content {\n  padding: 0px 15px 0px 15px;\n}\n#checkout-content .checkout-section {\n  font-size: 0.9rem;\n  padding: 0px 0px 10px 0px;\n  border-bottom: 1px dotted black;\n}\n#checkout-content .checkout-section:nth-child(n+2) {\n  margin-top: 5px;\n}\n#checkout-content .checkout-section .caption {\n  font-weight: bold;\n  margin-top: 5px;\n  margin-bottom: 3px;\n  font-size: 1.2rem;\n}\n#checkout-content .checkout-section .message {\n  color: #EF6C00;\n}\n#checkout-content .checkout-section .pickup-address {\n  line-height: 12px;\n}\n#checkout-content .checkout-section .delivery-address {\n  margin-top: 0px;\n  margin-bottom: 0px;\n}\n#checkout-content .checkout-section .delivery-address .delivery-address-name {\n  display: block;\n  font-weight: bold;\n}\n#checkout-content .checkout-section .checkout-item {\n  padding: 0px;\n  margin-top: 5px;\n  vertical-align: middle;\n}\n#checkout-content .checkout-section .checkout-item .item-price-qty {\n  text-align: right;\n}\n#checkout-content .checkout-section .checkout-note textarea {\n  width: 100%;\n  height: 60px;\n}\n#checkout-content .checkout-section .switcher {\n  display: inline-block;\n  border: 1px solid #E0E0E0;\n}\n#checkout-content .checkout-section .switcher .switcher-item {\n  padding: 2px 8px 2px 8px;\n  display: inline-block;\n}\n#checkout-content .checkout-section .switcher .switcher-item.switch-on {\n  color: white;\n  background-color: #0097A7;\n}\n#checkout-content .out-of-stock-error {\n  width: 100%;\n  text-align: center;\n}\n#checkout-content .out-of-stock-error .product-name {\n  color: #EF6C00;\n}\n#checkout-content .out-of-stock-error .stock {\n  color: #EF6C00;\n}\n#order-content {\n  font-size: 0.9rem;\n}\n#order-content table {\n  width: 100%;\n}\n#order-content table thead {\n  font-weight: bold;\n}\n#formNewAddress input {\n  font-size: 11px;\n}\n.AddrAutoCompleteBox {\n  list-style: none outside;\n  border: 1px solid black;\n  margin: 0px;\n  padding: 0px;\n  background-color: #eee;\n  z-index: 999;\n}\n", ""]);
	
	// exports


/***/ },
/* 5 */
/*!**************************************!*\
  !*** ./~/css-loader/lib/css-base.js ***!
  \**************************************/
/***/ function(module, exports) {

	"use strict";
	
	/*
		MIT License http://www.opensource.org/licenses/mit-license.php
		Author Tobias Koppers @sokra
	*/
	// css base code, injected by the css-loader
	module.exports = function () {
		var list = [];
	
		// return the list of modules as css string
		list.toString = function toString() {
			var result = [];
			for (var i = 0; i < this.length; i++) {
				var item = this[i];
				if (item[2]) {
					result.push("@media " + item[2] + "{" + item[1] + "}");
				} else {
					result.push(item[1]);
				}
			}
			return result.join("");
		};
	
		// import a list of modules into the list
		list.i = function (modules, mediaQuery) {
			if (typeof modules === "string") modules = [[null, modules, ""]];
			var alreadyImportedModules = {};
			for (var i = 0; i < this.length; i++) {
				var id = this[i][0];
				if (typeof id === "number") alreadyImportedModules[id] = true;
			}
			for (i = 0; i < modules.length; i++) {
				var item = modules[i];
				// skip already imported module
				// this implementation is not 100% perfect for weird media query combinations
				//  when a module is imported multiple times with different media queries.
				//  I hope this will never occur (Hey this way we have smaller bundles)
				if (typeof item[0] !== "number" || !alreadyImportedModules[item[0]]) {
					if (mediaQuery && !item[2]) {
						item[2] = mediaQuery;
					} else if (mediaQuery) {
						item[2] = "(" + item[2] + ") and (" + mediaQuery + ")";
					}
					list.push(item);
				}
			}
		};
		return list;
	};

/***/ },
/* 6 */
/*!*************************************!*\
  !*** ./~/style-loader/addStyles.js ***!
  \*************************************/
/***/ function(module, exports, __webpack_require__) {

	/*
		MIT License http://www.opensource.org/licenses/mit-license.php
		Author Tobias Koppers @sokra
	*/
	var stylesInDom = {},
		memoize = function(fn) {
			var memo;
			return function () {
				if (typeof memo === "undefined") memo = fn.apply(this, arguments);
				return memo;
			};
		},
		isOldIE = memoize(function() {
			return /msie [6-9]\b/.test(window.navigator.userAgent.toLowerCase());
		}),
		getHeadElement = memoize(function () {
			return document.head || document.getElementsByTagName("head")[0];
		}),
		singletonElement = null,
		singletonCounter = 0,
		styleElementsInsertedAtTop = [];
	
	module.exports = function(list, options) {
		if(true) {
			if(typeof document !== "object") throw new Error("The style-loader cannot be used in a non-browser environment");
		}
	
		options = options || {};
		// Force single-tag solution on IE6-9, which has a hard limit on the # of <style>
		// tags it will allow on a page
		if (typeof options.singleton === "undefined") options.singleton = isOldIE();
	
		// By default, add <style> tags to the bottom of <head>.
		if (typeof options.insertAt === "undefined") options.insertAt = "bottom";
	
		var styles = listToStyles(list);
		addStylesToDom(styles, options);
	
		return function update(newList) {
			var mayRemove = [];
			for(var i = 0; i < styles.length; i++) {
				var item = styles[i];
				var domStyle = stylesInDom[item.id];
				domStyle.refs--;
				mayRemove.push(domStyle);
			}
			if(newList) {
				var newStyles = listToStyles(newList);
				addStylesToDom(newStyles, options);
			}
			for(var i = 0; i < mayRemove.length; i++) {
				var domStyle = mayRemove[i];
				if(domStyle.refs === 0) {
					for(var j = 0; j < domStyle.parts.length; j++)
						domStyle.parts[j]();
					delete stylesInDom[domStyle.id];
				}
			}
		};
	}
	
	function addStylesToDom(styles, options) {
		for(var i = 0; i < styles.length; i++) {
			var item = styles[i];
			var domStyle = stylesInDom[item.id];
			if(domStyle) {
				domStyle.refs++;
				for(var j = 0; j < domStyle.parts.length; j++) {
					domStyle.parts[j](item.parts[j]);
				}
				for(; j < item.parts.length; j++) {
					domStyle.parts.push(addStyle(item.parts[j], options));
				}
			} else {
				var parts = [];
				for(var j = 0; j < item.parts.length; j++) {
					parts.push(addStyle(item.parts[j], options));
				}
				stylesInDom[item.id] = {id: item.id, refs: 1, parts: parts};
			}
		}
	}
	
	function listToStyles(list) {
		var styles = [];
		var newStyles = {};
		for(var i = 0; i < list.length; i++) {
			var item = list[i];
			var id = item[0];
			var css = item[1];
			var media = item[2];
			var sourceMap = item[3];
			var part = {css: css, media: media, sourceMap: sourceMap};
			if(!newStyles[id])
				styles.push(newStyles[id] = {id: id, parts: [part]});
			else
				newStyles[id].parts.push(part);
		}
		return styles;
	}
	
	function insertStyleElement(options, styleElement) {
		var head = getHeadElement();
		var lastStyleElementInsertedAtTop = styleElementsInsertedAtTop[styleElementsInsertedAtTop.length - 1];
		if (options.insertAt === "top") {
			if(!lastStyleElementInsertedAtTop) {
				head.insertBefore(styleElement, head.firstChild);
			} else if(lastStyleElementInsertedAtTop.nextSibling) {
				head.insertBefore(styleElement, lastStyleElementInsertedAtTop.nextSibling);
			} else {
				head.appendChild(styleElement);
			}
			styleElementsInsertedAtTop.push(styleElement);
		} else if (options.insertAt === "bottom") {
			head.appendChild(styleElement);
		} else {
			throw new Error("Invalid value for parameter 'insertAt'. Must be 'top' or 'bottom'.");
		}
	}
	
	function removeStyleElement(styleElement) {
		styleElement.parentNode.removeChild(styleElement);
		var idx = styleElementsInsertedAtTop.indexOf(styleElement);
		if(idx >= 0) {
			styleElementsInsertedAtTop.splice(idx, 1);
		}
	}
	
	function createStyleElement(options) {
		var styleElement = document.createElement("style");
		styleElement.type = "text/css";
		insertStyleElement(options, styleElement);
		return styleElement;
	}
	
	function createLinkElement(options) {
		var linkElement = document.createElement("link");
		linkElement.rel = "stylesheet";
		insertStyleElement(options, linkElement);
		return linkElement;
	}
	
	function addStyle(obj, options) {
		var styleElement, update, remove;
	
		if (options.singleton) {
			var styleIndex = singletonCounter++;
			styleElement = singletonElement || (singletonElement = createStyleElement(options));
			update = applyToSingletonTag.bind(null, styleElement, styleIndex, false);
			remove = applyToSingletonTag.bind(null, styleElement, styleIndex, true);
		} else if(obj.sourceMap &&
			typeof URL === "function" &&
			typeof URL.createObjectURL === "function" &&
			typeof URL.revokeObjectURL === "function" &&
			typeof Blob === "function" &&
			typeof btoa === "function") {
			styleElement = createLinkElement(options);
			update = updateLink.bind(null, styleElement);
			remove = function() {
				removeStyleElement(styleElement);
				if(styleElement.href)
					URL.revokeObjectURL(styleElement.href);
			};
		} else {
			styleElement = createStyleElement(options);
			update = applyToTag.bind(null, styleElement);
			remove = function() {
				removeStyleElement(styleElement);
			};
		}
	
		update(obj);
	
		return function updateStyle(newObj) {
			if(newObj) {
				if(newObj.css === obj.css && newObj.media === obj.media && newObj.sourceMap === obj.sourceMap)
					return;
				update(obj = newObj);
			} else {
				remove();
			}
		};
	}
	
	var replaceText = (function () {
		var textStore = [];
	
		return function (index, replacement) {
			textStore[index] = replacement;
			return textStore.filter(Boolean).join('\n');
		};
	})();
	
	function applyToSingletonTag(styleElement, index, remove, obj) {
		var css = remove ? "" : obj.css;
	
		if (styleElement.styleSheet) {
			styleElement.styleSheet.cssText = replaceText(index, css);
		} else {
			var cssNode = document.createTextNode(css);
			var childNodes = styleElement.childNodes;
			if (childNodes[index]) styleElement.removeChild(childNodes[index]);
			if (childNodes.length) {
				styleElement.insertBefore(cssNode, childNodes[index]);
			} else {
				styleElement.appendChild(cssNode);
			}
		}
	}
	
	function applyToTag(styleElement, obj) {
		var css = obj.css;
		var media = obj.media;
	
		if(media) {
			styleElement.setAttribute("media", media)
		}
	
		if(styleElement.styleSheet) {
			styleElement.styleSheet.cssText = css;
		} else {
			while(styleElement.firstChild) {
				styleElement.removeChild(styleElement.firstChild);
			}
			styleElement.appendChild(document.createTextNode(css));
		}
	}
	
	function updateLink(linkElement, obj) {
		var css = obj.css;
		var sourceMap = obj.sourceMap;
	
		if(sourceMap) {
			// http://stackoverflow.com/a/26603875
			css += "\n/*# sourceMappingURL=data:application/json;base64," + btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap)))) + " */";
		}
	
		var blob = new Blob([css], { type: "text/css" });
	
		var oldSrc = linkElement.href;
	
		linkElement.href = URL.createObjectURL(blob);
	
		if(oldSrc)
			URL.revokeObjectURL(oldSrc);
	}


/***/ }
/******/ ]);
//# sourceMappingURL=bundle.js.map