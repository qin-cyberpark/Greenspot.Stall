(function () {
    'use strict';
    var module = angular.module('greenspotStall');
    module.controller('HomeController', ['$window', '$location', '$http', 'CommonService',
        function ($window, $location, $http, comSrv) {
            var vm = this;

            //search condition
            vm.searchCondition = {
                isRecommend: true,
                category: "T",
                area: "NZ-Auckland",
                keyword: "",
                init: function (category) {
                    this.category = category;
                    var params = $location.search();
                    this.isRecommend = params.r != 'false';
                    this.area = params.a || this.area;
                    this.keyword = params.k || this.keyword;
                },
                queryString: function () {
                    return '?r=' + this.isRecommend + '&c=' + this.category + '&a=' + this.area + '&k=' + this.keyword;
                }
            }

            //parse the url

            //init 
            vm.init_cart = function () {
                //init cart
                vm.cart = new Greenspot.Cart();
                vm.cart.loadFromCookie();
            }

            //T - takeaway, H - homemade
            vm.init = function (category) {

                //init condition
                vm.searchCondition.init(category);
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
                vm.recommendStalls = null;

                //require url
                var url = '/api/home/recommend?c=' + vm.searchCondition.category + '&a=' + vm.searchCondition.area;

                //load
                $http.get(url).success(function (result) {
                    if (result.Succeeded) {
                        vm.recommendStalls = result.Data;
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
            //vm.redirectSearchPage = function () {
            //    //$location.url('/takeaway/index.html?' + vm.searchCondition.queryString);
            //    $window.location = '/takeaway/index.html?' + vm.searchCondition.queryString;
            //}

            vm.search = function () {
                comSrv.showLoading();
                vm.searchCondition.isRecommend = false;
                vm.recommendStalls = null;
                vm.matchedStalls = null;
                vm.matchedProducts = null;

                //var condition = 'r=false&c=' + vm.searchCondition.category + '&a=' + vm.searchCondition.area + '&k=' + vm.searchCondition.keyword;

                //store search condition
                if ($window.localStorage) {
                    $window.localStorage.setItem("pre_condition", JSON.stringify(vm.searchCondition));
                }

                $location.url('/takeaway/index.html' + vm.searchCondition.queryString());

                //search stall
                $http.get('/api/home/SearchStall' + vm.searchCondition.queryString()).success(function (result) {
                    if (result.Succeeded) {
                        vm.matchedStalls = result.Data;

                        //store result
                        if (vm.matchedStalls.length && $window.localStorage) {
                            $window.localStorage.setItem("matched_stalls", JSON.stringify(vm.matchedStalls))
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

                //search product
                $http.get('/api/home/SearchProduct' + vm.searchCondition.queryString()).success(function (result) {
                    if (result.Succeeded) {
                        vm.matchedProducts = result.Data;

                        //store result
                        if (vm.matchedProducts.length && $window.localStorage) {
                            $window.localStorage.setItem("matched_products", JSON.stringify(vm.matchedProducts))
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

            //
            vm.restoreSearchResult = function () {
                if ($window.localStorage) {
                    var storedCondition = JSON.parse($window.localStorage["pre_condition"]);
                    if (storedCondition.category != vm.searchCondition.category
                        || storedCondition.area != vm.searchCondition.area
                        || storedCondition.keyword != vm.searchCondition.keyword) {
                        //condition not match
                        return false;
                    }
                    vm.matchedStalls = JSON.parse($window.localStorage["matched_stalls"]);
                    vm.matchedProducts = JSON.parse($window.localStorage["matched_products"]);
                }

                return (vm.matchedStalls || false && vm.matchedStalls.length) || (vm.matchedProducts || false && vm.matchedProducts.length);
            }
        }]);
})();