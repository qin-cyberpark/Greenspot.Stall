"use strict";
var path = require("path");

module.exports = {
    entry: [
        "./Content/middle-js/app.js",
        "./Content/less/customer.less"
    ],
    output: {
        filename: "./static/bundle.js"
    },
    devServer: {
        contentBase: ".",
        host: "localhost",
        port: 9000
    },
    module: {
        loaders: [
            {
                test: /\.jsx?$/,
                loader: "babel-loader"
            },
            {
                test: /\.less$/,
                loader: "style!css!less"
            }
        ]
    }
};