var Greenspot = Greenspot || {};
(function () {
    Greenspot.Utilities = Greenspot.Utilities || {};
    Greenspot.Utilities.SearchResult = function (page, pageSize) {
        var self = this;
        //page
        self.page = page || 0,
        //pageSize
        self.pageSize = pageSize || 10,
        //items
        self.items = [],
        //has more records
        self.hasMore = false;
        //has record
        self.hasRecord = false;
        //append
        self.append = function (data) {
            if (data && data.length) {
                self.items = self.items.concat(data);
            }
            self.hasMore = data && data.length == self.pageSize;
            self.hasRecord = data && data.length;
        }
    }

    //parse
    Greenspot.Utilities.SearchResult.Parse = function (jsonStr) {
        var obj = JSON.parse(jsonStr);
        var newResult = new Greenspot.Utilities.SearchResult(obj.page, obj.pageSize);
        newResult.items = obj.items;
        newResult.hasMore = obj.hasMore;
        return newResult;
    }
})();