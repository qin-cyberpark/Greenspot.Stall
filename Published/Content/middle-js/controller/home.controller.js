﻿(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('HomeController', ['$window', '$scope', '$location', '$http', 'CommonService',
        function ($window, $scope, $location, $http, comSrv) {
            var vm = this;

            //loading
            vm.isLoading = function () {
                return comSrv.isLoading();
            }

            //search condition
            vm.searchCondition = {
                isRecommend: true,
                category: "T",
                area: "nz:auckland",
                keyword: "",
                init: function () {
                    var params = $location.search();
                    this.category = 'T';
                    if (params.c === 'H') {
                        this.category = 'H';
                    }
                    this.isRecommend = params.r != 'false';
                    this.area = params.a || this.area;
                    this.keyword = params.k || this.keyword;
                },
                queryString: function () {
                    return '?c=' + this.category + '&r=' + this.isRecommend + '&a=' + this.area + '&k=' + this.keyword;
                },
                queryObject: function () {
                    return { 'r': this.isRecommend, 'c': this.category, 'a': this.area, 'k': this.keyword };
                }
            }

            //parse the url

            //init 
            vm.init_cart = function () {
                //init cart
                vm.cart = new Greenspot.Cart();
                vm.cart.load();
            }

            //T - takeaway, H - homemade
            vm.init = function () {
                //init condition
                vm.searchCondition.init();
                if (vm.searchCondition.isRecommend) {
                    //load recommend
                    vm.loadRecommendStalls();
                } else {
                    //reload
                    if (!vm.restoreSearchResult()) {
                        //research
                        vm.search();
                    }
                }

                //init cart
                vm.init_cart();
            }

            //load recommend
            vm.loadRecommendStalls = function () {
                comSrv.showLoading();
                vm.recommendStalls = new Greenspot.Utilities.SearchResult(0, 20);

                //require url
                var url = '/api/stall/recommend?c=' + vm.searchCondition.category + '&a=' + vm.searchCondition.area;

                //load
                $http.get(url).success(function (result) {
                    if (result.Succeeded) {
                        vm.recommendStalls.append(result.Data);
                    }
                    else {
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error)
                }).finally(function () {
                    comSrv.hideLoading();
                });
            }


            /*
            === search ===
            */
            //search product
            vm.searchProduct = function (page, pageSize) {
                page = page || 0;
                pageSize = pageSize || 10;
                if (page == 0) {
                    vm.matchedProducts = new Greenspot.Utilities.SearchResult(page, pageSize);
                } else {
                    vm.matchedProducts.page = page;
                    vm.matchedProducts.pageSize = pageSize;
                }

                $http.get('/api/product/search' + vm.searchCondition.queryString() + '&p=' + page + '&ps=' + pageSize).success(function (result) {
                    if (result.Succeeded) {
                        vm.matchedProducts.append(result.Data);
                        //store result
                        if ($window.localStorage && vm.matchedProducts) {
                            $window.localStorage.setItem("matched_products", JSON.stringify(vm.matchedProducts));
                        }
                    }
                    else {
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error)
                }).finally(function () {
                    comSrv.hideLoading();
                });
            }

            //load more product
            vm.loadMoreProduct = function () {
                comSrv.showLoading();
                vm.searchProduct(vm.matchedProducts.page + 1, vm.matchedProducts.pageSize);
            }

            //search stall
            vm.searchStall = function (page, pageSize) {
                page = page || 0;
                pageSize = pageSize || 10;

                if (page == 0) {
                    vm.matchedStalls = new Greenspot.Utilities.SearchResult(page, pageSize);
                } else {
                    vm.matchedStalls.page = page;
                    vm.matchedStalls.pageSize = pageSize;
                }

                //search stall
                $http.get('/api/stall/search' + vm.searchCondition.queryString() + '&p=' + page + '&ps=' + pageSize).success(function (result) {
                    if (result.Succeeded) {

                        vm.matchedStalls.append(result.Data);

                        //store result
                        if ($window.localStorage && vm.matchedStalls) {
                            $window.localStorage.setItem("matched_stalls", JSON.stringify(vm.matchedStalls));
                        }
                    }
                    else {
                        console.log(result.Message);
                    }
                }).error(function (error) {
                    console.log(error)
                }).finally(function () {
                    comSrv.hideLoading();
                });
            }

            //load more stall
            vm.loadMoreStall = function () {
                comSrv.showLoading();
                vm.searchStall(vm.matchedStalls.page + 1, vm.matchedStalls.pageSize);
            }

            //search all
            vm.search = function (stallPageSize, productPageSize) {
                comSrv.showLoading();
                vm.searchCondition.isRecommend = false;
                vm.recommendStalls = null;

                //store search condition
                if ($window.localStorage) {
                    $window.localStorage.setItem("pre_condition", JSON.stringify(vm.searchCondition));
                }

                //
                vm.searchProduct(0, productPageSize);
                vm.searchStall(0, stallPageSize);

                $location.search(vm.searchCondition.queryObject());

            }

            //
            vm.gotoStall = function (stallId) {
                $window.localStorage.removeItem("pre_stall");
                $scope.gotoUrl('/stall/' + stallId);
            }

            //restore result
            vm.restoreSearchResult = function () {
                if ($window.localStorage && $window.localStorage["pre_condition"]) {
                    var storedCondition = JSON.parse($window.localStorage["pre_condition"]);
                    if (storedCondition.category != vm.searchCondition.category
                        || storedCondition.area != vm.searchCondition.area
                        || storedCondition.keyword != vm.searchCondition.keyword) {
                        //condition not match
                        return false;
                    }

                    if ($window.localStorage["matched_stalls"]) {
                        vm.matchedStalls = Greenspot.Utilities.SearchResult.Parse($window.localStorage["matched_stalls"]);
                    }

                    if ($window.localStorage["matched_products"]) {
                        vm.matchedProducts = Greenspot.Utilities.SearchResult.Parse($window.localStorage["matched_products"]);
                    }
                }

                return (vm.matchedStalls || false && vm.matchedStalls.length) || (vm.matchedProducts || false && vm.matchedProducts.length);
            }
        }]);
})();